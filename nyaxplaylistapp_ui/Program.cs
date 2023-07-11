using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Security.Cryptography;
using nyaxplaylistapp_ui.reports;
using nyaxplaylistapp_dal;

namespace nyaxplaylistapp_ui
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                SplashScreen.ShowSplashScreen();

                Application.DoEvents();

                //SplashScreen.SetStatus("Checking for [ " + SystemsConfigFile + " ] ...");
                //if (!File.Exists(SystemsConfigFile))
                //    throw new FileNotFoundException("SB Payroll cannot locate configuration file " + SystemsConfigFile);

                SplashScreen.SetStatus("Checking for Default System...");
                //SBSystem defSys = SQLHelper.GetDataDefaultSystem();
                //if (defSys == null)
                //    throw new ArgumentException("No Default System is Set", "system");
                 
                //SplashScreen.SetStatus("Connecting to the SQL Server [" + defSys.Server + "]");
                //if (!SQLHelper.ServerExists(defSys))
                //    throw new ArgumentException("Unable to connect to Server [" + defSys.Server + "] ", "server");

                SplashScreen.SetStatus("Checking for a valid Database...");
                //if (!SQLHelper.DatabaseExists(defSys))
                //    throw new ArgumentException("Database [ " + defSys.Database + " ] does not exist in Server [ " + defSys.Server + " ] ", "database");

                SplashScreen.SetStatus("Checking for Database Version...");
                //string dbver = SQLHelper.DatabaseVersion(defSys);
                //string sysver = Assembly.GetEntryAssembly().GetName().Version.ToString();
                //if (!dbver.Equals(sysver))
                //    throw new ArgumentException("Database and System Version do not match; the Database may not be usable. Use a Database Migration Tool", "version");
                
                SplashScreen.SetStatus("Checking Defaults Tables are populated...");

                System.Threading.Thread.Sleep(5000);
                
                SplashScreen.CloseForm();

                Application.Run(new create_playlist());

            }
            catch (Exception ex)
            {
                Utils.ShowError(ex);
            }
            
        }
    }

    public static class msgtype
    {
        public const string info = "info";
        public const string warn = "warn";
        public const string error = "error";
    }


    /// <summary>
    /// Salted password hashing with PBKDF2-SHA1.
    /// Compatibility: .NET 3.0 and later.
    /// </summary>
    /// <remarks>See http://crackstation.net/hashing-security.htm for much more on password hashing.</remarks>
    public static class PasswordHashProvider
    {
        /// <summary>
        /// The salt byte size, 64 length ensures safety but could be increased / decreased
        /// </summary>
        private const int SaltByteSize = 64;
        /// <summary>
        /// The hash byte size,
        /// </summary>
        private const int HashByteSize = 64;
        /// <summary>
        /// High iteration count is less likely to be cracked
        /// </summary>
        private const int Pbkdf2Iterations = 10000;
        /// <summary>
        /// Creates a salted PBKDF2 hash of the password.
        /// </summary>
        /// <remarks>
        /// The salt and the hash have to be persisted side by side for the password. They could be persisted as bytes or as a string using the convenience methods in the next class to convert from byte[] to string and later back again when executing password validation.
        /// </remarks>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hash of the password.</returns>
        public static PasswordHashContainer CreateHash(string password)
        {
            // Generate a random salt
            using (var csprng = new RNGCryptoServiceProvider())
            {
                // create a unique salt for every password hash to prevent rainbow and dictionary based attacks
                var salt = new byte[SaltByteSize];
                csprng.GetBytes(salt);
                // Hash the password and encode the parameters
                var hash = Pbkdf2(password, salt, Pbkdf2Iterations, HashByteSize);
                return new PasswordHashContainer(hash, salt);
            }
        }
        /// <summary>
        /// Recreates a password hash based on the incoming password string and the stored salt
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <param name="salt">The salt existing.</param>
        /// <returns>the generated hash based on the password and salt</returns>
        public static byte[] CreateHash(string password, byte[] salt)
        {
            // Extract the parameters from the hash
            return Pbkdf2(password, salt, Pbkdf2Iterations, HashByteSize);
        }
        /// <summary>
        /// Validates a password given a hash of the correct one.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <param name="salt">The existing stored salt.</param>
        /// <param name="correctHash">The hash of the existing password.</param>
        /// <returns><c>true</c> if the password is correct. <c>false</c> otherwise. </returns>
        public static bool ValidatePassword(string password, byte[] salt, byte[] correctHash)
        {
            // Extract the parameters from the hash 
            byte[] testHash = Pbkdf2(password, salt, Pbkdf2Iterations, HashByteSize);
            return CompareHashes(correctHash, testHash);
        }
        /// <summary>
        /// Compares two byte arrays (hashes)
        /// </summary>
        /// <param name="array1">The array1.</param>
        /// <param name="array2">The array2.</param>
        /// <returns><c>true</c> if they are the same, otherwise <c>false</c></returns>
        public static bool CompareHashes(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length) return false;
            return !array1.Where((t, i) => t != array2[i]).Any();
        }
        /// <summary>
        /// Computes the PBKDF2-SHA1 hash of a password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The PBKDF2 iteration count.</param>
        /// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
        /// <returns>A hash of the password.</returns>
        private static byte[] Pbkdf2(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt))
            {
                pbkdf2.IterationCount = iterations;
                return pbkdf2.GetBytes(outputBytes);
            }
        }
    }
    /// <summary>
    /// Container for password hash and salt and iterations.
    /// </summary>
    public sealed class PasswordHashContainer
    {
        /// <summary>
        /// Gets the hashed password.
        /// </summary>
        public byte[] HashedPassword { get; private set; }
        /// <summary>
        /// Gets the salt.
        /// </summary>
        public byte[] Salt { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordHashContainer" /> class.
        /// </summary>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <param name="salt">The salt.</param>
        public PasswordHashContainer(byte[] hashedPassword, byte[] salt)
        {
            this.HashedPassword = hashedPassword;
            this.Salt = salt;
        }
    }
    /// <summary>
    /// Convenience methods for converting between hex strings and byte array.
    /// </summary> 
    public static class ByteConverter
    {
        /// <summary>
        /// Converts the hex representation string to an array of bytes
        /// </summary>
        /// <param name="hexedString">The hexed string.</param>
        /// <returns></returns>
        public static byte[] GetHexBytes(string hexedString)
        {
            var bytes = new byte[hexedString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var strPos = i * 2;
                var chars = hexedString.Substring(strPos, 2);
                bytes[i] = Convert.ToByte(chars, 16);
            }
            return bytes;
        }
        /// <summary>
        /// Gets a hex string representation of the byte array passed in.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public static string GetHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
        }
    }



}
