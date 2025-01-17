using DataBase.Entities;
using DataBase.Repository;
using DataObjects.Activities;
using Fitbit.Api.Portable;
using FitbitSDK.Transformer;
using Microsoft.Extensions.Logging;
using StravaSDK;

namespace Services
{
    public class MigrateActivityService
    {
        private readonly ILogger<MigrateActivityService> _logger;
        private readonly IStravaClient _stravaClient;
        private readonly IFitbitClient _fitbitClient;
        private readonly IExcerciseRepository _excerciseRepository;

        public MigrateActivityService(IFitbitClient fitbitClient, IStravaClient stravaClient, IExcerciseRepository excerciseRepository,
            ILogger<MigrateActivityService> logger)
        {
            _fitbitClient = fitbitClient;
            _stravaClient = stravaClient;
            _excerciseRepository = excerciseRepository;
            _logger = logger;
        }
        public async Task ExecuteAsync(long activityId, DateTime date, string fitbitToken, string userId, ActivityType? migrationActivityType = null)
        {
            _logger.LogTrace($"Retrieving activity information from Fitbit for activityId {activityId}");
            var migrationData = await _fitbitClient.GetActivityLogsListByIdAsync(date, activityId);
            if (migrationData == null) {
                _logger.LogError($"No activity data detected in database for activityId {activityId}");
                return;
            }

            var activity = await FitbitToDataObjects.RetrieveActivityAsync(migrationData, fitbitToken);
            activity.MigrationActivityType = migrationActivityType;
            _logger.LogTrace($"Creating activity in Strava for activityId {activityId}");
            var createdResult = await this._stravaClient.CreateActivities(new List<BaseActivity>() { activity });
            foreach (var item in createdResult.Where(c => c.Success))
            {
                await _excerciseRepository.AddAsync(new Excercise()
                {
                    ExcerciseId = activityId,
                    MigrationDate = DateTime.Now,
                    UserId = userId,
                    StravaUploadError = item.Error,
                    StravaUploadId = item.UploadId,
                    StravaUploadStatus = item.Status,
                    StravaId = item.StravaActivityId
                });
            }

        }

    }
}
