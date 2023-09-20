using System;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	// Token: 0x02000027 RID: 39
	internal class FieldSaveData : MemberSaveData
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600015F RID: 351 RVA: 0x00006C3B File Offset: 0x00004E3B
		// (set) Token: 0x06000160 RID: 352 RVA: 0x00006C43 File Offset: 0x00004E43
		public FieldDefinition FieldDefinition { get; private set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000161 RID: 353 RVA: 0x00006C4C File Offset: 0x00004E4C
		// (set) Token: 0x06000162 RID: 354 RVA: 0x00006C54 File Offset: 0x00004E54
		public MemberTypeId SaveId { get; private set; }

		// Token: 0x06000163 RID: 355 RVA: 0x00006C5D File Offset: 0x00004E5D
		public FieldSaveData(ObjectSaveData objectSaveData, FieldDefinition fieldDefinition, MemberTypeId saveId)
			: base(objectSaveData)
		{
			this.FieldDefinition = fieldDefinition;
			this.SaveId = saveId;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00006C74 File Offset: 0x00004E74
		public override void Initialize(TypeDefinitionBase typeDefinition)
		{
			object value = this.FieldDefinition.GetValue(base.ObjectSaveData.Target);
			Type fieldType = this.FieldDefinition.FieldInfo.FieldType;
			base.InitializeData(this.SaveId, fieldType, typeDefinition, value);
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00006CB8 File Offset: 0x00004EB8
		public override void InitializeAsCustomStruct(int structId)
		{
			base.InitializeDataAsCustomStruct(this.SaveId, structId);
		}
	}
}
