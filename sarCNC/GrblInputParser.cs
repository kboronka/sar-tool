/*
 * Created by SharpDevelop.
 * User: Kevin
 * Date: 1/24/2017
 * Time: 10:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Threading;
using System.IO.Ports;

using sar;
using sar.Tools;
using sar.Timing;

namespace sar.CNC
{
	public class GrblInputParser
	{
		private string rxBuffer;
		private SerialPort port;
		private Stopwatch commandResponce = new Stopwatch();
		
		public bool Open
		{
			get
			{
				return port.IsOpen;
			}
			set
			{
				if (value)
				{
					port.DtrEnable = true;
					port.RtsEnable = true;
					port.Open();
					//port.DiscardInBuffer();
					var welcome = @"Grbl 1.1e ['$' for help]";
					var responce = "";
					var timeout = new Interval(60000);
					port.ReadTimeout = 40000;
					responce = port.ReadLine();
					
					while (!responce.Contains(welcome))
					{
						responce += port.ReadLine();
						if (timeout.Ready)
						{
							Logger.Log("  timeout connecting");
							port.Close();
						}
						
						Thread.Sleep(10);
					}
					
					Thread.Sleep(100);
					port.DiscardInBuffer();
				}
				else
				{
					port.Close();
				}
			}
		}
		
		public GrblInputParser(string portName)
		{
			Logger.Log("Port = " + portName);
			this.port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
			rxBuffer = "";
			commandResponce.Start();
		}
		
		public string SendCommand(string command)
		{
			if (!Open) return "";
			
			// flush
			port.DiscardInBuffer();
			
			// send command
			var startTime = commandResponce.ElapsedMilliseconds;
			var endTime = long.MaxValue;
			
			Logger.Log("  output -> " + command);
			port.Write(command + "\r");
			
			// wait or ack
			var responce = "";
			
			while (true)
			{
				if ((startTime - commandResponce.ElapsedMilliseconds) > 90000)
				{
					Logger.Log("  timeout -> " + command);
					return null;
				}
				else if (responce.StartsWith("error:"))
				{
					//var errorNumber = responce.Substring("error:".Length, responce.Length - "error:".Length - 2);
					Logger.Log("  " + responce);
					return responce.Substring(0, responce.Length-2);
				}
				else if (responce.EndsWith("ok\r\n"))
				{
					endTime = commandResponce.ElapsedMilliseconds;
					break;
				}

				if (port.BytesToRead > 0)
				{
					responce += port.ReadExisting();
				}
				else
				{
					Thread.Sleep(1);
				}
			}
			
			// trim ok\r\n
			responce = responce.Substring(0, responce.Length-4);
			
			if (!String.IsNullOrEmpty(responce))
			{
				if (responce.EndsWith("\r\n"))
				{
					responce = responce.Substring(0, responce.Length-2);
				}
				
				Logger.Log("  input -> " + responce);
			}
			
			var totalTime = endTime - startTime;
			if (totalTime > 500)
			{
				Logger.Log("+ time -> " + totalTime.ToString() + "ms");
			}
			else
			{
				Logger.Log("  time -> " + totalTime.ToString() + "ms");
			}

			return responce;
		}
		
		public GrblStatusResponce ReadStaus()
		{
			if (!Open) return null;
			
			var responce = SendCommand("?");
			
			if (responce[0] == '<' && responce[responce.Length - 1] == '>')
			{
				return new GrblStatusResponce(responce);
			}
			
			return null;
		}
		
		public GrblResponce Read()
		{
			if (!Open) return null;
			
			if(port.BytesToRead > 0)
			{
				rxBuffer += port.ReadExisting();
				
				if (!String.IsNullOrEmpty(rxBuffer) && rxBuffer.Contains("\r\n"))
				{
					var responce = rxBuffer.Substring(0, rxBuffer.IndexOf('\n') - 1);
					rxBuffer = rxBuffer.Substring(responce.Length + 2, (rxBuffer.Length - responce.Length - 2));
					
					if (responce[0] == '<' && responce[responce.Length - 1] == '>')
					{
						return new GrblStatusResponce(responce);
					}
				}
			}
			
			return null;
		}
	}
}
