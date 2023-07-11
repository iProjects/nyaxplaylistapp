using iTextSharp.text;
using nyaxplaylistapp_dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVX;

namespace nyaxplaylistapp_ui.reports
{
    public class pdfgen
    {
        #region "Properties"
        private bool bRet = false;
        private string resourcePath;
        private string sMsg = "";

        public string TAG;
        public string Message
        {
            get { return sMsg; }
            set { sMsg = value; }
        }
        public bool Success
        {
            get { return bRet; }
            set { bRet = value; }
        }
        ////delegate
        //public delegate void ReportsEngineCompleteEventHandler(object sender, ReportsEngineCompleteEventArg e);
        ////event
        //public event ReportsEngineCompleteEventHandler OnCompleteReportsEngine;

        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;

        #endregion "Properties"

        #region "Constructor"
        public pdfgen()
        {

        }
        public pdfgen(string ResourcePath, EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            TAG = this.GetType().Name;

            _notificationmessageEventname = notificationmessageEventname;

            resourcePath = ResourcePath;

        }
        #endregion "Constructor"

        #region "Helper methods"
        /// <summary>
        /// Safely attempts to insert an image file into the document
        /// </summary>
        /// <param name="document">iTextSharp Document in which it needs to be inserted</param>
        /// <param name="sFilename">the name of the file to be inserted</param>
        /// <returns>false if failed to do so</returns>
        private bool DoInsertImageFile(Document document, string sFilename, bool bInsertMsg)
        {
            bool bRet = false;

            try
            {
                if (File.Exists(sFilename) == false)
                {
                    string sMsg = "Unable to find '" + sFilename + "' in the current folder.\n\n"
                                + "Would you like to locate it?";
                    if (MsgBox.Confirm(sMsg))
                        sFilename = FileDialog.GetFilenameToOpen(FileDialog.FileType.Image);
                }

                Image img = null;
                if (File.Exists(sFilename))
                {
                    this.DoGetImageFile(sFilename, out img);
                }

                if (img != null)
                {
                    document.Add(img);
                    bRet = true;
                }
                else
                {
                    if (bInsertMsg)
                        document.Add(new Paragraph(sFilename + " not found"));
                }
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
            }

            return bRet;
        }
        public Image DoGetImageFile(string sFilename)
        {
            bool bRet = false;
            Image img = null;

            try
            {
                if (File.Exists(sFilename) == false)
                {
                    string sMsg = "Unable to find '" + sFilename + "' in the current folder.\n\n"
                                + "Would you like to locate it?";
                    if (MsgBox.Confirm(sMsg))
                        sFilename = FileDialog.GetFilenameToOpen(FileDialog.FileType.Image);
                }

                if (File.Exists(sFilename))
                {
                    img = Image.GetInstance(sFilename);
                }

                bRet = (img != null);
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
            }

            return img;
        }
        private bool DoGetImageFile(string sFilename, out Image img)
        {
            bool bRet = false;
            img = null;

            try
            {
                if (File.Exists(sFilename) == false)
                {
                    string sMsg = "Unable to find '" + sFilename + "' in the current folder.\n\n"
                                + "Would you like to locate it?";
                    if (MsgBox.Confirm(sMsg))
                        sFilename = FileDialog.GetFilenameToOpen(FileDialog.FileType.Image);
                }

                if (File.Exists(sFilename))
                {
                    img = Image.GetInstance(sFilename);
                }

                bRet = (img != null);
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
            }

            return bRet;
        }
        private bool DoLocateImageFile(ref string sFilename)
        {
            bool bRet = false;

            try
            {
                if (File.Exists(sFilename) == false)
                {
                    string sMsg = "Unable to find '" + sFilename + "' in the current folder.\n\n"
                                + "Would you like to locate it?";

                    if (MsgBox.Confirm(sMsg))
                        sFilename = FileDialog.GetFilenameToOpen(FileDialog.FileType.Image);
                }

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
            }

            return bRet = File.Exists(sFilename);
        }
        #endregion "Helper methods"


        public bool show_playlist_report(string app, reportmodel _reportmodel, string sFilePDF)
        {
            bRet = false;
            try
            {
                if ("pdf".Equals(app.ToLower()))
                {
                    pdfbuilder pdfbuilder = new pdfbuilder(_reportmodel, sFilePDF, _notificationmessageEventname);
                    pdfbuilder.GetPDF();
                    return true;
                }
                else
                {
                    excelbuilder excelbuilder = new excelbuilder(_reportmodel, sFilePDF, _notificationmessageEventname);
                    excelbuilder.GetExcel();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Utils.ShowError(ex);
                return false;
            }
        }




    }
}
