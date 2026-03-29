using DAL.DBContext;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        public Account? GetAccountByEmail(string email)
        {
            using (var context = new GlassesShopContext())
            {
                return context.Accounts
                              .Include(a => a.Role)
                              .FirstOrDefault(a => a.Email == email);
            }
        }

        public Account? GetAccountById(long id)
        {
            using (var context = new GlassesShopContext())
            {
                return context.Accounts
                              .Include(a => a.Role)
                              .FirstOrDefault(a => a.Id == id);
            }
        }

        public List<Account> GetAllAccounts()
        {
            using (var context = new GlassesShopContext())
            {
                return context.Accounts
                              .Include(a => a.Role)
                              .OrderBy(a => a.Id)
                              .ToList();
            }
        }

        public Role? GetRoleByName(string roleName)
        {
            using (var context = new GlassesShopContext())
            {
                return context.Roles.FirstOrDefault(r => r.Name == roleName);
            }
        }

        public void AddAccount(Account account)
        {
            using (var context = new GlassesShopContext())
            {
                context.Accounts.Add(account);
                context.SaveChanges();
            }
        }

        public void AddCartForAccount(Cart cart)
        {
            using (var context = new GlassesShopContext())
            {
                context.Carts.Add(cart);
                context.SaveChanges();
            }
        }

        public void UpdateAccount(Account account)
        {
            using (var context = new GlassesShopContext())
            {
                context.Accounts.Update(account);
                context.SaveChanges();
            }
        }
    }
}