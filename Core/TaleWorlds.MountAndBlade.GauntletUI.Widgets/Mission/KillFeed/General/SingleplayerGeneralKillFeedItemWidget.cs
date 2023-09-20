using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.KillFeed.General
{
	// Token: 0x020000E0 RID: 224
	public class SingleplayerGeneralKillFeedItemWidget : Widget
	{
		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06000B9F RID: 2975 RVA: 0x000205D8 File Offset: 0x0001E7D8
		// (set) Token: 0x06000BA0 RID: 2976 RVA: 0x000205E0 File Offset: 0x0001E7E0
		public Widget MurdererTypeWidget { get; set; }

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06000BA1 RID: 2977 RVA: 0x000205E9 File Offset: 0x0001E7E9
		// (set) Token: 0x06000BA2 RID: 2978 RVA: 0x000205F1 File Offset: 0x0001E7F1
		public Widget VictimTypeWidget { get; set; }

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06000BA3 RID: 2979 RVA: 0x000205FA File Offset: 0x0001E7FA
		// (set) Token: 0x06000BA4 RID: 2980 RVA: 0x00020602 File Offset: 0x0001E802
		public Widget ActionIconWidget { get; set; }

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06000BA5 RID: 2981 RVA: 0x0002060B File Offset: 0x0001E80B
		// (set) Token: 0x06000BA6 RID: 2982 RVA: 0x00020613 File Offset: 0x0001E813
		public Widget BackgroundWidget { get; set; }

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06000BA7 RID: 2983 RVA: 0x0002061C File Offset: 0x0001E81C
		// (set) Token: 0x06000BA8 RID: 2984 RVA: 0x00020624 File Offset: 0x0001E824
		public AutoHideTextWidget VictimNameWidget { get; set; }

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06000BA9 RID: 2985 RVA: 0x0002062D File Offset: 0x0001E82D
		// (set) Token: 0x06000BAA RID: 2986 RVA: 0x00020635 File Offset: 0x0001E835
		public AutoHideTextWidget MurdererNameWidget { get; set; }

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06000BAB RID: 2987 RVA: 0x0002063E File Offset: 0x0001E83E
		// (set) Token: 0x06000BAC RID: 2988 RVA: 0x00020646 File Offset: 0x0001E846
		public float FadeInTime { get; set; } = 0.7f;

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06000BAD RID: 2989 RVA: 0x0002064F File Offset: 0x0001E84F
		// (set) Token: 0x06000BAE RID: 2990 RVA: 0x00020657 File Offset: 0x0001E857
		public float StayTime { get; set; } = 3f;

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06000BAF RID: 2991 RVA: 0x00020660 File Offset: 0x0001E860
		// (set) Token: 0x06000BB0 RID: 2992 RVA: 0x00020668 File Offset: 0x0001E868
		public float FadeOutTime { get; set; } = 0.7f;

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06000BB1 RID: 2993 RVA: 0x00020671 File Offset: 0x0001E871
		private float CurrentAlpha
		{
			get
			{
				return base.AlphaFactor;
			}
		}

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06000BB2 RID: 2994 RVA: 0x00020679 File Offset: 0x0001E879
		// (set) Token: 0x06000BB3 RID: 2995 RVA: 0x00020681 File Offset: 0x0001E881
		public float TimeSinceCreation { get; private set; }

		// Token: 0x06000BB4 RID: 2996 RVA: 0x0002068A File Offset: 0x0001E88A
		public SingleplayerGeneralKillFeedItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x000206C0 File Offset: 0x0001E8C0
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

		// Token: 0x06000BB6 RID: 2998 RVA: 0x00020946 File Offset: 0x0001EB46
		public void SetSpeedModifier(float newSpeed)
		{
			if (newSpeed > this._speedModifier)
			{
				this._speedModifier = newSpeed;
			}
		}

		// Token: 0x06000BB7 RID: 2999 RVA: 0x00020958 File Offset: 0x0001EB58
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

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06000BB8 RID: 3000 RVA: 0x000209CF File Offset: 0x0001EBCF
		// (set) Token: 0x06000BB9 RID: 3001 RVA: 0x000209D7 File Offset: 0x0001EBD7
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

		// Token: 0x04000550 RID: 1360
		private const char _seperator = '\0';

		// Token: 0x04000557 RID: 1367
		private string _murdererType;

		// Token: 0x04000558 RID: 1368
		private string _victimType;

		// Token: 0x04000559 RID: 1369
		private string _murdererName;

		// Token: 0x0400055A RID: 1370
		private string _victimName;

		// Token: 0x0400055B RID: 1371
		private bool _isUnconscious;

		// Token: 0x0400055C RID: 1372
		private bool _isHeadshot;

		// Token: 0x0400055D RID: 1373
		private Color _color;

		// Token: 0x04000561 RID: 1377
		private float _speedModifier = 1f;

		// Token: 0x04000563 RID: 1379
		private bool _initialized;

		// Token: 0x04000564 RID: 1380
		private string _message;
	}
}
