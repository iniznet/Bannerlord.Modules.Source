using System;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	internal class ElementSaveData : VariableSaveData
	{
		public object ElementValue { get; private set; }

		public int ElementIndex { get; private set; }

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
				base.InitializeDataAsCustomStruct(MemberTypeId.Invalid, index, typeDefinition);
				return;
			}
			base.InitializeData(MemberTypeId.Invalid, value.GetType(), typeDefinition, value);
		}
	}
}
