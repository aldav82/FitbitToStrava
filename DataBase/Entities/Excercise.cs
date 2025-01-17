using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataBase.Entities
{
    public class Excercise
    {
        public int Id { get; set; } // Primary Key
        public string UserId { get; set; } = string.Empty; // Required
        public long ExcerciseId { get; set; } // Required
        public DateTime MigrationDate { get; set; }
        public long? StravaUploadId { get; set; }
        public string StravaUploadError { get; set; }
        public string StravaUploadStatus { get; set; }

        public long? StravaId { get; set; }

        public void MarkAsDuplicate(string user, DateTime date)
        {
            this.StravaUploadError = null;
            this.MigrationDate = date;
            this.UserId = user;
            this.StravaUploadStatus = "Migrated";
        }

        public void MarkAsError(string status, string error)
        {
            this.StravaUploadStatus = status;
            this.StravaUploadError = error;
        }

        public void MarkAsMigrationComplete(string user)
        {
            this.StravaUploadStatus = "Migrated";
            this.StravaUploadError = null;
            this.MigrationDate = DateTime.Now;
            this.UserId = user;
        }
    }
}
