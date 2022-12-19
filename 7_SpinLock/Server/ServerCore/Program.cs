using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class SpinLock  // Lock의 권한을 받기 위해 반복적으로 비교연산을 시도하는 Lock
    {
        volatile int _locked = 0;

        public void Acquire()
        {
            while (true)
            {
                //if (Interlocked.Exchange(ref _locked, 1) == 0)
                //    break;

                // CAS Compare-And-Swap(비교 후 삽입)
                // if (_locked == 0)
                //      _locked = 1;
                int expected = 0;   // 현재 예상값
                int desired = 1;    // 예상과 같다면 삽입할 값
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;

                // Context Switching
                //Thread.Sleep(1);    // 무조건 휴식 => 1ms만큼 휴	
                //Thread.Sleep(0);    // 조건부 양보 => 우선순위가 나와 같거나 높은 쓰레드에게 양
                Thread.Yield();     // 관대한 양보 => 실행 가능한 쓰레드가 있다면 양보
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }

    class AutoLock  // Event를 이용한 Lock(OS에게 요청하기에 부담이 크고 소요시간이 크다.)[커널 동기화 객체]
    {
        // bool <- 커널
        AutoResetEvent _available = new AutoResetEvent(true);
        public void Acquire()
        {
            _available.WaitOne();   // 입장시도, flag = false;
        }
        public void Release()
        {
            _available.Set();       // flag = true
        }
    }
    class ManualLock  // Event를 이용한 Lock(OS에게 요청하기에 부담이 크고 소요시간이 크다.)[커널 동기화 객체]
    {
        // bool <- 커널
        ManualResetEvent _available = new ManualResetEvent(true);
        public void Acquire()
        {
            _available.WaitOne();   // 입장시도
            _available.Reset();     // flag = false;
        }
        public void Release()
        {
            _available.Set();       // flag = true
        }
    }

    class Program
    {
        static int _num = 0;
        //static SpinLock _lock = new SpinLock();
        //static AutoLock _lock = new AutoLock();
        //static ManualLock _lock = new ManualLock();
        static Mutex _lock = new Mutex(); //[커널 동기화 객체]

        static void Thread_1()
        {
            for (int i = 0; i < 10000; ++i)
            {
                //_lock.Acquire();
                //++_num;
                //_lock.Release();

                // Mutex
                _lock.WaitOne();
                ++_num;
                _lock.ReleaseMutex();
            }
        }
        static void Thread_2()
        {
            for (int i = 0; i < 10000; ++i)
            {
                //_lock.Acquire();
                //--_num;
                //_lock.Release();

                // Mutex
                _lock.WaitOne();
                --_num;
                _lock.ReleaseMutex();
            }
        }
        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);
            Console.WriteLine(_num);
        }
    }
}