using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer
{
    public interface IChangeService
    {
        /// <summary>
        /// Uses changeStore to read data from .json into memory
        /// </summary>
        /// <param name="changeStore"></param>
        /// <returns></returns>
        public IChangeService LoadChanges();

        /// <summary>
        /// Uses changeStore to serialize change data into .json format and store
        /// </summary>
        /// <param name="changeStore"></param>
        /// <returns></returns>
        public IChangeService StoreChanges();

        /// <summary>
        /// Create an AccountChange object with the current timestamp and store it in the change dictionary
        /// </summary>
        /// <param name="account"></param>
        /// <param name="fieldName"></param>
        /// <param name="previous"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        public IChangeService AddChange(Guid identifier, string fieldName, string previous, string updated);

        /// <summary>
        /// Get Hashset of AccountChanges from dictionary for Key Guid
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>Hashset of AccountChanges or empty Hashset</returns>
        public HashSet<AccountChange> GetAccountChanges(Guid identifier);
    }

    public class ChangeService : IChangeService, IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return new ChangeService();
        }

        protected Dictionary<Guid, HashSet<AccountChange>> changeDictionary { get; set; }

        public IChangeService LoadChanges()
        {
            try
            {
                changeDictionary = ChangeStore.GetChanges();
            }
            catch (Exception)// for expediency, handle any exception by initializing a new dictionary
            {
                changeDictionary = new Dictionary<Guid, HashSet<AccountChange>>();
            }

            return this;
        }

        public IChangeService StoreChanges()
        {
            ChangeStore.Store(changeDictionary);

            return this;
        }

        public IChangeService AddChange(Guid identifier, string fieldName, string previous, string updated)
        {
            //get the current time and update details for the AccountChange
            DateTime timestamp = DateTime.Now;

            AccountChange change = new AccountChange(identifier, fieldName, previous, updated, timestamp);

            //if there isn't already a collection for this account, create one
            if (!changeDictionary.ContainsKey(identifier))
            {
                changeDictionary.Add(identifier, new HashSet<AccountChange>());
            }

            //add the change to the dictionary
            changeDictionary[identifier].Add(change);

            return this;
        }

        public HashSet<AccountChange> GetAccountChanges(Guid identifier)
        {
            if (changeDictionary.TryGetValue(identifier, out HashSet<AccountChange> changes))
            {
                return changes;
            }

            //if key not found, return empty collection
            return new HashSet<AccountChange>();
        }
    }
}
