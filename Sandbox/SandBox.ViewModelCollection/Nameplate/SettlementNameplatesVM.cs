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
	public class SettlementNameplatesVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Nameplates.ApplyActionOnAllItems(delegate(SettlementNameplateVM x)
			{
				x.RefreshValues();
			});
		}

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

		public void Update()
		{
			this._cachedCameraPosition = this._mapCamera.Position;
			TWParallel.For(0, this.Nameplates.Count, this.UpdateNameplateAuxMTPredicate, 16);
			for (int i = 0; i < this.Nameplates.Count; i++)
			{
				this.Nameplates[i].RefreshBindValues();
			}
		}

		private void OnSiegeEventStartedOnSettlement(SiegeEvent siegeEvent)
		{
			SettlementNameplateVM settlementNameplateVM = this.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == siegeEvent.BesiegedSettlement);
			if (settlementNameplateVM == null)
			{
				return;
			}
			settlementNameplateVM.OnSiegeEventStartedOnSettlement(siegeEvent);
		}

		private void OnSiegeEventEndedOnSettlement(SiegeEvent siegeEvent)
		{
			SettlementNameplateVM settlementNameplateVM = this.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == siegeEvent.BesiegedSettlement);
			if (settlementNameplateVM == null)
			{
				return;
			}
			settlementNameplateVM.OnSiegeEventEndedOnSettlement(siegeEvent);
		}

		private void OnMapEventStartedOnSettlement(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
		{
			SettlementNameplateVM settlementNameplateVM = this.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == mapEvent.MapEventSettlement);
			if (settlementNameplateVM == null)
			{
				return;
			}
			settlementNameplateVM.OnMapEventStartedOnSettlement(mapEvent);
		}

		private void OnMapEventEndedOnSettlement(MapEvent mapEvent)
		{
			SettlementNameplateVM settlementNameplateVM = this.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == mapEvent.MapEventSettlement);
			if (settlementNameplateVM == null)
			{
				return;
			}
			settlementNameplateVM.OnMapEventEndedOnSettlement();
		}

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

		private void OnPeaceDeclared(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
		{
			this.OnPeaceOrWarDeclared(faction1, faction2);
		}

		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail arg3)
		{
			this.OnPeaceOrWarDeclared(faction1, faction2);
		}

		private void OnPeaceOrWarDeclared(IFaction faction1, IFaction faction2)
		{
			if (faction1 == Hero.MainHero.MapFaction || faction1 == Hero.MainHero.Clan || faction2 == Hero.MainHero.MapFaction || faction2 == Hero.MainHero.Clan)
			{
				this.RefreshRelationsOfNameplates();
			}
		}

		private void OnClanChangeKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			this.RefreshRelationsOfNameplates();
		}

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

		private void OnRebelliousClanDisbandedAtSettlement(Settlement settlement, Clan clan)
		{
			SettlementNameplateVM settlementNameplateVM = this.Nameplates.FirstOrDefault((SettlementNameplateVM n) => n.Settlement == settlement);
			if (settlementNameplateVM == null)
			{
				return;
			}
			settlementNameplateVM.OnRebelliousClanDisbanded(clan);
		}

		private void RefreshRelationsOfNameplates()
		{
			foreach (SettlementNameplateVM settlementNameplateVM in this.Nameplates)
			{
				settlementNameplateVM.RefreshRelationStatus();
			}
		}

		private void RefreshDynamicPropertiesOfNameplates()
		{
			foreach (SettlementNameplateVM settlementNameplateVM in this.Nameplates)
			{
				settlementNameplateVM.RefreshDynamicProperties(false);
			}
		}

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

		private readonly Camera _mapCamera;

		private Vec3 _cachedCameraPosition;

		private readonly TWParallel.ParallelForAuxPredicate UpdateNameplateAuxMTPredicate;

		private readonly Action<Vec2> _fastMoveCameraToPosition;

		private IEnumerable<Tuple<Settlement, GameEntity>> _allHideouts;

		private IEnumerable<Tuple<Settlement, GameEntity>> _allRetreats;

		private MBBindingList<SettlementNameplateVM> _nameplates;
	}
}
