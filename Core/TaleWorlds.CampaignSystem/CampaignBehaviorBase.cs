using System;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200002B RID: 43
	public abstract class CampaignBehaviorBase : ICampaignBehavior
	{
		// Token: 0x060002C1 RID: 705 RVA: 0x000121ED File Offset: 0x000103ED
		public CampaignBehaviorBase(string stringId)
		{
			this.StringId = stringId;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x000121FC File Offset: 0x000103FC
		public CampaignBehaviorBase()
		{
			this.StringId = base.GetType().Name;
		}

		// Token: 0x060002C3 RID: 707
		public abstract void RegisterEvents();

		// Token: 0x060002C4 RID: 708 RVA: 0x00012215 File Offset: 0x00010415
		public static T GetCampaignBehavior<T>()
		{
			return Campaign.Current.GetCampaignBehavior<T>();
		}

		// Token: 0x060002C5 RID: 709
		public abstract void SyncData(IDataStore dataStore);

		// Token: 0x040000DF RID: 223
		public readonly string StringId;

		// Token: 0x0200047E RID: 1150
		public abstract class SaveableCampaignBehaviorTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x06003FE1 RID: 16353 RVA: 0x00130B67 File Offset: 0x0012ED67
			public SaveableCampaignBehaviorTypeDefiner(int saveBaseId)
				: base(saveBaseId)
			{
			}
		}
	}
}
