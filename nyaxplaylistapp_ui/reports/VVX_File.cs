using System;
using System.IO;

namespace VVX
{
    /// <summary>
    /// Simple class to simplify access to a few File methods, such as Delete
    /// WARNING: By no means complete!!!
    /// </summary>
    class File
    {
        public struct FileResult 
        {
            public bool Success;
            public string Msg;

            public FileResult(bool bDefault, string sMsg)
            {
                this.Success = bDefault;
                this.Msg = "";
            }
        }

        public static bool Exists(string filename)
        {
            bool bExists = false;
            try
            {
                FileInfo fi = new FileInfo(filename);
                bExists = fi.Exists;
            }
            catch (Exception ex)
            {
                FileResult frRet = new FileResult(true, "");
                frRet.Success = false;
                frRet.Msg = ex.ToString();
            }

            return bExists;
        }

        public static bool IsReadOnly(string filename)
        {
            bool bIsReadOnly = false;
            try
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists)
                    bIsReadOnly = fi.IsReadOnly;
            }
            catch (Exception ex)
            {
                FileResult frRet = new FileResult(true, "");
                frRet.Success = false;
                frRet.Msg = ex.ToString();
            }

            return bIsReadOnly;
        }

        public static bool Delete(string filename)
        {
            FileResult frRet = new FileResult(true, "");

            return Delete(filename, ref frRet);
        }

        public static bool Delete(string filename, ref FileResult frRet)
        {
            frRet = new FileResult(true,"");
            try
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists)
                    fi.Delete();
            }
            catch (Exception ex)
            {
                frRet.Success = false;
                frRet.Msg = ex.ToString();
            }

            return frRet.Success;
        }

        public static bool Encrypt(string filename)
        {
            FileResult frRet = new FileResult(true, "");

            return Encrypt(filename, ref frRet);
        }

        public static bool Encrypt(string filename, ref FileResult frRet)
        {
            frRet = new FileResult(true, "");
            try
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists)
                    fi.Encrypt();
            }
            catch (Exception ex)
            {
                frRet.Success = false;
                frRet.Msg = ex.ToString();
            }

            return frRet.Success;
        }

        public static bool Decrypt(string filename)
        {
            FileResult frRet = new FileResult(true, "");

            return Decrypt(filename, ref frRet);
        }

        public static bool Decrypt(string filename, ref FileResult frRet)
        {
            frRet = new FileResult(true, "");
            try
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists)
                    fi.Decrypt();

            }
            catch (Exception ex)
            {
                frRet.Success = false;
                frRet.Msg = ex.ToString();
            }

            return frRet.Success;
        }

    }
}
