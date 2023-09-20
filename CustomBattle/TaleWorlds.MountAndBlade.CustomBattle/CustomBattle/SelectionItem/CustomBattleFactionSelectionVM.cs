using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	public class CustomBattleFactionSelectionVM : ViewModel
	{
		public CustomBattleFactionSelectionVM(Action<BasicCultureObject> onSelectionChanged)
		{
			this._onSelectionChanged = onSelectionChanged;
			this.Factions = new MBBindingList<FactionItemVM>();
			foreach (BasicCultureObject basicCultureObject in CustomBattleData.Factions)
			{
				this.Factions.Add(new FactionItemVM(basicCultureObject, new Action<FactionItemVM>(this.OnFactionSelected)));
			}
			this.SelectedItem = this.Factions[0];
			this.SelectFaction(0);
			this.Factions.ApplyActionOnAllItems(delegate(FactionItemVM x)
			{
				x.RefreshValues();
			});
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			FactionItemVM selectedItem = this.SelectedItem;
			this.SelectedFactionName = ((selectedItem != null) ? selectedItem.Faction.Name.ToString() : null);
		}

		public void SelectFaction(int index)
		{
			if (index >= 0 && index < this.Factions.Count)
			{
				this.SelectedItem.IsSelected = false;
				this.SelectedItem = this.Factions[index];
				this.SelectedItem.IsSelected = true;
			}
		}

		public void ExecuteRandomize()
		{
			int num = MBRandom.RandomInt(this.Factions.Count);
			this.SelectFaction(num);
		}

		private void OnFactionSelected(FactionItemVM faction)
		{
			this.SelectedItem = faction;
			this._onSelectionChanged(faction.Faction);
			this.SelectedFactionName = this.SelectedItem.Faction.Name.ToString();
		}

		[DataSourceProperty]
		public MBBindingList<FactionItemVM> Factions
		{
			get
			{
				return this._factions;
			}
			set
			{
				if (value != this._factions)
				{
					this._factions = value;
					base.OnPropertyChangedWithValue<MBBindingList<FactionItemVM>>(value, "Factions");
				}
			}
		}

		[DataSourceProperty]
		public string SelectedFactionName
		{
			get
			{
				return this._selectedFactionName;
			}
			set
			{
				if (value != this._selectedFactionName)
				{
					this._selectedFactionName = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectedFactionName");
				}
			}
		}

		public FactionItemVM SelectedItem;

		private Action<BasicCultureObject> _onSelectionChanged;

		private MBBindingList<FactionItemVM> _factions;

		private string _selectedFactionName;
	}
}
