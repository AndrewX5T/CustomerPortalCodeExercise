using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortalCodeExercise.Modelstate
{
    public class LoginAttempt
    {
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        /// <summary>
        /// Pass input credential account to account service
        /// </summary>
        /// <param name="accountService"></param>
        /// <param name="hasher"></param>
        /// <param name="account">The account matching the attempted login, or null of not exists</param>
        /// <returns>boolean: if the credentials match the stored account</returns>
        public bool AttemptAccount(
            IAccountService accountService,
            IHashingService hasher,
            out UserAccount account)
        {
            return accountService
                .LoginAttemptValid(Email, hasher.Hash(Password), out account);
        }
    }
}
