using System;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	// Token: 0x0200002B RID: 43
	internal class PropertySaveData : MemberSaveData
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000186 RID: 390 RVA: 0x0000750B File Offset: 0x0000570B
		// (set) Token: 0x06000187 RID: 391 RVA: 0x00007513 File Offset: 0x00005713
		public PropertyDefinition PropertyDefinition { get; private set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000188 RID: 392 RVA: 0x0000751C File Offset: 0x0000571C
		// (set) Token: 0x06000189 RID: 393 RVA: 0x00007524 File Offset: 0x00005724
		public MemberTypeId SaveId { get; private set; }

		// Token: 0x0600018A RID: 394 RVA: 0x0000752D File Offset: 0x0000572D
		public PropertySaveData(ObjectSaveData objectSaveData, PropertyDefinition propertyDefinition, MemberTypeId saveId)
			: base(objectSaveData)
		{
			this.PropertyDefinition = propertyDefinition;
			this.SaveId = saveId;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00007544 File Offset: 0x00005744
		public override void Initialize(TypeDefinitionBase typeDefinition)
		{
			object value = this.PropertyDefinition.GetValue(base.ObjectSaveData.Target);
			base.InitializeData(this.SaveId, this.PropertyDefinition.PropertyInfo.PropertyType, typeDefinition, value);
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00007586 File Offset: 0x00005786
		public override void InitializeAsCustomStruct(int structId)
		{
			base.InitializeDataAsCustomStruct(this.SaveId, structId);
		}
	}
}
