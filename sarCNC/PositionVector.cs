using System;

namespace sar.CNC
{

	public class PositionVector
	{
		public double X { get; private set; }
		public double Y { get; private set; }
		public double Z { get; private set; }
		
		public PositionVector(double x, double y, double z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		
		public PositionVector(string x, string y, string z)
		{
			this.X = double.Parse(x);
			this.Y = double.Parse(y);
			this.Z = double.Parse(z);
		}
		
		
		public void Move(PositionVector newPosition)
		{
			this.X = newPosition.X;
			this.Y = newPosition.Y;
			this.Z = newPosition.Z;
		}
	}
}
