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
	public class PlayerTrackCompanionBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.CharacterBecameFugitive.AddNonSerializedListener(this, new Action<Hero>(this.HeroBecameFugitive));
			CampaignEvents.CompanionRemoved.AddNonSerializedListener(this, new Action<Hero, RemoveCompanionAction.RemoveCompanionDetail>(this.CompanionRemoved));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.SettlementEntered));
			CampaignEvents.NewCompanionAdded.AddNonSerializedListener(this, new Action<Hero>(this.CompanionAdded));
			CampaignEvents.HeroPrisonerReleased.AddNonSerializedListener(this, new Action<Hero, PartyBase, IFaction, EndCaptivityDetail>(this.PrisonerReleased));
			CampaignEvents.CanBeGovernorOrHavePartyRoleEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.CanBeGovernorOrHavePartyRole));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Hero, CampaignTime>>("ScatteredCompanions", ref this.ScatteredCompanions);
		}

		private void CanBeGovernorOrHavePartyRole(Hero hero, ref bool canBeGovernorOrHavePartyRole)
		{
			if (this.ScatteredCompanions.ContainsKey(hero))
			{
				canBeGovernorOrHavePartyRole = false;
			}
		}

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

		private void HeroBecameFugitive(Hero hero)
		{
			this.AddHeroToScatteredCompanions(hero);
		}

		private void PrisonerReleased(Hero releasedHero, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail)
		{
			this.AddHeroToScatteredCompanions(releasedHero);
		}

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

		private void CompanionAdded(Hero companion)
		{
			if (this.ScatteredCompanions.ContainsKey(companion))
			{
				this.ScatteredCompanions.Remove(companion);
			}
		}

		private void CompanionRemoved(Hero companion, RemoveCompanionAction.RemoveCompanionDetail detail)
		{
			if (this.ScatteredCompanions.ContainsKey(companion))
			{
				this.ScatteredCompanions.Remove(companion);
			}
		}

		public Dictionary<Hero, CampaignTime> ScatteredCompanions = new Dictionary<Hero, CampaignTime>();
	}
}
