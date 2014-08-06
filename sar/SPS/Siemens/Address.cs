
using System;
using System.IO;

using sar.Tools;

namespace sar.SPS.Siemens
{
	public enum Areas : byte { P = 0x80, I = 0x81, Q = 0x82, M = 0x83, DB = 0x84, DI = 0x85, L = 0x86, VL = 0x87 };
	
	public struct Address
	{
		public Areas area;
		public ushort dataBlock;
		public uint startAddress;
		public ushort length;
		
		public Address(string address)
		{
			address = address.ToUpper();
			
			if (address.Length >= 1 && address[0] == 'M') this.area = Areas.M;
			//else if (address.Length >= 1 && address[0] == 'P') this.AddressArea = AddressArea.P;
			else if (address.Length >= 1 && address[0] == 'I') this.area = Areas.I;
			else if (address.Length >= 1 && address[0] == 'Q') this.area = Areas.Q;
			//else if (address.Length >= 1 && address[0] == 'L') this.AddressArea = AddressArea.L;
			else if (address.Length >= 2 && address.Substring(0, 2) == "DB") this.area = Areas.DB;
			//else if (address.Length >= 2 && address.Substring(0, 2) == "DI") this.AddressArea = AddressArea.DI;
			//else if (address.Length >= 2 && address.Substring(0, 2) == "VL") this.AddressArea = AddressArea.VL;
			else throw new InvalidDataException("Invalid Address");
			
			
			if (this.area == Areas.DB)
			{
				address = address.Substring(2);
				this.dataBlock = ushort.Parse(address.Substring(0, address.IndexOf('.')));
				address = address.Substring(address.IndexOf('.') + 1);
				
				if (address.Substring(0, 2) != "DB") throw new InvalidDataException("Invalid DB Address");
				address = address.Substring(2);
				
			}
			else
			{
				address = address.Substring(1);
				this.dataBlock = 0;
			}

			
			if (address.Length > 1 && address[0] == 'D') this.length = 4 * 8;
			else if (address.Length > 1 && address[0] == 'W') this.length = 2 * 8;
			else if (address.Length > 1 && address[0] == 'B') this.length = 1 * 8;
			else if (address.Length > 1 && this.area == Areas.DB && address[0] == 'X') this.length = 1;
			else if (address.Length > 1 && this.area != Areas.DB && StringHelper.IsNumeric(address[0])) this.length = 1;
			else throw new InvalidDataException("Invalid Address Type");

			
			if (this.length > 1 || this.area == Areas.DB && address[0] == 'X') address = address.Substring(1);
			
			// verify valid address
			if (!StringHelper.IsNumeric(address)) throw new InvalidDataException("Invalid address location");
			
			// bit address verification
			if (this.length == 1 && address.IndexOf('.') == -1) throw new InvalidDataException("Invalid bit address location");
			if (this.length == 1 && int.Parse(address.Substring(address.IndexOf('.') + 1)) > 7) throw new InvalidDataException("Invalid bit address location");

			// non bit address verification
			if (this.length != 1 && address.IndexOf('.') != -1) throw new InvalidDataException("Invalid non-bit address location");
			

			double startAddress = double.Parse(address);
			this.startAddress = (uint)(Math.Floor(startAddress) * 8) + (uint)((startAddress - Math.Floor(startAddress)) * 10);
		}
	};
}
