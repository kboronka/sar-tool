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
		
		private Thread readLoopThread;
		private bool loopShutdownRequest = false;
		private bool loopStopped = false;
		private GrblInputParser parser;
		
		private SerialPort port;

		public Grbl(string portName)
		{
			this.port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
			this.parser = new GrblInputParser();
			
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
						
						if (MachinePosition == null)
						{
							MachinePosition = status.MachinePosition;
						}
						else
						{
							MachinePosition.Move(status.MachinePosition);
						}
						Logger.Log("x = " + status.MachinePosition.X.ToString());
					}
					
					break;
					
				case ProcessState.WriteOutput:
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
			// TODO: implement
		}
	}
}
