using DataObjects.Tools;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitBitToStravaApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SigninController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await ExternalCallback();
        }

        [AllowAnonymous]
        [HttpGet(ApplicationType.Strava)]
        public async Task<IActionResult> Strava()
        {
            return await ExternalCallback();

        }
        public async Task<IActionResult> ExternalCallback()
        {


            // Authenticate with Fitbit
            var fitbitResult = await HttpContext.AuthenticateAsync(ApplicationType.Fitbit);
            var stravaResult = await HttpContext.AuthenticateAsync(ApplicationType.Strava);

            if (fitbitResult.Succeeded && stravaResult.Succeeded)
            {
                // Create a new ClaimsIdentity
                var claimsIdentity = new ClaimsIdentity();

                // Add claims from Fitbit
                if (fitbitResult.Principal != null)
                {
                    claimsIdentity.AddClaims(fitbitResult.Principal.Claims);
                }

                // Add claims from Strava
                if (stravaResult.Principal != null)
                {
                    claimsIdentity.AddClaims(stravaResult.Principal.Claims);
                }

                // Create a new ClaimsPrincipal with the merged identity
                var mergedPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Set the merged identity as the current user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, mergedPrincipal);

                // Continue with the rest of your logic (e.g., redirect)
                return RedirectToAction("Index", "Home");
            }

            // Handle failure if needed
            return RedirectToAction("Error", "Home");

        }
    }
}
