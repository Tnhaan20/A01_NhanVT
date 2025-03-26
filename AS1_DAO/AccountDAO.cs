using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS1_BusinessModel;
using Microsoft.EntityFrameworkCore;

namespace AS1_DAO
{
    public class AccountDAO
    {
        private FunewsManagementContext _dbcontext;
        private static AccountDAO instance;

        public AccountDAO()
        {
            _dbcontext = new FunewsManagementContext();
        }

        public static AccountDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountDAO();
                }
                return instance;
            }
        }

        public List<SystemAccount> getAllAcounts()
        {
            return _dbcontext.SystemAccounts.ToList();
        }

        public SystemAccount GetAccount(String email, string password)
        {
            return _dbcontext.SystemAccounts.FirstOrDefault(m => m.AccountEmail.Equals(email) && m.AccountPassword.Equals(password));
        }

        public SystemAccount getAccByID(short accId)
        {
            return _dbcontext.SystemAccounts.SingleOrDefault(m => m.AccountId == accId);
        }

        public SystemAccount GetAccount(String accEmail)
        {
            return _dbcontext.SystemAccounts.SingleOrDefault(m => m.AccountEmail.Equals(accEmail));
        }

        public void AddAccount(SystemAccount systemAccount)
        {
            try
            {
                SystemAccount curAccount = GetAccount(systemAccount.AccountEmail);
                if (curAccount == null)
                {
                    short maxId = 0;
                    if (_dbcontext.SystemAccounts.Any())
                    {
                        maxId = _dbcontext.SystemAccounts.Max(a => a.AccountId);
                    }

                    systemAccount.AccountId = (short)(maxId + 1);

                    _dbcontext.SystemAccounts.Add(systemAccount);
                    _dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding account: {ex.Message}", ex);
            }
        }

        public void RemoveAccount(short accId)
        {
            SystemAccount currentSystemAccount = getAccByID((short)accId);
            if (currentSystemAccount != null)
            {
                _dbcontext.SystemAccounts.Remove(currentSystemAccount);
                _dbcontext.SaveChanges();
            }
        }

        public void UpdateAccount(short accId, SystemAccount account)
        {
            try
            {
                var existingSystemAccount = getAccByID((short)accId);
                if (existingSystemAccount == null)
                {
                    throw new Exception($"SystemAccount with ID {accId} not found!");
                }

                // Check if new SystemAccount email already exists for different SystemAccount
                var accountWithSameEmail = GetAccount(account.AccountEmail);
                if (accountWithSameEmail != null && accountWithSameEmail.AccountId != accId)
                {
                    throw new Exception($"SystemAccount with email '{account.AccountEmail}' already exists!");
                }

                existingSystemAccount.AccountName = account.AccountName;
                existingSystemAccount.AccountEmail = account.AccountEmail;
                existingSystemAccount.AccountPassword = account.AccountPassword;
                existingSystemAccount.AccountRole = account.AccountRole;

                _dbcontext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Database error while updating SystemAccount: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating SystemAccount: {ex.Message}");
            }
        }


        public List<object> GetSelectAccountList()
        {
            return _dbcontext.SystemAccounts
                .Select(c => new
                {
                    Value = c.AccountId.ToString(),
                    Text = c.AccountName.ToString()
                })
                .ToList<object>();
        }

    }
}