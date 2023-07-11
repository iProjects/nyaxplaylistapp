using CrystalDecisions.CrystalReports.Engine;
using nyaxplaylistapp_dal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nyaxplaylistapp_ui.reports
{
    public partial class crystal_reports_form : Form
    {
        public string TAG;
        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;

        public crystal_reports_form(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            InitializeComponent();
            TAG = this.GetType().Name;

            _notificationmessageEventname = notificationmessageEventname;
        }

        private void crystal_reports_form_Load(object sender, EventArgs e)
        {
            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("loaded crystal_reports_form", TAG));
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void loadReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Text = "printing...";
            loadReportToolStripMenuItem.Text = "printing...";
            print_report();
            loadReportToolStripMenuItem.Text = "print report";
            this.Text = "crystal reports";
        }
        private void print_report()
        {
            try
            {
                DataSet dataset = new DataSet();
                dataset = new reportsDataSet();
                DataTable reportsDataTable = dataset.Tables["reportsDataTable"];
                DataRow row;
                List<playlist_dto> lst_records = playlist_utilz_singleton.getInstance(_notificationmessageEventname).getallrecordsfromdb();
                int total_records = lst_records.Count;
                lst_records = lst_records.Take(10).ToList();
                int i;
                int count = 0;

                for (i = 0; i < lst_records.Count(); i++)
                {
                    row = reportsDataTable.NewRow();

                    row["Id"] = lst_records[i].media_id;
                    row["Name"] = lst_records[i].media_name;
                    row["Size"] = lst_records[i].media_size;//Utils.formatmediasize(lst_records[i].media_size);
                    row["Type"] = lst_records[i].media_type;
                    row["Created Date"] = lst_records[i].created_date;
                    row["Status"] = lst_records[i].media_status;

                    reportsDataTable.Rows.Add(row);
                    count++;
                }

                groupBox1.Text = count.ToString();

                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("fetched [ " + count.ToString() + " ] records for printing...", TAG));

                ReportDocument cryRpt = new ReportDocument();
                string datetime = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss_tt");
                string file_name = "Crystal_Report_" + datetime + ".rpt";
                string report_path = Path.Combine(Utils.get_application_path(), "reports");
                string report_file = Path.Combine(report_path, "CrystalReport.rpt");

                if (!File.Exists(report_file))
                    File.Create(report_file).Close();

                cryRpt.Load(report_file);
                cryRpt.SetDataSource(reportsDataTable);
                crystalReportViewer.ReportSource = cryRpt;
                crystalReportViewer.Refresh();

                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("created crystal report sucessfully...", TAG));

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
            }
        }




    }
}
