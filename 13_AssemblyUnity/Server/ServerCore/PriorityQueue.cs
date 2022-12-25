using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class PriorityQueue<T> where T : IComparable<T> // T에 대한 인터페이스를 가져옴
    {
        List<T> _heap = new List<T>();

        public int Count { get { return _heap.Count(); } }

        // O(log_2(N)) -> heap의 높이에 관련있다.
        public void Push(T data)
        {
            // heap의 맨 끝에 새로운 데이터를 삽입한다.
            _heap.Add(data);

            // 도장깨기
            int now = _heap.Count - 1;
            while (now > 0)
            {
                int next = (now - 1) / 2;
                if (_heap[now].CompareTo(_heap[next]) < 0)
                    break;

                // 교체
                T tmp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = tmp;

                // 이동
                now = next;
            }
        }
        // O(log_2(N)) -> heap의 높이에 관련있다.
        public T Pop()
        {
            // return data 저장
            T ret = _heap[0];

            // 마지막 데이터를 root 노드로 이동
            int lastIndex = _heap.Count - 1;
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);
            --lastIndex;

            // 도장깨기
            int now = 0;
            while (true)
            {
                int left = 2 * now + 1;
                int right = 2 * now + 2;
                int next = now;
                // 좌우 데이터 확인후 이동
                if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                    next = left;
                if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                    next = right;
                // 좌우 데이터가 현재값보다 작으면 종료
                if (next == now)
                    break;

                // 교체
                T tmp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = tmp;

                // 이동
                now = next;
            }
            return ret;
        }

        public T Peek()
        {
            if (_heap.Count == 0)
                return default(T);
            return _heap[0];
        }
    }
}
