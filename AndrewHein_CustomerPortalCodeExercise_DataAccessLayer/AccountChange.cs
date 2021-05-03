using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer
{
    public class AccountChange
    {
        public AccountChange(
            Guid accountIdentifier,
            string field,
            string previous,
            string updated,
            DateTime timestamp
            )
        {
            this.Identifier = accountIdentifier.ToString();
            this.Field = field;
            this.PreviousValue = previous;
            this.UpdatedValue = updated;
            this.Timestamp = timestamp;
        }

        public string Identifier { get; set; }
        public string Field { get; set; }
        public string PreviousValue { get; set; }
        public string UpdatedValue { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
