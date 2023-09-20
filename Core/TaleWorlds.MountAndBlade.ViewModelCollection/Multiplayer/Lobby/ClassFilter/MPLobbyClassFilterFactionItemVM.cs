using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.ClassFilter
{
	// Token: 0x02000091 RID: 145
	public class MPLobbyClassFilterFactionItemVM : ViewModel
	{
		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06000D9E RID: 3486 RVA: 0x0002EEC5 File Offset: 0x0002D0C5
		// (set) Token: 0x06000D9F RID: 3487 RVA: 0x0002EECD File Offset: 0x0002D0CD
		public BasicCultureObject Culture { get; private set; }

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06000DA0 RID: 3488 RVA: 0x0002EED6 File Offset: 0x0002D0D6
		// (set) Token: 0x06000DA1 RID: 3489 RVA: 0x0002EEDE File Offset: 0x0002D0DE
		public MPLobbyClassFilterClassItemVM SelectedClassItem { get; private set; }

		// Token: 0x06000DA2 RID: 3490 RVA: 0x0002EEE8 File Offset: 0x0002D0E8
		public MPLobbyClassFilterFactionItemVM(string cultureCode, bool isEnabled, Action<MPLobbyClassFilterFactionItemVM> onActiveChanged, Action<MPLobbyClassFilterClassItemVM> onClassSelect)
		{
			this._onActiveChanged = onActiveChanged;
			this._onClassSelect = onClassSelect;
			this.CultureCode = cultureCode;
			this.IsEnabled = isEnabled;
			this.Culture = MBObjectManager.Instance.GetObject<BasicCultureObject>(cultureCode);
			this.CreateClassGroupAndClasses(cultureCode);
			this.RefreshValues();
		}

		// Token: 0x06000DA3 RID: 3491 RVA: 0x0002EF38 File Offset: 0x0002D138
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Hint = new HintViewModel(this.Culture.Name, null);
			this.ClassGroups.ApplyActionOnAllItems(delegate(MPLobbyClassFilterClassGroupItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000DA4 RID: 3492 RVA: 0x0002EF8C File Offset: 0x0002D18C
		public override void OnFinalize()
		{
			this.Culture = null;
			this._classGroupDictionary.Clear();
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x0002EFA0 File Offset: 0x0002D1A0
		private void CreateClassGroupAndClasses(string cultureCode)
		{
			this._classGroupDictionary = new Dictionary<string, MPLobbyClassFilterClassGroupItemVM>();
			this.ClassGroups = new MBBindingList<MPLobbyClassFilterClassGroupItemVM>();
			foreach (MultiplayerClassDivisions.MPHeroClassGroup mpheroClassGroup in MultiplayerClassDivisions.MultiplayerHeroClassGroups)
			{
				MPLobbyClassFilterClassGroupItemVM mplobbyClassFilterClassGroupItemVM = new MPLobbyClassFilterClassGroupItemVM(mpheroClassGroup);
				this.ClassGroups.Add(mplobbyClassFilterClassGroupItemVM);
				this._classGroupDictionary.Add(mpheroClassGroup.StringId, mplobbyClassFilterClassGroupItemVM);
			}
			foreach (MultiplayerClassDivisions.MPHeroClass mpheroClass in MultiplayerClassDivisions.GetMPHeroClasses(this.Culture))
			{
				this._classGroupDictionary[mpheroClass.ClassGroup.StringId].AddClass(cultureCode, mpheroClass, new Action<MPLobbyClassFilterClassItemVM>(this.OnClassItemSelect));
			}
			for (int i = this.ClassGroups.Count - 1; i >= 0; i--)
			{
				if (this.ClassGroups[i].Classes.Count == 0)
				{
					this.ClassGroups.RemoveAt(i);
				}
			}
			MPLobbyClassFilterClassItemVM mplobbyClassFilterClassItemVM = this.ClassGroups[0].Classes[0];
			mplobbyClassFilterClassItemVM.IsSelected = true;
			this.SelectedClassItem = mplobbyClassFilterClassItemVM;
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x0002F0FC File Offset: 0x0002D2FC
		private void OnClassItemSelect(MPLobbyClassFilterClassItemVM selectedClassItem)
		{
			foreach (MPLobbyClassFilterClassGroupItemVM mplobbyClassFilterClassGroupItemVM in this.ClassGroups)
			{
				foreach (MPLobbyClassFilterClassItemVM mplobbyClassFilterClassItemVM in mplobbyClassFilterClassGroupItemVM.Classes)
				{
					if (mplobbyClassFilterClassItemVM != selectedClassItem)
					{
						mplobbyClassFilterClassItemVM.IsSelected = false;
					}
				}
			}
			this.SelectedClassItem = selectedClassItem;
			if (this._onClassSelect != null)
			{
				this._onClassSelect(selectedClassItem);
			}
		}

		// Token: 0x06000DA7 RID: 3495 RVA: 0x0002F19C File Offset: 0x0002D39C
		private void IsActiveChanged()
		{
			if (this.IsActive && this._onActiveChanged != null)
			{
				this._onActiveChanged(this);
			}
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06000DA8 RID: 3496 RVA: 0x0002F1BA File Offset: 0x0002D3BA
		// (set) Token: 0x06000DA9 RID: 3497 RVA: 0x0002F1C2 File Offset: 0x0002D3C2
		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
					this.IsActiveChanged();
				}
			}
		}

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06000DAA RID: 3498 RVA: 0x0002F1E6 File Offset: 0x0002D3E6
		// (set) Token: 0x06000DAB RID: 3499 RVA: 0x0002F1EE File Offset: 0x0002D3EE
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06000DAC RID: 3500 RVA: 0x0002F20C File Offset: 0x0002D40C
		// (set) Token: 0x06000DAD RID: 3501 RVA: 0x0002F214 File Offset: 0x0002D414
		[DataSourceProperty]
		public string CultureCode
		{
			get
			{
				return this._cultureCode;
			}
			set
			{
				if (value != this._cultureCode)
				{
					this._cultureCode = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureCode");
				}
			}
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06000DAE RID: 3502 RVA: 0x0002F237 File Offset: 0x0002D437
		// (set) Token: 0x06000DAF RID: 3503 RVA: 0x0002F23F File Offset: 0x0002D43F
		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06000DB0 RID: 3504 RVA: 0x0002F25D File Offset: 0x0002D45D
		// (set) Token: 0x06000DB1 RID: 3505 RVA: 0x0002F265 File Offset: 0x0002D465
		[DataSourceProperty]
		public MBBindingList<MPLobbyClassFilterClassGroupItemVM> ClassGroups
		{
			get
			{
				return this._classGroups;
			}
			set
			{
				if (value != this._classGroups)
				{
					this._classGroups = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyClassFilterClassGroupItemVM>>(value, "ClassGroups");
				}
			}
		}

		// Token: 0x04000685 RID: 1669
		private Action<MPLobbyClassFilterFactionItemVM> _onActiveChanged;

		// Token: 0x04000686 RID: 1670
		private Action<MPLobbyClassFilterClassItemVM> _onClassSelect;

		// Token: 0x04000687 RID: 1671
		private Dictionary<string, MPLobbyClassFilterClassGroupItemVM> _classGroupDictionary;

		// Token: 0x0400068A RID: 1674
		private bool _isActive;

		// Token: 0x0400068B RID: 1675
		private bool _isEnabled;

		// Token: 0x0400068C RID: 1676
		private string _cultureCode;

		// Token: 0x0400068D RID: 1677
		private HintViewModel _hint;

		// Token: 0x0400068E RID: 1678
		private MBBindingList<MPLobbyClassFilterClassGroupItemVM> _classGroups;
	}
}
