using System;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	// Token: 0x02000026 RID: 38
	internal class ElementSaveData : VariableSaveData
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600015A RID: 346 RVA: 0x00006B97 File Offset: 0x00004D97
		// (set) Token: 0x0600015B RID: 347 RVA: 0x00006B9F File Offset: 0x00004D9F
		public object ElementValue { get; private set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600015C RID: 348 RVA: 0x00006BA8 File Offset: 0x00004DA8
		// (set) Token: 0x0600015D RID: 349 RVA: 0x00006BB0 File Offset: 0x00004DB0
		public int ElementIndex { get; private set; }

		// Token: 0x0600015E RID: 350 RVA: 0x00006BBC File Offset: 0x00004DBC
		public ElementSaveData(ContainerSaveData containerSaveData, object value, int index)
			: base(containerSaveData.Context)
		{
			this.ElementValue = value;
			this.ElementIndex = index;
			if (value == null)
			{
				base.InitializeDataAsNullObject(MemberTypeId.Invalid);
				return;
			}
			TypeDefinitionBase typeDefinition = containerSaveData.Context.DefinitionContext.GetTypeDefinition(value.GetType());
			TypeDefinition typeDefinition2 = typeDefinition as TypeDefinition;
			if (typeDefinition2 != null && !typeDefinition2.IsClassDefinition)
			{
				base.InitializeDataAsCustomStruct(MemberTypeId.Invalid, index);
				return;
			}
			base.InitializeData(MemberTypeId.Invalid, value.GetType(), typeDefinition, value);
		}
	}
}
