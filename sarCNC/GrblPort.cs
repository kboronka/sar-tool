/*
 * Created by SharpDevelop.
 * User: kboronka
 * Date: 7/7/2016
 * Time: 1:03 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;

using sar.Tools;
using sar.Timing;
using sar.CNC.Http;

namespace sar.CNC
{
	public class GrblPort
	{
		public static List<string> Ports
		{
			get
			{
				return GrblPort.Ports;
			}
		}
		
		private SerialPort port;
		private List<GrblCommand> commandQueue = new List<GrblCommand>();
		private List<GrblCommand> commandSent = new List<GrblCommand>();
		private List<GrblCommand> commandBuffered = new List<GrblCommand>();
		private List<GrblCommand> commandHistory = new List<GrblCommand>();
		private int nextCommandID;
		private GrblCommand currentCommand;
		
		private GrblCommand CurrentCommand
		{
			get { return currentCommand; }
			set
			{
				if (this.currentCommand != null)
				{
					this.currentCommand.Active = false;
					
					if (value != null)
					{
						value.Active = true;
					}
				}
				
				this.currentCommand = value;
				GrblStatus.CurrentCommand = value;
			}
		}
		
		public bool IsOpen
		{
			get
			{
				return port.IsOpen;
			}
		}
		
		public GrblPort(string portName)
		{
			port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
			readLoopShutdown = false;
			//serialPort1.DataReceived += new SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
			this.readLoopThread = new Thread(this.ReadLoop);
			this.readLoopThread.IsBackground = true;
			this.readLoopThread.Start();
		}
		
		~GrblPort()
		{
			readLoopShutdown = true;
			this.readLoopThread.Join();
		}
		
		public bool SendCommand(string command)
		{
			lock (port)
			{
				if (command == "?")
				{

				}
				else if (command.Length == 1)
				{
					commandQueue.Insert(0, new GrblCommand(nextCommandID++, command));
				}
				else
				{
					commandQueue.Add(new GrblCommand(nextCommandID++, command));
				}
				
				//port.Write(command + "\n");
				return true;
			}
		}

		#region service
		
		private Thread readLoopThread;
		private bool readLoopShutdown = false;
		
		private void ReadLoop()
		{
			var readInterval = new Interval(250, 500);
			var requestStatusInterval = new Interval(350, 500);
			var txInterval = new Interval(5000, 500);
			var running = GrblStatus.Running;
			
			string rxBuffer = "";
			
			while (!readLoopShutdown)
			{
				
				// open the port if it's not already oppend
				try
				{
					lock(port)
					{
						if (!port.IsOpen)
						{
							port.Open();
							Thread.Sleep(1000);
						}
					}
				}
				catch
				{
					Thread.Sleep(500);
					continue;
				}
				
				try
				{
					if (readInterval.Ready)
					{
						lock(port)
						{
							if(port.BytesToRead > 0)
							{
								rxBuffer += port.ReadExisting();
							}
						}
					}
					
					if (requestStatusInterval.Ready)
					{
						lock (port)
						{
							port.Write("?");
						}
					}
					
					if (!String.IsNullOrEmpty(rxBuffer) && rxBuffer.Contains("\r\n"))
					{
						var responce = rxBuffer.Substring(0, rxBuffer.IndexOf('\n') - 1);
						
						if (responce == "ok" || responce.StartsWith("error"))
						{
							if (commandSent.Count > 0)
							{
								var command = commandSent[0];
								commandSent.RemoveAt(0);
								
								command.Responce = responce;
								command.Buffered = true;
								if (command.Command.Length > 0) commandBuffered.Add(command);
							}
							
							OnResponceRecived(responce);
						}
						else if (responce.StartsWith("<") && responce.EndsWith(">"))
						{
							GrblStatus.Parse(responce);
							
							// command complete
							if (commandBuffered.Count > GrblStatus.MotionBuffer)
							{
								for (int c = 0; c < (commandBuffered.Count - GrblStatus.MotionBuffer); c++)
								{
									var command = commandBuffered[0];
									commandBuffered.RemoveAt(0);
									command.Completed = true;
									commandHistory.Add(command);
								}
							}
							
							this.CurrentCommand = (commandBuffered.Count > 0) ? commandBuffered[0] : null;
							
							if (GrblStatus.Running)
							{
								GrblStatus.CommandsTotal = this.commandQueue.Count + this.commandSent.Count + this.commandBuffered.Count + this.commandHistory.Count;
								GrblStatus.CommandsCompleted = this.commandHistory.Count;
							}
							
							// send update
							Program.LogRaw(GrblStatus.ToJSON());
							
							// just finished job
							if (running && !GrblStatus.Running)
							{
								GrblStatus.CommandsCompleted = GrblStatus.CommandsTotal;
								this.commandQueue.Clear();
								this.commandSent.Clear();
								this.commandBuffered.Clear();
								this.commandHistory.Clear();
							}
							
							running = GrblStatus.Running;
						}
						else
						{
							OnResponceRecived(responce);
						}
						
						rxBuffer = rxBuffer.Substring(responce.Length + 2, (rxBuffer.Length - responce.Length - 2));
					}
					
					// send queued command to grbl if rxBuffer can fit new message
					const int MAX_RX_BUFFER = 127;
					if (this.commandQueue.Count > 0  &&
					    ((GrblStatus.RxBuffer + this.commandQueue[0].Command.Length + 1) < 127 || this.commandQueue[0].Command.Length == 1))
					{
						GrblStatus.RxBuffer += this.commandQueue[0].Command.Length + 1;
						
						var command = commandQueue[0];
						commandQueue.RemoveAt(0);
						Program.LogCommand(command.Command);
						port.Write(command.Command + "\n");
						command.Sent = true;
						commandSent.Add(command);
					}
					
					Thread.Sleep(10);
				}
				catch (Exception ex)
				{
					Program.Log(ex);
					Thread.Sleep(2000);
				}
			}
		}

		private void SendStatusUpdate()
		{
			var json = new Dictionary<string, object>();
		}
		
		#endregion
		
		#region events
		
		#region session expired

		public delegate void ResponceRecivedHandler(string responce);
		private ResponceRecivedHandler responceRecived = null;
		public event ResponceRecivedHandler ResponceRecived
		{
			add
			{
				this.responceRecived += value;
			}
			remove
			{
				this.responceRecived -= value;
			}
		}
		
		private void OnResponceRecived(string responce)
		{
			try
			{
				ResponceRecivedHandler handler;
				if (null != (handler = (ResponceRecivedHandler)this.responceRecived))
				{
					handler(responce);
				}
			}
			catch (Exception ex)
			{
				Program.Log(ex);
			}
		}
		
		#endregion

		#endregion
	}
}
