using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodusApi
{
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
}
