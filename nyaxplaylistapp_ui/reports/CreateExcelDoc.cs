using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using nyaxplaylistapp_dal;

namespace nyaxplaylistapp_ui.reports
{
   public  class CreateExcelDoc
    {
        private Microsoft.Office.Interop.Excel.Application appExcel = null;
        private Workbook workbook = null;
        private Worksheet worksheet = null;
        private Range workSheet_range = null;
        private Dictionary<int, string> alpha = new Dictionary<int, string>();

        public CreateExcelDoc()
        {
            Initialize();
            createDoc();
        }

        private void Initialize()
        {
            alpha.Add(1, "A");
            alpha.Add(2, "B");
            alpha.Add(3, "C");
            alpha.Add(4, "D");
            alpha.Add(5, "E");
            alpha.Add(6, "F");
            alpha.Add(7, "G");
            alpha.Add(8, "H");
            alpha.Add(9, "I");
            alpha.Add(10, "J");
            alpha.Add(11, "K");
            alpha.Add(12, "L");
            alpha.Add(13, "M");
            alpha.Add(14, "N");
            alpha.Add(15, "O");
            alpha.Add(16, "P");
            alpha.Add(17, "Q");
            alpha.Add(18, "R");
            alpha.Add(19, "S");
            alpha.Add(20, "T");
            alpha.Add(21, "U");
            alpha.Add(22, "V");
            alpha.Add(23, "W");
            alpha.Add(24, "X");
            alpha.Add(25, "Y");
            alpha.Add(26, "Z");
            alpha.Add(27, "AA");
            alpha.Add(28, "AB");
            alpha.Add(29, "AC");
            alpha.Add(30, "AD");
            alpha.Add(31, "AE");
            alpha.Add(32, "AF");
            alpha.Add(33, "AG");
            alpha.Add(34, "AH");
            alpha.Add(35, "AI");
            alpha.Add(36, "AJ");
            alpha.Add(37, "AK");
            alpha.Add(38, "AL");
            alpha.Add(39, "AM");
            alpha.Add(40, "AN");
            alpha.Add(41, "AO");
            alpha.Add(42, "AP");
            alpha.Add(43, "AQ");
            alpha.Add(44, "AR");
            alpha.Add(45, "AS");
            alpha.Add(46, "AT");
            alpha.Add(47, "AU");
            alpha.Add(48, "AV");
            alpha.Add(49, "AW");
            alpha.Add(50, "AX");
            alpha.Add(51, "AY");
            alpha.Add(52, "AZ");
            alpha.Add(53, "BA");
            alpha.Add(54, "BB");
            alpha.Add(55, "BC");
            alpha.Add(56, "BD");
            alpha.Add(57, "BE");
            alpha.Add(58, "BF");
            alpha.Add(59, "BG");
            alpha.Add(60, "BH");
            alpha.Add(61, "BI");
            alpha.Add(62, "BJ");
            alpha.Add(63, "BK");
            alpha.Add(64, "BL");
            alpha.Add(65, "BM");
            alpha.Add(66, "BN");
            alpha.Add(67, "BO");
            alpha.Add(68, "BP");
            alpha.Add(69, "BQ");
            alpha.Add(70, "BR");
            alpha.Add(71, "BS");
            alpha.Add(72, "BT");
            alpha.Add(73, "BU");
            alpha.Add(74, "BV");
            alpha.Add(75, "BW");
            alpha.Add(76, "BX");
            alpha.Add(77, "BY");
            alpha.Add(78, "BZ");
        }

        public void createDoc()
        {
            try
            {
                appExcel = new Microsoft.Office.Interop.Excel.Application();
                appExcel.Visible = true;
                workbook = appExcel.Workbooks.Add(1);
                appExcel.Windows.get_Item(1).DisplayGridlines = false;
                worksheet = (Worksheet)workbook.Sheets[1];
                worksheet.PageSetup.PrintGridlines = false;

            }
            catch (Exception ex)
            {
                Utils.ShowError(ex);
            }
            finally
            {

            }
        }

        
        public void Save(string filename)
        {
            //if (appExcel != null) appExcel.SaveWorkspace(filename);
            if (workbook != null) workbook.SaveAs(filename);
        }

        
        public void createHeaders(int row, int col, string htext, string cell1, string cell2, int mergeColumns, string color, bool font, int size, string fcolor)
        {
            worksheet.Cells[row, col] = htext;
            workSheet_range = worksheet.get_Range(cell1, cell2);
            workSheet_range.Merge(mergeColumns);
            switch (color)
            {
                case "YELLOW":
                    workSheet_range.Interior.Color = System.Drawing.Color.Yellow.ToArgb();
                    break;
                case "GRAY":
                    workSheet_range.Interior.Color = System.Drawing.Color.Gray.ToArgb();
                    break;
                case "GAINSBORO":
                    workSheet_range.Interior.Color = System.Drawing.Color.Gainsboro.ToArgb();
                    break;
                case "Turquoise":
                    workSheet_range.Interior.Color = System.Drawing.Color.Turquoise.ToArgb();
                    break;
                case "PeachPuff":
                    workSheet_range.Interior.Color = System.Drawing.Color.PeachPuff.ToArgb();
                    break;
                case "WHITE":
                    workSheet_range.Interior.Color = System.Drawing.Color.White.ToArgb();
                    break;
                default:
                      workSheet_range.Interior.Color = System.Drawing.Color.White.ToArgb();
                    break;

            }
             
            workSheet_range.Borders.Color = System.Drawing.Color.White.ToArgb();
            workSheet_range.Borders.LineStyle = XlLineStyle.xlLineStyleNone;
            workSheet_range.Font.Bold = font;
            workSheet_range.ColumnWidth = size;
            if (fcolor.Equals(""))
            {
                workSheet_range.Font.Color = System.Drawing.Color.Black.ToArgb();
            }
            else
            {
                workSheet_range.Font.Color = System.Drawing.Color.Black.ToArgb();
            } 
        }

        public void addData(int row, int col, string data, string cell1, string cell2, string format)
        {
            worksheet.Cells[row, col] = data;
            workSheet_range = worksheet.get_Range(cell1, cell2);
            workSheet_range.Borders.Color = System.Drawing.Color.White.ToArgb();
            workSheet_range.Borders.LineStyle = XlLineStyle.xlLineStyleNone;
            workSheet_range.NumberFormat = format;
        }


        public string IntAlpha(int i)
        {
            return alpha[i];
        }
    }



}
