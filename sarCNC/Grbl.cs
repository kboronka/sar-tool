/*
 * Created by SharpDevelop.
 * User: Kevin
 * Date: 1/24/2017
 * Time: 9:27 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

using sar;
using sar.Tools;
using sar.Timing;

using sar.CNC.Http;

namespace sar.CNC
{
	public class Grbl
	{
		public PositionVector MachinePosition { get; private set; }
		public PositionVector WorkCoordinateOffset { get; private set; }
		public PositionVector WorkCoordinate { get; private set; }
		public int PlannerBlocksAvailble;
		public int RxBufferBytesAvailble;
		public int FeedRate;
		public int RPM;
		
		private CommandQueue queue;
		private Thread readLoopThread;
		private bool loopShutdownRequest = false;
		private bool loopStopped = false;
		private GrblInputParser parser;
		private bool jobActive;
		private Stopwatch jobTimer;
		

		
		public bool JobActive
		{
			get  { return jobActive; }
			set
			{
				jobActive = value;
				
				if (value)
				{
					jobTimer = new Stopwatch();
					jobTimer.Start();
					Logger.Log("*****************************************************");
					Logger.Log("Job Started");
					Logger.Log("*****************************************************");
				}
				else
				{
					var totalTime = jobTimer.ElapsedMilliseconds;
					Logger.Log("*****************************************************");
					Logger.Log("Job Complete");
					Logger.Log("Total Time: " + totalTime.ToString() + "ms");
					Logger.Log("*****************************************************");
				}
			}
		}
		
		public Grbl(string portName)
		{
			this.parser = new GrblInputParser(portName);
			this.queue = new CommandQueue();
			
			this.readLoopThread = new Thread(this.Loop);
			this.readLoopThread.IsBackground = true;
			this.readLoopThread.Start();
		}
		
		public void Stop()
		{
			loopShutdownRequest = true;
			this.readLoopThread.Join();
		}
		
		private void Loop()
		{
			while (!loopStopped)
			{
				try
				{
					StateMachine();
					Thread.Sleep(1);
				}
				catch (Exception ex)
				{
					Logger.Log(ex);
					Thread.Sleep(2000);
				}
			}
		}

		public enum ProcessState
		{
			Init,
			CheckIfOnline,
			CheckSettings,
			Idle,
			Faulted,
			CheckStatus,
			WriteOutput,
			Stop,
			Stopping,
			Stopped
		}

		public enum Command
		{
			Begin,
			End,
			Pause,
			Resume,
			Exit
		}
		
		private Interval statusCheck = new Interval(105, 0);
		private Interval readPort = new Interval(10, 0);
		
		private ProcessState state;
		private void StateMachine()
		{
			switch (state)
			{
				case ProcessState.Init:
					this.parser.Open = true;
					
					state = ProcessState.CheckIfOnline;
					break;
					
				case ProcessState.CheckIfOnline:
					if (this.parser.Open)
					{
						state = ProcessState.CheckSettings;
						Thread.Sleep(300);
					}
					else
					{
						state = ProcessState.Init;
						Thread.Sleep(500);
					}
					
					break;
					
				case ProcessState.CheckSettings:
					var settings = new List<string>();
					settings.Add("$0=10");			// (step pulse, usec)
					settings.Add("$1=25");			// (step idle delay, msec)
					settings.Add("$2=0");			// (step port invert mask:00000000)
					settings.Add("$3=3");			// (dir port invert mask:00000011)
					settings.Add("$4=0");			// (step enable invert, bool)
					settings.Add("$5=0");			// (limit pins invert, bool)
					settings.Add("$6=0");			// (probe pin invert, bool)
					settings.Add("$10=3");			// (status report mask:00000011)
					settings.Add("$11=0.020");		// (junction deviation, mm)
					settings.Add("$12=0.001");		// (arc tolerance, mm)
					settings.Add("$13=0");			//(report inches, bool)
					settings.Add("$20=0");			// (soft limits, bool)
					settings.Add("$21=0");			// (hard limits, bool)
					settings.Add("$22=0");			// (homing cycle, bool)
					settings.Add("$23=1");			// (homing dir invert mask:00000001)
					settings.Add("$24=50.000");		// (homing feed, mm/min)
					settings.Add("$25=635.000");	// (homing seek, mm/min)
					settings.Add("$26=250");		// (homing debounce, msec)
					settings.Add("$27=1.000");		// (homing pull-off, mm)
					settings.Add("$100=320.000");	// (x, step/mm)
					settings.Add("$101=320.000");	// (y, step/mm)
					settings.Add("$102=320.000");	// (z, step/mm)
					settings.Add("$110=3000.000");	// (x max rate, mm/min)
					settings.Add("$111=3000.000");	// (y max rate, mm/min)
					settings.Add("$112=3000.000");	// (z max rate, mm/min)
					settings.Add("$120=50.000");	// (x accel, mm/sec^2)
					settings.Add("$121=50.000");	// (y accel, mm/sec^2)
					settings.Add("$122=50.000");	// (z accel, mm/sec^2)
					settings.Add("$130=225.000");	// (x max travel, mm)
					settings.Add("$131=125.000");	// (y max travel, mm)
					settings.Add("$132=170.000");	// (z max travel, mm)
					
					var result = this.parser.SendCommand("$$");
					if (String.IsNullOrEmpty(result))
					{
						this.parser.Open = false;
						state = ProcessState.Init;
						Thread.Sleep(5000);
						return;
					}
					
					var currentSettings = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
					
					foreach (var setting in currentSettings)
					{
						settings.RemoveAll(s => s == setting);
					}
					
					foreach (var setting in settings)
					{
						QueueCommand(setting);
					}
					
					if (settings.Count > 0)
					{
						this.parser.SendCommand(((char)24).ToString());
					}
					
					state = ProcessState.Idle;
					break;
					

				case ProcessState.Idle:
					if (statusCheck.Ready)
					{
						state = ProcessState.CheckStatus;
					}
					else
					{
						state = ProcessState.WriteOutput;
					}
					
					break;
					
					
				case ProcessState.WriteOutput:
					state = ProcessState.Idle;
					while (this.PlannerBlocksAvailble > 0)
					{
						var nextCommand = queue.GetNextCommand();
						
						// no more commands
						if (String.IsNullOrEmpty(nextCommand))
						{
							break;
						}
						
						// grbl rxBuffer full
						if (this.RxBufferBytesAvailble < (nextCommand.Length + 5))
						{
							break;
						}

						this.RxBufferBytesAvailble -= (nextCommand.Length + 2);
						this.PlannerBlocksAvailble --;
						
						var message = this.parser.SendCommand(nextCommand);
						queue.Remove(nextCommand);
						
						if (!String.IsNullOrEmpty(message) && message.Contains("[MSG:Pgm End]"))
						{
							this.JobActive = false;
						}
					}
					
					break;
					
				case ProcessState.CheckStatus:
					state = ProcessState.Idle;
					
					var status = this.parser.ReadStaus();
					if (status == null) return;

					
					this.PlannerBlocksAvailble = status.PlannerBlocksAvailble;
					this.RxBufferBytesAvailble = status.RxBufferBytesAvailble;
					
					if (MachinePosition == null)
					{
						MachinePosition = status.MachinePosition;
						WorkCoordinate = new PositionVector(MachinePosition.X, MachinePosition.Y, MachinePosition.Z);
					}
					else
					{
						MachinePosition.Move(status.MachinePosition);
						WorkCoordinate.Move(MachinePosition);
					}
					
					if (status.WorkCoordinateOffset != null)
					{
						WorkCoordinate.Offset(status.WorkCoordinateOffset);
					}
					
					GrblWebSocket.SendToWebSocketClients(this.NamedParameters.ToJSON());
					break;
			}
		}
		
		
		public void QueueCommand(string ncCommand)
		{
			this.queue.Add(ncCommand);
		}
		
		public Dictionary<string, object> NamedParameters
		{
			get
			{
				var json = new Dictionary<string, object>();
				var statusJson = new Dictionary<string, object>();
				statusJson.Add("wX", this.WorkCoordinate.X);
				statusJson.Add("wY", this.WorkCoordinate.Y);
				statusJson.Add("wZ", this.WorkCoordinate.Z);

				statusJson.Add("running", this.queue.Length > 0);
				statusJson.Add("motionBuffer", this.PlannerBlocksAvailble);
				statusJson.Add("rxBuffer", this.RxBufferBytesAvailble);
				
				statusJson.Add("motionQueue", this.queue.Length);
				
				json.Add("status", statusJson);
				return json;
			}
		}
	}
}
