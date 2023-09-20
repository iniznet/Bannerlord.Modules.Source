using System;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000171 RID: 369
	public abstract class MapVisibilityModel : GameModel
	{
		// Token: 0x060018F4 RID: 6388
		public abstract ExplainedNumber GetPartySpottingRange(MobileParty party, bool includeDescriptions = false);

		// Token: 0x060018F5 RID: 6389
		public abstract float GetPartyRelativeInspectionRange(IMapPoint party);

		// Token: 0x060018F6 RID: 6390
		public abstract float GetPartySpottingDifficulty(MobileParty spotterParty, MobileParty party);

		// Token: 0x060018F7 RID: 6391
		public abstract float GetHideoutSpottingDistance();
	}
}
