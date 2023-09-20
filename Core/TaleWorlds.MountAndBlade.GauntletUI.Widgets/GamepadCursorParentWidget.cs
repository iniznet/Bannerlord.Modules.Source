using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class GamepadCursorParentWidget : Widget
	{
		public GamepadCursorParentWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.CenterWidget.SetGlobalAlphaRecursively(MathF.Lerp(this.CenterWidget.AlphaFactor, this.HasTarget ? 0.67f : 1f, 0.16f, 1E-05f));
		}

		public float XOffset
		{
			get
			{
				return this._xOffset;
			}
			set
			{
				if (value != this._xOffset)
				{
					this._xOffset = value;
					base.OnPropertyChanged(value, "XOffset");
					this.CenterWidget.ScaledPositionXOffset = value;
				}
			}
		}

		public float YOffset
		{
			get
			{
				return this._yOffset;
			}
			set
			{
				if (value != this._yOffset)
				{
					this._yOffset = value;
					base.OnPropertyChanged(value, "YOffset");
					this.CenterWidget.ScaledPositionYOffset = value;
				}
			}
		}

		public bool HasTarget
		{
			get
			{
				return this._hasTarget;
			}
			set
			{
				if (value != this._hasTarget)
				{
					this._hasTarget = value;
					base.OnPropertyChanged(value, "HasTarget");
				}
			}
		}

		public BrushWidget CenterWidget
		{
			get
			{
				return this._centerWidget;
			}
			set
			{
				if (value != this._centerWidget)
				{
					this._centerWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "CenterWidget");
				}
			}
		}

		private float _xOffset;

		private float _yOffset;

		private bool _hasTarget;

		private BrushWidget _centerWidget;
	}
}
