using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000043 RID: 67
	internal class ShortBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x0600024F RID: 591 RVA: 0x0000A4CA File Offset: 0x000086CA
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteShort((short)value);
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000A4D8 File Offset: 0x000086D8
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadShort();
		}
	}
}
