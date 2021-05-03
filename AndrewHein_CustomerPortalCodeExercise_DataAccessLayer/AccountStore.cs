using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace DataAccessLayer
{
    public abstract class AccountStore
    {
        //Path to the accounts file
        const string FLAT_FILE_PATH = @"./Store/Accounts.json";

        public static void Store(HashSet<UserAccount> accounts)
        {
            JsonSerializerOptions jsonOpts = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            string serializedAccounts = JsonSerializer.Serialize(accounts, typeof(HashSet<UserAccount>), jsonOpts);

            File.WriteAllText(FLAT_FILE_PATH, serializedAccounts);
        }
 
        public static HashSet<UserAccount> GetAccounts()
        {
            return JsonSerializer.Deserialize<HashSet<UserAccount>>(File.ReadAllText(FLAT_FILE_PATH));
        }

    }
}
