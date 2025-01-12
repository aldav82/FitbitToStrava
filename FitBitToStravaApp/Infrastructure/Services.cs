using Fitbit.Api.Portable;
using FitbitSDK;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Services;
using StravaSDK;
using System.Security.Claims;

namespace FitBitToStravaApp.Infrastructure
{
    public static class Services
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            // Todo: Check lifecycle
            //services.AddSingleton<IFitbitReader>(provider => {
            //    // Resolve configuration
            //    var config = provider.GetRequiredService<IConfiguration>();

            //    // Get the value from appsettings.json
            //    var clientId = config["Fitbit:ClientId"];
            //    var clientSecret = config["Fitbit:ClientSecret"];
            //    var returnUrl = config["Fitbit:ReturnUrl"];

            //    // Pass the value to A's constructor
            //    return new FitbitReader(clientId, clientSecret, returnUrl);
            //});
            services.AddScoped<IStravaClient>(provider =>
            {
                var context = provider.GetRequiredService<IHttpContextAccessor>();
                var logger = provider.GetRequiredService<ILogger<StravaClient>>();
                var stravaToken = context.HttpContext.User.GetStravaToken();
                return new StravaClient(stravaToken, logger);
            });
            services.AddScoped<IFitbitClient>(provider =>
            {
                return new Fitbit.Api.Portable.FitbitClient(handler =>
                {
                    var context = provider.GetRequiredService<IHttpContextAccessor>();
                    var token = context.HttpContext.User.GetFitbitToken();
                    var accessToken = token;
                    var client = new HttpClient(handler);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    return client;                    
                });
            });


            services.AddScoped<MigrateActivityService>();
            services.AddScoped<CheckUploadStatusService>();


        }
    }
}
