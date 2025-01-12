using DataBase.Repository;
using DataObjects.Activities;
using Fitbit.Api.Portable;
using FitbitSDK.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using StravaSDK;

namespace FitBitToStravaApp.Pages
{
    [Authorize()]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfigRepository _config;
        private readonly IExcerciseRepository excerciseRepository;
        private readonly IStravaClient stravaClient;
        private readonly MigrateActivityService _migrateActivityService;
        private readonly CheckUploadStatusService _checkUploadStatusService;
        private DateTime checkDate = DateTime.Now.AddDays(1);
        public DateTime LastCheckedDate { 
            get { return this.checkDate; } set { this.checkDate = value; } }
        public bool IsFitbitAuthenticated {
            get
            {
                string val = HttpContext.Session.GetString(nameof(IsFitbitAuthenticated));
                bool isFitbitAuthenticated = false;
                bool.TryParse(val, out isFitbitAuthenticated);
                return isFitbitAuthenticated;
            }
            set {
                HttpContext.Session.SetString(nameof(IsFitbitAuthenticated), value.ToString());
            }
        }

        public List<BaseActivity> Activities { get; private set; } = new List<BaseActivity>();

        private IFitbitClient _fitbitClient;

        public IndexModel(ILogger<IndexModel> logger, IFitbitClient fitbitClient, IConfigRepository config, IExcerciseRepository excerciseRepository, IStravaClient stravaClient,
            MigrateActivityService migrateActivityService, CheckUploadStatusService checkUploadStatusService)
        {
            _fitbitClient = fitbitClient;
            _logger = logger;
            _config = config;
            this.stravaClient = stravaClient;
            this.excerciseRepository = excerciseRepository;
            _migrateActivityService = migrateActivityService;
            _checkUploadStatusService = checkUploadStatusService;
        }

        public async Task OnGet()
        {
            if (string.IsNullOrWhiteSpace(User.GetFitbitToken()))
            {
                Redirect("/Privacy");
            }

            var date = DateTime.Now;

            if (Request.Query.ContainsKey("date"))
            {
                DateTime.TryParseExact(Request.Query["date"], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out date);
            }
            LastCheckedDate = date;

            var activities = (await _fitbitClient.GetActivityLogsListAsync(LastCheckedDate))?.Activities?? new List<Fitbit.Api.Portable.Models.Activities>();
            foreach (var item in activities)
            {
                this.Activities.Add(await FitbitToDataObjects.RetrieveActivityAsync(item, User.GetFitbitToken()));
            }
            var migrated = this.excerciseRepository.GetByIds(this.Activities.Select(c => c.ActivityId).ToArray()).ToList();
            foreach (var activity in migrated)
            {
                var item = Activities.First(c => c.ActivityId == activity.ExcerciseId);
                item.Migrated = true;
                item.UploadStatus = activity.StravaUploadStatus;
                item.UploadId = activity.StravaUploadId;
                item.MigrationError = activity.StravaUploadError;
            }

        }

        public async Task<IActionResult> OnPostMigrateAsync(long id, [FromQuery] DateTime date)
        {

            await _migrateActivityService.ExecuteAsync(id, date, User.GetFitbitToken(), User.GetUserId());
            return RedirectToPage(new {date= date.ToString("yyyyMMdd") });
        }



        public async Task<IActionResult> OnPostRefreshAsync(long id, [FromQuery] DateTime date)
        {
            await _checkUploadStatusService.ExecuteAsync(id, date, User.GetUserId(), User.GetFitbitToken());
            return RedirectToPage(new { date = date.ToString("yyyyMMdd") });
        }
    }
}
