/* Copyright (C) 2016 Kevin Boronka
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

using sar.Tools;

namespace sar
{
	public static class Logger
	{
		public static event LoggerEventHandler OnLog;
		public static bool LogToConsole { get; set; }
		
		public static void Log(Exception ex)
		{
			Base.Program.Log(ex);
			if (OnLog != null) OnLog(new LoggerEventArgs(ex));
			
			if (LogToConsole) ConsoleHelper.WriteException(ex);
		}
		
		public static void Log(string message)
		{
			Base.Program.Log(message);
			if (OnLog != null) OnLog(new LoggerEventArgs(message));
			
			if (LogToConsole) ConsoleHelper.WriteLine(message);
		}
		
		public static void FlushLogs()
		{
			Base.Program.FlushLogs();
		}
		
		public static void LogInfo()
		{
			Base.Program.LogInfo();
		}
	}
}
