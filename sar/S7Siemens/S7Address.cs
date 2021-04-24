/* Copyright (C) 2021 Kevin Boronka
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

using sar.Tools;
using System;
using System.IO;

namespace sar.S7Siemens
{
	public enum Areas : byte { P = 0x80, I = 0x81, Q = 0x82, M = 0x83, DB = 0x84, DI = 0x85, L = 0x86, VL = 0x87 };

	public struct Address
	{
		public Areas area;
		public ushort dataBlock;
		public uint startAddress;
		public ushort length;
		public ushort byteLength;
		public ushort byteAdddress;
		public ushort bitAddress;
		public TransportType transportType;

		public Address(string address)
		{
			address = address.ToUpper();

			if (address.Length >= 1 && address[0] == 'M')
				this.area = Areas.M;
			//else if (address.Length >= 1 && address[0] == 'P') this.AddressArea = AddressArea.P;
			else if (address.Length >= 1 && address[0] == 'I')
				this.area = Areas.I;
			else if (address.Length >= 1 && address[0] == 'Q')
				this.area = Areas.Q;
			//else if (address.Length >= 1 && address[0] == 'L') this.AddressArea = AddressArea.L;
			else if (address.Length >= 2 && address.Substring(0, 2) == "DB")
				this.area = Areas.DB;
			//else if (address.Length >= 2 && address.Substring(0, 2) == "DI") this.AddressArea = AddressArea.DI;
			//else if (address.Length >= 2 && address.Substring(0, 2) == "VL") this.AddressArea = AddressArea.VL;
			else
				throw new InvalidDataException("Invalid Address");

			if (this.area == Areas.DB)
			{
				address = address.Substring(2);
				this.dataBlock = ushort.Parse(address.Substring(0, address.IndexOf('.')));
				address = address.Substring(address.IndexOf('.') + 1);

				if (address.Substring(0, 2) != "DB")
					throw new InvalidDataException("Invalid DB Address");
				address = address.Substring(2);
			}
			else
			{
				address = address.Substring(1);
				this.dataBlock = 0;
			}

			if (address.Length > 1 && address[0] == 'D')
				this.length = 4 * 8;
			else if (address.Length > 1 && address[0] == 'W')
				this.length = 2 * 8;
			else if (address.Length > 1 && address[0] == 'B')
				this.length = 1 * 8;
			else if (address.Length > 1 && this.area == Areas.DB && address[0] == 'X')
				this.length = 1;
			else if (address.Length > 1 && this.area != Areas.DB && address[0].IsNumeric())
				this.length = 1;
			else
				throw new InvalidDataException("Invalid Address Type");

			if (this.length == 1)
			{
				this.transportType = TransportType.Bit;
			}
			else
			{
				this.transportType = TransportType.Byte;
			}

			if (this.length > 1 || this.area == Areas.DB && address[0] == 'X')
				address = address.Substring(1);

			// verify valid address
			if (!address.IsNumeric())
				throw new InvalidDataException("Invalid address location");

			// bit address verification
			if (this.length == 1 && address.IndexOf('.') == -1)
				throw new InvalidDataException("Invalid bit address location");
			if (this.length == 1 && int.Parse(address.Substring(address.IndexOf('.') + 1)) > 7)
				throw new InvalidDataException("Invalid bit address location");

			// non bit address verification
			if (this.length != 1 && address.IndexOf('.') != -1)
				throw new InvalidDataException("Invalid non-bit address location");

			var startAddress = double.Parse(address);
			this.byteAdddress = (ushort)Math.Floor(startAddress);
			this.bitAddress = (ushort)((Math.Round(startAddress - (double)this.byteAdddress, 2)) * 10);

			this.startAddress = (uint)(this.byteAdddress * 8) + this.bitAddress;

			this.byteLength = (ushort)(this.length / 8);
		}

		public static bool operator ==(Address address1, Address address2)
		{
			return address1.Equals(address2);
		}

		public static bool operator !=(Address address1, Address address2)
		{
			return !(address1 == address2);
		}

		public static bool operator ==(Address address1, string address2)
		{
			if (string.IsNullOrEmpty(address2))
				return false;
			return (address1 == new Address(address2));
		}

		public static bool operator !=(Address address1, string address2)
		{
			return !(address1 == address2);
		}

		public override bool Equals(object obj)
		{
			return this.Equals((Address)obj);
		}

		public bool Equals(Address other)
		{
			return
				this.area == other.area &&
				this.dataBlock == other.dataBlock &&
				this.startAddress == other.startAddress &&
				this.transportType == other.transportType &&
				this.length == other.length;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			string address = "";
			address += this.area.ToString();
			if (this.area == Areas.DB)
			{
				address += this.dataBlock.ToString();
				address += ".DB";

				if (this.length == 1 || this.length > 4 * 8)
					address += "X";
				if (this.length == 1 * 8)
					address += "B";
				if (this.length == 2 * 8)
					address += "W";
				if (this.length == 4 * 8)
					address += "D";

				address += this.byteAdddress.ToString();
				if (this.transportType == TransportType.Bit)
					address += "." + this.bitAddress.ToString();
			}
			else
			{
				if (this.length == 1 * 8)
					address += "B";
				if (this.length == 2 * 8)
					address += "W";
				if (this.length == 4 * 8)
					address += "D";
				address += this.byteAdddress.ToString();

				if (this.transportType == TransportType.Bit)
					address += "." + this.bitAddress.ToString();
			}

			if (this.length > 4 * 8)
				address += " LEN=" + this.length.ToString();

			return address;
		}
	};
}
