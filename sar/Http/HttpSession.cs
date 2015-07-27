using System;
using System.Collections.Generic;

namespace sar.Http
{
	public class HttpSession
	{
		// TODO add expiration date
		private static Dictionary<string, HttpSession> sessions;
		private static string sessionLock = "";
		
		public static bool Contains(string id)
		{
			bool result;
			lock (sessionLock)
			{
				if (sessions == null) sessions = new Dictionary<string, HttpSession>();
				result = sessions.ContainsKey(id);
			}
			
			return result;
		}
		
		public static HttpSession Find(string id)
		{
			HttpSession responce;
			
			lock(sessionLock)
			{
				if (sessions == null) sessions = new Dictionary<string, HttpSession>();

				if (sessions.ContainsKey(id))
				{
					responce = sessions[id];
				}
				else
				{
					responce = new HttpSession();
				}
			}
			
			
			return responce;
		}
		
		public string ID { get; private set; }
		public DateTime Timestamp { get; private set; }
		
		public HttpSession()
		{
			this.ID = Guid.NewGuid().ToString("D");
			this.Timestamp = DateTime.Now;
			
			sessions.Add(this.ID, this);
		}
	}
}
