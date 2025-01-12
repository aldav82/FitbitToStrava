using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitBitToStravaApp.Pages
{
    public class ClearCacheModel : PageModel
    {
        public LinkGenerator LinkGenerator { get; }

        

        public ClearCacheModel(LinkGenerator linkGenerator)
        {
            LinkGenerator = linkGenerator;
        }


        public void OnGet()
        {
        }
    }
}
