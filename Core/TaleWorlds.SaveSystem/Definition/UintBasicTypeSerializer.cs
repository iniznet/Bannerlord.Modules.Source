using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000042 RID: 66
	internal class UintBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x0600024C RID: 588 RVA: 0x0000A4A7 File Offset: 0x000086A7
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteUInt((uint)value);
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000A4B5 File Offset: 0x000086B5
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadUInt();
		}
	}
}
