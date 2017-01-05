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
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

using sar.Tools;

namespace sar.Http
{
	// used for testing ajax calls
	[SarController]
	public static class DebugController
	{
		public static HttpContent Info(HttpRequest request)
		{
			string data = "";
			if (request.Data != null)
			{
				data = StringHelper.GetString(request.Data);
			}
			
			var baseContent = new Dictionary<string, HttpContent>() {};

			// get connections
			lock (request.Server.Connections)
			{
				var totalConnections = request.Server.Connections.Count.ToString();
				var connections = "";
				
				foreach (var connection in request.Server.Connections)
				{
					if (!connection.Stopped)
					{
						var ip = ((IPEndPoint)connection.Socket.Client.RemoteEndPoint).Address.ToString();
						var port = ((IPEndPoint)connection.Socket.Client.RemoteEndPoint).Port.ToString();
						
						connections += ip + ":" + port + "\n";
					}
					else
					{
						connections += "timed out\n";
					}
				}
				
				baseContent.Add("TotalConnections", new HttpContent(totalConnections));
				baseContent.Add("Connections", new HttpContent(connections));
			}
			
			// get sessions
			// TODO: finish
			lock (request.Server.Sessions)
			{
				var totalSessions = request.Server.Sessions.Count.ToString();
				var sessions = "";
				
				foreach (var session in request.Server.Sessions)
				{
					sessions += session.Key + " - " + session.Value.CreationDate.ToString() + "\n";
				}
				
				
				baseContent.Add("TotalSessions", new HttpContent(totalSessions));
				baseContent.Add("Sessions", new HttpContent(sessions));
			}

			var currentSession = "";
			currentSession += "ID: " + request.Session.ID + "\n";
			currentSession += "Created: " + request.Session.CreationDate.ToString() + "\n";
			currentSession += "ExpiryDate: " + request.Session.ExpiryDate.ToString() + "\n";
			
			baseContent.Add("Session", new HttpContent(currentSession));
			
			return HttpContent.Read(request.Server, "sar.Http.Views.Debug.Info.html", baseContent);
		}
		
		public static HttpContent Header(HttpRequest request)
		{
			var baseContent = new Dictionary<string, HttpContent>() {};
			baseContent.Add("Header", new HttpContent(request.Header));
			return HttpContent.Read(request.Server, "sar.Http.Views.Debug.Header.html", baseContent);
		}
	}
}
