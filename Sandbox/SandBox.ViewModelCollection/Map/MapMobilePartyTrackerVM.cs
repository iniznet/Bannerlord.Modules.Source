using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Map
{
	public class MapMobilePartyTrackerVM : ViewModel
	{
		public MBBindingList<MobilePartyTrackItemVM> Trackers
		{
			get
			{
				return this._trackers;
			}
			set
			{
				if (value != this._trackers)
				{
					this._trackers = value;
					base.OnPropertyChangedWithValue<MBBindingList<MobilePartyTrackItemVM>>(value, "Trackers");
				}
			}
		}

		public MapMobilePartyTrackerVM(Camera mapCamera, Action<Vec2> fastMoveCameraToPosition)
		{
			this._mapCamera = mapCamera;
			this._fastMoveCameraToPosition = fastMoveCameraToPosition;
			this.UpdateTrackerPropertiesAuxPredicate = new TWParallel.ParallelForAuxPredicate(this.UpdateTrackerPropertiesAux);
			this.Trackers = new MBBindingList<MobilePartyTrackItemVM>();
			this.InitList();
			CampaignEvents.ArmyCreated.AddNonSerializedListener(this, new Action<Army>(this.OnArmyCreated));
			CampaignEvents.ArmyDispersed.AddNonSerializedListener(this, new Action<Army, Army.ArmyDispersionReason, bool>(this.OnArmyDispersed));
			CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, new Action<MobileParty>(this.OnMobilePartyCreated));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnPartyDestroyed));
			CampaignEvents.OnPartyDisbandedEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnPartyDisbanded));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			CampaignEvents.OnCompanionClanCreatedEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnCompanionClanCreated));
		}

		private void InitList()
		{
			this.Trackers.Clear();
			foreach (WarPartyComponent warPartyComponent in Clan.PlayerClan.WarPartyComponents)
			{
				if (this.CanAddParty(warPartyComponent.MobileParty))
				{
					this.Trackers.Add(new MobilePartyTrackItemVM(warPartyComponent.MobileParty, this._mapCamera, this._fastMoveCameraToPosition));
				}
			}
			foreach (CaravanPartyComponent caravanPartyComponent in Clan.PlayerClan.Heroes.SelectMany((Hero h) => h.OwnedCaravans))
			{
				if (this.CanAddParty(caravanPartyComponent.MobileParty))
				{
					this.Trackers.Add(new MobilePartyTrackItemVM(caravanPartyComponent.MobileParty, this._mapCamera, this._fastMoveCameraToPosition));
				}
			}
			if (Clan.PlayerClan.Kingdom != null)
			{
				foreach (Army army in Clan.PlayerClan.Kingdom.Armies)
				{
					this.Trackers.Add(new MobilePartyTrackItemVM(army, this._mapCamera, this._fastMoveCameraToPosition));
				}
			}
			using (List<TrackedObject>.Enumerator enumerator4 = Campaign.Current.VisualTrackerManager.TrackedObjects.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					MobileParty mobileParty;
					if ((mobileParty = enumerator4.Current.Object as MobileParty) != null && mobileParty.LeaderHero == null && mobileParty.IsCurrentlyUsedByAQuest)
					{
						this.Trackers.Add(new MobilePartyTrackItemVM(mobileParty, this._mapCamera, this._fastMoveCameraToPosition));
					}
				}
			}
		}

		private void UpdateTrackerPropertiesAux(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this.Trackers[i].UpdateProperties();
				this.Trackers[i].UpdatePosition();
			}
		}

		public void Update()
		{
			TWParallel.For(0, this.Trackers.Count, this.UpdateTrackerPropertiesAuxPredicate, 16);
			this.Trackers.ApplyActionOnAllItems(delegate(MobilePartyTrackItemVM t)
			{
				t.RefreshBinding();
			});
		}

		public void UpdateProperties()
		{
			this.Trackers.ApplyActionOnAllItems(delegate(MobilePartyTrackItemVM t)
			{
				t.UpdateProperties();
			});
		}

		private bool CanAddParty(MobileParty party)
		{
			return party != null && !party.IsMainParty && !party.IsMilitia && !party.IsGarrison && !party.IsVillager && !party.IsBandit && !party.IsBanditBossParty && !party.IsCurrentlyUsedByAQuest && (!party.IsCaravan || party.CaravanPartyComponent.Owner == Hero.MainHero);
		}

		private void AddIfNotAdded(Army army)
		{
			if (this.Trackers.FirstOrDefault((MobilePartyTrackItemVM t) => t.TrackedArmy == army) == null)
			{
				this.Trackers.Add(new MobilePartyTrackItemVM(army, this._mapCamera, this._fastMoveCameraToPosition));
			}
		}

		private void AddIfNotAdded(MobileParty party)
		{
			for (int i = 0; i < this.Trackers.Count; i++)
			{
				if (this.Trackers[i].TrackedParty == party)
				{
					return;
				}
			}
			this.Trackers.Add(new MobilePartyTrackItemVM(party, this._mapCamera, this._fastMoveCameraToPosition));
		}

		private void RemoveIfExists(Army army)
		{
			MobilePartyTrackItemVM mobilePartyTrackItemVM = this.Trackers.FirstOrDefault((MobilePartyTrackItemVM t) => t.TrackedArmy == army);
			if (mobilePartyTrackItemVM != null)
			{
				this.Trackers.Remove(mobilePartyTrackItemVM);
			}
		}

		private void RemoveIfExists(MobileParty party)
		{
			for (int i = 0; i < this.Trackers.Count; i++)
			{
				if (this.Trackers[i].TrackedParty == party)
				{
					this.Trackers.RemoveAt(i);
					return;
				}
			}
		}

		private void OnPartyDestroyed(MobileParty mobileParty, PartyBase arg2)
		{
			this.RemoveIfExists(mobileParty);
		}

		private void OnPartyDisbanded(MobileParty disbandedParty, Settlement relatedSettlement)
		{
			this.RemoveIfExists(disbandedParty);
		}

		private void OnMobilePartyCreated(MobileParty party)
		{
			if (party.IsLordParty)
			{
				if (Clan.PlayerClan.WarPartyComponents.Contains(party.WarPartyComponent))
				{
					this.AddIfNotAdded(party);
					return;
				}
			}
			else if (this.CanAddParty(party))
			{
				this.AddIfNotAdded(party);
			}
		}

		private void OnArmyDispersed(Army army, Army.ArmyDispersionReason arg2, bool arg3)
		{
			if (army.MapFaction == Hero.MainHero.MapFaction)
			{
				this.RemoveIfExists(army);
			}
		}

		private void OnArmyCreated(Army army)
		{
			if (army.MapFaction == Hero.MainHero.MapFaction)
			{
				this.AddIfNotAdded(army);
			}
		}

		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			if (clan == Clan.PlayerClan)
			{
				this.InitList();
			}
		}

		private void OnCompanionClanCreated(Clan clan)
		{
			this.RemoveIfExists(clan.Leader.PartyBelongedTo);
		}

		private readonly Camera _mapCamera;

		private readonly Action<Vec2> _fastMoveCameraToPosition;

		private readonly TWParallel.ParallelForAuxPredicate UpdateTrackerPropertiesAuxPredicate;

		private MBBindingList<MobilePartyTrackItemVM> _trackers;
	}
}
