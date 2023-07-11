using iTextSharp.text;
using iTextSharp.text.pdf;
using nyaxplaylistapp_dal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace nyaxplaylistapp_ui.reports
{
    public class pdfbuilder
    {
        public reportmodel _reportmodel;
        Document document;
        string Message;
        string sFilePDF;
        public string TAG;

        //DEFINED fONTS
        Font hFont1 = new Font(Font.TIMES_ROMAN, 12, Font.BOLD);
        Font hFont2 = new Font(Font.TIMES_ROMAN, 10, Font.BOLD);
        Font bFont1 = new Font(Font.TIMES_ROMAN, 10, Font.NORMAL);//body 
        Font tHFont = new Font(Font.TIMES_ROMAN, 8, Font.BOLD); //table Header
        Font tHfont1 = new Font(Font.TIMES_ROMAN, 9, Font.BOLD);//TABLE HEADER
        Font helv8Font = new Font(Font.HELVETICA, 8, Font.NORMAL);//table cell
        Font rms6Normal = new Font(Font.TIMES_ROMAN, 6, Font.NORMAL);
        Font rms8Normal = new Font(Font.TIMES_ROMAN, 8, Font.NORMAL);
        Font rms6Bold = new Font(Font.TIMES_ROMAN, 6, Font.BOLD);
        Font rms8Bold = new Font(Font.TIMES_ROMAN, 8, Font.BOLD);
        Font rms10Normal = new Font(Font.HELVETICA, 10, Font.NORMAL);

        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;

        //constructor
        public pdfbuilder(reportmodel reportmodel, string FileName, EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            TAG = this.GetType().Name;

            sFilePDF = FileName;
            _notificationmessageEventname = notificationmessageEventname;

            if (reportmodel == null)
                throw new ArgumentNullException("ReportModel is null");
            _reportmodel = reportmodel;

        }
        public string GetPDF()
        {
            try
            {
                BuildPDF();
                return sFilePDF;
            }
            catch (Exception ex)
            {
                Utils.ShowError(ex);
                return null;
            }
        }
        /*Build the document **/
        private void BuildPDF()
        {

            try
            {
                //step 1 creation of the document
                document = new Document(PageSize.A4.Rotate());

                // step 2: we create a writer that listens to the document
                PdfWriter.GetInstance(document, new FileStream(sFilePDF, FileMode.Create));

                //open the document
                document.Open();

                //add header
                AddDocHeader();

                //add body
                AddDocBody();

                //add footer
                AddDocFooter();

                //close the document
                document.Close();
            }
            catch (DocumentException de)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(de.ToString(), TAG));
                this.Message = de.Message;
                Log.WriteToErrorLogFile(de);
            }
            catch (IOException ioe)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ioe.ToString(), TAG));
                this.Message = ioe.Message;
                Log.WriteToErrorLogFile(ioe);
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile(ex);
            }
        }

        //document header
        private void AddDocHeader()
        {

            Table _table = new Table(5);
            _table.WidthPercentage = 100;
            _table.Padding = 1;
            _table.Spacing = 1;
            _table.Border = Table.NO_BORDER;

            Cell reportNameCell = new Cell(new Phrase(_reportmodel.reportname, new Font(Font.TIMES_ROMAN, 15, Font.BOLD | Font.UNDERLINE, Color.BLACK)));
            reportNameCell.HorizontalAlignment = Cell.ALIGN_CENTER;
            reportNameCell.Colspan = 5;
            reportNameCell.Border = Cell.NO_BORDER;
            _table.AddCell(reportNameCell);

            Cell PrintedonCell = new Cell(new Phrase("Printed on: " + _reportmodel.printedon.ToString("dd-dddd-MMMM-yyyy"), hFont2));
            PrintedonCell.HorizontalAlignment = Cell.ALIGN_LEFT;
            PrintedonCell.Colspan = 4;
            PrintedonCell.Border = Cell.NO_BORDER;
            _table.AddCell(PrintedonCell);

            //create the logo
            pdfgen pdfgen = new pdfgen();
            Image img0 = pdfgen.DoGetImageFile(_reportmodel.logo);
            img0.Alignment = Image.ALIGN_MIDDLE;
            Cell logoCell = new Cell(img0);
            logoCell.HorizontalAlignment = Cell.ALIGN_LEFT;
            logoCell.Border = Cell.NO_BORDER;
            logoCell.Add(new Phrase("", new Font(Font.HELVETICA, 8, Font.ITALIC, Color.BLACK)));
            _table.AddCell(logoCell);

            document.Add(_table);

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("created pdf document header", TAG));
        }


        //document body
        private void AddDocBody()
        {

            Table _table = new Table(5);
            _table.Border = Table.RECTANGLE;
            _table.Border = Table.ALIGN_CENTER;
            _table.Padding = 1;
            _table.Spacing = 1;
            _table.WidthPercentage = 100;

            //add table header
            AddBodyTableHeaders(_table);

            //addtable details
            foreach (var n in _reportmodel.playlist)
            {
                AddBodyTableDetails(n, _table);
            }

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("created pdf table details", TAG));

            //add table totals
            AddBodyTableTotal(_table);

            document.Add(_table);

        }

        //table headers
        private void AddBodyTableHeaders(Table _table)
        {
            Cell id = new Cell(new Phrase("ID", tHfont1));
            id.Border = Cell.RECTANGLE;
            id.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(id);

            Cell employeenumberCell = new Cell(new Phrase("NAME", tHfont1));
            employeenumberCell.Border = Cell.RECTANGLE;
            employeenumberCell.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(employeenumberCell);

            Cell surNameCell = new Cell(new Phrase("SIZE", tHfont1));
            surNameCell.Border = Cell.RECTANGLE;
            surNameCell.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(surNameCell);

            Cell otherNamesCell = new Cell(new Phrase("TYPE", tHfont1));
            otherNamesCell.Border = Cell.RECTANGLE;
            otherNamesCell.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(otherNamesCell);

            Cell idNoCell = new Cell(new Phrase("CREATED DATE", tHfont1));
            idNoCell.Border = Cell.RECTANGLE;
            idNoCell.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(idNoCell);

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("created pdf table header", TAG));
        }

        //table details
        private void AddBodyTableDetails(playlist_dto dto, Table _table)
        {
            Cell id = new Cell(new Phrase(dto.media_id, helv8Font));
            id.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(id);//Col 1

            Cell A = new Cell(new Phrase(dto.media_name, helv8Font));
            A.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(A);//Col 1

            Cell B = new Cell(new Phrase(Utils.formatmediasize(dto.media_size), helv8Font));
            B.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(B);//Col 2

            Cell C = new Cell(new Phrase(dto.media_type, helv8Font));
            C.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(C);//Col 3

            Cell D = new Cell(new Phrase(dto.created_date, helv8Font));
            D.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(D);//Col 4

        }


        //table totals
        private void AddBodyTableTotal(Table _table)
        {
            Cell E1 = new Cell(new Phrase("TOTAL SIZE", tHFont));
            E1.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(E1);//Col 1

            Cell e3 = new Cell(new Phrase("", tHFont));
            e3.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(e3);//Col 1

            Cell E2 = new Cell(new Phrase(Utils.formatmediasize(_reportmodel.total_size.ToString()), tHFont));
            E2.HorizontalAlignment = Cell.ALIGN_CENTER;
            _table.AddCell(E2);//Col 

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("created pdf table totals", TAG));
        }


        //document footer
        private void AddDocFooter()
        {

            Table _table = new Table(1);
            _table.WidthPercentage = 100;
            _table.Border = Table.NO_BORDER;

            Cell sgCell = new Cell(new Phrase("Signature.....................................................................................................", rms10Normal));
            sgCell.HorizontalAlignment = Cell.ALIGN_LEFT;
            sgCell.Border = Cell.NO_BORDER;
            _table.AddCell(sgCell);

            document.Add(_table);

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("created pdf document footer", TAG));
        }




    }
}
