using DataBase.Repository;
using Fitbit.Api.Portable;
using FitbitSDK.Transformer;
using Microsoft.Extensions.Logging;
using StravaSDK;

namespace Services
{
    public class CheckUploadStatusService
    {
        private readonly ILogger<CheckUploadStatusService> _logger;
        private readonly IStravaClient _stravaClient;
        private readonly IFitbitClient _fitbitClient;
        private readonly IExcerciseRepository _excerciseRepository;

        public CheckUploadStatusService(IFitbitClient fitbitClient, IStravaClient stravaClient, IExcerciseRepository excerciseRepository,
            ILogger<CheckUploadStatusService> logger)
        {
            _fitbitClient = fitbitClient;
            _stravaClient = stravaClient;
            _excerciseRepository = excerciseRepository;
            _logger = logger;
        }
        public async Task ExecuteAsync(long activityId, DateTime activityDate, string userId, string fitbitToken)
        {
            var excercise = _excerciseRepository.GetByIds(new long[] { activityId }).FirstOrDefault();
            if (excercise == null)
            {
                _logger.LogError($"Could not retrieve activity information from local BD for id {activityId}");
                return;
            }
            if (excercise.StravaUploadId.HasValue)
            {
                var result = await _stravaClient.RetrieveUploadStatus(excercise.StravaUploadId.Value);
                if (result.status == "Your activity is ready.")
                {
                    // TCX data is updated as a run activity. We are going to correct activity data with an update
                    var migrationData = await _fitbitClient.GetActivityLogsListByIdAsync(activityDate, activityId);
                    var activity = await FitbitToDataObjects.RetrieveActivityAsync(migrationData, fitbitToken);
                    await _stravaClient.UpdateActivity(result.activity_id.Value, activity);
                    excercise.MarkAsMigrationComplete(userId);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(result.error) && result.error.Contains("duplicate of"))
                    {
                        _logger.LogWarning($"Activity {activityId} is a duplicate");
                        excercise.MarkAsDuplicate(userId, DateTime.Now);
                    }
                    else
                    {
                        _logger.LogWarning($"Activity {activityId} is a marked as error by Strava. with message {result.error} and status {result.status}");
                        excercise.MarkAsError(result.status, result.error);
                    }
                }
                await _excerciseRepository.UpdateAsync(excercise);
            } else
            {
                _logger.LogError($"Trying to check the upload status of activity id {activityId}, but the activity does not have a Strava upload id");

            }
        }
    }
}
