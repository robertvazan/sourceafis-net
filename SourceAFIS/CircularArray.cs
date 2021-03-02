// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections;
using System.Collections.Generic;

namespace SourceAFIS
{
	class CircularArray<T>
	{
		internal T[] Array;
		internal int Head;
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
			Array = new T[capacity];
		}

		internal void ValidateItemIndex(int index)
		{
			if (index < 0 || index >= Size)
				throw new ArgumentOutOfRangeException();
		}
		internal void ValidateCursorIndex(int index)
		{
			if (index < 0 || index > Size)
				throw new ArgumentOutOfRangeException();
		}
		internal int Location(int index) { return Head + index < Array.Length ? Head + index : Head + index - Array.Length; }
		internal void Enlarge()
		{
			T[] enlarged = new T[2 * Array.Length];
			for (int i = 0; i < Size; ++i)
				enlarged[i] = Array[Location(i)];
			Array = enlarged;
			Head = 0;
		}
		internal void Move(int from, int to, int length)
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
			while (Size + amount > Array.Length)
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
				this[index + i] = default(T);
		}
		public void Remove(int index, int amount) {
			ValidateCursorIndex(index);
			if (amount < 0)
				throw new ArgumentOutOfRangeException();
			ValidateCursorIndex(index + amount);
			if (2 * index >= Size - amount) {
				Move(index + amount, index, Size - amount - index);
				for (int i = 0; i < amount; ++i)
					this[Size - i - 1] = default(T);
				Size -= amount;
			} else {
				Move(0, amount, index);
				for (int i = 0; i < amount; ++i)
					this[i] = default(T);
				Head += amount;
				Size -= amount;
				if (Head >= Array.Length)
					Head -= Array.Length;
			}
		}
	}
}
