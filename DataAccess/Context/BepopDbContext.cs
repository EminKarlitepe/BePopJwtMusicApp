using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection.Emit;

namespace DataAccess.Context
{
    public class BepopDbContext : DbContext
    {
        public BepopDbContext(DbContextOptions<BepopDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<ArtistFollow> ArtistFollows { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<SongGenre> SongGenres { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<PlayHistory> PlayHistories { get; set; }
        public DbSet<Chart> Charts { get; set; }
        public DbSet<ChartSong> ChartSongs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFollow>()
                .HasKey(uf => new { uf.FollowerId, uf.FollowingId });

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(uf => uf.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            // ArtistFollow many-to-many
            modelBuilder.Entity<ArtistFollow>()
                .HasKey(af => new { af.UserId, af.ArtistId });

            modelBuilder.Entity<ArtistFollow>()
                .HasOne(af => af.User)
                .WithMany(u => u.FollowingArtists)
                .HasForeignKey(af => af.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArtistFollow>()
                .HasOne(af => af.Artist)
                .WithMany(a => a.Followers)
                .HasForeignKey(af => af.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            // PlaylistSong many-to-many
            modelBuilder.Entity<PlaylistSong>()
                .HasKey(ps => new { ps.PlaylistId, ps.SongId });

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.PlaylistSongs)
                .HasForeignKey(ps => ps.PlaylistId);

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Song)
                .WithMany(s => s.PlaylistSongs)
                .HasForeignKey(ps => ps.SongId);

            // SongGenre many-to-many
            modelBuilder.Entity<SongGenre>()
                .HasKey(sg => new { sg.SongId, sg.GenreId });

            modelBuilder.Entity<SongGenre>()
                .HasOne(sg => sg.Song)
                .WithMany(s => s.SongGenres)
                .HasForeignKey(sg => sg.SongId);

            modelBuilder.Entity<SongGenre>()
                .HasOne(sg => sg.Genre)
                .WithMany(g => g.SongGenres)
                .HasForeignKey(sg => sg.GenreId);

            // ChartSong many-to-many
            modelBuilder.Entity<ChartSong>()
                .HasKey(cs => new { cs.ChartId, cs.SongId });

            modelBuilder.Entity<ChartSong>()
                .HasOne(cs => cs.Chart)
                .WithMany(c => c.ChartSongs)
                .HasForeignKey(cs => cs.ChartId);

            modelBuilder.Entity<ChartSong>()
                .HasOne(cs => cs.Song)
                .WithMany(s => s.ChartSongs)
                .HasForeignKey(cs => cs.SongId);

            // PlayHistory
            modelBuilder.Entity<PlayHistory>()
                .HasOne(ph => ph.User)
                .WithMany(u => u.PlayHistories)
                .HasForeignKey(ph => ph.UserId);

            modelBuilder.Entity<PlayHistory>()
                .HasOne(ph => ph.Song)
                .WithMany(s => s.PlayHistories)
                .HasForeignKey(ph => ph.SongId);

            // Album-Artist relationship
            modelBuilder.Entity<Album>()
                .HasOne(a => a.Artist)
                .WithMany(ar => ar.Albums)
                .HasForeignKey(a => a.ArtistId);

            // Song-Artist relationship
            modelBuilder.Entity<Song>()
                .HasOne(s => s.Artist)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.ArtistId);

            // Song-Album relationship
            modelBuilder.Entity<Song>()
                .HasOne(s => s.Album)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.AlbumId);

            // Playlist-User relationship
            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.User)
                .WithMany(u => u.Playlists)
                .HasForeignKey(p => p.UserId);

            // UserRole join
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });
        }
    }
}
