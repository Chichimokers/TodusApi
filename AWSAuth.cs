using System;
using System.Text;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.Net.Sockets;
using System.Net.Security;
using System.Text.RegularExpressions;

namespace toDusS3X
{
    public static class CodeExtensions
    {
        public static string toUTF8(this string value)
        {
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(value));
        }

        public static string choice(this Random rnd, string choices)
        {
            string strings = choices;

            int num = rnd.Next(0, strings.Length - 1);


            return strings[num].ToString();
        }
    }
   
    public class s3Request : IDisposable
    {
        String url = String.Empty;
        String token = String.Empty;

        public s3Request(String Url, String Token)
        {
            url = Url;
            token = Token;
        }


        public string RequestURL()
        {
            return get_signed_url(url, token);

        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private string generate_session_id()
        {

            String ascii_lowercase = "abcdefghijklmnopqrstuvwxyz";

            String ascii_uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            String ascii_letters = ascii_lowercase + ascii_uppercase;

            String digits = "0123456789";

            Random ran = new Random();

            string output = string.Empty;

            foreach (int index in Enumerable.Range(0, 5))
            {
                output += ran.choice(ascii_letters + digits);
            }

            return output;
        }

        private string b64decode(string input)
        {
            var base64ByteArray = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(base64ByteArray);
        }
        private string b64encode(string input)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);

            return Convert.ToBase64String(data);
        }

        private string get_signed_url(String url, String token)
        {

            var decodedB64 = b64decode(token.Split('.')[1]);

            var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(decodedB64), new System.Xml.XmlDictionaryReaderQuotas());

            var token_decoded = XElement.Load(jsonReader);

            var phone = token_decoded.Element("username").Value;

            var authstr = "\0" + phone + "\0" + token;
            authstr = authstr.toUTF8();

            //print(authstr);

            authstr = b64encode(authstr);

            //print(authstr);

            var sid = generate_session_id();


            var host = "im.todus.cu";

            var port = 1756;

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sock.Connect(host, port);

            NetworkStream streamnet = new NetworkStream(sock);

            SslStream sendStream = new SslStream(streamnet, false);

            sendStream.AuthenticateAsClient(host, null, System.Security.Authentication.SslProtocols.Tls, true);

            return start_message_loop(sendStream, sid, authstr, url);

        }

        private string start_message_loop(SslStream stream, String sid, String authStr, String url)
        {
            //byte[] request_data = Encoding.ASCII.GetBytes("<stream:stream xmlns='jc' o='im.todus.cu' xmlns:stream='x1' v='1.0'>");

            //send_data(stream, request_data);

            //analyze_answer(stream, sid, authStr, url);

            return test_all(stream, sid, authStr, url);


        }

        public string test_all(SslStream stream, String sid, String authStr, String url)
        {
            //Enviando la petición incial.
            send_data(stream, Encoding.UTF8.GetBytes("<stream:stream xmlns='jc' o='im.todus.cu' xmlns:stream='x1' v='1.0'>"));

            //Recibiendo la información de respuesta.
            receive_data(stream);


            //Recibimos el OAUTH2
            receive_data(stream);


            //Enviamos el string de autorización generado previamente.
            send_data(stream, Encoding.UTF8.GetBytes("<ah xmlns='ah:ns' e='PLAIN'>" + authStr + "</ah>"));


            //Recibimos la notificación de autentificación.
            receive_data(stream);



            //Enviamos el string de autorización generado previamente.
            send_data(stream, Encoding.UTF8.GetBytes("<stream:stream xmlns='jc' o='im.todus.cu' xmlns:stream='x1' v='1.0'>"));


            //Recibimos la primera respuesta
            receive_data(stream);


            //Recibimos la notificación de que el stream está listo para enviar el id.
            receive_data(stream);



            //Enviamos el string de autorización generado previamente.
            send_data(stream, Encoding.UTF8.GetBytes("<iq i='" + sid + "-1' t='set'><b1 xmlns='x4'></b1></iq>"));


            //Recibimos la respuesta de la sesión.
            receive_data(stream);



            //Enviamos el string de certificación de la url.
            send_data(stream, Encoding.UTF8.GetBytes("<iq i='" + sid + "-2' t='get'><query xmlns='todus:gurl' url='" + url + "'></query></iq>"));


            //Recibimos la respuesta de la sesión.
            var response = receive_data(stream);

            var match = Regex.Match(response, ".*du='(.*)' stat.*");



            Console.ForegroundColor = ConsoleColor.DarkRed;

            Console.ForegroundColor = ConsoleColor.White;

            return match.Groups[1].Value.Replace("amp;", "");


        }

        private void analyze_answer(SslStream stream, String sid, String authStr, String url)
        {
            //byte[] buffer = new byte[1024];
            //var bytes_count = stream.Read(buffer, 0, buffer.Length);

            string response = receive_data(stream);


            if (response.StartsWith("<?xml version='1.0'?><stream:stream i='") & response.EndsWith("v='1.0' xml:lang='en' xmlns:stream='x1' f='im.todus.cu' xmlns='jc'>"))
            {
                analyze_answer(stream, sid, authStr, url);
            }
        }

        private void send_data(SslStream stream, byte[] data)
        {
            //Console.ForegroundColor = ConsoleColor.Blue;
            //Console.Write("[SEND] ");
            //Console.ForegroundColor = ConsoleColor.White;
            //Global.print(Encoding.UTF8.GetString(data));
            stream.Write(data);
        }

        private string receive_data(SslStream stream, int buffer_size = 1024)
        {

            byte[] buffer = new byte[1024];

            var bytes_count = stream.Read(buffer, 0, buffer.Length);

            var result = Encoding.UTF8.GetString(buffer, 0, bytes_count);

            //Console.ForegroundColor = ConsoleColor.Green;

            //Console.Write("[RECEIVED] ");

            //Console.ForegroundColor = ConsoleColor.White;

            //Global.print(result);

            return result;

        }
        //token = eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2MjIwNzUzOTIsInVzZXJuYW1lIjoiNTM1MzM2OTEyNSIsInZlcnNpb24iOiIyMTgwNiJ9.w1lK_1dcvE4Y7mfgqOZD6n9F5pgxIz-yMmEvZSyQNz0
        //https://s3.todus.cu/todus/file/2021-05-26/10f/10f2cab3eaa3731b133b78be804bfd1d72a473e26ef8b6db2ddd621c5dd5c49f

    }


}