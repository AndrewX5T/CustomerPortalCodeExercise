using DataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortalCodeExercise.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IAccountService accountService;
        protected readonly IAccountStoringService accountStore;
        protected readonly IHashingService hasher;

        public BaseController(
            IAccountService accountService,
            IAccountStoringService accountStore,
            IHashingService hashingService)
        {
            this.accountService = accountService;
            this.accountStore = accountStore;
            this.hasher = hashingService;
        }

        protected UserAccount SetLoggedInUser(UserAccount account)
        {
            ISession session = ControllerContext.HttpContext.Session;

            session.Set("userAuthToken", account.Identifier.ToByteArray());

            return account;
        }

        protected void RemoveLoggedInUser()
        {
            ISession session = ControllerContext.HttpContext.Session;

            session.Remove("userAuthToken");
        }

        protected bool LoggedInUser(out UserAccount account)
        {
            ISession session = ControllerContext.HttpContext.Session;

            byte[] userAuthToken = session.Get("userAuthToken");

            if (userAuthToken != null)
            {
                Guid identifier = new Guid(userAuthToken);

                if(accountService.AccountExists(identifier, out account))
                {
                    ViewData["loggedIn"] = true;
                    ViewData["user"] = account;

                    return true;
                }
            }

            account = null;
            return false;
        }
    }
}
