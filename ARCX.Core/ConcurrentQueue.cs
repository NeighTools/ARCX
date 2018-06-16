using System.Collections.Generic;
using System.Threading;

namespace ARCX.Core
{
	public class ConcurrentQueue<T>
	{
		protected List<T> internalList;

		public int Count
		{
			get
			{
				lock (internalList)
				{
					return internalList.Count;
				}
			}
		}

		public ConcurrentQueue()
		{
			internalList = new List<T>();
		}

		public ConcurrentQueue(int capacity)
		{
			internalList = new List<T>(capacity);
		}

		public ConcurrentQueue(IEnumerable<T> items)
		{
			internalList = new List<T>(items);
		}

		public void Enqueue(T item)
		{
			lock (internalList)
			{
				internalList.Add(item);
			}
		}

		public T Peek()
		{
			lock (internalList)
			{
				return internalList[0];
			}
		}

		public T Dequeue()
		{
			lock (internalList)
			{
				T item = Peek();

				internalList.RemoveAt(0);

				return item;
			}
		}

		public bool TryDequeue(out T item, int millisecondsTimeout)
		{
			item = default(T);

			bool lockStatus = Monitor.TryEnter(internalList, millisecondsTimeout);

			if (!lockStatus)
				return false;

			item = Dequeue();

			Monitor.Exit(internalList);

			return true;
		}
	}
}
