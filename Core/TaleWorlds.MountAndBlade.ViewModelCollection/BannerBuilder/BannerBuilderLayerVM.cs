using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.BannerBuilder
{
	public class BannerBuilderLayerVM : ViewModel
	{
		public BannerData Data { get; private set; }

		public BannerBuilderLayerVM(BannerData data, int layerIndex)
		{
			this.Data = data;
			this.LayerIndex = layerIndex;
			this._rotationValue = this.Data.Rotation;
			this._positionValue = this.Data.Position;
			this._sizeValue = this.Data.Size;
			this._isDrawStrokeActive = this.Data.DrawStroke;
			this._isMirrorActive = this.Data.Mirror;
			this.Refresh();
			this.IsLayerPattern = layerIndex == 0;
			this.CanDeleteLayer = !this.IsLayerPattern;
			this.TotalAreaSize = 1528;
			this.EditableAreaSize = 512;
		}

		public void Refresh()
		{
			this.IconID = this.Data.MeshId;
			this.IconIDAsString = this.IconID.ToString();
			uint color = BannerManager.Instance.ReadOnlyColorPalette[this.Data.ColorId].Color;
			this.Color1 = Color.FromUint(color);
			uint color2 = BannerManager.Instance.ReadOnlyColorPalette[this.Data.ColorId2].Color;
			this.Color2 = Color.FromUint(color2);
		}

		public void ExecuteDelete()
		{
			Action<BannerBuilderLayerVM> onDeletion = BannerBuilderLayerVM._onDeletion;
			if (onDeletion == null)
			{
				return;
			}
			onDeletion(this);
		}

		public void ExecuteSelection()
		{
			Action<BannerBuilderLayerVM> onSelection = BannerBuilderLayerVM._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		public void SetLayerIndex(int newIndex)
		{
			this.LayerIndex = newIndex;
		}

		public void ExecuteSelectColor1()
		{
			Action<int, Action<BannerBuilderColorItemVM>> onColorSelection = BannerBuilderLayerVM._onColorSelection;
			if (onColorSelection == null)
			{
				return;
			}
			onColorSelection(this.Data.ColorId, new Action<BannerBuilderColorItemVM>(this.OnSelectColor1));
		}

		private void OnSelectColor1(BannerBuilderColorItemVM selectedColor)
		{
			this.Data.ColorId = selectedColor.ColorID;
			this.Color1 = Color.FromUint(selectedColor.BannerColor.Color);
			this.ExecuteUpdateBanner();
		}

		public void ExecuteSelectColor2()
		{
			Action<int, Action<BannerBuilderColorItemVM>> onColorSelection = BannerBuilderLayerVM._onColorSelection;
			if (onColorSelection == null)
			{
				return;
			}
			onColorSelection(this.Data.ColorId2, new Action<BannerBuilderColorItemVM>(this.OnSelectColor2));
		}

		private void OnSelectColor2(BannerBuilderColorItemVM selectedColor)
		{
			this.Data.ColorId2 = selectedColor.ColorID;
			this.Color2 = Color.FromUint(selectedColor.BannerColor.Color);
			this.ExecuteUpdateBanner();
		}

		public void ExecuteSwapColors()
		{
			int colorId = this.Data.ColorId2;
			this.Data.ColorId2 = this.Data.ColorId;
			this.Data.ColorId = colorId;
			Color color = this.Color2;
			Color color2 = this.Color1;
			this.Color1 = color;
			this.Color2 = color2;
			this.Refresh();
			this.ExecuteUpdateBanner();
		}

		public void ExecuteCenterSigil()
		{
			this.PositionValue = new Vec2((float)this.TotalAreaSize / 2f, (float)this.TotalAreaSize / 2f);
			this.ExecuteUpdateBanner();
		}

		public void ExecuteResetSize()
		{
			float num = (float)(this.IsLayerPattern ? this.TotalAreaSize : 483);
			this.SizeValue = new Vec2(num, num);
			this.ExecuteUpdateBanner();
		}

		public void ExecuteUpdateBanner()
		{
			Action refresh = BannerBuilderLayerVM._refresh;
			if (refresh == null)
			{
				return;
			}
			refresh();
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
				}
			}
		}

		[DataSourceProperty]
		public bool CanDeleteLayer
		{
			get
			{
				return this._canDeleteLayer;
			}
			set
			{
				if (value != this._canDeleteLayer)
				{
					this._canDeleteLayer = value;
					base.OnPropertyChangedWithValue(value, "CanDeleteLayer");
				}
			}
		}

		[DataSourceProperty]
		public bool IsLayerPattern
		{
			get
			{
				return this._isLayerPattern;
			}
			set
			{
				if (value != this._isLayerPattern)
				{
					this._isLayerPattern = value;
					base.OnPropertyChangedWithValue(value, "IsLayerPattern");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDrawStrokeActive
		{
			get
			{
				return this._isDrawStrokeActive;
			}
			set
			{
				if (value != this._isDrawStrokeActive)
				{
					this._isDrawStrokeActive = value;
					base.OnPropertyChangedWithValue(value, "IsDrawStrokeActive");
					this.Data.DrawStroke = value;
					this.ExecuteUpdateBanner();
				}
			}
		}

		[DataSourceProperty]
		public bool IsMirrorActive
		{
			get
			{
				return this._isMirrorActive;
			}
			set
			{
				if (value != this._isMirrorActive)
				{
					this._isMirrorActive = value;
					base.OnPropertyChangedWithValue(value, "IsMirrorActive");
					this.Data.Mirror = value;
					this.ExecuteUpdateBanner();
				}
			}
		}

		[DataSourceProperty]
		public float RotationValue
		{
			get
			{
				return this._rotationValue;
			}
			set
			{
				if (value != this._rotationValue)
				{
					this._rotationValue = value;
					this.Data.RotationValue = value;
					base.OnPropertyChangedWithValue(value, "RotationValue");
					base.OnPropertyChanged("RotationValue360");
				}
			}
		}

		[DataSourceProperty]
		public int RotationValue360
		{
			get
			{
				return (int)(this._rotationValue * 360f);
			}
			set
			{
				if (value != (int)(this._rotationValue * 360f))
				{
					this.RotationValue = (float)value / 360f;
					base.OnPropertyChangedWithValue(value, "RotationValue360");
				}
			}
		}

		[DataSourceProperty]
		public int IconID
		{
			get
			{
				return this._iconID;
			}
			set
			{
				if (value != this._iconID)
				{
					this._iconID = value;
					base.OnPropertyChangedWithValue(value, "IconID");
				}
			}
		}

		[DataSourceProperty]
		public int LayerIndex
		{
			get
			{
				return this._layerIndex;
			}
			set
			{
				if (value != this._layerIndex)
				{
					this._layerIndex = value;
					base.OnPropertyChangedWithValue(value, "LayerIndex");
				}
			}
		}

		[DataSourceProperty]
		public int EditableAreaSize
		{
			get
			{
				return this._editableAreaSize;
			}
			set
			{
				if (value != this._editableAreaSize)
				{
					this._editableAreaSize = value;
					base.OnPropertyChangedWithValue(value, "EditableAreaSize");
				}
			}
		}

		[DataSourceProperty]
		public int TotalAreaSize
		{
			get
			{
				return this._totalAreaSize;
			}
			set
			{
				if (value != this._totalAreaSize)
				{
					this._totalAreaSize = value;
					base.OnPropertyChangedWithValue(value, "TotalAreaSize");
				}
			}
		}

		[DataSourceProperty]
		public string IconIDAsString
		{
			get
			{
				return this._iconIDAsString;
			}
			set
			{
				if (value != this._iconIDAsString)
				{
					this._iconIDAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "IconIDAsString");
				}
			}
		}

		[DataSourceProperty]
		public Color Color1
		{
			get
			{
				return this._color1;
			}
			set
			{
				if (value != this._color1)
				{
					this._color1 = value;
					base.OnPropertyChangedWithValue(value, "Color1");
					this.Color1AsStr = value.ToString();
				}
			}
		}

		[DataSourceProperty]
		public Color Color2
		{
			get
			{
				return this._color2;
			}
			set
			{
				if (value != this._color2)
				{
					this._color2 = value;
					base.OnPropertyChangedWithValue(value, "Color2");
					this.Color2AsStr = value.ToString();
				}
			}
		}

		[DataSourceProperty]
		public string Color1AsStr
		{
			get
			{
				return this._color1AsStr;
			}
			set
			{
				if (value != this._color1AsStr)
				{
					this._color1AsStr = value;
					base.OnPropertyChangedWithValue<string>(value, "Color1AsStr");
				}
			}
		}

		[DataSourceProperty]
		public string Color2AsStr
		{
			get
			{
				return this._color2AsStr;
			}
			set
			{
				if (value != this._color2AsStr)
				{
					this._color2AsStr = value;
					base.OnPropertyChangedWithValue<string>(value, "Color2AsStr");
				}
			}
		}

		[DataSourceProperty]
		public Vec2 PositionValue
		{
			get
			{
				return this._positionValue;
			}
			set
			{
				if (this._positionValue != value)
				{
					this._positionValue = value;
					base.OnPropertyChangedWithValue(value, "PositionValue");
					base.OnPropertyChanged("PositionValueX");
					base.OnPropertyChanged("PositionValueY");
					this.Data.Position = value;
				}
			}
		}

		[DataSourceProperty]
		public float PositionValueX
		{
			get
			{
				return (float)Math.Round((double)this._positionValue.X);
			}
			set
			{
				value = (float)Math.Round((double)value);
				if (value != this._positionValue.X)
				{
					this.PositionValue = new Vec2(value, this._positionValue.Y);
					this.Data.Position = this._positionValue;
					base.OnPropertyChangedWithValue(value, "PositionValueX");
				}
			}
		}

		[DataSourceProperty]
		public float PositionValueY
		{
			get
			{
				return (float)Math.Round((double)this._positionValue.Y);
			}
			set
			{
				value = (float)Math.Round((double)value);
				if (value != this._positionValue.Y)
				{
					this.PositionValue = new Vec2(this._positionValue.X, value);
					this.Data.Position = this._positionValue;
					base.OnPropertyChangedWithValue(value, "PositionValueY");
				}
			}
		}

		[DataSourceProperty]
		public Vec2 SizeValue
		{
			get
			{
				return this._sizeValue;
			}
			set
			{
				if (this._sizeValue != value)
				{
					this._sizeValue = value;
					base.OnPropertyChangedWithValue(value, "SizeValue");
					base.OnPropertyChanged("SizeValueX");
					base.OnPropertyChanged("SizeValueY");
					this.Data.Size = value;
				}
			}
		}

		[DataSourceProperty]
		public float SizeValueX
		{
			get
			{
				return (float)Math.Round((double)this._sizeValue.X);
			}
			set
			{
				value = (float)Math.Round((double)value);
				if (value != this._sizeValue.X)
				{
					this.SizeValue = new Vec2(value, this._sizeValue.Y);
					this.Data.Size = this._sizeValue;
					base.OnPropertyChangedWithValue(value, "SizeValueX");
				}
			}
		}

		[DataSourceProperty]
		public float SizeValueY
		{
			get
			{
				return (float)Math.Round((double)this._sizeValue.Y);
			}
			set
			{
				value = (float)Math.Round((double)value);
				if (value != this._sizeValue.Y)
				{
					this.SizeValue = new Vec2(this._sizeValue.X, value);
					this.Data.Size = this._sizeValue;
					base.OnPropertyChangedWithValue(value, "SizeValueY");
				}
			}
		}

		public static void SetLayerActions(Action refresh, Action<BannerBuilderLayerVM> onSelection, Action<BannerBuilderLayerVM> onDeletion, Action<int, Action<BannerBuilderColorItemVM>> onColorSelection)
		{
			BannerBuilderLayerVM._onSelection = onSelection;
			BannerBuilderLayerVM._onDeletion = onDeletion;
			BannerBuilderLayerVM._onColorSelection = onColorSelection;
			BannerBuilderLayerVM._refresh = refresh;
		}

		public static void ResetLayerActions()
		{
			BannerBuilderLayerVM._onSelection = null;
			BannerBuilderLayerVM._onDeletion = null;
			BannerBuilderLayerVM._onColorSelection = null;
			BannerBuilderLayerVM._refresh = null;
		}

		private static Action<BannerBuilderLayerVM> _onSelection;

		private static Action<BannerBuilderLayerVM> _onDeletion;

		private static Action<int, Action<BannerBuilderColorItemVM>> _onColorSelection;

		private static Action _refresh;

		private int _iconID;

		private string _iconIDAsString;

		private Color _color1;

		private Color _color2;

		private string _color1AsStr;

		private string _color2AsStr;

		private bool _isSelected;

		private bool _canDeleteLayer;

		private bool _isLayerPattern;

		private bool _isDrawStrokeActive;

		private bool _isMirrorActive;

		private int _editableAreaSize;

		private int _totalAreaSize;

		private int _layerIndex;

		private float _rotationValue;

		private Vec2 _positionValue;

		private Vec2 _sizeValue;
	}
}
