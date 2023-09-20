using System;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	internal class PropertySaveData : MemberSaveData
	{
		public PropertyDefinition PropertyDefinition { get; private set; }

		public MemberTypeId SaveId { get; private set; }

		public PropertySaveData(ObjectSaveData objectSaveData, PropertyDefinition propertyDefinition, MemberTypeId saveId)
			: base(objectSaveData)
		{
			this.PropertyDefinition = propertyDefinition;
			this.SaveId = saveId;
		}

		public override void Initialize(TypeDefinitionBase typeDefinition)
		{
			object value = this.PropertyDefinition.GetValue(base.ObjectSaveData.Target);
			base.InitializeData(this.SaveId, this.PropertyDefinition.PropertyInfo.PropertyType, typeDefinition, value);
		}

		public override void InitializeAsCustomStruct(int structId)
		{
			base.InitializeDataAsCustomStruct(this.SaveId, structId);
		}
	}
}
