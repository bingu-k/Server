using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        static int number = 0;
        static Object _obj = new Object();

        // atomic(원자성)[한번에 과정을 실행]
        // 참조하고 있는 변수를 다른 곳에서 참조할 수 없기때문에
        // 순서를 정해주는 효과도 있다.
        // 주소값을 넣어주는 이유는 변수의 복사값이 아닌 참조할 수 이는 주소를 알아야하기 때문이다.

        // 상호 배제(Mutual Exclusive)
        // DeadLock(발생 오류)
        // 어느 한 곳에서 lock을 걸고 의도치 않게 죽어버림
        // 다른 한 곳에서 unlock되길 기다리며 무한 대기
        // 이런 상태를 DeadLock이라 한다.
        static void Thread_1()
        {
            for (int i = 0; i < 1000000000; ++i)
            {
                lock(_obj)
                {
                    ++number;
                }

                //Monitor.Enter(_obj);    // mutex lock
                //Interlocked.Increment(ref number);  // 성능차이가 어마어마하다.
                ////++number;
                //Monitor.Exit(_obj);     // mutex unlock
            }
        }
        static void Thread_2()
        {
            for (int i = 0; i < 1000000000; ++i)
            {
                lock (_obj)
                {
                    --number;
                }
                //Monitor.Enter(_obj);    // mutex lock
                //Interlocked.Decrement(ref number);  // 성능차이가 어마어마하다.
                ////--number;
                //Monitor.Exit(_obj);     // mutex unlock
            }
        }
        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();
            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}