using System;
using System.Collections.Generic;
using System.Linq;
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
			this.AvailableGovernors = new MBBindingList<SettlementGovernorSelectionItemVM>
			{
				new SettlementGovernorSelectionItemVM(null, new Action<SettlementGovernorSelectionItemVM>(this.OnSelection))
			};
			if (((settlement != null) ? settlement.OwnerClan : null) != null)
			{
				using (List<Hero>.Enumerator enumerator = settlement.OwnerClan.Heroes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Hero hero = enumerator.Current;
						if (Campaign.Current.Models.ClanPoliticsModel.CanHeroBeGovernor(hero) && !this.AvailableGovernors.Any((SettlementGovernorSelectionItemVM G) => G.Governor == hero) && (hero.GovernorOf == this._settlement.Town || hero.GovernorOf == null))
						{
							this.AvailableGovernors.Add(new SettlementGovernorSelectionItemVM(hero, new Action<SettlementGovernorSelectionItemVM>(this.OnSelection)));
						}
					}
				}
			}
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
