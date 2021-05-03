using CustomerPortalCodeExercise.Modelstate;
using DataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortalCodeExercise.Controllers
{
    public class AccountController : BaseController
    {
        protected readonly IChangeService changeService;

        public AccountController(
            IAccountService accountService,
            IHashingService hasher,
            IChangeService changeService
            ) : base(accountService, hasher)
        {
            this.changeService = changeService;
        }

        // GET: AccountController/Login
        public ActionResult Login()
        {
            if (LoggedInUser( out _))
            {
                return RedirectToAction("Details");
            }

            return View();
        }

        // POST: AccountController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([FromForm] LoginAttempt loginAttempt)
        {
            //if already logged in, redirect to log out screen instead.
            if (LoggedInUser(out _))
            {
                RedirectToAction("Logout");
            }

            //attempt to get the login details and pass them to the account service
            //if the credentials are valid and the account exists, store the account identifier in session
            //not a secure practice.
            if(loginAttempt.AttemptAccount(accountService, hasher, out UserAccount loginAccount))
            {
                SetLoggedInUser(loginAccount);
                changeService.AddChange(loginAccount, "Logged in", string.Empty, DateTime.Now.ToString());
                changeService.StoreChanges();
            }
            else
            {
                ViewData["info"] = "Incorrect email or password";
                ViewData["textClass"] = "text-danger";
                return View();
            }

            return Redirect("/Home/Index");
        }

        // POST: AccountController/Logout
        public ActionResult Logout()
        {
            if(LoggedInUser(out _)){
                RemoveLoggedInUser();
            }
            return Redirect("/");
        }

        // GET: AccountController/Details/
        public ActionResult Details()
        {
            //Must be logged in to see account details
            if (LoggedInUser(out UserAccount loginUser))
            {
                ViewData["changes"] = changeService.GetAccountChanges(loginUser);
                return View(loginUser);
            }

            //When not logged in, redirect to the login page
            return RedirectToAction("Login");
        }

        // GET: AccountController/Create
        public ActionResult Create()
        {
            //Can't create account while logged in, redirect to account details
            if(LoggedInUser(out _))
            {
                RedirectToAction("Details");
            }

            return View();
        }

        // POST: AccountController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] UserAccountBuilder userAccountAttempt) 
        {
            //if already logged in, redirect to account details
            if (LoggedInUser(out _))
            {
                RedirectToAction("Details");
            }

            if (ModelState.IsValid)
            {
                //pass the account to the service to validate and add it
                if (CreateNewAccount(userAccountAttempt))
                {
                    //direct user to login (new account created)
                    return RedirectToAction("AccountSuccess");
                }
            }

            return View();
        }

        public ActionResult AccountSuccess()
        {
            return View();
        }

        // GET: AccountController/Edit/5
        public ActionResult Edit()
        {
            if(LoggedInUser(out UserAccount account))
            {
                return View(new UserAccountBuilder(account));
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] UserAccountBuilder userAccountAttempt)
        {
            if(LoggedInUser(out UserAccount account))
            {
                UserAccount updatedFields = userAccountAttempt.CreateUserAccount(hasher);

                //Check for which fields are being updated, since the forms are exclusive these checks will be as well.

                //First or Last Name form:
                if (!string.IsNullOrEmpty(updatedFields.FirstName) || !string.IsNullOrEmpty(updatedFields.LastName))
                {
                    //update functions compare values, only update on change

                    accountService.UpdateAccountFirstName(
                        updatedFields.FirstName,
                        account,
                        changeService);

                    accountService.UpdateAccountLastName(
                        updatedFields.LastName,
                        account,
                        changeService);
                }
                //Email form:
                else if (!string.IsNullOrEmpty(updatedFields.Email))
                {
                    accountService.UpdateAccountEmail(
                        updatedFields.Email,
                        account,
                        changeService);
                }
                //Password form
                else if (!string.IsNullOrEmpty(updatedFields.PasswordHash))
                {
                    accountService.UpdateAccountPassword(
                        updatedFields.PasswordHash,
                        account,
                        changeService);
                }

                changeService.StoreChanges();

                return RedirectToAction("Details");
            }
            else
            { 
                return RedirectToAction("Login");
            }
        }

        private bool CreateNewAccount(UserAccountBuilder userAccountAttempt)
        {
            //create the account from the form data
            UserAccount user = userAccountAttempt.CreateUserAccount(hasher);

            bool accountCreated = accountService.AddAccount(user);

            if (accountCreated)
            {
                changeService.AddChange(user, "New Account", string.Empty, user.Identifier.ToString());
                changeService.AddChange(user, "First Name", string.Empty, user.FirstName);
                changeService.AddChange(user, "Last Name", string.Empty, user.LastName);
                changeService.AddChange(user, "Email", string.Empty, user.Email);
                changeService.AddChange(user, "PasswordHash", string.Empty, user.PasswordHash);
                changeService.StoreChanges();
            }

            return accountCreated;
        }
    }
}
