using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	// Token: 0x0200005F RID: 95
	public class PartyUpgradeButtonWidget : ButtonWidget
	{
		// Token: 0x06000516 RID: 1302 RVA: 0x0000F8AB File Offset: 0x0000DAAB
		public PartyUpgradeButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x0000F8B4 File Offset: 0x0000DAB4
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

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000518 RID: 1304 RVA: 0x0000F9E7 File Offset: 0x0000DBE7
		// (set) Token: 0x06000519 RID: 1305 RVA: 0x0000F9EF File Offset: 0x0000DBEF
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

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x0600051A RID: 1306 RVA: 0x0000FA0D File Offset: 0x0000DC0D
		// (set) Token: 0x0600051B RID: 1307 RVA: 0x0000FA15 File Offset: 0x0000DC15
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

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x0600051C RID: 1308 RVA: 0x0000FA33 File Offset: 0x0000DC33
		// (set) Token: 0x0600051D RID: 1309 RVA: 0x0000FA3B File Offset: 0x0000DC3B
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

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x0600051E RID: 1310 RVA: 0x0000FA59 File Offset: 0x0000DC59
		// (set) Token: 0x0600051F RID: 1311 RVA: 0x0000FA61 File Offset: 0x0000DC61
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

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000520 RID: 1312 RVA: 0x0000FA7F File Offset: 0x0000DC7F
		// (set) Token: 0x06000521 RID: 1313 RVA: 0x0000FA87 File Offset: 0x0000DC87
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

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000522 RID: 1314 RVA: 0x0000FAAB File Offset: 0x0000DCAB
		// (set) Token: 0x06000523 RID: 1315 RVA: 0x0000FAB3 File Offset: 0x0000DCB3
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

		// Token: 0x04000233 RID: 563
		private ImageIdentifierWidget _imageIdentifierWidget;

		// Token: 0x04000234 RID: 564
		private Brush _defaultBrush;

		// Token: 0x04000235 RID: 565
		private Brush _unavailableBrush;

		// Token: 0x04000236 RID: 566
		private Brush _insufficientBrush;

		// Token: 0x04000237 RID: 567
		private bool _isAvailable;

		// Token: 0x04000238 RID: 568
		private bool _isInsufficient;
	}
}
