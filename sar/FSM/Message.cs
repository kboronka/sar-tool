/* Copyright (C) 2017 Kevin Boronka
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Linq;
using System.Threading;

using sar.Timing;
using sar.Json;

namespace sar.FSM
{
	/// <summary>
	/// Description of Message.
	/// </summary>
	public class Message<T>
	{
		private int timeout;
		private object recivedLock = new object();
		public bool Sent { get; set; }
		public bool Recived { get; set; }
		
		private bool shutdownLoop;
		private Thread timeoutThread;
		public bool Expired { get; private set; }

		public T PayLoad { get; set; }
		public delegate void MessageCallback(JsonKeyValuePairs kvp);
		public delegate void MessageExpiredCallback(T payload);

		public MessageCallback ResponceCallback { get; private set; }
		public MessageExpiredCallback TimeoutCallback { get; private set; }
		
		public Message(T payload)
		{
			this.PayLoad = payload;
		}
		
		~Message()
		{
			shutdownLoop = true;
		}
		
		public Message(T payload, MessageCallback responceCallback, int timeout, MessageExpiredCallback timeoutCallback)
			: this(payload)
		{
			this.ResponceCallback = responceCallback;
			this.TimeoutCallback = timeoutCallback;
			this.timeout = timeout;
			
			timeoutThread = new Thread(TimeoutLoop);
			timeoutThread.Name = "Message Timeout Thread";
			timeoutThread.IsBackground = true;
			timeoutThread.Priority = ThreadPriority.Lowest;
			timeoutThread.Start();
		}
		
		public void RequestSent()
		{
			this.Sent = true;
		}
		
		public void ResponceRecived(JsonKeyValuePairs kvp)
		{
			lock (recivedLock)
			{
				if (this.Expired) return;
				
				this.Recived = true;
				
				if (this.ResponceCallback != null)
				{
					this.ResponceCallback(kvp);
				}
			}
		}
		
		private void TimeoutLoop()
		{
			var timeoutTimer = new Interval(timeout);
			
			while (!shutdownLoop && !Expired)
			{
				Thread.Sleep(10);
				
				lock (recivedLock)
				{
					if (!Recived && timeoutTimer.Ready)
					{
						Expired = true;
						
						if (this.TimeoutCallback != null)
						{
							this.TimeoutCallback(PayLoad);
						}
					}
				}
			}
		}
	}
}
