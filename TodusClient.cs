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
    public static T[] Slice<T>(this T[] arr, uint indexFrom, uint indexTo)
    {
        if (indexFrom > indexTo)
        {
            throw new ArgumentOutOfRangeException("indexFrom is bigger than indexTo!");
        }

        uint length = indexTo - indexFrom;
        T[] result = new T[length];
        Array.Copy(arr, indexFrom, result, 0, length);

        return result;
    }

    public static string ToISO88591(this string text)
    {
        return Encoding.GetEncoding("ISO-8859-1").GetString(Encoding.Default.GetBytes(text));
    }

    public static string ToHEXString(this byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:X2} ", b);
        return hex.ToString();
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

        foreach (int index in Enumerable.Range(0,150))
        {
            output += ran.choice(ascii_letters + digits);
        }

        return output;
    }

    public static string Register(string PhoneNumber)
    {
        Console.WriteLine("Haciendo peticiones a auth.todus.cu");        
        string telefono = PhoneNumber.toUTF8();
        string sessionid = generate_session_id();
        string data = "\x0a\x0a" + PhoneNumber.toUTF8() + "\x12\x96\x1" + generate_session_id().toUTF8();
        HttpWebResponse response = TodusApi.HttpClass.POST("https://auth.todus.cu/v2/auth/users.reserve",data);
        string respose = new StreamReader(response.GetResponseStream()).ReadToEnd();
        Console.WriteLine(response.StatusCode + " Se ha registrador correctamente");
        return Convert.ToString(response.StatusCode);     
    }
    public static string ValidateToken(string code,string PhoneNumber)
    {
        Console.WriteLine("ValidandoToken");      
        string telefono = PhoneNumber.toUTF8();
        string sessionid = generate_session_id();
        string data = "\x0a\x0a" + PhoneNumber.toUTF8() + "\x12\x96\x1" + generate_session_id().toUTF8()+"\x1a\x06"+code.toUTF8();
        HttpWebResponse respse = TodusApi.HttpClass.POST("https://auth.todus.cu/v2/auth/users.register", data);
        string response = new StreamReader(respse.GetResponseStream()).ReadToEnd();
        string contenido = string.Empty;
        if(response.Contains('`'))
        {
            int index = response.IndexOf('`') + 1 ;
            string content = response.ToISO88591().Substring(index,96);
            contenido = content;
        }
        return contenido;
     }
    public static bool Writetable(char character)
    {
        string abecedario = "ABCDEFHIJKLMNÑOPQRSTUVWXYZabcdefghijklmñopqrstubwxyz123456789=-";
        List<char> validletters = abecedario.ToList<char>();
        foreach (char letra in validletters)
        {
            if (character == letra)
            {
                return true;
            }
        }
        return false;
    }
    public static string Login(string phonenumber, string password)
    {
        string FinalToken = string.Empty;
        string ReadyToken = string.Empty;
        Console.WriteLine("Logueando");
        string telefono = phonenumber.toUTF8();
        string sessionid = generate_session_id().toUTF8();
        string data = "\n\n" + telefono + "\x12\x96\x01" + sessionid + "\x12\x60" + password.toUTF8() + "\x1a\x05" +"21820".toUTF8();
        HttpWebResponse respse = TodusApi.HttpClass.POST("https://auth.todus.cu/v2/auth/token", data);
        string response = new StreamReader(respse.GetResponseStream()).ReadToEnd();
        string contenido = string.Empty;
        if (Convert.ToString(respse.StatusCode) == "OK" || Convert.ToString(respse.StatusCode) == "100")
        {
            Console.WriteLine("Obteninedo Token para descargas y subidas ");
            ReadyToken = response.toUTF8();
            foreach (var caracter in ReadyToken)
            {
                if (Writetable(caracter))
                {
                    FinalToken += caracter;
                }
            }
        }
        return "Token Obtenido : " + FinalToken;
    }
  

}


