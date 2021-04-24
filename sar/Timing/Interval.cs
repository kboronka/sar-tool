/* Copyright (C) 2021 Kevin Boronka
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
		private bool paused;
		private long pauseStartTime;
		private long pauseTime;

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
				var pauseTime = this.PausedTime;

				lock (time)
				{
					return (time.ElapsedMilliseconds - lastTrigger) - pauseTime;
				}
			}
		}

		/// <summary>
		///  Returns number of milliseconds remianing before interval is ready
		/// </summary>
		public long Remaining
		{
			get
			{
				return this.SetPoint - Math.Min(this.ElapsedMilliseconds, this.SetPoint);
			}
		}

		/// <summary>
		///  Returns a percentage (0 - 100).  100% = Ready.
		/// </summary>
		public double PercentComplete
		{
			get
			{
				var elapsedTime = (double)Math.Min(this.ElapsedMilliseconds, this.SetPoint);
				return (elapsedTime / (double)this.SetPoint) * 100.0;
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

		public bool Paused
		{
			get
			{
				return this.paused;
			}
		}

		/// <summary>
		///  Returns number of milliseconds interval has been paused
		/// </summary>
		public long PausedTime
		{
			get
			{
				lock (time)
				{
					if (this.paused)
					{
						return this.pauseTime + (time.ElapsedMilliseconds - this.pauseStartTime);
					}

					return this.pauseTime;
				}
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
			this.paused = false;
			this.pauseTime = 0;
		}

		public Interval(long setPoint) : this(setPoint, setPoint)
		{

		}

		#endregion

		public void Reset()
		{
			lock (time)
			{
				this.lastTrigger += setPoint + pauseTime;
				this.pauseTime = 0;

				var timeToNextTrigger = time.ElapsedMilliseconds - this.lastTrigger;

				if (timeToNextTrigger > setPoint || timeToNextTrigger < 0)
				{
					this.lastTrigger = time.ElapsedMilliseconds;
				}
			}
		}

		public void Pause()
		{
			lock (time)
			{
				if (!this.paused)
				{
					this.paused = true;
					this.pauseStartTime = time.ElapsedMilliseconds;
				}
			}
		}

		public void Continue()
		{
			lock (time)
			{
				if (this.paused)
				{
					this.paused = false;
					this.pauseTime += (time.ElapsedMilliseconds - this.pauseStartTime);
				}
			}
		}
	}
}
