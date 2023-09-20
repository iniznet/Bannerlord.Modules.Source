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
	// Token: 0x02000126 RID: 294
	public class DefaultPartySpeedCalculatingModel : PartySpeedModel
	{
		// Token: 0x1700060C RID: 1548
		// (get) Token: 0x0600168F RID: 5775 RVA: 0x0006D596 File Offset: 0x0006B796
		public override float BaseSpeed
		{
			get
			{
				return 5f;
			}
		}

		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x06001690 RID: 5776 RVA: 0x0006D59D File Offset: 0x0006B79D
		public override float MinimumSpeed
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06001691 RID: 5777 RVA: 0x0006D5A4 File Offset: 0x0006B7A4
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
			this.GetCavalryRatioModifier(mobileParty, num4, num7, ref explainedNumber);
			this.GetFootmenPerkBonus(mobileParty, num4, num8, ref explainedNumber);
			int num12 = MathF.Min(num8, num);
			float mountedFootmenRatioModifier = this.GetMountedFootmenRatioModifier(num4, num12);
			explainedNumber.AddFactor(mountedFootmenRatioModifier, DefaultPartySpeedCalculatingModel._textMountedFootmen);
			if (mountedFootmenRatioModifier > 0f && mobileParty.LeaderHero != null && mobileParty.LeaderHero.GetPerkValue(DefaultPerks.Riding.NomadicTraditions))
			{
				explainedNumber.AddFactor(mountedFootmenRatioModifier * DefaultPerks.Riding.NomadicTraditions.PrimaryBonus, DefaultPerks.Riding.NomadicTraditions.Name);
			}
			float num13 = MathF.Min(num5, (float)num6);
			if (num13 > 0f)
			{
				float cargoEffect = this.GetCargoEffect(num13, num6);
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

		// Token: 0x06001692 RID: 5778 RVA: 0x0006DA50 File Offset: 0x0006BC50
		private static void AddCargoStats(MobileParty mobileParty, ref int numberOfAvailableMounts, ref float totalWeightCarried, ref int herdSize)
		{
			ItemRoster itemRoster = mobileParty.ItemRoster;
			int numberOfPackAnimals = itemRoster.NumberOfPackAnimals;
			int numberOfLivestockAnimals = itemRoster.NumberOfLivestockAnimals;
			herdSize += numberOfPackAnimals + numberOfLivestockAnimals;
			numberOfAvailableMounts += itemRoster.NumberOfMounts;
			totalWeightCarried += itemRoster.TotalWeight;
		}

		// Token: 0x06001693 RID: 5779 RVA: 0x0006DA90 File Offset: 0x0006BC90
		private float CalculateBaseSpeedForParty(int menCount)
		{
			return this.BaseSpeed * MathF.Pow(200f / (200f + (float)menCount), 0.4f);
		}

		// Token: 0x06001694 RID: 5780 RVA: 0x0006DAB4 File Offset: 0x0006BCB4
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
				float num2 = ((num / (float)mobileParty.MemberRoster.Count > 0.75f) ? (-0.15f) : (-0.3f));
				finalSpeed.AddFactor(num2, DefaultPartySpeedCalculatingModel._movingInForest);
				if (PartyBaseHelper.HasFeat(mobileParty.Party, DefaultCulturalFeats.BattanianForestSpeedFeat))
				{
					float num3 = DefaultCulturalFeats.BattanianForestSpeedFeat.EffectBonus * 0.3f;
					finalSpeed.AddFactor(num3, DefaultPartySpeedCalculatingModel._culture);
				}
			}
			else if (faceTerrainType == TerrainType.Water || faceTerrainType == TerrainType.River || faceTerrainType == TerrainType.Bridge || faceTerrainType == TerrainType.ShallowRiver)
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
			if (Campaign.Current.Models.MapWeatherModel.GetIsSnowTerrainInPos(mobileParty.Position2D.ToVec3(0f)))
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
				PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Scouting.UncannyInsight, effectiveScout.CharacterObject, DefaultSkills.Scouting, true, ref finalSpeed, 200);
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

		// Token: 0x06001695 RID: 5781 RVA: 0x0006DE69 File Offset: 0x0006C069
		private float GetCargoEffect(float weightCarried, int partyCapacity)
		{
			return -0.02f * weightCarried / (float)partyCapacity;
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x0006DE76 File Offset: 0x0006C076
		private float GetOverBurdenedEffect(float totalWeightCarried, int partyCapacity)
		{
			return -0.4f * (totalWeightCarried / (float)partyCapacity);
		}

		// Token: 0x06001697 RID: 5783 RVA: 0x0006DE84 File Offset: 0x0006C084
		private float GetOverPartySizeEffect(MobileParty mobileParty)
		{
			int partySizeLimit = mobileParty.Party.PartySizeLimit;
			int numberOfAllMembers = mobileParty.Party.NumberOfAllMembers;
			return 1f / ((float)numberOfAllMembers / (float)partySizeLimit) - 1f;
		}

		// Token: 0x06001698 RID: 5784 RVA: 0x0006DEBC File Offset: 0x0006C0BC
		private float GetOverPrisonerSizeEffect(MobileParty mobileParty)
		{
			int prisonerSizeLimit = mobileParty.Party.PrisonerSizeLimit;
			int numberOfPrisoners = mobileParty.Party.NumberOfPrisoners;
			return 1f / ((float)numberOfPrisoners / (float)prisonerSizeLimit) - 1f;
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x0006DEF2 File Offset: 0x0006C0F2
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

		// Token: 0x0600169A RID: 5786 RVA: 0x0006DF24 File Offset: 0x0006C124
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

		// Token: 0x0600169B RID: 5787 RVA: 0x0006DF7C File Offset: 0x0006C17C
		private void GetCavalryRatioModifier(MobileParty party, int totalMenCount, int totalCavalryCount, ref ExplainedNumber result)
		{
			if (totalMenCount > 0 && totalCavalryCount > 0)
			{
				float num = 0.4f * (float)totalCavalryCount / (float)totalMenCount;
				result.AddFactor(num, DefaultPartySpeedCalculatingModel._textCavalry);
			}
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x0006DFAA File Offset: 0x0006C1AA
		private float GetMountedFootmenRatioModifier(int totalMenCount, int totalCavalryCount)
		{
			if (totalMenCount == 0)
			{
				return 0f;
			}
			return 0.2f * (float)totalCavalryCount / (float)totalMenCount;
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x0006DFC0 File Offset: 0x0006C1C0
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

		// Token: 0x0600169E RID: 5790 RVA: 0x0006E014 File Offset: 0x0006C214
		private static float GetSizeModifierWounded(int totalMenCount, int totalWoundedMenCount)
		{
			return MathF.Pow((10f + (float)totalMenCount) / (10f + (float)totalMenCount - (float)totalWoundedMenCount), 0.33f);
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x0006E034 File Offset: 0x0006C234
		private static float GetSizeModifierPrisoner(int totalMenCount, int totalPrisonerCount)
		{
			return MathF.Pow((10f + (float)totalMenCount + (float)totalPrisonerCount) / (10f + (float)totalMenCount), 0.33f);
		}

		// Token: 0x040007EF RID: 2031
		private static readonly TextObject _textCargo = new TextObject("{=fSGY71wd}Cargo within capacity", null);

		// Token: 0x040007F0 RID: 2032
		private static readonly TextObject _textOverburdened = new TextObject("{=xgO3cCgR}Overburdened", null);

		// Token: 0x040007F1 RID: 2033
		private static readonly TextObject _textOverPartySize = new TextObject("{=bO5gL3FI}Men within party size", null);

		// Token: 0x040007F2 RID: 2034
		private static readonly TextObject _textOverPrisonerSize = new TextObject("{=Ix8YjLPD}Men within prisoner size", null);

		// Token: 0x040007F3 RID: 2035
		private static readonly TextObject _textCavalry = new TextObject("{=YVGtcLHF}Cavalry", null);

		// Token: 0x040007F4 RID: 2036
		private static readonly TextObject _textKhuzaitCavalryBonus = new TextObject("{=yi07dBks}Khuzait Cavalry Bonus", null);

		// Token: 0x040007F5 RID: 2037
		private static readonly TextObject _textMountedFootmen = new TextObject("{=5bSWSaPl}Footmen on horses", null);

		// Token: 0x040007F6 RID: 2038
		private static readonly TextObject _textWounded = new TextObject("{=aLsVKIRy}Wounded Members", null);

		// Token: 0x040007F7 RID: 2039
		private static readonly TextObject _textPrisoners = new TextObject("{=N6QTvjMf}Prisoners", null);

		// Token: 0x040007F8 RID: 2040
		private static readonly TextObject _textHerd = new TextObject("{=NhAMSaWU}Herd", null);

		// Token: 0x040007F9 RID: 2041
		private static readonly TextObject _textHighMorale = new TextObject("{=aDQcIGfH}High Morale", null);

		// Token: 0x040007FA RID: 2042
		private static readonly TextObject _textLowMorale = new TextObject("{=ydspCDIy}Low Morale", null);

		// Token: 0x040007FB RID: 2043
		private static readonly TextObject _textCaravan = new TextObject("{=vvabqi2w}Caravan", null);

		// Token: 0x040007FC RID: 2044
		private static readonly TextObject _textDisorganized = new TextObject("{=JuwBb2Yg}Disorganized", null);

		// Token: 0x040007FD RID: 2045
		private static readonly TextObject _movingInForest = new TextObject("{=rTFaZCdY}Forest", null);

		// Token: 0x040007FE RID: 2046
		private static readonly TextObject _fordEffect = new TextObject("{=NT5fwUuJ}Fording", null);

		// Token: 0x040007FF RID: 2047
		private static readonly TextObject _night = new TextObject("{=fAxjyMt5}Night", null);

		// Token: 0x04000800 RID: 2048
		private static readonly TextObject _snow = new TextObject("{=vLjgcdgB}Snow", null);

		// Token: 0x04000801 RID: 2049
		private static readonly TextObject _desert = new TextObject("{=ecUwABe2}Desert", null);

		// Token: 0x04000802 RID: 2050
		private static readonly TextObject _sturgiaSnowBonus = new TextObject("{=0VfEGekD}Sturgia Snow Bonus", null);

		// Token: 0x04000803 RID: 2051
		private static readonly TextObject _culture = GameTexts.FindText("str_culture", null);

		// Token: 0x04000804 RID: 2052
		private const float MovingAtForestEffect = -0.3f;

		// Token: 0x04000805 RID: 2053
		private const float MovingAtWaterEffect = -0.3f;

		// Token: 0x04000806 RID: 2054
		private const float MovingAtNightEffect = -0.25f;

		// Token: 0x04000807 RID: 2055
		private const float MovingOnSnowEffect = -0.1f;

		// Token: 0x04000808 RID: 2056
		private const float MovingInDesertEffect = -0.1f;

		// Token: 0x04000809 RID: 2057
		private const float CavalryEffect = 0.4f;

		// Token: 0x0400080A RID: 2058
		private const float MountedFootMenEffect = 0.2f;

		// Token: 0x0400080B RID: 2059
		private const float HerdEffect = -0.4f;

		// Token: 0x0400080C RID: 2060
		private const float WoundedEffect = -0.05f;

		// Token: 0x0400080D RID: 2061
		private const float CargoEffect = -0.02f;

		// Token: 0x0400080E RID: 2062
		private const float OverburdenedEffect = -0.4f;

		// Token: 0x0400080F RID: 2063
		private const float HighMoraleThresold = 70f;

		// Token: 0x04000810 RID: 2064
		private const float LowMoraleThresold = 30f;

		// Token: 0x04000811 RID: 2065
		private const float HighMoraleEffect = 0.05f;

		// Token: 0x04000812 RID: 2066
		private const float LowMoraleEffect = -0.1f;

		// Token: 0x04000813 RID: 2067
		private const float DisorganizedEffect = -0.4f;
	}
}
