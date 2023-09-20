using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000044 RID: 68
	internal class UshortBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000252 RID: 594 RVA: 0x0000A4ED File Offset: 0x000086ED
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteUShort((ushort)value);
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000A4FB File Offset: 0x000086FB
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadUShort();
		}
	}
}
