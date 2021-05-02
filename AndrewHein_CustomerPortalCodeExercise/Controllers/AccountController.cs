using CustomerPortalCodeExercise.Models;
using DataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortalCodeExercise.Controllers
{
    public class AccountController : BaseController
    {
        // GET: AccountController/Login
        public ActionResult Login([FromServices] IAccountService accountService)
        {
            if(LoggedInUser(accountService, out _))
            {
                return RedirectToAction("Details");
            }

            return View();
        }

        // POST: AccountController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(
            [FromForm] LoginAttempt loginAttempt,
            [FromServices] IAccountService accountService,
            [FromServices] IHashingService hasher
            )
        {
            //if already logged in, redirect to log out screen instead.
            if(LoggedInUser(accountService, out _))
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

            return View();
        }

        // GET: AccountController/Details/5
        public ActionResult Details([FromServices] IAccountService accountService)
        {
            //Must be logged in to see account details
            if (LoggedInUser(accountService, out UserAccount loginUser))
            {
                return View(loginUser);
            }

            //When not logged in, redirect to the login page
            return RedirectToAction("Login");
        }

        // GET: AccountController/Create
        public ActionResult Create([FromServices] IAccountService accountService)
        {
            //Can't create account while logged in, redirect to account details
            if(LoggedInUser(accountService, out _))
            {
                RedirectToAction("Details");
            }

            //ViewData vars used to report validation issues
            ViewData["accountFailure"] = false;
            ViewData["failureReason"] = string.Empty;

            return View();
        }

        // POST: AccountController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [FromForm] UserAccountBuilder userAccountAttempt,
            [FromServices] IAccountService accountService,
            [FromServices] IAccountStoringService accountStore,
            [FromServices] IHashingService hasher
            ) 
        {
            //if already logged in, redirect to account details
            if (LoggedInUser(accountService, out _))
            {
                RedirectToAction("Details");
            }

            //create the account from the form data
            UserAccount user = userAccountAttempt.CreateUserAccount(hasher);

            //pass the account to the service to validate and add it
            if (accountService.AddAccount(user, accountStore))
            {
                //direct user to login (new account created)
                return RedirectToAction("Login");
            }
            else
            {
                //prompt user that account creation failed
                ViewData["accountFailure"] = true;
                ViewData["failureReason"] = "Email already in use.";

                return View();
            }
        }

        // GET: AccountController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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
