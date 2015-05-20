using System;

namespace sar.S7Siemens
{
	public class Adapter_Sim : Adapter
	{
		public Adapter_Sim() : base ()
		{
			
		}
		
		protected override byte[] ReadBytesRaw(Address address)
		{
			var bytes = address.byteLength;
			var result = new byte[address.byteLength];
			
			for (int i = 0; i<= result.Length; i++)
			{
				result[i] = (byte)(i % byte.MaxValue);
			}
			
			return result;
		}
	}
}
