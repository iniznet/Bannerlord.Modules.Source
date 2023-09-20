using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x0200004B RID: 75
	internal class Vec2BasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000267 RID: 615 RVA: 0x0000A5E4 File Offset: 0x000087E4
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Vec2 vec = (Vec2)value;
			writer.WriteVec2(vec);
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000A5FF File Offset: 0x000087FF
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadVec2();
		}
	}
}
