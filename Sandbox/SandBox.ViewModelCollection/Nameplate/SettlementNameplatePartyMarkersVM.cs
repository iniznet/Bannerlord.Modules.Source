using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate
{
	public class SettlementNameplatePartyMarkersVM : ViewModel
	{
		public SettlementNameplatePartyMarkersVM(Settlement settlement)
		{
			this._settlement = settlement;
			this.PartiesInSettlement = new MBBindingList<SettlementNameplatePartyMarkerItemVM>();
			this._itemComparer = new SettlementNameplatePartyMarkersVM.PartyMarkerItemComparer();
		}

		private void PopulatePartyList()
		{
			this.PartiesInSettlement.Clear();
			foreach (MobileParty mobileParty in this._settlement.Parties.Where((MobileParty p) => this.IsMobilePartyValid(p)))
			{
				this.PartiesInSettlement.Add(new SettlementNameplatePartyMarkerItemVM(mobileParty));
			}
			this.PartiesInSettlement.Sort(this._itemComparer);
		}

		private bool IsMobilePartyValid(MobileParty party)
		{
			if (party.IsGarrison || party.IsMilitia)
			{
				return false;
			}
			if (party.IsMainParty && (!party.IsMainParty || Campaign.Current.IsMainHeroDisguised))
			{
				return false;
			}
			if (party.Army != null)
			{
				Army army = party.Army;
				return army != null && army.LeaderParty.IsMainParty && !Campaign.Current.IsMainHeroDisguised;
			}
			return true;
		}

		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (settlement == this._settlement)
			{
				SettlementNameplatePartyMarkerItemVM settlementNameplatePartyMarkerItemVM = this.PartiesInSettlement.SingleOrDefault((SettlementNameplatePartyMarkerItemVM p) => p.Party == party);
				if (settlementNameplatePartyMarkerItemVM != null)
				{
					this.PartiesInSettlement.Remove(settlementNameplatePartyMarkerItemVM);
				}
			}
		}

		private void OnSettlementEntered(MobileParty partyEnteredSettlement, Settlement settlement, Hero leader)
		{
			if (settlement == this._settlement && partyEnteredSettlement != null && this.PartiesInSettlement.SingleOrDefault((SettlementNameplatePartyMarkerItemVM p) => p.Party == partyEnteredSettlement) == null && this.IsMobilePartyValid(partyEnteredSettlement))
			{
				this.PartiesInSettlement.Add(new SettlementNameplatePartyMarkerItemVM(partyEnteredSettlement));
				this.PartiesInSettlement.Sort(this._itemComparer);
			}
		}

		private void OnMapEventEnded(MapEvent obj)
		{
			if (obj.MapEventSettlement != null && obj.MapEventSettlement == this._settlement)
			{
				this.PopulatePartyList();
			}
		}

		public void RegisterEvents()
		{
			if (!this._eventsRegistered)
			{
				this.PopulatePartyList();
				CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
				CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
				CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
				this._eventsRegistered = true;
			}
		}

		public void UnloadEvents()
		{
			if (this._eventsRegistered)
			{
				CampaignEvents.SettlementEntered.ClearListeners(this);
				CampaignEvents.OnSettlementLeftEvent.ClearListeners(this);
				CampaignEvents.MapEventEnded.ClearListeners(this);
				this.PartiesInSettlement.Clear();
				this._eventsRegistered = false;
			}
		}

		public MBBindingList<SettlementNameplatePartyMarkerItemVM> PartiesInSettlement
		{
			get
			{
				return this._partiesInSettlement;
			}
			set
			{
				if (value != this._partiesInSettlement)
				{
					this._partiesInSettlement = value;
					base.OnPropertyChangedWithValue<MBBindingList<SettlementNameplatePartyMarkerItemVM>>(value, "PartiesInSettlement");
				}
			}
		}

		private Settlement _settlement;

		private bool _eventsRegistered;

		private SettlementNameplatePartyMarkersVM.PartyMarkerItemComparer _itemComparer;

		private MBBindingList<SettlementNameplatePartyMarkerItemVM> _partiesInSettlement;

		public class PartyMarkerItemComparer : IComparer<SettlementNameplatePartyMarkerItemVM>
		{
			public int Compare(SettlementNameplatePartyMarkerItemVM x, SettlementNameplatePartyMarkerItemVM y)
			{
				return x.SortIndex.CompareTo(y.SortIndex);
			}
		}
	}
}
