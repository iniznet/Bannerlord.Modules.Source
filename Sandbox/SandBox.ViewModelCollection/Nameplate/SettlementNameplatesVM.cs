using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate
{
	// Token: 0x0200001A RID: 26
	public class SettlementNameplatesVM : ViewModel
	{
		// Token: 0x0600025D RID: 605 RVA: 0x0000BBD4 File Offset: 0x00009DD4
		public SettlementNameplatesVM(Camera mapCamera, Action<Vec2> fastMoveCameraToPosition)
		{
			this.Nameplates = new MBBindingList<SettlementNameplateVM>();
			this._mapCamera = mapCamera;
			this._fastMoveCameraToPosition = fastMoveCameraToPosition;
			CampaignEvents.PartyVisibilityChangedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.OnPartyBaseVisibilityChange));
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnPeaceDeclared));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangeKingdom));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.OnSiegeEventStartedOnSettlement));
			CampaignEvents.OnSiegeEventEndedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.OnSiegeEventEndedOnSettlement));
			CampaignEvents.RebelliousClanDisbandedAtSettlement.AddNonSerializedListener(this, new Action<Settlement, Clan>(this.OnRebelliousClanDisbandedAtSettlement));
			this.UpdateNameplateAuxMTPredicate = new TWParallel.ParallelForAuxPredicate(this.UpdateNameplateAuxMT);
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000BCCA File Offset: 0x00009ECA
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Nameplates.ApplyActionOnAllItems(delegate(SettlementNameplateVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000BCFC File Offset: 0x00009EFC
		public void Initialize(IEnumerable<Tuple<Settlement, GameEntity>> settlements)
		{
			IEnumerable<Tuple<Settlement, GameEntity>> enumerable = settlements.Where((Tuple<Settlement, GameEntity> x) => !x.Item1.IsHideout && !(x.Item1.SettlementComponent is RetirementSettlementComponent));
			this._allHideouts = settlements.Where((Tuple<Settlement, GameEntity> x) => x.Item1.IsHideout && !(x.Item1.SettlementComponent is RetirementSettlementComponent));
			this._allRetreats = settlements.Where((Tuple<Settlement, GameEntity> x) => !x.Item1.IsHideout && x.Item1.SettlementComponent is RetirementSettlementComponent);
			foreach (Tuple<Settlement, GameEntity> tuple in enumerable)
			{
				SettlementNameplateVM settlementNameplateVM = new SettlementNameplateVM(tuple.Item1, tuple.Item2, this._mapCamera, this._fastMoveCameraToPosition);
				this.Nameplates.Add(settlementNameplateVM);
			}
			foreach (Tuple<Settlement, GameEntity> tuple2 in this._allHideouts)
			{
				if (tuple2.Item1.Hideout.IsSpotted)
				{
					SettlementNameplateVM settlementNameplateVM2 = new SettlementNameplateVM(tuple2.Item1, tuple2.Item2, this._mapCamera, this._fastMoveCameraToPosition);
					this.Nameplates.Add(settlementNameplateVM2);
				}
			}
			foreach (Tuple<Settlement, GameEntity> tuple3 in this._allRetreats)
			{
				RetirementSettlementComponent retirementSettlementComponent;
				if ((retirementSettlementComponent = tuple3.Item1.SettlementComponent as RetirementSettlementComponent) != null)
				{
					if (retirementSettlementComponent.IsSpotted)
					{
						SettlementNameplateVM settlementNameplateVM3 = new SettlementNameplateVM(tuple3.Item1, tuple3.Item2, this._mapCamera, this._fastMoveCameraToPosition);
						this.Nameplates.Add(settlementNameplateVM3);
					}
				}
				else
				{
					Debug.FailedAssert("A seetlement which is IsRetreat doesn't have a retirement component.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\Nameplate\\SettlementNameplatesVM.cs", "Initialize", 83);
				}
			}
			foreach (SettlementNameplateVM settlementNameplateVM4 in this.Nameplates)
			{
				Settlement settlement = settlementNameplateVM4.Settlement;
				if (((settlement != null) ? settlement.SiegeEvent : null) != null)
				{
					SettlementNameplateVM settlementNameplateVM5 = settlementNameplateVM4;
					Settlement settlement2 = settlementNameplateVM4.Settlement;
					settlementNameplateVM5.OnSiegeEventStartedOnSettlement((settlement2 != null) ? settlement2.SiegeEvent : null);
				}
				else if (settlementNameplateVM4.Settlement.IsTown || settlementNameplateVM4.Settlement.IsCastle)
				{
					Clan ownerClan = settlementNameplateVM4.Settlement.OwnerClan;
					if (ownerClan != null && ownerClan.IsRebelClan)
					{
						settlementNameplateVM4.OnRebelliousClanFormed(settlementNameplateVM4.Settlement.OwnerClan);
					}
				}
			}
			this.RefreshRelationsOfNameplates();
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000BFB0 File Offset: 0x0000A1B0
		private void UpdateNameplateAuxMT(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				SettlementNameplateVM settlementNameplateVM = this.Nameplates[i];
				settlementNameplateVM.CalculatePosition(this._cachedCameraPosition);
				settlementNameplateVM.DetermineIsInsideWindow();
				settlementNameplateVM.DetermineIsVisibleOnMap(this._cachedCameraPosition);
				settlementNameplateVM.RefreshPosition();
				settlementNameplateVM.RefreshDynamicProperties(false);
			}
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000C000 File Offset: 0x0000A200
		public void Update()
		{
			this._cachedCameraPosition = this._mapCamera.Position;
			TWParallel.For(0, this.Nameplates.Count, this.UpdateNameplateAuxMTPredicate, 16);
			for (int i = 0; i < this.Nameplates.Count; i++)
			{
				this.Nameplates[i].RefreshBindValues();
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000C060 File Offset: 0x0000A260
		private void OnSiegeEventStartedOnSettlement(SiegeEvent siegeEvent)
		{
			SettlementNameplateVM settlementNameplateVM = this.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == siegeEvent.BesiegedSettlement);
			if (settlementNameplateVM == null)
			{
				return;
			}
			settlementNameplateVM.OnSiegeEventStartedOnSettlement(siegeEvent);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000C0A4 File Offset: 0x0000A2A4
		private void OnSiegeEventEndedOnSettlement(SiegeEvent siegeEvent)
		{
			SettlementNameplateVM settlementNameplateVM = this.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == siegeEvent.BesiegedSettlement);
			if (settlementNameplateVM == null)
			{
				return;
			}
			settlementNameplateVM.OnSiegeEventEndedOnSettlement(siegeEvent);
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000C0E8 File Offset: 0x0000A2E8
		private void OnMapEventStartedOnSettlement(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
		{
			SettlementNameplateVM settlementNameplateVM = this.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == mapEvent.MapEventSettlement);
			if (settlementNameplateVM == null)
			{
				return;
			}
			settlementNameplateVM.OnMapEventStartedOnSettlement(mapEvent);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000C12C File Offset: 0x0000A32C
		private void OnMapEventEndedOnSettlement(MapEvent mapEvent)
		{
			SettlementNameplateVM settlementNameplateVM = this.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == mapEvent.MapEventSettlement);
			if (settlementNameplateVM == null)
			{
				return;
			}
			settlementNameplateVM.OnMapEventEndedOnSettlement();
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000C168 File Offset: 0x0000A368
		private void OnPartyBaseVisibilityChange(PartyBase party)
		{
			if (party.IsSettlement)
			{
				Tuple<Settlement, GameEntity> desiredSettlementTuple = null;
				if (party.Settlement.IsHideout)
				{
					desiredSettlementTuple = this._allHideouts.SingleOrDefault((Tuple<Settlement, GameEntity> h) => h.Item1.Hideout == party.Settlement.Hideout);
				}
				else if (party.Settlement.SettlementComponent is RetirementSettlementComponent)
				{
					desiredSettlementTuple = this._allRetreats.SingleOrDefault((Tuple<Settlement, GameEntity> h) => h.Item1.SettlementComponent as RetirementSettlementComponent == party.Settlement.SettlementComponent as RetirementSettlementComponent);
				}
				else
				{
					Debug.FailedAssert("We don't support hiding non retreat or non hideout settlements.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\Nameplate\\SettlementNameplatesVM.cs", "OnPartyBaseVisibilityChange", 180);
				}
				if (desiredSettlementTuple != null)
				{
					SettlementNameplateVM settlementNameplateVM = this.Nameplates.SingleOrDefault((SettlementNameplateVM n) => n.Settlement == desiredSettlementTuple.Item1);
					if (party.IsVisible && settlementNameplateVM == null)
					{
						SettlementNameplateVM settlementNameplateVM2 = new SettlementNameplateVM(desiredSettlementTuple.Item1, desiredSettlementTuple.Item2, this._mapCamera, this._fastMoveCameraToPosition);
						this.Nameplates.Add(settlementNameplateVM2);
						settlementNameplateVM2.RefreshRelationStatus();
						return;
					}
					if (!party.IsVisible && settlementNameplateVM != null)
					{
						this.Nameplates.Remove(settlementNameplateVM);
					}
				}
			}
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000C2AA File Offset: 0x0000A4AA
		private void OnPeaceDeclared(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
		{
			this.OnPeaceOrWarDeclared(faction1, faction2);
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000C2B4 File Offset: 0x0000A4B4
		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail arg3)
		{
			this.OnPeaceOrWarDeclared(faction1, faction2);
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000C2BE File Offset: 0x0000A4BE
		private void OnPeaceOrWarDeclared(IFaction faction1, IFaction faction2)
		{
			if (faction1 == Hero.MainHero.MapFaction || faction1 == Hero.MainHero.Clan || faction2 == Hero.MainHero.MapFaction || faction2 == Hero.MainHero.Clan)
			{
				this.RefreshRelationsOfNameplates();
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000C2FA File Offset: 0x0000A4FA
		private void OnClanChangeKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			this.RefreshRelationsOfNameplates();
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000C304 File Offset: 0x0000A504
		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero previousOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			SettlementNameplateVM settlementNameplateVM = this.Nameplates.SingleOrDefault((SettlementNameplateVM n) => n.Settlement == settlement);
			if (settlementNameplateVM != null)
			{
				settlementNameplateVM.RefreshDynamicProperties(true);
			}
			if (settlementNameplateVM != null)
			{
				settlementNameplateVM.RefreshRelationStatus();
			}
			using (List<Village>.Enumerator enumerator = settlement.BoundVillages.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Village village = enumerator.Current;
					SettlementNameplateVM settlementNameplateVM2 = this.Nameplates.SingleOrDefault((SettlementNameplateVM n) => n.Settlement.IsVillage && n.Settlement.Village == village);
					if (settlementNameplateVM2 != null)
					{
						settlementNameplateVM2.RefreshDynamicProperties(true);
					}
					if (settlementNameplateVM2 != null)
					{
						settlementNameplateVM2.RefreshRelationStatus();
					}
				}
			}
			if (detail != 7)
			{
				if (previousOwner != null && previousOwner.IsRebel)
				{
					SettlementNameplateVM settlementNameplateVM3 = this.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == settlement);
					if (settlementNameplateVM3 == null)
					{
						return;
					}
					settlementNameplateVM3.OnRebelliousClanDisbanded(previousOwner.Clan);
				}
				return;
			}
			SettlementNameplateVM settlementNameplateVM4 = this.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == settlement);
			if (settlementNameplateVM4 == null)
			{
				return;
			}
			settlementNameplateVM4.OnRebelliousClanFormed(newOwner.Clan);
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000C430 File Offset: 0x0000A630
		private void OnRebelliousClanDisbandedAtSettlement(Settlement settlement, Clan clan)
		{
			SettlementNameplateVM settlementNameplateVM = this.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == settlement);
			if (settlementNameplateVM == null)
			{
				return;
			}
			settlementNameplateVM.OnRebelliousClanDisbanded(clan);
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000C46C File Offset: 0x0000A66C
		private void RefreshRelationsOfNameplates()
		{
			foreach (SettlementNameplateVM settlementNameplateVM in this.Nameplates)
			{
				settlementNameplateVM.RefreshRelationStatus();
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000C4B8 File Offset: 0x0000A6B8
		private void RefreshDynamicPropertiesOfNameplates()
		{
			foreach (SettlementNameplateVM settlementNameplateVM in this.Nameplates)
			{
				settlementNameplateVM.RefreshDynamicProperties(false);
			}
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000C504 File Offset: 0x0000A704
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.PartyVisibilityChangedEvent.ClearListeners(this);
			CampaignEvents.WarDeclared.ClearListeners(this);
			CampaignEvents.MakePeace.ClearListeners(this);
			CampaignEvents.ClanChangedKingdom.ClearListeners(this);
			CampaignEvents.OnSettlementOwnerChangedEvent.ClearListeners(this);
			CampaignEvents.OnSiegeEventStartedEvent.ClearListeners(this);
			CampaignEvents.OnSiegeEventEndedEvent.ClearListeners(this);
			CampaignEvents.RebelliousClanDisbandedAtSettlement.ClearListeners(this);
			this.Nameplates.ApplyActionOnAllItems(delegate(SettlementNameplateVM n)
			{
				n.OnFinalize();
			});
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000270 RID: 624 RVA: 0x0000C599 File Offset: 0x0000A799
		// (set) Token: 0x06000271 RID: 625 RVA: 0x0000C5A1 File Offset: 0x0000A7A1
		[DataSourceProperty]
		public MBBindingList<SettlementNameplateVM> Nameplates
		{
			get
			{
				return this._nameplates;
			}
			set
			{
				if (this._nameplates != value)
				{
					this._nameplates = value;
					base.OnPropertyChangedWithValue<MBBindingList<SettlementNameplateVM>>(value, "Nameplates");
				}
			}
		}

		// Token: 0x0400011F RID: 287
		private readonly Camera _mapCamera;

		// Token: 0x04000120 RID: 288
		private Vec3 _cachedCameraPosition;

		// Token: 0x04000121 RID: 289
		private readonly TWParallel.ParallelForAuxPredicate UpdateNameplateAuxMTPredicate;

		// Token: 0x04000122 RID: 290
		private readonly Action<Vec2> _fastMoveCameraToPosition;

		// Token: 0x04000123 RID: 291
		private IEnumerable<Tuple<Settlement, GameEntity>> _allHideouts;

		// Token: 0x04000124 RID: 292
		private IEnumerable<Tuple<Settlement, GameEntity>> _allRetreats;

		// Token: 0x04000125 RID: 293
		private MBBindingList<SettlementNameplateVM> _nameplates;
	}
}
