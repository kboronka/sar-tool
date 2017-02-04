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

using sar;
using sar.Timing;

namespace sar.CNC
{
	public class Grbl
	{
		public PositionVector MachinePosition { get; private set; }
		public PositionVector WorkCoordinateOffset { get; private set; }
		public int PlannerBlocksAvailble;
		public int RxBufferBytesAvailble;
		public int FeedRate;
		public int RPM;
		
		
		private CommandQueue queue;
		private Thread readLoopThread;
		private bool loopShutdownRequest = false;
		private bool loopStopped = false;
		private GrblInputParser parser;
		
		private SerialPort port;

		public Grbl(string portName)
		{
			this.port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
			this.parser = new GrblInputParser();
			this.queue = new CommandQueue();
			
			Logger.Log("Port = " + portName);
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
					Thread.Sleep(10);
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
			Idle,
			Faulted,
			CheckStatus,
			ReadInput,
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
		
		private Interval statusCheck = new Interval(250, 0);
		private ProcessState state;
		private void StateMachine()
		{
			switch (state)
			{
				case ProcessState.Init:
					port.Open();
					var settings = new List<string>();
					settings.Add("$0=10");			// (step pulse, usec)
					settings.Add("$1=25");		// (step idle delay, msec)
					settings.Add("$2=0");			// (step port invert mask:00000000)
					settings.Add("$3=6");			// (dir port invert mask:00000110)
					settings.Add("$4=0");			// (step enable invert, bool)
					settings.Add("$5=0");			// (limit pins invert, bool)
					settings.Add("$6=0");			// (probe pin invert, bool)
					settings.Add("$10=3");			// (status report mask:00000011)
					settings.Add("$11=0.020");		// (junction deviation, mm)
					settings.Add("$12=0.002");		// (arc tolerance, mm)
					settings.Add("$13=0");			//(report inches, bool)
					settings.Add("$20=0");			// (soft limits, bool)
					settings.Add("$21=0");			// (hard limits, bool)
					settings.Add("$22=0");			// (homing cycle, bool)
					settings.Add("$23=1");			// (homing dir invert mask:00000001)
					settings.Add("$24=50.000");		// (homing feed, mm/min)
					settings.Add("$25=635.000");	// (homing seek, mm/min)
					settings.Add("$26=250");		// (homing debounce, msec)
					settings.Add("$27=1.000");		// (homing pull-off, mm)
					settings.Add("$100=314.961");	// (x, step/mm)
					settings.Add("$101=314.961");	// (y, step/mm)
					settings.Add("$102=314.961");	// (z, step/mm)
					settings.Add("$110=635.000");	// (x max rate, mm/min)
					settings.Add("$111=635.000");	// (y max rate, mm/min)
					settings.Add("$112=635.000");	// (z max rate, mm/min)
					settings.Add("$120=50.000");		// (x accel, mm/sec^2)
					settings.Add("$121=50.000");		// (y accel, mm/sec^2)
					settings.Add("$122=50.000");		// (z accel, mm/sec^2)
					settings.Add("$130=225.000");	// (x max travel, mm)
					settings.Add("$131=125.000");	// (y max travel, mm)
					settings.Add("$132=170.000");	// (z max travel, mm)
					
					foreach (var setting in settings)
					{
						port.WriteLine(setting);
						Thread.Sleep(250);
					}
					
					state = ProcessState.CheckIfOnline;
					break;
					
					
				case ProcessState.CheckIfOnline:
					if (port.IsOpen)
					{
						state = ProcessState.Idle;
					}
					else
					{
						state = ProcessState.Init;
						Thread.Sleep(500);
					}
					
					break;
					

				case ProcessState.Idle:
					if (statusCheck.Ready)
					{
						state = ProcessState.CheckStatus;
					}
					else
					{
						state = ProcessState.ReadInput;
					}
					
					break;
					
				case ProcessState.ReadInput:
					var input = parser.Read(port);
					if (input == null)
					{
						state = ProcessState.Idle;
					}
					else if (input is GrblStatusResponce)
					{
						var status = (GrblStatusResponce)input;
						state = ProcessState.WriteOutput;
						
						this.PlannerBlocksAvailble = status.PlannerBlocksAvailble;
						this.RxBufferBytesAvailble = status.RxBufferBytesAvailble;
						
						if (MachinePosition == null)
						{
							MachinePosition = status.MachinePosition;
						}
						else
						{
							MachinePosition.Move(status.MachinePosition);
						}
						
						GrblWebSocket.SendToWebSocketClients(this.ToJSON());
						Logger.Log("x = " + status.MachinePosition.X.ToString());
					}
					
					break;
					
				case ProcessState.WriteOutput:
					var nextCommand = queue.GetNextCommand();
					
					if (!String.IsNullOrEmpty(nextCommand) && this.PlannerBlocksAvailble > 0 && this.RxBufferBytesAvailble > nextCommand.Length)
					{
						port.WriteLine(nextCommand);
						queue.Remove(nextCommand);
					}
					
					state = ProcessState.Idle;
					break;
					
				case ProcessState.CheckStatus:
					port.Write("?");
					state = ProcessState.Idle;
					break;
			}
		}
		
		private bool Connect()
		{
			if (!port.IsOpen)
			{
				Logger.Log("Opening Comm Port");
				port.Open();
			}
			
			return port.IsOpen;
		}
		
		public void SendCommand(string ncCommand, string comment)
		{
			this.queue.Add(ncCommand);
		}
		
		public string ToJSON()
		{
			var json = new Dictionary<string, object>();
			json.Add("wX", this.MachinePosition.X);
			json.Add("wY", this.MachinePosition.Y);
			json.Add("wZ", this.MachinePosition.Z);

			json.Add("running", this.queue.Length > 0);
			json.Add("motionBuffer", this.PlannerBlocksAvailble);
			json.Add("rxBuffer", this.RxBufferBytesAvailble);
		}
	}
}
