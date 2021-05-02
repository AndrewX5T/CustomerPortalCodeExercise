using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer
{
    public class UserAccount
    {
        //Had to keep Identifier as a property with public setter for JSON serialization.
        //Just be careful not to modify it, it should never change.
        public Guid Identifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
