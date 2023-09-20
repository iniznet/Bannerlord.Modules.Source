using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Siege
{
	// Token: 0x020000FF RID: 255
	public class MapSiegeScreenWidget : Widget
	{
		// Token: 0x06000D2C RID: 3372 RVA: 0x00024D17 File Offset: 0x00022F17
		public MapSiegeScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000D2D RID: 3373 RVA: 0x00024D20 File Offset: 0x00022F20
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			Widget latestMouseUpWidget = base.EventManager.LatestMouseUpWidget;
			if (this._currentSelectedButton != null && latestMouseUpWidget != null && !(latestMouseUpWidget is MapSiegeMachineButtonWidget) && !this._currentSelectedButton.CheckIsMyChildRecursive(latestMouseUpWidget) && this.IsWidgetChildOfType<MapSiegeMachineButtonWidget>(latestMouseUpWidget) == null)
			{
				this.SetCurrentButton(null);
			}
			if (base.EventManager.LatestMouseUpWidget == null)
			{
				this.SetCurrentButton(null);
			}
			if (this.DeployableSiegeMachinesPopup != null)
			{
				this.DeployableSiegeMachinesPopup.IsVisible = this._currentSelectedButton != null;
			}
		}

		// Token: 0x06000D2E RID: 3374 RVA: 0x00024DA4 File Offset: 0x00022FA4
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._currentSelectedButton != null && this.DeployableSiegeMachinesPopup != null)
			{
				this.DeployableSiegeMachinesPopup.ScaledPositionXOffset = Mathf.Clamp(this._currentSelectedButton.GlobalPosition.X - this.DeployableSiegeMachinesPopup.Size.X / 2f + this._currentSelectedButton.Size.X / 2f, 0f, base.EventManager.PageSize.X - this.DeployableSiegeMachinesPopup.Size.X);
				this.DeployableSiegeMachinesPopup.ScaledPositionYOffset = Mathf.Clamp(this._currentSelectedButton.GlobalPosition.Y + this._currentSelectedButton.Size.Y + 10f * base._inverseScaleToUse, 0f, base.EventManager.PageSize.Y - this.DeployableSiegeMachinesPopup.Size.Y);
			}
		}

		// Token: 0x06000D2F RID: 3375 RVA: 0x00024EA6 File Offset: 0x000230A6
		public void SetCurrentButton(MapSiegeMachineButtonWidget button)
		{
			if (button == null)
			{
				this._currentSelectedButton = null;
				return;
			}
			if (this._currentSelectedButton == button || !button.IsDeploymentTarget)
			{
				this.SetCurrentButton(null);
				return;
			}
			this._currentSelectedButton = button;
		}

		// Token: 0x06000D30 RID: 3376 RVA: 0x00024ED3 File Offset: 0x000230D3
		protected override bool OnPreviewMousePressed()
		{
			this.SetCurrentButton(null);
			return false;
		}

		// Token: 0x06000D31 RID: 3377 RVA: 0x00024EDD File Offset: 0x000230DD
		protected override bool OnPreviewDragEnd()
		{
			return false;
		}

		// Token: 0x06000D32 RID: 3378 RVA: 0x00024EE0 File Offset: 0x000230E0
		protected override bool OnPreviewDragBegin()
		{
			return false;
		}

		// Token: 0x06000D33 RID: 3379 RVA: 0x00024EE3 File Offset: 0x000230E3
		protected override bool OnPreviewDrop()
		{
			return false;
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x00024EE6 File Offset: 0x000230E6
		protected override bool OnPreviewDragHover()
		{
			return false;
		}

		// Token: 0x06000D35 RID: 3381 RVA: 0x00024EE9 File Offset: 0x000230E9
		protected override bool OnPreviewMouseMove()
		{
			return false;
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x00024EEC File Offset: 0x000230EC
		protected override bool OnPreviewMouseReleased()
		{
			return false;
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x00024EEF File Offset: 0x000230EF
		protected override bool OnPreviewMouseScroll()
		{
			return false;
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x00024EF2 File Offset: 0x000230F2
		protected override bool OnPreviewMouseAlternatePressed()
		{
			return false;
		}

		// Token: 0x06000D39 RID: 3385 RVA: 0x00024EF5 File Offset: 0x000230F5
		protected override bool OnPreviewMouseAlternateReleased()
		{
			return false;
		}

		// Token: 0x06000D3A RID: 3386 RVA: 0x00024EF8 File Offset: 0x000230F8
		private T IsWidgetChildOfType<T>(Widget currentWidget) where T : Widget
		{
			while (currentWidget != null)
			{
				if (currentWidget is T)
				{
					return (T)((object)currentWidget);
				}
				currentWidget = currentWidget.ParentWidget;
			}
			return default(T);
		}

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x06000D3B RID: 3387 RVA: 0x00024F2A File Offset: 0x0002312A
		// (set) Token: 0x06000D3C RID: 3388 RVA: 0x00024F32 File Offset: 0x00023132
		[Editor(false)]
		public Widget DeployableSiegeMachinesPopup
		{
			get
			{
				return this._deployableSiegeMachinesPopup;
			}
			set
			{
				if (value != this._deployableSiegeMachinesPopup)
				{
					this._deployableSiegeMachinesPopup = value;
					base.OnPropertyChanged<Widget>(value, "DeployableSiegeMachinesPopup");
				}
			}
		}

		// Token: 0x0400061B RID: 1563
		private Widget _deployableSiegeMachinesPopup;

		// Token: 0x0400061C RID: 1564
		private MapSiegeMachineButtonWidget _currentSelectedButton;
	}
}
