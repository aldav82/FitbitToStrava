using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StravaSDK.Types
{
    public class StravaUpload
    {
        public string id_str { get; set; }
        public long? activity_id { get; set; }
        public string external_id { get; set; }
        public long id { get; set; }
        public string error { get; set; }
        public string status { get; set; }
    }
}
