﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();
            
            while (true)
            {
                try
                {
                    connector.Connect(endPoint, () => { return new ServerSession(); });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(1000);
            }
        }
    }
}