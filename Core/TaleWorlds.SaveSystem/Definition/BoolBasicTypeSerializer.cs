using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000054 RID: 84
	internal class BoolBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000282 RID: 642 RVA: 0x0000A904 File Offset: 0x00008B04
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteBool((bool)value);
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000A912 File Offset: 0x00008B12
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadBool();
		}
	}
}
