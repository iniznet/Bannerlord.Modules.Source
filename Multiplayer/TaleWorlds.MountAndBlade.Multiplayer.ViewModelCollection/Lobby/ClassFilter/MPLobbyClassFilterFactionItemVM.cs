using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.ClassFilter
{
	public class MPLobbyClassFilterFactionItemVM : ViewModel
	{
		public BasicCultureObject Culture { get; private set; }

		public MPLobbyClassFilterClassItemVM SelectedClassItem { get; private set; }

		public MPLobbyClassFilterFactionItemVM(string cultureCode, bool isEnabled, Action<MPLobbyClassFilterFactionItemVM> onActiveChanged, Action<MPLobbyClassFilterClassItemVM> onClassSelect)
		{
			this._onActiveChanged = onActiveChanged;
			this._onClassSelect = onClassSelect;
			this.CultureCode = cultureCode;
			this.IsEnabled = isEnabled;
			this.Culture = MBObjectManager.Instance.GetObject<BasicCultureObject>(cultureCode);
			this.CreateClassGroupAndClasses(this.Culture);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Hint = new HintViewModel(this.Culture.Name, null);
			this.ClassGroups.ApplyActionOnAllItems(delegate(MPLobbyClassFilterClassGroupItemVM x)
			{
				x.RefreshValues();
			});
		}

		public override void OnFinalize()
		{
			this.Culture = null;
			this._classGroupDictionary.Clear();
		}

		private void CreateClassGroupAndClasses(BasicCultureObject culture)
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
				this._classGroupDictionary[mpheroClass.ClassGroup.StringId].AddClass(culture, mpheroClass, new Action<MPLobbyClassFilterClassItemVM>(this.OnClassItemSelect));
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

		private void IsActiveChanged()
		{
			if (this.IsActive && this._onActiveChanged != null)
			{
				this._onActiveChanged(this);
			}
		}

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

		private Action<MPLobbyClassFilterFactionItemVM> _onActiveChanged;

		private Action<MPLobbyClassFilterClassItemVM> _onClassSelect;

		private Dictionary<string, MPLobbyClassFilterClassGroupItemVM> _classGroupDictionary;

		private bool _isActive;

		private bool _isEnabled;

		private string _cultureCode;

		private HintViewModel _hint;

		private MBBindingList<MPLobbyClassFilterClassGroupItemVM> _classGroups;
	}
}
