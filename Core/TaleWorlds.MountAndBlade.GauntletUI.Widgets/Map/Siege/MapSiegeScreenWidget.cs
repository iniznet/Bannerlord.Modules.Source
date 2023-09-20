using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Siege
{
	public class MapSiegeScreenWidget : Widget
	{
		public MapSiegeScreenWidget(UIContext context)
			: base(context)
		{
		}

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

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._currentSelectedButton != null && this.DeployableSiegeMachinesPopup != null)
			{
				this.DeployableSiegeMachinesPopup.ScaledPositionXOffset = Mathf.Clamp(this._currentSelectedButton.GlobalPosition.X - this.DeployableSiegeMachinesPopup.Size.X / 2f + this._currentSelectedButton.Size.X / 2f, 0f, base.EventManager.PageSize.X - this.DeployableSiegeMachinesPopup.Size.X);
				this.DeployableSiegeMachinesPopup.ScaledPositionYOffset = Mathf.Clamp(this._currentSelectedButton.GlobalPosition.Y + this._currentSelectedButton.Size.Y + 10f * base._inverseScaleToUse, 0f, base.EventManager.PageSize.Y - this.DeployableSiegeMachinesPopup.Size.Y);
			}
		}

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

		protected override bool OnPreviewMousePressed()
		{
			this.SetCurrentButton(null);
			return false;
		}

		protected override bool OnPreviewDragEnd()
		{
			return false;
		}

		protected override bool OnPreviewDragBegin()
		{
			return false;
		}

		protected override bool OnPreviewDrop()
		{
			return false;
		}

		protected override bool OnPreviewDragHover()
		{
			return false;
		}

		protected override bool OnPreviewMouseMove()
		{
			return false;
		}

		protected override bool OnPreviewMouseReleased()
		{
			return false;
		}

		protected override bool OnPreviewMouseScroll()
		{
			return false;
		}

		protected override bool OnPreviewMouseAlternatePressed()
		{
			return false;
		}

		protected override bool OnPreviewMouseAlternateReleased()
		{
			return false;
		}

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

		private Widget _deployableSiegeMachinesPopup;

		private MapSiegeMachineButtonWidget _currentSelectedButton;
	}
}
