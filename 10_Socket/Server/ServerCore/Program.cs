using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clinetSocket)
        {
            try
            {
                // 받는다
                byte[] recvBuff = new byte[1024];
                int recyBytes = clinetSocket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recyBytes);
                Console.WriteLine($"[From Client] : {recvData}");

                // 보낸다
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                clinetSocket.Send(sendBuff);

                // 쫓아낸다.
                clinetSocket.Shutdown(SocketShutdown.Both);
                clinetSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            // DNS(Domain Name System)
            // www.byeukim.com(Domain Name) -> 172.1.2.3
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");
            
            while (true)
            {
                ;
            }
        }
    }
}