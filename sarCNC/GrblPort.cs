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

using sar.Tools;
using sar.Timing;

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
		private List<string> commandBuffer = new List<string>();
		
		public bool IsOpen
		{
			get
			{
				return port.IsOpen;
			}
		}
		
		
		public GrblPort(string portName)
		{
			port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
			this.readLoopThread = new Thread(this.ReadLoop);
			this.readLoopThread.IsBackground = true;
			this.readLoopThread.Start();
		}
		
		~GrblPort()
		{
			readLoopShutdown = true;
			this.readLoopThread.Join();
		}

		#region service
		
		private Thread readLoopThread;
		private bool readLoopShutdown = false;
		
		private void ReadLoop()
		{
			var readInterval = new Interval(250, 500);
			
			while (!readLoopShutdown)
			{
				try
				{
					if (readInterval.Ready)
					{
						port.ReadLine();
					}
					
					Thread.Sleep(1000);
				}
				catch (Exception ex)
				{
					Program.Log(ex);
					Thread.Sleep(2000);
				}
			}
		}

		#endregion
	
		#region events
		
		public delegate void DataRecivedHandler(GrblPort port);

		#region session expired
		
		private DataRecivedHandler dataRecived = null;
		public event DataRecivedHandler DataRecived
		{
			add
			{
				this.dataRecived += value;
			}
			remove
			{
				this.dataRecived -= value;
			}
		}
		
		private void OnSessionExpiring(GrblPort port)
		{
			try
			{
				DataRecivedHandler handler;
				if (null != (handler = (DataRecivedHandler)this.dataRecived))
				{
					handler(port);
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
