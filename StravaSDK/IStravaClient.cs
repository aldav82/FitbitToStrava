using DataObjects.Activities;
using StravaSDK.Types;

namespace StravaSDK
{
    public interface IStravaClient
    {
        Task<StravaUploadData[]> CreateActivities(List<BaseActivity> activities);
        Task<StravaUpload> RetrieveUploadStatus(long stravaUploadId);
        Task UpdateActivity(long stravaActivityId, BaseActivity activityInfo);
        Task<StravaUploadData> CreateNonTcxActivityAsync(BaseActivity activity);
        Task<StravaUploadData> CreateTcxActivityAsync(BaseActivity activity);
    }
}
