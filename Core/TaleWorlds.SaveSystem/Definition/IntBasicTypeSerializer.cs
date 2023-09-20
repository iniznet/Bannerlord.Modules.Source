using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000041 RID: 65
	internal class IntBasicTypeSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000249 RID: 585 RVA: 0x0000A484 File Offset: 0x00008684
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
			writer.WriteInt((int)value);
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000A492 File Offset: 0x00008692
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return reader.ReadInt();
		}
	}
}
