using System;
using System.Linq;

using sar.Timing;
using sar.Json;

namespace sar.FSM
{
	/// <summary>
	/// Description of Message.
	/// </summary>
	public class Message<T>
	{
		public Interval timer;
		public bool Sent { get; set; }
		public bool Recived { get; set; }
		public T payload { get; set; }
		public delegate void MessageCallback(JsonKeyValuePairs kvp);

		public MessageCallback ResponceCallback { get; private set; }
		public MessageCallback Timeout { get; private set; }
		
		public Message(T payload)
		{
			this.payload = payload;
		}
		
		public Message(T payload, MessageCallback responceCallback, int timeout, MessageCallback timeoutCallback)
			: this(payload)
		{
			this.ResponceCallback = responceCallback;
			this.timer = new Interval(timeout);
			this.timer.Reset();
		}
		
		public void RequestSent()
		{
			this.Sent = true;
		}
		
		public void ResponceRecived(JsonKeyValuePairs kvp)
		{
			this.Recived = true;
			
			if (this.ResponceCallback != null)
			{
				this.ResponceCallback(kvp);
			}
		}
	}
}
