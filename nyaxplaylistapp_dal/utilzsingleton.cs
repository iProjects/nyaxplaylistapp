using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nyaxplaylistapp_dal
{
    /// <summary>
    /// Description of utilzsingleton.
    /// </summary>
    public sealed class utilzsingleton
    {
        // Because the _instance member is made private, the only way to get the single
        // instance is via the static Instance property below. This can also be similarly
        // achieved with a GetInstance() method instead of the property. 
        private static utilzsingleton singleInstance;

        public static utilzsingleton getInstance(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new utilzsingleton(notificationmessageEventname);
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }

        public event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        public string TAG;

        private utilzsingleton(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            TAG = this.GetType().Name;
            _notificationmessageEventname = notificationmessageEventname;
        }

        private utilzsingleton()
        {

        }
		

        public string getappsettinggivenkey(string key = "", string defaultvalue = "")
        {
            try
            {

                string configvalue = "";

                configvalue = System.Configuration.ConfigurationManager.AppSettings[key];

                if (configvalue == null || String.IsNullOrEmpty(configvalue))
                {
                    return defaultvalue;
                }
                else
                {
                    return configvalue;
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, this.TAG));
                return defaultvalue;
            }
        }

        public playlist_dto builddtogivendatatable(DataTable dt, int _index)
        {
            playlist_dto _dto = new playlist_dto();
            _dto.media_id = Convert.ToString(dt.Rows[_index][DBContract.playlist_entity_table.MEDIA_ID]);
            _dto.media_name = Convert.ToString(dt.Rows[_index][DBContract.playlist_entity_table.MEDIA_NAME]);
            _dto.media_title = Convert.ToString(dt.Rows[_index][DBContract.playlist_entity_table.MEDIA_TITLE]);
            _dto.media_size = Convert.ToString(dt.Rows[_index][DBContract.playlist_entity_table.MEDIA_SIZE]);
            _dto.media_type = Convert.ToString(dt.Rows[_index][DBContract.playlist_entity_table.MEDIA_TYPE]);
            _dto.media_status = Convert.ToString(dt.Rows[_index][DBContract.playlist_entity_table.MEDIA_STATUS]);
            _dto.created_date = Convert.ToString(dt.Rows[_index][DBContract.playlist_entity_table.CREATED_DATE]);

            return _dto;
        }


    }
}
