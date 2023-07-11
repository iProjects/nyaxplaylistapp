using iTextSharp.text;
using nyaxplaylistapp_dal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VVX;

namespace nyaxplaylistapp_ui.reports
{
    public partial class pdfviewer : Form
    {
        private string msAppName = "nyax playlist report.....";
        pdfgen _pdfgen;
        string current_file_name = "";
        string msFolder = "";
        string _resourcesPath = null;
        public string TAG;

        // Timers namespace rather than Threading
        System.Timers.Timer elapsed_timer = new System.Timers.Timer(); // Doesn't require any args
        private int _TimeCounter = 0;
        DateTime _startDate = DateTime.Now;

        public List<notificationdto> _lstnotificationdto = new List<notificationdto>();

        ////delegate
        //public delegate void ReportsProcessCompleteEventHandler(object sender, ReportsProcessCompleteEventArg e);
        ////event
        //public event ReportsProcessCompleteEventHandler OnCompleteReportsProcess;
        ////delegate
        //public delegate void ReportsEngineCompleteEventHandler(object sender, ReportsEngineCompleteEventArg e);
        ////event
        //public event ReportsEngineCompleteEventHandler OnCompleteReportsEngine;

        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        private event EventHandler<notificationmessageEventArgs> _parent_notificationmessageEventname;

        public pdfviewer(EventHandler<notificationmessageEventArgs> parent_notificationmessageEventname)
        {
            InitializeComponent();

            TAG = this.GetType().Name;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);

            //Subscribing to the event: 
            //Dynamically:
            //EventName += HandlerName;
            _notificationmessageEventname += notificationmessageHandler;
            _parent_notificationmessageEventname = parent_notificationmessageEventname;

