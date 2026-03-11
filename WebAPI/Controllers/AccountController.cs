using BepopStreamProject.Helpers;
using Core.Entities;
using DataAccess.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BepopStreamProject.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly BepopDbContext _context;

        public AccountController(BepopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var userId = User.GetUserId();
            if (userId <= 0) return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return NotFound();

            var followingArtists = _context.ArtistFollows
                .Include(af => af.Artist)
                .Where(af => af.UserId == userId)
                .Select(af => af.Artist)
                .ToList();

            ViewBag.FollowingArtists = followingArtists;
            ViewBag.FollowingArtistsCount = followingArtists.Count;
            ViewBag.FollowersCount = _context.UserFollows.Count(uf => uf.FollowingId == userId);
            ViewBag.FollowingUsersCount = _context.UserFollows.Count(uf => uf.FollowerId == userId);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(User model)
        {
            var userId = User.GetUserId();
            if (userId <= 0) return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return NotFound();

            user.DisplayName = model.DisplayName;
            user.Bio = model.Bio;
            user.Country = model.Country;
            user.DateOfBirth = model.DateOfBirth;
            user.ProfileImageUrl = model.ProfileImageUrl;
            _context.SaveChanges();

            TempData["ProfileSaved"] = "Profil bilgileriniz güncellendi.";
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public IActionResult SearchUsers(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Json(Array.Empty<object>());

            var myId = User.GetUserId();
            q = q.Trim();

            var users = _context.Users
                .Where(u => u.UserId != myId &&
                            (u.Username.Contains(q) || (u.DisplayName != null && u.DisplayName.Contains(q))))
                .Take(10)
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    DisplayName = u.DisplayName ?? u.Username,
                    u.ProfileImageUrl,
                    u.Country
                })
                .ToList();

            return Json(users);
        }

        [HttpGet]
        public IActionResult UserProfile(int id)
        {
            var myId = User.GetUserId();
            if (id == myId) return RedirectToAction("Profile");

            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();

            var followingArtists = _context.ArtistFollows
                .Include(af => af.Artist)
                .Where(af => af.UserId == id)
                .Select(af => af.Artist)
                .ToList();

            ViewBag.FollowingArtists = followingArtists;
            ViewBag.FollowersCount = _context.UserFollows.Count(uf => uf.FollowingId == id);
            ViewBag.FollowingUsersCount = _context.UserFollows.Count(uf => uf.FollowerId == id);
            ViewBag.IsFollowing = _context.UserFollows.Any(uf => uf.FollowerId == myId && uf.FollowingId == id);
            ViewBag.MyId = myId;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleUserFollow(int targetId)
        {
            var myId = User.GetUserId();
            if (myId <= 0 || myId == targetId) return BadRequest();

            if (!_context.Users.Any(u => u.UserId == targetId)) return NotFound();

            var existing = _context.UserFollows
                .FirstOrDefault(uf => uf.FollowerId == myId && uf.FollowingId == targetId);

            if (existing == null)
                _context.UserFollows.Add(new UserFollow { FollowerId = myId, FollowingId = targetId });
            else
                _context.UserFollows.Remove(existing);

            _context.SaveChanges();
            return RedirectToAction("UserProfile", new { id = targetId });
        }

        [HttpGet]
        public IActionResult Followers(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();

            var myId = User.GetUserId();

            var followingIds = _context.UserFollows
                .Where(x => x.FollowerId == myId)
                .Select(x => x.FollowingId)
                .ToHashSet();

            var items = _context.UserFollows
                .Where(uf => uf.FollowingId == id)
                .Include(uf => uf.Follower)
                .Select(uf => uf.Follower)
                .ToList()
                .Select(u => new UserListItemVm
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    DisplayName = u.DisplayName ?? u.Username,
                    ProfileImageUrl = u.ProfileImageUrl,
                    Country = u.Country,
                    IsFollowingBack = followingIds.Contains(u.UserId),
                    IsMe = u.UserId == myId
                }).ToList();

            ViewBag.TargetUser = user;
            ViewBag.ListType = "followers";
            ViewBag.MyId = myId;
            return View("FollowList", items);
        }

        [HttpGet]
        public IActionResult Following(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();

            var myId = User.GetUserId();

            var followingIds = _context.UserFollows
                .Where(x => x.FollowerId == myId)
                .Select(x => x.FollowingId)
                .ToHashSet();

            var items = _context.UserFollows
                .Where(uf => uf.FollowerId == id)
                .Include(uf => uf.Following)
                .Select(uf => uf.Following)
                .ToList()
                .Select(u => new UserListItemVm
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    DisplayName = u.DisplayName ?? u.Username,
                    ProfileImageUrl = u.ProfileImageUrl,
                    Country = u.Country,
                    IsFollowingBack = followingIds.Contains(u.UserId),
                    IsMe = u.UserId == myId
                }).ToList();

            ViewBag.TargetUser = user;
            ViewBag.ListType = "following";
            ViewBag.MyId = myId;
            return View("FollowList", items);
        }
    }

    public class UserListItemVm
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? ProfileImageUrl { get; set; }
        public string? Country { get; set; }
        public bool IsFollowingBack { get; set; }
        public bool IsMe { get; set; }
    }
}