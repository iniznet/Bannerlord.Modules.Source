using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000055 RID: 85
	internal class StringSerializer : IBasicTypeSerializer
	{
		// Token: 0x06000285 RID: 645 RVA: 0x0000A927 File Offset: 0x00008B27
		void IBasicTypeSerializer.Serialize(IWriter writer, object value)
		{
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000A929 File Offset: 0x00008B29
		object IBasicTypeSerializer.Deserialize(IReader reader)
		{
			return null;
		}
	}
}
