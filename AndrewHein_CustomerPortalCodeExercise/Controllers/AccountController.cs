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
        public AccountController(
            IAccountService accountService,
            IAccountStoringService accountStore,
            IHashingService hasher
            ) : base(accountService, accountStore, hasher)
        {}

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
            }
            else
            {
                ViewData["incorrectLogin"] = true;
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
                //create the account from the form data
                UserAccount user = userAccountAttempt.CreateUserAccount(hasher);

                bool accountCreated = accountService.AddAccount(user, accountStore);

                //pass the account to the service to validate and add it
                if (accountCreated)
                {
                    //direct user to login (new account created)
                    return RedirectToAction("Login");
                }
            }

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
                if (!string.IsNullOrEmpty(updatedFields.FirstName)
                    || !string.IsNullOrEmpty(updatedFields.LastName))
                {
                    accountService.UpdateAccountName(
                        updatedFields.FirstName, updatedFields.LastName, account);
                }
                //Email form:
                else if (!string.IsNullOrEmpty(updatedFields.Email))
                {
                    accountService.UpdateAccountEmail(updatedFields.Email, account);
                }
                //Password form
                else if (!string.IsNullOrEmpty(updatedFields.PasswordHash))
                {
                    accountService.UpdateAccountPassword(updatedFields.PasswordHash, account);
                }

                return RedirectToAction("Details");
            }
            else
            { 
                return RedirectToAction("Login");
            }
        }

        // GET: AccountController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccountController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
