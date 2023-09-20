using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame
{
	// Token: 0x0200008A RID: 138
	public class MPCustomGameFilterItemVM : ViewModel
	{
		// Token: 0x06000CB3 RID: 3251 RVA: 0x0002C92D File Offset: 0x0002AB2D
		public MPCustomGameFilterItemVM(MPCustomGameFiltersVM.CustomGameFilterType filterType, TextObject description, Func<GameServerEntry, bool> getFilterApplicaple, Action onSelectionChange)
		{
			this._filterType = filterType;
			this._descriptionObj = description;
			this.GetIsApplicaple = getFilterApplicaple;
			this._onSelectionChange = onSelectionChange;
			this.SetInitialState();
			this.RefreshValues();
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x0002C95E File Offset: 0x0002AB5E
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Description = this._descriptionObj.ToString();
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x0002C978 File Offset: 0x0002AB78
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

		// Token: 0x06000CB6 RID: 3254 RVA: 0x0002C9E4 File Offset: 0x0002ABE4
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

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06000CB7 RID: 3255 RVA: 0x0002CA5B File Offset: 0x0002AC5B
		// (set) Token: 0x06000CB8 RID: 3256 RVA: 0x0002CA63 File Offset: 0x0002AC63
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

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06000CB9 RID: 3257 RVA: 0x0002CA87 File Offset: 0x0002AC87
		// (set) Token: 0x06000CBA RID: 3258 RVA: 0x0002CA8F File Offset: 0x0002AC8F
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

		// Token: 0x04000612 RID: 1554
		public readonly Func<GameServerEntry, bool> GetIsApplicaple;

		// Token: 0x04000613 RID: 1555
		public readonly Action _onSelectionChange;

		// Token: 0x04000614 RID: 1556
		private readonly TextObject _descriptionObj;

		// Token: 0x04000615 RID: 1557
		private MPCustomGameFiltersVM.CustomGameFilterType _filterType;

		// Token: 0x04000616 RID: 1558
		private bool _isSelected;

		// Token: 0x04000617 RID: 1559
		private string _description;
	}
}
