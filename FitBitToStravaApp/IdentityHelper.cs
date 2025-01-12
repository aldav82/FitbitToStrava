using FitBitToStravaApp.Infrastructure;
using System.Security.Claims;

namespace FitBitToStravaApp
{
    public static class IdentityHelper
    {
        public static string GetUserId(this ClaimsPrincipal identity)
        {
            return identity?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        public static string GetUserId(this ClaimsIdentity identity)
        {
            return identity?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        public static string GetFitbitToken(this ClaimsPrincipal identity)
        {
            return identity?.Claims.FirstOrDefault(c => c.Type == CustomClaims.FitbitToken)?.Value;
        }
        public static string GetFitbitToken(this ClaimsIdentity identity)
        {
            return identity?.Claims.FirstOrDefault(c => c.Type == CustomClaims.FitbitToken)?.Value;
        }
        public static string GetStravaToken(this ClaimsPrincipal identity)
        {
            return identity?.Claims.FirstOrDefault(c => c.Type == CustomClaims.StravaToken)?.Value;
        }
        public static string GetStravaToken(this ClaimsIdentity identity)
        {
            return identity?.Claims.FirstOrDefault(c => c.Type == CustomClaims.StravaToken)?.Value;
        }
    }
}
