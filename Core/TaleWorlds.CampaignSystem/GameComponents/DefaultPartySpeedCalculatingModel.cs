using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultPartySpeedCalculatingModel : PartySpeedModel
	{
		public override float BaseSpeed
		{
			get
			{
				return 5f;
			}
		}

		public override float MinimumSpeed
		{
			get
			{
				return 1f;
			}
		}

		public override ExplainedNumber CalculateBaseSpeed(MobileParty mobileParty, bool includeDescriptions = false, int additionalTroopOnFootCount = 0, int additionalTroopOnHorseCount = 0)
		{
			PartyBase party = mobileParty.Party;
			int num = 0;
			float num2 = 0f;
			int num3 = 0;
			int num4 = mobileParty.MemberRoster.TotalManCount + additionalTroopOnFootCount + additionalTroopOnHorseCount;
			DefaultPartySpeedCalculatingModel.AddCargoStats(mobileParty, ref num, ref num2, ref num3);
			float num5 = mobileParty.ItemRoster.TotalWeight;
			int num6 = (int)Campaign.Current.Models.InventoryCapacityModel.CalculateInventoryCapacity(mobileParty, false, additionalTroopOnFootCount, additionalTroopOnHorseCount, 0, false).ResultNumber;
			int num7 = party.NumberOfMenWithHorse + additionalTroopOnHorseCount;
			int num8 = party.NumberOfMenWithoutHorse + additionalTroopOnFootCount;
			int num9 = party.MemberRoster.TotalWounded;
			int num10 = party.PrisonRoster.TotalManCount;
			float morale = mobileParty.Morale;
			if (mobileParty.AttachedParties.Count != 0)
			{
				foreach (MobileParty mobileParty2 in mobileParty.AttachedParties)
				{
					DefaultPartySpeedCalculatingModel.AddCargoStats(mobileParty2, ref num, ref num2, ref num3);
					num4 += mobileParty2.MemberRoster.TotalManCount;
					num5 += mobileParty2.ItemRoster.TotalWeight;
					num6 += mobileParty2.InventoryCapacity;
					num7 += mobileParty2.Party.NumberOfMenWithHorse;
					num8 += mobileParty2.Party.NumberOfMenWithoutHorse;
					num9 += mobileParty2.MemberRoster.TotalWounded;
					num10 += mobileParty2.PrisonRoster.TotalManCount;
				}
			}
			float num11 = this.CalculateBaseSpeedForParty(num4);
			ExplainedNumber explainedNumber = new ExplainedNumber(num11, includeDescriptions, null);
			bool flag = Campaign.Current.Models.MapWeatherModel.GetWeatherEffectOnTerrainForPosition(mobileParty.Position2D) == MapWeatherModel.WeatherEventEffectOnTerrain.Wet;
			this.GetFootmenPerkBonus(mobileParty, num4, num8, ref explainedNumber);
			float cavalryRatioModifier = this.GetCavalryRatioModifier(num4, num7);
			int num12 = MathF.Min(num8, num);
			float mountedFootmenRatioModifier = this.GetMountedFootmenRatioModifier(num4, num12);
			explainedNumber.AddFactor(cavalryRatioModifier, DefaultPartySpeedCalculatingModel._textCavalry);
			explainedNumber.AddFactor(mountedFootmenRatioModifier, DefaultPartySpeedCalculatingModel._textMountedFootmen);
			if (flag)
			{
				float num13 = cavalryRatioModifier * 0.3f;
				float num14 = mountedFootmenRatioModifier * 0.3f;
				explainedNumber.AddFactor(-num13, DefaultPartySpeedCalculatingModel._textCavalryWeatherPenalty);
				explainedNumber.AddFactor(-num14, DefaultPartySpeedCalculatingModel._textMountedFootmenWeatherPenalty);
			}
			if (mountedFootmenRatioModifier > 0f && mobileParty.LeaderHero != null && mobileParty.LeaderHero.GetPerkValue(DefaultPerks.Riding.NomadicTraditions))
			{
				explainedNumber.AddFactor(mountedFootmenRatioModifier * DefaultPerks.Riding.NomadicTraditions.PrimaryBonus, DefaultPerks.Riding.NomadicTraditions.Name);
			}
			float num15 = MathF.Min(num5, (float)num6);
			if (num15 > 0f)
			{
				float cargoEffect = this.GetCargoEffect(num15, num6);
				explainedNumber.AddFactor(cargoEffect, DefaultPartySpeedCalculatingModel._textCargo);
			}
			if (num2 > (float)num6)
			{
				float overBurdenedEffect = this.GetOverBurdenedEffect(num2 - (float)num6, num6);
				explainedNumber.AddFactor(overBurdenedEffect, DefaultPartySpeedCalculatingModel._textOverburdened);
				if (mobileParty.HasPerk(DefaultPerks.Athletics.Energetic, false))
				{
					explainedNumber.AddFactor(overBurdenedEffect * DefaultPerks.Athletics.Energetic.PrimaryBonus, DefaultPerks.Athletics.Energetic.Name);
				}
				if (mobileParty.HasPerk(DefaultPerks.Scouting.Unburdened, false))
				{
					explainedNumber.AddFactor(overBurdenedEffect * DefaultPerks.Scouting.Unburdened.PrimaryBonus, DefaultPerks.Scouting.Unburdened.Name);
				}
			}
			if (mobileParty.HasPerk(DefaultPerks.Riding.SweepingWind, true))
			{
				explainedNumber.AddFactor(DefaultPerks.Riding.SweepingWind.SecondaryBonus, DefaultPerks.Riding.SweepingWind.Name);
			}
			if (mobileParty.Party.NumberOfAllMembers > mobileParty.Party.PartySizeLimit)
			{
				float overPartySizeEffect = this.GetOverPartySizeEffect(mobileParty);
				explainedNumber.AddFactor(overPartySizeEffect, DefaultPartySpeedCalculatingModel._textOverPartySize);
			}
			num3 += MathF.Max(0, num - num12);
			if (!mobileParty.IsVillager)
			{
				float herdingModifier = this.GetHerdingModifier(num4, num3);
				explainedNumber.AddFactor(herdingModifier, DefaultPartySpeedCalculatingModel._textHerd);
				if (mobileParty.HasPerk(DefaultPerks.Riding.Shepherd, false))
				{
					explainedNumber.AddFactor(herdingModifier * DefaultPerks.Riding.Shepherd.PrimaryBonus, DefaultPerks.Riding.Shepherd.Name);
				}
			}
			float woundedModifier = this.GetWoundedModifier(num4, num9, mobileParty);
			explainedNumber.AddFactor(woundedModifier, DefaultPartySpeedCalculatingModel._textWounded);
			if (!mobileParty.IsCaravan)
			{
				if (mobileParty.Party.NumberOfPrisoners > mobileParty.Party.PrisonerSizeLimit)
				{
					float overPrisonerSizeEffect = this.GetOverPrisonerSizeEffect(mobileParty);
					explainedNumber.AddFactor(overPrisonerSizeEffect, DefaultPartySpeedCalculatingModel._textOverPrisonerSize);
				}
				float sizeModifierPrisoner = DefaultPartySpeedCalculatingModel.GetSizeModifierPrisoner(num4, num10);
				explainedNumber.AddFactor(1f / sizeModifierPrisoner - 1f, DefaultPartySpeedCalculatingModel._textPrisoners);
			}
			if (morale > 70f)
			{
				explainedNumber.AddFactor(0.05f * ((morale - 70f) / 30f), DefaultPartySpeedCalculatingModel._textHighMorale);
			}
			if (morale < 30f)
			{
				explainedNumber.AddFactor(-0.1f * (1f - mobileParty.Morale / 30f), DefaultPartySpeedCalculatingModel._textLowMorale);
			}
			if (mobileParty == MobileParty.MainParty)
			{
				float playerMapMovementSpeedBonusMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerMapMovementSpeedBonusMultiplier();
				if (playerMapMovementSpeedBonusMultiplier > 0f)
				{
					explainedNumber.AddFactor(playerMapMovementSpeedBonusMultiplier, GameTexts.FindText("str_game_difficulty", null));
				}
			}
			if (mobileParty.IsCaravan)
			{
				explainedNumber.AddFactor(0.1f, DefaultPartySpeedCalculatingModel._textCaravan);
			}
			if (mobileParty.IsDisorganized)
			{
				explainedNumber.AddFactor(-0.4f, DefaultPartySpeedCalculatingModel._textDisorganized);
			}
			explainedNumber.LimitMin(this.MinimumSpeed);
			return explainedNumber;
		}

		private static void AddCargoStats(MobileParty mobileParty, ref int numberOfAvailableMounts, ref float totalWeightCarried, ref int herdSize)
		{
			ItemRoster itemRoster = mobileParty.ItemRoster;
			int numberOfPackAnimals = itemRoster.NumberOfPackAnimals;
			int numberOfLivestockAnimals = itemRoster.NumberOfLivestockAnimals;
			herdSize += numberOfPackAnimals + numberOfLivestockAnimals;
			numberOfAvailableMounts += itemRoster.NumberOfMounts;
			totalWeightCarried += itemRoster.TotalWeight;
		}

		private float CalculateBaseSpeedForParty(int menCount)
		{
			return this.BaseSpeed * MathF.Pow(200f / (200f + (float)menCount), 0.4f);
		}

		public override ExplainedNumber CalculateFinalSpeed(MobileParty mobileParty, ExplainedNumber finalSpeed)
		{
			if (mobileParty.IsCustomParty && !((CustomPartyComponent)mobileParty.PartyComponent).BaseSpeed.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				finalSpeed = new ExplainedNumber(((CustomPartyComponent)mobileParty.PartyComponent).BaseSpeed, false, null);
			}
			TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(mobileParty.CurrentNavigationFace);
			Hero effectiveScout = mobileParty.EffectiveScout;
			if (faceTerrainType == TerrainType.Forest)
			{
				float num = 0f;
				if (effectiveScout != null && effectiveScout.GetPerkValue(DefaultPerks.Scouting.ForestKin))
				{
					for (int i = 0; i < mobileParty.MemberRoster.Count; i++)
					{
						if (!mobileParty.MemberRoster.GetCharacterAtIndex(i).IsMounted)
						{
							num += (float)mobileParty.MemberRoster.GetElementNumber(i);
						}
					}
				}
				float num2 = ((num / (float)mobileParty.MemberRoster.TotalManCount >= 0.75f) ? (-0.3f * -DefaultPerks.Scouting.ForestKin.PrimaryBonus) : (-0.3f));
				finalSpeed.AddFactor(num2, DefaultPartySpeedCalculatingModel._movingInForest);
				if (PartyBaseHelper.HasFeat(mobileParty.Party, DefaultCulturalFeats.BattanianForestSpeedFeat))
				{
					float num3 = DefaultCulturalFeats.BattanianForestSpeedFeat.EffectBonus * 0.3f;
					finalSpeed.AddFactor(num3, DefaultPartySpeedCalculatingModel._culture);
				}
			}
			else if (faceTerrainType == TerrainType.Water || faceTerrainType == TerrainType.River || faceTerrainType == TerrainType.Bridge || faceTerrainType == TerrainType.Fording)
			{
				finalSpeed.AddFactor(-0.3f, DefaultPartySpeedCalculatingModel._fordEffect);
			}
			else if (faceTerrainType == TerrainType.Desert || faceTerrainType == TerrainType.Dune)
			{
				if (!PartyBaseHelper.HasFeat(mobileParty.Party, DefaultCulturalFeats.AseraiDesertFeat))
				{
					finalSpeed.AddFactor(-0.1f, DefaultPartySpeedCalculatingModel._desert);
				}
				if (effectiveScout != null && effectiveScout.GetPerkValue(DefaultPerks.Scouting.DesertBorn))
				{
					finalSpeed.AddFactor(DefaultPerks.Scouting.DesertBorn.PrimaryBonus, DefaultPerks.Scouting.DesertBorn.Name);
				}
			}
			else if ((faceTerrainType == TerrainType.Plain || faceTerrainType == TerrainType.Steppe) && effectiveScout != null && effectiveScout.GetPerkValue(DefaultPerks.Scouting.Pathfinder))
			{
				finalSpeed.AddFactor(DefaultPerks.Scouting.Pathfinder.PrimaryBonus, DefaultPerks.Scouting.Pathfinder.Name);
			}
			MapWeatherModel.WeatherEvent weatherEventInPosition = Campaign.Current.Models.MapWeatherModel.GetWeatherEventInPosition(mobileParty.Position2D);
			if (weatherEventInPosition == MapWeatherModel.WeatherEvent.Snowy || weatherEventInPosition == MapWeatherModel.WeatherEvent.Blizzard)
			{
				finalSpeed.AddFactor(-0.1f, DefaultPartySpeedCalculatingModel._snow);
			}
			if (Campaign.Current.IsNight)
			{
				finalSpeed.AddFactor(-0.25f, DefaultPartySpeedCalculatingModel._night);
				if (effectiveScout != null && effectiveScout.GetPerkValue(DefaultPerks.Scouting.NightRunner))
				{
					finalSpeed.AddFactor(DefaultPerks.Scouting.NightRunner.PrimaryBonus, DefaultPerks.Scouting.NightRunner.Name);
				}
			}
			else if (effectiveScout != null && effectiveScout.GetPerkValue(DefaultPerks.Scouting.DayTraveler))
			{
				finalSpeed.AddFactor(DefaultPerks.Scouting.DayTraveler.PrimaryBonus, DefaultPerks.Scouting.DayTraveler.Name);
			}
			if (effectiveScout != null)
			{
				PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Scouting.UncannyInsight, effectiveScout.CharacterObject, DefaultSkills.Scouting, true, ref finalSpeed, Campaign.Current.Models.CharacterDevelopmentModel.MinSkillRequiredForEpicPerkBonus);
				if (effectiveScout.GetPerkValue(DefaultPerks.Scouting.ForcedMarch) && mobileParty.Morale > 75f)
				{
					finalSpeed.AddFactor(DefaultPerks.Scouting.ForcedMarch.PrimaryBonus, DefaultPerks.Scouting.ForcedMarch.Name);
				}
				if (mobileParty.DefaultBehavior == AiBehavior.EngageParty)
				{
					MobileParty targetParty = mobileParty.TargetParty;
					if (targetParty != null && targetParty.MapFaction.IsAtWarWith(mobileParty.MapFaction) && effectiveScout.GetPerkValue(DefaultPerks.Scouting.Tracker))
					{
						finalSpeed.AddFactor(DefaultPerks.Scouting.Tracker.SecondaryBonus, DefaultPerks.Scouting.Tracker.Name);
					}
				}
			}
			Army army = mobileParty.Army;
			if (((army != null) ? army.LeaderParty : null) != null && mobileParty.Army.LeaderParty != mobileParty && mobileParty.AttachedTo != mobileParty.Army.LeaderParty && mobileParty.Army.LeaderParty.HasPerk(DefaultPerks.Tactics.CallToArms, false))
			{
				finalSpeed.AddFactor(DefaultPerks.Tactics.CallToArms.PrimaryBonus, DefaultPerks.Tactics.CallToArms.Name);
			}
			finalSpeed.LimitMin(this.MinimumSpeed);
			return finalSpeed;
		}

		private float GetCargoEffect(float weightCarried, int partyCapacity)
		{
			return -0.02f * weightCarried / (float)partyCapacity;
		}

		private float GetOverBurdenedEffect(float totalWeightCarried, int partyCapacity)
		{
			return -0.4f * (totalWeightCarried / (float)partyCapacity);
		}

		private float GetOverPartySizeEffect(MobileParty mobileParty)
		{
			int partySizeLimit = mobileParty.Party.PartySizeLimit;
			int numberOfAllMembers = mobileParty.Party.NumberOfAllMembers;
			return 1f / ((float)numberOfAllMembers / (float)partySizeLimit) - 1f;
		}

		private float GetOverPrisonerSizeEffect(MobileParty mobileParty)
		{
			int prisonerSizeLimit = mobileParty.Party.PrisonerSizeLimit;
			int numberOfPrisoners = mobileParty.Party.NumberOfPrisoners;
			return 1f / ((float)numberOfPrisoners / (float)prisonerSizeLimit) - 1f;
		}

		private float GetHerdingModifier(int totalMenCount, int herdSize)
		{
			herdSize -= totalMenCount;
			if (herdSize <= 0)
			{
				return 0f;
			}
			if (totalMenCount == 0)
			{
				return -0.8f;
			}
			return MathF.Max(-0.8f, -0.3f * ((float)herdSize / (float)totalMenCount));
		}

		private float GetWoundedModifier(int totalMenCount, int numWounded, MobileParty party)
		{
			if (numWounded <= totalMenCount / 4)
			{
				return 0f;
			}
			if (totalMenCount == 0)
			{
				return -0.5f;
			}
			float num = MathF.Max(-0.8f, -0.05f * (float)numWounded / (float)totalMenCount);
			ExplainedNumber explainedNumber = new ExplainedNumber(num, false, null);
			PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.Sledges, party, true, ref explainedNumber);
			return explainedNumber.ResultNumber;
		}

		private float GetCavalryRatioModifier(int totalMenCount, int totalCavalryCount)
		{
			if (totalMenCount == 0 || totalCavalryCount == 0)
			{
				return 0f;
			}
			return 0.4f * (float)totalCavalryCount / (float)totalMenCount;
		}

		private float GetMountedFootmenRatioModifier(int totalMenCount, int totalMountedFootmenCount)
		{
			if (totalMenCount == 0 || totalMountedFootmenCount == 0)
			{
				return 0f;
			}
			return 0.2f * (float)totalMountedFootmenCount / (float)totalMenCount;
		}

		private void GetFootmenPerkBonus(MobileParty party, int totalMenCount, int totalFootmenCount, ref ExplainedNumber result)
		{
			if (totalMenCount == 0)
			{
				return;
			}
			float num = (float)totalFootmenCount / (float)totalMenCount;
			if (party.HasPerk(DefaultPerks.Athletics.Strong, true) && !num.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				result.AddFactor(num * DefaultPerks.Athletics.Strong.SecondaryBonus, DefaultPerks.Athletics.Strong.Name);
			}
		}

		private static float GetSizeModifierWounded(int totalMenCount, int totalWoundedMenCount)
		{
			return MathF.Pow((10f + (float)totalMenCount) / (10f + (float)totalMenCount - (float)totalWoundedMenCount), 0.33f);
		}

		private static float GetSizeModifierPrisoner(int totalMenCount, int totalPrisonerCount)
		{
			return MathF.Pow((10f + (float)totalMenCount + (float)totalPrisonerCount) / (10f + (float)totalMenCount), 0.33f);
		}

		private static readonly TextObject _textCargo = new TextObject("{=fSGY71wd}Cargo within capacity", null);

		private static readonly TextObject _textOverburdened = new TextObject("{=xgO3cCgR}Overburdened", null);

		private static readonly TextObject _textOverPartySize = new TextObject("{=bO5gL3FI}Men within party size", null);

		private static readonly TextObject _textOverPrisonerSize = new TextObject("{=Ix8YjLPD}Men within prisoner size", null);

		private static readonly TextObject _textCavalry = new TextObject("{=YVGtcLHF}Cavalry", null);

		private static readonly TextObject _textCavalryWeatherPenalty = new TextObject("{=Cb0k9KM8}Cavalry weather penalty", null);

		private static readonly TextObject _textKhuzaitCavalryBonus = new TextObject("{=yi07dBks}Khuzait cavalry bonus", null);

		private static readonly TextObject _textMountedFootmen = new TextObject("{=5bSWSaPl}Footmen on horses", null);

		private static readonly TextObject _textMountedFootmenWeatherPenalty = new TextObject("{=JAKoFNgt}Footmen on horses weather penalty", null);

		private static readonly TextObject _textWounded = new TextObject("{=aLsVKIRy}Wounded members", null);

		private static readonly TextObject _textPrisoners = new TextObject("{=N6QTvjMf}Prisoners", null);

		private static readonly TextObject _textHerd = new TextObject("{=NhAMSaWU}Herding", null);

		private static readonly TextObject _textHighMorale = new TextObject("{=aDQcIGfH}High morale", null);

		private static readonly TextObject _textLowMorale = new TextObject("{=ydspCDIy}Low morale", null);

		private static readonly TextObject _textCaravan = new TextObject("{=vvabqi2w}Caravan", null);

		private static readonly TextObject _textDisorganized = new TextObject("{=JuwBb2Yg}Disorganized", null);

		private static readonly TextObject _movingInForest = new TextObject("{=rTFaZCdY}Forest", null);

		private static readonly TextObject _fordEffect = new TextObject("{=NT5fwUuJ}Fording", null);

		private static readonly TextObject _night = new TextObject("{=fAxjyMt5}Night", null);

		private static readonly TextObject _snow = new TextObject("{=vLjgcdgB}Snow", null);

		private static readonly TextObject _desert = new TextObject("{=ecUwABe2}Desert", null);

		private static readonly TextObject _sturgiaSnowBonus = new TextObject("{=0VfEGekD}Sturgia snow bonus", null);

		private static readonly TextObject _culture = GameTexts.FindText("str_culture", null);

		private const float MovingAtForestEffect = -0.3f;

		private const float MovingAtWaterEffect = -0.3f;

		private const float MovingAtNightEffect = -0.25f;

		private const float MovingOnSnowEffect = -0.1f;

		private const float MovingInDesertEffect = -0.1f;

		private const float CavalryEffect = 0.4f;

		private const float MountedFootMenEffect = 0.2f;

		private const float HerdEffect = -0.4f;

		private const float WoundedEffect = -0.05f;

		private const float CargoEffect = -0.02f;

		private const float OverburdenedEffect = -0.4f;

		private const float HighMoraleThresold = 70f;

		private const float LowMoraleThresold = 30f;

		private const float HighMoraleEffect = 0.05f;

		private const float LowMoraleEffect = -0.1f;

		private const float DisorganizedEffect = -0.4f;
	}
}
