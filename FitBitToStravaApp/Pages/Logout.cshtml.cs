using Fitbit.Api.Portable;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace FitBitToStravaApp.Pages
{
    public class LogoutModel : PageModel
    {
        private IConfiguration _configuration;

        public LogoutModel(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public async Task<IActionResult> OnGet()
        {
            var fitbitToken = User.GetFitbitToken();
            var stravaToken = User.GetStravaToken();
            // Sign out of the default authentication scheme (e.g., Cookies)
            // Clear the session
            HttpContext.Session.Clear();

            // Sign out
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!string.IsNullOrWhiteSpace(fitbitToken))
            {
                await RevokeFitbitToken(fitbitToken);
            }

            if (HttpContext.Request.Cookies.ContainsKey(".AspNetCore.Cookies"))
            {
                HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            }
            HttpContext.User = new System.Security.Claims.ClaimsPrincipal();
            Response.Headers.Remove("Cache-Control");
            Response.Headers.Remove("Pragma");
            Response.Headers.Append("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");
            Response.Headers.Append("Pragma", "no-cache");
            // Redirect to the home page or any other desired page
            return RedirectToPage("/ClearCache");
        }

        public async Task RevokeFitbitToken(string accessToken)
        {
            var stravaConfig = _configuration.GetSection("Authentication:Fitbit");
            var clientId = stravaConfig["ClientId"];
            var clientSecret = stravaConfig["ClientSecret"];

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.fitbit.com/oauth2/revoke");
            request.Content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("token", accessToken)
    });
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                // Handle revocation failure
            }
        }
    }
}
