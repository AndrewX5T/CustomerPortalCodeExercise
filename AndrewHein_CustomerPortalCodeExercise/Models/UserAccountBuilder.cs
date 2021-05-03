using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortalCodeExercise.Modelstate
{
    public class UserAccountBuilder
    {
        [Display(Name = "First Name"), DataType(DataType.Text), Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name"), DataType(DataType.Text), Required]
        public string LastName { get; set; }

        [Display(Name = "Email Address"), DataType(DataType.EmailAddress), Required]
        [EmailAddress(ErrorMessage = "Please use a correctly formatted email address")]
        public string Email { get; set; }

        [Display(Name = "Password"), DataType(DataType.Password), Required]
        [StringLength(256, MinimumLength = 4, ErrorMessage = "{0} must be between {2} and {1} characters")]
        [RegularExpression(@"^(?=(.*[a-z]){1,})(?=(.*[A-Z]){1,})(?=(.*[0-9]){1,})(?=(.*[!@#$%^&*()\]\-_+.]){1,}).{4,}$", ErrorMessage = "{0} must include: 1 uppercase character, 1 lowercase character, 1 number, 1 symbol")]
        public string Password { get; set; }

        [Display(Name = "Verify Password"), DataType(DataType.Password), Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords must match")]
        public string VerifyPassword { get; set; }

        public UserAccount CreateUserAccount(IHashingService hasher)
        {
            return new UserAccount()
            {
                Identifier = Guid.NewGuid(),
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                PasswordHash = hasher.Hash(Password)
            };
        }
    }
}
