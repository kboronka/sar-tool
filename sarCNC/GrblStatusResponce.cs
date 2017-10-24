using System;
using System.Linq;

/*
	https://github.com/gnea/grbl/blob/edge/doc/markdown/change_summary.md
	<Idle|MPos:0.000,0.000,0.000|Bf:15,126|FS:0,0|WCO:0.000,0.000,0.000>
	<Idle|MPos:0.000,0.000,0.000|Bf:15,126|FS:0,0|Ov:100,100,100>
	<Idle|MPos:0.000,0.000,0.000|Bf:15,126|FS:0,0>
	
	Bf=(blocks available in planner),(bytes available in rx buffer)
	FS = feed, speed (rpm)
	WCO = work coordinate offset
	Ov=feed,rapid,spindle (override %)
	Pn: XYZPDHRS (XYZ-axes limits, Probe, Door, Hold, soft-Reset, cycle Start pins)
		For example, a triggered Z-limit and probe pin would report Pn:ZP
 */

namespace sar.CNC
{

	public class GrblStatusResponce : GrblResponce
	{
		public PositionVector MachinePosition { get; private set; }
		public PositionVector WorkCoordinateOffset { get; private set; }

		public int PlannerBlocksAvailble;
		public int RxBufferBytesAvailble;
		public int FeedRate;
		public int RPM;
		
		public GrblStatusResponce(string responce)
		{
			responce = responce.TrimStart('<').TrimEnd('>');
			
			var fields = responce.Split('|');
			
			var status = fields[0];
			
			foreach (var field in fields.Skip(1)) {
				var components = field.Split(':');
				var header = components[0];
				var content = components[1];
				
				switch (header) {
					case "MPos":
						var mPos = content.Split(',');
						this.MachinePosition = new PositionVector(mPos[0], mPos[1], mPos[2]);
						break;
					case "Bf":
						var bf = content.Split(',');
						this.PlannerBlocksAvailble = int.Parse(bf[0]);
						this.RxBufferBytesAvailble = int.Parse(bf[1]);
						break;
					case "FS":
						var fs = content.Split(',');
						this.FeedRate = int.Parse(fs[0]);
						this.RPM = int.Parse(fs[1]);
						break;
					case "WCO":
						var wco = content.Split(',');
						this.WorkCoordinateOffset = new PositionVector(wco[0], wco[1], wco[2]);
						break;
					case "Pn":
						// TODO: support pins
						break;
					default:
						break;
				}
			}
		}
	}
}