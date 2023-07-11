using System;
using System.Windows.Forms;

namespace VVX
{
    /// <summary>
    /// Simple class to simplify access to OpenFileDialog and SaveFileDialog
    /// WARNING: By no means complete!!!
    /// </summary>
    public static class FileDialog
    {
        public enum FileType
        {
            All,
            Image,
            Text,
            XML
        }

        public static string GetFilters(FileType enFileType)
        {
            string sRet = "";

            switch (enFileType)
            {
                default:
                case FileType.All:
                    break;

                case FileType.Image:
                    sRet = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";
                    break;

                case FileType.Text :
                    sRet = "Text Files(*.TXT;*.CSV)|*.TXT;*.CSV";
                    break;

                case FileType.XML:
                    sRet = "XML Files(*.XML)|*.XML";
                    break;
            }

            if (sRet.Length > 0)
                sRet += "|";
            sRet += "All files (*.*)|*.*";

            return sRet;
        }

        public static string GetFilenameToOpen(FileType enFileType)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = GetFilters(enFileType);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.FileName;
            }
            return "";
        }

        public static string GetFilenameToSave(FileType enFileType, string sFile)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = GetFilters(enFileType);
            dlg.FileName = sFile;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.FileName;
            }
            return "";
        }
    }
}
