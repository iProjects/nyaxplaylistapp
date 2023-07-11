using nyaxplaylistapp_dal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace nyaxplaylistapp_ui.reports
{
    public class excelbuilder
    {
        //private attributes 
        reportmodel _reportmodel;
        CreateExcelDoc document;
        string Message;
        string sFileExcel;
        public string TAG;

        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;

        //constructor
        public excelbuilder(reportmodel reportmodel, string FileName, EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            TAG = this.GetType().Name;

            sFileExcel = FileName;
            _notificationmessageEventname = notificationmessageEventname;

            if (reportmodel == null)
                throw new ArgumentNullException("ReportModel is null");
            _reportmodel = reportmodel;

        }

        public string GetExcel()
        {
            BuildExcel();
            document.Save(sFileExcel);
            return sFileExcel;
        }

        /*Build the document **/
        private void BuildExcel()
        {
            // step 1: creation of a document-object
            document = new CreateExcelDoc();

            try
            {
                //Add  Header 
                int row = 1; int col = 1;
                AddDocHeader(ref row, ref col);

                //Add  Body
                AddDocBody(ref row, ref col);

                //Add Footer
                AddDocFooter(ref row, ref col);

            }
            catch (IOException ioe)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ioe.Message, TAG));
                this.Message = ioe.Message;
                Log.WriteToErrorLogFile(ioe);
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                Log.WriteToErrorLogFile(ex);
            }

        }

        /*Build the document**/

        private void AddDocHeader(ref int row, ref int col)
        {
            //createHeaders(int row, int col, string htext, string cell1, string cell2, int mergeColumns, string b, bool font, int size, string fcolor)

            col = 2; row = 1;
            string cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, _reportmodel.reportname, cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            row++;
            cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, "Printed on: " + _reportmodel.printedon.ToString("dd-dddd-MMMM-yyyy"), cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("created excel document header", TAG));
        }


        private void AddDocBody(ref int row, ref int col)
        {
            //Add table headers
            AddBodytableHeaders(ref  row, ref  col);

            //Add table detail
            foreach (var d in _reportmodel.playlist)
            {
                AddBodyTableDetail(d, ref  row, ref  col);
            }

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("created excel table details", TAG));

            //Add table footer
            AddDocBodyTableTotals(ref  row, ref  col);

        }

        //table headers
        private void AddBodytableHeaders(ref int row, ref int col)
        {
            //createHeaders(int row, int col, string htext, string cell1, string cell2, int mergeColumns, string b, bool font, int size, string fcolor)


            //row 1
            row = row + 2; col = 1;
            string cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, "ID", cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            col++;
            cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, "NAME", cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            col++;
            cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, "SIZE", cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            col++;
            cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, "TYPE", cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            col++;
            cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, "CREATED DATE", cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("created excel table header", TAG));
        }

        //table details
        private void AddBodyTableDetail(playlist_dto dto, ref int row, ref int col)
        {

            row++; col = 1;
            string cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, dto.media_id, cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            col++;
            cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, dto.media_name, cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            col++;
            cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, Utils.formatmediasize(dto.media_size), cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            col++;
            cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, dto.media_type, cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            col++;
            cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, dto.created_date, cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

        }

        //table footer
        private void AddDocBodyTableTotals(ref int row, ref int col)
        {
            row++; col = 1;
            string cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, "Total size", cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            col++;
            cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, "", cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            col++;
            cellrangeaddr1 = document.IntAlpha(col) + row;
            document.createHeaders(row, col, Utils.formatmediasize(_reportmodel.total_size.ToString()), cellrangeaddr1, cellrangeaddr1, 0, "WHITE", true, 10, "n");

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("created excel table totals", TAG));
        }

        //document footer
        private void AddDocFooter(ref int row, ref int col)
        {

        }







    }
}
