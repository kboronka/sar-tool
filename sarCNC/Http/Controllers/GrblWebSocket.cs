/*
 * Created by SharpDevelop.
 * User: kboronka
 * Date: 7/7/2016
 * Time: 10:28 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO.Ports;

using sar.Http;
using sar.Tools;

// https://github.com/grbl/grbl/wiki/Configuring-Grbl-v0.9
	/*
	$$ (view Grbl settings)
	$# (view # parameters)
	$G (view parser state)
	$I (view build info)
	$N (view startup blocks)
	$x=value (save Grbl setting)
	$Nx=line (save startup block)
	$C (check gcode mode)
	$X (kill alarm lock)
	$H (run homing cycle)
	~ (cycle start)
	! (feed hold)
	? (current status)
	ctrl-x (reset Grbl)
	
	$0=10 (step pulse, usec)
	$1=25 (step idle delay, msec)
	$2=0 (step port invert mask:00000000)
	$3=6 (dir port invert mask:00000110)
	$4=0 (step enable invert, bool)
	$5=0 (limit pins invert, bool)
	$6=0 (probe pin invert, bool)
	$10=3 (status report mask:00000011)
	$11=0.020 (junction deviation, mm)
	$12=0.002 (arc tolerance, mm)
	$13=0 (report inches, bool)
	$20=0 (soft limits, bool)
	$21=0 (hard limits, bool)
	$22=0 (homing cycle, bool)
	$23=1 (homing dir invert mask:00000001)
	$24=50.000 (homing feed, mm/min)
	$25=635.000 (homing seek, mm/min)
	$26=250 (homing debounce, msec)
	$27=1.000 (homing pull-off, mm)
	$100=314.961 (x, step/mm)
	$101=314.961 (y, step/mm)
	$102=314.961 (z, step/mm)
	$110=635.000 (x max rate, mm/min)
	$111=635.000 (y max rate, mm/min)
	$112=635.000 (z max rate, mm/min)
	$120=50.000 (x accel, mm/sec^2)
	$121=50.000 (y accel, mm/sec^2)
	$122=50.000 (z accel, mm/sec^2)
	$130=225.000 (x max travel, mm)
	$131=125.000 (y max travel, mm)
	$132=170.000 (z max travel, mm)
	*/

namespace sar.CNC.Http
{
	[SarWebSocketController]
	public class GrblWebSocket : sar.Http.HttpWebSocket
	{
		#region static
		
		private static GrblWebSocket Singleton { get; set; }
		
		#endregion 

		public GrblWebSocket(HttpRequest request) : base(request)
		{
			Singleton = this;
		}
		
		override public void NewData(byte[] data)
		{
			
		}
	}
}
