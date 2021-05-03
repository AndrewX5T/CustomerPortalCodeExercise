using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace DataAccessLayer
{
    public abstract class AccountStore : DataStore
    {
        //Path to the accounts file
        public const string FLAT_FILE_PATH = @"./Store/Accounts.json";
    }
}
