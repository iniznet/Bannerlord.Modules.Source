using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultMapVisibilityModel : MapVisibilityModel
	{
		public override ExplainedNumber GetPartySpottingRange(MobileParty party, bool includeDescriptions = false)
		{
			float num = (Campaign.Current.IsNight ? 6f : 12f);
			ExplainedNumber explainedNumber = new ExplainedNumber(num, includeDescriptions, null);
			TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(party.CurrentNavigationFace);
			SkillHelper.AddSkillBonusForParty(DefaultSkills.Scouting, DefaultSkillEffects.TrackingSpottingDistance, party, ref explainedNumber);
			PerkHelper.AddPerkBonusForParty(DefaultPerks.Bow.EagleEye, party, false, ref explainedNumber);
			Hero effectiveScout = party.EffectiveScout;
			if (effectiveScout != null)
			{
				if ((faceTerrainType == TerrainType.Plain || faceTerrainType == TerrainType.Steppe) && effectiveScout.GetPerkValue(DefaultPerks.Scouting.WaterDiviner))
				{
					explainedNumber.AddFactor(DefaultPerks.Scouting.WaterDiviner.PrimaryBonus, DefaultPerks.Scouting.WaterDiviner.Name);
				}
				else if (faceTerrainType == TerrainType.Forest && PartyBaseHelper.HasFeat(party.Party, DefaultCulturalFeats.BattanianForestSpeedFeat))
				{
					explainedNumber.AddFactor(0.15f, GameTexts.FindText("str_culture", null));
				}
				if (Campaign.Current.IsNight && effectiveScout.GetPerkValue(DefaultPerks.Scouting.NightRunner))
				{
					explainedNumber.AddFactor(DefaultPerks.Scouting.NightRunner.SecondaryBonus, DefaultPerks.Scouting.NightRunner.Name);
				}
				else if (effectiveScout.GetPerkValue(DefaultPerks.Scouting.DayTraveler))
				{
					explainedNumber.AddFactor(DefaultPerks.Scouting.DayTraveler.SecondaryBonus, DefaultPerks.Scouting.DayTraveler.Name);
				}
				if (!party.IsMoving && party.StationaryStartTime.ElapsedHoursUntilNow >= 1f && effectiveScout.GetPerkValue(DefaultPerks.Scouting.VantagePoint))
				{
					explainedNumber.AddFactor(DefaultPerks.Scouting.VantagePoint.PrimaryBonus, DefaultPerks.Scouting.VantagePoint.Name);
				}
				if (effectiveScout.GetPerkValue(DefaultPerks.Scouting.MountedScouts))
				{
					float num2 = 0f;
					for (int i = 0; i < party.MemberRoster.Count; i++)
					{
						if (party.MemberRoster.GetCharacterAtIndex(i).DefaultFormationClass.Equals(FormationClass.Cavalry))
						{
							num2 += (float)party.MemberRoster.GetElementNumber(i);
						}
					}
					if (num2 / (float)party.MemberRoster.TotalManCount >= 0.5f)
					{
						explainedNumber.AddFactor(DefaultPerks.Scouting.MountedScouts.PrimaryBonus, DefaultPerks.Scouting.MountedScouts.Name);
					}
				}
			}
			return explainedNumber;
		}

		public override float GetPartyRelativeInspectionRange(IMapPoint party)
		{
			return 0.5f;
		}

		public override float GetPartySpottingDifficulty(MobileParty spottingParty, MobileParty party)
		{
			float num = 1f;
			if (party != null && spottingParty != null && Campaign.Current.MapSceneWrapper.GetFaceTerrainType(party.CurrentNavigationFace) == TerrainType.Forest)
			{
				float num2 = 0.3f;
				if (spottingParty.HasPerk(DefaultPerks.Scouting.KeenSight, false))
				{
					num2 += num2 * DefaultPerks.Scouting.KeenSight.PrimaryBonus;
				}
				num += num2;
			}
			return (1f / MathF.Pow((float)(party.Party.NumberOfAllMembers + party.Party.NumberOfPrisoners + 2) * 0.2f, 0.6f) + 0.94f) * num;
		}

		public override float GetHideoutSpottingDistance()
		{
			if (MobileParty.MainParty.HasPerk(DefaultPerks.Scouting.RumourNetwork, true))
			{
				return MobileParty.MainParty.SeeingRange * 1.2f * (1f + DefaultPerks.Scouting.RumourNetwork.SecondaryBonus);
			}
			return MobileParty.MainParty.SeeingRange * 1.2f;
		}

		private const float PartySpottingDifficultyInForests = 0.3f;
	}
}
