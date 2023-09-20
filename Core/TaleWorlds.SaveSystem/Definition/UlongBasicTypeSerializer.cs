using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x0200004A RID: 74
	internal class UlongBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000264 RID: 612 RVA: 0x0000A5BF File Offset: 0x000087BF
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteULong((ulong)value);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000A5CD File Offset: 0x000087CD
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadULong();
		}
	}
}
