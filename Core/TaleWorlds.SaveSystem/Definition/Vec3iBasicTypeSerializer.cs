using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x0200004E RID: 78
	internal class Vec3iBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000270 RID: 624 RVA: 0x0000A6A4 File Offset: 0x000088A4
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Vec3i vec3i = (Vec3i)value;
			writer.WriteVec3Int(vec3i);
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000A6BF File Offset: 0x000088BF
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadVec3Int();
		}
	}
}
