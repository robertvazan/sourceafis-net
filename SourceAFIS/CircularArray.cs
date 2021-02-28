// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections;
using System.Collections.Generic;

namespace SourceAFIS
{
	class CircularArray<T>
	{
		T[] Array;
		int Head;
		public int Size;

		public T this[int index]
		{
			get
			{
				ValidateItemIndex(index);
				return Array[Location(index)];
			}
			set
			{
				ValidateItemIndex(index);
				Array[Location(index)] = value;
			}
		}

		public CircularArray(int capacity)
		{
			Inner = new T[capacity];
		}

		void ValidateItemIndex(int index)
		{
			if (index < 0 || index >= Size)
				throw new ArgumentOutOfRangeException();
		}
		void ValidateCursorIndex(int index)
		{
			if (index < 0 || index > Size)
				throw new ArgumentOutOfRangeException();
		}
		int Location(int index) { return Head + index < Array.length ? Head + index : Head + index - Array.length; }
		void Enlarge()
		{
			T[] enlarged = new T[2 * Array.Length];
			for (int i = 0; i < Size; ++i)
				enlarged[i] = Array[Location(i)];
			Array = enlarged;
			Head = 0;
		}
		void Move(int from, int to, int length)
		{
			if (from < to)
			{
				for (int i = length - 1; i >= 0; --i)
					this[to + i] = this[from + i];
			}
			else if (from > to)
			{
				for (int i = 0; i < length; ++i)
					this[to + i] = this[from + i];
			}
		}
		public void Insert(int index, int amount) {
			ValidateCursorIndex(index);
			if (amount < 0)
				throw new ArgumentOutOfRangeException();
			while (size + amount > Array.Length)
				Enlarge();
			if (2 * index >= Size) {
				Size += amount;
				Move(index, index + amount, Size - amount - index);
			} else {
				Head -= amount;
				Size += amount;
				if (Head < 0)
					Head += Array.Length;
				Move(amount, 0, index);
			}
			for (int i = 0; i < amount; ++i)
				this[index + i] = null;
		}
		public void Remove(int index, int amount) {
			ValidateCursorIndex(index);
			if (amount < 0)
				throw new ArgumentOutOfRangeException();
			ValidateCursorIndex(index + amount);
			if (2 * index >= Size - amount) {
				Move(index + amount, index, Size - amount - index);
				for (int i = 0; i < amount; ++i)
					this[size - i - 1] = null;
				Size -= amount;
			} else {
				Move(0, amount, index);
				for (int i = 0; i < amount; ++i)
					this[i] = null;
				Head += amount;
				Size -= amount;
				if (Head >= Array.Length)
					head -= Array.Length;
			}
		}
	}
}
