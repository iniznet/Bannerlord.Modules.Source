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
	public class DefaultAlleyModel : AlleyModel
	{
		private CharacterObject _thug
		{
			get
			{
				return MBObjectManager.Instance.GetObject<CharacterObject>("gangster_1");
			}
		}

		private CharacterObject _expertThug
		{
			get
			{
				return MBObjectManager.Instance.GetObject<CharacterObject>("gangster_2");
			}
		}

		private CharacterObject _masterThug
		{
			get
			{
				return MBObjectManager.Instance.GetObject<CharacterObject>("gangster_3");
			}
		}

		public override CampaignTime DestroyAlleyAfterDaysWhenLeaderIsDeath
		{
			get
			{
				return CampaignTime.Days(4f);
			}
		}

		public override int MinimumTroopCountInPlayerOwnedAlley
		{
			get
			{
				return 5;
			}
		}

		public override int MaximumTroopCountInPlayerOwnedAlley
		{
			get
			{
				return 10;
			}
		}

		public override float GetDailyCrimeRatingOfAlley
		{
			get
			{
				return 0.5f;
			}
		}

		public override float GetDailyXpGainForAssignedClanMember(Hero assignedHero)
		{
			return 200f;
		}

		public override float GetDailyXpGainForMainHero()
		{
			return 40f;
		}

		public override float GetInitialXpGainForMainHero()
		{
			return 1500f;
		}

		public override float GetXpGainAfterSuccessfulAlleyDefenseForMainHero()
		{
			return 6000f;
		}

		public override TroopRoster GetTroopsOfAIOwnedAlley(Alley alley)
		{
			return this.GetTroopsOfAlleyInternal(alley);
		}

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

		public override float GetAlleyAttackResponseTimeInDays(TroopRoster troopRoster)
		{
			float num = 0f;
			foreach (TroopRosterElement troopRosterElement in troopRoster.GetTroopRoster())
			{
				num += (((float)troopRosterElement.Character.Tier > 4f) ? 4f : ((float)troopRosterElement.Character.Tier)) * (float)troopRosterElement.Number;
			}
			return (float)Math.Min(10, 5 + (int)(num / 8f));
		}

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

		public override int GetDailyIncomeOfAlley(Alley alley)
		{
			return (int)(alley.Settlement.Prosperity / 50f);
		}

		private const int BaseResponseTimeInDays = 5;

		private const int MaxResponseTimeInDays = 10;

		public const int MinimumRoguerySkillNeededForLeadingAnAlley = 30;

		public const int MaximumMercyTraitNeededForLeadingAnAlley = 0;

		public enum AlleyMemberAvailabilityDetail
		{
			Available,
			AvailableWithDelay,
			NotEnoughRoguerySkill,
			NotEnoughMercyTrait,
			CanNotLeadParty,
			AlreadyAlleyLeader,
			IsPrisoner
		}
	}
}
