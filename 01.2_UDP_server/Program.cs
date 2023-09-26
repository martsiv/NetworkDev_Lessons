using System.Net.Sockets;
using System.Net;
using System.Text;

namespace _01._2_UDP_server
{
    class Program
    {
        static string address = "127.0.0.1"; // поточний адрес
        static int port = 8080;              // порт для приема входящих запросов

        static void Main(string[] args)
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

            // об'єкт для отримання адреси відправника
            IPEndPoint remoteEndPoint = null;

            // создаем сокет
            //Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // связываем сокет с локальной точкой, по которой будем принимать данные
            UdpClient listener = new UdpClient(ipPoint);

            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                //listenSocket.Bind(ipPoint);
                Console.WriteLine("Server started! Waiting for connection...");

                while (true)
                {
                    // получаем сообщение
                    //int bytes = 0;
                    //byte[] data = new byte[1024];
                    //bytes = listenSocket.ReceiveFrom(data, ref remoteEndPoint);
                    byte[] data = listener.Receive(ref remoteEndPoint);

                    string msg = Encoding.Unicode.GetString(data);
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: {msg} from {remoteEndPoint}");

                    // отправляем ответ
                    //string message = "Message was send!";
                    string message = GetAnswerToRequest.GetAnswer(msg);
                    data = Encoding.Unicode.GetBytes(message);
                    //listenSocket.SendTo(data, remoteEndPoint);
                    listener.Send(data, data.Length, remoteEndPoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //listenSocket.Shutdown(SocketShutdown.Both);
            //listenSocket.Close();
            listener.Close();
        }
    }
    public class GetAnswerToRequest
    {
        private static string hello = "Hello!";
        private static string bye = "Bye!";
        private static string another = "I received your message";
        public static string GetAnswer(string request)
        {
            if (request == "Hello" || request == "Hi")
                return hello;
            else if (request == "Good bye" || request == "Bye")
                return bye;
            else
                return another;
        }
    }
}