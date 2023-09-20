using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order
{
	public class OrderSiegeDeploymentItemButtonWidget : ButtonWidget
	{
		public OrderSiegeDeploymentItemButtonWidget(UIContext context)
			: base(context)
		{
		}

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

		private void UpdatePosition()
		{
			if (this.IsInsideWindow)
			{
				base.ScaledPositionXOffset = this.Position.x - base.Size.X / 2f - base.EventManager.LeftUsableAreaStart;
				base.ScaledPositionYOffset = this.Position.y - base.Size.Y - base.EventManager.TopUsableAreaStart;
			}
		}

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

		private bool preSelectedState;

		private bool _isVisualsDirty = true;

		private Vec2 _position;

		private bool _isInsideWindow;

		private bool _isInFront;

		private bool _isPlayerGeneral;

		private OrderSiegeDeploymentScreenWidget _screenWidget;

		private int _pointType;

		private Widget _typeIconWidget;

		private TextWidget _breachedTextWidget;
	}
}
