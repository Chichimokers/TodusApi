using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodusApi
{
    
    class Program
    {
        static void Main(string[] args)
        {
            string telefono = "5358126024";

            // TodusClient.Register(telefono);

            // string code = Console.ReadLine();

            // string password = TodusClient.ValidateToken(code, telefono);

            // Console.WriteLine("El passwor es : " +password);

            // string token = TodusClient.Login(telefono, password);

            //Console.WriteLine(token);

            string url = "https://s3.todus.cu/todus/picture/2021-08-01/2e9/2e9bc874b50b73da506ef83de27c6ba55faf4049750121d63f829c83349f9e21";

            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2Mjc5MjExNjAsInVzZXJuYW1lIjoiNTM1NTM2NjU4MyIsInZlcnNpb24iOiIyMTgyMyJ9.CaYSU-bOpoU0DpotOBxzTg6CXDtvd9lMA_m3zcyUhVA";

            //toDusS3X.s3Request asd = new toDusS3X.s3Request(url, token);

            //var signedurl = asd.RequestURL();

            //  TodusClient.DowlandFile(url, token, "caca.rar");

            TodusClient.DowlandFromTxt("heroku_x64.txt", token);
            
            Console.ReadKey();
        }
    }
}
