using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.HUD
{
	public class MoraleArrowBrushWidget : BrushWidget
	{
		public bool LeftSideArrow { get; set; }

		public float BaseHorizontalExtendRange
		{
			get
			{
				return 3.3f;
			}
		}

		private float BaseSpeedModifier
		{
			get
			{
				return 13f;
			}
		}

		public bool AreMoralesIndependent { get; set; }

		public MoraleArrowBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this._initialized)
			{
				base.Brush.GlobalAlphaFactor = 0f;
				this._initialized = true;
			}
			base.IsVisible = this._currentFlow > 0 && !this.AreMoralesIndependent;
			if (base.IsVisible)
			{
				float num = this.BaseSpeedModifier * (float)this._currentFlow;
				float num2 = this.BaseHorizontalExtendRange * (float)this._currentFlow;
				if (this._currentAnimState == MoraleArrowBrushWidget.AnimStates.FadeIn)
				{
					if (base.ReadOnlyBrush.GlobalAlphaFactor < 1f)
					{
						this.SetGlobalAlphaRecursively(Mathf.Lerp(base.ReadOnlyBrush.GlobalAlphaFactor, 1f, dt * num));
					}
					if ((double)base.ReadOnlyBrush.GlobalAlphaFactor >= 0.99)
					{
						this._currentAnimState = MoraleArrowBrushWidget.AnimStates.Move;
					}
				}
				else if (this._currentAnimState == MoraleArrowBrushWidget.AnimStates.Move)
				{
					if (Math.Abs(base.PositionXOffset) < num2)
					{
						int num3 = (this.LeftSideArrow ? (-1) : 1);
						base.PositionXOffset = Mathf.Lerp(base.PositionXOffset, num2 * (float)num3, dt * num);
					}
					if ((double)Math.Abs(base.PositionXOffset) >= (double)num2 - 0.01)
					{
						this._currentAnimState = MoraleArrowBrushWidget.AnimStates.FadeOut;
					}
				}
				else if (this._currentAnimState == MoraleArrowBrushWidget.AnimStates.FadeOut)
				{
					if (base.ReadOnlyBrush.GlobalAlphaFactor > 0f)
					{
						this.SetGlobalAlphaRecursively(Mathf.Lerp(base.ReadOnlyBrush.GlobalAlphaFactor, 0f, dt * num));
					}
					if ((double)base.ReadOnlyBrush.GlobalAlphaFactor <= 0.01)
					{
						this._currentAnimState = MoraleArrowBrushWidget.AnimStates.GoToInitPos;
					}
				}
				else
				{
					base.PositionXOffset = 0f;
					this._currentAnimState = MoraleArrowBrushWidget.AnimStates.FadeIn;
				}
			}
			else
			{
				base.PositionXOffset = 0f;
				this._currentAnimState = MoraleArrowBrushWidget.AnimStates.FadeIn;
			}
			this._timeSinceCreation += dt;
		}

		public void SetFlowLevel(int flow)
		{
			this._currentFlow = flow;
			base.IsVisible = this._currentFlow > 0 && !this.AreMoralesIndependent;
		}

		private float _timeSinceCreation;

		private bool _initialized;

		private int _currentFlow;

		private MoraleArrowBrushWidget.AnimStates _currentAnimState;

		private enum AnimStates
		{
			FadeIn,
			Move,
			FadeOut,
			GoToInitPos
		}
	}
}
