using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultTroopSacrificeModel : TroopSacrificeModel
	{
		public override int BreakOutArmyLeaderRelationPenalty
		{
			get
			{
				return -5;
			}
		}

		public override int BreakOutArmyMemberRelationPenalty
		{
			get
			{
				return -1;
			}
		}

		public override int GetLostTroopCountForBreakingInBesiegedSettlement(MobileParty party, SiegeEvent siegeEvent)
		{
			return this.GetLostTroopCount(party, siegeEvent);
		}

		public override int GetLostTroopCountForBreakingOutOfBesiegedSettlement(MobileParty party, SiegeEvent siegeEvent)
		{
			return this.GetLostTroopCount(party, siegeEvent);
		}

		public override int GetNumberOfTroopsSacrificedForTryingToGetAway(BattleSideEnum battleSide, MapEvent mapEvent)
		{
			mapEvent.RecalculateStrengthOfSides();
			MapEventSide mapEventSide = mapEvent.GetMapEventSide(battleSide);
			float num = mapEvent.StrengthOfSide[(int)battleSide] + 1f;
			float num2 = mapEvent.StrengthOfSide[(int)battleSide.GetOppositeSide()] / num;
			int num3 = PartyBase.MainParty.NumberOfRegularMembers;
			if (MobileParty.MainParty.Army != null)
			{
				foreach (MobileParty mobileParty in MobileParty.MainParty.Army.LeaderParty.AttachedParties)
				{
					num3 += mobileParty.Party.NumberOfRegularMembers;
				}
			}
			int num4 = mapEventSide.CountTroops((FlattenedTroopRosterElement x) => x.State == RosterTroopState.Active && !x.Troop.IsHero);
			ExplainedNumber explainedNumber = new ExplainedNumber(1f, false, null);
			SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Tactics, DefaultSkillEffects.TacticsTroopSacrificeReduction, CharacterObject.PlayerCharacter, ref explainedNumber, -1, false, 0);
			float num5 = (float)num3 * MathF.Pow(MathF.Min(num2, 3f), 1.3f) * 0.1f + 5f;
			ExplainedNumber explainedNumber2 = new ExplainedNumber((float)MathF.Max(MathF.Round(num5 * explainedNumber.ResultNumber), 1), false, null);
			if (MobileParty.MainParty.HasPerk(DefaultPerks.Tactics.SwiftRegroup, true))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.SwiftRegroup, MobileParty.MainParty, false, ref explainedNumber2);
			}
			if (explainedNumber2.ResultNumber <= (float)num4)
			{
				return MathF.Round(explainedNumber2.ResultNumber);
			}
			return -1;
		}

		private int GetLostTroopCount(MobileParty party, SiegeEvent siegeEvent)
		{
			int num = 5;
			ExplainedNumber explainedNumber = new ExplainedNumber(1f, false, null);
			SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Tactics, DefaultSkillEffects.TacticsTroopSacrificeReduction, CharacterObject.PlayerCharacter, ref explainedNumber, -1, true, 0);
			float num2 = explainedNumber.ResultNumber - 1f;
			float num3 = 0f;
			foreach (PartyBase partyBase in siegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege))
			{
				num3 += partyBase.TotalStrength;
			}
			float num4;
			int num5;
			if (party.Army != null)
			{
				num4 = party.Army.LeaderParty.Party.TotalStrength;
				foreach (MobileParty mobileParty in party.Army.LeaderParty.AttachedParties)
				{
					num4 += mobileParty.Party.TotalStrength;
				}
				num5 = party.Army.TotalRegularCount;
			}
			else
			{
				num4 = party.Party.TotalStrength;
				num5 = party.MemberRoster.TotalRegulars;
			}
			float num6 = MathF.Clamp(0.12f * MathF.Pow((num3 + 1f) / (num4 + 1f), 0.25f), 0.12f, 0.24f);
			ExplainedNumber explainedNumber2 = new ExplainedNumber((float)(num + (int)(num6 * MathF.Max(0f, 1f - num2) * (float)num5)), false, null);
			if (MobileParty.MainParty.HasPerk(DefaultPerks.Tactics.Improviser, true))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.Improviser, MobileParty.MainParty, false, ref explainedNumber2);
			}
			return MathF.Round(explainedNumber2.ResultNumber);
		}
	}
}
