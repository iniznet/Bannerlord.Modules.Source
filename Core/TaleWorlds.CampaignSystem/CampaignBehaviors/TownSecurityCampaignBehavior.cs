using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class TownSecurityCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
			CampaignEvents.OnSiegeEventEndedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.SiegeEventEnded));
			CampaignEvents.OnHideoutDeactivatedEvent.AddNonSerializedListener(this, new Action<Settlement>(this.OnHideoutDeactivated));
		}

		private void OnHideoutDeactivated(Settlement hideout)
		{
			SettlementSecurityModel model = Campaign.Current.Models.SettlementSecurityModel;
			foreach (Settlement settlement in Settlement.All.Where((Settlement t) => t.IsTown && t.GatePosition.DistanceSquared(hideout.GatePosition) < model.HideoutClearedSecurityEffectRadius * model.HideoutClearedSecurityEffectRadius).ToList<Settlement>())
			{
				settlement.Town.Security += (float)model.HideoutClearedSecurityGain;
			}
		}

		private void MapEventEnded(MapEvent mapEvent)
		{
			if (mapEvent.IsFieldBattle && mapEvent.HasWinner)
			{
				SettlementSecurityModel model = Campaign.Current.Models.SettlementSecurityModel;
				using (List<Settlement>.Enumerator enumerator = Settlement.All.Where((Settlement t) => t.IsTown && t.GatePosition.DistanceSquared(mapEvent.Position) < model.MapEventSecurityEffectRadius * model.MapEventSecurityEffectRadius).ToList<Settlement>().GetEnumerator())
				{
					Func<PartyBase, bool> <>9__3;
					while (enumerator.MoveNext())
					{
						Settlement town = enumerator.Current;
						if (mapEvent.Winner.Parties.Any((MapEventParty party) => party.Party.IsMobile && party.Party.MobileParty.IsBandit) && mapEvent.InvolvedParties.Any((PartyBase party) => this.ValidCivilianPartyCondition(party, mapEvent, town.MapFaction)))
						{
							float num = mapEvent.StrengthOfSide[(int)mapEvent.DefeatedSide];
							town.Town.Security += model.GetLootedNearbyPartySecurityEffect(town.Town, num);
						}
						else
						{
							IEnumerable<PartyBase> involvedParties = mapEvent.InvolvedParties;
							Func<PartyBase, bool> func;
							if ((func = <>9__3) == null)
							{
								func = (<>9__3 = (PartyBase party) => this.ValidBanditPartyCondition(party, mapEvent));
							}
							if (involvedParties.Any(func))
							{
								float num2 = mapEvent.StrengthOfSide[(int)mapEvent.DefeatedSide];
								town.Town.Security += model.GetNearbyBanditPartyDefeatedSecurityEffect(town.Town, num2);
							}
						}
					}
				}
			}
		}

		private bool ValidCivilianPartyCondition(PartyBase party, MapEvent mapEvent, IFaction mapFaction)
		{
			return party.IsMobile && ((party.Side != mapEvent.WinningSide && party.MobileParty.IsVillager && FactionManager.IsAlliedWithFaction(party.MapFaction, mapFaction)) || (party.MobileParty.IsCaravan && !party.MapFaction.IsAtWarWith(mapFaction)));
		}

		private bool ValidBanditPartyCondition(PartyBase party, MapEvent mapEvent)
		{
			if (party.Side != mapEvent.WinningSide)
			{
				MobileParty mobileParty = party.MobileParty;
				return mobileParty != null && mobileParty.IsBandit;
			}
			return false;
		}

		private void SiegeEventEnded(SiegeEvent siegeEvent)
		{
		}

		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
