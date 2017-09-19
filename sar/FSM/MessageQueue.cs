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
		
		public void QueueItem(T message, Message<T>.MessageCallback responceCallback, int timeout, Message<T>.MessageExpiredCallback timeoutCallback)
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
						return message.PayLoad;
					}
				}
			}
			
			return default(T);
		}
		
		public void Cleanup()
		{
			lock (queueLock)
			{
				queued.RemoveAll(m => m.Recived || m.Expired);
			}
		}
	}
}
