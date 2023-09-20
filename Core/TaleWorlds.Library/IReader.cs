using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000040 RID: 64
	public interface IReader
	{
		// Token: 0x060001F0 RID: 496
		ISerializableObject ReadSerializableObject();

		// Token: 0x060001F1 RID: 497
		int ReadInt();

		// Token: 0x060001F2 RID: 498
		short ReadShort();

		// Token: 0x060001F3 RID: 499
		string ReadString();

		// Token: 0x060001F4 RID: 500
		Color ReadColor();

		// Token: 0x060001F5 RID: 501
		bool ReadBool();

		// Token: 0x060001F6 RID: 502
		float ReadFloat();

		// Token: 0x060001F7 RID: 503
		uint ReadUInt();

		// Token: 0x060001F8 RID: 504
		ulong ReadULong();

		// Token: 0x060001F9 RID: 505
		long ReadLong();

		// Token: 0x060001FA RID: 506
		byte ReadByte();

		// Token: 0x060001FB RID: 507
		byte[] ReadBytes(int length);

		// Token: 0x060001FC RID: 508
		Vec2 ReadVec2();

		// Token: 0x060001FD RID: 509
		Vec3 ReadVec3();

		// Token: 0x060001FE RID: 510
		Vec3i ReadVec3Int();

		// Token: 0x060001FF RID: 511
		sbyte ReadSByte();

		// Token: 0x06000200 RID: 512
		ushort ReadUShort();

		// Token: 0x06000201 RID: 513
		double ReadDouble();
	}
}
