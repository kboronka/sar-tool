/* Copyright (C) 2016 Kevin Boronka
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
using System.Diagnostics;

namespace sar.Timing
{
	public class Interval
	{
		private Stopwatch time;
		private long setPoint;
		private long lastTrigger;
		
		#region properties
		
		public long Clock
		{
			get
			{
				lock (time)
				{
					return time.ElapsedMilliseconds;
				}
			}
		}
		public long ElapsedMilliseconds
		{
			get
			{
				lock (time)
				{
					return time.ElapsedMilliseconds - lastTrigger;
				}
			}
		}
		
		public long SetPoint
		{
			get
			{
				lock (time)
				{
					return setPoint;
				}
			}
		}
		
		public bool Ready
		{
			get
			{
				if (this.ElapsedMilliseconds > setPoint)
				{
					this.Reset();
					return true;
				}
				
				return false;
			}
		}
		
		#endregion
		
		#region constructor
		
		public Interval(long setPoint, long firstRunDelay)
		{
			this.time = new Stopwatch();
			this.time.Start();
			this.setPoint = setPoint;
			this.lastTrigger = time.ElapsedMilliseconds - setPoint + firstRunDelay;
		}		
		
		public Interval(long setPoint)
		{
			this.time = new Stopwatch();
			this.time.Start();
			this.setPoint = setPoint;
			this.lastTrigger = time.ElapsedMilliseconds;
		}
		
		#endregion
		
		public void Reset()
		{
			lock (time)
			{
				this.lastTrigger += setPoint;
				
				var timeToNextTrigger = time.ElapsedMilliseconds - this.lastTrigger;
					
				if (timeToNextTrigger > setPoint ||timeToNextTrigger < 0)
				{
					this.lastTrigger = time.ElapsedMilliseconds;
				}
			}
		}
	}
}
