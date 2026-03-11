using Business.Abstract;
using Core.Entities;
using DataAccess.Context;

namespace Business.Concrete
{
    public class MembershipManager : IMembershipService
    {
        private readonly BepopDbContext _context;

        public MembershipManager(BepopDbContext context)
        {
            _context = context;
        }

        public List<Product> GetActivePackages()
        {
            return _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Level)
                .ToList();
        }

        public Product? GetByLevel(int level)
        {
            return _context.Products
                .FirstOrDefault(p => p.Level == level && p.IsActive);
        }

        public bool UpgradeUser(int userId, int newLevel)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return false;

            var package = GetByLevel(newLevel);
            if (package == null) return false;

            user.MembershipLevel = newLevel;
            _context.SaveChanges();
            return true;
        }
    }
}