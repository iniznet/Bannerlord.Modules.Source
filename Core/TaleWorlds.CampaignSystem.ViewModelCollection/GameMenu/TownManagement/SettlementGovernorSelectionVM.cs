using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	public class SettlementGovernorSelectionVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.AvailableGovernors.ApplyActionOnAllItems(delegate(SettlementGovernorSelectionItemVM x)
			{
				x.RefreshValues();
			});
		}

		private void OnSelection(SettlementGovernorSelectionItemVM item)
		{
			Action<Hero> onDone = this._onDone;
			if (onDone == null)
			{
				return;
			}
			onDone(item.Governor);
		}

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

		private readonly Settlement _settlement;

		private readonly Action<Hero> _onDone;

		private MBBindingList<SettlementGovernorSelectionItemVM> _availableGovernors;

		private int _currentGovernorIndex;
	}
}
