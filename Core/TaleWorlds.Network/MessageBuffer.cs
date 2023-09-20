using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000015 RID: 21
	internal class MessageBuffer
	{
		// Token: 0x06000073 RID: 115 RVA: 0x0000301F File Offset: 0x0000121F
		internal MessageBuffer(byte[] buffer, int dataLength)
		{
			this.Buffer = buffer;
			this.DataLength = dataLength;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00003035 File Offset: 0x00001235
		internal MessageBuffer(byte[] buffer)
		{
			this.Buffer = buffer;
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00003044 File Offset: 0x00001244
		// (set) Token: 0x06000076 RID: 118 RVA: 0x0000304C File Offset: 0x0000124C
		internal byte[] Buffer { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00003055 File Offset: 0x00001255
		// (set) Token: 0x06000078 RID: 120 RVA: 0x0000305D File Offset: 0x0000125D
		internal int DataLength { get; set; }

		// Token: 0x06000079 RID: 121 RVA: 0x00003066 File Offset: 0x00001266
		internal string GetDebugText()
		{
			return BitConverter.ToString(this.Buffer, 0, this.DataLength);
		}

		// Token: 0x04000031 RID: 49
		internal const int MessageHeaderSize = 4;
	}
}
