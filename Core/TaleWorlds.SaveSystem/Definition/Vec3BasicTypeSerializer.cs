using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x0200004D RID: 77
	internal class Vec3BasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x0600026D RID: 621 RVA: 0x0000A674 File Offset: 0x00008874
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Vec3 vec = (Vec3)value;
			writer.WriteVec3(vec);
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000A68F File Offset: 0x0000888F
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadVec3();
		}
	}
}
