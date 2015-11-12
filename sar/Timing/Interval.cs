/* Copyright (C) 2015 Kevin Boronka
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
		
		private long Time
		{
			get
			{
				return time.ElapsedMilliseconds;
			}
		}
		
		private long interval;
		private long lastTrigger;
		
		public Interval(long interval, long firstRunDelay)
		{
			this.time = new Stopwatch();
			this.time.Start();			
			this.interval = interval;
			this.lastTrigger = this.Time - interval + firstRunDelay;
		}
		
		public bool Ready
		{
			get
			{
				if (Time - lastTrigger > interval)
				{
					this.lastTrigger += interval;
					return true;
				}
				
				return false;
			}
		}
	}
}
