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
    
    static string version_name = "0.40.16";
    static string version_code = "21820";

    
    public static string GenerateToken(int length = 150)
    {

        const String ascii_lowercase = "abcdefghijklmnopqrstuvwxyz";
        const String ascii_uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const String ascii_letters = ascii_lowercase + ascii_uppercase;
        const String digits = "0123456789";

        const String chars = ascii_letters + digits;

        Random random = new Random();

        string output = string.Empty;

        foreach (int item in Enumerable.Range(1, length))
        {
            output += chars[random.Next(chars.Length)];
        }

        //return new string(Enumerable.Range(1, 10)
        //.Select(_ => chars[random.Next(chars.Length)]).ToArray());

        return output;
    }

    public static string Register(string phone_number)
    {
        Console.WriteLine("Haciendo peticiones a auth.todus.cu");  

        string data = "\x0a\x0a" + phone_number.toUTF8() + "\x12\x96\x1" + GenerateToken(150).toUTF8();
        HttpWebResponse response = TodusApi.HttpClass.POST("https://auth.todus.cu/v2/auth/users.reserve",data);
        string respose = new StreamReader(response.GetResponseStream()).ReadToEnd();
        Console.WriteLine(response.StatusCode + " Se ha registrador correctamente");
        return Convert.ToString(response.StatusCode);     
    }
    
    public static string ValidateToken(string code,string phone_number)
    {
        Console.WriteLine("ValidandoToken");  

        string data = "\x0a\x0a" + phone_number.toUTF8() + "\x12\x96\x1" + GenerateToken(150).toUTF8() +"\x1a\x06" + code.toUTF8();
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
    public static string Login(string phone_number, string password)
    {
        string FinalToken = string.Empty;
        string ReadyToken = string.Empty;
        Console.WriteLine("Logueando");

        string data = "\n\n" + phone_number.toUTF8() + "\x12\x96\x01" + GenerateToken(150).toUTF8() + "\x12\x60" + password.toUTF8() + "\x1a\x05" + version_code.toUTF8();
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


