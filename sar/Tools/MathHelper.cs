using System;
using System.Drawing;

namespace sar.Tools
{
	public static class MathHelper
	{
		public static PointF GetIntersection(float Ax, float Ay,
		                                   float Bx, float By,
		                                   float Cx, float Cy,
		                                   float Dx, float Dy)
		{
			float dy1 = By - Ay;
			float dx1 = Bx - Ax;
			float dy2 = Dy - Cy;
			float dx2 = Dx - Cx;
			
			//check whether the two line parallel
			if (dy1 * dx2 == dy2 * dx1)
			{
				throw new ApplicationException("no point");
				//Return P with a specific data
			}
			else
			{
				float x = ((Cy - Ay) * dx1 * dx2 + dy1 * dx2 * Ax - dy2 * dx1 * Cx) / (dy1 * dx2 - dy2 * dx1);
				float y = Ay + (dy1 / dx1) * (x - Ax);
				return new PointF(x, y);
			}
		}
	}
}
