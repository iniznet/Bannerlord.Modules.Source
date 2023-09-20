using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x0200004C RID: 76
	internal class Vec2iBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x0600026A RID: 618 RVA: 0x0000A614 File Offset: 0x00008814
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Vec2i vec2i = (Vec2i)value;
			writer.WriteFloat((float)vec2i.Item1);
			writer.WriteFloat((float)vec2i.Item2);
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000A644 File Offset: 0x00008844
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			int num = reader.ReadInt();
			int num2 = reader.ReadInt();
			return new Vec2i(num, num2);
		}
	}
}
