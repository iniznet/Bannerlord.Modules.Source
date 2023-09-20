using System;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x0200003F RID: 63
	internal class BasicTypeDefinition : TypeDefinitionBase
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000244 RID: 580 RVA: 0x0000A45D File Offset: 0x0000865D
		// (set) Token: 0x06000245 RID: 581 RVA: 0x0000A465 File Offset: 0x00008665
		public IBasicTypeSerializer Serializer { get; private set; }

		// Token: 0x06000246 RID: 582 RVA: 0x0000A46E File Offset: 0x0000866E
		public BasicTypeDefinition(Type type, int saveId, IBasicTypeSerializer serializer)
			: base(type, new TypeSaveId(saveId))
		{
			this.Serializer = serializer;
		}
	}
}
