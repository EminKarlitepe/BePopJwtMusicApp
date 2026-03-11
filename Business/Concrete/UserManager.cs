using Business.Abstract;
using Business.DTOs;
using Core.Entities;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly BepopDbContext _context;

        public UserManager(BepopDbContext context)
        {
            _context = context;
        }

        public User? GetById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public User? GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public bool EmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public User Register(UserRegisterDto dto)
        {
            if (EmailExists(dto.Email))
                throw new InvalidOperationException("Bu email zaten kayıtlı.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                MembershipLevel = dto.MembershipLevel > 0 ? dto.MembershipLevel : 1,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public UserLoginResultDto Login(UserLoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return new UserLoginResultDto { Success = false, ErrorMessage = "Hatalı email veya şifre." };

            var roles = GetRoles(user.UserId);

            return new UserLoginResultDto
            {
                Success = true,
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                MembershipLevel = user.MembershipLevel,
                Roles = roles
            };
        }

        public void UpdateProfile(int userId, UserProfileDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return;

            user.DisplayName = dto.DisplayName;
            user.Bio = dto.Bio;
            user.Country = dto.Country;
            user.DateOfBirth = dto.DateOfBirth;
            user.ProfileImageUrl = dto.ProfileImageUrl;
            _context.SaveChanges();
        }

        public void UpdateMembershipLevel(int userId, int newLevel)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return;
            user.MembershipLevel = newLevel;
            _context.SaveChanges();
        }

        public int GetMembershipLevel(int userId)
        {
            return _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.MembershipLevel)
                .FirstOrDefault();
        }

        public List<string> GetRoles(int userId)
        {
            return _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => r.Name)
                .ToList();
        }
    }
}