using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.BannerEditor;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public class BannerEditorVM : ViewModel
	{
		public BasicCharacterObject Character { get; }

		public BannerEditorVM(BasicCharacterObject character, Banner banner, Action<bool> onExit, Action refresh, int currentStageIndex, int totalStagesCount, int furthestIndex, Action<int> goToIndex)
		{
			this.Character = character;
			this._initialBanner = banner.Serialize();
			this._banner = banner;
			this.IconsList = new MBBindingList<BannerIconVM>();
			this.PrimaryColorList = new MBBindingList<BannerColorVM>();
			this.SigilColorList = new MBBindingList<BannerColorVM>();
			this._refresh = refresh;
			this.OnExit = onExit;
			this.BannerVM = new BannerViewModel(banner);
			this.CanChangeBackgroundColor = true;
			this._shield = this.FindShield();
			if (this._shield != null)
			{
				this.ShieldRosterElement = new ItemRosterElement(this._shield, 1, null);
			}
			else
			{
				Debug.FailedAssert("Banner Editor couldn't find a shield to show", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\BannerEditorVM.cs", ".ctor", 55);
			}
			this._goToIndex = goToIndex;
			this.TotalStageCount = totalStagesCount;
			this.CurrentStageIndex = currentStageIndex;
			this.FurthestIndex = furthestIndex;
			this.MinIconSize = 100;
			this.MaxIconSize = 700;
			this.BannerVM.SetCode(banner.Serialize());
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Title = GameTexts.FindText("str_banner_editor", null).ToString();
			this.CancelText = GameTexts.FindText("str_cancel", null).ToString();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.ResetHint = new HintViewModel(GameTexts.FindText("str_reset_icon", null), null);
			this.RandomizeHint = new HintViewModel(GameTexts.FindText("str_randomize", null), null);
			this.UndoHint = new HintViewModel(GameTexts.FindText("str_undo", null), null);
			this.RedoHint = new HintViewModel(GameTexts.FindText("str_redo", null), null);
			this.PrimaryColorText = new TextObject("{=xwRWjlar}Background Color:", null).ToString();
			this.SigilColorText = new TextObject("{=7tBOCHm6}Sigil Color:", null).ToString();
			this.SizeText = new TextObject("{=OkWLI5C8}Size:", null).ToString();
			this.CategoryNames = new MBBindingList<HintViewModel>();
			foreach (BannerIconGroup bannerIconGroup in BannerManager.Instance.BannerIconGroups)
			{
				if (!bannerIconGroup.IsPattern)
				{
					foreach (KeyValuePair<int, BannerIconData> keyValuePair in bannerIconGroup.AvailableIcons)
					{
						BannerIconVM bannerIconVM = new BannerIconVM(keyValuePair.Key, new Action<BannerIconVM>(this.OnIconSelection));
						this.IconsList.Add(bannerIconVM);
						bannerIconVM.IsSelected = bannerIconVM.IconID == this._banner.BannerDataList[1].MeshId;
					}
					this.CategoryNames.Add(new HintViewModel(bannerIconGroup.Name, "banner_group_hint_" + bannerIconGroup.Id));
				}
			}
			bool flag = this.IsColorsSwitched();
			foreach (KeyValuePair<int, BannerColor> keyValuePair2 in BannerManager.Instance.ReadOnlyColorPalette)
			{
				bool flag2 = (flag ? keyValuePair2.Value.PlayerCanChooseForSigil : keyValuePair2.Value.PlayerCanChooseForBackground);
				bool flag3 = (flag ? keyValuePair2.Value.PlayerCanChooseForBackground : keyValuePair2.Value.PlayerCanChooseForSigil);
				if (flag2)
				{
					BannerColorVM bannerColorVM = new BannerColorVM(keyValuePair2.Key, keyValuePair2.Value.Color, new Action<BannerColorVM>(this.OnPrimaryColorSelection));
					if (bannerColorVM.ColorID == this._banner.BannerDataList[0].ColorId)
					{
						bannerColorVM.IsSelected = true;
						this._currentSelectedPrimaryColor = bannerColorVM;
					}
					this.PrimaryColorList.Add(bannerColorVM);
				}
				if (flag3)
				{
					BannerColorVM bannerColorVM2 = new BannerColorVM(keyValuePair2.Key, keyValuePair2.Value.Color, new Action<BannerColorVM>(this.OnSigilColorSelection));
					if (bannerColorVM2.ColorID == this._banner.BannerDataList[1].ColorId)
					{
						bannerColorVM2.IsSelected = true;
						this._currentSelectedSigilColor = bannerColorVM2;
					}
					this.SigilColorList.Add(bannerColorVM2);
				}
			}
			this.CurrentIconSize = (int)this._banner.BannerDataList[1].Size.X;
			this._initialized = true;
		}

		private bool IsColorsSwitched()
		{
			foreach (KeyValuePair<int, BannerColor> keyValuePair in BannerManager.Instance.ReadOnlyColorPalette)
			{
				if (keyValuePair.Value.PlayerCanChooseForBackground && keyValuePair.Key == this._banner.BannerDataList[0].ColorId)
				{
					return false;
				}
			}
			return true;
		}

		public void SetClanRelatedRules(bool canChangeBackgroundColor)
		{
			this.CanChangeBackgroundColor = canChangeBackgroundColor;
		}

		private void OnIconSelection(BannerIconVM icon)
		{
			if (icon != this._currentSelectedIcon)
			{
				if (this._currentSelectedIcon != null)
				{
					this._currentSelectedIcon.IsSelected = false;
				}
				this._currentSelectedIcon = icon;
				icon.IsSelected = true;
				this.BannerVM.SetIconMeshID(icon.IconID);
				this._refresh();
			}
		}

		public void ExecuteSwitchColors()
		{
			if (this._currentSelectedPrimaryColor == null || this._currentSelectedSigilColor == null)
			{
				Debug.FailedAssert("Couldn't find current player clan colors in the list of selectable banner editor colors.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\BannerEditorVM.cs", "ExecuteSwitchColors", 179);
				return;
			}
			MBBindingList<BannerColorVM> primaryColorList = this.PrimaryColorList;
			this.PrimaryColorList = this.SigilColorList;
			this.SigilColorList = primaryColorList;
			this.PrimaryColorList.ApplyActionOnAllItems(delegate(BannerColorVM x)
			{
				x.SetOnSelectionAction(new Action<BannerColorVM>(this.OnPrimaryColorSelection));
			});
			this.SigilColorList.ApplyActionOnAllItems(delegate(BannerColorVM x)
			{
				x.SetOnSelectionAction(new Action<BannerColorVM>(this.OnSigilColorSelection));
			});
			BannerColorVM currentSelectedPrimaryColor = this._currentSelectedPrimaryColor;
			this._currentSelectedPrimaryColor = this._currentSelectedSigilColor;
			this._currentSelectedSigilColor = currentSelectedPrimaryColor;
			this._currentSelectedPrimaryColor.IsSelected = true;
			this._currentSelectedSigilColor.IsSelected = true;
			this.BannerVM.SetPrimaryColorId(this._currentSelectedPrimaryColor.ColorID);
			this.BannerVM.SetSecondaryColorId(this._currentSelectedPrimaryColor.ColorID);
			this.BannerVM.SetSigilColorId(this._currentSelectedSigilColor.ColorID);
			this._refresh();
		}

		private void OnPrimaryColorSelection(BannerColorVM color)
		{
			if (color != this._currentSelectedPrimaryColor)
			{
				if (this._currentSelectedPrimaryColor != null)
				{
					this._currentSelectedPrimaryColor.IsSelected = false;
				}
				this._currentSelectedPrimaryColor = color;
				color.IsSelected = true;
				this.BannerVM.SetPrimaryColorId(color.ColorID);
				this.BannerVM.SetSecondaryColorId(color.ColorID);
				this._refresh();
			}
		}

		private void OnSigilColorSelection(BannerColorVM color)
		{
			if (color != this._currentSelectedSigilColor)
			{
				if (this._currentSelectedSigilColor != null)
				{
					this._currentSelectedSigilColor.IsSelected = false;
				}
				this._currentSelectedSigilColor = color;
				color.IsSelected = true;
				this.BannerVM.SetSigilColorId(color.ColorID);
				this._refresh();
			}
		}

		private void OnBannerIconSizeChange(int newSize)
		{
			this.BannerVM.SetIconSize(newSize);
			this._refresh();
		}

		public void ExecuteDone()
		{
			this.OnExit(false);
		}

		public void ExecuteCancel()
		{
			this._banner.Deserialize(this._initialBanner);
			this.OnExit(true);
		}

		private ItemObject FindShield()
		{
			for (int i = 0; i < 4; i++)
			{
				EquipmentElement equipmentFromSlot = this.Character.Equipment.GetEquipmentFromSlot((EquipmentIndex)i);
				ItemObject item = equipmentFromSlot.Item;
				if (((item != null) ? item.PrimaryWeapon : null) != null && equipmentFromSlot.Item.PrimaryWeapon.IsShield && equipmentFromSlot.Item.IsUsingTableau)
				{
					return equipmentFromSlot.Item;
				}
			}
			foreach (ItemObject itemObject in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
			{
				if (itemObject.PrimaryWeapon != null && itemObject.PrimaryWeapon.IsShield && itemObject.IsUsingTableau)
				{
					return itemObject;
				}
			}
			return null;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey == null)
			{
				return;
			}
			doneInputKey.OnFinalize();
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		public void ExecuteGoToIndex(int index)
		{
			this._goToIndex(index);
		}

		[DataSourceProperty]
		public MBBindingList<HintViewModel> CategoryNames
		{
			get
			{
				return this._categoryNames;
			}
			set
			{
				if (value != this._categoryNames)
				{
					this._categoryNames = value;
					base.OnPropertyChangedWithValue<MBBindingList<HintViewModel>>(value, "CategoryNames");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<BannerIconVM> IconsList
		{
			get
			{
				return this._iconsList;
			}
			set
			{
				if (value != this._iconsList)
				{
					this._iconsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BannerIconVM>>(value, "IconsList");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<BannerColorVM> PrimaryColorList
		{
			get
			{
				return this._primaryColorList;
			}
			set
			{
				if (value != this._primaryColorList)
				{
					this._primaryColorList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BannerColorVM>>(value, "PrimaryColorList");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<BannerColorVM> SigilColorList
		{
			get
			{
				return this._sigilColorList;
			}
			set
			{
				if (value != this._sigilColorList)
				{
					this._sigilColorList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BannerColorVM>>(value, "SigilColorList");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel RandomizeHint
		{
			get
			{
				return this._randomizeHint;
			}
			set
			{
				if (value != this._randomizeHint)
				{
					this._randomizeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RandomizeHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel UndoHint
		{
			get
			{
				return this._undoHint;
			}
			set
			{
				if (value != this._undoHint)
				{
					this._undoHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "UndoHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel RedoHint
		{
			get
			{
				return this._redoHint;
			}
			set
			{
				if (value != this._redoHint)
				{
					this._redoHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RedoHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ResetHint
		{
			get
			{
				return this._resetHint;
			}
			set
			{
				if (value != this._resetHint)
				{
					this._resetHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ResetHint");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentShieldName
		{
			get
			{
				return this._currentShieldName;
			}
			set
			{
				if (value != this._currentShieldName)
				{
					this._currentShieldName = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentShieldName");
				}
			}
		}

		[DataSourceProperty]
		public int MinIconSize
		{
			get
			{
				return this._minIconSize;
			}
			set
			{
				if (value != this._minIconSize)
				{
					this._minIconSize = value;
					base.OnPropertyChangedWithValue(value, "MinIconSize");
				}
			}
		}

		[DataSourceProperty]
		public int MaxIconSize
		{
			get
			{
				return this._maxIconSize;
			}
			set
			{
				if (value != this._maxIconSize)
				{
					this._maxIconSize = value;
					base.OnPropertyChangedWithValue(value, "MaxIconSize");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentIconSize
		{
			get
			{
				return this._currentIconSize;
			}
			set
			{
				if (value != this._currentIconSize)
				{
					this._currentIconSize = value;
					base.OnPropertyChangedWithValue(value, "CurrentIconSize");
					if (this._initialized)
					{
						this.OnBannerIconSizeChange(value);
					}
				}
			}
		}

		[DataSourceProperty]
		public string PrimaryColorText
		{
			get
			{
				return this._primaryColorText;
			}
			set
			{
				if (value != this._primaryColorText)
				{
					this._primaryColorText = value;
					base.OnPropertyChangedWithValue<string>(value, "PrimaryColorText");
				}
			}
		}

		[DataSourceProperty]
		public string SizeText
		{
			get
			{
				return this._sizeText;
			}
			set
			{
				if (value != this._sizeText)
				{
					this._sizeText = value;
					base.OnPropertyChangedWithValue<string>(value, "SizeText");
				}
			}
		}

		[DataSourceProperty]
		public string SigilColorText
		{
			get
			{
				return this._sigilColorText;
			}
			set
			{
				if (value != this._sigilColorText)
				{
					this._sigilColorText = value;
					base.OnPropertyChangedWithValue<string>(value, "SigilColorText");
				}
			}
		}

		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelText");
				}
			}
		}

		[DataSourceProperty]
		public string DoneText
		{
			get
			{
				return this._doneText;
			}
			set
			{
				if (value != this._doneText)
				{
					this._doneText = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneText");
				}
			}
		}

		[DataSourceProperty]
		public BannerViewModel BannerVM
		{
			get
			{
				return this._bannerVM;
			}
			set
			{
				if (value != this._bannerVM)
				{
					this._bannerVM = value;
					base.OnPropertyChangedWithValue<BannerViewModel>(value, "BannerVM");
				}
			}
		}

		[DataSourceProperty]
		public string IconCodes
		{
			get
			{
				return this._iconCodes;
			}
			set
			{
				if (value != this._iconCodes)
				{
					this._iconCodes = value;
					base.OnPropertyChangedWithValue<string>(value, "IconCodes");
				}
			}
		}

		[DataSourceProperty]
		public string ColorCodes
		{
			get
			{
				return this._colorCodes;
			}
			set
			{
				if (value != this._colorCodes)
				{
					this._colorCodes = value;
					base.OnPropertyChangedWithValue<string>(value, "ColorCodes");
				}
			}
		}

		[DataSourceProperty]
		public bool CanChangeBackgroundColor
		{
			get
			{
				return this._canChangeBackgroundColor;
			}
			set
			{
				if (value != this._canChangeBackgroundColor)
				{
					this._canChangeBackgroundColor = value;
					base.OnPropertyChangedWithValue(value, "CanChangeBackgroundColor");
				}
			}
		}

		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
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

		[DataSourceProperty]
		public int TotalStageCount
		{
			get
			{
				return this._totalStageCount;
			}
			set
			{
				if (value != this._totalStageCount)
				{
					this._totalStageCount = value;
					base.OnPropertyChangedWithValue(value, "TotalStageCount");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentStageIndex
		{
			get
			{
				return this._currentStageIndex;
			}
			set
			{
				if (value != this._currentStageIndex)
				{
					this._currentStageIndex = value;
					base.OnPropertyChangedWithValue(value, "CurrentStageIndex");
				}
			}
		}

		[DataSourceProperty]
		public int FurthestIndex
		{
			get
			{
				return this._furthestIndex;
			}
			set
			{
				if (value != this._furthestIndex)
				{
					this._furthestIndex = value;
					base.OnPropertyChangedWithValue(value, "FurthestIndex");
				}
			}
		}

		private readonly string _initialBanner;

		public int ShieldSlotIndex = 3;

		public int CurrentShieldIndex;

		public ItemRosterElement ShieldRosterElement;

		private readonly Action<bool> OnExit;

		private readonly Action _refresh;

		private readonly ItemObject _shield;

		private readonly Banner _banner;

		private BannerIconVM _currentSelectedIcon;

		private BannerColorVM _currentSelectedPrimaryColor;

		private BannerColorVM _currentSelectedSigilColor;

		private readonly Action<int> _goToIndex;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private string _iconCodes;

		private string _colorCodes;

		private BannerViewModel _bannerVM;

		private MBBindingList<BannerIconVM> _iconsList;

		private MBBindingList<BannerColorVM> _primaryColorList;

		private MBBindingList<BannerColorVM> _sigilColorList;

		private string _cancelText;

		private string _doneText;

		private string _sizeText;

		private string _primaryColorText;

		private string _sigilColorText;

		private string _currentShieldName;

		private bool _canChangeBackgroundColor;

		private int _currentIconSize;

		private int _minIconSize;

		private int _maxIconSize;

		private HintViewModel _resetHint;

		private HintViewModel _randomizeHint;

		private HintViewModel _undoHint;

		private HintViewModel _redoHint;

		private MBBindingList<HintViewModel> _categoryNames;

		private string _title = "";

		private string _description = "";

		private int _totalStageCount = -1;

		private int _currentStageIndex = -1;

		private int _furthestIndex = -1;

		private bool _initialized;
	}
}
