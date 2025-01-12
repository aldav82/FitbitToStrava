using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StravaSDK.Types
{
    public class StravaUploadData
    {
        public bool Success { get; set; }
        public long ActivityId { get; set; }
        public long? UploadId { get; set; }
        public string Error { get; set; }
        public string Status { get; set; }
    }

    public class StravaUpdateData
    {
        public bool commute { get; set; }
        public bool trainer { get; set; }
        public string description { get; set; }

        public string name { get; set; }

        public string type { get; set; }

        public string sport_type { get; set; }
        public string gear_id { get; set; }

    }
}
