using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        static ReaderWriterLockSlim _WRlock = new ReaderWriterLockSlim();

        class Reward
        { }

        static Reward GetRewardById(int id)
        {
            _WRlock.EnterReadLock();
            _WRlock.ExitReadLock();
            return null;
        }

        static void AddReward(Reward reward)
        {
            _WRlock.EnterWriteLock();
            _WRlock.ExitWriteLock();
        }

        static volatile int count = 0;
        static Lock _lock = new Lock();
        static void Main(string[] args)
        {
            Task t1 = new Task(delegate ()
            {
                for (int i = 0; i < 10000; ++i)
                {
                    //_lock.ReadLock();// 문제 생겨야 정상
                    _lock.WriteLock();
                    ++count;
                    //_lock.ReadUnlock();
                    _lock.WriteUnlock();
                }
            });
            Task t2 = new Task(delegate ()
            {
                for (int i = 0; i < 10000; ++i)
                {
                    //_lock.ReadLock();
                    _lock.WriteLock();
                    --count;
                    //_lock.ReadUnlock();
                    _lock.WriteUnlock();
                }
            });

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(count);
        }
    }
}