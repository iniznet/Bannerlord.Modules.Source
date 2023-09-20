using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.KillFeed
{
	public class MultiplayerGeneralKillFeedItemWidget : Widget
	{
		private float CurrentAlpha
		{
			get
			{
				return base.AlphaFactor;
			}
		}

		public float TimeSinceCreation { get; private set; }

		public MultiplayerGeneralKillFeedItemWidget(UIContext context)
			: base(context)
		{
		}

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

		public void SetSpeedModifier(float newSpeed)
		{
			if (newSpeed > this._speedModifier)
			{
				this._speedModifier = newSpeed;
			}
		}

		private const float FadeInTime = 0.15f;

		private const float StayTime = 3.5f;

		private const float FadeOutTime = 1f;

		private float _speedModifier = 1f;

		private bool _initialized;
	}
}
