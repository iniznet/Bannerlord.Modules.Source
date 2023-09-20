using System;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	// Token: 0x02000029 RID: 41
	internal abstract class MemberSaveData : VariableSaveData
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600016B RID: 363 RVA: 0x00006CC7 File Offset: 0x00004EC7
		// (set) Token: 0x0600016C RID: 364 RVA: 0x00006CCF File Offset: 0x00004ECF
		public ObjectSaveData ObjectSaveData { get; private set; }

		// Token: 0x0600016D RID: 365 RVA: 0x00006CD8 File Offset: 0x00004ED8
		protected MemberSaveData(ObjectSaveData objectSaveData)
			: base(objectSaveData.Context)
		{
			this.ObjectSaveData = objectSaveData;
		}

		// Token: 0x0600016E RID: 366
		public abstract void Initialize(TypeDefinitionBase typeDefinition);

		// Token: 0x0600016F RID: 367
		public abstract void InitializeAsCustomStruct(int structId);
	}
}
