using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        // Accessable Global Value
        // volatile 휘발성 Data(컴파일러가 디버깅할때, 건들지 않게끔 처리)
        volatile static bool _stop = false;

        static void ThreadMain()
        {
            Console.WriteLine("Thread Start!");

            int i = 0;
            while (_stop == false)
            {
                Thread.Sleep(100);
                Console.Write(++i);
            }
            Console.WriteLine("\nThread Stop!");
        }
        static void Main(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000);

            _stop = true;
            Console.WriteLine("Call Stop!");

            t.Wait();
            Console.WriteLine("Success Stop!");
        }
    }
}