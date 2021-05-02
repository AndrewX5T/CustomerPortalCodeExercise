using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataAccessLayer
{
    public interface IAccountService
    {
        public HashSet<UserAccount> Accounts { get; set; }
        public IAccountService LoadAccounts(IAccountStoringService accountStore);
        public IAccountService StoreAccounts(IAccountStoringService accountStore);
        public bool AddAccount(UserAccount account, IAccountStoringService accountStore);
        public IAccountService UpdateAccount(UserAccount updatedAccount, UserAccount storedAccount);
        public bool AccountExists(UserAccount account, out UserAccount matchAccount);
        public bool AccountExists(Guid identifer, out UserAccount matchAccount);
        public bool LoginAttemptValid(UserAccount account, out UserAccount loginAccount);
    }


    public class AccountService : IAccountService, IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return new AccountService();
        }

        public HashSet<UserAccount> Accounts { get; set; }

        /// <summary>
        /// Uses accountStore to read account data from .json into memory
        /// </summary>
        /// <param name="accountStore"></param>
        /// <returns></returns>
        public IAccountService LoadAccounts(IAccountStoringService accountStore)
        {
            try
            {
                Accounts = accountStore.GetAccounts<UserAccount>();
            }
            catch(Exception) // for expediency, handle any exception by initializing a new hashset
            {
                Accounts = new HashSet<UserAccount>();
            }

            return this;
        }

        /// <summary>
        /// Uses accountStore to serialize account data into .json format and store
        /// </summary>
        /// <param name="accountStore"></param>
        /// <returns></returns>
        public IAccountService StoreAccounts(IAccountStoringService accountStore)
        {
            accountStore.StoreAccounts<UserAccount>(Accounts);

            return this;
        }

        /// <summary>
        /// Check that an account with provided email does not exist, if not, add the account and store
        /// </summary>
        /// <param name="account"></param>
        /// <returns>boolean: was the account successfully added</returns>
        public bool AddAccount(UserAccount account, IAccountStoringService accountStore)
        {
            if(!AccountExists(account, out _))
            {
                Accounts.Add(account);

                StoreAccounts(accountStore);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Check each property of stored account and provided updates, applying where different.
        /// Factors in special requirements such as uniqueness.
        /// </summary>
        /// <param name="updatedAccount"></param>
        /// <param name="storedAccount"></param>
        /// <returns></returns>
        public IAccountService UpdateAccount(UserAccount updatedAccount, UserAccount storedAccount)
        {
            //here we loop through a collection of properties, comparing the value of each for changes
            foreach (PropertyInfo prop in typeof(UserAccount).GetProperties())
            {
                object stored = prop.GetValue(storedAccount);
                object updated = prop.GetValue(updatedAccount);

                //if the updated object differs from the stored, apply the update, contingent upon the property in question.
                if (!stored.Equals(updated))
                {
                    switch (prop.Name)
                    {
                        case "Identifier":
                            //We never want to change this value. Ignore it even if different.
                            break;

                        case "Email":
                            //This is account-unique, special check to make sure it isn't duplicate
                            UpdateEmail(updatedAccount, storedAccount);
                            break;

                        default:
                            UpdateProperty(updatedAccount, storedAccount, prop);
                            break;
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// email change: as email is unique, first check that the new email is available, then update
        /// </summary>
        /// <param name="updatedAccount"></param>
        /// <param name="storedAccount"></param>
        /// <returns>boolean: update was performed</returns>
        bool UpdateEmail(UserAccount updatedAccount, UserAccount storedAccount)
        {
            if (!AccountExists(updatedAccount, out _)) //this will test for an account with same email
            {
                //Safe to proceed updating to the provided email
                storedAccount.Email = updatedAccount.Email;

                return true;
            }
            return false;
        }

        /// <summary>
        /// We know the account exists, just apply the change to one property (non-email)
        /// Get the value of the updated property and apply it to the property of the stored account.
        /// </summary>
        /// <param name="updatedAccount"></param>
        /// <param name="storedAccount"></param>
        /// <returns>boolean: update was performed</returns>
        bool UpdateProperty(UserAccount updatedAccount, UserAccount storedAccount, PropertyInfo prop)
        {
            object updatedProp = prop.GetValue(updatedAccount);

            storedAccount.GetType().GetProperty(prop.Name).SetValue(storedAccount, updatedProp);

            return true; //This doesn't feel right semantically, but this would ideally be a database update result or similar
        }

        /// <summary>
        /// Email field must be unique. Check if the provided email matches any in the Accounts collection.
        /// </summary>
        /// <param name="account"></param>
        /// <returns>boolean: Account.Email Exists in Accounts</returns>
        public bool AccountExists(UserAccount account, out UserAccount matchAccount)
        {
            matchAccount = Accounts
                .Where(x => x.Email.Equals(account.Email))
                .FirstOrDefault();

            return matchAccount != null;
        }

        /// <summary>
        /// Identifier must be unique. Once an account is created, its identifier should not be modified,
        /// and can be used to lookup the exact account from the service.
        /// </summary>
        /// <param name="account"></param>
        /// <returns>boolean: Account.Email Exists in Accounts</returns>
        public bool AccountExists(Guid identifier, out UserAccount matchAccount)
        {
            matchAccount = Accounts
                .Where(x => x.Identifier.Equals(identifier))
                .FirstOrDefault();

            return matchAccount != null;
        }

        /// <summary>
        /// Check that the account exists and that the password hash matches the provided
        /// </summary>
        /// <param name="account"></param>
        /// <param name="hasher"></param>
        /// <returns>boolean: Log in criteria met</returns>
        public bool LoginAttemptValid(UserAccount account, out UserAccount loginAccount)
        {
            if (AccountExists(account, out loginAccount))
            {
                string inputHash = account.PasswordHash;
                string storedHash = loginAccount.PasswordHash;

                return storedHash.Equals(inputHash);
            }
            return false;
        }
    }
}
