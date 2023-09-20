using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Map
{
	// Token: 0x020000C8 RID: 200
	public interface IMapEntity
	{
		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06001262 RID: 4706
		Vec2 InteractionPosition { get; }

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06001263 RID: 4707
		TextObject Name { get; }

		// Token: 0x06001264 RID: 4708
		bool OnMapClick(bool followModifierUsed);

		// Token: 0x06001265 RID: 4709
		void OnHover();

		// Token: 0x06001266 RID: 4710
		void OnOpenEncyclopedia();

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06001267 RID: 4711
		bool IsMobileEntity { get; }

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x06001268 RID: 4712
		IMapEntity AttachedEntity { get; }

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x06001269 RID: 4713
		IPartyVisual PartyVisual { get; }

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x0600126A RID: 4714
		bool ShowCircleAroundEntity { get; }

		// Token: 0x0600126B RID: 4715
		bool IsMainEntity();

		// Token: 0x0600126C RID: 4716
		bool IsEnemyOf(IFaction faction);

		// Token: 0x0600126D RID: 4717
		bool IsAllyOf(IFaction faction);

		// Token: 0x0600126E RID: 4718
		void GetMountAndHarnessVisualIdsForPartyIcon(out string mountStringId, out string harnessStringId);

		// Token: 0x0600126F RID: 4719
		void OnPartyInteraction(MobileParty mobileParty);
	}
}
