using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nyaxplaylistapp_dal
{
    public static class DBContract
    {
        public static String DATABASE_NAME = "nyax_playlist_db";
        public static String SQLITE_DATABASE_NAME = "nyax_playlist_db.sqlite3";

        public static String error = "error";
        public static String info = "info";
        public static String warn = "warn";
        public static String APP = "c-sharp";

        public static String mssql = "mssql";
        public static String mysql = "mysql";
        public static String sqlite = "sqlite";
        public static String postgresql = "postgresql";

        public const String video = "video";
        public const String series = "series";
        public const String audio = "audio";
        public const String movie = "movie";

        public static DialogResult dialogresult;

        public static String PLAYLIST_SELECT_ALL_QUERY = "SELECT * FROM " +
                                    DBContract.playlist_entity_table.TABLE_NAME;

        public static String PLAYLIST_SELECT_ALL_FILTER_QUERY = "SELECT * FROM " +
                            DBContract.playlist_entity_table.TABLE_NAME +
                            " where " +
                            DBContract.playlist_entity_table.MEDIA_STATUS +
                            " = " +
                            "'active'";

        //playlist table
        public static class playlist_entity_table
        {
            public static String TABLE_NAME = "tbl_nyax_playlist";
            //Columns of the table
            public static String MEDIA_ID = "media_id";
            public static String MEDIA_NAME = "media_name"; 
            public static String MEDIA_TITLE = "media_title";
            public static String MEDIA_SIZE = "media_size";
            public static String MEDIA_TYPE = "media_type";
            public static String MEDIA_STATUS = "media_status";
            public static String CREATED_DATE = "created_date";

        }
 



    }
}
