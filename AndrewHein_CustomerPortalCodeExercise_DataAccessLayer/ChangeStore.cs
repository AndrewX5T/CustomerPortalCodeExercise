using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace DataAccessLayer
{
    public abstract class ChangeStore : DataStore
    {
        //Path to the change history file
        public const string FLAT_FILE_PATH = @"./Store/ChangeHistory.json";
    }
}
