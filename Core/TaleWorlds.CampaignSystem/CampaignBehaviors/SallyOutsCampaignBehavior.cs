using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003D0 RID: 976
	public class SallyOutsCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003ADF RID: 15071 RVA: 0x00113849 File Offset: 0x00111A49
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.HourlyTickSettlement));
			CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
		}

		// Token: 0x06003AE0 RID: 15072 RVA: 0x00113879 File Offset: 0x00111A79
		private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
		{
			if (defenderParty.SiegeEvent != null)
			{
				this.CheckForSettlementSallyOut(defenderParty.SiegeEvent.BesiegedSettlement, false);
			}
		}

		// Token: 0x06003AE1 RID: 15073 RVA: 0x00113895 File Offset: 0x00111A95
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003AE2 RID: 15074 RVA: 0x00113897 File Offset: 0x00111A97
		public void HourlyTickSettlement(Settlement settlement)
		{
			this.CheckForSettlementSallyOut(settlement, false);
		}

		// Token: 0x06003AE3 RID: 15075 RVA: 0x001138A4 File Offset: 0x00111AA4
		private void CheckForSettlementSallyOut(Settlement settlement, bool forceForCheck = false)
		{
			if (settlement.IsFortification && settlement.SiegeEvent != null && settlement.Party.MapEvent == null && settlement.Town.GarrisonParty != null && settlement.Town.GarrisonParty.MapEvent == null)
			{
				bool flag = settlement.SiegeEvent.BesiegerCamp.BesiegerParty.MapEvent != null && settlement.SiegeEvent.BesiegerCamp.BesiegerParty.MapEvent.IsSiegeOutside;
				if ((flag || MathF.Floor(CampaignTime.Now.ToHours) % 4 == 0) && (Hero.MainHero.CurrentSettlement != settlement || Campaign.Current.Models.EncounterModel.GetLeaderOfSiegeEvent(settlement.SiegeEvent, BattleSideEnum.Defender) != Hero.MainHero))
				{
					MobileParty besiegerParty = settlement.SiegeEvent.BesiegerCamp.BesiegerParty;
					float num = 0f;
					float num2 = 0f;
					float num3 = settlement.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.SallyOut).Sum((PartyBase x) => x.TotalStrength);
					LocatableSearchData<MobileParty> locatableSearchData = MobileParty.StartFindingLocatablesAroundPosition(settlement.SiegeEvent.BesiegerCamp.BesiegerParty.Position2D, 3f);
					for (MobileParty mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData); mobileParty != null; mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData))
					{
						if (mobileParty.CurrentSettlement == null && mobileParty.Aggressiveness > 0f)
						{
							float num4 = ((mobileParty.Aggressiveness > 0.5f) ? 1f : (mobileParty.Aggressiveness * 2f));
							if (mobileParty.MapFaction.IsAtWarWith(settlement.Party.MapFaction))
							{
								num += num4 * mobileParty.Party.TotalStrength;
							}
							else if (mobileParty.MapFaction == settlement.MapFaction)
							{
								num2 += num4 * mobileParty.Party.TotalStrength;
							}
						}
					}
					float num5 = num3 + num2;
					float num6 = (flag ? 1.5f : 2f);
					if (num5 > num * num6)
					{
						if (flag)
						{
							using (IEnumerator<PartyBase> enumerator = settlement.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.SallyOut).GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									PartyBase partyBase = enumerator.Current;
									if (partyBase.IsMobile && !partyBase.MobileParty.IsMainParty && partyBase.MapEventSide == null)
									{
										partyBase.MapEventSide = settlement.SiegeEvent.BesiegerCamp.BesiegerParty.MapEvent.AttackerSide;
									}
								}
								return;
							}
						}
						EncounterManager.StartPartyEncounter(settlement.Town.GarrisonParty.Party, besiegerParty.Party);
					}
				}
			}
		}

		// Token: 0x0400120D RID: 4621
		private const int SallyOutCheckPeriodInHours = 4;

		// Token: 0x0400120E RID: 4622
		private const float SallyOutPowerRatioForHelpingReliefForce = 1.5f;

		// Token: 0x0400120F RID: 4623
		private const float SallyOutPowerRatio = 2f;
	}
}
