using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003B7 RID: 951
	public class PartiesSellPrisonerCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600389E RID: 14494 RVA: 0x00101E8E File Offset: 0x0010008E
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
		}

		// Token: 0x0600389F RID: 14495 RVA: 0x00101EBE File Offset: 0x001000BE
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060038A0 RID: 14496 RVA: 0x00101EC0 File Offset: 0x001000C0
		private void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (Campaign.Current.GameStarted && settlement.IsFortification && mobileParty != null && mobileParty.MapFaction != null && !mobileParty.IsMainParty && !mobileParty.IsDisbanding && (mobileParty.IsLordParty || mobileParty.IsCaravan) && !mobileParty.MapFaction.IsAtWarWith(settlement.MapFaction) && mobileParty.PrisonRoster.Count > 0)
			{
				if (mobileParty.MapFaction.IsKingdomFaction && mobileParty.ActualClan != null)
				{
					FlattenedTroopRoster flattenedTroopRoster = new FlattenedTroopRoster(4);
					TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
					foreach (TroopRosterElement troopRosterElement in mobileParty.PrisonRoster.GetTroopRoster())
					{
						if (troopRosterElement.Number == 0)
						{
							Debug.FailedAssert(string.Format("{0} number is 0 in prison roster!", troopRosterElement.Character.Name), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CampaignBehaviors\\PartiesSellPrisonerCampaignBehavior.cs", "OnSettlementEntered", 45);
						}
						else if (!troopRosterElement.Character.IsHero)
						{
							flattenedTroopRoster.Add(troopRosterElement);
							settlement.Party.PrisonRoster.Add(troopRosterElement);
							mobileParty.PrisonRoster.RemoveTroop(troopRosterElement.Character, troopRosterElement.Number, default(UniqueTroopDescriptor), 0);
						}
						else
						{
							flattenedTroopRoster.Add(troopRosterElement);
							troopRoster.Add(troopRosterElement);
						}
					}
					SellPrisonersAction.ApplyForSelectedPrisoners(mobileParty, troopRoster, settlement);
					CampaignEventDispatcher.Instance.OnPrisonerDonatedToSettlement(mobileParty, flattenedTroopRoster, settlement);
					return;
				}
				SellPrisonersAction.ApplyForAllPrisoners(mobileParty, mobileParty.PrisonRoster, settlement, true);
			}
		}

		// Token: 0x060038A1 RID: 14497 RVA: 0x00102068 File Offset: 0x00100268
		private void DailyTickSettlement(Settlement settlement)
		{
			if (settlement.IsFortification)
			{
				TroopRoster prisonRoster = settlement.Party.PrisonRoster;
				TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
				if (settlement.Owner != Hero.MainHero)
				{
					if (prisonRoster.TotalRegulars <= 0)
					{
						goto IL_1A6;
					}
					int num = (int)((float)prisonRoster.TotalRegulars * 0.1f);
					if (num <= 0)
					{
						goto IL_1A6;
					}
					using (List<TroopRosterElement>.Enumerator enumerator = prisonRoster.GetTroopRoster().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TroopRosterElement troopRosterElement = enumerator.Current;
							if (!troopRosterElement.Character.IsHero)
							{
								int num2 = ((num > troopRosterElement.Number) ? troopRosterElement.Number : num);
								num -= num2;
								troopRoster.AddToCounts(troopRosterElement.Character, num2, false, 0, 0, true, -1);
								if (num <= 0)
								{
									break;
								}
							}
						}
						goto IL_1A6;
					}
				}
				if (prisonRoster.TotalManCount > settlement.Party.PrisonerSizeLimit)
				{
					int num3 = prisonRoster.TotalManCount - settlement.Party.PrisonerSizeLimit;
					foreach (TroopRosterElement troopRosterElement2 in from t in prisonRoster.GetTroopRoster()
						orderby t.Character.Tier
						select t)
					{
						if (!troopRosterElement2.Character.IsHero)
						{
							if (num3 >= troopRosterElement2.Number)
							{
								num3 -= troopRosterElement2.Number;
								troopRoster.AddToCounts(troopRosterElement2.Character, troopRosterElement2.Number, false, 0, 0, true, -1);
							}
							else
							{
								troopRoster.AddToCounts(troopRosterElement2.Character, num3, false, 0, 0, true, -1);
								num3 = 0;
							}
							if (num3 <= 0)
							{
								break;
							}
						}
					}
				}
				IL_1A6:
				if (troopRoster.TotalManCount > 0)
				{
					SellPrisonersAction.ApplyForSettlementPrisoners(settlement, troopRoster, true);
				}
			}
		}
	}
}
