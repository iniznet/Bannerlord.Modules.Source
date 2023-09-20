using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000053 RID: 83
	internal class ColorBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x0600027F RID: 639 RVA: 0x0000A8D4 File Offset: 0x00008AD4
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			Color color = (Color)value;
			writer.WriteColor(color);
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000A8EF File Offset: 0x00008AEF
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadColor();
		}
	}
}
