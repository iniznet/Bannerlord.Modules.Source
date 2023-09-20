using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000046 RID: 70
	internal class SbyteBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000258 RID: 600 RVA: 0x0000A533 File Offset: 0x00008733
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteSByte((sbyte)value);
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000A541 File Offset: 0x00008741
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadSByte();
		}
	}
}
