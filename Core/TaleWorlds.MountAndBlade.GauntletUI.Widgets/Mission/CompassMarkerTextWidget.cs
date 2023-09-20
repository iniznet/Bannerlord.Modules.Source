using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000C6 RID: 198
	public class CompassMarkerTextWidget : TextWidget
	{
		// Token: 0x060009F2 RID: 2546 RVA: 0x0001C5ED File Offset: 0x0001A7ED
		public CompassMarkerTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x0001C5F6 File Offset: 0x0001A7F6
		private void UpdateBrush()
		{
			if (this.PrimaryBrush != null && this.SecondaryBrush != null)
			{
				base.Brush = (this.IsPrimary ? this.PrimaryBrush : this.SecondaryBrush);
			}
		}

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x060009F4 RID: 2548 RVA: 0x0001C624 File Offset: 0x0001A824
		// (set) Token: 0x060009F5 RID: 2549 RVA: 0x0001C62C File Offset: 0x0001A82C
		public bool IsPrimary
		{
			get
			{
				return this._isPrimary;
			}
			set
			{
				if (this._isPrimary != value)
				{
					this._isPrimary = value;
					base.OnPropertyChanged(value, "IsPrimary");
					this.UpdateBrush();
				}
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x060009F6 RID: 2550 RVA: 0x0001C650 File Offset: 0x0001A850
		// (set) Token: 0x060009F7 RID: 2551 RVA: 0x0001C658 File Offset: 0x0001A858
		public float Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (Math.Abs(this._position - value) > 1E-45f)
				{
					this._position = value;
					base.OnPropertyChanged(value, "Position");
				}
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x060009F8 RID: 2552 RVA: 0x0001C681 File Offset: 0x0001A881
		// (set) Token: 0x060009F9 RID: 2553 RVA: 0x0001C689 File Offset: 0x0001A889
		public Brush PrimaryBrush
		{
			get
			{
				return this._primaryBrush;
			}
			set
			{
				if (this._primaryBrush != value)
				{
					this._primaryBrush = value;
					base.OnPropertyChanged<Brush>(value, "PrimaryBrush");
					this.UpdateBrush();
				}
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x060009FA RID: 2554 RVA: 0x0001C6AD File Offset: 0x0001A8AD
		// (set) Token: 0x060009FB RID: 2555 RVA: 0x0001C6B5 File Offset: 0x0001A8B5
		public Brush SecondaryBrush
		{
			get
			{
				return this._secondaryBrush;
			}
			set
			{
				if (this._secondaryBrush != value)
				{
					this._secondaryBrush = value;
					base.OnPropertyChanged<Brush>(value, "SecondaryBrush");
					this.UpdateBrush();
				}
			}
		}

		// Token: 0x0400048B RID: 1163
		private bool _isPrimary;

		// Token: 0x0400048C RID: 1164
		private float _position;

		// Token: 0x0400048D RID: 1165
		private Brush _primaryBrush;

		// Token: 0x0400048E RID: 1166
		private Brush _secondaryBrush;
	}
}
