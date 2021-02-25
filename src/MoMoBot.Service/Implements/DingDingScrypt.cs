using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MoMoBot.Service.Implements
{
    public class DingDingScrypt
    {
        public static string key = "uzJLCABvzdeklT==";

        /// <summary>
        /// 获取当前时间戳（UTC）
        /// </summary>
        /// <returns>时间戳</returns>
        public static string GetTimeSpanByDate()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var time = Convert.ToInt64(ts.TotalSeconds).ToString();
            return time;
        }

        /// <summary>
        /// 根据时间戳获取时间
        /// </summary>
        /// <param name="date">时间戳</param>
        /// <returns>时间</returns>
        public static DateTime GetDateByTimeSpan(string date)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime dt = startTime.AddSeconds(long.Parse(date));
            return dt;
        }

        /// <summary>
        /// 有密码的AES加密 
        /// </summary>
        /// <param name="text">加密字符</param>
        /// <param name="password">加密的密码</param>
        /// <param name="iv">密钥</param>
        /// <returns></returns>
        public static string jEncrypt(string toEncrypt)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return HttpUtility.UrlEncode(HttpUtility.UrlEncode(Convert.ToBase64String(resultArray, 0, resultArray.Length), Encoding.UTF8), Encoding.UTF8);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="password"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string jDecrypt(string toDecrypt)
        {
            toDecrypt = HttpUtility.UrlDecode(toDecrypt);
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

    }
}
