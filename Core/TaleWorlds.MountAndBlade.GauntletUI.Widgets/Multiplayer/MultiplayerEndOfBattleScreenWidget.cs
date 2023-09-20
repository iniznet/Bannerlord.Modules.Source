using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	// Token: 0x0200007A RID: 122
	public class MultiplayerEndOfBattleScreenWidget : Widget
	{
		// Token: 0x060006CE RID: 1742 RVA: 0x00014472 File Offset: 0x00012672
		public MultiplayerEndOfBattleScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x00014494 File Offset: 0x00012694
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isAnimationStarted)
			{
				this.SetGlobalAlphaRecursively(MathF.Lerp(this._initialAlpha, this._targetAlpha, this._fadeInTimeElapsed / this.FadeInDuration, 1E-05f));
				this._fadeInTimeElapsed += dt;
				if (this._fadeInTimeElapsed >= this.FadeInDuration)
				{
					this._isAnimationStarted = false;
				}
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x060006D0 RID: 1744 RVA: 0x000144FC File Offset: 0x000126FC
		// (set) Token: 0x060006D1 RID: 1745 RVA: 0x00014504 File Offset: 0x00012704
		[Editor(false)]
		public bool IsShown
		{
			get
			{
				return this._isShown;
			}
			set
			{
				if (value != this._isShown)
				{
					this._isShown = value;
					base.OnPropertyChanged(value, "IsShown");
					this.SetGlobalAlphaRecursively(0f);
					this._isAnimationStarted = value;
					base.IsVisible = value;
					this._fadeInTimeElapsed = 0f;
				}
			}
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x060006D2 RID: 1746 RVA: 0x00014551 File Offset: 0x00012751
		// (set) Token: 0x060006D3 RID: 1747 RVA: 0x00014559 File Offset: 0x00012759
		[Editor(false)]
		public float FadeInDuration
		{
			get
			{
				return this._fadeInDuration;
			}
			set
			{
				if (value != this._fadeInDuration)
				{
					this._fadeInDuration = value;
					base.OnPropertyChanged(value, "FadeInDuration");
				}
			}
		}

		// Token: 0x04000301 RID: 769
		private float _initialAlpha;

		// Token: 0x04000302 RID: 770
		private float _targetAlpha = 1f;

		// Token: 0x04000303 RID: 771
		private bool _isAnimationStarted;

		// Token: 0x04000304 RID: 772
		private float _fadeInTimeElapsed;

		// Token: 0x04000305 RID: 773
		private bool _isShown;

		// Token: 0x04000306 RID: 774
		private float _fadeInDuration = 0.3f;
	}
}
