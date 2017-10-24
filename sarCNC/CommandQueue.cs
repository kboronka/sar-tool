/*
 * Created by SharpDevelop.
 * User: kboronka
 * Date: 1/29/2017
 * Time: 6:34 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace sar.CNC
{
	public class CommandQueue
	{
		private object queueLock = new object();
		private List<string> commands;
		
		public int Length
		{
			get
			{
				lock (queueLock)
				{
					return commands.Count;
				}
			}
		}

		public CommandQueue()
		{
			commands = new List<string>();
		}
		
		public void Add(string command)
		{
			lock (queueLock)
			{
				commands.Add(command);
			}
		}
		
		public void Remove(string command)
		{
			lock (queueLock)
			{
				if (commands.Count > 0)
				{
					if (command == commands[0])
					{
						commands = commands.Skip(1)
							.ToList();
					}
				}
			}
		}
		
		public string GetNextCommand()
		{
			lock (queueLock)
			{
				if (commands.Count > 0)
				{
					return commands[0];
				}
			}
			
			return "";
		}
		
	}
}
