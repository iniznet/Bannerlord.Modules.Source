using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x0200004F RID: 79
	internal class Mat2BasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000273 RID: 627 RVA: 0x0000A6D4 File Offset: 0x000088D4
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Mat2 mat = (Mat2)value;
			writer.WriteVec2(mat.s);
			writer.WriteVec2(mat.f);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000A700 File Offset: 0x00008900
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			Vec2 vec = reader.ReadVec2();
			Vec2 vec2 = reader.ReadVec2();
			return new Mat2(vec.x, vec.y, vec2.x, vec2.y);
		}
	}
}
