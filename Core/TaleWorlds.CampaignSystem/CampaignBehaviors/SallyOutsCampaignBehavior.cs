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
	public class SallyOutsCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.HourlyTickSettlement));
			CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
		}

		private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
		{
			if (defenderParty.SiegeEvent != null)
			{
				this.CheckForSettlementSallyOut(defenderParty.SiegeEvent.BesiegedSettlement, false);
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		public void HourlyTickSettlement(Settlement settlement)
		{
			this.CheckForSettlementSallyOut(settlement, false);
		}

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

		private const int SallyOutCheckPeriodInHours = 4;

		private const float SallyOutPowerRatioForHelpingReliefForce = 1.5f;

		private const float SallyOutPowerRatio = 2f;
	}
}
