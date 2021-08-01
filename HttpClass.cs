using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
namespace TodusApi
{
    public static class HttpClass
    {
        public static HttpWebResponse POST(string url,string data)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            byte[] bytess = Encoding.GetEncoding("ISO-8859-1").GetBytes(data);
            request.Method = "POST";
            request.Host = "auth.todus.cu";
            request.ContentType = "application/x-protobuf";
            request.UserAgent = "ToDus 0.40.16 Auth";
            using (var strea = request.GetRequestStream())
            {
                strea.Write(bytess, 0, bytess.Length);
            }
            HttpWebResponse respse = (HttpWebResponse)request.GetResponse();
            //var response = new StreamReader(respse.GetResponseStream()).ReadToEnd();
            return respse;
        }
    }
}
