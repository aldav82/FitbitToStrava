namespace DataObjects.Activities
{
    public abstract class BaseActivity
    {
        public abstract ActivityType ActivityType { get; }
        public ActivityType? MigrationActivityType { get; set; }
        public long ActivityId { get; set; }
        public long? StravaActivityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan Duration { get; set; }
        public double Distance { get; set; }
        public double DistanceKm { get { return Distance / 1000; } }

        public bool Migrated { get; set; }
        public bool StravaError
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.UploadStatus) && this.UploadStatus.Contains("There was an error processing your activity.");
            }
        }
        public string UploadStatus { get; set; }
        public long? UploadId { get; set; }
        public string TcxContent { get; set; }
        public string DistanceUnit { get; set; }

        public bool IsProcessing {
            get
            {
                return !string.IsNullOrWhiteSpace(this.UploadStatus) && this.UploadStatus != "Migrated";
            }
        }

        public string MigrationError { get; set; }
        public bool ContainsTcx
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TcxContent);
            }
        }
    }
}
