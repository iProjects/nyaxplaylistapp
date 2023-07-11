using nyaxplaylistapp_dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nyaxplaylistapp_ui.reports
{
    public class reportmodel
    {
        public double total_size
        {
            get
            {
                return playlist.Sum(t => double.Parse(t.media_size));
            }
        }
        public string logo { get; set; } 
        public DateTime printedon { get; set; }
         
        
        public string reportname
        {
            get
            {
                return "playlist " + DateTime.Now.ToString("dd MM yyyy HH mm ss tt");
            }
        }
        public List<playlist_dto> playlist { get; set; }
    }

    public class ReportsEngineCompleteEventArg : System.EventArgs
    {
        private int svalue;
        private string _template;

        public ReportsEngineCompleteEventArg(int value, string template)
        {
            svalue = value;
            _template = template;
        }

        public int StatusValue
        {
            get { return svalue; }
        }

        public string _Template
        {
            get { return _template; }
        }
    }
    public class ReportsProcessCompleteEventArg : System.EventArgs
    {
        private int svalue;

        public ReportsProcessCompleteEventArg(int value)
        {
            svalue = value;
        }

        public int StatusValue
        {
            get { return svalue; }
        }
    }



}
