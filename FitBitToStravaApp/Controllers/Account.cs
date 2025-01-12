using DataObjects.Tools;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitBitToStravaApp.Controllers
{
    [Route("Account/[action]")]
    public class Account : Controller
    {
        public IActionResult StravaLogin()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/"
            }, ApplicationType.Strava);


        }
        public IActionResult FitbitLogin()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/"
            }, ApplicationType.Fitbit);


        }
    }
}
