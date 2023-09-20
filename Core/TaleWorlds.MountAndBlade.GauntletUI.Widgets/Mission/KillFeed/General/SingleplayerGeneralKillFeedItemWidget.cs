using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.KillFeed.General
{
	public class SingleplayerGeneralKillFeedItemWidget : Widget
	{
		public Widget MurdererTypeWidget { get; set; }

		public Widget VictimTypeWidget { get; set; }

		public Widget ActionIconWidget { get; set; }

		public Widget BackgroundWidget { get; set; }

		public AutoHideTextWidget VictimNameWidget { get; set; }

		public AutoHideTextWidget MurdererNameWidget { get; set; }

		public float FadeInTime { get; set; } = 0.7f;

		public float StayTime { get; set; } = 3f;

		public float FadeOutTime { get; set; } = 0.7f;

		private float CurrentAlpha
		{
			get
			{
				return base.AlphaFactor;
			}
		}

		public float TimeSinceCreation { get; private set; }

		public SingleplayerGeneralKillFeedItemWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.MurdererTypeWidget.Sprite = this.MurdererTypeWidget.Context.SpriteData.GetSprite("General\\compass\\" + this._murdererType);
				this.VictimTypeWidget.Sprite = this.MurdererTypeWidget.Context.SpriteData.GetSprite("General\\compass\\" + this._victimType);
				this.ActionIconWidget.Sprite = this.ActionIconWidget.Context.SpriteData.GetSprite("General\\Mission\\PersonalKillfeed\\" + (this._isHeadshot ? "headshot_kill_icon" : "kill_feed_skull"));
				this.SetGlobalAlphaRecursively(0f);
				this.ActionIconWidget.Color = (this._isUnconscious ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 0f, 0f, 1f));
				this.BackgroundWidget.Color = this._color;
				this.VictimNameWidget.Text = this._victimName;
				this.MurdererNameWidget.Text = this._murdererName;
				if (this._victimName.Length == 0)
				{
					this.ActionIconWidget.MarginRight = 0f;
					this.VictimTypeWidget.MarginLeft = 5f;
				}
				if (this._murdererName.Length == 0)
				{
					this.ActionIconWidget.MarginLeft = 0f;
					this.MurdererTypeWidget.MarginRight = 5f;
				}
				this._initialized = true;
			}
			this.TimeSinceCreation += dt * this._speedModifier;
			if (this.TimeSinceCreation <= this.FadeInTime)
			{
				this.SetGlobalAlphaRecursively(Mathf.Lerp(this.CurrentAlpha, 0.5f, this.TimeSinceCreation / this.FadeInTime));
				return;
			}
			if (this.TimeSinceCreation - this.FadeInTime <= this.StayTime)
			{
				this.SetGlobalAlphaRecursively(0.5f);
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

		public void SetSpeedModifier(float newSpeed)
		{
			if (newSpeed > this._speedModifier)
			{
				this._speedModifier = newSpeed;
			}
		}

		private void HandleMessage(string value)
		{
			string[] array = value.Split(new char[1]);
			this._murdererName = array[0];
			this._murdererType = array[1];
			this._victimName = array[2];
			this._victimType = array[3];
			this._isUnconscious = array[4].Equals("1");
			this._isHeadshot = array[5].Equals("1");
			this._color = Color.FromUint(uint.Parse(array[6]));
		}

		public string Message
		{
			get
			{
				return this._message;
			}
			set
			{
				if (value != this._message)
				{
					this._message = value;
					base.OnPropertyChanged<string>(value, "Message");
					this.HandleMessage(value);
				}
			}
		}

		private const char _seperator = '\0';

		private string _murdererType;

		private string _victimType;

		private string _murdererName;

		private string _victimName;

		private bool _isUnconscious;

		private bool _isHeadshot;

		private Color _color;

		private float _speedModifier = 1f;

		private bool _initialized;

		private string _message;
	}
}
