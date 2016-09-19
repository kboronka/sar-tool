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
	public class ScanTimeMonitor
	{
		private Interval log;
		
		private string description;
		private long logInterval;
		private Stopwatch time;
		private long lastTrigger;
		
		private long total;
		private long min;
		private long max;
		private long counts;

		public ScanTimeMonitor(string description, long logInterval)
		{
			this.log = new Interval(logInterval);
			
			this.description = description;
			this.logInterval = logInterval;
			
			this.time = new Stopwatch();
			this.time.Start();
			
			this.counts = 0;
			this.total = 0;
			this.min = long.MaxValue;
			this.min = long.MinValue;
		}
		
		public void Start()
		{
			this.lastTrigger = time.ElapsedMilliseconds;
		}
		
		public void Stop()
		{
			var currentTime = time.ElapsedMilliseconds;
			var scantime = currentTime - lastTrigger;
			lastTrigger = currentTime;
			
			this.counts++;
			this.total += scantime;
			this.min = Math.Min(this.min, scantime);
			this.max = Math.Max(this.max, scantime);
			
			if (this.log.Ready)
			{
				Logger.Log(description + " - " +
				           " min: " + this.min.ToString() +
				           " avg: " + (this.total / this.counts).ToString() +
				           " max: " + this.max.ToString());
				
				this.counts = 0;
				this.total = 0;
				this.min = long.MaxValue;
				this.max = long.MinValue;
			}
		}
	}
}
