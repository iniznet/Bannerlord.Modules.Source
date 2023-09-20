using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000040 RID: 64
	public interface IBasicTypeSerializer
	{
		// Token: 0x06000247 RID: 583
		void Serialize(IWriter writer, object value);

		// Token: 0x06000248 RID: 584
		object Deserialize(IReader reader);
	}
}
