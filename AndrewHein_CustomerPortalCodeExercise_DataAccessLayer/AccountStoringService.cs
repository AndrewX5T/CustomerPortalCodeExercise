using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace DataAccessLayer
{
    public interface IAccountStoringService
    {
        public IAccountStoringService StoreAccounts<T>(HashSet<T> accounts);

        public HashSet<T> GetAccounts<T>();
    }

    public class AccountStoringService : IAccountStoringService, IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return new AccountStoringService();
        }

        //Path to the accounts file
        const string FLAT_FILE_PATH = @"./Store/Accounts.json";

        public IAccountStoringService StoreAccounts<T>(HashSet<T> accounts)
        {
            JsonSerializerOptions jsonOpts = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            string serializedAccounts = JsonSerializer.Serialize(accounts, typeof(HashSet<T>), jsonOpts);

            File.WriteAllText(FLAT_FILE_PATH, serializedAccounts);

            return this;
        }

        public HashSet<T> GetAccounts<T>()
        {
            return JsonSerializer.Deserialize<HashSet<T>>(File.ReadAllText(FLAT_FILE_PATH));
        }

    }
}
