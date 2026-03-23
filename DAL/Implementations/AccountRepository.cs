using DAL.DBContext;
using DAL.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        public Account GetAccountByEmail(string email)
        {
            using (var context = new GlassesShopContext())
            {
                return context.Accounts.FirstOrDefault(a => a.Email == email && a.IsActive == true);
            }
        }

        public Role GetRoleByName(string roleName)
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
    }
}
