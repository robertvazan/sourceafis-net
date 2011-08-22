package sourceafis.general;

import sourceafis.jport.InvalidOperationException;
import sourceafis.matching.minutia.MinutiaPair;

 public  class PriorityQueueF<E> {
        static class Item<E>
        {
            public float Key;
            public E Value;
        }

        Item[] Heap; 
        int ItemCount;

        public PriorityQueueF()
        {
            Heap = new Item[1];
            Heap[0]=new Item<E>();
        }

        public void Clear()
        {
            ItemCount = 0;
        }

        public int getCount() {
        	 return ItemCount; 
         }

        void Enlarge()
        {
            Item[] larger = new Item[2 * Heap.length];
            //Array.copy(Heap, larger, Heap.length);
            System.arraycopy(Heap, 0, larger, 0, Heap.length);
            Heap = larger;
        }

        void BubbleUp(int bottom)
        {
            for (int bubble = bottom; bubble > 1; bubble = bubble >> 1)
            {
                int parent = bubble >> 1;
                if (Heap[parent].Key < Heap[bubble].Key)
                    break;
                Item tmp = Heap[bubble];
                Heap[bubble] = Heap[parent];
                Heap[parent] = tmp;
            }
        }

        public void Enqueue(float key, E value)
        {
            if (ItemCount + 1 >= Heap.length)
                Enlarge();
            ++ItemCount;
            //Extra in java as it was not initilized
            if(Heap[ItemCount]==null){
            	Heap[ItemCount]=new Item<E>();
            }
            
            Heap[ItemCount].Key = key;
            Heap[ItemCount].Value = value;
            BubbleUp(ItemCount);
        }

        void BubbleDown()
        {
            int bubble = 1;
            while (true)
            {
                int left = bubble << 1;
                int right = (bubble << 1) + 1;
                if (left > ItemCount)
                    break;
                int child;
                if (right > ItemCount || Heap[left].Key < Heap[right].Key)
                    child = left;
                else
                    child = right;
                if (Heap[bubble].Key < Heap[child].Key)
                    break;
                Item tmp = Heap[bubble];
                Heap[bubble] = Heap[child];
                Heap[child] = tmp;
                bubble = child;
            }
        }

        public E Peek()
        {
            if (ItemCount <= 0)
                throw new InvalidOperationException();
            return (E) Heap[1].Value;
        }

        public E Dequeue()
        {
            if (ItemCount == 0)
                throw new InvalidOperationException();
            //E result = (E) Heap[1].Value;
            E result = (E) Heap[1].Value;
            
            // Heap[1] = Heap[ItemCount];
               Item i = Heap[ItemCount];
               float key=i.Key;
               MinutiaPair v=new MinutiaPair();
               int p=v.Probe;
               int c=v.Candidate;
              Item n=new Item<E>();
              n.Key=key;
              n.Value=new MinutiaPair(p,c);
              Heap[1] = n;
              //*/
            --ItemCount;
            BubbleDown();
            return result;
        }

        /*IEnumerator<V> GetEnumerator()
        {
            while (ItemCount > 0)
                yield return Dequeue();
        }*/
    }