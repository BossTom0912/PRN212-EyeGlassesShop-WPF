using DAL.Implementations;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services
{
    public class AdminUserService
    {
        private readonly IAccountRepository _accountRepository;

        public AdminUserService()
        {
            _accountRepository = new AccountRepository();
        }

        public List<Account> GetAllUsers()
        {
            return _accountRepository.GetAllAccounts();
        }

        public void ToggleUserStatus(long accountId)
        {
            var account = _accountRepository.GetAccountById(accountId);
            if (account == null)
                throw new Exception("Không tìm thấy tài khoản.");

            account.IsActive = !account.IsActive;
            account.UpdatedAt = DateTime.UtcNow;

            _accountRepository.UpdateAccount(account);
        }
    }
}