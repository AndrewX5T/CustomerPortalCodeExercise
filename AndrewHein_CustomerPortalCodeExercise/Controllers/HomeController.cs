using CustomerPortalCodeExercise.Modelstate;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortalCodeExercise.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(
            IAccountService accountService,
            IHashingService hasher
            ) : base(accountService, hasher)
        {}


        public IActionResult Index()
        {
            LoggedInUser(out _);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
