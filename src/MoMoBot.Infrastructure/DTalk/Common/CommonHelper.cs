using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MoMoBot.Infrastructure.DTalk
{
    public static class CommonHelper
    {
        /// <summary> 
        /// 根据GUID获取16位的唯一字符串 
        /// </summary> 
        /// <returns></returns> 
        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }


        /// <summary>
        /// 获取签名
        /// </summary>
        /// <returns></returns>
        //public static string GetUrl()
        //{
        //    return HttpContext.Current.Request.Url.ToString();
        //}
        public static string Sha1(string str)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] bytes_sha1_in = Encoding.UTF8.GetBytes(str);
                byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
                string str_sha1_out = BitConverter.ToString(bytes_sha1_out);
                str_sha1_out = str_sha1_out.Replace("-", "");
                return str_sha1_out.ToLower();
            }
        }

    }
}