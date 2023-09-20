using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.CustomGame
{
	public class MPCustomGameFilterItemVM : ViewModel
	{
		public MPCustomGameFilterItemVM(MPCustomGameFiltersVM.CustomGameFilterType filterType, TextObject description, Func<GameServerEntry, bool> getFilterApplicaple, Action onSelectionChange)
		{
			this._filterType = filterType;
			this._descriptionObj = description;
			this.GetIsApplicaple = getFilterApplicaple;
			this._onSelectionChange = onSelectionChange;
			this.SetInitialState();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Description = this._descriptionObj.ToString();
		}

		private void SetInitialState()
		{
			switch (this._filterType)
			{
			case MPCustomGameFiltersVM.CustomGameFilterType.NotFull:
				this.IsSelected = BannerlordConfig.HideFullServers;
				return;
			case MPCustomGameFiltersVM.CustomGameFilterType.HasPlayers:
				this.IsSelected = BannerlordConfig.HideEmptyServers;
				return;
			case MPCustomGameFiltersVM.CustomGameFilterType.HasPasswordProtection:
				this.IsSelected = BannerlordConfig.HidePasswordProtectedServers;
				return;
			case MPCustomGameFiltersVM.CustomGameFilterType.IsOfficial:
				this.IsSelected = BannerlordConfig.HideUnofficialServers;
				return;
			case MPCustomGameFiltersVM.CustomGameFilterType.ModuleCompatible:
				this.IsSelected = BannerlordConfig.HideModuleIncompatibleServers;
				return;
			default:
				return;
			}
		}

		private void OnToggled()
		{
			this._onSelectionChange();
			switch (this._filterType)
			{
			case MPCustomGameFiltersVM.CustomGameFilterType.NotFull:
				BannerlordConfig.HideFullServers = this.IsSelected;
				return;
			case MPCustomGameFiltersVM.CustomGameFilterType.HasPlayers:
				BannerlordConfig.HideEmptyServers = this.IsSelected;
				return;
			case MPCustomGameFiltersVM.CustomGameFilterType.HasPasswordProtection:
				BannerlordConfig.HidePasswordProtectedServers = this.IsSelected;
				return;
			case MPCustomGameFiltersVM.CustomGameFilterType.IsOfficial:
				BannerlordConfig.HideUnofficialServers = this.IsSelected;
				return;
			case MPCustomGameFiltersVM.CustomGameFilterType.ModuleCompatible:
				BannerlordConfig.HideModuleIncompatibleServers = this.IsSelected;
				return;
			default:
				return;
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
					this.OnToggled();
				}
			}
		}

		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		public readonly Func<GameServerEntry, bool> GetIsApplicaple;

		public readonly Action _onSelectionChange;

		private readonly TextObject _descriptionObj;

		private MPCustomGameFiltersVM.CustomGameFilterType _filterType;

		private bool _isSelected;

		private string _description;
	}
}
