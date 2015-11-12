using System;
using System.Diagnostics;

namespace sar.Control
{
	public class Interval
	{
		private static Stopwatch time;
		
		private static long Time
		{
			get
			{
				
				if (time == null)
				{
					time = new Stopwatch();
					time.Start();
				}
				
				return time.ElapsedMilliseconds;
			}
		}
		
		private long interval;
		private long lastTrigger;
		
		public Interval(long interval)
		{
			this.interval = interval;
			this.lastTrigger = Interval.Time;
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
