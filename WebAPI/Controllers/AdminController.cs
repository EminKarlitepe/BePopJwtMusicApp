using Core.Entities;
using DataAccess.Context;
using BepopStreamProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BepopStreamProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly BepopDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(BepopDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult SearchAlbums(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Json(Array.Empty<object>());
            }

            q = q.Trim();

            var albums = _context.Albums
                .Include(a => a.Artist)
                .Where(a => a.Title.Contains(q))
                .OrderBy(a => a.Title)
                .Take(10)
                .Select(a => new
                {
                    a.AlbumId,
                    a.Title,
                    ArtistName = a.Artist != null ? a.Artist.Name : string.Empty
                })
                .ToList();

            return Json(albums);
        }

        [HttpGet]
        public IActionResult SearchArtists(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Json(Array.Empty<object>());
            }

            q = q.Trim();

            var artists = _context.Artists
                .Where(a => a.Name.Contains(q))
                .OrderBy(a => a.Name)
                .Take(10)
                .Select(a => new { a.ArtistId, a.Name })
                .ToList();

            return Json(artists);
        }

        public IActionResult Index()
        {
            ViewBag.UserCount = _context.Users.Count();
            ViewBag.SongCount = _context.Songs.Count();
            ViewBag.ArtistCount = _context.Artists.Count();
            ViewBag.AlbumCount = _context.Albums.Count();
            ViewBag.GenreCount = _context.Genres.Count();
            ViewBag.ChartCount = _context.Charts.Count();
            ViewBag.PlaylistCount = _context.Playlists.Count();
            ViewBag.ProductCount = _context.Products.Count();
            return View();
        }

        public IActionResult Artists()
        {
            var artists = _context.Artists.ToList();
            return View("Artists/Index", artists);
        }

        [HttpGet]
        public IActionResult CreateArtist()
        {
            return View("Artists/Create");
        }

        [HttpPost]
        public IActionResult CreateArtist(Artist artist)
        {
            if (string.IsNullOrWhiteSpace(artist.Name))
            {
                ViewBag.Error = "Sanatçı adı zorunludur.";
                return View("Artists/Create", artist);
            }
            _context.Artists.Add(artist);
            _context.SaveChanges();
            return RedirectToAction("Artists");
        }

        [HttpGet]
        public IActionResult EditArtist(int id)
        {
            var artist = _context.Artists.FirstOrDefault(a => a.ArtistId == id);
            if (artist == null) return NotFound();
            return View("Artists/Edit", artist);
        }

        [HttpPost]
        public IActionResult EditArtist(Artist artist)
        {
            if (string.IsNullOrWhiteSpace(artist.Name))
            {
                ViewBag.Error = "Sanatçı adı zorunludur.";
                return View("Artists/Edit", artist);
            }
            _context.Artists.Update(artist);
            _context.SaveChanges();
            return RedirectToAction("Artists");
        }

        [HttpPost]
        public IActionResult DeleteArtist(int id)
        {
            var artist = _context.Artists.FirstOrDefault(a => a.ArtistId == id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
                _context.SaveChanges();
            }
            return RedirectToAction("Artists");
        }
        public IActionResult Songs(int page = 1)
        {
            const int pageSize = 20;
            var songs = _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SongViewModel
                {
                    SongId = s.SongId,
                    Title = s.Title,
                    ArtistName = s.Artist != null ? s.Artist.Name : "?",
                    FileUrl = s.FileUrl,
                    Level = s.Level,
                    ImageUrl = s.Album != null ? s.Album.CoverImageUrl : null
                })
                .ToList();

            ViewBag.TotalSongs = _context.Songs.Count();
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View("Songs", songs);
        }

        [HttpGet]
        public IActionResult UploadSong()
        {
            var model = new UploadSongViewModel
            {
                Artists = _context.Artists.OrderBy(a => a.Name).ToList(),
                Albums = _context.Albums.Include(a => a.Artist).OrderBy(a => a.Title).ToList(),
                Genres = _context.Genres.OrderBy(g => g.Name).ToList()
            };
            return View("UploadSong", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSong(UploadSongViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Artists = _context.Artists.OrderBy(a => a.Name).ToList();
                model.Albums = _context.Albums.Include(a => a.Artist).OrderBy(a => a.Title).ToList();
                model.Genres = _context.Genres.OrderBy(g => g.Name).ToList();
                return View("UploadSong", model);
            }

            string? fileUrl = null;

            if (model.Mp3File != null && model.Mp3File.Length > 0)
            {
                var ext = Path.GetExtension(model.Mp3File.FileName).ToLower();
                var allowed = new[] { ".mp3", ".wav", ".ogg", ".flac" };
                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("Mp3File", "Sadece MP3, WAV, OGG veya FLAC yükleyebilirsiniz.");
                    model.Artists = _context.Artists.OrderBy(a => a.Name).ToList();
                    model.Albums = _context.Albums.Include(a => a.Artist).OrderBy(a => a.Title).ToList();
                    return View("UploadSong", model);
                }

                var folder = Path.Combine(_env.WebRootPath, "uploads", "songs");
                Directory.CreateDirectory(folder);
                var fileName = $"{Guid.NewGuid()}{ext}";
                using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
                await model.Mp3File.CopyToAsync(stream);
                fileUrl = $"/uploads/songs/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(model.ExternalUrl))
            {
                fileUrl = model.ExternalUrl.Trim();
            }
            else
            {
                ModelState.AddModelError("Mp3File", "Lütfen bir MP3 dosyası yükleyin veya harici URL girin.");
                model.Artists = _context.Artists.OrderBy(a => a.Name).ToList();
                model.Albums = _context.Albums.Include(a => a.Artist).OrderBy(a => a.Title).ToList();
                return View("UploadSong", model);
            }

            if (model.CoverFile != null && model.CoverFile.Length > 0)
            {
                var coverFolder = Path.Combine(_env.WebRootPath, "uploads", "covers");
                Directory.CreateDirectory(coverFolder);
                var coverExt = Path.GetExtension(model.CoverFile.FileName).ToLower();
                var coverName = $"{Guid.NewGuid()}{coverExt}";
                using var cs = new FileStream(Path.Combine(coverFolder, coverName), FileMode.Create);
                await model.CoverFile.CopyToAsync(cs);
            }

            var song = new Song
            {
                Title = model.Title.Trim(),
                ArtistId = model.ArtistId,
                AlbumId = model.AlbumId == 0 ? null : model.AlbumId,
                FileUrl = fileUrl!,
                Level = model.Level,
                CreatedAt = DateTime.Now
            };

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            // Save selected genres
            if (model.GenreIds != null && model.GenreIds.Count > 0)
            {
                foreach (var gid in model.GenreIds.Distinct())
                {
                    _context.SongGenres.Add(new SongGenre { SongId = song.SongId, GenreId = gid });
                }
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = $"'{song.Title}' başarıyla eklendi! (ID: {song.SongId})";
            return RedirectToAction("Songs");
        }

        [HttpGet]
        public IActionResult EditSong(int id)
        {
            var song = _context.Songs
                .Include(s => s.SongGenres)
                .Include(s => s.Artist)
                .FirstOrDefault(s => s.SongId == id);
            if (song == null) return NotFound();
            ViewBag.Artists = _context.Artists.ToList();
            ViewBag.Albums = _context.Albums.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.SelectedGenres = song.SongGenres.Select(sg => sg.GenreId).ToList();
            return View("Songs/Edit", song);
        }

        [HttpPost]
        public IActionResult EditSong(Song song, List<int> genreIds)
        {
            if (string.IsNullOrWhiteSpace(song.Title) || string.IsNullOrWhiteSpace(song.FileUrl))
            {
                ViewBag.Error = "Şarkı adı ve dosya URL zorunludur.";
                ViewBag.Artists = _context.Artists.ToList();
                ViewBag.Albums = _context.Albums.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View("Songs/Edit", song);
            }
            _context.Songs.Update(song);
            var oldGenres = _context.SongGenres.Where(sg => sg.SongId == song.SongId);
            _context.SongGenres.RemoveRange(oldGenres);
            if (genreIds != null && genreIds.Count > 0)
            {
                foreach (var genreId in genreIds.Distinct())
                {
                    _context.SongGenres.Add(new SongGenre { SongId = song.SongId, GenreId = genreId });
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Songs");
        }

        [HttpPost]
        public IActionResult DeleteSong(int id)
        {
            var song = _context.Songs.FirstOrDefault(s => s.SongId == id);
            if (song != null)
            {
                if (!string.IsNullOrEmpty(song.FileUrl) && song.FileUrl.StartsWith("/uploads/"))
                {
                    var fullPath = Path.Combine(_env.WebRootPath,
                        song.FileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                }
                _context.Songs.Remove(song);
                _context.SaveChanges();
            }
            return RedirectToAction("Songs");
        }


        public IActionResult Albums()
        {
            var albums = _context.Albums.Include(a => a.Artist).ToList();
            return View("Albums/Index", albums);
        }

        [HttpGet]
        public IActionResult CreateAlbum()
        {
            return View("Albums/Create");
        }

        [HttpPost]
        public IActionResult CreateAlbum(Album album)
        {
            // Attempt to resolve artist when client only supplied the name (artistName)
            if (album.ArtistId == 0)
            {
                var artistName = Request.Form["artistName"].ToString();
                if (!string.IsNullOrWhiteSpace(artistName))
                {
                    var existing = _context.Artists.FirstOrDefault(a => a.Name.ToLower() == artistName.Trim().ToLower());
                    if (existing != null)
                    {
                        album.ArtistId = existing.ArtistId;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(album.Title))
            {
                ViewBag.Error = "Albüm adı zorunludur.";
                return View("Albums/Create", album);
            }

            if (album.ArtistId == 0 || !_context.Artists.Any(a => a.ArtistId == album.ArtistId))
            {
                ViewBag.Error = "Geçerli bir sanatçı seçmelisiniz.";
                return View("Albums/Create", album);
            }

            _context.Albums.Add(album);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ViewBag.Error = "Albüm kaydedilirken veritabanı hatası oluştu.";
                return View("Albums/Create", album);
            }

            return RedirectToAction("Albums");
        }

        [HttpGet]
        public IActionResult EditAlbum(int id)
        {
            var album = _context.Albums.Include(a => a.Artist).FirstOrDefault(a => a.AlbumId == id);
            if (album == null) return NotFound();
            ViewBag.Artists = _context.Artists.ToList();
            return View("Albums/Edit", album);
        }

        [HttpPost]
        public IActionResult EditAlbum(Album album)
        {
            if (string.IsNullOrWhiteSpace(album.Title))
            {
                ViewBag.Error = "Albüm adı zorunludur.";
                ViewBag.Artists = _context.Artists.ToList();
                return View("Albums/Edit", album);
            }
            _context.Albums.Update(album);
            _context.SaveChanges();
            return RedirectToAction("Albums");
        }

        [HttpPost]
        public IActionResult DeleteAlbum(int id)
        {
            var album = _context.Albums.FirstOrDefault(a => a.AlbumId == id);
            if (album != null)
            {
                _context.Albums.Remove(album);
                _context.SaveChanges();
            }
            return RedirectToAction("Albums");
        }


        public IActionResult Genres()
        {
            var genres = _context.Genres.ToList();
            return View("Genres/Index", genres);
        }

        [HttpGet]
        public IActionResult CreateGenre()
        {
            return View("Genres/Create");
        }

        [HttpPost]
        public IActionResult CreateGenre(Genre genre)
        {
            if (string.IsNullOrWhiteSpace(genre.Name))
            {
                ViewBag.Error = "Tür adı zorunludur.";
                return View("Genres/Create", genre);
            }
            _context.Genres.Add(genre);
            _context.SaveChanges();
            return RedirectToAction("Genres");
        }

        [HttpGet]
        public IActionResult EditGenre(int id)
        {
            var genre = _context.Genres.FirstOrDefault(g => g.GenreId == id);
            if (genre == null) return NotFound();
            return View("Genres/Edit", genre);
        }

        [HttpPost]
        public IActionResult EditGenre(Genre genre)
        {
            if (string.IsNullOrWhiteSpace(genre.Name))
            {
                ViewBag.Error = "Tür adı zorunludur.";
                return View("Genres/Edit", genre);
            }
            _context.Genres.Update(genre);
            _context.SaveChanges();
            return RedirectToAction("Genres");
        }

        [HttpPost]
        public IActionResult DeleteGenre(int id)
        {
            var genre = _context.Genres.FirstOrDefault(g => g.GenreId == id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                _context.SaveChanges();
            }
            return RedirectToAction("Genres");
        }


        public IActionResult Charts()
        {
            var charts = _context.Charts.ToList();
            return View("Charts/Index", charts);
        }

        [HttpGet]
        public IActionResult CreateChart()
        {
            return View("Charts/Create");
        }

        [HttpPost]
        public IActionResult CreateChart(Chart chart)
        {
            if (string.IsNullOrWhiteSpace(chart.Name))
            {
                ViewBag.Error = "Chart adı zorunludur.";
                return View("Charts/Create", chart);
            }
            chart.CreatedAt = DateTime.Now;
            _context.Charts.Add(chart);
            _context.SaveChanges();
            return RedirectToAction("Charts");
        }

        [HttpGet]
        public IActionResult EditChart(int id)
        {
            var chart = _context.Charts.FirstOrDefault(c => c.ChartId == id);
            if (chart == null) return NotFound();
            return View("Charts/Edit", chart);
        }

        [HttpPost]
        public IActionResult EditChart(Chart chart)
        {
            if (string.IsNullOrWhiteSpace(chart.Name))
            {
                ViewBag.Error = "Chart adı zorunludur.";
                return View("Charts/Edit", chart);
            }
            _context.Charts.Update(chart);
            _context.SaveChanges();
            return RedirectToAction("Charts");
        }

        [HttpPost]
        public IActionResult DeleteChart(int id)
        {
            var chart = _context.Charts.FirstOrDefault(c => c.ChartId == id);
            if (chart != null)
            {
                _context.Charts.Remove(chart);
                _context.SaveChanges();
            }
            return RedirectToAction("Charts");
        }

        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View("Users/Index", users);
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();
            ViewBag.Roles = _context.Roles.ToList();
            return View("Users/Edit", user);
        }

        [HttpPost]
        public IActionResult EditUser(User user, int roleId)
        {
            var existing = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existing == null) return NotFound();
            existing.Username = user.Username;
            existing.Email = user.Email;
            existing.MembershipLevel = user.MembershipLevel;
            existing.DisplayName = user.DisplayName;
            existing.Country = user.Country;
            existing.Bio = user.Bio;
            var existingRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == user.UserId);
            if (existingRole != null)
                existingRole.RoleId = roleId;
            else
                _context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = roleId });
            _context.SaveChanges();
            return RedirectToAction("Users");
        }

        public IActionResult Products()
        {
            var products = _context.Products.ToList();
            return View("Products/Index", products);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View("Products/Create");
        }

        [HttpPost]
        public IActionResult CreateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                ViewBag.Error = "Ürün adı zorunludur.";
                ViewBag.Categories = _context.Categories.ToList();
                return View("Products/Create", product);
            }
            product.CreatedAt = DateTime.Now;
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Products");
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();
            ViewBag.Categories = _context.Categories.ToList();
            return View("Products/Edit", product);
        }

        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                ViewBag.Error = "Ürün adı zorunludur.";
                ViewBag.Categories = _context.Categories.ToList();
                return View("Products/Edit", product);
            }
            _context.Products.Update(product);
            _context.SaveChanges();
            return RedirectToAction("Products");
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Products");
        }
    }
}
