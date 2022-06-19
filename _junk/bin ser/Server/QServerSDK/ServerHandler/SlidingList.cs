

using System;

namespace QServerSDK.ServerHandler
{
    public class SlidingList<T> where T : class, IAckable
    {
        readonly int n;
        // current cycle pointer
        int cycle;
        int cycleLastAcked;

        T[] snapshots;

        public SlidingList(int n)
        {
            this.n = n;
            snapshots = new T[n];

            cycle = -1;
            cycleLastAcked = -1;
        }

        public T Current => cycle != -1 ? snapshots[cycle] : null;

        public T LastAcked => cycleLastAcked != -1 ? snapshots[cycleLastAcked] : null;

        public T this[int i]
        {
            get
            {
                return snapshots[i];
            }
        }

        public void Insert(T elem)
        {
            cycle = (cycle + 1) % n;
            snapshots[cycle] = elem;
        }
        
        public void Ack(int i)
        {
            i = i % n;

            if (snapshots[i] == null)
                throw new Exception("nonexistant_gamestate");

            snapshots[i].Ack = true;

            // cyclic max find on lastAcked i
            if (i > cycleLastAcked || i == 0 && cycleLastAcked == n-1)
                cycleLastAcked = i;
        }

        
    }
}
