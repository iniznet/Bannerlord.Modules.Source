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
	// Token: 0x02000019 RID: 25
	public class SettlementNameplatePartyMarkersVM : ViewModel
	{
		// Token: 0x06000252 RID: 594 RVA: 0x0000B8F9 File Offset: 0x00009AF9
		public SettlementNameplatePartyMarkersVM(Settlement settlement)
		{
			this._settlement = settlement;
			this.PartiesInSettlement = new MBBindingList<SettlementNameplatePartyMarkerItemVM>();
			this._itemComparer = new SettlementNameplatePartyMarkersVM.PartyMarkerItemComparer();
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000B920 File Offset: 0x00009B20
		private void PopulatePartyList()
		{
			this.PartiesInSettlement.Clear();
			foreach (MobileParty mobileParty in this._settlement.Parties.Where((MobileParty p) => this.IsMobilePartyValid(p)))
			{
				this.PartiesInSettlement.Add(new SettlementNameplatePartyMarkerItemVM(mobileParty));
			}
			this.PartiesInSettlement.Sort(this._itemComparer);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000B9AC File Offset: 0x00009BAC
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

		// Token: 0x06000255 RID: 597 RVA: 0x0000BA1C File Offset: 0x00009C1C
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

		// Token: 0x06000256 RID: 598 RVA: 0x0000BA68 File Offset: 0x00009C68
		private void OnSettlementEntered(MobileParty partyEnteredSettlement, Settlement settlement, Hero leader)
		{
			if (settlement == this._settlement && partyEnteredSettlement != null && this.PartiesInSettlement.SingleOrDefault((SettlementNameplatePartyMarkerItemVM p) => p.Party == partyEnteredSettlement) == null && this.IsMobilePartyValid(partyEnteredSettlement))
			{
				this.PartiesInSettlement.Add(new SettlementNameplatePartyMarkerItemVM(partyEnteredSettlement));
				this.PartiesInSettlement.Sort(this._itemComparer);
			}
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000BAE1 File Offset: 0x00009CE1
		private void OnMapEventEnded(MapEvent obj)
		{
			if (obj.MapEventSettlement != null && obj.MapEventSettlement == this._settlement)
			{
				this.PopulatePartyList();
			}
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000BB00 File Offset: 0x00009D00
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

		// Token: 0x06000259 RID: 601 RVA: 0x0000BB67 File Offset: 0x00009D67
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

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x0600025A RID: 602 RVA: 0x0000BBA4 File Offset: 0x00009DA4
		// (set) Token: 0x0600025B RID: 603 RVA: 0x0000BBAC File Offset: 0x00009DAC
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

		// Token: 0x0400011B RID: 283
		private Settlement _settlement;

		// Token: 0x0400011C RID: 284
		private bool _eventsRegistered;

		// Token: 0x0400011D RID: 285
		private SettlementNameplatePartyMarkersVM.PartyMarkerItemComparer _itemComparer;

		// Token: 0x0400011E RID: 286
		private MBBindingList<SettlementNameplatePartyMarkerItemVM> _partiesInSettlement;

		// Token: 0x02000067 RID: 103
		public class PartyMarkerItemComparer : IComparer<SettlementNameplatePartyMarkerItemVM>
		{
			// Token: 0x060004FA RID: 1274 RVA: 0x0001408C File Offset: 0x0001228C
			public int Compare(SettlementNameplatePartyMarkerItemVM x, SettlementNameplatePartyMarkerItemVM y)
			{
				return x.SortIndex.CompareTo(y.SortIndex);
			}
		}
	}
}
