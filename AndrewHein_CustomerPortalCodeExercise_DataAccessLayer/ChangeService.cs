using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer
{
    public interface IChangeService
    {
        /// <summary>
        /// Uses changeStore to read data from .json into memory
        /// </summary>
        /// <returns></returns>
        public IChangeService LoadChanges();

        /// <summary>
        /// Uses changeStore to serialize change data into .json format and store
        /// </summary>
        /// <returns></returns>
        public IChangeService StoreChanges();

        /// <summary>
        /// Create an AccountChange object with the current timestamp and store it in the change history
        /// </summary>
        /// <param name="account"></param>
        /// <param name="fieldName"></param>
        /// <param name="previous"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        public IChangeService AddChange(UserAccount account, string fieldName, string previous, string updated);

        /// <summary>
        /// Get Hashset of AccountChanges from hashset for Account Identity
        /// </summary>
        /// <param name="account"></param>
        /// <returns>Hashset of AccountChanges or empty Hashset</returns>
        public HashSet<AccountChange> GetAccountChanges(UserAccount account);
    }

    public class ChangeService : IChangeService, IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return new ChangeService();
        }

        protected HashSet<AccountChange> changeHistory { get; set; }

        public IChangeService LoadChanges()
        {
            try
            {
                changeHistory = ChangeStore.GetValues<AccountChange>(ChangeStore.FLAT_FILE_PATH).ToHashSet();
            }
            catch (Exception except)// for expediency, handle any exception by initializing a new hashset
            {
                changeHistory = new HashSet<AccountChange>();
            }

            return this;
        }

        public IChangeService StoreChanges()
        {
            ChangeStore.Store(changeHistory, ChangeStore.FLAT_FILE_PATH);

            return this;
        }

        public IChangeService AddChange(UserAccount account, string fieldName, string previous, string updated)
        {
            //get the current time and update details for the AccountChange
            DateTime timestamp = DateTime.Now;

            Guid identifier = account.Identifier;

            AccountChange change = new AccountChange(identifier, fieldName, previous, updated, timestamp);

            //add the change to the dictionary
            changeHistory.Add(change);

            return this;
        }

        public HashSet<AccountChange> GetAccountChanges(UserAccount account)
        {
            HashSet<AccountChange> userChanges = new HashSet<AccountChange>();

            string userIdentifier = account.Identifier.ToString();

            userChanges = changeHistory
                .Where(x => x.Identifier == userIdentifier)
                .ToHashSet();

            return userChanges;
        }
    }
}
