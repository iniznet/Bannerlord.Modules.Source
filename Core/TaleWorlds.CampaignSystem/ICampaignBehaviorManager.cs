using System;
using System.Collections.Generic;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200008A RID: 138
	public interface ICampaignBehaviorManager
	{
		// Token: 0x0600107C RID: 4220
		void RegisterEvents();

		// Token: 0x0600107D RID: 4221
		T GetBehavior<T>();

		// Token: 0x0600107E RID: 4222
		IEnumerable<T> GetBehaviors<T>();

		// Token: 0x0600107F RID: 4223
		void AddBehavior(CampaignBehaviorBase campaignBehavior);

		// Token: 0x06001080 RID: 4224
		void RemoveBehavior<T>() where T : CampaignBehaviorBase;

		// Token: 0x06001081 RID: 4225
		void ClearBehaviors();

		// Token: 0x06001082 RID: 4226
		void LoadBehaviorData();

		// Token: 0x06001083 RID: 4227
		void InitializeCampaignBehaviors(IEnumerable<CampaignBehaviorBase> inputComponents);
	}
}
