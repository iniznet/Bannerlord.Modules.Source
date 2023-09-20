using System;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000061 RID: 97
	internal class InterfaceDefinition : TypeDefinitionBase
	{
		// Token: 0x060002DE RID: 734 RVA: 0x0000BE50 File Offset: 0x0000A050
		public InterfaceDefinition(Type type, SaveId saveId)
			: base(type, saveId)
		{
		}

		// Token: 0x060002DF RID: 735 RVA: 0x0000BE5A File Offset: 0x0000A05A
		public InterfaceDefinition(Type type, int saveId)
			: base(type, new TypeSaveId(saveId))
		{
		}
	}
}
