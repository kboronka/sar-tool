using System;
using System.IO;

namespace sar.Tools
{
	public delegate void ProgressChangeDelegate(double Persentage, ref bool Cancel);
	public delegate void Completedelegate();

	// credit to Anton Semenov: http://stackoverflow.com/questions/6044629/file-copy-with-progress-bar
	public class FileCopier
	{
		public string Source { get; set; }
		public string Destination { get; set; }

		public event ProgressChangeDelegate OnProgressChanged;
		public event Completedelegate OnComplete;
		
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
						double persentage = (double)totalBytes * 100.0 / fileLength;

						dest.Write(buffer, 0, currentBlockSize);

						cancelFlag = false;
						OnProgressChanged(persentage, ref cancelFlag);

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