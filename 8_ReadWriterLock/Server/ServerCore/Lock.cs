using System;

namespace ServerCore
{
    // Recursive (Yes) [WriteLock->WriteLock {OK}][WriteLock->ReadLock {OK}][ReadLock->WriteLock {No}] 
    // SpinLock policy(5000 -> Yield)
    public class Lock
	{
		const int EMPTY_FLAG = 0x00000000;
		const int WRITE_MASK = 0x7FFF0000;
		const int READ_MASK = 0x0000FFFF;
		const int MAX_SPIN_COUNT = 5000;

		// [Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
		int _flag = EMPTY_FLAG;
        int _writeCount = 0;

        public void WriteLock()
        {
            // 동일 Thread가 WriteLock을 이미 획득했는가?
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                ++_writeCount;
                return;
            }

            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            // 아무도 WRLock을 획득하지 않았을때, 경합해서 소유권을 얻는다.
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; ++i)
                {
                    // 시도해서 성공시 return
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                }
                Thread.Yield();
            }
        }
        public void WriteUnlock()
        {
            int lockCount = --_writeCount;
            if (lockCount == 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            // 동일 Thread가 WriteLock을 이미 획득했는가?
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            // 아무도 WriteLock을 획득하지 않았을때, ReadCount를 1 늘린다.
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; ++i)
                {
                    int expected = _flag & READ_MASK;
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return;
                }
            }
        }
        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }
    }
}

