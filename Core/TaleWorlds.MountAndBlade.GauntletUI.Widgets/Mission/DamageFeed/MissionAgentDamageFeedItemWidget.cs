using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.DamageFeed
{
	// Token: 0x020000E4 RID: 228
	public class MissionAgentDamageFeedItemWidget : Widget
	{
		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06000BEA RID: 3050 RVA: 0x00021601 File Offset: 0x0001F801
		// (set) Token: 0x06000BEB RID: 3051 RVA: 0x00021609 File Offset: 0x0001F809
		public float FadeInTime { get; set; } = 0.1f;

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06000BEC RID: 3052 RVA: 0x00021612 File Offset: 0x0001F812
		// (set) Token: 0x06000BED RID: 3053 RVA: 0x0002161A File Offset: 0x0001F81A
		public float StayTime { get; set; } = 1.5f;

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06000BEE RID: 3054 RVA: 0x00021623 File Offset: 0x0001F823
		// (set) Token: 0x06000BEF RID: 3055 RVA: 0x0002162B File Offset: 0x0001F82B
		public float FadeOutTime { get; set; } = 0.3f;

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06000BF0 RID: 3056 RVA: 0x00021634 File Offset: 0x0001F834
		private float CurrentAlpha
		{
			get
			{
				return base.AlphaFactor;
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06000BF1 RID: 3057 RVA: 0x0002163C File Offset: 0x0001F83C
		// (set) Token: 0x06000BF2 RID: 3058 RVA: 0x00021644 File Offset: 0x0001F844
		public float TimeSinceCreation { get; private set; }

		// Token: 0x06000BF3 RID: 3059 RVA: 0x0002164D File Offset: 0x0001F84D
		public MissionAgentDamageFeedItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x00021682 File Offset: 0x0001F882
		public void ShowFeed()
		{
			this._isShown = true;
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x0002168C File Offset: 0x0001F88C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isInitialized)
			{
				this.SetGlobalAlphaRecursively(0f);
				this._isInitialized = true;
			}
			if (this._isShown)
			{
				this.TimeSinceCreation += dt * this._speedModifier;
				if (this.TimeSinceCreation <= this.FadeInTime)
				{
					this.SetGlobalAlphaRecursively(Mathf.Lerp(this.CurrentAlpha, 1f, this.TimeSinceCreation / this.FadeInTime));
					return;
				}
				if (this.TimeSinceCreation - this.FadeInTime <= this.StayTime)
				{
					this.SetGlobalAlphaRecursively(1f);
					return;
				}
				if (this.TimeSinceCreation - (this.FadeInTime + this.StayTime) <= this.FadeOutTime)
				{
					this.SetGlobalAlphaRecursively(Mathf.Lerp(this.CurrentAlpha, 0f, (this.TimeSinceCreation - (this.FadeInTime + this.StayTime)) / this.FadeOutTime));
					if (this.CurrentAlpha <= 0.1f)
					{
						base.EventFired("OnRemove", Array.Empty<object>());
						return;
					}
				}
				else
				{
					base.EventFired("OnRemove", Array.Empty<object>());
				}
			}
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x000217A8 File Offset: 0x0001F9A8
		public void SetSpeedModifier(float newSpeed)
		{
			if (newSpeed > this._speedModifier)
			{
				this._speedModifier = newSpeed;
			}
		}

		// Token: 0x04000582 RID: 1410
		private float _speedModifier = 1f;

		// Token: 0x04000584 RID: 1412
		private bool _isInitialized;

		// Token: 0x04000585 RID: 1413
		private bool _isShown;
	}
}
