using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000052 RID: 82
	internal class QuaternionBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x0600027C RID: 636 RVA: 0x0000A850 File Offset: 0x00008A50
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Quaternion quaternion = (Quaternion)value;
			writer.WriteFloat(quaternion.X);
			writer.WriteFloat(quaternion.Y);
			writer.WriteFloat(quaternion.Z);
			writer.WriteFloat(quaternion.W);
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000A894 File Offset: 0x00008A94
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			float num = reader.ReadFloat();
			float num2 = reader.ReadFloat();
			float num3 = reader.ReadFloat();
			float num4 = reader.ReadFloat();
			return new Quaternion(num, num2, num3, num4);
		}
	}
}
