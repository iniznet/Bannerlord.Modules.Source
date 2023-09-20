using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Siege
{
	public class MapSiegePOIBrushWidget : BrushWidget
	{
		private Color _fullColor
		{
			get
			{
				return new Color(0.2784314f, 0.9882353f, 0.44313726f, 1f);
			}
		}

		private Color _emptyColor
		{
			get
			{
				return new Color(0.9882353f, 0.2784314f, 0.2784314f, 1f);
			}
		}

		public SliderWidget Slider { get; set; }

		public Brush ConstructionBrush { get; set; }

		public Brush NormalBrush { get; set; }

		public Vec2 ScreenPosition { get; set; }

		public MapSiegePOIBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			base.ScaledPositionXOffset = this.ScreenPosition.x - base.Size.X / 2f;
			base.ScaledPositionYOffset = this.ScreenPosition.y;
			float num = (float)(this.IsInVisibleRange ? 1 : 0);
			float num2 = MathF.Lerp(base.ReadOnlyBrush.GlobalAlphaFactor, num, dt * 10f, 1E-05f);
			this.SetGlobalAlphaRecursively(num2);
			base.IsEnabled = false;
			if (this._animState == MapSiegePOIBrushWidget.AnimState.Start)
			{
				this._tickCount++;
				if (this._tickCount > 5)
				{
					this._animState = MapSiegePOIBrushWidget.AnimState.Starting;
				}
			}
			else if (this._animState == MapSiegePOIBrushWidget.AnimState.Starting)
			{
				(this.Slider.Filler as BrushWidget).BrushRenderer.RestartAnimation();
				if (this.QueueIndex == 0)
				{
					this.HammerAnimWidget.BrushRenderer.RestartAnimation();
				}
				this._animState = MapSiegePOIBrushWidget.AnimState.Playing;
			}
			if (!this._isBrushChanged)
			{
				(this.Slider.Filler as BrushWidget).Brush = (this.IsConstructing ? this.ConstructionBrush : this.NormalBrush);
				this._animState = MapSiegePOIBrushWidget.AnimState.Start;
				this._isBrushChanged = true;
			}
			if (!this.IsConstructing)
			{
				this.UpdateColorOfSlider();
			}
		}

		protected override void OnMousePressed()
		{
			base.OnMousePressed();
			this.IsPOISelected = true;
			base.EventFired("OnSelection", Array.Empty<object>());
		}

		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
		}

		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
		}

		private void SetMachineTypeIcon(int machineType)
		{
			string text = "SPGeneral\\MapSiege\\";
			switch (machineType)
			{
			case 0:
				text += "wall";
				break;
			case 1:
				text += "broken_wall";
				break;
			case 2:
				text += "ballista";
				break;
			case 3:
				text += "trebuchet";
				break;
			case 4:
				text += "ladder";
				break;
			case 5:
				text += "ram";
				break;
			case 6:
				text += "tower";
				break;
			case 7:
				text += "mangonel";
				break;
			default:
				text += "fallback";
				break;
			}
			this.MachineTypeIconWidget.Sprite = base.Context.SpriteData.GetSprite(text);
		}

		private void UpdateColorOfSlider()
		{
			(this.Slider.Filler as BrushWidget).Brush.Color = Color.Lerp(this._emptyColor, this._fullColor, this.Slider.ValueFloat / this.Slider.MaxValueFloat);
		}

		public MapSiegeConstructionControllerWidget ConstructionControllerWidget
		{
			get
			{
				return this._constructionControllerWidget;
			}
			set
			{
				if (this._constructionControllerWidget != value)
				{
					this._constructionControllerWidget = value;
				}
			}
		}

		public bool IsPlayerSidePOI
		{
			get
			{
				return this._isPlayerSidePOI;
			}
			set
			{
				if (this._isPlayerSidePOI != value)
				{
					this._isPlayerSidePOI = value;
				}
			}
		}

		public bool IsInVisibleRange
		{
			get
			{
				return this._isInVisibleRange;
			}
			set
			{
				if (this._isInVisibleRange != value)
				{
					this._isInVisibleRange = value;
				}
			}
		}

		public bool IsPOISelected
		{
			get
			{
				return this._isPOISelected;
			}
			set
			{
				if (this._isPOISelected != value)
				{
					this._isPOISelected = value;
					this.ConstructionControllerWidget.SetCurrentPOIWidget(value ? this : null);
				}
			}
		}

		public bool IsConstructing
		{
			get
			{
				return this._isConstructing;
			}
			set
			{
				if (this._isConstructing != value)
				{
					this._isConstructing = value;
					this._isBrushChanged = false;
					this._animState = MapSiegePOIBrushWidget.AnimState.Idle;
				}
			}
		}

		public int MachineType
		{
			get
			{
				return this._machineType;
			}
			set
			{
				if (this._machineType != value)
				{
					this._machineType = value;
					this.SetMachineTypeIcon(value);
				}
			}
		}

		public int QueueIndex
		{
			get
			{
				return this._queueIndex;
			}
			set
			{
				if (this._queueIndex != value)
				{
					this._queueIndex = value;
					this._animState = MapSiegePOIBrushWidget.AnimState.Start;
					this._tickCount = 0;
				}
			}
		}

		public Widget MachineTypeIconWidget
		{
			get
			{
				return this._machineTypeIconWidget;
			}
			set
			{
				if (this._machineTypeIconWidget != value)
				{
					this._machineTypeIconWidget = value;
				}
			}
		}

		public BrushWidget HammerAnimWidget
		{
			get
			{
				return this._hammerAnimWidget;
			}
			set
			{
				if (this._hammerAnimWidget != value)
				{
					this._hammerAnimWidget = value;
				}
			}
		}

		private MapSiegePOIBrushWidget.AnimState _animState;

		private bool _isBrushChanged;

		private int _tickCount;

		private bool _isConstructing;

		private bool _isPlayerSidePOI;

		private bool _isInVisibleRange;

		private bool _isPOISelected;

		private BrushWidget _hammerAnimWidget;

		private Widget _machineTypeIconWidget;

		private int _machineType = -1;

		private int _queueIndex = -1;

		private MapSiegeConstructionControllerWidget _constructionControllerWidget;

		public enum AnimState
		{
			Idle,
			Start,
			Starting,
			Playing
		}
	}
}
