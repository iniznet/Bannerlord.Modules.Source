using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000050 RID: 80
	internal class Mat3BasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000276 RID: 630 RVA: 0x0000A748 File Offset: 0x00008948
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Mat3 mat = (Mat3)value;
			writer.WriteVec3(mat.s);
			writer.WriteVec3(mat.f);
			writer.WriteVec3(mat.u);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000A780 File Offset: 0x00008980
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			Vec3 vec = reader.ReadVec3();
			Vec3 vec2 = reader.ReadVec3();
			Vec3 vec3 = reader.ReadVec3();
			return new Mat3(vec, vec2, vec3);
		}
	}
}
