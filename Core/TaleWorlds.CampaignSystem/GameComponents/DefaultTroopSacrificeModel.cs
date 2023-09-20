﻿using System;
using System.Collections.Generic;
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
	// Token: 0x02000147 RID: 327
	public class DefaultTroopSacrificeModel : TroopSacrificeModel
	{
		// Token: 0x060017F9 RID: 6137 RVA: 0x00078FC7 File Offset: 0x000771C7
		public override int GetLostTroopCountForBreakingInBesiegedSettlement(MobileParty party, SiegeEvent siegeEvent)
		{
			return this.GetLostTroopCount(party, siegeEvent);
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x00078FD1 File Offset: 0x000771D1
		public override int GetLostTroopCountForBreakingOutOfBesiegedSettlement(MobileParty party, SiegeEvent siegeEvent)
		{
			return this.GetLostTroopCount(party, siegeEvent);
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x00078FDC File Offset: 0x000771DC
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

		// Token: 0x060017FC RID: 6140 RVA: 0x0007915C File Offset: 0x0007735C
		private int GetLostTroopCount(MobileParty party, SiegeEvent siegeEvent)
		{
			int num = 5;
			ExplainedNumber explainedNumber = new ExplainedNumber(1f, false, null);
			SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Tactics, DefaultSkillEffects.TacticsTroopSacrificeReduction, CharacterObject.PlayerCharacter, ref explainedNumber, -1, true, 0);
			float num2 = explainedNumber.ResultNumber - 1f;
			MobileParty besiegerParty = siegeEvent.BesiegerCamp.BesiegerParty;
			float num3;
			if (besiegerParty.Army != null)
			{
				num3 = besiegerParty.Army.LeaderParty.Party.TotalStrength;
				using (List<MobileParty>.Enumerator enumerator = besiegerParty.Army.LeaderParty.AttachedParties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MobileParty mobileParty = enumerator.Current;
						num3 += mobileParty.Party.TotalStrength;
					}
					goto IL_C0;
				}
			}
			num3 = besiegerParty.Party.TotalStrength;
			IL_C0:
			float num4;
			int num5;
			if (party.Army != null)
			{
				num4 = party.Army.LeaderParty.Party.TotalStrength;
				foreach (MobileParty mobileParty2 in party.Army.LeaderParty.AttachedParties)
				{
					num4 += mobileParty2.Party.TotalStrength;
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
