using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	// Token: 0x020000D4 RID: 212
	public class OrderOfBattleFormationMarkerBrushWidget : BrushWidget
	{
		// Token: 0x06000AD2 RID: 2770 RVA: 0x0001E1D0 File Offset: 0x0001C3D0
		public OrderOfBattleFormationMarkerBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x0001E1DC File Offset: 0x0001C3DC
		protected override void OnUpdate(float dt)
		{
			base.IsVisible = this.IsAvailable && this.WSign > 0;
			if (base.IsVisible)
			{
				base.ScaledPositionXOffset = this.Position.x - base.Size.X / 2f;
				base.ScaledPositionYOffset = this.Position.y - base.Size.Y / 2f;
			}
		}

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06000AD4 RID: 2772 RVA: 0x0001E251 File Offset: 0x0001C451
		// (set) Token: 0x06000AD5 RID: 2773 RVA: 0x0001E259 File Offset: 0x0001C459
		[Editor(false)]
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (value != this._position)
				{
					this._position = value;
					base.OnPropertyChanged(value, "Position");
				}
			}
		}

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06000AD6 RID: 2774 RVA: 0x0001E27C File Offset: 0x0001C47C
		// (set) Token: 0x06000AD7 RID: 2775 RVA: 0x0001E284 File Offset: 0x0001C484
		[Editor(false)]
		public bool IsAvailable
		{
			get
			{
				return this._isAvailable;
			}
			set
			{
				if (value != this._isAvailable)
				{
					this._isAvailable = value;
					base.OnPropertyChanged(value, "IsAvailable");
				}
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06000AD8 RID: 2776 RVA: 0x0001E2A2 File Offset: 0x0001C4A2
		// (set) Token: 0x06000AD9 RID: 2777 RVA: 0x0001E2AA File Offset: 0x0001C4AA
		[Editor(false)]
		public bool IsTracked
		{
			get
			{
				return this._isTracked;
			}
			set
			{
				if (value != this._isTracked)
				{
					this._isTracked = value;
					base.OnPropertyChanged(value, "IsTracked");
				}
			}
		}

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06000ADA RID: 2778 RVA: 0x0001E2C8 File Offset: 0x0001C4C8
		// (set) Token: 0x06000ADB RID: 2779 RVA: 0x0001E2D0 File Offset: 0x0001C4D0
		[Editor(false)]
		public int WSign
		{
			get
			{
				return this._wSign;
			}
			set
			{
				if (this._wSign != value)
				{
					this._wSign = value;
					base.OnPropertyChanged(value, "WSign");
				}
			}
		}

		// Token: 0x040004EF RID: 1263
		private Vec2 _position;

		// Token: 0x040004F0 RID: 1264
		private bool _isAvailable;

		// Token: 0x040004F1 RID: 1265
		private bool _isTracked;

		// Token: 0x040004F2 RID: 1266
		private int _wSign;
	}
}
