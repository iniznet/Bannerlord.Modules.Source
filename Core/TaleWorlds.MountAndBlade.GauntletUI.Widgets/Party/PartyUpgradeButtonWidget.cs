using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	public class PartyUpgradeButtonWidget : ButtonWidget
	{
		public PartyUpgradeButtonWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateVisual()
		{
			if (this.ImageIdentifierWidget == null || this.UnavailableBrush == null || this.InsufficientBrush == null)
			{
				return;
			}
			if (!this.IsAvailable)
			{
				this.ImageIdentifierWidget.Brush.GlobalColor = new Color(1f, 1f, 1f, 1f);
				this.ImageIdentifierWidget.Brush.SaturationFactor = -100f;
				base.IsEnabled = true;
				base.Brush = this.UnavailableBrush;
				return;
			}
			if (this.IsAvailable && this.IsInsufficient)
			{
				this.ImageIdentifierWidget.Brush.GlobalColor = new Color(0.9f, 0.5f, 0.5f, 1f);
				this.ImageIdentifierWidget.Brush.SaturationFactor = -150f;
				base.IsEnabled = true;
				base.Brush = this.InsufficientBrush;
				return;
			}
			this.ImageIdentifierWidget.Brush.GlobalColor = new Color(1f, 1f, 1f, 1f);
			this.ImageIdentifierWidget.Brush.SaturationFactor = 0f;
			base.IsEnabled = true;
			base.Brush = this.DefaultBrush;
		}

		[Editor(false)]
		public ImageIdentifierWidget ImageIdentifierWidget
		{
			get
			{
				return this._imageIdentifierWidget;
			}
			set
			{
				if (this._imageIdentifierWidget != value)
				{
					this._imageIdentifierWidget = value;
					base.OnPropertyChanged<ImageIdentifierWidget>(value, "ImageIdentifierWidget");
				}
			}
		}

		[Editor(false)]
		public Brush DefaultBrush
		{
			get
			{
				return this._defaultBrush;
			}
			set
			{
				if (this._defaultBrush != value)
				{
					this._defaultBrush = value;
					base.OnPropertyChanged<Brush>(value, "DefaultBrush");
				}
			}
		}

		[Editor(false)]
		public Brush UnavailableBrush
		{
			get
			{
				return this._unavailableBrush;
			}
			set
			{
				if (this._unavailableBrush != value)
				{
					this._unavailableBrush = value;
					base.OnPropertyChanged<Brush>(value, "UnavailableBrush");
				}
			}
		}

		[Editor(false)]
		public Brush InsufficientBrush
		{
			get
			{
				return this._insufficientBrush;
			}
			set
			{
				if (this._insufficientBrush != value)
				{
					this._insufficientBrush = value;
					base.OnPropertyChanged<Brush>(value, "InsufficientBrush");
				}
			}
		}

		[Editor(false)]
		public bool IsAvailable
		{
			get
			{
				return this._isAvailable;
			}
			set
			{
				if (this._isAvailable != value)
				{
					this._isAvailable = value;
					base.OnPropertyChanged(value, "IsAvailable");
				}
				this.UpdateVisual();
			}
		}

		[Editor(false)]
		public bool IsInsufficient
		{
			get
			{
				return this._isInsufficient;
			}
			set
			{
				if (this._isInsufficient != value)
				{
					this._isInsufficient = value;
					base.OnPropertyChanged(value, "IsInsufficient");
				}
				this.UpdateVisual();
			}
		}

		private ImageIdentifierWidget _imageIdentifierWidget;

		private Brush _defaultBrush;

		private Brush _unavailableBrush;

		private Brush _insufficientBrush;

		private bool _isAvailable;

		private bool _isInsufficient;
	}
}
