using System;
using System.Collections.Generic;
using System.Linq;

namespace sar.FSM
{
	/// <summary>
	/// Description of MessageFIFO.
	/// </summary>
	public class MessageQueue<T>
	{
		private readonly object queueLock = new object();
		private readonly List<Message<T>> queued;
		
		public bool Available { get { return queued.Any(m => !m.Sent); } }
		
		public List<Message<T>> Messages
		{
			get
			{
				return queued;
			}
		}
		
		public MessageQueue()
		{
			lock (queueLock)
			{
				queued = new List<Message<T>>();
			}
		}
		
		public void QueueItem(T message)
		{
			lock (queueLock)
			{
				queued.Add(new Message<T>(message));
			}
		}
		
		public void QueueItem(T message, Message<T>.MessageCallback responceCallback, int timeout, Message<T>.MessageCallback timeoutCallback)
		{
			lock (queueLock)
			{
				queued.Add(new Message<T>(message, responceCallback, timeout, timeoutCallback));
			}
		}
		
		public T DequeueItem()
		{
			lock (queueLock)
			{
				foreach (var message in queued)
				{
					if (!message.Sent)
					{
						message.Sent = true;
						return message.payload;
					}
				}
			}
			
			return default(T);
		}
		
		public void Cleanup()
		{
			lock (queueLock)
			{
				queued.RemoveAll(m => m.Recived);
			}
		}
	}
}
