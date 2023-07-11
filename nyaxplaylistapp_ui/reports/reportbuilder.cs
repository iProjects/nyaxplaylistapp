using nyaxplaylistapp_dal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nyaxplaylistapp_ui.reports
{
    public class reportbuilder
    {
        reportmodel _reportmodel;
        public bool error = false;
        public string TAG;

        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;

        public reportbuilder(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            try
            {
                TAG = this.GetType().Name;

                //initialization
                _notificationmessageEventname = notificationmessageEventname;

            }
            catch (Exception ex)
            {
                error = true;
                Utils.ShowError(ex);
            }
        }

        public reportmodel GetReport()
        {
            try
            {
                Build();
                return _reportmodel;
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
                return null;
            }
        }

        private void Build()
        {
            try
            {
                _reportmodel = new reportmodel();
                _reportmodel.logo = getlogo();
                _reportmodel.printedon = DateTime.Today;
                _reportmodel.playlist = this.getplaylist();
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
            }
        }
        private List<playlist_dto> getplaylist()
        {
            try
            {
                List<playlist_dto> lst_records = new List<playlist_dto>();

                lst_records = playlist_utilz_singleton.getInstance(_notificationmessageEventname).getallrecordsfromdb();
                int count = lst_records.Count;

                return lst_records.Take(10).ToList();
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
                return null;
            }
        }
        private string getlogo()
        {
            string logo = "";
            string app_dir = Utils.get_application_path();
            var logo_path = Path.Combine(app_dir, "resources");

            if (!Directory.Exists(logo_path))
            {
                Directory.CreateDirectory(logo_path);
            }

            logo = Path.Combine(logo_path, "logo.jpg");
            return logo;
        }


    }
}
