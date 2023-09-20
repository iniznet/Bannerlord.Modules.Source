using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x020000ED RID: 237
	public class DefaultAlleyModel : AlleyModel
	{
		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x06001442 RID: 5186 RVA: 0x00059BEC File Offset: 0x00057DEC
		private CharacterObject _thug
		{
			get
			{
				return MBObjectManager.Instance.GetObject<CharacterObject>("gangster_1");
			}
		}

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x06001443 RID: 5187 RVA: 0x00059BFD File Offset: 0x00057DFD
		private CharacterObject _expertThug
		{
			get
			{
				return MBObjectManager.Instance.GetObject<CharacterObject>("gangster_2");
			}
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x06001444 RID: 5188 RVA: 0x00059C0E File Offset: 0x00057E0E
		private CharacterObject _masterThug
		{
			get
			{
				return MBObjectManager.Instance.GetObject<CharacterObject>("gangster_3");
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x06001445 RID: 5189 RVA: 0x00059C1F File Offset: 0x00057E1F
		public override CampaignTime DestroyAlleyAfterDaysWhenLeaderIsDeath
		{
			get
			{
				return CampaignTime.Days(4f);
			}
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x06001446 RID: 5190 RVA: 0x00059C2B File Offset: 0x00057E2B
		public override int MinimumTroopCountInPlayerOwnedAlley
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x06001447 RID: 5191 RVA: 0x00059C2E File Offset: 0x00057E2E
		public override int MaximumTroopCountInPlayerOwnedAlley
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x06001448 RID: 5192 RVA: 0x00059C32 File Offset: 0x00057E32
		public override float GetDailyCrimeRatingOfAlley
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x00059C39 File Offset: 0x00057E39
		public override float GetDailyXpGainForAssignedClanMember(Hero assignedHero)
		{
			return 200f;
		}

		// Token: 0x0600144A RID: 5194 RVA: 0x00059C40 File Offset: 0x00057E40
		public override float GetDailyXpGainForMainHero()
		{
			return 40f;
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x00059C47 File Offset: 0x00057E47
		public override float GetInitialXpGainForMainHero()
		{
			return 1500f;
		}

		// Token: 0x0600144C RID: 5196 RVA: 0x00059C4E File Offset: 0x00057E4E
		public override float GetXpGainAfterSuccessfulAlleyDefenseForMainHero()
		{
			return 6000f;
		}

		// Token: 0x0600144D RID: 5197 RVA: 0x00059C55 File Offset: 0x00057E55
		public override TroopRoster GetTroopsOfAIOwnedAlley(Alley alley)
		{
			return this.GetTroopsOfAlleyInternal(alley);
		}

		// Token: 0x0600144E RID: 5198 RVA: 0x00059C60 File Offset: 0x00057E60
		public override TroopRoster GetTroopsOfAlleyForBattleMission(Alley alley)
		{
			TroopRoster troopsOfAlleyInternal = this.GetTroopsOfAlleyInternal(alley);
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			foreach (TroopRosterElement troopRosterElement in troopsOfAlleyInternal.GetTroopRoster())
			{
				troopRoster.AddToCounts(troopRosterElement.Character, troopRosterElement.Number * 2, false, 0, 0, true, -1);
			}
			return troopRoster;
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x00059CD4 File Offset: 0x00057ED4
		private TroopRoster GetTroopsOfAlleyInternal(Alley alley)
		{
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			Hero owner = alley.Owner;
			if (owner.Power <= 100f)
			{
				if ((float)owner.RandomValue > 0.5f)
				{
					troopRoster.AddToCounts(this._thug, 3, false, 0, 0, true, -1);
				}
				else
				{
					troopRoster.AddToCounts(this._thug, 2, false, 0, 0, true, -1);
					troopRoster.AddToCounts(this._masterThug, 1, false, 0, 0, true, -1);
				}
			}
			else if (owner.Power <= 200f)
			{
				if ((float)owner.RandomValue > 0.5f)
				{
					troopRoster.AddToCounts(this._thug, 2, false, 0, 0, true, -1);
					troopRoster.AddToCounts(this._expertThug, 1, false, 0, 0, true, -1);
					troopRoster.AddToCounts(this._masterThug, 2, false, 0, 0, true, -1);
				}
				else
				{
					troopRoster.AddToCounts(this._thug, 1, false, 0, 0, true, -1);
					troopRoster.AddToCounts(this._expertThug, 2, false, 0, 0, true, -1);
					troopRoster.AddToCounts(this._masterThug, 2, false, 0, 0, true, -1);
				}
			}
			else if (owner.Power <= 300f)
			{
				if ((float)owner.RandomValue > 0.5f)
				{
					troopRoster.AddToCounts(this._thug, 3, false, 0, 0, true, -1);
					troopRoster.AddToCounts(this._expertThug, 2, false, 0, 0, true, -1);
					troopRoster.AddToCounts(this._masterThug, 2, false, 0, 0, true, -1);
				}
				else
				{
					troopRoster.AddToCounts(this._thug, 1, false, 0, 0, true, -1);
					troopRoster.AddToCounts(this._expertThug, 3, false, 0, 0, true, -1);
					troopRoster.AddToCounts(this._masterThug, 3, false, 0, 0, true, -1);
				}
			}
			else if ((float)owner.RandomValue > 0.5f)
			{
				troopRoster.AddToCounts(this._thug, 3, false, 0, 0, true, -1);
				troopRoster.AddToCounts(this._expertThug, 3, false, 0, 0, true, -1);
				troopRoster.AddToCounts(this._masterThug, 3, false, 0, 0, true, -1);
			}
			else
			{
				troopRoster.AddToCounts(this._thug, 1, false, 0, 0, true, -1);
				troopRoster.AddToCounts(this._expertThug, 4, false, 0, 0, true, -1);
				troopRoster.AddToCounts(this._masterThug, 4, false, 0, 0, true, -1);
			}
			return troopRoster;
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x00059F04 File Offset: 0x00058104
		public override List<ValueTuple<Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail>> GetClanMembersAndAvailabilityDetailsForLeadingAnAlley(Alley alley)
		{
			List<ValueTuple<Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail>> list = new List<ValueTuple<Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail>>();
			foreach (Hero hero in Clan.PlayerClan.Lords)
			{
				if (hero != Hero.MainHero && !hero.IsDead)
				{
					list.Add(new ValueTuple<Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail>(hero, this.GetAvailability(alley, hero)));
				}
			}
			foreach (Hero hero2 in Clan.PlayerClan.Companions)
			{
				if (hero2 != Hero.MainHero && !hero2.IsDead)
				{
					list.Add(new ValueTuple<Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail>(hero2, this.GetAvailability(alley, hero2)));
				}
			}
			return list;
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x00059FE4 File Offset: 0x000581E4
		public override TroopRoster GetTroopsToRecruitFromAlleyDependingOnAlleyRandom(Alley alley, float random)
		{
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			if (random >= 0.5f)
			{
				return troopRoster;
			}
			Clan relatedBanditClanDependingOnAlleySettlementFaction = this.GetRelatedBanditClanDependingOnAlleySettlementFaction(alley);
			if (random > 0.3f)
			{
				troopRoster.AddToCounts(this._thug, 1, false, 0, 0, true, -1);
				troopRoster.AddToCounts(relatedBanditClanDependingOnAlleySettlementFaction.BasicTroop, 1, false, 0, 0, true, -1);
			}
			else if (random > 0.15f)
			{
				troopRoster.AddToCounts(this._thug, 2, false, 0, 0, true, -1);
				troopRoster.AddToCounts(relatedBanditClanDependingOnAlleySettlementFaction.BasicTroop, 1, false, 0, 0, true, -1);
				troopRoster.AddToCounts(relatedBanditClanDependingOnAlleySettlementFaction.BasicTroop.UpgradeTargets[0], 1, false, 0, 0, true, -1);
			}
			else if (random > 0.05f)
			{
				troopRoster.AddToCounts(this._thug, 3, false, 0, 0, true, -1);
				troopRoster.AddToCounts(relatedBanditClanDependingOnAlleySettlementFaction.BasicTroop, 2, false, 0, 0, true, -1);
				troopRoster.AddToCounts(relatedBanditClanDependingOnAlleySettlementFaction.BasicTroop.UpgradeTargets[0], 1, false, 0, 0, true, -1);
			}
			else
			{
				troopRoster.AddToCounts(this._thug, 2, false, 0, 0, true, -1);
				troopRoster.AddToCounts(relatedBanditClanDependingOnAlleySettlementFaction.BasicTroop, 3, false, 0, 0, true, -1);
				troopRoster.AddToCounts(relatedBanditClanDependingOnAlleySettlementFaction.BasicTroop.UpgradeTargets[0], 3, false, 0, 0, true, -1);
			}
			return troopRoster;
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x0005A114 File Offset: 0x00058314
		public override TextObject GetDisabledReasonTextForHero(Hero hero, Alley alley, DefaultAlleyModel.AlleyMemberAvailabilityDetail detail)
		{
			switch (detail)
			{
			case DefaultAlleyModel.AlleyMemberAvailabilityDetail.Available:
				return TextObject.Empty;
			case DefaultAlleyModel.AlleyMemberAvailabilityDetail.AvailableWithDelay:
			{
				TextObject textObject = new TextObject("{=dgUF5awO}It will take {HOURS} {?HOURS > 1}hours{?}hour{\\?} for this clan member to arrive.", null);
				textObject.SetTextVariable("HOURS", (int)Math.Ceiling((double)Campaign.Current.Models.DelayedTeleportationModel.GetTeleportationDelayAsHours(hero, alley.Settlement.Party).ResultNumber));
				return textObject;
			}
			case DefaultAlleyModel.AlleyMemberAvailabilityDetail.NotEnoughRoguerySkill:
			{
				TextObject textObject2 = GameTexts.FindText("str_character_role_disabled_tooltip", null);
				textObject2.SetTextVariable("SKILL_NAME", DefaultSkills.Roguery.Name.ToString());
				textObject2.SetTextVariable("MIN_SKILL_AMOUNT", 30);
				return textObject2;
			}
			case DefaultAlleyModel.AlleyMemberAvailabilityDetail.NotEnoughMercyTrait:
			{
				TextObject textObject3 = GameTexts.FindText("str_hero_needs_trait_tooltip", null);
				textObject3.SetTextVariable("TRAIT_NAME", DefaultTraits.Mercy.Name.ToString());
				textObject3.SetTextVariable("MAX_TRAIT_AMOUNT", 0);
				return textObject3;
			}
			case DefaultAlleyModel.AlleyMemberAvailabilityDetail.CanNotLeadParty:
				return new TextObject("{=qClVr2ka}This hero cannot lead a party", null);
			case DefaultAlleyModel.AlleyMemberAvailabilityDetail.AlreadyAlleyLeader:
				return GameTexts.FindText("str_hero_is_already_alley_leader", null);
			case DefaultAlleyModel.AlleyMemberAvailabilityDetail.IsPrisoner:
				return new TextObject("{=qhRC8XWU}This hero is currently prisoner.", null);
			default:
				return TextObject.Empty;
			}
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x0005A228 File Offset: 0x00058428
		public override float GetAlleyAttackResponseTimeInDays(TroopRoster troopRoster)
		{
			float num = 0f;
			foreach (TroopRosterElement troopRosterElement in troopRoster.GetTroopRoster())
			{
				num += (((float)troopRosterElement.Character.Tier > 4f) ? 4f : ((float)troopRosterElement.Character.Tier)) * (float)troopRosterElement.Number;
			}
			return (float)Math.Min(10, 5 + (int)(num / 8f));
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x0005A2C0 File Offset: 0x000584C0
		private Clan GetRelatedBanditClanDependingOnAlleySettlementFaction(Alley alley)
		{
			string stringId = alley.Settlement.Culture.StringId;
			Clan clan = null;
			if (stringId == "khuzait")
			{
				clan = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "steppe_bandits");
			}
			else if (stringId == "vlandia" || stringId.Contains("empire"))
			{
				clan = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "mountain_bandits");
			}
			else if (stringId == "aserai")
			{
				clan = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "desert_bandits");
			}
			else if (stringId == "battania")
			{
				clan = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "forest_bandits");
			}
			else if (stringId == "sturgia")
			{
				clan = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "sea_raiders");
			}
			return clan;
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x0005A410 File Offset: 0x00058610
		private DefaultAlleyModel.AlleyMemberAvailabilityDetail GetAvailability(Alley alley, Hero hero)
		{
			IAlleyCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<IAlleyCampaignBehavior>();
			if (hero.GetSkillValue(DefaultSkills.Roguery) < 30)
			{
				return DefaultAlleyModel.AlleyMemberAvailabilityDetail.NotEnoughRoguerySkill;
			}
			if (hero.GetTraitLevel(DefaultTraits.Mercy) > 0)
			{
				return DefaultAlleyModel.AlleyMemberAvailabilityDetail.NotEnoughMercyTrait;
			}
			if (campaignBehavior != null && campaignBehavior.GetAllAssignedClanMembersForOwnedAlleys().Contains(hero))
			{
				return DefaultAlleyModel.AlleyMemberAvailabilityDetail.AlreadyAlleyLeader;
			}
			if (!hero.CanLeadParty())
			{
				return DefaultAlleyModel.AlleyMemberAvailabilityDetail.CanNotLeadParty;
			}
			if (hero.IsPrisoner)
			{
				return DefaultAlleyModel.AlleyMemberAvailabilityDetail.IsPrisoner;
			}
			if (Campaign.Current.Models.DelayedTeleportationModel.GetTeleportationDelayAsHours(hero, alley.Settlement.Party).BaseNumber > 0f)
			{
				return DefaultAlleyModel.AlleyMemberAvailabilityDetail.AvailableWithDelay;
			}
			return DefaultAlleyModel.AlleyMemberAvailabilityDetail.Available;
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x0005A4A2 File Offset: 0x000586A2
		public override int GetDailyIncomeOfAlley(Alley alley)
		{
			return (int)(alley.Settlement.Prosperity / 50f);
		}

		// Token: 0x04000723 RID: 1827
		private const int BaseResponseTimeInDays = 5;

		// Token: 0x04000724 RID: 1828
		private const int MaxResponseTimeInDays = 10;

		// Token: 0x04000725 RID: 1829
		public const int MinimumRoguerySkillNeededForLeadingAnAlley = 30;

		// Token: 0x04000726 RID: 1830
		public const int MaximumMercyTraitNeededForLeadingAnAlley = 0;

		// Token: 0x020004F9 RID: 1273
		public enum AlleyMemberAvailabilityDetail
		{
			// Token: 0x0400156D RID: 5485
			Available,
			// Token: 0x0400156E RID: 5486
			AvailableWithDelay,
			// Token: 0x0400156F RID: 5487
			NotEnoughRoguerySkill,
			// Token: 0x04001570 RID: 5488
			NotEnoughMercyTrait,
			// Token: 0x04001571 RID: 5489
			CanNotLeadParty,
			// Token: 0x04001572 RID: 5490
			AlreadyAlleyLeader,
			// Token: 0x04001573 RID: 5491
			IsPrisoner
		}
	}
}
