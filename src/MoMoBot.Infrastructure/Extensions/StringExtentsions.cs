using System;
using System.Security.Cryptography;
using System.Text;

namespace MoMoBot.Infrastructure.Extensions
{
    public static class StringExtentsions
    {
        public static string MD5Hash(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }

        public static string ReplaceSqlChars(this string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            return str.Replace("'", "‘")
                .Replace("%"," ")
                .Replace("\"","“");
        }
    }
}
