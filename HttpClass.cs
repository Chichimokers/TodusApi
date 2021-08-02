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
        public static void DowlandFile(string URL,string token,string Filename)
        {
            toDusS3X.s3Request asd = new toDusS3X.s3Request(URL, token);
            var signedurl = asd.RequestURL();
            /* WebClient dowland = new WebClient();
              dowland.Headers.Set("Method", "Get");
             dowland.Headers.Set("authorization","Bearer "+token);
               dowland.Headers.Set("Pragma", "no-cache");             
              dowland.DownloadFile(URL, "main.rar");*/
            HttpWebRequest dowland = (HttpWebRequest)WebRequest.Create(signedurl);
            dowland.Method = "GET";
            dowland.Host = "s3.todus.cu";
            dowland.Headers.Set("authorization","Bearer "+token);
            dowland.ContentType = "application/x-protobuf";
            dowland.UserAgent = "ToDus 0.40.16 HTTP-Download";
            long bytestraferidos = 0;
            using (HttpWebResponse respse = (HttpWebResponse)dowland.GetResponse())
            {

                using (var responseStream = respse.GetResponseStream()) 
                using (FileStream fileStream = File.Create(Filename))
                {
                    byte[] buffer = new byte[4096 * 1024];
                    int size = responseStream.Read(buffer, 0, buffer.Length);
                    while (size > 0)
                    {
                        fileStream.Write(buffer, 0, size);
                        size = responseStream.Read(buffer,0,buffer.Length);
                        if (respse.ContentLength != bytestraferidos)
                        {
                            bytestraferidos += size;
                        }
                        Console.WriteLine(FileSize.Check(bytestraferidos));
                    }
                    fileStream.Close();
                 
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine("Descarga Finalizada");
                    Console.BackgroundColor = ConsoleColor.Black;
                }

            }     
        }
        public static HttpWebResponse POST(string url,string data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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
