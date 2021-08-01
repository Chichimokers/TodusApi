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
            string telefono = "53TELEFONO";
            TodusClient.Register(telefono);
            string code = Console.ReadLine();
            string password = TodusClient.ValidateToken(code, telefono);
            Console.WriteLine("El passwor es : " +password);
            string token = TodusClient.Login(telefono, password);
            Console.WriteLine(token);
            string url = "https://s3.todus.cu/todus/picture/2021-08-01/2e9/2e9bc874b50b73da506ef83de27c6ba55faf4049750121d63f829c83349f9e21";
            //string token = "TOKEN";
            toDusS3X.s3Request asd = new toDusS3X.s3Request(url, token);
            var signedurl = asd.RequestURL();
            TodusClient.DowlandFile(signedurl, token);
            Console.ReadKey();
        }
    }
}
