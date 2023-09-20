using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000041 RID: 65
	public interface IWriter
	{
		// Token: 0x06000202 RID: 514
		void WriteSerializableObject(ISerializableObject serializableObject);

		// Token: 0x06000203 RID: 515
		void WriteByte(byte value);

		// Token: 0x06000204 RID: 516
		void WriteSByte(sbyte value);

		// Token: 0x06000205 RID: 517
		void WriteBytes(byte[] bytes);

		// Token: 0x06000206 RID: 518
		void WriteInt(int value);

		// Token: 0x06000207 RID: 519
		void WriteUInt(uint value);

		// Token: 0x06000208 RID: 520
		void WriteShort(short value);

		// Token: 0x06000209 RID: 521
		void WriteUShort(ushort value);

		// Token: 0x0600020A RID: 522
		void WriteString(string value);

		// Token: 0x0600020B RID: 523
		void WriteColor(Color value);

		// Token: 0x0600020C RID: 524
		void WriteBool(bool value);

		// Token: 0x0600020D RID: 525
		void WriteFloat(float value);

		// Token: 0x0600020E RID: 526
		void WriteDouble(double value);

		// Token: 0x0600020F RID: 527
		void WriteULong(ulong value);

		// Token: 0x06000210 RID: 528
		void WriteLong(long value);

		// Token: 0x06000211 RID: 529
		void WriteVec2(Vec2 vec2);

		// Token: 0x06000212 RID: 530
		void WriteVec3(Vec3 vec3);

		// Token: 0x06000213 RID: 531
		void WriteVec3Int(Vec3i vec3);
	}
}
