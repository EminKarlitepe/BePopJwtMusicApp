using Business.Abstract;
using Business.Concrete;
using Business.DTOs;
using Business.Validators;
using Core.Entities;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Business
{
    public static class BusinessServiceRegistration
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Servisler
            services.AddScoped<ISongService, SongManager>();
            services.AddScoped<IUserService, UserManager>();
            services.AddScoped<IPlaylistService, PlaylistManager>();
            services.AddScoped<IArtistService, ArtistManager>();
            services.AddScoped<IAlbumService, AlbumManager>();
            services.AddScoped<IGenreService, GenreManager>();
            services.AddScoped<IMembershipService, MembershipManager>();

            // FluentValidation
            services.AddScoped<IValidator<UserRegisterDto>, UserRegisterValidator>();
            services.AddScoped<IValidator<UserLoginDto>, UserLoginValidator>();
            services.AddScoped<IValidator<Song>, SongValidator>();
            services.AddScoped<IValidator<Artist>, ArtistValidator>();
            services.AddScoped<IValidator<Album>, AlbumValidator>();
            services.AddScoped<IValidator<Playlist>, PlaylistValidator>();

            return services;
        }
    }
}