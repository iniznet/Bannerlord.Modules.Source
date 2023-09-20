using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001A6 RID: 422
	public abstract class MobilePartyMoraleModel : GameModel
	{
		// Token: 0x06001A92 RID: 6802
		public abstract float CalculateMoraleChange(MobileParty party);

		// Token: 0x06001A93 RID: 6803
		public abstract TextObject GetMoraleTooltipText(MobileParty party);
	}
}
