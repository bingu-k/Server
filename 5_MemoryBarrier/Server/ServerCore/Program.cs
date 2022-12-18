﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    // Memory Barrier
    // A) 코드 재배치 억제
    // B) 가시성

    // 종류
    // 1. Full Memory Barrier (ASM MFENCE, C# Thread.MemoryBarrier) : Store/Load 막는다.
    // 2. Store Memory Barrier (ASM SFENCE) : Store 막는다.
    // 3. Load Memory Barrier (ASM LFENCE) : Load 막는다.
    class Program
    {
        int _answer;
        bool _complete;

        void A()
        {
            _answer = 123;
            Thread.MemoryBarrier();
            _complete = true;
            Thread.MemoryBarrier();
        }

        void B()
        {
            Thread.MemoryBarrier();
            if (_complete)
            {
                Thread.MemoryBarrier();
                Console.WriteLine(_answer);
            }
        }

        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;

        static void Thread_1()
        {
            y = 1;  // Store
            Thread.MemoryBarrier();
            r1 = x; // Load
        }
        static void Thread_2()
        {
            x = 1;  // Store
            Thread.MemoryBarrier();
            r2 = y; // Load
        }

        static void Main(string[] args)
        {
            int count = 0;
            while (true)
            {
                ++count;
                x = y = r1 = r2 = 0;

                Task t1 = new Task(Thread_1);
                Task t2 = new Task(Thread_2);

                t1.Start();
                t2.Start();
                Task.WaitAll(t1, t2);
                if (r1 == 0 && r2 == 0)
                    break;
            }
            Console.WriteLine($"{count}번만에 빠져나옴!");
        }
    }
}