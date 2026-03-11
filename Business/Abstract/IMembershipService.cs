using Core.Entities;

namespace Business.Abstract
{
    public interface IMembershipService
    {
        List<Product> GetActivePackages();
        Product? GetByLevel(int level);
        bool UpgradeUser(int userId, int newLevel);
    }
}