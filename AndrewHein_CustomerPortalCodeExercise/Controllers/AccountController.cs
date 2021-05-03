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
            ViewData["validationIssues"] = new List<string>();

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
            ViewData["validationIssues"] = new List<string>();
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
                ((List<string>)ViewData["validationIssues"]).Add("Incorrect email or password.");
                return View();
            }

            return Redirect("/Home/Index");
        }

        // POST: AccountController/Logout

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

            //ViewData vars used to report validation issues
            ViewData["validationIssues"] = new List<string>();

            return View();
        }

        // POST: AccountController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] UserAccountBuilder userAccountAttempt) 
        {
            ViewData["validationIssues"] = new List<string>();

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
                else //account was not created
                {
                    //prompt user that account creation failed
                    ((List<string>)ViewData["validationIssues"]).Add("Email already in use.");
                }
            }
            else //model state is invalid
            {
                foreach(ModelStateEntry state in ModelState.Values)
                {
                    foreach(ModelError error in state.Errors)
                    {
                        ((List<string>)ViewData["validationIssues"]).Add(error.ErrorMessage);
                    }
                }
            }

            return View();
        }

        // GET: AccountController/Edit/5
        public ActionResult Edit()
        {
            if(LoggedInUser(out UserAccount account))
            {
                return View(account);
            }
            else
            {
                return RedirectToAction("Login");
            }
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
