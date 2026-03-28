using DAL.Models;
using System.Collections.Generic;

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
