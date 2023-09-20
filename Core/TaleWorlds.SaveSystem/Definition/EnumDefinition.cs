using System;
using TaleWorlds.SaveSystem.Resolvers;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x0200005D RID: 93
	internal class EnumDefinition : TypeDefinitionBase
	{
		// Token: 0x060002C8 RID: 712 RVA: 0x0000BBF0 File Offset: 0x00009DF0
		public EnumDefinition(Type type, SaveId saveId, IEnumResolver resolver)
			: base(type, saveId)
		{
			this.Resolver = resolver;
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0000BC01 File Offset: 0x00009E01
		public EnumDefinition(Type type, int saveId, IEnumResolver resolver)
			: this(type, new TypeSaveId(saveId), resolver)
		{
		}

		// Token: 0x040000DF RID: 223
		public readonly IEnumResolver Resolver;
	}
}
