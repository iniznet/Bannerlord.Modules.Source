using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.HUD
{
	// Token: 0x020000B3 RID: 179
	public class MoraleArrowBrushWidget : BrushWidget
	{
		// Token: 0x17000336 RID: 822
		// (get) Token: 0x0600092D RID: 2349 RVA: 0x0001A482 File Offset: 0x00018682
		// (set) Token: 0x0600092E RID: 2350 RVA: 0x0001A48A File Offset: 0x0001868A
		public bool LeftSideArrow { get; set; }

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x0600092F RID: 2351 RVA: 0x0001A493 File Offset: 0x00018693
		public float BaseHorizontalExtendRange
		{
			get
			{
				return 3.3f;
			}
		}

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06000930 RID: 2352 RVA: 0x0001A49A File Offset: 0x0001869A
		private float BaseSpeedModifier
		{
			get
			{
				return 13f;
			}
		}

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06000931 RID: 2353 RVA: 0x0001A4A1 File Offset: 0x000186A1
		// (set) Token: 0x06000932 RID: 2354 RVA: 0x0001A4A9 File Offset: 0x000186A9
		public bool AreMoralesIndependent { get; set; }

		// Token: 0x06000933 RID: 2355 RVA: 0x0001A4B2 File Offset: 0x000186B2
		public MoraleArrowBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x0001A4BC File Offset: 0x000186BC
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

		// Token: 0x06000935 RID: 2357 RVA: 0x0001A680 File Offset: 0x00018880
		public void SetFlowLevel(int flow)
		{
			this._currentFlow = flow;
			base.IsVisible = this._currentFlow > 0 && !this.AreMoralesIndependent;
		}

		// Token: 0x0400042E RID: 1070
		private float _timeSinceCreation;

		// Token: 0x0400042F RID: 1071
		private bool _initialized;

		// Token: 0x04000430 RID: 1072
		private int _currentFlow;

		// Token: 0x04000433 RID: 1075
		private MoraleArrowBrushWidget.AnimStates _currentAnimState;

		// Token: 0x0200018E RID: 398
		private enum AnimStates
		{
			// Token: 0x040008F0 RID: 2288
			FadeIn,
			// Token: 0x040008F1 RID: 2289
			Move,
			// Token: 0x040008F2 RID: 2290
			FadeOut,
			// Token: 0x040008F3 RID: 2291
			GoToInitPos
		}
	}
}
