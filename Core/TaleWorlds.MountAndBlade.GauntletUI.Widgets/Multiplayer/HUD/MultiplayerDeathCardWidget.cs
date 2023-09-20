using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.HUD
{
	// Token: 0x020000B5 RID: 181
	public class MultiplayerDeathCardWidget : Widget
	{
		// Token: 0x17000349 RID: 841
		// (get) Token: 0x0600095A RID: 2394 RVA: 0x0001AE5B File Offset: 0x0001905B
		// (set) Token: 0x0600095B RID: 2395 RVA: 0x0001AE63 File Offset: 0x00019063
		public TextWidget WeaponTextWidget { get; set; }

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x0600095C RID: 2396 RVA: 0x0001AE6C File Offset: 0x0001906C
		// (set) Token: 0x0600095D RID: 2397 RVA: 0x0001AE74 File Offset: 0x00019074
		public TextWidget TitleTextWidget { get; set; }

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x0600095E RID: 2398 RVA: 0x0001AE7D File Offset: 0x0001907D
		// (set) Token: 0x0600095F RID: 2399 RVA: 0x0001AE85 File Offset: 0x00019085
		public ScrollingRichTextWidget KillerNameTextWidget { get; set; }

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06000960 RID: 2400 RVA: 0x0001AE8E File Offset: 0x0001908E
		// (set) Token: 0x06000961 RID: 2401 RVA: 0x0001AE96 File Offset: 0x00019096
		public Widget KillCountContainer { get; set; }

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06000962 RID: 2402 RVA: 0x0001AE9F File Offset: 0x0001909F
		// (set) Token: 0x06000963 RID: 2403 RVA: 0x0001AEA7 File Offset: 0x000190A7
		public Brush SelfInflictedTitleBrush { get; set; }

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06000964 RID: 2404 RVA: 0x0001AEB0 File Offset: 0x000190B0
		// (set) Token: 0x06000965 RID: 2405 RVA: 0x0001AEB8 File Offset: 0x000190B8
		public Brush NormalBrushTitleBrush { get; set; }

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06000966 RID: 2406 RVA: 0x0001AEC1 File Offset: 0x000190C1
		// (set) Token: 0x06000967 RID: 2407 RVA: 0x0001AEC9 File Offset: 0x000190C9
		public float FadeInModifier { get; set; } = 2f;

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06000968 RID: 2408 RVA: 0x0001AED2 File Offset: 0x000190D2
		// (set) Token: 0x06000969 RID: 2409 RVA: 0x0001AEDA File Offset: 0x000190DA
		public float FadeOutModifier { get; set; } = 10f;

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x0600096A RID: 2410 RVA: 0x0001AEE3 File Offset: 0x000190E3
		// (set) Token: 0x0600096B RID: 2411 RVA: 0x0001AEEB File Offset: 0x000190EB
		public float StayTime { get; set; } = 7f;

		// Token: 0x0600096C RID: 2412 RVA: 0x0001AEF4 File Offset: 0x000190F4
		public MultiplayerDeathCardWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0001AF20 File Offset: 0x00019120
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this._initialized = true;
				base.IsEnabled = false;
				this._initAlpha = base.AlphaFactor;
				this.SetGlobalAlphaRecursively(this._targetAlpha);
			}
			if (Math.Abs(base.AlphaFactor - this._targetAlpha) > 1E-45f)
			{
				float num = ((base.AlphaFactor > this._targetAlpha) ? this.FadeOutModifier : this.FadeInModifier);
				float num2 = Mathf.Lerp(base.AlphaFactor, this._targetAlpha, dt * num);
				this.SetGlobalAlphaRecursively(num2);
			}
			if ((this.IsActive && base.AlphaFactor < 1E-45f) || base.Context.EventManager.Time - this._activeTimeStart > this.StayTime)
			{
				this.IsActive = false;
			}
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x0001AFF0 File Offset: 0x000191F0
		private void HandleIsActiveToggle(bool isActive)
		{
			this._targetAlpha = (isActive ? 1f : 0f);
			if (isActive)
			{
				this._activeTimeStart = base.Context.EventManager.Time;
			}
			this.KillCountContainer.IsVisible = !this.IsSelfInflicted && this.KillCountsEnabled;
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x0001B048 File Offset: 0x00019248
		private void HandleSelfInflictedToggle(bool isSelfInflicted)
		{
			this.TitleTextWidget.IsVisible = true;
			this.TitleTextWidget.Brush = (isSelfInflicted ? this.SelfInflictedTitleBrush : this.NormalBrushTitleBrush);
			this.KillerNameTextWidget.IsVisible = !isSelfInflicted;
			this.WeaponTextWidget.IsVisible = !isSelfInflicted;
			this.KillCountContainer.IsVisible = !this.IsSelfInflicted && this.KillCountsEnabled;
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x0001B0B7 File Offset: 0x000192B7
		private void HandleKillCountsEnabledSwitch(bool killCountsEnabled)
		{
			this.KillCountContainer.IsVisible = killCountsEnabled;
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06000971 RID: 2417 RVA: 0x0001B0C5 File Offset: 0x000192C5
		// (set) Token: 0x06000972 RID: 2418 RVA: 0x0001B0CD File Offset: 0x000192CD
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChanged(value, "IsActive");
					this.HandleIsActiveToggle(value);
				}
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x06000973 RID: 2419 RVA: 0x0001B0F2 File Offset: 0x000192F2
		// (set) Token: 0x06000974 RID: 2420 RVA: 0x0001B0FA File Offset: 0x000192FA
		public bool IsSelfInflicted
		{
			get
			{
				return this._isSelfInflicted;
			}
			set
			{
				if (value != this._isSelfInflicted)
				{
					this._isSelfInflicted = value;
					base.OnPropertyChanged(value, "IsSelfInflicted");
					this.HandleSelfInflictedToggle(value);
				}
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06000975 RID: 2421 RVA: 0x0001B11F File Offset: 0x0001931F
		// (set) Token: 0x06000976 RID: 2422 RVA: 0x0001B127 File Offset: 0x00019327
		public bool KillCountsEnabled
		{
			get
			{
				return this._killCountsEnabled;
			}
			set
			{
				if (value != this._killCountsEnabled)
				{
					this._killCountsEnabled = value;
					base.OnPropertyChanged(value, "KillCountsEnabled");
					this.HandleKillCountsEnabledSwitch(value);
				}
			}
		}

		// Token: 0x04000451 RID: 1105
		private float _targetAlpha;

		// Token: 0x04000452 RID: 1106
		private float _initAlpha;

		// Token: 0x04000456 RID: 1110
		private float _activeTimeStart;

		// Token: 0x04000457 RID: 1111
		private bool _initialized;

		// Token: 0x04000458 RID: 1112
		private bool _isActive;

		// Token: 0x04000459 RID: 1113
		private bool _isSelfInflicted;

		// Token: 0x0400045A RID: 1114
		private bool _killCountsEnabled;
	}
}
