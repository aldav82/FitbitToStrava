using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Diagnostics.Eventing.Reader;
using DataObjects.Tools;

namespace FitBitToStravaApp.Infrastructure
{
    public static class Authentication
    {
        public static JwtSecurityToken DecodeJwt(this string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(jwtToken))
            {
                // Decode the token
                return handler.ReadJwtToken(jwtToken);
            }
            else
            {
                return null;
            }
        }

        public static void AddThirdPartyAuthentication(this WebApplicationBuilder builder)
        {
            // Add Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = ApplicationType.Fitbit; // Ensure that the default challenge scheme is set to your OAuth scheme
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            })
            .AddCookie("Cookies", options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Adjust as needed
                options.SlidingExpiration = true;
            })
            .AddOAuth(ApplicationType.Fitbit, options =>
            {
                var fitbitConfig = builder.Configuration.GetSection("Authentication:Fitbit");

                options.ClientId = fitbitConfig["ClientId"];
                options.ClientSecret = fitbitConfig["ClientSecret"];
                options.CallbackPath = fitbitConfig["CallbackPath"];

                // Fitbit-specific endpoints
                options.AuthorizationEndpoint = "https://www.fitbit.com/oauth2/authorize";
                options.TokenEndpoint = "https://api.fitbit.com/oauth2/token";
                options.UserInformationEndpoint = "https://api.fitbit.com/1/user/-/profile.json";

                // Scopes
                options.Scope.Add("profile");
                options.Scope.Add("activity");
                options.Scope.Add("location");
                options.Scope.Add("respiratory_rate");

                options.SaveTokens = true;

                // Customize Backchannel to modify the token request
                options.Backchannel = new HttpClient(new HttpClientHandler())
                {
                    DefaultRequestHeaders =
                    {
            Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
                    $"{options.ClientId}:{options.ClientSecret}")))
                    }
                };

                // Retrieve user info
                options.Events.OnCreatingTicket = async context =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, options.UserInformationEndpoint);
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                    var response = await context.Backchannel.SendAsync(request);

                    try
                    {
                        response.EnsureSuccessStatusCode();
                        var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                        var root = user.RootElement;

                        // Map claims from the Fitbit profile
                        context.Identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, root.GetProperty("user").GetProperty("encodedId").GetString()!));
                        context.Identity?.AddClaim(new Claim(ClaimTypes.Name, root.GetProperty("user").GetProperty("fullName").GetString()!));
                        context.Identity?.AddClaim(new Claim("Image", root.GetProperty("user").GetProperty("avatar").GetString()!));
                        context.Identity?.AddClaim(new Claim(CustomClaims.FitbitToken, context.AccessToken));
                    }
                    catch (HttpRequestException ex)
                    {
                        context.Fail("Failed to retrieve user information from Fitbit.");

                        context.Response.Redirect($"/Error?code={ex.StatusCode}&app=fitbit");

                    } 

                    
                };
                options.Events.OnRemoteFailure = context =>
                {
                    // Handle errors during the OAuth flow (e.g., invalid tokens, authorization failures)
                    context.Response.Redirect("/Error?error=oauth_failure&app=fitbit");
                    context.HandleResponse(); // Prevent further processing by the middleware
                    return Task.CompletedTask;
                };
            })
            .AddOAuth(ApplicationType.Strava, options =>
            {
                var stravaConfig = builder.Configuration.GetSection("Authentication:Strava");
                options.ClientId = stravaConfig["ClientId"];
                options.ClientSecret = stravaConfig["ClientSecret"];
                options.CallbackPath = stravaConfig["CallbackPath"];
                options.AuthorizationEndpoint = "https://www.strava.com/oauth/authorize";
                options.TokenEndpoint = "https://www.strava.com/oauth/token";
                options.UserInformationEndpoint = "https://www.strava.com/api/v3/athlete";
                options.Scope.Add("activity:write,activity:read_all");
                options.SaveTokens = true;
                // Configure other Strava-specific options


                // Optional: Do not add claims for Strava
                options.Events.OnCreatingTicket = async context =>
                {
                    // Do nothing or log the response if necessary


                    var fitbitResult = context.Request.HttpContext.AuthenticateAsync(ApplicationType.Fitbit).GetAwaiter().GetResult();
                    if (fitbitResult.Succeeded)
                    {
                        var accessToken = fitbitResult?.Properties.GetTokenValue("access_token");
                        foreach (var item in fitbitResult.Principal.Claims)
                        {
                            context.Identity?.AddClaim(item);
                        }
                        if (string.IsNullOrWhiteSpace(context.Identity.GetFitbitToken()))
                        {
                            context.Identity?.AddClaim(new Claim(CustomClaims.FitbitToken, accessToken));
                        }
                    }

                    var request = new HttpRequestMessage(HttpMethod.Get, options.UserInformationEndpoint);

                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                    var response = await context.Backchannel.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                    var root = user.RootElement;
                    context.Identity?.AddClaim(new Claim(CustomClaims.StravaToken, context.AccessToken));

                    //// Map claims from the Fitbit profile
                    //string name = $"{root.GetProperty("firstname").GetString()} {root.GetProperty("lastname").GetString()}".Trim();
                    //context.Identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, root.GetProperty("id").GetInt64().ToString()));
                    //context.Identity?.AddClaim(new Claim(ClaimTypes.Name, name!));
                    //context.Identity?.AddClaim(new Claim("Image", root.GetProperty("profile").GetString()!));
                };

                //options.Events.OnRedirectToAuthorizationEndpoint = context =>
                //{
                //    if (context.Properties.Items.ContainsKey("state"))
                //    {
                //        var state = context.Properties.Items["state"];
                //        Console.WriteLine($"Redirecting to Strava with state: {context.Properties.Items["state"]}");
                //    }
                //    return Task.CompletedTask;
                //};
                //options.Events.OnTicketReceived = async context =>
                //{
                //    var state = context.Properties.Items["state"];
                //    Console.WriteLine($"Received state: {state}");
                //    // Inspect or log any other useful information for debugging
                //};
            });
        }
    }
}
