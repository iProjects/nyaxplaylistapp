using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nyaxplaylistapp_dal;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using nyaxplaylistapp_ui.reports;

namespace nyaxplaylistapp_ui
{
    /// <summary> 
    /// class to loop through files in folder and create a
    /// json formated string for append into a file to be used by the browser
    /// </summary>    

    public partial class create_playlist : Form
    {
        public string TAG;
        //Event declaration:
        //event for publishing messages to output
        public event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        //event for showing progress in long running tasks
        public event EventHandler<progressBarNotificationEventArgs> _progressBarNotificationEventname;
        //countdown 
        Stopwatch stopWatch = new Stopwatch();
        //root path where this application is running at
        public string _apppath;
        //hold the folder path
        string strpathfolder = "";
        //output file to append the playlist items
        string str_file_to_create = "";
        //list to hold messages
        public List<notificationdto> _lstnotificationdto = new List<notificationdto>();
        //global variables for playlist
        string strQuote, strfilenamewithnoextension, strfilename, strfilesize, strfiletype;
        //global total files counta
        int _total_media_files = 0;
        //set the global current position counter for use when creating json output
        int _current_media_file_pos = 0;
        //flag to be used in background worker
        bool _process_for_all = false;
        //set the global current counter for use when executing background task
        //int _global_time_lapsed_counta = 0;
        //flag to check if background worker is running
        bool _is_backgroud_worker_running = false;

        int _global_proccessed_files_counta = 0;
        /* to use a BackgroundWorker object to perform time-intensive operations in a background thread.
		You need to:
		1. Define a worker method that does the time-intensive work and call it from an event handler for the DoWork
		event of a BackgroundWorker.
		2. Start the execution with RunWorkerAsync. Any argument required by the worker method attached to DoWork
		can be passed in via the DoWorkEventArgs parameter to RunWorkerAsync.
		In addition to the DoWork event the BackgroundWorker class also defines two events that should be used for
		interacting with the user interface. These are optional.
		The RunWorkerCompleted event is triggered when the DoWork handlers have completed.
		The ProgressChanged event is triggered when the ReportProgress method is called. */
        BackgroundWorker bgWorker = new BackgroundWorker();
        // Timers namespace rather than Threading
        System.Timers.Timer running_timer = new System.Timers.Timer(); // Doesn't require any args
        // Timers namespace rather than Threading
        System.Timers.Timer elapsed_timer = new System.Timers.Timer(); // Doesn't require any args
        //task start time
        DateTime _task_start_time = DateTime.Now;
        //task end time
        DateTime _task_end_time = DateTime.Now;
        private int _TimeCounter = 0;
        DateTime _startDate = DateTime.Now;

        public create_playlist()
        {
            InitializeComponent();

            TAG = this.GetType().Name;

            AppDomain.CurrentDomain.UnhandledException += new
UnhandledExceptionEventHandler(UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);

            //Subscribing to the event: 
            //Dynamically:
            //EventName += HandlerName;
            _notificationmessageEventname += notificationmessageHandler;
            _progressBarNotificationEventname += progressBarNotificationHandler;

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("finished form initialization", TAG));

        }

