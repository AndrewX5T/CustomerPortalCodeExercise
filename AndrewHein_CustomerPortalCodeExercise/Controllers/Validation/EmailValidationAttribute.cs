using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortalCodeExercise.Controllers.Validation
{
    /// <summary>
    /// Checks the AccountService for an email already in use
    /// </summary>
    public class EmailValidationAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IAccountService accountService = (IAccountService)validationContext
                .GetService(typeof(IAccountService));

            //if email exists, it is in use and validation fails
            bool emailExists = accountService.AccountExists((string)value, out _);

            if (emailExists)
            {
                return new ValidationResult("This email address is already in use.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
