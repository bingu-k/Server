using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] arr = new int[10000, 10000];

            // 시간이 명확히 차이가 나는 이유는 인접한 요소를 접근할 것이라 생각해서
            // 일시적으로 캐시를 저장하고 있었기 때문이다.
            // Multi Thread를 사용할 때, 이런 부분 때문에 더욱 끔찍한 결과를 볼 수도 있다.
            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; ++y)
                    for (int x = 0; x < 10000; ++x)
                        arr[y, x] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(y, x) 순서 걸린 시간 {end - now}");
            }
            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; ++y)
                    for (int x = 0; x < 10000; ++x)
                        arr[x, y] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(x, y) 순서 걸린 시간 {end - now}");
            }

        }
    }
}