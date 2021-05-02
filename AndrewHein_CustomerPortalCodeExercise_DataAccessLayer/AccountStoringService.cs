using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace DataAccessLayer
{
    public interface IAccountStoringService
    {
        public void StoreAccounts<T>(IEnumerable<T> accounts);

        public IEnumerable<T> GetAccounts<T>();
    }

    public class AccountStoringService : IAccountStoringService, IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return new AccountStoringService();
        }

        //Path to the accounts file
        const string FLAT_FILE_PATH = @"./Accounts.json";

        public void StoreAccounts<T>(IEnumerable<T> accounts)
        {
            JsonSerializerOptions jsonOpts = new JsonSerializerOptions(); //not going to change any of the defaults

            string serializedAccounts = JsonSerializer.Serialize(accounts, typeof(T), jsonOpts);

            File.WriteAllText(FLAT_FILE_PATH, serializedAccounts);
        }

        public IEnumerable<T> GetAccounts<T>()
        {
            return JsonSerializer.Deserialize<IEnumerable<T>>(File.ReadAllText(FLAT_FILE_PATH));
        }

    }
}
