using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace nyaxplaylistapp_dal
{
    public sealed class playlist_utilz_singleton : IDisposable
    {

        // Because the _instance member is made private, the only way to get the single
        // instance is via the static Instance property below. This can also be similarly
        // achieved with a GetInstance() method instead of the property.
        private static playlist_utilz_singleton singleInstance;

        public static playlist_utilz_singleton getInstance(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new playlist_utilz_singleton(notificationmessageEventname);
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }

        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        private string TAG;

        private playlist_utilz_singleton(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            _notificationmessageEventname = notificationmessageEventname;
            try
            {
                TAG = this.GetType().Name;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        private playlist_utilz_singleton()
        {

        }

        public void Dispose()
        {
            Console.WriteLine("IDisposable");
        }

        public responsedto createplaylistindatabase(playlist_dto _playlist_dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                //mssql
                try
                {
                    responsedto _mssql_responsedto = mssqlapisingleton.getInstance(_notificationmessageEventname).createweightindatabase(_playlist_dto);

                    if (_mssql_responsedto.isresponseresultsuccessful)
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_mssql_responsedto.responsesuccessmessage, TAG));
                        _responsedto.responsesuccessmessage += (Environment.NewLine + _mssql_responsedto.responsesuccessmessage);
                    }
                    else
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_mssql_responsedto.responseerrormessage, TAG));
                        _responsedto.responseerrormessage += (Environment.NewLine + _mssql_responsedto.responseerrormessage);
                    }
                }
                catch (Exception ex)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage += ex.Message;
                }

                //mysql
                try
                {
                    responsedto _mysql_responsedto = mysqlapisingleton.getInstance(_notificationmessageEventname).createweightindatabase(_playlist_dto);

                    if (_mysql_responsedto.isresponseresultsuccessful)
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_mysql_responsedto.responsesuccessmessage, TAG));
                        _responsedto.responsesuccessmessage += (Environment.NewLine + _mysql_responsedto.responsesuccessmessage);
                    }
                    else
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_mysql_responsedto.responseerrormessage, TAG));
                        _responsedto.responseerrormessage += (Environment.NewLine + _mysql_responsedto.responseerrormessage);
                    }

                }
                catch (Exception ex)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage += ex.Message;
                }

                //postgresql
                try
                {
                    responsedto _postgresql_responsedto = postgresqlapisingleton.getInstance(_notificationmessageEventname).createweightindatabase(_playlist_dto);

                    if (_postgresql_responsedto.isresponseresultsuccessful)
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_postgresql_responsedto.responsesuccessmessage, TAG));
                        _responsedto.responsesuccessmessage += (Environment.NewLine + _postgresql_responsedto.responsesuccessmessage);
                    }
                    else
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_postgresql_responsedto.responseerrormessage, TAG));
                        _responsedto.responseerrormessage += (Environment.NewLine + _postgresql_responsedto.responseerrormessage);
                    }


                }
                catch (Exception ex)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage += ex.Message;
                }

                //sqlite
                try
                {
                    responsedto _sqlite_responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).createweightindatabase(_playlist_dto);

                    if (_sqlite_responsedto.isresponseresultsuccessful)
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_sqlite_responsedto.responsesuccessmessage, TAG));
                        _responsedto.responsesuccessmessage += (Environment.NewLine + _sqlite_responsedto.responsesuccessmessage);
                    }
                    else
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_sqlite_responsedto.responseerrormessage, TAG));
                        _responsedto.responseerrormessage += (Environment.NewLine + _sqlite_responsedto.responseerrormessage);
                    }


                }
                catch (Exception ex)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage += ex.Message;
                }

                return _responsedto;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }

        private DataTable getallrecordsfrommssql()
        {
            try
            {

                DataTable mssql_dt = mssqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal();
                if (mssql_dt != null && mssql_dt.Rows.Count != 0)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mssql records count: " + mssql_dt.Rows.Count, TAG));
                }
                return mssql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        private DataTable getallrecordsfrommysql()
        {
            try
            {

                DataTable mysql_dt = mysqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal();
                if (mysql_dt != null && mysql_dt.Rows.Count != 0)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mysql records count: " + mysql_dt.Rows.Count, TAG));
                }
                return mysql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        private DataTable getallrecordsfrompostgresql()
        {
            try
            {

                DataTable postgresql_dt = postgresqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal();
                if (postgresql_dt != null && postgresql_dt.Rows.Count != 0)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("postgresql records count: " + postgresql_dt.Rows.Count, TAG));
                }
                return postgresql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        private DataTable getallrecordsfromsqlite()
        {
            try
            {

                DataTable sqlite_dt = sqliteapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal();
                if (sqlite_dt != null && sqlite_dt.Rows.Count != 0)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite records count: " + sqlite_dt.Rows.Count, TAG));
                }
                return sqlite_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public List<playlist_dto> getallrecordsfromdb()
        {
            try
            {
                List<playlist_dto> lstdtos = new List<playlist_dto>();

                var mssql_dt = getallrecordsfrommssql();
                var mysql_dt = getallrecordsfrommysql();
                var postgresql_dt = getallrecordsfrompostgresql();
                var sqlite_dt = getallrecordsfromsqlite();

                var _recordscount = mssql_dt.Rows.Count;

                for (int i = 0; i < _recordscount; i++)
                {
                    playlist_dto _dto = utilzsingleton.getInstance(_notificationmessageEventname).builddtogivendatatable(mssql_dt, i);
                    lstdtos.Add(_dto);
                }
                 
                lstdtos.Reverse();
                 
                return lstdtos.ToList();

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG)); 
                return null;
            }

        }




    }
}
