using System;
using System.IO;

namespace WorkitemDaemon.Helpers
{
    public static class Base64Helper
    {
        public static string EncodeFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("Le fichier passé n'existe pas");

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] filebytes = new byte[fs.Length];
                fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
                return Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);
            }
        }

        public static void DecodeToFile(string base64data, string filePath)
        {
            if (string.IsNullOrEmpty(base64data))
                throw new Exception("base64data est vide");

            byte[] filebytes = Convert.FromBase64String(base64data);
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                fs.Write(filebytes, 0, filebytes.Length);
            }
        }
    }
}