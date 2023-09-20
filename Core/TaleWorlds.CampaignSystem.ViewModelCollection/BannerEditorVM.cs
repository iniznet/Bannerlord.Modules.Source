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
	// Token: 0x02000004 RID: 4
	public class BannerEditorVM : ViewModel
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
		public BasicCharacterObject Character { get; }

		// Token: 0x06000004 RID: 4 RVA: 0x00002060 File Offset: 0x00000260
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

		// Token: 0x06000005 RID: 5 RVA: 0x0000218C File Offset: 0x0000038C
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

		// Token: 0x06000006 RID: 6 RVA: 0x00002550 File Offset: 0x00000750
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

		// Token: 0x06000007 RID: 7 RVA: 0x000025D8 File Offset: 0x000007D8
		public void SetClanRelatedRules(bool canChangeBackgroundColor)
		{
			this.CanChangeBackgroundColor = canChangeBackgroundColor;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000025E4 File Offset: 0x000007E4
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

		// Token: 0x06000009 RID: 9 RVA: 0x00002638 File Offset: 0x00000838
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

		// Token: 0x0600000A RID: 10 RVA: 0x00002738 File Offset: 0x00000938
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

		// Token: 0x0600000B RID: 11 RVA: 0x000027A0 File Offset: 0x000009A0
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

		// Token: 0x0600000C RID: 12 RVA: 0x000027F4 File Offset: 0x000009F4
		private void OnBannerIconSizeChange(int newSize)
		{
			this.BannerVM.SetIconSize(newSize);
			this._refresh();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000280D File Offset: 0x00000A0D
		public void ExecuteDone()
		{
			this.OnExit(false);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000281B File Offset: 0x00000A1B
		public void ExecuteCancel()
		{
			this._banner.Deserialize(this._initialBanner);
			this.OnExit(true);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000283C File Offset: 0x00000A3C
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

		// Token: 0x06000010 RID: 16 RVA: 0x00002914 File Offset: 0x00000B14
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

		// Token: 0x06000011 RID: 17 RVA: 0x0000293D File Offset: 0x00000B3D
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000294C File Offset: 0x00000B4C
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000295B File Offset: 0x00000B5B
		// (set) Token: 0x06000014 RID: 20 RVA: 0x00002963 File Offset: 0x00000B63
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

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002981 File Offset: 0x00000B81
		// (set) Token: 0x06000016 RID: 22 RVA: 0x00002989 File Offset: 0x00000B89
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

		// Token: 0x06000017 RID: 23 RVA: 0x000029A7 File Offset: 0x00000BA7
		public void ExecuteGoToIndex(int index)
		{
			this._goToIndex(index);
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000018 RID: 24 RVA: 0x000029B5 File Offset: 0x00000BB5
		// (set) Token: 0x06000019 RID: 25 RVA: 0x000029BD File Offset: 0x00000BBD
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

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001A RID: 26 RVA: 0x000029DB File Offset: 0x00000BDB
		// (set) Token: 0x0600001B RID: 27 RVA: 0x000029E3 File Offset: 0x00000BE3
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

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002A01 File Offset: 0x00000C01
		// (set) Token: 0x0600001D RID: 29 RVA: 0x00002A09 File Offset: 0x00000C09
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

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002A27 File Offset: 0x00000C27
		// (set) Token: 0x0600001F RID: 31 RVA: 0x00002A2F File Offset: 0x00000C2F
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

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002A4D File Offset: 0x00000C4D
		// (set) Token: 0x06000021 RID: 33 RVA: 0x00002A55 File Offset: 0x00000C55
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

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00002A73 File Offset: 0x00000C73
		// (set) Token: 0x06000023 RID: 35 RVA: 0x00002A7B File Offset: 0x00000C7B
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

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00002A99 File Offset: 0x00000C99
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002AA1 File Offset: 0x00000CA1
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

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002ABF File Offset: 0x00000CBF
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00002AC7 File Offset: 0x00000CC7
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

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002AE5 File Offset: 0x00000CE5
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00002AED File Offset: 0x00000CED
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

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002B10 File Offset: 0x00000D10
		// (set) Token: 0x0600002B RID: 43 RVA: 0x00002B18 File Offset: 0x00000D18
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

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00002B36 File Offset: 0x00000D36
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00002B3E File Offset: 0x00000D3E
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

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002B5C File Offset: 0x00000D5C
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002B64 File Offset: 0x00000D64
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

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002B91 File Offset: 0x00000D91
		// (set) Token: 0x06000031 RID: 49 RVA: 0x00002B99 File Offset: 0x00000D99
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

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002BBC File Offset: 0x00000DBC
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002BC4 File Offset: 0x00000DC4
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

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002BE7 File Offset: 0x00000DE7
		// (set) Token: 0x06000035 RID: 53 RVA: 0x00002BEF File Offset: 0x00000DEF
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

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002C12 File Offset: 0x00000E12
		// (set) Token: 0x06000037 RID: 55 RVA: 0x00002C1A File Offset: 0x00000E1A
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

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002C3D File Offset: 0x00000E3D
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002C45 File Offset: 0x00000E45
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

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002C68 File Offset: 0x00000E68
		// (set) Token: 0x0600003B RID: 59 RVA: 0x00002C70 File Offset: 0x00000E70
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

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00002C8E File Offset: 0x00000E8E
		// (set) Token: 0x0600003D RID: 61 RVA: 0x00002C96 File Offset: 0x00000E96
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

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00002CB9 File Offset: 0x00000EB9
		// (set) Token: 0x0600003F RID: 63 RVA: 0x00002CC1 File Offset: 0x00000EC1
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

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00002CE4 File Offset: 0x00000EE4
		// (set) Token: 0x06000041 RID: 65 RVA: 0x00002CEC File Offset: 0x00000EEC
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

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00002D0A File Offset: 0x00000F0A
		// (set) Token: 0x06000043 RID: 67 RVA: 0x00002D12 File Offset: 0x00000F12
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

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00002D35 File Offset: 0x00000F35
		// (set) Token: 0x06000045 RID: 69 RVA: 0x00002D3D File Offset: 0x00000F3D
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

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00002D60 File Offset: 0x00000F60
		// (set) Token: 0x06000047 RID: 71 RVA: 0x00002D68 File Offset: 0x00000F68
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

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00002D86 File Offset: 0x00000F86
		// (set) Token: 0x06000049 RID: 73 RVA: 0x00002D8E File Offset: 0x00000F8E
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

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00002DAC File Offset: 0x00000FAC
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002DB4 File Offset: 0x00000FB4
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

		// Token: 0x04000001 RID: 1
		private readonly string _initialBanner;

		// Token: 0x04000002 RID: 2
		public int ShieldSlotIndex = 3;

		// Token: 0x04000003 RID: 3
		public int CurrentShieldIndex;

		// Token: 0x04000005 RID: 5
		public ItemRosterElement ShieldRosterElement;

		// Token: 0x04000006 RID: 6
		private readonly Action<bool> OnExit;

		// Token: 0x04000007 RID: 7
		private readonly Action _refresh;

		// Token: 0x04000008 RID: 8
		private readonly ItemObject _shield;

		// Token: 0x04000009 RID: 9
		private readonly Banner _banner;

		// Token: 0x0400000A RID: 10
		private BannerIconVM _currentSelectedIcon;

		// Token: 0x0400000B RID: 11
		private BannerColorVM _currentSelectedPrimaryColor;

		// Token: 0x0400000C RID: 12
		private BannerColorVM _currentSelectedSigilColor;

		// Token: 0x0400000D RID: 13
		private readonly Action<int> _goToIndex;

		// Token: 0x0400000E RID: 14
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x0400000F RID: 15
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000010 RID: 16
		private string _iconCodes;

		// Token: 0x04000011 RID: 17
		private string _colorCodes;

		// Token: 0x04000012 RID: 18
		private BannerViewModel _bannerVM;

		// Token: 0x04000013 RID: 19
		private MBBindingList<BannerIconVM> _iconsList;

		// Token: 0x04000014 RID: 20
		private MBBindingList<BannerColorVM> _primaryColorList;

		// Token: 0x04000015 RID: 21
		private MBBindingList<BannerColorVM> _sigilColorList;

		// Token: 0x04000016 RID: 22
		private string _cancelText;

		// Token: 0x04000017 RID: 23
		private string _doneText;

		// Token: 0x04000018 RID: 24
		private string _sizeText;

		// Token: 0x04000019 RID: 25
		private string _primaryColorText;

		// Token: 0x0400001A RID: 26
		private string _sigilColorText;

		// Token: 0x0400001B RID: 27
		private string _currentShieldName;

		// Token: 0x0400001C RID: 28
		private bool _canChangeBackgroundColor;

		// Token: 0x0400001D RID: 29
		private int _currentIconSize;

		// Token: 0x0400001E RID: 30
		private int _minIconSize;

		// Token: 0x0400001F RID: 31
		private int _maxIconSize;

		// Token: 0x04000020 RID: 32
		private HintViewModel _resetHint;

		// Token: 0x04000021 RID: 33
		private HintViewModel _randomizeHint;

		// Token: 0x04000022 RID: 34
		private HintViewModel _undoHint;

		// Token: 0x04000023 RID: 35
		private HintViewModel _redoHint;

		// Token: 0x04000024 RID: 36
		private MBBindingList<HintViewModel> _categoryNames;

		// Token: 0x04000025 RID: 37
		private string _title = "";

		// Token: 0x04000026 RID: 38
		private string _description = "";

		// Token: 0x04000027 RID: 39
		private int _totalStageCount = -1;

		// Token: 0x04000028 RID: 40
		private int _currentStageIndex = -1;

		// Token: 0x04000029 RID: 41
		private int _furthestIndex = -1;

		// Token: 0x0400002A RID: 42
		private bool _initialized;
	}
}
