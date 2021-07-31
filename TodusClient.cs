using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
public static class CodeExtensions
{
    public static string toUTF8(this string value)
    {
        return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(value));
    }
  
    public static string toUnicode(this string value)
    {
        return Encoding.Unicode.GetString(Encoding.Unicode.GetBytes(value));
    }

    public static string choice(this Random rnd, string choices)
    {
        string strings = choices;

        int num = rnd.Next(0, strings.Length - 1);


        return strings[num].ToString();
    }
}
public class TodusClient
{
 
    public static string generate_session_id()
    {
        String ascii_lowercase = "abcdefghijklmnopqrstuvwxyz";
        String ascii_uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        String ascii_letters = ascii_lowercase + ascii_uppercase;
        String digits = "0123456789";

        Random ran = new Random();

        string output = string.Empty;

        foreach (int index in Enumerable.Range(0,149))
        {
            output += ran.choice(ascii_letters + digits);
        }

        return output;
    }

    public static void Register(string PhoneNumber)
    {

        Console.WriteLine("Haciendo peticiones a auth.todus.cu");        
        string a = "\n\n";
        string b = "\x12\x96\x01";
        var bytessd = a.Select(x => Convert.ToByte(x)).ToArray();
        var bytessb = b.Select(x => Convert.ToByte(x)).ToArray();
        var request = (HttpWebRequest)WebRequest.Create("https://auth.todus.cu/v2/auth/users.reserve");
        string datos =
            "\n\n"
            + PhoneNumber.toUTF8()
            + "\x12\x96\x01"
            + generate_session_id().toUTF8();
        Console.WriteLine(datos);
        var conversion1 = "\n\n".toUnicode();
        var conversion2 = "\x12\x96\x01".toUnicode();
        string telefono = PhoneNumber.toUTF8();
        string sessionid = generate_session_id().toUTF8();

        byte[] bytess = Encoding.UTF8.GetBytes(conversion1 + telefono +conversion2 +sessionid);
        request.Method = "POST";
        request.Host = "auth.todus.cu";
        request.ContentType = "application/x-protobuf";
        request.UserAgent = "ToDus 0.39.4 Auth";
     
        using (var strea = request.GetRequestStream())
        {
            strea.Write(bytess, 0, bytess.Length);
        }
        var response = (HttpWebResponse)request.GetResponse();
        var respose = new StreamReader(response.GetResponseStream()).ReadToEnd();
        Console.WriteLine(respose);
    }

}

