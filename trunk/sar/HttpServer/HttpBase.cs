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

using sar.Tools;

namespace sar.HttpServer
{
	public class HttpBase
	{
		public HttpBase()
		{
		}
		
		#region logger
		
		FileLogger debugLog;
		ErrorLogger errorLogger;
		
		public FileLogger DebugLog
		{
			get { return this.debugLog; }
			private set { this.debugLog = value; }
		}
		
		public ErrorLogger ErrorLog
		{
			get { return this.errorLogger; }
			private set { this.errorLogger = value; }
		}
		
		protected void Log(string line)
		{
			if (this.debugLog == null) return;
			this.debugLog.WriteLine(this.ToString() + ": " + line);
		}
		
		protected void Log(Exception ex)
		{
			if (this.errorLogger == null) return;
			this.errorLogger.Write(ex);
		}
		
		#endregion		
	}
}
