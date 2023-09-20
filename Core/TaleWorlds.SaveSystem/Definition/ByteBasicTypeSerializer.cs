using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000045 RID: 69
	internal class ByteBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000255 RID: 597 RVA: 0x0000A510 File Offset: 0x00008710
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteByte((byte)value);
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000A51E File Offset: 0x0000871E
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadByte();
		}
	}
}
