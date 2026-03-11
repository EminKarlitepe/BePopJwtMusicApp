using Core.Entities;
using Business.DTOs;

namespace Business.Abstract
{
    public interface IUserService
    {
        User? GetById(int userId);
        User? GetByEmail(string email);
        bool EmailExists(string email);
        User Register(UserRegisterDto dto);
        UserLoginResultDto Login(UserLoginDto dto);
        void UpdateProfile(int userId, UserProfileDto dto);
        void UpdateMembershipLevel(int userId, int newLevel);
        int GetMembershipLevel(int userId);
        List<string> GetRoles(int userId);
    }
}