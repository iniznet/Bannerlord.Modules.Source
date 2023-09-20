using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003C7 RID: 967
	public class PrisonerCaptureCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003A02 RID: 14850 RVA: 0x0010AC34 File Offset: 0x00108E34
		public override void RegisterEvents()
		{
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
		}

		// Token: 0x06003A03 RID: 14851 RVA: 0x0010AC86 File Offset: 0x00108E86
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003A04 RID: 14852 RVA: 0x0010AC88 File Offset: 0x00108E88
		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			foreach (Settlement settlement in clan.Settlements.Where((Settlement x) => x.IsFortification))
			{
				this.HandleSettlementHeroes(settlement);
			}
		}

		// Token: 0x06003A05 RID: 14853 RVA: 0x0010ACFC File Offset: 0x00108EFC
		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
		{
			foreach (Settlement settlement in faction1.Settlements.Where((Settlement x) => x.IsFortification))
			{
				this.HandleSettlementHeroes(settlement);
			}
			foreach (Settlement settlement2 in faction2.Settlements.Where((Settlement x) => x.IsFortification))
			{
				this.HandleSettlementHeroes(settlement2);
			}
		}

		// Token: 0x06003A06 RID: 14854 RVA: 0x0010ADD0 File Offset: 0x00108FD0
		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			if (settlement.IsFortification)
			{
				this.HandleSettlementHeroes(settlement);
			}
		}

		// Token: 0x06003A07 RID: 14855 RVA: 0x0010ADE4 File Offset: 0x00108FE4
		private void HandleSettlementHeroes(Settlement settlement)
		{
			foreach (Hero hero in settlement.HeroesWithoutParty.Where(new Func<Hero, bool>(this.SettlementHeroCaptureCommonCondition)).ToList<Hero>())
			{
				TakePrisonerAction.Apply(hero.CurrentSettlement.Party, hero);
			}
			foreach (MobileParty mobileParty in settlement.Parties.Where((MobileParty x) => x.IsLordParty && (x.Army == null || (x.Army != null && x.Army.LeaderParty == x && !x.Army.Parties.Contains(MobileParty.MainParty))) && x.MapEvent == null && this.SettlementHeroCaptureCommonCondition(x.LeaderHero)).ToList<MobileParty>())
			{
				LeaveSettlementAction.ApplyForParty(mobileParty);
				SetPartyAiAction.GetActionForPatrollingAroundSettlement(mobileParty, settlement);
			}
		}

		// Token: 0x06003A08 RID: 14856 RVA: 0x0010AEB4 File Offset: 0x001090B4
		private bool SettlementHeroCaptureCommonCondition(Hero hero)
		{
			return hero != null && hero != Hero.MainHero && !hero.IsWanderer && !hero.IsNotable && hero.HeroState != Hero.CharacterStates.Prisoner && hero.HeroState != Hero.CharacterStates.Dead && hero.MapFaction != null && hero.CurrentSettlement != null && hero.MapFaction.IsAtWarWith(hero.CurrentSettlement.MapFaction);
		}
	}
}
