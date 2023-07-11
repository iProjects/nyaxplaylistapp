using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace nyaxplaylistapp_dal
{
    public class playlist_dto
    {
        public string media_id { get; set; }
        public string media_name { get; set; }
        public string media_title { get; set; }
        public string media_size { get; set; }
        public string media_type { get; set; }
        public string media_status { get; set; }
        public string created_date { get; set; }

    }
}
