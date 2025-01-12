using Newtonsoft.Json.Linq;

namespace FitBitToStravaApp.Infrastructure
{
    public class TokensCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public TokensCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the user is authenticated
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Check if the required claim exists
                
                if (context.User.Identity.IsAuthenticated)
                {
                    var fitbitToken = context.User.GetFitbitToken();
                    var decoded = fitbitToken.DecodeJwt();
                    if (decoded != null)
                    {
                        // Extract the 'exp' claim
                        var expClaim = decoded.Claims.FirstOrDefault(c => c.Type == "exp");
                        if (expClaim != null)
                        {

                            // Convert Unix timestamp to DateTime
                            var expUnix = long.Parse(expClaim.Value);
                            var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
                            if (expirationDate < DateTime.Now)
                            {
                                context.Response.Redirect("/Logout");
                            }
                        }
                        //

                    }
                }

            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