        private void create_playlist_Load(object sender, EventArgs e)
        {
            //app version
            var _buid_version = Application.ProductVersion;
            lblbuildversion.Text = "build version - " + _buid_version;

            //initalize the countdown
            stopWatch = new Stopwatch();
            stopWatch.Start();

            //initialize current running time timer
            running_timer.Interval = 1000;
            running_timer.Elapsed += running_timer_Elapsed; // Uses an event instead of a delegate
            running_timer.Start(); // Start the timer

            elapsed_timer.Interval = 1000;
            elapsed_timer.Elapsed += elapsed_timer_Elapsed; // Uses an event instead of a delegate
            elapsed_timer.Start(); // Start the timer

            //populate the media type combo box
            cbomediatype.Items.AddRange(new[] { 
                "series", 
                "video",
                "movie",
                "audio"
            });
            //set app path to hold the current root directory of the application
            _apppath = Application.StartupPath;
            //select the first item
            cbomediatype.SelectedIndex = 0;
            //on press enter keyboard button invoke click handler for btngenerate_playlist_json 
            this.AcceptButton = btngenerate_playlist_json;
            //on press escape keyboard button invoke click handler for btnexit 
            this.CancelButton = btnexit;
            //set focus to combo box
            cbomediatype.Focus();

            //reset progress controls
            toolStripProgressBar.Value = 0;
            lblprogresscounta.Text = string.Empty;
            lblprogresscounta.TextChanged += lblprogresscounta_TextChanged;
            lbltimelapsed.Text = string.Empty;
            lbltotalfilesprocessed.Text = string.Empty;
            lblprogresspercentage.Text = string.Empty;

            txtsource.Text = string.Empty;
            txtdestination.Text = string.Empty;

            //This allows the BackgroundWorker to be cancelled in between tasks
            bgWorker.WorkerSupportsCancellation = true;
            //This allows the worker to report progress between completion of tasks...
            //this must also be used in conjunction with the ProgressChanged event
            bgWorker.WorkerReportsProgress = true;

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("finished form load", TAG));

        }

        private void btngenerateforalltypes_Click(object sender, EventArgs e)
        {
            try
            {
                _task_start_time = DateTime.Now;

                _process_for_all = true;

                //process_files_in_background_worker();

                generate_playlist_for_all_media_types();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        private void btngenerate_playlist_json_Click(object sender, EventArgs e)
        {
            try
            {
                _task_start_time = DateTime.Now;

                _process_for_all = false;

                process_files_in_background_worker();
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            this._notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs(ex.Message, TAG));
        }

        void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            this._notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs(ex.Message, TAG));
        }

        //Event handler declaration:
        public void notificationmessageHandler(object sender, notificationmessageEventArgs args)
        {
            try
            {
                /* Handler logic */

                //Invoke(new Action(() =>
                //{

                notificationdto _notificationdto = new notificationdto();

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss");

                String _logtext = Environment.NewLine + "[ " + dateTimenow + " ]   " + args.message;

                _notificationdto._notification_message = _logtext;
                _notificationdto._created_datetime = dateTimenow;
                _notificationdto.TAG = args.TAG;

                Log.WriteToErrorLogFile(new Exception(args.message));

                Utils.LogEventViewer(new Exception(args.message));

                _lstnotificationdto.Add(_notificationdto);

                var _lstmsgdto = from msgdto in _lstnotificationdto
                                 orderby msgdto._created_datetime descending
                                 select msgdto._notification_message;

                String[] _logflippedlines = _lstmsgdto.ToArray();

                txtlog.Lines = _logflippedlines;
                txtlog.ScrollToCaret();

                notifyIconntharene.BalloonTipIcon = ToolTipIcon.Info;
                notifyIconntharene.BalloonTipText = _logtext;
                notifyIconntharene.Text = args.TAG;
                notifyIconntharene.BalloonTipTitle = args.TAG;
                //notifyIconntharene.ShowBalloonTip(2000);
                notifyIconntharene.BalloonTipClicked += new EventHandler(notifyIconntharene_BalloonTipClicked);

                //}));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        void notifyIconntharene_BalloonTipClicked(object sender, EventArgs e)
        {
            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(get_app_info(), TAG));
        }

        //Event handler declaration:
        public void progressBarNotificationHandler(object sender, progressBarNotificationEventArgs args)
        {
            /* Handler logic */
            toolStripProgressBar.Maximum = args.ProgressMaximum;

            if (args.ProgressPercentage == -1)
            {
                toolStripProgressBar.Value = 0;
                lblprogresscounta.Text = string.Empty;
            }
            else
            {
                Invoke(new Action(() =>
                {
                    toolStripProgressBar.PerformStep();

                    lblprogresscounta.Invoke(new Action(() =>
                    {
                        lblprogresscounta.Text = args.ProgressPercentage.ToString() + @"/" + args.ProgressMaximum.ToString();
                    }));

                    lbltotalfilesprocessed.Invoke(new Action(() =>
                    {
                        lbltotalfilesprocessed.Text = "total files processed [ " + _global_proccessed_files_counta.ToString() + " ]";
                    }));

                    lblprogresspercentage.Owner.Invoke(new Action(() =>
                    {
                        var _current_file = double.Parse(args.ProgressPercentage.ToString());
                        var _total_file_counta = double.Parse(args.ProgressMaximum.ToString());

                        var _divisor = ((double)args.ProgressPercentage / args.ProgressMaximum) * 100.0;
                        // msgboxform.Show("_divisor " + _divisor.ToString(), "progress", msgtype.info);
                        var _percentage = int.Parse((Math.Round(_divisor, MidpointRounding.AwayFromZero)).ToString());
                        //msgboxform.Show("ProgressPercentage " + args.ProgressPercentage.ToString() + " ProgressMaximum " + args.ProgressMaximum.ToString() + " percentage " + _percentage.ToString(), "progress", msgtype.info);
                        lblprogresspercentage.Text = _percentage.ToString() + "%";
                    }));

                }));
            }
            //if percentage is equal to maximum we are done reset 
            if (args.ProgressPercentage == args.ProgressMaximum)
            {
                //lblprogresscounta.Visible = false;
                //toolStripProgressBar.Value = 0;
                //lblprogresscounta.Text = string.Empty;
            }
        }

        void get_files_in_folder(string _selected_media_type)
        {
            try
            {
                //variable to hold path template
                string _str_path = @"D:\data\20170628\";

                //if root folder does not exists return
                if (!Directory.Exists(_str_path)) return;

                //get all files in the given folder
                var _arr_files_in_dir = Directory.GetFiles(_str_path, "*.*", SearchOption.AllDirectories);

                //reset the total media files count
                _total_media_files = 0;

                //append the selected media type to the path
                switch (_selected_media_type)
                {
                    case DBContract.movie:
                        //folder with media files
                        _str_path = _str_path + DBContract.movie;
                        //filter for mp4 files that are compatible with browsers
                        _arr_files_in_dir = Directory.GetFiles(_str_path, "*.mp4", SearchOption.AllDirectories);
                        //loop through each file, get  file info and format it into json and append into output file.
                        var _files_count = _arr_files_in_dir.Length;
                        var _iterator_counta = 0;
                        _total_media_files = _files_count;

                        foreach (var _str_file in _arr_files_in_dir)
                        {
                            //current file
                            var _current_file = _str_file;

                            txtsource.Invoke(new System.Action(() =>
                            {
                                txtsource.Text = _current_file;
                            }));

                            //get file information and populate variables
                            FileInfo inf = new FileInfo(_current_file);
                            //size of File in kilobytes
                            strfilesize = inf.Length.ToString();
                            //file name with extension
                            strfilename = inf.Name;
                            //file type is the extension
                            strfiletype = inf.Extension.Split(new[] { "." }, StringSplitOptions.None)[1];

                            //escape quotes with the verbatim operator
                            strQuote = @"""";
                            //file name with no extension
                            strfilenamewithnoextension = Path.GetFileNameWithoutExtension(_current_file);

                            txtdestination.Invoke(new System.Action(() =>
                            {
                                txtdestination.Text = str_file_to_create;
                            }));

                            //files count
                            _files_count = _arr_files_in_dir.Length;
                            //increment the counter
                            _iterator_counta++;
                            //set the global current position counter
                            _current_media_file_pos = _iterator_counta;

                            /*
                             params
                             quotes for json formating
                             filename with no extension
                             file name with extension
                             file size in kilobytes
                             file type being the extension
                             path folder to be created if does not exists
                             output file for json to be created if does not exists
                             */
                            saveplaylistjson(strQuote, strfilenamewithnoextension, strfilename, strfilesize, strfiletype, strpathfolder, str_file_to_create);

                            log_file_info(strfilename, strfilesize, strpathfolder);

                            _progressBarNotificationEventname.Invoke(this, new progressBarNotificationEventArgs(_iterator_counta, _files_count));

                        }
                        break;
                    case DBContract.video:
                        //folder with media files
                        _str_path = _str_path + DBContract.video;
                        //filter for mp4 files that are compatible with browsers
                        _arr_files_in_dir = Directory.GetFiles(_str_path, "*.mp4", SearchOption.AllDirectories);
                        //loop through each file, get  file info and format it into json and append into output file.
                        _files_count = _arr_files_in_dir.Length;
                        _iterator_counta = 0;
                        _total_media_files = _files_count;

                        foreach (var _str_file in _arr_files_in_dir)
                        {
                            //current file
                            var _current_file = _str_file;

                            txtsource.Invoke(new System.Action(() =>
                            {
                                txtsource.Text = _current_file;
                            }));

                            //get file information and populate variables
                            FileInfo inf = new FileInfo(_current_file);
                            //size of File in kilobytes
                            strfilesize = inf.Length.ToString();
                            //file name with extension
                            strfilename = inf.Name;
                            //file type is the extension
                            strfiletype = inf.Extension.Split(new[] { "." }, StringSplitOptions.None)[1];

                            //escape quotes with the verbatim operator
                            strQuote = @"""";
                            //file name with no extension
                            strfilenamewithnoextension = Path.GetFileNameWithoutExtension(_current_file);

                            txtdestination.Invoke(new System.Action(() =>
                            {
                                txtdestination.Text = str_file_to_create;
                            }));

                            //files count
                            _files_count = _arr_files_in_dir.Length;
                            //increment the counter
                            _iterator_counta++;
                            //set the global current position counter
                            _current_media_file_pos = _iterator_counta;

                            /*
                             params
                             quotes for json formating
                             filename with no extension
                             file name with extension
                             file size in kilobytes
                             file type being the extension
                             path folder to be created if does not exists
                             output file for json to be created if does not exists
                             */
                            saveplaylistjson(strQuote, strfilenamewithnoextension, strfilename, strfilesize, strfiletype, strpathfolder, str_file_to_create);

                            log_file_info(strfilename, strfilesize, strpathfolder);

                            _progressBarNotificationEventname.Invoke(this, new progressBarNotificationEventArgs(_iterator_counta, _files_count));

                        }
                        break;
                    case DBContract.series:
                        //folder with media files
                        _str_path = _str_path + DBContract.series;
                        //get all first level folders in series
                        var _dirs_in_series = Directory.GetDirectories(_str_path);
                        //files processed count
                        _iterator_counta = 0;

                        //filter for media files that are compatible with browsers
                        var _mp4_files_in_series_count = Directory.GetFiles(_str_path, "*.mp4", SearchOption.AllDirectories).Length;
                        Console.WriteLine("_mp4_files_in_series_count [ " + _mp4_files_in_series_count + " ]");
                        var _mp3_files_in_series_count = Directory.GetFiles(_str_path, "*.mp3", SearchOption.AllDirectories).Length;
                        Console.WriteLine("_mp3_files_in_series_count [ " + _mp3_files_in_series_count + " ]");
                        var _m4a_files_in_series_count = Directory.GetFiles(_str_path, "*.m4a", SearchOption.AllDirectories).Length;
                        Console.WriteLine("_m4a_files_in_series_count [ " + _m4a_files_in_series_count + " ]");
                        var _total_files_in_series_count = _mp4_files_in_series_count + _mp3_files_in_series_count + _m4a_files_in_series_count;
                        Console.WriteLine("_total_files_in_series_count [ " + _total_files_in_series_count + " ]");

                        //loop through all top level folders in series
                        foreach (var _series_dir in _dirs_in_series)
                        {
                            txtsource.Invoke(new System.Action(() =>
                            {
                                txtsource.Text = @_series_dir;
                            }));

                            Invoke(new System.Action(() =>
                            {
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("working in [ " + _series_dir + " ]", TAG));
                            }));

                            //filter for media files that are compatible with browsers
                            List<string> _mp4_files = Directory.GetFiles(_series_dir, "*.mp4", SearchOption.AllDirectories).ToList();
                            List<string> _mp3_files = Directory.GetFiles(_series_dir, "*.mp3", SearchOption.AllDirectories).ToList();
                            List<string> _m4a_files = Directory.GetFiles(_series_dir, "*.m4a", SearchOption.AllDirectories).ToList();

                            List<string> _media_files_in_series = new List<string>();
                            _media_files_in_series.AddRange(_mp4_files);
                            _media_files_in_series.AddRange(_mp3_files);
                            _media_files_in_series.AddRange(_m4a_files);

                            _arr_files_in_dir = _media_files_in_series.ToArray();

                            //files count in current folder
                            _files_count = _arr_files_in_dir.Length;
                            _total_media_files += _files_count;
                            //split the path to get the last folder name
                            var _dir_names_arr = _series_dir.Split(new[] { "\\" }, StringSplitOptions.None);
                            var _arr_length = _dir_names_arr.Length;
                            var _str_last_dir_name = _dir_names_arr[_arr_length - 1];
                            var _media_folder = _str_last_dir_name;

                            //loop through each file, get  file info and format it into json and append into output file.
                            foreach (var _str_file in _arr_files_in_dir)
                            {
                                //current file
                                var _current_file = _str_file;

                                txtsource.Invoke(new System.Action(() =>
                                {
                                    txtsource.Text = _current_file;
                                }));

                                //get file information and populate variables
                                FileInfo inf = new FileInfo(_current_file);
                                //size of File in kilobytes
                                strfilesize = inf.Length.ToString();
                                //file name with extension
                                strfilename = inf.Name;
                                //file type is the extension
                                strfiletype = inf.Extension.Split(new[] { "." }, StringSplitOptions.None)[1];

                                //escape quotes with the verbatim operator
                                strQuote = @"""";
                                //file name with no extension
                                strfilenamewithnoextension = Path.GetFileNameWithoutExtension(_current_file);

                                txtdestination.Invoke(new System.Action(() =>
                                {
                                    txtdestination.Text = str_file_to_create;
                                }));

                                //files count
                                _files_count = _arr_files_in_dir.Length;
                                //increment the counter
                                _iterator_counta++;
                                //set the global current position counter
                                _current_media_file_pos = _iterator_counta;

                                /*
                                 params
                                 quotes for json formating
                                 filename with no extension
                                 file name with extension
                                 file size in kilobytes
                                 file type being the extension
                                 path folder to be created if does not exists
                                 output file for json to be created if does not exists
                                 */
                                saveseriesplaylistjson(strQuote, strfilenamewithnoextension, strfilename, strfilesize, strfiletype, strpathfolder, str_file_to_create, _media_folder, _total_files_in_series_count);

                                log_file_info(strfilename, strfilesize, @_series_dir);

                                _progressBarNotificationEventname.Invoke(this, new progressBarNotificationEventArgs(_iterator_counta, _total_files_in_series_count));

                                bgWorker.ReportProgress(_files_count);

                            }
                        }
                        break;
                    case DBContract.audio:
                        //folder with media files
                        _str_path = _str_path + DBContract.audio;
                        //filter for mp4 files that are compatible with browsers
                        _arr_files_in_dir = Directory.GetFiles(_str_path, "*.mp3", SearchOption.AllDirectories);
                        //loop through each file, get  file info and format it into json and append into output file.
                        _files_count = _arr_files_in_dir.Length;
                        _iterator_counta = 0;
                        _total_media_files = _files_count;

                        foreach (var _str_file in _arr_files_in_dir)
                        {
                            //current file
                            var _current_file = _str_file;

                            txtsource.Invoke(new System.Action(() =>
                            {
                                txtsource.Text = _current_file;
                            }));

                            //get file information and populate variables
                            FileInfo inf = new FileInfo(_current_file);
                            //size of File in kilobytes
                            strfilesize = inf.Length.ToString();
                            //file name with extension
                            strfilename = inf.Name;
                            //file type is the extension
                            strfiletype = inf.Extension.Split(new[] { "." }, StringSplitOptions.None)[1];

                            //escape quotes with the verbatim operator
                            strQuote = @"""";
                            //file name with no extension
                            strfilenamewithnoextension = Path.GetFileNameWithoutExtension(_current_file);

                            txtdestination.Invoke(new System.Action(() =>
                            {
                                txtdestination.Text = str_file_to_create;
                            }));

                            //files count
                            _files_count = _arr_files_in_dir.Length;
                            //increment the counter
                            _iterator_counta++;
                            //set the global current position counter
                            _current_media_file_pos = _iterator_counta;

                            /*
                             params
                             quotes for json formating
                             filename with no extension
                             file name with extension
                             file size in kilobytes
                             file type being the extension
                             path folder to be created if does not exists
                             output file for json to be created if does not exists
                             */
                            saveplaylistjson(strQuote, strfilenamewithnoextension, strfilename, strfilesize, strfiletype, strpathfolder, str_file_to_create);

                            log_file_info(strfilename, strfilesize, strpathfolder);

                            _progressBarNotificationEventname.Invoke(this, new progressBarNotificationEventArgs(_iterator_counta, _files_count));

                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void log_file_info(string strfilename, string strfilesize, string strpathfolder)
        {
            try
            {
                StringBuilder _sb = new StringBuilder();
                _sb.AppendLine("");
                _sb.AppendLine("strfilename [ " + strfilename + " ]");
                _sb.AppendLine("strfilesize [ " + Utils.formatmediasize(strfilesize) + " ]");
                _sb.AppendLine("strpathfolder [ " + strpathfolder + " ]");
                _sb.AppendLine("hash [ " + generate_hash(strfilename) + " ]");

                Invoke(new System.Action(() =>
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_sb.ToString(), TAG));
                }));

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        /*
         params
         quotes for json formating
         filename with no extension
         file name with extension
         file size in kilobytes
         file type being the extension
         path folder to be created if does not exists
         output file for json to be created if does not exists
         */
        public void saveplaylistjson(String strQuote, String strfilenamewithnoextension, String strfilename, String strfilesize, String strfiletype, String _strpathfolder, String _str_file)
        {
            try
            {

                /*using is syntactic sugar that allows you to guarantee that a resource is cleaned up without needing an explicit try finally block. This means your code will be much cleaner, and you won't leak non-managed resources.*/
                using (FileStream _fileStream = new FileStream(_str_file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    //format the json output
                    StreamWriter sw = new StreamWriter(_fileStream);
                    //if we are at the beggining start with an opening brace to make it a json array
                    if (_current_media_file_pos == 1)
                    {
                        sw.WriteLine("{");
                    }
                    //json object identifier name
                    sw.Write(strQuote + strfilenamewithnoextension + strQuote);
                    //separate key part from value part
                    sw.WriteLine(": ");
                    //begining definition of the object
                    sw.WriteLine("{");
                    //property key
                    sw.Write(strQuote + "medianame" + strQuote);
                    //separate key part from value part
                    sw.Write(": ");
                    //property value, terminate with a comma to insert more properties
                    sw.WriteLine(strQuote + strfilename + strQuote + ",");
                    //property key
                    sw.Write(strQuote + "mediatitle" + strQuote);
                    //separate key part from value part
                    sw.Write(": ");
                    //property value, terminate with a comma to insert more properties
                    sw.WriteLine(strQuote + strfilenamewithnoextension + strQuote + ",");
                    //property key
                    sw.Write(strQuote + "mediasize" + strQuote);
                    //separate key part from value part
                    sw.Write(": ");
                    //property value, terminate with a comma to insert more properties
                    sw.WriteLine(strQuote + strfilesize + strQuote + ",");
                    //property key
                    sw.Write(strQuote + "mediatype" + strQuote);
                    //separate key part from value part
                    sw.Write(": ");
                    //property value, last property terminate with space
                    sw.WriteLine(strQuote + strfiletype + strQuote + "");
                    //if we are at the end terminate with a closing brace to close the object and array
                    if (_current_media_file_pos == _total_media_files)
                    {
                        //close the object with no comma
                        sw.WriteLine("}");
                        //close the array with no comma
                        sw.WriteLine("}");
                    }
                    else
                    {
                        //close the object, terminate with a comma to insert more objects
                        sw.WriteLine("},");
                    }

                    //sw.Close(); This will close ms and when we try to use ms later it will cause an exception
                    sw.Flush();

                    _global_proccessed_files_counta++;

                    playlist_dto _playlist_dto = new playlist_dto
                    {
                        media_name = strfilename,
                        media_title = strfilenamewithnoextension,
                        media_size = strfilesize,
                        media_status = "Active",
                        media_type = strfiletype,
                        created_date = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt")
                    };

                    bool created_in_db = save_playlist_in_databases(_playlist_dto);
                    Console.WriteLine(created_in_db);
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
            }

        }

        /*
         params
         quotes for json formating
         filename with no extension
         file name with extension
         file size in kilobytes
         file type being the extension
         path folder to be created if does not exists
         output file for json to be created if does not exists
         */
        public void saveseriesplaylistjson(String strQuote, String strfilenamewithnoextension, String strfilename, String strfilesize, String strfiletype, String _strpathfolder, String _str_file, String strmediafoldername, int total_files_count)
        {
            try
            {

                /*using is syntactic sugar that allows you to guarantee that a resource is cleaned up without needing an explicit try finally block. This means your code will be much cleaner, and you won't leak non-managed resources.*/
                using (FileStream _fileStream = new FileStream(_str_file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    //format the json output
                    StreamWriter sw = new StreamWriter(_fileStream);
                    //if we are at the beggining start with an opening brace to make it a json array
                    if (_current_media_file_pos == 1)
                    {
                        sw.WriteLine("{");
                    }
                    //json object identifier name
                    sw.Write(strQuote + strfilenamewithnoextension + strQuote);
                    //separate key part from value part
                    sw.WriteLine(": ");
                    //begining definition of the object
                    sw.WriteLine("{");
                    //property key
                    sw.Write(strQuote + "medianame" + strQuote);
                    //separate key part from value part
                    sw.Write(": ");
                    //property value, terminate with a comma to insert more properties
                    sw.WriteLine(strQuote + strfilename + strQuote + ",");
                    //property key
                    sw.Write(strQuote + "mediatitle" + strQuote);
                    //separate key part from value part
                    sw.Write(": ");
                    //property value, terminate with a comma to insert more properties
                    sw.WriteLine(strQuote + strfilenamewithnoextension + strQuote + ",");
                    //property key
                    sw.Write(strQuote + "mediasize" + strQuote);
                    //separate key part from value part
                    sw.Write(": ");
                    //property value, terminate with a comma to insert more properties
                    sw.WriteLine(strQuote + strfilesize + strQuote + ",");
                    //property key
                    sw.Write(strQuote + "mediatype" + strQuote);
                    //separate key part from value part
                    sw.Write(": ");
                    //property value, last property terminate with space
                    sw.WriteLine(strQuote + strfiletype + strQuote + ",");
                    //property key
                    sw.Write(strQuote + "mediafoldername" + strQuote);
                    //separate key part from value part
                    sw.Write(": ");
                    //property value, last property terminate with space
                    sw.WriteLine(strQuote + strmediafoldername + strQuote + "");
                    //if we are at the end terminate with a closing brace to close the object and array
                    if (_current_media_file_pos == total_files_count)
                    {
                        //close the object with no comma
                        sw.WriteLine("}");
                        //close the array with no comma
                        sw.WriteLine("}");
                    }
                    else
                    {
                        //close the object, terminate with a comma to insert more objects
                        sw.WriteLine("},");
                    }

                    //sw.Close(); This will close ms and when we try to use ms later it will cause an exception
                    sw.Flush();

                    _global_proccessed_files_counta++;

                    playlist_dto _playlist_dto = new playlist_dto
                    {
                        media_name = strfilename,
                        media_title = strfilenamewithnoextension,
                        media_size = strfilesize,
                        media_status = "Active",
                        media_type = strfiletype,
                        created_date = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt")
                    };

                    bool created_in_db = save_playlist_in_databases(_playlist_dto);
                    Console.WriteLine(created_in_db);
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
            }

        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            if (_is_backgroud_worker_running)
            {

                DialogResult _dialogresult = msgboxform.Show("BackgroundWorker running." + Environment.NewLine + "are you sure you don't want to wait for the worker to finish current task???..", "BackgroundWorker", msgtype.error);

                if (_dialogresult == DialogResult.OK)
                {
                    Application.Exit();
                }
                else
                {
                    return;
                }

            }
            else
            {
                Application.Exit();
            }
        }


        void lblprogresscounta_TextChanged(object sender, EventArgs e)
        {
            var evt = e;
            var snd = sender;
        }

        private void cbomediatype_SelectedIndexChanged(object sender, EventArgs e)
        {

            var _selected_media_type = cbomediatype.Text;
            var _source_path = "";
            var _source_destination = "";

            switch (_selected_media_type)
            {
                case DBContract.audio:
                    _source_path = @"D:\data\20170628\audio";
                    _source_destination = @"D:\wakxpx\csharp\visualstudio\nyaxplaylistapp\nyaxplaylistapp\bin\Debug\audio";
                    break;
                case DBContract.movie:
                    _source_path = @"D:\data\20170628\movie";
                    _source_destination = @"D:\wakxpx\csharp\visualstudio\nyaxplaylistapp\nyaxplaylistapp\bin\Debug\movie";
                    break;
                case DBContract.video:
                    _source_path = @"D:\data\20170628\video";
                    _source_destination = @"D:\wakxpx\csharp\visualstudio\nyaxplaylistapp\nyaxplaylistapp\bin\Debug\video";
                    break;
                case DBContract.series:
                    _source_path = @"D:\data\20170628\series";
                    _source_destination = @"D:\wakxpx\csharp\visualstudio\nyaxplaylistapp\nyaxplaylistapp\bin\Debug\series";
                    break;
            }
            txtsource.Text = _source_path;
            txtdestination.Text = _source_destination;
        }

        private void notifyIconntharene_Click(object sender, EventArgs e)
        {
            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(get_app_info(), TAG));
        }

        private void notifyIconntharene_DoubleClick(object sender, EventArgs e)
        {
            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(get_app_info(), TAG));
        }

        string get_app_info()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine("");
            _sb.AppendLine("ProductVersion: " + Application.ProductVersion);
            _sb.AppendLine("CompanyName: " + Application.CompanyName);
            _sb.AppendLine("ProductName: " + Application.ProductName);
            return _sb.ToString();
        }

        void process_files_in_background_worker()
        {
            if (_is_backgroud_worker_running)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("BackgroundWorker already running." + Environment.NewLine + "please wait for the worker to finish current task...", TAG));

                MessageBox.Show("BackgroundWorker already running." + Environment.NewLine + "please wait for the worker to finish current task...", "BackgroundWorker", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            //reset progress controls
            toolStripProgressBar.Value = 0;
            lblprogresscounta.Text = string.Empty;
            lbltotalfilesprocessed.Text = string.Empty;
            lblprogresspercentage.Text = string.Empty;
            _global_proccessed_files_counta = 0;

            //This allows the BackgroundWorker to be cancelled in between tasks
            bgWorker.WorkerSupportsCancellation = true;
            //This allows the worker to report progress between completion of tasks...
            //this must also be used in conjunction with the ProgressChanged event
            bgWorker.WorkerReportsProgress = true;

            //this assigns event handlers for the backgroundWorker
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.RunWorkerCompleted += bgWorker_WorkComplete;
            /* When you wish to have something occur when a change in progress
                occurs, (like the completion of a specific task) the "ProgressChanged"
                event handler is used. Note that ProgressChanged events may be invoked
                by calls to bgWorker.ReportProgress(...) only if bgWorker.WorkerReportsProgress
                is set to true. */
            bgWorker.ProgressChanged += bgWorker_ProgressChanged;

            //tell the backgroundWorker to raise the "DoWork" event, thus starting it.
            //Check to make sure the background worker is not already running.
            if (!bgWorker.IsBusy)
                bgWorker.RunWorkerAsync();

            // Control returns here before TimeintensiveMethod returns
            Console.WriteLine("You can read this while TimeintensiveMethod is still running.");
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //this is the method that the backgroundworker will perform on in the background thread without blocking the UI.
            /* One thing to note! A try catch is not necessary as any exceptions will terminate the
            backgroundWorker and report
            the error to the "RunWorkerCompleted" event */

            _is_backgroud_worker_running = true;

            generate_playlist_for_all_media_types();

            //e.Result = TimeintensiveMethod(e.Argument);
        }

        /*This is how the ProgressChanged event method signature looks like:*/
        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Things to be done when a progress change has been reported
            /* The ProgressChangedEventArgs gives access to a percentage,
            allowing for easy reporting of how far along a process is*/
            int progress = e.ProgressPercentage;
        }

        private void bgWorker_WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            // we can access possible return values of our Method via the Parameter e
            Console.WriteLine("Count: " + e.Result);

            //worker_timer.Stop(); // Stop the timer

            stopWatch.Stop();

            //_global_time_lapsed_counta = 0;

            _is_backgroud_worker_running = false;

            _task_end_time = DateTime.Now;

            var _time_lapsed = _task_end_time.Subtract(_task_start_time);

            //e.Error will contain any exceptions caught by the backgroundWorker
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "BackgroundWorker", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(e.Error.Message, TAG));
            }
            else
            {
                MessageBox.Show("Task Complete!" + Environment.NewLine + "Task took [ " + _time_lapsed + " ]", "BackgroundWorker", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Task Complete!", TAG));
            }
        }

        void generate_playlist_for_all_media_types()
        {
            if (!_process_for_all)
            {
                //get the selected media type
                var _selected_media_type = string.Empty;

                cbomediatype.Invoke(new System.Action(() =>
                {
                    _selected_media_type = cbomediatype.Text;
                }));

                //log the selected media type
                Invoke(new System.Action(() =>
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("creating playlist for media [ " + _selected_media_type + " ]", TAG));
                }));

                switch (_selected_media_type)
                {
                    case DBContract.movie:
                        //path folder
                        strpathfolder = _apppath + @"\movie\";
                        //output file to append playlist item
                        str_file_to_create = @"movie.json";
                        break;
                    case DBContract.video:
                        //path folder
                        strpathfolder = _apppath + @"\video\";
                        //output file to append playlist item
                        str_file_to_create = @"video.json";
                        break;
                    case DBContract.audio:
                        //path folder
                        strpathfolder = _apppath + @"\audio\";
                        //output file to append playlist item
                        str_file_to_create = @"audio.json";
                        break;
                    case DBContract.series:
                        //path folder
                        strpathfolder = _apppath + @"\series\";
                        //output file to append playlist item
                        str_file_to_create = @"series.json";
                        break;
                }
                //combine the folder path with the file name to contsruct a readable url
                str_file_to_create = strpathfolder + str_file_to_create;
                //create path if not exists
                if (!Directory.Exists(strpathfolder))
                    Directory.CreateDirectory(strpathfolder);
                //if output file exists delete then create new
                if (File.Exists(str_file_to_create))
                {
                    File.Delete(str_file_to_create);
                    File.Create(str_file_to_create).Close();
                }

                get_files_in_folder(_selected_media_type);
            }
            else
            {
                var _media_types_arr = new[] { 
                    "series", 
                    "movie",
                    "video",
                    "audio"
                };

                foreach (var _mediatype in _media_types_arr)
                {
                    //reset progress controls
                    toolStripProgressBar.Value = 0;
                    lblprogresscounta.Text = string.Empty;

                    //get the current media type
                    var _selected_media_type = _mediatype;

                    cbomediatype.Invoke(new Action(() =>
                    {
                        //set the current media type in combo box
                        cbomediatype.SelectedItem = _selected_media_type;
                    }));

                    Invoke(new Action(() =>
                    {
                        //log the selected media type
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("creating playlist for media [ " + _selected_media_type + " ]", TAG));
                    }));

                    //format destination path accordingly
                    switch (_selected_media_type)
                    {
                        case DBContract.movie:
                            //path folder
                            strpathfolder = _apppath + @"\movie\";
                            //output file to append playlist item
                            str_file_to_create = @"movie.json";
                            break;
                        case DBContract.video:
                            //path folder
                            strpathfolder = _apppath + @"\video\";
                            //output file to append playlist item
                            str_file_to_create = @"video.json";
                            break;
                        case DBContract.audio:
                            //path folder
                            strpathfolder = _apppath + @"\audio\";
                            //output file to append playlist item
                            str_file_to_create = @"audio.json";
                            break;
                        case DBContract.series:
                            //path folder
                            strpathfolder = _apppath + @"\series\";
                            //output file to append playlist item
                            str_file_to_create = @"series.json";
                            break;
                    }
                    //combine the folder path with the file name to contsruct a readable url
                    str_file_to_create = strpathfolder + str_file_to_create;
                    //create path if not exists
                    if (!Directory.Exists(strpathfolder))
                        Directory.CreateDirectory(strpathfolder);
                    //if output file exists delete then create new
                    if (File.Exists(str_file_to_create))
                    {
                        File.Delete(str_file_to_create);
                        File.Create(str_file_to_create).Close();
                    }

                    get_files_in_folder(_selected_media_type);
                }
            }
        }

        string secondstominutesandhours(double secs)
        {
            try
            {
                var hr = ('0' + Math.Floor(secs / 3600) % 60).ToString().Substring(-2);
                var min = ('0' + Math.Floor(secs / 60) % 60).ToString().Substring(-2);
                var sec = ('0' + Math.Floor(secs % 60)).ToString().Substring(-2);
                var _formatedtime = hr + ':' + min + ':' + sec;
                return _formatedtime;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return "0";
            }
        }

        private void btncancelworker_Click(object sender, EventArgs e)
        {
            bgWorker.CancelAsync();
        }

        string generate_hash(string source)
        {
            StringBuilder _sb = new StringBuilder();

            // Creates an instance of the default implementation of the MD5 hash algorithm.
            using (var md5Hash = MD5.Create())
            {
                // Byte array representation of source string
                var sourceBytes = Encoding.UTF8.GetBytes(source);
                // Generate hash value(Byte Array) for input data
                var hashBytes = md5Hash.ComputeHash(sourceBytes);
                // Convert hash byte array to string
                var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                // Output the MD5 hash
                _sb.AppendLine("MD5 - " + hash);
            }
            using (SHA1 sha1Hash = SHA1.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                _sb.AppendLine("SHA1 - " + hash);
            }
            using (SHA256 sha256Hash = SHA256.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
                byte[] hashBytes = sha256Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                _sb.AppendLine("SHA256 - " + hash);
            }
            using (SHA384 sha384Hash = SHA384.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
                byte[] hashBytes = sha384Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                _sb.AppendLine("SHA384 - " + hash);
            }
            using (SHA512 sha512Hash = SHA512.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
                byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                _sb.AppendLine("SHA512 - " + hash);
            }
            return _sb.ToString();
        }

        private bool save_playlist_in_databases(playlist_dto _playlist_dto)
        {
            bool created_in_db = false;
            try
            {
                responsedto _mssql_responsedto = playlist_utilz_singleton.getInstance(_notificationmessageEventname).createplaylistindatabase(_playlist_dto);

                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_mssql_responsedto.responsesuccessmessage, TAG));

                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_mssql_responsedto.responseerrormessage, TAG));

                created_in_db = _mssql_responsedto.isresponseresultsuccessful;
                return created_in_db;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                created_in_db = false;
                return created_in_db;
            }
        }


        private void btnreports_Click(object sender, EventArgs e)
        {
            reports_form reports_form = new reports_form(_notificationmessageEventname) { Owner = this };
            reports_form.Show();
        }

        private void elapsed_timer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                _TimeCounter++;
                DateTime nowDate = DateTime.Now;
                TimeSpan t = nowDate - _startDate;
                lbltimelapsed.Text = string.Format("{0} : {1} : {2} : {3}", t.Days, t.Hours, t.Minutes, t.Seconds);
            }
            catch (Exception ex)
            {
                //this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Console.WriteLine(ex.ToString());
            }
        }
        private void running_timer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                //current running time
                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd(dddd)-MM(MMMM)-yyyy HH:mm:ss");
                lbldatetime.Text = dateTimenow;
            }
            catch (Exception ex)
            {
                //this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Console.WriteLine(ex.ToString());
            }
        }




    }
}
