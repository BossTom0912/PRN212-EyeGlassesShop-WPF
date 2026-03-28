using DAL.Models;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IAccountRepository
    {
        Account? GetAccountByEmail(string email);
        Account? GetAccountById(long id);
        List<Account> GetAllAccounts();

        Role? GetRoleByName(string roleName);

        void AddAccount(Account account);
        void AddCartForAccount(Cart cart);
        void UpdateAccount(Account account);
    }
}