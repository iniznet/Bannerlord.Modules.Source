using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.BannerEditor;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.BannerBuilder
{
	public class BannerBuilderVM : ViewModel
	{
		public Banner CurrentBanner { get; private set; }

		public BannerBuilderVM(BasicCharacterObject character, string initialKey, Action<bool> onExit, Action refresh, Action copyBannerCode)
		{
			this._character = character;
			this._onExit = onExit;
			this._refresh = refresh;
			this._copyBannerCode = copyBannerCode;
			this.Categories = new MBBindingList<BannerBuilderCategoryVM>();
			this.Layers = new MBBindingList<BannerBuilderLayerVM>();
			this.ColorSelection = new BannerBuilderColorSelectionVM();
			BannerBuilderLayerVM.SetLayerActions(new Action(this.OnRefreshFromLayer), new Action<BannerBuilderLayerVM>(this.OnLayerSelection), new Action<BannerBuilderLayerVM>(this.OnLayerDeletion), new Action<int, Action<BannerBuilderColorItemVM>>(this.OnColorSelection));
			ItemObject itemObject = BannerBuilderVM.FindShield(this._character, "highland_riders_shield");
			if (itemObject != null)
			{
				this.ShieldRosterElement = new ItemRosterElement(itemObject, 1, null);
			}
			this.CurrentBanner = new Banner(initialKey);
			this.PopulateCategories();
			this.PopulateLayers();
			this.OnLayerSelection(this.Layers[0]);
			this.BannerCodeAsString = initialKey;
			this.BannerImageIdentifier = new ImageIdentifierVM(BannerCode.CreateFrom(initialKey), true);
			this.RefreshValues();
			this.IsEditorPreviewActive = true;
			this.IsLayerPreviewActive = true;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Title = new TextObject("{=!}Banner Builder", null).ToString();
			this.CancelText = GameTexts.FindText("str_cancel", null).ToString();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.ResetHint = new HintViewModel(GameTexts.FindText("str_reset_icon", null), null);
			this.RandomizeHint = new HintViewModel(GameTexts.FindText("str_randomize", null), null);
			this.UndoHint = new HintViewModel(GameTexts.FindText("str_undo", null), null);
			this.RedoHint = new HintViewModel(GameTexts.FindText("str_redo", null), null);
			this.DrawStrokeHint = new HintViewModel(new TextObject("{=!}Draw Stroke", null), null);
			this.CenterHint = new HintViewModel(new TextObject("{=!}Align Center", null), null);
			this.ResetSizeHint = new HintViewModel(new TextObject("{=!}Reset Size", null), null);
			this.MirrorHint = new HintViewModel(new TextObject("{=!}Mirror", null), null);
		}

		private void PopulateCategories()
		{
			this.Categories.Clear();
			for (int i = 0; i < BannerManager.Instance.BannerIconGroups.Count; i++)
			{
				BannerIconGroup bannerIconGroup = BannerManager.Instance.BannerIconGroups[i];
				this.Categories.Add(new BannerBuilderCategoryVM(bannerIconGroup, new Action<BannerBuilderItemVM>(this.OnItemSelection)));
			}
		}

		private void PopulateLayers()
		{
			this.Layers.Clear();
			for (int i = 0; i < this.CurrentBanner.BannerDataList.Count; i++)
			{
				this.Layers.Add(new BannerBuilderLayerVM(this.CurrentBanner.BannerDataList[i], i));
			}
		}

		private void OnColorSelection(int selectedColorId, Action<BannerBuilderColorItemVM> onSelection)
		{
			this.ColorSelection.EnableWith(selectedColorId, onSelection);
		}

		private void OnLayerSelection(BannerBuilderLayerVM selectedLayer)
		{
			if (this.CurrentSelectedLayer != null)
			{
				this.CurrentSelectedLayer.IsSelected = false;
			}
			if (this.CurrentSelectedItem != null)
			{
				this.CurrentSelectedItem.IsSelected = false;
			}
			this.CurrentSelectedLayer = selectedLayer;
			if (this.CurrentSelectedLayer != null)
			{
				BannerBuilderItemVM itemFromID = this.GetItemFromID(this.CurrentSelectedLayer.IconID);
				if (itemFromID != null)
				{
					this.CurrentSelectedItem = itemFromID;
					this.CurrentSelectedItem.IsSelected = true;
				}
				this.CurrentSelectedLayer.IsSelected = true;
				bool isPatternLayerSelected = this.CurrentSelectedLayer.LayerIndex == 0;
				this.Categories.ApplyActionOnAllItems(delegate(BannerBuilderCategoryVM c)
				{
					c.IsEnabled = (c.IsPattern ? isPatternLayerSelected : (!isPatternLayerSelected));
				});
				this.UpdateSelectedItem();
			}
		}

		private void OnLayerDeletion(BannerBuilderLayerVM layerToDelete)
		{
			if (layerToDelete == null || layerToDelete.LayerIndex != 0)
			{
				this.CurrentBanner.RemoveIconDataAtIndex(layerToDelete.LayerIndex);
				if (this.CurrentSelectedLayer == layerToDelete)
				{
					this.OnLayerSelection(this.Layers[layerToDelete.LayerIndex - 1]);
				}
				this.Layers.RemoveAt(layerToDelete.LayerIndex);
				this.RefreshLayerIndicies();
				this.Refresh();
			}
		}

		private void OnItemSelection(BannerBuilderItemVM selectedItem)
		{
			if (this.CurrentSelectedLayer != null)
			{
				this.CurrentBanner.BannerDataList[this.CurrentSelectedLayer.LayerIndex].MeshId = selectedItem.MeshID;
				this.UpdateSelectedItem();
				this.CurrentSelectedLayer.Refresh();
				this.Refresh();
			}
		}

		private void UpdateSelectedItem()
		{
			if (this.CurrentSelectedLayer != null)
			{
				int meshId = this.CurrentBanner.BannerDataList[this.CurrentSelectedLayer.LayerIndex].MeshId;
				for (int i = 0; i < this.Categories.Count; i++)
				{
					BannerBuilderCategoryVM bannerBuilderCategoryVM = this.Categories[i];
					for (int j = 0; j < bannerBuilderCategoryVM.ItemsList.Count; j++)
					{
						BannerBuilderItemVM bannerBuilderItemVM = bannerBuilderCategoryVM.ItemsList[j];
						bannerBuilderItemVM.IsSelected = bannerBuilderItemVM.MeshID == meshId;
					}
				}
			}
		}

		public void ExecuteCancel()
		{
			Action<bool> onExit = this._onExit;
			if (onExit == null)
			{
				return;
			}
			onExit(true);
		}

		public void ExecuteDone()
		{
			Action<bool> onExit = this._onExit;
			if (onExit == null)
			{
				return;
			}
			onExit(false);
		}

		public void ExecuteAddDefaultLayer()
		{
			BannerData defaultBannerData = BannerBuilderVM.GetDefaultBannerData();
			this.CurrentBanner.AddIconData(defaultBannerData);
			BannerBuilderLayerVM bannerBuilderLayerVM = new BannerBuilderLayerVM(defaultBannerData, this.Layers.Count);
			this.Layers.Add(bannerBuilderLayerVM);
			this.OnLayerSelection(bannerBuilderLayerVM);
			this.Refresh();
		}

		public void ExecuteDuplicateCurrentLayer()
		{
			BannerBuilderLayerVM currentSelectedLayer = this.CurrentSelectedLayer;
			if (currentSelectedLayer != null && !currentSelectedLayer.IsLayerPattern)
			{
				BannerData bannerData = new BannerData(this.CurrentSelectedLayer.Data);
				this.CurrentBanner.AddIconData(bannerData);
				BannerBuilderLayerVM bannerBuilderLayerVM = new BannerBuilderLayerVM(bannerData, this.Layers.Count);
				this.Layers.Add(bannerBuilderLayerVM);
				this.OnLayerSelection(bannerBuilderLayerVM);
				this.Refresh();
			}
		}

		public void ExecuteCopyBannerCode()
		{
			Action copyBannerCode = this._copyBannerCode;
			if (copyBannerCode == null)
			{
				return;
			}
			copyBannerCode();
		}

		public void ExecuteReorderWithParameters(BannerBuilderLayerVM layer, int index, string targetTag)
		{
			if (layer.IsLayerPattern || index == 0)
			{
				return;
			}
			int num = ((layer.LayerIndex >= index) ? index : (index - 1));
			this.Layers.Remove(layer);
			this.Layers.Insert(num, layer);
			this.CurrentBanner.RemoveIconDataAtIndex(layer.LayerIndex);
			this.CurrentBanner.AddIconData(layer.Data, num);
			this.RefreshLayerIndicies();
			this.OnRefreshFromLayer();
		}

		public void ExecuteReorderToEndWithParameters(BannerBuilderLayerVM layer, int index, string targetTag)
		{
			if (layer.IsLayerPattern)
			{
				return;
			}
			this.ExecuteReorderWithParameters(layer, this.Layers.Count, string.Empty);
		}

		private void OnRefreshFromLayer()
		{
			this.Refresh();
		}

		public string GetBannerCode()
		{
			return this.BannerCodeAsString;
		}

		public void SetBannerCode(string v)
		{
			string bannerCodeAsString = this.BannerCodeAsString;
			try
			{
				this.CurrentBanner.Deserialize(v);
				this.PopulateLayers();
				this.OnLayerSelection(this.Layers[0]);
				this.Refresh();
			}
			catch (Exception)
			{
				InformationManager.DisplayMessage(new InformationMessage("Couldn't parse the clipboard text."));
				this.CurrentBanner.Deserialize(bannerCodeAsString);
				this.PopulateLayers();
				this.OnLayerSelection(this.Layers[0]);
				this.Refresh();
			}
		}

		private void Refresh()
		{
			Action refresh = this._refresh;
			if (refresh != null)
			{
				refresh();
			}
			this.BannerImageIdentifier = new ImageIdentifierVM(BannerCode.CreateFrom(this.BannerCodeAsString), true);
		}

		private BannerBuilderItemVM GetItemFromID(int id)
		{
			for (int i = 0; i < this.Categories.Count; i++)
			{
				BannerBuilderCategoryVM bannerBuilderCategoryVM = this.Categories[i];
				for (int j = 0; j < bannerBuilderCategoryVM.ItemsList.Count; j++)
				{
					BannerBuilderItemVM bannerBuilderItemVM = bannerBuilderCategoryVM.ItemsList[j];
					if (bannerBuilderItemVM.MeshID == id)
					{
						return bannerBuilderItemVM;
					}
				}
			}
			return null;
		}

		private void RefreshLayerIndicies()
		{
			for (int i = 0; i < this.Layers.Count; i++)
			{
				this.Layers[i].SetLayerIndex(i);
			}
		}

		public void TranslateCurrentLayerWith(Vec2 moveDirection)
		{
			this.CurrentSelectedLayer.PositionValueX += moveDirection.x;
			this.CurrentSelectedLayer.PositionValueY += moveDirection.y;
			this.CurrentSelectedLayer.PositionValueX = MathF.Clamp(this.CurrentSelectedLayer.PositionValueX, 0f, 1528f);
			this.CurrentSelectedLayer.PositionValueY = MathF.Clamp(this.CurrentSelectedLayer.PositionValueY, 0f, 1528f);
			this.Refresh();
		}

		public void DeleteCurrentLayer()
		{
			BannerBuilderLayerVM currentSelectedLayer = this.CurrentSelectedLayer;
			if (currentSelectedLayer != null && !currentSelectedLayer.IsLayerPattern)
			{
				this.OnLayerDeletion(this.Layers[this.CurrentSelectedLayer.LayerIndex]);
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			BannerBuilderLayerVM.ResetLayerActions();
		}

		[DataSourceProperty]
		public ImageIdentifierVM BannerImageIdentifier
		{
			get
			{
				return this._bannerImageIdentifier;
			}
			set
			{
				if (value != this._bannerImageIdentifier)
				{
					this._bannerImageIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "BannerImageIdentifier");
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
		public MBBindingList<BannerBuilderCategoryVM> Categories
		{
			get
			{
				return this._categories;
			}
			set
			{
				if (value != this._categories)
				{
					this._categories = value;
					base.OnPropertyChangedWithValue<MBBindingList<BannerBuilderCategoryVM>>(value, "Categories");
				}
			}
		}

		[DataSourceProperty]
		public BannerBuilderColorSelectionVM ColorSelection
		{
			get
			{
				return this._colorSelection;
			}
			set
			{
				if (value != this._colorSelection)
				{
					this._colorSelection = value;
					base.OnPropertyChangedWithValue<BannerBuilderColorSelectionVM>(value, "ColorSelection");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<BannerBuilderLayerVM> Layers
		{
			get
			{
				return this._layers;
			}
			set
			{
				if (value != this._layers)
				{
					this._layers = value;
					base.OnPropertyChangedWithValue<MBBindingList<BannerBuilderLayerVM>>(value, "Layers");
				}
			}
		}

		[DataSourceProperty]
		public BannerBuilderLayerVM CurrentSelectedLayer
		{
			get
			{
				return this._currentSelectedLayer;
			}
			set
			{
				if (value != this._currentSelectedLayer)
				{
					this._currentSelectedLayer = value;
					base.OnPropertyChangedWithValue<BannerBuilderLayerVM>(value, "CurrentSelectedLayer");
				}
			}
		}

		[DataSourceProperty]
		public BannerBuilderItemVM CurrentSelectedItem
		{
			get
			{
				return this._currentSelectedItem;
			}
			set
			{
				if (value != this._currentSelectedItem)
				{
					this._currentSelectedItem = value;
					base.OnPropertyChangedWithValue<BannerBuilderItemVM>(value, "CurrentSelectedItem");
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
		public HintViewModel DrawStrokeHint
		{
			get
			{
				return this._drawStrokeHint;
			}
			set
			{
				if (value != this._drawStrokeHint)
				{
					this._drawStrokeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DrawStrokeHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel CenterHint
		{
			get
			{
				return this._centerHint;
			}
			set
			{
				if (value != this._centerHint)
				{
					this._centerHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CenterHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ResetSizeHint
		{
			get
			{
				return this._resetSizeHint;
			}
			set
			{
				if (value != this._resetSizeHint)
				{
					this._resetSizeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ResetSizeHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel MirrorHint
		{
			get
			{
				return this._mirrorHint;
			}
			set
			{
				if (value != this._mirrorHint)
				{
					this._mirrorHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "MirrorHint");
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
		public string BannerCodeAsString
		{
			get
			{
				return this._bannerCodeAsString;
			}
			set
			{
				if (value != this._bannerCodeAsString)
				{
					this._bannerCodeAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "BannerCodeAsString");
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
		public bool IsBannerPreviewsActive
		{
			get
			{
				return this._isBannerPreviewsActive;
			}
			set
			{
				if (value != this._isBannerPreviewsActive)
				{
					this._isBannerPreviewsActive = value;
					base.OnPropertyChangedWithValue(value, "IsBannerPreviewsActive");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEditorPreviewActive
		{
			get
			{
				return this._isEditorPreviewActive;
			}
			set
			{
				if (value != this._isEditorPreviewActive)
				{
					this._isEditorPreviewActive = value;
					base.OnPropertyChangedWithValue(value, "IsEditorPreviewActive");
				}
			}
		}

		[DataSourceProperty]
		public bool IsLayerPreviewActive
		{
			get
			{
				return this._isLayerPreviewActive;
			}
			set
			{
				if (value != this._isLayerPreviewActive)
				{
					this._isLayerPreviewActive = value;
					base.OnPropertyChangedWithValue(value, "IsLayerPreviewActive");
				}
			}
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

		private static ItemObject FindShield(BasicCharacterObject character, string desiredShieldID = "")
		{
			for (int i = 0; i < 4; i++)
			{
				EquipmentElement equipmentFromSlot = character.Equipment.GetEquipmentFromSlot((EquipmentIndex)i);
				ItemObject item = equipmentFromSlot.Item;
				if (((item != null) ? item.PrimaryWeapon : null) != null && equipmentFromSlot.Item.PrimaryWeapon.IsShield && equipmentFromSlot.Item.IsUsingTableau)
				{
					return equipmentFromSlot.Item;
				}
			}
			if (!string.IsNullOrEmpty(desiredShieldID))
			{
				ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(desiredShieldID);
				if (@object != null)
				{
					WeaponComponentData primaryWeapon = @object.PrimaryWeapon;
					if (primaryWeapon != null && primaryWeapon.IsShield)
					{
						return @object;
					}
				}
			}
			MBReadOnlyList<ItemObject> objectTypeList = Game.Current.ObjectManager.GetObjectTypeList<ItemObject>();
			for (int j = 0; j < objectTypeList.Count; j++)
			{
				ItemObject itemObject = objectTypeList[j];
				WeaponComponentData primaryWeapon2 = itemObject.PrimaryWeapon;
				if (primaryWeapon2 != null && primaryWeapon2.IsShield && itemObject.IsUsingTableau)
				{
					return itemObject;
				}
			}
			return null;
		}

		private static BannerData GetDefaultBannerData()
		{
			return new BannerData(133, 171, 171, new Vec2(483f, 483f), new Vec2(764f, 764f), false, false, 0f);
		}

		private const int PatternLayerIndex = 0;

		public int ShieldSlotIndex = 3;

		public int CurrentShieldIndex;

		public ItemRosterElement ShieldRosterElement;

		private readonly BasicCharacterObject _character;

		private readonly Action<bool> _onExit;

		private readonly Action _refresh;

		private readonly Action _copyBannerCode;

		private ImageIdentifierVM _bannerImageIdentifier;

		private string _iconCodes;

		private string _colorCodes;

		private string _bannerCodeAsString;

		private BannerViewModel _bannerVM;

		private MBBindingList<BannerBuilderCategoryVM> _categories;

		private MBBindingList<BannerBuilderLayerVM> _layers;

		private BannerBuilderLayerVM _currentSelectedLayer;

		private BannerBuilderItemVM _currentSelectedItem;

		private BannerBuilderColorSelectionVM _colorSelection;

		private string _title;

		private string _cancelText;

		private string _doneText;

		private string _currentShieldName;

		private bool _canChangeBackgroundColor;

		private bool _isBannerPreviewsActive;

		private bool _isEditorPreviewActive;

		private bool _isLayerPreviewActive;

		private int _minIconSize;

		private int _maxIconSize;

		private HintViewModel _resetHint;

		private HintViewModel _randomizeHint;

		private HintViewModel _undoHint;

		private HintViewModel _redoHint;

		private HintViewModel _drawStrokeHint;

		private HintViewModel _centerHint;

		private HintViewModel _resetSizeHint;

		private HintViewModel _mirrorHint;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;
	}
}
