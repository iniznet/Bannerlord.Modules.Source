using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.DamageFeed
{
	public class MissionAgentDamageFeedItemWidget : Widget
	{
		public float FadeInTime { get; set; } = 0.1f;

		public float StayTime { get; set; } = 1.5f;

		public float FadeOutTime { get; set; } = 0.3f;

		private float CurrentAlpha
		{
			get
			{
				return base.AlphaFactor;
			}
		}

		public float TimeSinceCreation { get; private set; }

		public MissionAgentDamageFeedItemWidget(UIContext context)
			: base(context)
		{
		}

		public void ShowFeed()
		{
			this._isShown = true;
		}

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

		public void SetSpeedModifier(float newSpeed)
		{
			if (newSpeed > this._speedModifier)
			{
				this._speedModifier = newSpeed;
			}
		}

		private float _speedModifier = 1f;

		private bool _isInitialized;

		private bool _isShown;
	}
}
