using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000020 RID: 32
	public interface INetworkMessageReader
	{
		// Token: 0x060000CE RID: 206
		int ReadInt32();

		// Token: 0x060000CF RID: 207
		short ReadInt16();

		// Token: 0x060000D0 RID: 208
		bool ReadBoolean();

		// Token: 0x060000D1 RID: 209
		byte ReadByte();

		// Token: 0x060000D2 RID: 210
		string ReadString();

		// Token: 0x060000D3 RID: 211
		float ReadFloat();

		// Token: 0x060000D4 RID: 212
		long ReadInt64();

		// Token: 0x060000D5 RID: 213
		ulong ReadUInt64();

		// Token: 0x060000D6 RID: 214
		Guid ReadGuid();

		// Token: 0x060000D7 RID: 215
		byte[] ReadByteArray();
	}
}
