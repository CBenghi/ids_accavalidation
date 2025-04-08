using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace IDSAuditToolLib.Utility
{
    public static class IDSValidatorUtility
    {
        public static string DecodeBase64String(string base64String)
        {
            if (!string.IsNullOrEmpty(base64String))
            {
                var base64Bytes = Convert.FromBase64String(base64String);
                return Encoding.UTF8.GetString(base64Bytes);
            }
            return "";
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
