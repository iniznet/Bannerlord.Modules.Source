using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000048 RID: 72
	internal class DoubleBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x0600025E RID: 606 RVA: 0x0000A579 File Offset: 0x00008779
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteDouble((double)value);
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000A587 File Offset: 0x00008787
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadDouble();
		}
	}
}