            //--- init the folder in which generated PDF's will be saved.
            msFolder = Application.ExecutablePath;
            int n = msFolder.LastIndexOf(@"\");
            msFolder = msFolder.Substring(0, n + 1);

            SetResourcePath();
            _pdfgen = new pdfgen(_resourcesPath, _notificationmessageEventname);

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("finished form initialization", TAG));

        }
        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            this._notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs(ex.Message, TAG));
        }

        private void ThreadException(object sender, ThreadExceptionEventArgs e)
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

                _parent_notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(args.message, TAG));

                Log.WriteToErrorLogFile(new Exception(args.message));

                _lstnotificationdto.Add(_notificationdto);

                var _lstmsgdto = from msgdto in _lstnotificationdto
                                 orderby msgdto._created_datetime descending
                                 select msgdto._notification_message;

                String[] _logflippedlines = _lstmsgdto.ToArray();

                txtlog.Lines = _logflippedlines;
                txtlog.ScrollToCaret();

                //}));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private void pdfviewer_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeControls();

                RefreshGrid();

                NavigateToHomePage();

                //initialize current running time timer
                elapsed_timer.Interval = 1000;
                elapsed_timer.Elapsed += elapsed_timer_Elapsed; // Uses an event instead of a delegate
                elapsed_timer.Start(); // Start the timer

                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("finished form load", TAG));

            }
            catch (Exception ex)
            {
                Utils.ShowError(ex);
            }
        }

        private void pdfreportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            show_playlist_report("pdf", sender, e);
        }
        private void excelreportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            show_playlist_report("xls", sender, e);
        }
        private void exitToolStripMenuItem_Click_(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            NavigateToHomePage();
        }
        private void NavigateToHomePage()
        {
            try
            {
                string dir = pathlookup("about");
                string sPath = Utils.build_file_path(dir, "about.html");

                for (int i = 0; i < 6; i++)
                {
                    if (VVX.File.Exists(sPath))
                    {
                        FileInfo fi = new FileInfo(sPath);
                        this.pdfwebBrowser.Navigate(fi.FullName);
                        break;
                    }
                    sPath = @"..\" + sPath;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowError(ex);
            }
        }

        #region "General Purpose Helpers for this Form"
        //************************************************************
        /// <summary>
        /// Refreshes the window's Caption/Title bar
        /// </summary>
        private void DoUpdateCaption()
        {
            try
            {
                this.Text = this.msAppName;

                if (this.current_file_name.Length == 0)
                {
                    this.Text += "<...no PDF file created...>";
                }
                else
                {
                    FileInfo fi = new FileInfo(GetURI(this.current_file_name));
                    this.Text += @"...\" + fi.Name;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowError(ex);
            }
        }
        private void DoPreProcess(object sender, EventArgs e)
        {
            string msg = "processing report...";
            this.lblstatusinfo.Text = msg;
            this.Text = msg;
        }
        private void DoPostProcess(object sender, EventArgs e)
        {
            try
            {
                string dir = pathlookup("reports");
                string sRet = Utils.build_file_path(dir, current_file_name);
                int pdfCount = Directory.GetFiles(dir, "*.pdf", SearchOption.TopDirectoryOnly).Length;
                int excelCount = Directory.GetFiles(dir, "*.xls", SearchOption.TopDirectoryOnly).Length;
                int _totalFiles = pdfCount + excelCount;
                this.lblstatusinfo.Text = current_file_name.ToString() + "     [  " + _totalFiles.ToString() + "  ] ";
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
            }
        }
        private string GetURI(string sFile)
        {
            string sRet;
            try
            {
                string dir = pathlookup("reports");
                sRet = Utils.build_file_path(dir, sFile);
                //check if directory exists.
                if (!Directory.Exists(dir))
                {
                    sRet = msFolder + "output\\" + sFile;
                }
            }
            catch (Exception e)
            {
                Utils.ShowError(e);
                sRet = msFolder + "output\\" + sFile;
            }
            return sRet;
        }
        private void SetResourcePath()
        {
            string sRet = string.Empty;
            try
            {
                string dir = pathlookup("resources");
                if (!Directory.Exists(dir))
                {
                    sRet = Utils.build_file_path(msFolder, "resources");
                }
                else
                {
                    sRet = dir;
                }

                this._resourcesPath = sRet;
            }
            catch (Exception e)
            {
                Utils.ShowError(e);
                this._resourcesPath = Utils.build_file_path(msFolder, "resources");
            }
        }
        public string pathlookup(string folder)
        {
            try
            {
                string app_dir = Utils.get_application_path();
                var dir = Path.Combine(app_dir, folder);


                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                return dir;

            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile(ex);
                return null;
            }
        }
        private void DoShowPDF(string sFilePDF)
        {
            this.DoUpdateCaption();
            this.pdfwebBrowser.Navigate(GetURI(sFilePDF));
        }
        #endregion "General Purpose Helpers for this Form"


        private void show_playlist_report(string app, object sender, EventArgs e)
        {
            try
            {
                //create model
                reportbuilder reportbuilder = new reportbuilder(_notificationmessageEventname);
                reportmodel reportmodel = reportbuilder.GetReport();

                DoPreProcess(sender, e);
                if ("pdf".Equals(app.ToLower()))
                {
                    current_file_name = "playlist " + DateTime.Now.ToString("dd_MM_yyyy HH_mm_ss_tt") + ".pdf";
                }
                else
                {
                    current_file_name = "playlist " + DateTime.Now.ToString("dd_MM_yyyy HH_mm_ss_tt") + ".xlsx";
                }

                if (_pdfgen.show_playlist_report(app, reportmodel, GetURI(current_file_name)))
                {
                    DoShowPDF(current_file_name);
                }
                this.DoPostProcess(sender, e);
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
            }
        }

        private void RefreshGrid()
        {
            try
            {
                //bindingSourcePayroll.DataSource = openPayrolls;
                //dataGridViewPayroll.DataSource = bindingSourcePayroll;
                //groupBox3.Text = bindingSourcePayroll.Count.ToString();                 
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
            }
        }
        private void InitializeControls()
        {
            try
            {
                string app_dir = Utils.get_application_path();
                var resources_path = Path.Combine(app_dir, "resources");

                if (!Directory.Exists(resources_path))
                {
                    Directory.CreateDirectory(resources_path);
                }

                string reports_dir = Path.Combine(app_dir, "reports");

                if (!Directory.Exists(reports_dir))
                {
                    Directory.CreateDirectory(reports_dir);
                }

                string output_dir = Path.Combine(app_dir, "output");

                if (!Directory.Exists(output_dir))
                {
                    Directory.CreateDirectory(output_dir);
                }

                lblstatusinfo.Text = string.Empty;
                lbltimelapsed.Text = string.Empty;

                //dataGridViewPayroll.AutoGenerateColumns = false;
                //this.dataGridViewPayroll.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                //cbYr.DataSource = de.GetPayrollYears();
                //cbYr.DisplayMember = "Year";

                //dataGridViewPayrollItem.AutoGenerateColumns = false;
                //this.dataGridViewPayrollItem.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                //bindingSourcePayrollItem.DataSource = de.GetActivePayrollItems();
                //dataGridViewPayrollItem.DataSource = bindingSourcePayrollItem;
                //groupBox2.Text = bindingSourcePayrollItem.Count.ToString();

                //cbopayrollproducts.SelectedIndex = 0;

                //dataGridViewEmployers.AutoGenerateColumns = false;
                //this.dataGridViewEmployers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                //bindingSourceEmployers.DataSource = rep.GetAllActiveEmployers();
                //dataGridViewEmployers.DataSource = bindingSourceEmployers;
                //groupBox4.Text = bindingSourceEmployers.Count.ToString();
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
            }
        }

        private void lblstatusinfo_Click(object sender, EventArgs e)
        {
            lblstatusinfo_DoubleClick(sender, e);
        }

        private void lblstatusinfo_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                string app_dir = Utils.get_application_path();

                string reports_dir = Path.Combine(app_dir, "reports");
                string file_name = current_file_name;

                if (Directory.Exists(reports_dir))
                {
                    string file_path = Utils.build_file_path(reports_dir, file_name);
                    if (VVX.File.Exists(reports_dir))
                    {
                        string args = string.Format("/e, /select, \"{0}\"", file_path);
                        ProcessStartInfo info = new ProcessStartInfo();
                        info.FileName = "explorer";
                        info.Arguments = args;
                        Process.Start(info);

                    }
                }
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
            }
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










    }
}
