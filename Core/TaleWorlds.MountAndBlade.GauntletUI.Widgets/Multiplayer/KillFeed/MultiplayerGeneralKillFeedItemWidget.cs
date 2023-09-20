using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.KillFeed
{
	// Token: 0x020000AD RID: 173
	public class MultiplayerGeneralKillFeedItemWidget : Widget
	{
		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060008E7 RID: 2279 RVA: 0x000196C9 File Offset: 0x000178C9
		private float CurrentAlpha
		{
			get
			{
				return base.AlphaFactor;
			}
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x060008E8 RID: 2280 RVA: 0x000196D1 File Offset: 0x000178D1
		// (set) Token: 0x060008E9 RID: 2281 RVA: 0x000196D9 File Offset: 0x000178D9
		public float TimeSinceCreation { get; private set; }

		// Token: 0x060008EA RID: 2282 RVA: 0x000196E2 File Offset: 0x000178E2
		public MultiplayerGeneralKillFeedItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x000196F8 File Offset: 0x000178F8
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.SetGlobalAlphaRecursively(0f);
				this._initialized = true;
			}
			this.TimeSinceCreation += dt * this._speedModifier;
			if (this.TimeSinceCreation <= 0.15f)
			{
				this.SetGlobalAlphaRecursively(Mathf.Lerp(this.CurrentAlpha, 1f, this.TimeSinceCreation / 0.15f));
				return;
			}
			if (this.TimeSinceCreation - 0.15f <= 3.5f)
			{
				this.SetGlobalAlphaRecursively(1f);
				return;
			}
			if (this.TimeSinceCreation - 3.65f <= 1f)
			{
				this.SetGlobalAlphaRecursively(Mathf.Lerp(this.CurrentAlpha, 0f, (this.TimeSinceCreation - 3.65f) / 1f));
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

		// Token: 0x060008EC RID: 2284 RVA: 0x000197F3 File Offset: 0x000179F3
		public void SetSpeedModifier(float newSpeed)
		{
			if (newSpeed > this._speedModifier)
			{
				this._speedModifier = newSpeed;
			}
		}

		// Token: 0x0400040D RID: 1037
		private const float FadeInTime = 0.15f;

		// Token: 0x0400040E RID: 1038
		private const float StayTime = 3.5f;

		// Token: 0x0400040F RID: 1039
		private const float FadeOutTime = 1f;

		// Token: 0x04000410 RID: 1040
		private float _speedModifier = 1f;

		// Token: 0x04000412 RID: 1042
		private bool _initialized;
	}
}
