package sourceafis.general;

import java.util.ArrayList;

public class PriorityQueueF<E> {
	static class Item<E> {
		public float key;
		public E value;
		public Item() {}
		public Item(float k, E v) { key = k; value = v; }
	}

	ArrayList<Item<E>> heap = new ArrayList<Item<E>>();

	public PriorityQueueF() { heap.add(new Item<E>()); }
	public int size() { return heap.size() - 1; }

	public void clear() {
		heap.clear();
		heap.add(new Item<E>());
	}

	void swap(int first, int second) {
		Item<E> tmp = heap.get(first);
		heap.set(first, heap.get(second));
		heap.set(second, tmp);
	}

	void bubbleUp(int bottom) {
		for (int bubble = bottom; bubble > 1; bubble = bubble >> 1) {
			int parent = bubble >> 1;
			if (heap.get(parent).key < heap.get(bubble).key)
				break;
			swap(bubble, parent);
		}
	}

	public void enqueue(float key, E value) {
		heap.add(new Item<E>(key, value));
		bubbleUp(heap.size() - 1);
	}

	void bubbleDown() {
		int bubble = 1;
		while (true) {
			int left = bubble << 1;
			int right = (bubble << 1) + 1;
			if (left > size())
				break;
			int child;
			if (right > size() || heap.get(left).key < heap.get(right).key)
				child = left;
			else
				child = right;
			if (heap.get(bubble).key < heap.get(child).key)
				break;
			swap(bubble, child);
			bubble = child;
		}
	}

	public E peek() {
		if (size() == 0)
			throw new IllegalStateException();
		return heap.get(1).value;
	}

	public E dequeue() {
		E result = peek();
		heap.set(1, heap.get(heap.size() - 1));
		heap.remove(heap.size() - 1);
		bubbleDown();
		return result;
	}

	/*
	 * IEnumerator<V> GetEnumerator() { while (ItemCount > 0) yield return
	 * Dequeue(); }
	 */
}