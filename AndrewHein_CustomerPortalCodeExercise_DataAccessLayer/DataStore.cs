using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace DataAccessLayer
{
    public abstract class DataStore
    {
        /// <summary>
        /// Serializes and stores Dictionary of Change Collections in .json format
        /// </summary>
        /// <param name="changes"></param>
        public static void Store<T>(IEnumerable<T> changes, string FLAT_FILE_PATH)
        {
            JsonSerializerOptions jsonOpts = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            string serializedAccounts = JsonSerializer.Serialize(
                changes, typeof(IEnumerable<T>), jsonOpts);

            File.WriteAllText(FLAT_FILE_PATH, serializedAccounts);
        }

        /// <summary>
        /// Deserializes and loads .json ChangeHistory file into memory
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>(string FLAT_FILE_PATH)
        {
            return JsonSerializer.Deserialize<IEnumerable<T>>(File.ReadAllText(FLAT_FILE_PATH));
        }
    }
}
