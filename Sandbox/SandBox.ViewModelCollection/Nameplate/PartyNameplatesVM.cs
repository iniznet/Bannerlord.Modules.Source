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
	// Token: 0x02000014 RID: 20
	public class PartyNameplatesVM : ViewModel
	{
		// Token: 0x060001DB RID: 475 RVA: 0x0000957C File Offset: 0x0000777C
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

		// Token: 0x060001DC RID: 476 RVA: 0x00009644 File Offset: 0x00007844
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Nameplates.ApplyActionOnAllItems(delegate(PartyNameplateVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00009678 File Offset: 0x00007878
		public void Initialize()
		{
			foreach (MobileParty mobileParty in MobileParty.All.Where((MobileParty p) => p.IsSpotted() && p.CurrentSettlement == null))
			{
				PartyNameplateVM partyNameplateVM = new PartyNameplateVM(mobileParty, this._mapCamera, this._resetCamera, this._isShowPartyNamesEnabled);
				this.Nameplates.Add(partyNameplateVM);
			}
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00009704 File Offset: 0x00007904
		private void OnClanChangeKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			foreach (PartyNameplateVM partyNameplateVM in this.Nameplates.Where((PartyNameplateVM p) => p.Party.LeaderHero != null && p.Party.LeaderHero.Clan == clan))
			{
				partyNameplateVM.RefreshDynamicProperties(true);
			}
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00009770 File Offset: 0x00007970
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

		// Token: 0x060001E0 RID: 480 RVA: 0x000097BC File Offset: 0x000079BC
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

		// Token: 0x060001E1 RID: 481 RVA: 0x000098F4 File Offset: 0x00007AF4
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

		// Token: 0x060001E2 RID: 482 RVA: 0x000099E4 File Offset: 0x00007BE4
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

		// Token: 0x060001E3 RID: 483 RVA: 0x00009A60 File Offset: 0x00007C60
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

		// Token: 0x060001E4 RID: 484 RVA: 0x00009B3C File Offset: 0x00007D3C
		private void OnGameOver()
		{
			PartyNameplateVM partyNameplateVM = this.Nameplates.FirstOrDefault((PartyNameplateVM n) => n.IsMainParty);
			if (partyNameplateVM != null)
			{
				partyNameplateVM.OnFinalize();
				this.Nameplates.Remove(partyNameplateVM);
			}
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00009B8A File Offset: 0x00007D8A
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Nameplates.Clear();
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x00009B9D File Offset: 0x00007D9D
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x00009BA5 File Offset: 0x00007DA5
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

		// Token: 0x040000C8 RID: 200
		private readonly Camera _mapCamera;

		// Token: 0x040000C9 RID: 201
		private readonly Action _resetCamera;

		// Token: 0x040000CA RID: 202
		private readonly Func<bool> _isShowPartyNamesEnabled;

		// Token: 0x040000CB RID: 203
		private readonly PartyNameplatesVM.NameplateDistanceComparer _nameplateComparer;

		// Token: 0x040000CC RID: 204
		private MBBindingList<PartyNameplateVM> _nameplates;

		// Token: 0x0200005D RID: 93
		public class NameplateDistanceComparer : IComparer<PartyNameplateVM>
		{
			// Token: 0x060004E0 RID: 1248 RVA: 0x00013F18 File Offset: 0x00012118
			public int Compare(PartyNameplateVM x, PartyNameplateVM y)
			{
				return y.DistanceToCamera.CompareTo(x.DistanceToCamera);
			}
		}
	}
}
