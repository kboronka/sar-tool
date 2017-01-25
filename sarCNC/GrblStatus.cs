/*
 * Created by SharpDevelop.
 * User: Kevin
 * Date: 7/8/2016
 * Time: 6:48 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using sar.Tools;

namespace sar.CNC
{
	public static class GrblStatus
	{
		private static double mX = 0.0f;
		private static double mY = 0.0f;
		private static double mZ = 0.0f;
		private static double wX = 0.0f;
		private static double wY = 0.0f;
		private static double wZ = 0.0f;
		
		private static int motionBuffer = 0;
		public static int MotionBuffer
		{
			get { return motionBuffer; }
			set	{ motionBuffer = value; }
		}
		
		private static int rxBuffer = 0;
		public static int RxBuffer
		{
			get { return rxBuffer; }
			set	{ rxBuffer = value; }
		}
		
		private static GrblCommand currentCommand;
		public static GrblCommand CurrentCommand
		{
			set
			{
				if (value != null)
				{
					currentCommand = value;
				}
			}
		}
		
		
		private static Stopwatch runStopWatch;
		private static long runTime = 0;

		private static bool running = false;
		public static bool Running
		{
			get { return running; }
			set
			{
				if (running != value)
				{
					if (value)
					{
						// just started
						runStopWatch = new Stopwatch();
						runStopWatch.Start();
					}
					else
					{
						// just stopped
						runStopWatch.Stop();
					}
					
					running = value;
				}
			}
		}
		
		public static int CommandsTotal { get; set; }
		public static int CommandsCompleted { get; set; }

		public static void Parse(string data, bool commandQueued)
		{
			Logger.Log(data);
			var re = new Regex(@"<(.*),MPos:(-?\d+.\d+),(-?\d+.\d+),(-?\d+.\d+),WPos:(-?\d+.\d+),(-?\d+.\d+),(-?\d+.\d+),Buf:(\d+),RX:(\d+)>");
			var reMatch = re.Match(data);
			
			if (reMatch.Groups.Count > 0)
			{
				Running = reMatch.Groups[1].Value != "Idle" || commandQueued;
				mX = Convert.ToDouble(reMatch.Groups[2].Value);
				mY = Convert.ToDouble(reMatch.Groups[3].Value);
				mZ = Convert.ToDouble(reMatch.Groups[4].Value);
				wX = Convert.ToDouble(reMatch.Groups[5].Value);
				wY = Convert.ToDouble(reMatch.Groups[6].Value);
				wZ = Convert.ToDouble(reMatch.Groups[7].Value);
				motionBuffer = Convert.ToInt32(reMatch.Groups[8].Value);
				rxBuffer = Convert.ToInt32(reMatch.Groups[9].Value);
			}
			
			
			if (runStopWatch != null)
			{
				runTime = runStopWatch.ElapsedMilliseconds;
			}
		}
		
		public static string ToJSON()
		{
			var json = new Dictionary<string, object>();
			var statusJSON = new Dictionary<string, object>();
			statusJSON.Add("running", running);
			statusJSON.Add("mX", mX);
			statusJSON.Add("mY", mY);
			statusJSON.Add("mZ", mZ);
			statusJSON.Add("wX", wX);
			statusJSON.Add("wY", wY);
			statusJSON.Add("wZ", wZ);
			statusJSON.Add("motionBuffer", motionBuffer);
			statusJSON.Add("rxBuffer", rxBuffer);
			statusJSON.Add("runTime", runTime);
			statusJSON.Add("commandsTotal", CommandsTotal);
			statusJSON.Add("commandsComplete", CommandsCompleted);
			json.Add("status", statusJSON);
			if (currentCommand != null)	json.Add("currentCommand", currentCommand.NamedParameters);
			
			return json.ToJSON();
		}
	}
}
