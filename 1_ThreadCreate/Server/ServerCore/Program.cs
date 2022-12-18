using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        static void MainThread(Object state)
        {
            for (int i = 0; i < 5; ++i)
                Console.WriteLine("Hello Thread!");
        }

        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            for (int i = 0; i < 5; ++i)
            {
                Task t = new Task(() => { Thread.Sleep(1000); }, TaskCreationOptions.LongRunning);
                t.Start();
            }

            //for (int i = 0; i < 5; ++i)
            //    ThreadPool.QueueUserWorkItem((obj) => { Console.WriteLine($"Thread : {obj.ToString()}"); Thread.Sleep(1000); }, i);
            ThreadPool.QueueUserWorkItem(MainThread);

            //Thread t = new Thread(MainThread);
            //t.Name = "Test Thread";
            //t.IsBackground = true;              // Background로 실행
            //t.Start();                          // 기본적으로 Foreground로 실행

            //Console.WriteLine("Waiting for Thread!");

            //t.Join();                           // Thread가 종료될 때까지 기다림

            Thread.Sleep(5000);
            Console.WriteLine("End Thread");
        }
    }
}