using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order
{
	// Token: 0x02000065 RID: 101
	public class OrderSiegeDeploymentItemButtonWidget : ButtonWidget
	{
		// Token: 0x06000556 RID: 1366 RVA: 0x0001010F File Offset: 0x0000E30F
		public OrderSiegeDeploymentItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x00010120 File Offset: 0x0000E320
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			base.IsVisible = this.IsInsideWindow && this.IsInFront;
			base.IsEnabled = this.IsPlayerGeneral && this.PointType != 2;
			if (this.preSelectedState != base.IsSelected)
			{
				if (base.IsSelected)
				{
					this.ScreenWidget.SetSelectedDeploymentItem(this);
				}
				this.preSelectedState = base.IsSelected;
			}
			if (this._isVisualsDirty)
			{
				this.UpdateTypeVisuals();
				this._isVisualsDirty = false;
			}
			this.UpdatePosition();
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x000101B4 File Offset: 0x0000E3B4
		private void UpdatePosition()
		{
			if (this.IsInsideWindow)
			{
				base.ScaledPositionXOffset = this.Position.x - base.Size.X / 2f - base.EventManager.LeftUsableAreaStart;
				base.ScaledPositionYOffset = this.Position.y - base.Size.Y - base.EventManager.TopUsableAreaStart;
			}
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x00010224 File Offset: 0x0000E424
		private void UpdateTypeVisuals()
		{
			this.TypeIconWidget.RegisterBrushStatesOfWidget();
			this.BreachedTextWidget.IsVisible = this.PointType == 2;
			this.TypeIconWidget.IsVisible = this.PointType != 2;
			if (this.PointType == 0)
			{
				this.TypeIconWidget.SetState("BatteringRam");
				return;
			}
			if (this.PointType == 1)
			{
				this.TypeIconWidget.SetState("TowerLadder");
				return;
			}
			if (this.PointType == 2)
			{
				this.TypeIconWidget.SetState("Breach");
				return;
			}
			if (this.PointType == 3)
			{
				this.TypeIconWidget.SetState("Ranged");
				return;
			}
			this.TypeIconWidget.SetState("Default");
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x0600055A RID: 1370 RVA: 0x000102DE File Offset: 0x0000E4DE
		// (set) Token: 0x0600055B RID: 1371 RVA: 0x000102E6 File Offset: 0x0000E4E6
		[Editor(false)]
		public TextWidget BreachedTextWidget
		{
			get
			{
				return this._breachedTextWidget;
			}
			set
			{
				if (this._breachedTextWidget != value)
				{
					this._breachedTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "BreachedTextWidget");
					this._isVisualsDirty = true;
				}
			}
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x0600055C RID: 1372 RVA: 0x0001030B File Offset: 0x0000E50B
		// (set) Token: 0x0600055D RID: 1373 RVA: 0x00010313 File Offset: 0x0000E513
		[Editor(false)]
		public Widget TypeIconWidget
		{
			get
			{
				return this._typeIconWidget;
			}
			set
			{
				if (this._typeIconWidget != value)
				{
					this._typeIconWidget = value;
					base.OnPropertyChanged<Widget>(value, "TypeIconWidget");
					this._isVisualsDirty = true;
				}
			}
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x0600055E RID: 1374 RVA: 0x00010338 File Offset: 0x0000E538
		// (set) Token: 0x0600055F RID: 1375 RVA: 0x00010340 File Offset: 0x0000E540
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (this._position != value)
				{
					this._position = value;
					base.OnPropertyChanged(value, "Position");
				}
			}
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000560 RID: 1376 RVA: 0x00010363 File Offset: 0x0000E563
		// (set) Token: 0x06000561 RID: 1377 RVA: 0x0001036B File Offset: 0x0000E56B
		public int PointType
		{
			get
			{
				return this._pointType;
			}
			set
			{
				if (this._pointType != value)
				{
					this._pointType = value;
					base.OnPropertyChanged(value, "PointType");
				}
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000562 RID: 1378 RVA: 0x00010389 File Offset: 0x0000E589
		// (set) Token: 0x06000563 RID: 1379 RVA: 0x00010391 File Offset: 0x0000E591
		public bool IsInsideWindow
		{
			get
			{
				return this._isInsideWindow;
			}
			set
			{
				if (this._isInsideWindow != value)
				{
					this._isInsideWindow = value;
					base.OnPropertyChanged(value, "IsInsideWindow");
				}
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06000564 RID: 1380 RVA: 0x000103AF File Offset: 0x0000E5AF
		// (set) Token: 0x06000565 RID: 1381 RVA: 0x000103B7 File Offset: 0x0000E5B7
		public bool IsInFront
		{
			get
			{
				return this._isInFront;
			}
			set
			{
				if (this._isInFront != value)
				{
					this._isInFront = value;
					base.OnPropertyChanged(value, "IsInFront");
				}
			}
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06000566 RID: 1382 RVA: 0x000103D5 File Offset: 0x0000E5D5
		// (set) Token: 0x06000567 RID: 1383 RVA: 0x000103DD File Offset: 0x0000E5DD
		public bool IsPlayerGeneral
		{
			get
			{
				return this._isPlayerGeneral;
			}
			set
			{
				if (this._isPlayerGeneral != value)
				{
					this._isPlayerGeneral = value;
					base.OnPropertyChanged(value, "IsPlayerGeneral");
				}
			}
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000568 RID: 1384 RVA: 0x000103FB File Offset: 0x0000E5FB
		// (set) Token: 0x06000569 RID: 1385 RVA: 0x00010403 File Offset: 0x0000E603
		public OrderSiegeDeploymentScreenWidget ScreenWidget
		{
			get
			{
				return this._screenWidget;
			}
			set
			{
				if (this._screenWidget != value)
				{
					this._screenWidget = value;
					base.OnPropertyChanged<OrderSiegeDeploymentScreenWidget>(value, "ScreenWidget");
				}
			}
		}

		// Token: 0x0400024E RID: 590
		private bool preSelectedState;

		// Token: 0x0400024F RID: 591
		private bool _isVisualsDirty = true;

		// Token: 0x04000250 RID: 592
		private Vec2 _position;

		// Token: 0x04000251 RID: 593
		private bool _isInsideWindow;

		// Token: 0x04000252 RID: 594
		private bool _isInFront;

		// Token: 0x04000253 RID: 595
		private bool _isPlayerGeneral;

		// Token: 0x04000254 RID: 596
		private OrderSiegeDeploymentScreenWidget _screenWidget;

		// Token: 0x04000255 RID: 597
		private int _pointType;

		// Token: 0x04000256 RID: 598
		private Widget _typeIconWidget;

		// Token: 0x04000257 RID: 599
		private TextWidget _breachedTextWidget;
	}
}
