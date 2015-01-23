/* Copyright (C) 2014 Kevin Boronka
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

namespace sar.Tools
{
	public class ErrorLogger : FileLogger
	{
		public ErrorLogger(string filename) : base(filename, false)
		{
		}
		
		public void Write(Exception ex)
		{
			Exception inner = ExceptionHandler.GetInnerException(ex);
			
			base.WriteLine(ConsoleHelper.HR);
			base.WriteLine("Time: " + DateTime.Now.ToString());
			base.WriteLine("Type: " + inner.GetType().ToString());
			base.WriteLine("Error: " + inner.Message);
			base.WriteLine(ConsoleHelper.HR);
			base.WriteLine(inner.GetStackTrace());
			base.WriteLine("");
		}
	}
}
