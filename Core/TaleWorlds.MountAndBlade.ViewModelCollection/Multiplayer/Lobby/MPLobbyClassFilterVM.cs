using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.ClassFilter;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x02000056 RID: 86
	public class MPLobbyClassFilterVM : ViewModel
	{
		// Token: 0x17000236 RID: 566
		// (get) Token: 0x0600073F RID: 1855 RVA: 0x0001CF62 File Offset: 0x0001B162
		// (set) Token: 0x06000740 RID: 1856 RVA: 0x0001CF6A File Offset: 0x0001B16A
		public MPLobbyClassFilterClassItemVM SelectedClassItem { get; private set; }

		// Token: 0x06000741 RID: 1857 RVA: 0x0001CF74 File Offset: 0x0001B174
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

		// Token: 0x06000742 RID: 1858 RVA: 0x0001D0D0 File Offset: 0x0001B2D0
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

		// Token: 0x06000743 RID: 1859 RVA: 0x0001D14D File Offset: 0x0001B34D
		private void OnFactionFilterChanged(MPLobbyClassFilterFactionItemVM factionItemVm)
		{
			this.ActiveClassGroups = factionItemVm.ClassGroups;
			this.OnSelectionChange(factionItemVm.SelectedClassItem);
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x0001D167 File Offset: 0x0001B367
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

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000745 RID: 1861 RVA: 0x0001D182 File Offset: 0x0001B382
		// (set) Token: 0x06000746 RID: 1862 RVA: 0x0001D18A File Offset: 0x0001B38A
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

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000747 RID: 1863 RVA: 0x0001D1AD File Offset: 0x0001B3AD
		// (set) Token: 0x06000748 RID: 1864 RVA: 0x0001D1B5 File Offset: 0x0001B3B5
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

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06000749 RID: 1865 RVA: 0x0001D1D3 File Offset: 0x0001B3D3
		// (set) Token: 0x0600074A RID: 1866 RVA: 0x0001D1DB File Offset: 0x0001B3DB
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

		// Token: 0x040003B0 RID: 944
		private Action<MPLobbyClassFilterClassItemVM, bool> _onSelectionChange;

		// Token: 0x040003B2 RID: 946
		private string _titleText;

		// Token: 0x040003B3 RID: 947
		private MBBindingList<MPLobbyClassFilterFactionItemVM> _factions;

		// Token: 0x040003B4 RID: 948
		private MBBindingList<MPLobbyClassFilterClassGroupItemVM> _activeClassGroups;
	}
}
