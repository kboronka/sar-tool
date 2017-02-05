using System;

namespace sar.CNC
{

	public class PositionVector
	{
		private double x;
		private double y;
		private double z;

		private double offsetX;
		private double offsetY;
		private double offsetZ;
		
		public double X
		{
			get { return (x + offsetX); }
			private set { x = value; }
		}
		
		public double Y
		{
			get { return (y + offsetY); }
			private set { y = value; }
		}
		
		public double Z 		
		{
			get { return (z + offsetZ); }
			private set { z = value; }
		}
		
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
		
		public void Offset(PositionVector newOffset)
		{
			this.offsetX = -newOffset.X;
			this.offsetY = -newOffset.Y;
			this.offsetZ = -newOffset.Z;
		}
		
		public void Move(PositionVector newPosition)
		{
			this.X = newPosition.X;
			this.Y = newPosition.Y;
			this.Z = newPosition.Z;
		}
	}
}
