/**
 * @author Veaceslav Dubenco
 * @since 09.10.2012
 */
package sourceafis.general;

import java.util.Collection;
import java.util.Iterator;
import java.util.List;
import java.util.ListIterator;

/**
 * 
 */
public final class CircularArray<T> implements List<T> {

	private T[] Inner;
	private int First;
	private int ItemCount;

	public int getHeadCount() {
		return Math.min(ItemCount, Inner.length - First);
	}

	public int getTailCount() {
		return Math.max(0, First + ItemCount - Inner.length);
	}

	@SuppressWarnings("unchecked")
	public CircularArray() {
		Inner = (T[]) new Object[16];
	}

	void CheckIndex(int index) {
		if (index < 0 || index >= ItemCount)
			throw new IndexOutOfBoundsException();
	}

	int GetRealIndex(int index) {
		return index < getHeadCount() ? First + index : index - getHeadCount();
	}

	void IncFirst() {
		++First;
		if (First >= Inner.length)
			First -= Inner.length;
	}

	void DecFirst() {
		--First;
		if (First < 0)
			First += Inner.length;
	}

	@SuppressWarnings("unchecked")
	void Enlarge() {
		T[] enlarged = (T[]) new Object[2 * Inner.length];
		for (int i = 0; i < ItemCount; ++i)
			enlarged[i] = Inner[GetRealIndex(i)];
		Inner = enlarged;
		First = 0;
	}

	void Move(int from, int to, int length) {
		if (from < to) {
			for (int i = length - 1; i >= 0; --i)
				Inner[GetRealIndex(to + i)] = Inner[GetRealIndex(from + i)];
		} else {
			for (int i = 0; i < length; ++i)
				Inner[GetRealIndex(to + i)] = Inner[GetRealIndex(from + i)];
		}
	}

	void MoveForward(int from, int length) {
		Move(from, from + 1, length);
	}

	void MoveBackward(int from, int length) {
		Move(from, from - 1, length);
	}

	void InsertSpaceForward(int index) {
		++ItemCount;
		MoveForward(index, ItemCount - index - 1);
	}

	void InsertSpaceBackward(int index) {
		DecFirst();
		++ItemCount;
		MoveBackward(1, index + 1);
	}

	void InsertSpace(int index) {
		if (ItemCount >= Inner.length)
			Enlarge();
		if (index >= ItemCount / 2)
			InsertSpaceForward(index);
		else
			InsertSpaceBackward(index);
	}

	void RemoveSpaceForward(int index) {
		MoveBackward(index + 1, ItemCount - index - 1);
		--ItemCount;
	}

	void RemoveSpaceBackward(int index) {
		MoveForward(0, index);
		IncFirst();
		--ItemCount;
	}

	void RemoveSpace(int index) {
		if (index >= ItemCount / 2)
			RemoveSpaceForward(index);
		else
			RemoveSpaceBackward(index);
	}

	@Override
	public boolean add(T item) {
		if (ItemCount >= Inner.length)
			Enlarge();
		++ItemCount;
		Inner[GetRealIndex(ItemCount - 1)] = item;
		return true;
	}

	@Override
	public void add(int index, T element) {
		Insert(index, element);
	}

	public void Insert(int index, T item) {
		CheckIndex(index);
		if (index > 0) {
			InsertSpace(index);
			Inner[GetRealIndex(index)] = item;
		} else {
			if (ItemCount >= Inner.length)
				Enlarge();
			DecFirst();
			++ItemCount;
			Inner[GetRealIndex(0)] = item;
		}
	}

	@Override
	public boolean addAll(Collection<? extends T> c) {
		throw new UnsupportedOperationException();
	}

	@Override
	public boolean addAll(int index, Collection<? extends T> c) {
		throw new UnsupportedOperationException();
	}

	@Override
	public void clear() {
		First = 0;
		ItemCount = 0;
	}

	private int indexOf(T item, int startIdx, int count) {
		int endIdx = startIdx + count;
		for (int i = startIdx; i < endIdx; i++) {
			if ((Inner[i] != null && Inner[i].equals(item))
					|| (Inner[i] == null && item == null)) {
				return i;
			}
		}
		return -1;
	}

