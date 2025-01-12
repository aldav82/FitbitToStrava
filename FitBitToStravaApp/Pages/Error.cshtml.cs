using DataObjects.Tools;
using Fitbit.Api.Portable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace FitBitToStravaApp.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    [AllowAnonymous]
    public class ErrorModel : PageModel
    {
        public string ErrorCode { get; set; }
        public string App { get; set; }
        public string Message { get; set; }


        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

            // Get the exception details
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            //if ( != null)
            //{
            //    ExceptionMessage = exceptionFeature.Error.Message;
            //    StackTrace = exceptionFeature.Error.StackTrace;
            //}

            var exception = exceptionFeature?.Error;
            if (exception is FitbitRateLimitException)
            {
                this.App = ApplicationType.Fitbit;
                this.Message = exception.Message;
            }
            else
            {
                this.ErrorCode = Request.Query["code"];
                this.App = Request.Query["app"];
            }
        }
    }

}
