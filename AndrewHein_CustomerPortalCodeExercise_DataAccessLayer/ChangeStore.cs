using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace DataAccessLayer
{
    public abstract class ChangeStore
    {
        private const string FLAT_FILE_PATH = @"./Store/ChangeHistory.json";

        public static void Store(Dictionary<Guid, HashSet<AccountChange>> changes)
        {
            JsonSerializerOptions jsonOpts = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            string serializedAccounts = JsonSerializer.Serialize(
                changes, typeof(Dictionary<Guid,HashSet<AccountChange>>), jsonOpts);

            File.WriteAllText(FLAT_FILE_PATH, serializedAccounts);
        }

        public static Dictionary<Guid, HashSet<AccountChange>> GetChanges()
        {
            return JsonSerializer.Deserialize<Dictionary<Guid, HashSet<AccountChange>>>(File.ReadAllText(FLAT_FILE_PATH));
        }
    }
}
