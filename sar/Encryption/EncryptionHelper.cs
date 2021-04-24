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

using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace sar.Encryption
{
	public static class EncryptionHelper
	{
		public static void EncryptFile(string inputFile, string outputFile, string password, string salt)
		{
			// length of salt must be at least 8 bytes
			var saltyBytes = Encoding.ASCII.GetBytes(salt);
			if (saltyBytes.Length < 8)
			{
				var saltyList = saltyBytes.ToList();
				for (var i = 0; i < 8 - saltyBytes.Length; i++)
				{
					saltyList.Add(0x0);
				}
				saltyBytes = saltyList.ToArray();
			}

			var key = new Rfc2898DeriveBytes(password, saltyBytes);
			var outStream = new FileStream(outputFile, FileMode.Create);

			var encryption = new RijndaelManaged();

			encryption.Key = key.GetBytes(encryption.KeySize / 8);
			encryption.IV = key.GetBytes(encryption.BlockSize / 8);
			encryption.Padding = PaddingMode.PKCS7;

			var encryptionStream = new CryptoStream(outStream,
				encryption.CreateEncryptor(),
				CryptoStreamMode.Write);

			var inStream = new FileStream(inputFile, FileMode.Open);
			inStream.Position = 0;

			inStream.CopyTo(encryptionStream);
			encryptionStream.FlushFinalBlock();

			int data;
			while ((data = inStream.ReadByte()) != -1)
				encryptionStream.WriteByte((byte)data);

			inStream.Close();
			encryptionStream.Close();
			outStream.Close();
		}

		public static void DecryptFile(string inputFile, string outputFile, string password, string salt)
		{
			// length of salt must be at least 8 bytes
			var saltyBytes = Encoding.ASCII.GetBytes(salt);
			if (saltyBytes.Length < 8)
			{
				var saltyList = saltyBytes.ToList();
				for (var i = 0; i < 8 - saltyBytes.Length; i++)
				{
					saltyList.Add(0x0);
				}
				saltyBytes = saltyList.ToArray();
			}

			var key = new Rfc2898DeriveBytes(password, saltyBytes);
			var inStream = new FileStream(inputFile, FileMode.Open);

			var encryption = new RijndaelManaged();

			encryption.Key = key.GetBytes(encryption.KeySize / 8);
			encryption.IV = key.GetBytes(encryption.BlockSize / 8);
			encryption.Padding = PaddingMode.PKCS7;

			var encryptionStream = new CryptoStream(inStream,
				encryption.CreateDecryptor(),
				CryptoStreamMode.Read);

			var outStream = new FileStream(outputFile, FileMode.Create);

			int data;
			while ((data = encryptionStream.ReadByte()) != -1)
				outStream.WriteByte((byte)data);

			outStream.Close();
			encryptionStream.Close();
			inStream.Close();
		}
	}
}
