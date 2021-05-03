using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataAccessLayer
{
    public interface IAccountService
    {
        /// <summary>
        /// Uses accountStore to read account data from .json into memory
        /// </summary>
        /// <returns>Hashset of UserAccounts</returns>
        public IAccountService LoadAccounts();

        /// <summary>
        /// Uses accountStore to serialize account data into .json format and store
        /// </summary>
        /// <returns>Service</returns>
        public IAccountService StoreAccounts();

        /// <summary>
        /// Check that an account with provided email does not exist, if not, add the account and store
        /// </summary>
        /// <param name="account"></param>
        /// <returns>boolean: was the account successfully added</returns>
        public bool AddAccount(UserAccount account);

        /// <summary>
        /// Compare name inputs to stored names, if they are valid and not identical, update stored
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="storedAccount"></param>
        /// <returns>Service</returns>
        public IAccountService UpdateAccountFirstName(string firstName, UserAccount storedAccount, IChangeService changeService);

        /// <summary>
        /// Compare name inputs to stored names, if they are valid and not identical, update stored
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="storedAccount"></param>
        /// <returns>Service</returns>
        public IAccountService UpdateAccountLastName(string lastName, UserAccount storedAccount, IChangeService changeService);

        /// <summary>
        /// Compare email to stored email, if valid and not already in use, update
        /// </summary>
        /// <param name="email"></param>
        /// <param name="storedAccount"></param>
        /// <returns>Service</returns>
        public IAccountService UpdateAccountEmail(string email, UserAccount storedAccount, IChangeService changeService);

        /// <summary>
        /// Compare password to stored password, if valid and not identical, update
        /// </summary>
        /// <param name="passwordHash"></param>
        /// <param name="storedAccount"></param>
        /// <returns>Service</returns>
        public IAccountService UpdateAccountPassword(string passwordHash,UserAccount storedAccount, IChangeService changeService);

        /// <summary>
        /// Email field must be unique. Check if the provided email matches any in the Accounts collection.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="matchAccount"></param>
        /// <returns>boolean: Account.Email Exists in Accounts</returns>
        public bool AccountExists(string emailAddress, out UserAccount matchAccount);

        /// <summary>
        /// Identifier must be unique. Once an account is created, its identifier should not be modified,
        /// and can be used to lookup the exact account from the service.
        /// </summary>
        /// <param name="identifer"></param>
        /// <param name="matchAccount"></param>
        /// <returns>boolean: Account.Email Exists in Accounts</returns>
        public bool AccountExists(Guid identifer, out UserAccount matchAccount);

        /// <summary>
        /// Check that the account exists and that the password hash matches the provided
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="passwordHash"></param>
        /// <param name="loginAccount"></param>
        /// <returns>boolean: Log in criteria met</returns>
        public bool LoginAttemptValid(string emailAddress, string passwordHash, out UserAccount loginAccount);
    }


    public class AccountService : IAccountService, IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return new AccountService();
        }

        protected HashSet<UserAccount> Accounts { get; set; }

        public IAccountService LoadAccounts()
        {
            try
            {
                Accounts = AccountStore.GetValues<UserAccount>(AccountStore.FLAT_FILE_PATH).ToHashSet();
            }
            catch(Exception) // for expediency, handle any exception by initializing a new hashset
            {
                Accounts = new HashSet<UserAccount>();
            }

            return this;
        }

        public IAccountService StoreAccounts()
        {
            AccountStore.Store(Accounts, AccountStore.FLAT_FILE_PATH);

            return this;
        }

        public bool AddAccount(UserAccount account)
        {
            if(!AccountExists(account.Email, out _))
            {
                Accounts.Add(account);

                StoreAccounts();

                return true;
            }
            return false;
        }

        public IAccountService UpdateAccountFirstName(string firstName, UserAccount storedAccount, IChangeService changeService)
        {
            if (!string.IsNullOrEmpty(firstName) //firstName not blank
                && !storedAccount.FirstName.Equals(firstName)) //firstname not = to stored
            {
                changeService.AddChange(storedAccount, "FirstName", storedAccount.FirstName, firstName);

                storedAccount.FirstName = firstName;

                StoreAccounts();
            }

            

            return this;
        }

        public IAccountService UpdateAccountLastName(string lastName, UserAccount storedAccount, IChangeService changeService)
        {
            if (!string.IsNullOrEmpty(lastName) //lastName not blank
                && !storedAccount.FirstName.Equals(lastName)) //lastName not = to stored
            {
                changeService.AddChange(storedAccount, "LastName", storedAccount.LastName, lastName);

                storedAccount.LastName = lastName;

                StoreAccounts();
            }

            

            return this;
        }

        public IAccountService UpdateAccountEmail(
            string email, UserAccount storedAccount, IChangeService changeService)
        {
            if (!string.IsNullOrEmpty(email) //email not blank
                && !storedAccount.Email.Equals(email) //email not = to stored
                && !AccountExists(email, out _)) //email not already in use
            {
                changeService.AddChange(storedAccount, "Email", storedAccount.Email, email);

                storedAccount.Email = email;

                StoreAccounts();
            }

            return this;
        }

        public IAccountService UpdateAccountPassword(
            string passwordHash, UserAccount storedAccount, IChangeService changeService)
        {
            if (!string.IsNullOrEmpty(passwordHash) //password not blank
                && !storedAccount.PasswordHash.Equals(passwordHash)) //password not = to stored
            {
                changeService.AddChange(storedAccount, "PasswordHash", storedAccount.PasswordHash, passwordHash);

                storedAccount.PasswordHash = passwordHash;

                StoreAccounts();
            }

            return this;
        }

        public bool AccountExists(string emailAddress, out UserAccount matchAccount)
        {
            matchAccount = Accounts
                .Where(x => x.Email.Equals(emailAddress))
                .FirstOrDefault();

            return matchAccount != null;
        }

        public bool AccountExists(Guid identifier, out UserAccount matchAccount)
        {
            matchAccount = Accounts
                .Where(x => x.Identifier.Equals(identifier))
                .FirstOrDefault();

            return matchAccount != null;
        }

        public bool LoginAttemptValid(string emailAddress, string passwordHash, out UserAccount loginAccount)
        {
            if (AccountExists(emailAddress, out loginAccount))
            {
                string inputHash = passwordHash;
                string storedHash = loginAccount.PasswordHash;

                return storedHash.Equals(inputHash);
            }
            return false;
        }
    }
}
