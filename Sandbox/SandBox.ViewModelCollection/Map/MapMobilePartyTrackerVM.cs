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
	// Token: 0x0200002D RID: 45
	public class MapMobilePartyTrackerVM : ViewModel
	{
		// Token: 0x1700010A RID: 266
		// (get) Token: 0x0600034B RID: 843 RVA: 0x0000FED9 File Offset: 0x0000E0D9
		// (set) Token: 0x0600034C RID: 844 RVA: 0x0000FEE1 File Offset: 0x0000E0E1
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

		// Token: 0x0600034D RID: 845 RVA: 0x0000FF00 File Offset: 0x0000E100
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

		// Token: 0x0600034E RID: 846 RVA: 0x0000FFE8 File Offset: 0x0000E1E8
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

		// Token: 0x0600034F RID: 847 RVA: 0x000101F4 File Offset: 0x0000E3F4
		private void UpdateTrackerPropertiesAux(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this.Trackers[i].UpdateProperties();
				this.Trackers[i].UpdatePosition();
			}
		}

		// Token: 0x06000350 RID: 848 RVA: 0x00010230 File Offset: 0x0000E430
		public void Update()
		{
			TWParallel.For(0, this.Trackers.Count, this.UpdateTrackerPropertiesAuxPredicate, 16);
			this.Trackers.ApplyActionOnAllItems(delegate(MobilePartyTrackItemVM t)
			{
				t.RefreshBinding();
			});
		}

		// Token: 0x06000351 RID: 849 RVA: 0x00010280 File Offset: 0x0000E480
		public void UpdateProperties()
		{
			this.Trackers.ApplyActionOnAllItems(delegate(MobilePartyTrackItemVM t)
			{
				t.UpdateProperties();
			});
		}

		// Token: 0x06000352 RID: 850 RVA: 0x000102AC File Offset: 0x0000E4AC
		private bool CanAddParty(MobileParty party)
		{
			return party != null && !party.IsMainParty && !party.IsMilitia && !party.IsGarrison && !party.IsVillager && !party.IsBandit && !party.IsBanditBossParty && !party.IsCurrentlyUsedByAQuest && (!party.IsCaravan || party.CaravanPartyComponent.Owner == Hero.MainHero);
		}

		// Token: 0x06000353 RID: 851 RVA: 0x00010314 File Offset: 0x0000E514
		private void AddIfNotAdded(Army army)
		{
			if (this.Trackers.FirstOrDefault((MobilePartyTrackItemVM t) => t.TrackedArmy == army) == null)
			{
				this.Trackers.Add(new MobilePartyTrackItemVM(army, this._mapCamera, this._fastMoveCameraToPosition));
			}
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0001036C File Offset: 0x0000E56C
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

		// Token: 0x06000355 RID: 853 RVA: 0x000103C4 File Offset: 0x0000E5C4
		private void RemoveIfExists(Army army)
		{
			MobilePartyTrackItemVM mobilePartyTrackItemVM = this.Trackers.FirstOrDefault((MobilePartyTrackItemVM t) => t.TrackedArmy == army);
			if (mobilePartyTrackItemVM != null)
			{
				this.Trackers.Remove(mobilePartyTrackItemVM);
			}
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00010408 File Offset: 0x0000E608
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

		// Token: 0x06000357 RID: 855 RVA: 0x0001044C File Offset: 0x0000E64C
		private void OnPartyDestroyed(MobileParty mobileParty, PartyBase arg2)
		{
			this.RemoveIfExists(mobileParty);
		}

		// Token: 0x06000358 RID: 856 RVA: 0x00010455 File Offset: 0x0000E655
		private void OnPartyDisbanded(MobileParty disbandedParty, Settlement relatedSettlement)
		{
			this.RemoveIfExists(disbandedParty);
		}

		// Token: 0x06000359 RID: 857 RVA: 0x0001045E File Offset: 0x0000E65E
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

		// Token: 0x0600035A RID: 858 RVA: 0x00010497 File Offset: 0x0000E697
		private void OnArmyDispersed(Army army, Army.ArmyDispersionReason arg2, bool arg3)
		{
			if (army.MapFaction == Hero.MainHero.MapFaction)
			{
				this.RemoveIfExists(army);
			}
		}

		// Token: 0x0600035B RID: 859 RVA: 0x000104B2 File Offset: 0x0000E6B2
		private void OnArmyCreated(Army army)
		{
			if (army.MapFaction == Hero.MainHero.MapFaction)
			{
				this.AddIfNotAdded(army);
			}
		}

		// Token: 0x0600035C RID: 860 RVA: 0x000104CD File Offset: 0x0000E6CD
		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			if (clan == Clan.PlayerClan)
			{
				this.InitList();
			}
		}

		// Token: 0x0600035D RID: 861 RVA: 0x000104DD File Offset: 0x0000E6DD
		private void OnCompanionClanCreated(Clan clan)
		{
			this.RemoveIfExists(clan.Leader.PartyBelongedTo);
		}

		// Token: 0x040001BC RID: 444
		private readonly Camera _mapCamera;

		// Token: 0x040001BD RID: 445
		private readonly Action<Vec2> _fastMoveCameraToPosition;

		// Token: 0x040001BE RID: 446
		private readonly TWParallel.ParallelForAuxPredicate UpdateTrackerPropertiesAuxPredicate;

		// Token: 0x040001BF RID: 447
		private MBBindingList<MobilePartyTrackItemVM> _trackers;
	}
}
