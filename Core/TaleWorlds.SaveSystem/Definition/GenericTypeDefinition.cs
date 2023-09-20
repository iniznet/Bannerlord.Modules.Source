using System;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000060 RID: 96
	internal class GenericTypeDefinition : TypeDefinition
	{
		// Token: 0x060002DD RID: 733 RVA: 0x0000BE45 File Offset: 0x0000A045
		public GenericTypeDefinition(Type type, GenericSaveId saveId)
			: base(type, saveId, null)
		{
		}
	}
}
