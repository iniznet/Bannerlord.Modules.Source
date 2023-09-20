using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003C1 RID: 961
	public class PlayerTrackCompanionBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600399B RID: 14747 RVA: 0x00109070 File Offset: 0x00107270
		public override void RegisterEvents()
		{
			CampaignEvents.CharacterBecameFugitive.AddNonSerializedListener(this, new Action<Hero>(this.HeroBecameFugitive));
			CampaignEvents.CompanionRemoved.AddNonSerializedListener(this, new Action<Hero, RemoveCompanionAction.RemoveCompanionDetail>(this.CompanionRemoved));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.SettlementEntered));
			CampaignEvents.NewCompanionAdded.AddNonSerializedListener(this, new Action<Hero>(this.CompanionAdded));
			CampaignEvents.HeroPrisonerReleased.AddNonSerializedListener(this, new Action<Hero, PartyBase, IFaction, EndCaptivityDetail>(this.PrisonerReleased));
		}

		// Token: 0x0600399C RID: 14748 RVA: 0x001090F0 File Offset: 0x001072F0
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Hero, CampaignTime>>("ScatteredCompanions", ref this.ScatteredCompanions);
		}

		// Token: 0x0600399D RID: 14749 RVA: 0x00109104 File Offset: 0x00107304
		private void AddHeroToScatteredCompanions(Hero hero)
		{
			if (hero.IsPlayerCompanion)
			{
				if (!this.ScatteredCompanions.ContainsKey(hero))
				{
					this.ScatteredCompanions.Add(hero, CampaignTime.Now);
					return;
				}
				this.ScatteredCompanions[hero] = CampaignTime.Now;
			}
		}

		// Token: 0x0600399E RID: 14750 RVA: 0x0010913F File Offset: 0x0010733F
		private void HeroBecameFugitive(Hero hero)
		{
			this.AddHeroToScatteredCompanions(hero);
		}

		// Token: 0x0600399F RID: 14751 RVA: 0x00109148 File Offset: 0x00107348
		private void PrisonerReleased(Hero releasedHero, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail)
		{
			this.AddHeroToScatteredCompanions(releasedHero);
		}

		// Token: 0x060039A0 RID: 14752 RVA: 0x00109154 File Offset: 0x00107354
		private void SettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (party == MobileParty.MainParty)
			{
				foreach (Hero hero2 in settlement.HeroesWithoutParty)
				{
					if (this.ScatteredCompanions.ContainsKey(hero2))
					{
						TextObject textObject = new TextObject("{=ahpSGaow}You hear that your companion {NOTABLE.LINK}, who was separated from you after a battle, is currently in this settlement.", null);
						StringHelpers.SetCharacterProperties("NOTABLE", hero2.CharacterObject, textObject, false);
						InformationManager.ShowInquiry(new InquiryData(new TextObject("{=dx0hmeH6}Tracking", null).ToString(), textObject.ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), "", null, null, "", 0f, null, null, null), false, false);
						this.ScatteredCompanions.Remove(hero2);
					}
				}
			}
		}

		// Token: 0x060039A1 RID: 14753 RVA: 0x00109234 File Offset: 0x00107434
		private void CompanionAdded(Hero companion)
		{
			if (this.ScatteredCompanions.ContainsKey(companion))
			{
				this.ScatteredCompanions.Remove(companion);
			}
		}

		// Token: 0x060039A2 RID: 14754 RVA: 0x00109251 File Offset: 0x00107451
		private void CompanionRemoved(Hero companion, RemoveCompanionAction.RemoveCompanionDetail detail)
		{
			if (this.ScatteredCompanions.ContainsKey(companion))
			{
				this.ScatteredCompanions.Remove(companion);
			}
		}

		// Token: 0x040011CB RID: 4555
		public Dictionary<Hero, CampaignTime> ScatteredCompanions = new Dictionary<Hero, CampaignTime>();
	}
}
