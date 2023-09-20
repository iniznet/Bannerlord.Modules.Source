using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000049 RID: 73
	internal class LongBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000261 RID: 609 RVA: 0x0000A59C File Offset: 0x0000879C
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteLong((long)value);
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000A5AA File Offset: 0x000087AA
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadLong();
		}
	}
}