	@SuppressWarnings("unchecked")
	@Override
	public boolean contains(Object item) {
		return Contains((T) item);
	}

	public boolean Contains(T item) {
		return indexOf(item, First, getHeadCount()) >= 0
				|| indexOf(item, 0, getTailCount()) >= 0;
	}

	@Override
	public boolean containsAll(Collection<?> c) {
		throw new UnsupportedOperationException();
	}

	@Override
	public T get(int index) {
		CheckIndex(index);
		return Inner[GetRealIndex(index)];
	}

	@SuppressWarnings("unchecked")
	@Override
	public int indexOf(Object o) {
		return IndexOf((T) o);
	}

	public int IndexOf(T item) {
		int index = indexOf(item, First,
				Math.min(ItemCount, Inner.length - First));
		if (index >= 0)
			return index - First;
		else if (First + ItemCount > Inner.length)
			return indexOf(item, 0, First + ItemCount - Inner.length);
		else
			return -1;
	}

	@Override
	public boolean isEmpty() {
		return ItemCount == 0;
	}

	class InnerIterator<E> implements java.util.Iterator<E> {
		CircularArray<E> Array;
		int Index;

		public InnerIterator(CircularArray<E> array) {
			Array = array;
			Index = -1;
		}

		public E getCurrent() {
			return Array.get(Index);
		}

		public boolean MoveNext() {
			if (Index < Array.ItemCount) {
				++Index;
				return Index < Array.ItemCount;
			} else
				return false;
		}

		@Override
		public boolean hasNext() {
			return Index < Array.ItemCount;
		}

		@Override
		public E next() {
			if (MoveNext()) {
				return getCurrent();
			}
			return null;
		}

		@Override
		public void remove() {
			Array.remove(Index);
			Index--;
		}

		public void Reset() {
			Index = -1;
		}
	}

	InnerIterator<T> getIterator() {
		return new InnerIterator<T>(this);
	}

	@Override
	public Iterator<T> iterator() {
		return new InnerIterator<T>(this);
	}

	@Override
	public int lastIndexOf(Object o) {
		throw new UnsupportedOperationException();
	}

	@Override
	public ListIterator<T> listIterator() {
		throw new UnsupportedOperationException();
	}

	@Override
	public ListIterator<T> listIterator(int index) {
		throw new UnsupportedOperationException();
	}

	public boolean Remove(T item) {
		int index = IndexOf(item);
		if (index >= 0) {
			RemoveAt(index);
			return true;
		} else
			return false;
	}

	@SuppressWarnings("unchecked")
	@Override
	public boolean remove(Object o) {
		return Remove((T) o);
	}

	public void RemoveAt(int index) {
		if (index == 0) {
			IncFirst();
			--ItemCount;
		} else if (index == ItemCount - 1)
			--ItemCount;
		else {
			CheckIndex(index);
			RemoveSpace(index);
		}
	}

	@Override
	public T remove(int index) {
		T prevVal = get(index);
		RemoveAt(index);
		return prevVal;
	}

	@Override
	public boolean removeAll(Collection<?> c) {
		throw new UnsupportedOperationException();
	}

	@Override
	public boolean retainAll(Collection<?> c) {
		throw new UnsupportedOperationException();
	}

	@Override
	public T set(int index, T value) {
		CheckIndex(index);
		T prevVal = Inner[GetRealIndex(index)];
		Inner[GetRealIndex(index)] = value;
		return prevVal;
	}

	public boolean IsReadOnly() {
		return false;
	}

	@Override
	public int size() {
		return ItemCount;
	}

	@Override
	public List<T> subList(int fromIndex, int toIndex) {
		throw new UnsupportedOperationException();
	}

	public void CopyTo(T[] array, int at) {
		ArrayUtils.copyArray(Inner, First, array, at, getHeadCount());
		ArrayUtils.copyArray(Inner, 0, array, at + getHeadCount(),
				getTailCount());
	}

	@Override
	public Object[] toArray() {
		@SuppressWarnings("unchecked")
		T[] ret = (T[]) new Object[Inner.length];
		CopyTo(ret, 0);
		return ret;
	}

	@SuppressWarnings("unchecked")
	@Override
	public <E> E[] toArray(E[] a) {
		CopyTo((T[]) a, 0);
		return a;
	}

}
