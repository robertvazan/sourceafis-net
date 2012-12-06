/**
 * @author Veaceslav Dubenco
 * @since 10.10.2012
 */
package sourceafis.general;

import java.util.Collection;
import java.util.Iterator;
import java.util.List;
import java.util.ListIterator;

/**
 * 
 */
public final class ReversedList<T> implements List<T> {
	List<T> Inner;

	public ReversedList(List<T> inner) {
		Inner = inner;
	}

	public int IndexOf(T item) {
		for (int i = size() - 1; i >= 0; --i)
			if (Inner.get(i).equals(item))
				return size() - i - 1;
		return -1;
	}

	public void Insert(int position, T item) {
		Inner.add(size() - position, item);
	}

	public void RemoveAt(int position) {
		Inner.remove(size() - position - 1);
	}

	@Override
	public T get(int index) {
		return Inner.get(size() - index - 1);
	}

	@Override
	public T set(int index, T value) {
		T prevVal = Inner.get(size() - index - 1);
		Inner.set(size() - index - 1, value);
		return prevVal;
	}

	@Override
	public boolean add(T item) {
		Inner.add(0, item);
		return true;
	}

	public void Clear() {
		Inner.clear();
	}

	@Override
	public boolean contains(Object item) {
		return Inner.contains(item);
	}

	public void CopyTo(T[] array, int at) {
		ArrayUtils.copyArray(Inner.toArray(), array, at);
		ArrayUtils.reverse(array, at, size());
	}

	@Override
	public boolean remove(Object item) {
		int index = indexOf(item);
		if (index >= 0) {
			Inner.remove(size() - index - 1);
			return true;
		} else
			return false;
	}

	public int Count() {
		return Inner.size();
	}

	class InnerIterator<E> implements Iterator<E> {
		List<E> Inner;
		int Position;

		public InnerIterator(List<E> inner) {
			Inner = inner;
			Position = Inner.size();
		}

		public E getCurrent() {
			return Inner.get(Position);
		}

		public boolean MoveNext() {
			if (Position >= 0) {
				--Position;
				return Position >= 0;
			} else
				return false;
		}

		public void Reset() {
			Position = Inner.size();
		}

		@Override
		public boolean hasNext() {
			return Position > 0;
		}

		@Override
		public E next() {
			if (MoveNext()) {
				return getCurrent();
			} else {
				return null;
			}
		}

		@Override
		public void remove() {
			Inner.remove(Position);
		}
	}

	InnerIterator<T> getIterator() {
		return new InnerIterator<T>(Inner);
	}

	@Override
	public void add(int index, T element) {
		Insert(index, element);
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
		Inner.clear();
	}

	@Override
	public boolean containsAll(Collection<?> c) {
		throw new UnsupportedOperationException();
	}

	@Override
	public int indexOf(Object o) {
		int innerIdx = Inner.indexOf(o);
		if (innerIdx >= 0) {
			return Inner.size() - innerIdx - 1;
		} else {
			return -1;
		}
	}

	@Override
	public boolean isEmpty() {
		return Inner.isEmpty();
	}

	@Override
	public Iterator<T> iterator() {
		return new InnerIterator<T>(Inner);
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

	@Override
	public T remove(int index) {
		int innerIdx = Inner.size() - index - 1;
		T prevVal = Inner.get(innerIdx);
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
	public int size() {
		return Inner.size();
	}

	@Override
	public List<T> subList(int fromIndex, int toIndex) {
		throw new UnsupportedOperationException();
	}

	@Override
	public Object[] toArray() {
		Object[] ret = Inner.toArray();
		ArrayUtils.reverse(ret, 0, Inner.size());
		return ret;
	}

	@Override
	public <E> E[] toArray(E[] a) {
		Inner.toArray(a);
		ArrayUtils.reverse(a, 0, a.length);
		return a;
	}

}
