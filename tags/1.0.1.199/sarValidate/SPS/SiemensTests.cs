using System;
using System.Diagnostics;
using System.Collections.Generic;

using sar.Tools;
using sar.SPS.Siemens;

namespace sarValidate.SPS
{
	public static class Tests
	{
		public static bool CheckAddress(string parameter)
		{
			bool result = true;

			// ---------------------------------------------------------------------------------
			// Illegal Addresses - exception == pass
			// ---------------------------------------------------------------------------------
			ConsoleHelper.WriteLine("Checking illegal Siemens addresses", ConsoleColor.White);
			List<string> illegalAddresses = new List<string> { "MB17.7", "M7", "M17.8", "S88", "M10.0.2", "MX.A", "DBX11", "DB400.DBX99.9",	"DB40.DB15", "DB777.W70", "DB1.X70.1", "DB44.0.DBW15", "DB44O.DBW15", "DB44DBW15" };
			
			foreach (string address in illegalAddresses)
			{
				ConsoleHelper.Write("  ");
				
				try
				{
					Address a1 = new Address(address);
					ConsoleHelper.WritePassFail(false);
				}
				catch
				{
					result = false;
					ConsoleHelper.WritePassFail(true);
				}
				
				ConsoleHelper.WriteLine(" illegal address " + address);
			}
			
			
			// ---------------------------------------------------------------------------------
			// Legal Addresses - pass == pass
			// ---------------------------------------------------------------------------------

			ConsoleHelper.WriteLine();
			ConsoleHelper.WriteLine("Checking legal Siemens addresses", ConsoleColor.White);
			List<string> legalAddresses	= new List<string> 	{	"M0.0",		"M0.7",		"M1.0",		"M17.7",	"MB7", 		"MW85",		"MD0",		"IB20",		"I22.5",	"QW11",		"DB1.DBX1.5",	"DB40.DBW15",	"DB4.DBB1",		"DB77.DBD84" };
			List<Areas> legalArea		= new List<Areas>	{	Areas.M,	Areas.M,	Areas.M,	Areas.M,	Areas.M,	Areas.M,	Areas.M,	Areas.I,	Areas.I,	Areas.Q,	Areas.DB,		Areas.DB,		Areas.DB,		Areas.DB };
			List<int> legalStart		= new List<int> 	{	0*8 + 0,	0*8 + 7,	1*8 + 0,	17*8 + 7,	7*8, 		85 * 8,		0,			20*8,		22*8+5,		11*8,		1*8+5,			15*8+0,			1*8+0,			84*8+0 };
			List<int> legalLength		= new List<int> 	{	1,			1,			1,			1,			8, 			16,			32,			8, 			8, 			16,			1,				16,				8,				32 };
			List<int> legalDB			= new List<int> 	{	0,			0,			0,			0,			0, 			0,			0,			0, 			0, 			0,			1,				40,				4,				77 };


			int index = 0;
			
			foreach (string address in legalAddresses)
			{
				ConsoleHelper.Write("  ");

				try
				{
					Address a1 = new Address(address);
					bool pass = true;
					pass = pass && a1.area == legalArea[index];
					pass = pass && a1.length == legalLength[index];
					pass = pass && a1.startAddress == legalStart[index];
					pass = pass && a1.dataBlock == legalDB[index];
					
					ConsoleHelper.WritePassFail(pass);
				}
				catch
				{
					result = false;
					ConsoleHelper.WritePassFail(false);
				}
				
				ConsoleHelper.WriteLine(" legal address " + address);
				index++;
			}
			
			
			return result;
		}
	}
}
