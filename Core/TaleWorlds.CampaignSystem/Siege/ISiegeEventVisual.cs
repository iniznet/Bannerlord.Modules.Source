using System;

namespace TaleWorlds.CampaignSystem.Siege
{
	// Token: 0x0200028B RID: 651
	public interface ISiegeEventVisual
	{
		// Token: 0x06002266 RID: 8806
		void Initialize();

		// Token: 0x06002267 RID: 8807
		void OnSiegeEventEnd();

		// Token: 0x06002268 RID: 8808
		void Tick();
	}
}
