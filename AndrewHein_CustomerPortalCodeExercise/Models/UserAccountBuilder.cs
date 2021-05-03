using CustomerPortalCodeExercise.Controllers.Validation;
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
        public UserAccountBuilder(){}

        public UserAccountBuilder(UserAccount account)
        {
            this.PreserveIdentifier = true;
            this.Identifier = account.Identifier;
            this.FirstName = account.FirstName;
            this.LastName = account.LastName;
            this.Email = account.Email;
        }

        private bool PreserveIdentifier = false;
        public Guid Identifier { get; set; }

        [Display(Name = "First Name"), DataType(DataType.Text), Required]
        public string FirstName { get; set; }


        [Display(Name = "Last Name"), DataType(DataType.Text), Required]
        public string LastName { get; set; }


        [Display(Name = "Email Address"), DataType(DataType.EmailAddress), Required]
        [EmailAddress(ErrorMessage = "Please use a correctly formatted email address")]
        [EmailValidation]
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
            //null check the password before giving it to the hasher
            //this can occur while updating an account but not updating the password
            string hashedPassword = !string.IsNullOrEmpty(Password)
                ? hasher.Hash(Password) : null;

            //if creating an account, create a new guid as well
            //if performing an edit, preserve the guid from the last object
            Guid useIdentifier = PreserveIdentifier ? this.Identifier : Guid.NewGuid();

            return new UserAccount()
            {
                Identifier = useIdentifier,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                PasswordHash = hashedPassword
            };
        }
    }
}
