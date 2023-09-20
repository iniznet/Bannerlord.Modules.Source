using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	// Token: 0x0200008F RID: 143
	public class SettlementGovernorSelectionVM : ViewModel
	{
		// Token: 0x06000DEC RID: 3564 RVA: 0x00037DDC File Offset: 0x00035FDC
		public SettlementGovernorSelectionVM(Settlement settlement, Action<Hero> onDone)
		{
			this._settlement = settlement;
			this._onDone = onDone;
			this.AvailableGovernors = new MBBindingList<SettlementGovernorSelectionItemVM>();
			this.AvailableGovernors.Add(new SettlementGovernorSelectionItemVM(null, new Action<SettlementGovernorSelectionItemVM>(this.OnSelection)));
			if (((settlement != null) ? settlement.OwnerClan : null) != null)
			{
				foreach (Hero hero in settlement.OwnerClan.Heroes)
				{
					if (this.IsHeroApplicableForGovernor(hero))
					{
						this.AvailableGovernors.Add(new SettlementGovernorSelectionItemVM(hero, new Action<SettlementGovernorSelectionItemVM>(this.OnSelection)));
					}
				}
			}
		}

		// Token: 0x06000DED RID: 3565 RVA: 0x00037EA0 File Offset: 0x000360A0
		private bool IsHeroApplicableForGovernor(Hero hero)
		{
			if (!hero.IsDead && !hero.IsDisabled && !hero.IsChild && hero.CanBeGovernorOrHavePartyRole() && hero != Hero.MainHero)
			{
				MobileParty partyBelongedTo = hero.PartyBelongedTo;
				if (((partyBelongedTo != null) ? partyBelongedTo.LeaderHero : null) != hero && (hero.GovernorOf == this._settlement.Town || hero.GovernorOf == null) && !hero.IsFugitive && !hero.IsReleased)
				{
					return !this.AvailableGovernors.Any((SettlementGovernorSelectionItemVM G) => G.Governor == hero);
				}
			}
			return false;
		}

		// Token: 0x06000DEE RID: 3566 RVA: 0x00037F81 File Offset: 0x00036181
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.AvailableGovernors.ApplyActionOnAllItems(delegate(SettlementGovernorSelectionItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000DEF RID: 3567 RVA: 0x00037FB3 File Offset: 0x000361B3
		private void OnSelection(SettlementGovernorSelectionItemVM item)
		{
			Action<Hero> onDone = this._onDone;
			if (onDone == null)
			{
				return;
			}
			onDone(item.Governor);
		}

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06000DF0 RID: 3568 RVA: 0x00037FCB File Offset: 0x000361CB
		// (set) Token: 0x06000DF1 RID: 3569 RVA: 0x00037FD3 File Offset: 0x000361D3
		[DataSourceProperty]
		public MBBindingList<SettlementGovernorSelectionItemVM> AvailableGovernors
		{
			get
			{
				return this._availableGovernors;
			}
			set
			{
				if (value != this._availableGovernors)
				{
					this._availableGovernors = value;
					base.OnPropertyChangedWithValue<MBBindingList<SettlementGovernorSelectionItemVM>>(value, "AvailableGovernors");
				}
			}
		}

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06000DF2 RID: 3570 RVA: 0x00037FF1 File Offset: 0x000361F1
		// (set) Token: 0x06000DF3 RID: 3571 RVA: 0x00037FF9 File Offset: 0x000361F9
		[DataSourceProperty]
		public int CurrentGovernorIndex
		{
			get
			{
				return this._currentGovernorIndex;
			}
			set
			{
				if (value != this._currentGovernorIndex)
				{
					this._currentGovernorIndex = value;
					base.OnPropertyChangedWithValue(value, "CurrentGovernorIndex");
				}
			}
		}

		// Token: 0x04000674 RID: 1652
		private readonly Settlement _settlement;

		// Token: 0x04000675 RID: 1653
		private readonly Action<Hero> _onDone;

		// Token: 0x04000676 RID: 1654
		private MBBindingList<SettlementGovernorSelectionItemVM> _availableGovernors;

		// Token: 0x04000677 RID: 1655
		private int _currentGovernorIndex;
	}
}
