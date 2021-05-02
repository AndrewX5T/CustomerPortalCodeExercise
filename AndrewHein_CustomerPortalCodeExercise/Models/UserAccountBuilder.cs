using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortalCodeExercise.Models
{
    public class UserAccountBuilder
    {
        [Display(Name = "First Name")]
        [DataType(DataType.Text)]
        public string firstName { get; set; }

        [Display(Name = "Last Name")]
        [DataType(DataType.Text)]
        public string lastName { get; set; }

        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Display(Name = "Verify Password")]
        [DataType(DataType.Password)]
        public string verifyPassword { get; set; }

        public UserAccount CreateUserAccount(IHashingService hasher)
        {
            return new UserAccount()
            {
                Identifier = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = hasher.Hash(password)
            };
        }
    }
}
