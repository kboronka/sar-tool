/* Copyright (C) 2015 Kevin Boronka
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
using System.Threading;

using sar.Tools;
using Base=sar.Base;

namespace quickLog
{
	class Program : Base.Program
	{
		public static int Main(string[] args)
		{
			try
			{
				Base.Program.LogInfo();
				ConsoleHelper.Start();
				
				ConsoleHelper.WriteLine("Hello", ConsoleColor.Magenta);
				
				// TODO: Implement Functionality Here
				Thread.Sleep(1500);
				
				Base.Program.ErrorLog.FlushFile();
				Base.Program.DebugLog.FlushFile();
				
				ConsoleHelper.Shutdown();
				return ConsoleHelper.EXIT_OK;
			}
			catch (Exception ex)
			{
				ConsoleHelper.WriteException(ex);

				Base.Program.ErrorLog.FlushFile();
				Base.Program.DebugLog.FlushFile();
				
				ConsoleHelper.Shutdown();
				return ConsoleHelper.EXIT_ERROR;
			}
		}
	}
}