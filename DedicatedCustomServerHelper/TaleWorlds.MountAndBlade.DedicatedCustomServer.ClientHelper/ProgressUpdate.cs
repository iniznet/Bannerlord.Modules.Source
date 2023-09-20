using System;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	// Token: 0x02000009 RID: 9
	public class ProgressUpdate
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000062 RID: 98 RVA: 0x0000304D File Offset: 0x0000124D
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00003055 File Offset: 0x00001255
		public long BytesRead { get; private set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000064 RID: 100 RVA: 0x0000305E File Offset: 0x0000125E
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00003066 File Offset: 0x00001266
		public long TotalBytes { get; private set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000066 RID: 102 RVA: 0x0000306F File Offset: 0x0000126F
		public float MegaBytesRead
		{
			get
			{
				return (float)this.BytesRead / 1048576f;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000067 RID: 103 RVA: 0x0000307E File Offset: 0x0000127E
		public float TotalMegaBytes
		{
			get
			{
				return (float)this.TotalBytes / 1048576f;
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000308D File Offset: 0x0000128D
		public ProgressUpdate(long bytesRead, long totalBytes)
		{
			this.BytesRead = bytesRead;
			this.TotalBytes = totalBytes;
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000069 RID: 105 RVA: 0x000030A3 File Offset: 0x000012A3
		public float ProgressRatio
		{
			get
			{
				return (float)this.BytesRead / (float)this.TotalBytes;
			}
		}

		// Token: 0x0400002B RID: 43
		private const float bytesPerMegaByte = 1048576f;
	}
}
