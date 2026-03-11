using Business.DTOs;
using Core.Entities;
using FluentValidation;

namespace Business.Validators
{
    public class UserRegisterValidator : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
                .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalı.")
                .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olabilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalı.");

            RuleFor(x => x.MembershipLevel)
                .InclusiveBetween(1, 3).WithMessage("Geçersiz üyelik seviyesi.");
        }
    }

    public class UserLoginValidator : AbstractValidator<UserLoginDto>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.");
        }
    }

    public class SongValidator : AbstractValidator<Song>
    {
        public SongValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Şarkı adı boş olamaz.")
                .MaximumLength(200).WithMessage("Şarkı adı en fazla 200 karakter olabilir.");

            RuleFor(x => x.FileUrl)
                .NotEmpty().WithMessage("Dosya URL boş olamaz.");

            RuleFor(x => x.Level)
                .InclusiveBetween(1, 3).WithMessage("Geçersiz seviye (1=Ücretsiz, 2=Gold, 3=Premium).");

            RuleFor(x => x.ArtistId)
                .GreaterThan(0).WithMessage("Sanatçı seçilmelidir.");
        }
    }

    public class ArtistValidator : AbstractValidator<Artist>
    {
        public ArtistValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Sanatçı adı boş olamaz.")
                .MaximumLength(100).WithMessage("Sanatçı adı en fazla 100 karakter olabilir.");
        }
    }

    public class AlbumValidator : AbstractValidator<Album>
    {
        public AlbumValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Albüm adı boş olamaz.")
                .MaximumLength(200).WithMessage("Albüm adı en fazla 200 karakter olabilir.");

            RuleFor(x => x.ArtistId)
                .GreaterThan(0).WithMessage("Sanatçı seçilmelidir.");
        }
    }

    public class PlaylistValidator : AbstractValidator<Playlist>
    {
        public PlaylistValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Çalma listesi adı boş olamaz.")
                .MaximumLength(100).WithMessage("Liste adı en fazla 100 karakter olabilir.");
        }
    }
}