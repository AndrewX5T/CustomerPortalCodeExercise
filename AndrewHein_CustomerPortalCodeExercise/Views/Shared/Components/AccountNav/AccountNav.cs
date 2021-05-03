using DataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortalCodeExercise.Views.Shared.Components.AccountNav
{
    [ViewComponent(Name = "AccountNav")]
    public class AccountNav : ViewComponent
    {
        private UserAccount account = null;

        public AccountNav(IAccountService accountService, IHttpContextAccessor context)
        {
            if(context.HttpContext.Session.TryGetValue("userAuthToken", out byte[] userTokenBytes))
            {
                if (accountService.AccountExists(new Guid(userTokenBytes), out UserAccount matchAccount))
                {
                    account = matchAccount;
                }
            }
        }

        public IViewComponentResult Invoke()
        {
            return View(account);
        }
    }
}
