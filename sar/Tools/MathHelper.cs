/* Copyright (C) 2017 Kevin Boronka
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

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
