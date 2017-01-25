/*
 * Created by SharpDevelop.
 * User: Kevin
 * Date: 1/24/2017
 * Time: 10:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO.Ports;

namespace sar.CNC
{
	public class GrblInputParser
	{
		private string rxBuffer;
		public GrblInputParser()
		{
			rxBuffer = "";
		}
		
		public GrblResponce Read(SerialPort port)
		{
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
