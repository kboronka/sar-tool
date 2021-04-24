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

namespace sar.Tools
{
	public delegate void ProgressChangeDelegate(double percentage, ref bool cancel);
	public delegate void CompleteDelegate();

	// credit to Anton Semenov: http://stackoverflow.com/questions/6044629/file-copy-with-progress-bar
	public class FileCopier
	{
		public string Source { get; set; }
		public string Destination { get; set; }

		public event ProgressChangeDelegate OnProgressChanged;
		public event CompleteDelegate OnComplete;

		public FileCopier(string source, string destination)
		{
			this.Source = source;
			this.Destination = destination;

			OnProgressChanged += delegate
			{
			};
			OnComplete += delegate
			{
			};
		}

		public void Copy()
		{
			var buffer = new byte[1024 * 1024]; // 1MB buffer
			bool cancelFlag = false;

			using (var source = new FileStream(Source, FileMode.Open, FileAccess.Read))
			{
				long fileLength = source.Length;
				using (var dest = new FileStream(Destination, FileMode.CreateNew, FileAccess.Write))
				{
					long totalBytes = 0;
					int currentBlockSize = 0;

					while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
					{
						totalBytes += currentBlockSize;
						double percentage = (double)totalBytes * 100.0 / fileLength;

						dest.Write(buffer, 0, currentBlockSize);

						cancelFlag = false;
						OnProgressChanged(percentage, ref cancelFlag);

						if (cancelFlag == true)
						{
							// Delete dest file here
							break;
						}
					}
				}
			}

			OnComplete();
		}
	}
}