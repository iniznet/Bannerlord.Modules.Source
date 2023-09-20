using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace SandBox.ViewModelCollection.Nameplate
{
	public class PartyNameplatesVM : ViewModel
	{
		public PartyNameplatesVM(Camera mapCamera, Action resetCamera, Func<bool> isShowPartyNamesEnabled)
		{
			this.Nameplates = new MBBindingList<PartyNameplateVM>();
			this._nameplateComparer = new PartyNameplatesVM.NameplateDistanceComparer();
			this._mapCamera = mapCamera;
			this._resetCamera = resetCamera;
			this._isShowPartyNamesEnabled = isShowPartyNamesEnabled;
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.PartyVisibilityChangedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.OnPartyVisibilityChanged));
			CampaignEvents.OnPlayerCharacterChangedEvent.AddNonSerializedListener(this, new Action<Hero, Hero, MobileParty, bool>(this.OnPlayerCharacterChangedEvent));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangeKingdom));
			CampaignEvents.OnGameOverEvent.AddNonSerializedListener(this, new Action(this.OnGameOver));
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Nameplates.ApplyActionOnAllItems(delegate(PartyNameplateVM x)
			{
				x.RefreshValues();
			});
		}

		public void Initialize()
		{
			foreach (MobileParty mobileParty in MobileParty.All.Where((MobileParty p) => p.IsSpotted() && p.CurrentSettlement == null))
			{
				PartyNameplateVM partyNameplateVM = new PartyNameplateVM(mobileParty, this._mapCamera, this._resetCamera, this._isShowPartyNamesEnabled);
				this.Nameplates.Add(partyNameplateVM);
			}
		}

		private void OnClanChangeKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			foreach (PartyNameplateVM partyNameplateVM in this.Nameplates.Where((PartyNameplateVM p) => p.Party.LeaderHero != null && p.Party.LeaderHero.Clan == clan))
			{
				partyNameplateVM.RefreshDynamicProperties(true);
			}
		}

		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			for (int i = 0; i < this.Nameplates.Count; i++)
			{
				PartyNameplateVM partyNameplateVM = this.Nameplates[i];
				if (partyNameplateVM.Party == party)
				{
					partyNameplateVM.OnFinalize();
					this.Nameplates.RemoveAt(i);
					return;
				}
			}
		}

		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party.Army != null && party.Army.LeaderParty == party)
			{
				using (List<MobileParty>.Enumerator enumerator = party.Army.Parties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MobileParty armyParty = enumerator.Current;
						if (armyParty.IsSpotted() && this.Nameplates.All((PartyNameplateVM p) => p.Party != armyParty))
						{
							this.Nameplates.Add(new PartyNameplateVM(armyParty, this._mapCamera, this._resetCamera, this._isShowPartyNamesEnabled));
						}
					}
				}
				return;
			}
			if (party.IsSpotted() && this.Nameplates.All((PartyNameplateVM p) => p.Party != party))
			{
				this.Nameplates.Add(new PartyNameplateVM(party, this._mapCamera, this._resetCamera, this._isShowPartyNamesEnabled));
			}
		}

		private void OnPartyVisibilityChanged(PartyBase party)
		{
			if (party.IsMobile)
			{
				if (party.MobileParty.IsSpotted() && party.MobileParty.CurrentSettlement == null && this.Nameplates.All((PartyNameplateVM p) => p.Party != party.MobileParty))
				{
					this.Nameplates.Add(new PartyNameplateVM(party.MobileParty, this._mapCamera, this._resetCamera, this._isShowPartyNamesEnabled));
					return;
				}
				PartyNameplateVM partyNameplateVM;
				if ((!party.MobileParty.IsSpotted() || party.MobileParty.CurrentSettlement != null) && (partyNameplateVM = this.Nameplates.FirstOrDefault((PartyNameplateVM p) => p.Party == party.MobileParty)) != null && !partyNameplateVM.IsMainParty)
				{
					partyNameplateVM.OnFinalize();
					this.Nameplates.Remove(partyNameplateVM);
				}
			}
		}

		public void Update()
		{
			for (int i = 0; i < this.Nameplates.Count; i++)
			{
				PartyNameplateVM partyNameplateVM = this.Nameplates[i];
				partyNameplateVM.RefreshPosition();
				partyNameplateVM.DetermineIsVisibleOnMap();
				partyNameplateVM.RefreshDynamicProperties(false);
			}
			for (int j = 0; j < this.Nameplates.Count; j++)
			{
				this.Nameplates[j].RefreshBinding();
			}
			this.Nameplates.Sort(this._nameplateComparer);
		}

		private void OnPlayerCharacterChangedEvent(Hero oldPlayer, Hero newPlayer, MobileParty newMainParty, bool isMainPartyChanged)
		{
			PartyNameplateVM partyNameplateVM = this.Nameplates.FirstOrDefault((PartyNameplateVM n) => n.GetIsMainParty);
			if (partyNameplateVM != null)
			{
				partyNameplateVM.OnFinalize();
				this.Nameplates.Remove(partyNameplateVM);
			}
			if (LinQuick.AllQ<PartyNameplateVM>(this.Nameplates, (PartyNameplateVM p) => p.Party.LeaderHero != newPlayer))
			{
				this.Nameplates.Add(new PartyNameplateVM(newMainParty, this._mapCamera, this._resetCamera, this._isShowPartyNamesEnabled));
			}
			foreach (PartyNameplateVM partyNameplateVM2 in this.Nameplates)
			{
				partyNameplateVM2.OnPlayerCharacterChanged(newPlayer);
			}
		}

		private void OnGameOver()
		{
			PartyNameplateVM partyNameplateVM = this.Nameplates.FirstOrDefault((PartyNameplateVM n) => n.IsMainParty);
			if (partyNameplateVM != null)
			{
				partyNameplateVM.OnFinalize();
				this.Nameplates.Remove(partyNameplateVM);
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Nameplates.Clear();
		}

		[DataSourceProperty]
		public MBBindingList<PartyNameplateVM> Nameplates
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
					base.OnPropertyChangedWithValue<MBBindingList<PartyNameplateVM>>(value, "Nameplates");
				}
			}
		}

		private readonly Camera _mapCamera;

		private readonly Action _resetCamera;

		private readonly Func<bool> _isShowPartyNamesEnabled;

		private readonly PartyNameplatesVM.NameplateDistanceComparer _nameplateComparer;

		private MBBindingList<PartyNameplateVM> _nameplates;

		public class NameplateDistanceComparer : IComparer<PartyNameplateVM>
		{
			public int Compare(PartyNameplateVM x, PartyNameplateVM y)
			{
				return y.DistanceToCamera.CompareTo(x.DistanceToCamera);
			}
		}
	}
}
