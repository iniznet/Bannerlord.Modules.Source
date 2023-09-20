using System;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	public class ProgressUpdate
	{
		public long BytesRead { get; private set; }

		public long TotalBytes { get; private set; }

		public float MegaBytesRead
		{
			get
			{
				return (float)this.BytesRead / 1048576f;
			}
		}

		public float TotalMegaBytes
		{
			get
			{
				return (float)this.TotalBytes / 1048576f;
			}
		}

		public ProgressUpdate(long bytesRead, long totalBytes)
		{
			this.BytesRead = bytesRead;
			this.TotalBytes = totalBytes;
		}

		public float ProgressRatio
		{
			get
			{
				return (float)this.BytesRead / (float)this.TotalBytes;
			}
		}

		private const float bytesPerMegaByte = 1048576f;
	}
}
