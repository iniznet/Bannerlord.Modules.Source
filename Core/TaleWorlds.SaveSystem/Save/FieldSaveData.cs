using System;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	internal class FieldSaveData : MemberSaveData
	{
		public FieldDefinition FieldDefinition { get; private set; }

		public MemberTypeId SaveId { get; private set; }

		public FieldSaveData(ObjectSaveData objectSaveData, FieldDefinition fieldDefinition, MemberTypeId saveId)
			: base(objectSaveData)
		{
			this.FieldDefinition = fieldDefinition;
			this.SaveId = saveId;
		}

		public override void Initialize(TypeDefinitionBase typeDefinition)
		{
			object value = this.FieldDefinition.GetValue(base.ObjectSaveData.Target);
			Type fieldType = this.FieldDefinition.FieldInfo.FieldType;
			base.InitializeData(this.SaveId, fieldType, typeDefinition, value);
		}

		public override void InitializeAsCustomStruct(int structId)
		{
			base.InitializeDataAsCustomStruct(this.SaveId, structId);
		}
	}
}
