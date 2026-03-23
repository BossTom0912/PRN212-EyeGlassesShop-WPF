using DAL.Implementations;
using DAL.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepo;

        public AccountService()
        {
            _accountRepo = new AccountRepository();
        }

        public Account Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var account = _accountRepo.GetAccountByEmail(email);

            if (account != null && account.PasswordHash == password)
            {
                return account;
            }
            return null;
        }

        public bool Register(string fullName, string email, string password, string phone)
        {
            try
            {
                var existingAccount = _accountRepo.GetAccountByEmail(email);
                if (existingAccount != null)
                {
                    throw new Exception("Email này đã được đăng ký!");
                }

                var userRole = _accountRepo.GetRoleByName("User");
                if (userRole == null) throw new Exception("Hệ thống chưa setup Role User.");

                var newAccount = new Account
                {
                    FullName = fullName,
                    Email = email,
                    PasswordHash = password, 
                    Phone = phone,
                    RoleId = userRole.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _accountRepo.AddAccount(newAccount);

                var newCart = new Cart
                {
                    AccountId = newAccount.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _accountRepo.AddCartForAccount(newCart);

                return true;
            }
            catch (Exception ex)
            {
                string errorDetails = ex.Message;
                if (ex.InnerException != null)
                {
                    errorDetails += "\nChi tiết từ DB: " + ex.InnerException.Message;
                }
                throw new Exception(errorDetails);
            }
        }
    }
}
