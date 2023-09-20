using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000047 RID: 71
	internal class FloatBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x0600025B RID: 603 RVA: 0x0000A556 File Offset: 0x00008756
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteFloat((float)value);
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000A564 File Offset: 0x00008764
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadFloat();
		}
	}
}
