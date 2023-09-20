using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000427 RID: 1063
	public static class BreakInOutBesiegedSettlementAction
	{
		// Token: 0x06003E90 RID: 16016 RVA: 0x0012AC28 File Offset: 0x00128E28
		public static void ApplyBreakIn(out TroopRoster casualties, out int armyCasualtiesCount)
		{
			BreakInOutBesiegedSettlementAction.ApplyInternal(true, out casualties, out armyCasualtiesCount);
		}

		// Token: 0x06003E91 RID: 16017 RVA: 0x0012AC32 File Offset: 0x00128E32
		public static void ApplyBreakOut(out TroopRoster casualties, out int armyCasualtiesCount)
		{
			BreakInOutBesiegedSettlementAction.ApplyInternal(false, out casualties, out armyCasualtiesCount);
		}

		// Token: 0x06003E92 RID: 16018 RVA: 0x0012AC3C File Offset: 0x00128E3C
		private static void ApplyInternal(bool breakIn, out TroopRoster casualties, out int armyCasualtiesCount)
		{
			casualties = TroopRoster.CreateDummyTroopRoster();
			armyCasualtiesCount = -1;
			MobileParty mainParty = MobileParty.MainParty;
			SiegeEvent siegeEvent = Settlement.CurrentSettlement.SiegeEvent;
			int num;
			if (breakIn)
			{
				num = Campaign.Current.Models.TroopSacrificeModel.GetLostTroopCountForBreakingInBesiegedSettlement(mainParty, siegeEvent);
			}
			else
			{
				num = Campaign.Current.Models.TroopSacrificeModel.GetLostTroopCountForBreakingOutOfBesiegedSettlement(mainParty, siegeEvent);
			}
			if (mainParty.Army == null)
			{
				TroopRoster memberRoster = mainParty.MemberRoster;
				for (int i = 0; i < num; i++)
				{
					int num2 = MBRandom.RandomInt(memberRoster.Count);
					CharacterObject characterAtIndex = memberRoster.GetCharacterAtIndex(num2);
					if (!characterAtIndex.IsRegular || memberRoster.GetElementNumber(num2) == 0)
					{
						i--;
					}
					else
					{
						memberRoster.AddToCountsAtIndex(num2, -1, 0, 0, true);
						casualties.AddToCounts(characterAtIndex, 1, false, 0, 0, true, -1);
					}
				}
				return;
			}
			armyCasualtiesCount = 0;
			Army army = mainParty.Army;
			int num3 = 0;
			foreach (MobileParty mobileParty in army.Parties)
			{
				num3 += mobileParty.MemberRoster.TotalManCount - mobileParty.MemberRoster.TotalHeroes;
			}
			for (int j = 0; j < num; j++)
			{
				float num4 = MBRandom.RandomFloat * (float)num3;
				foreach (MobileParty mobileParty2 in army.Parties)
				{
					num4 -= (float)(mobileParty2.MemberRoster.TotalManCount - mobileParty2.MemberRoster.TotalHeroes);
					if (num4 < 0f)
					{
						num4 += (float)(mobileParty2.MemberRoster.TotalManCount - mobileParty2.MemberRoster.TotalHeroes);
						int num5 = -1;
						for (int k = 0; k < mobileParty2.MemberRoster.Count; k++)
						{
							if (!mobileParty2.MemberRoster.GetCharacterAtIndex(k).IsHero)
							{
								num4 -= (float)(mobileParty2.MemberRoster.GetElementNumber(k) + mobileParty2.MemberRoster.GetElementWoundedNumber(k));
								if (num4 < 0f)
								{
									num5 = k;
									break;
								}
							}
						}
						if (num5 >= 0)
						{
							CharacterObject characterAtIndex2 = mobileParty2.MemberRoster.GetCharacterAtIndex(num5);
							mobileParty2.MemberRoster.AddToCountsAtIndex(num5, -1, 0, 0, true);
							num3--;
							if (mobileParty2 == MobileParty.MainParty)
							{
								casualties.AddToCounts(characterAtIndex2, 1, false, 0, 0, true, -1);
								break;
							}
							armyCasualtiesCount++;
							break;
						}
					}
				}
			}
		}
	}
}
