using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.ClassFilter;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby
{
	public class MPLobbyClassFilterVM : ViewModel
	{
		public MPLobbyClassFilterClassItemVM SelectedClassItem { get; private set; }

		public MPLobbyClassFilterVM(Action<MPLobbyClassFilterClassItemVM, bool> onSelectionChange)
		{
			this._onSelectionChange = onSelectionChange;
			this.Factions = new MBBindingList<MPLobbyClassFilterFactionItemVM>();
			this.Factions.Add(new MPLobbyClassFilterFactionItemVM("empire", true, new Action<MPLobbyClassFilterFactionItemVM>(this.OnFactionFilterChanged), new Action<MPLobbyClassFilterClassItemVM>(this.OnSelectionChange)));
			this.Factions.Add(new MPLobbyClassFilterFactionItemVM("vlandia", true, new Action<MPLobbyClassFilterFactionItemVM>(this.OnFactionFilterChanged), new Action<MPLobbyClassFilterClassItemVM>(this.OnSelectionChange)));
			this.Factions.Add(new MPLobbyClassFilterFactionItemVM("battania", true, new Action<MPLobbyClassFilterFactionItemVM>(this.OnFactionFilterChanged), new Action<MPLobbyClassFilterClassItemVM>(this.OnSelectionChange)));
			this.Factions.Add(new MPLobbyClassFilterFactionItemVM("sturgia", true, new Action<MPLobbyClassFilterFactionItemVM>(this.OnFactionFilterChanged), new Action<MPLobbyClassFilterClassItemVM>(this.OnSelectionChange)));
			this.Factions.Add(new MPLobbyClassFilterFactionItemVM("khuzait", true, new Action<MPLobbyClassFilterFactionItemVM>(this.OnFactionFilterChanged), new Action<MPLobbyClassFilterClassItemVM>(this.OnSelectionChange)));
			this.Factions.Add(new MPLobbyClassFilterFactionItemVM("aserai", true, new Action<MPLobbyClassFilterFactionItemVM>(this.OnFactionFilterChanged), new Action<MPLobbyClassFilterClassItemVM>(this.OnSelectionChange)));
			this.ActiveClassGroups = new MBBindingList<MPLobbyClassFilterClassGroupItemVM>();
			this.Factions[0].IsActive = true;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=Q50X65NB}Classes", null).ToString();
			this.Factions.ApplyActionOnAllItems(delegate(MPLobbyClassFilterFactionItemVM x)
			{
				x.RefreshValues();
			});
			this.ActiveClassGroups.ApplyActionOnAllItems(delegate(MPLobbyClassFilterClassGroupItemVM x)
			{
				x.RefreshValues();
			});
		}

		private void OnFactionFilterChanged(MPLobbyClassFilterFactionItemVM factionItemVm)
		{
			this.ActiveClassGroups = factionItemVm.ClassGroups;
			this.OnSelectionChange(factionItemVm.SelectedClassItem);
		}

		private void OnSelectionChange(MPLobbyClassFilterClassItemVM selectedItemVm)
		{
			this.SelectedClassItem = selectedItemVm;
			Action<MPLobbyClassFilterClassItemVM, bool> onSelectionChange = this._onSelectionChange;
			if (onSelectionChange == null)
			{
				return;
			}
			onSelectionChange(selectedItemVm, false);
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPLobbyClassFilterFactionItemVM> Factions
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
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyClassFilterFactionItemVM>>(value, "Factions");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPLobbyClassFilterClassGroupItemVM> ActiveClassGroups
		{
			get
			{
				return this._activeClassGroups;
			}
			set
			{
				if (value != this._activeClassGroups)
				{
					this._activeClassGroups = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyClassFilterClassGroupItemVM>>(value, "ActiveClassGroups");
				}
			}
		}

		private Action<MPLobbyClassFilterClassItemVM, bool> _onSelectionChange;

		private string _titleText;

		private MBBindingList<MPLobbyClassFilterFactionItemVM> _factions;

		private MBBindingList<MPLobbyClassFilterClassGroupItemVM> _activeClassGroups;
	}
}
