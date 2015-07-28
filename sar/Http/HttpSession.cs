using System;
using System.Collections.Generic;

namespace sar.Http
{
	public class HttpSession
	{
		#region static
		
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
		
		#endregion
		
		
		private string dataLock;
		public string ID { get; private set; }
		public DateTime CreationDate { get; private set; }
		public DateTime LastRequest { get; private set; }
		public const int MAX_LIFE = 2;
		
		private Dictionary<string, object> data;
		public Dictionary<string, object> Data
		{
			get
			{
				lock (dataLock)
				{
					return data;
				}
			}
		}

		private DateTime expiryDate;
		public DateTime ExpiryDate
		{
			get
			{
				lock (dataLock)
				{
					this.LastRequest = DateTime.Now;
					expiryDate = this.LastRequest.AddDays(MAX_LIFE);
					return expiryDate;
				}
			}
		}
		
		public HttpSession()
		{
			this.ID = Guid.NewGuid().ToString("D");
			this.CreationDate = DateTime.Now;
			this.LastRequest = DateTime.Now;
			this.ExpiryDate = this.LastRequest.AddDays(MAX_LIFE);
			this.Data = new Dictionary<string, object>();
			
			sessions.Add(this.ID, this);
		}
	}
}
