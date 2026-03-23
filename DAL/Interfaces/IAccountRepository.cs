using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAccountRepository
    {
        Account GetAccountByEmail(string email);
        void AddAccount(Account account);
        Role GetRoleByName(string roleName);
        void AddCartForAccount(Cart cart);
    }
}
