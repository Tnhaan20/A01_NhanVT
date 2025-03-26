using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS1_BusinessModel;
using AS1_DAO;

namespace AS1_Repository
{
    public interface IAccountRepository
    {
        public SystemAccount GetAccount(String email, String password);

        public List<SystemAccount> getAllAccounts();
        public SystemAccount GetAccountById(short accountId);
        public SystemAccount GetAccountByEmail(String email);
        public void AddAccount(SystemAccount account);
        public void UpdateAccount(short accId, SystemAccount account);
        public void DeleteAccount(short accountId);
        public List<object> GetSelectAccountList();

    }
}
