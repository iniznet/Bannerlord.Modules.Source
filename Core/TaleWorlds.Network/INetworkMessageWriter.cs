using System;

namespace TaleWorlds.Network
{
	// Token: 0x0200001F RID: 31
	public interface INetworkMessageWriter
	{
		// Token: 0x060000C4 RID: 196
		void Write(string data);

		// Token: 0x060000C5 RID: 197
		void Write(int data);

		// Token: 0x060000C6 RID: 198
		void Write(short data);

		// Token: 0x060000C7 RID: 199
		void Write(bool data);

		// Token: 0x060000C8 RID: 200
		void Write(byte data);

		// Token: 0x060000C9 RID: 201
		void Write(float data);

		// Token: 0x060000CA RID: 202
		void Write(long data);

		// Token: 0x060000CB RID: 203
		void Write(ulong data);

		// Token: 0x060000CC RID: 204
		void Write(Guid data);

		// Token: 0x060000CD RID: 205
		void Write(byte[] data);
	}
}
