using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.KillFeed
{
	// Token: 0x020000AF RID: 175
	public class MultiplayerPersonalKillFeedItemWidget : Widget
	{
		// Token: 0x17000324 RID: 804
		// (get) Token: 0x060008F4 RID: 2292 RVA: 0x0001995C File Offset: 0x00017B5C
		// (set) Token: 0x060008F5 RID: 2293 RVA: 0x00019964 File Offset: 0x00017B64
		public Widget NotificationTypeIconWidget { get; set; }

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x060008F6 RID: 2294 RVA: 0x0001996D File Offset: 0x00017B6D
		// (set) Token: 0x060008F7 RID: 2295 RVA: 0x00019975 File Offset: 0x00017B75
		public Widget NotificationBackgroundWidget { get; set; }

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x060008F8 RID: 2296 RVA: 0x0001997E File Offset: 0x00017B7E
		// (set) Token: 0x060008F9 RID: 2297 RVA: 0x00019986 File Offset: 0x00017B86
		public TextWidget AmountTextWidget { get; set; }

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x060008FA RID: 2298 RVA: 0x0001998F File Offset: 0x00017B8F
		// (set) Token: 0x060008FB RID: 2299 RVA: 0x00019997 File Offset: 0x00017B97
		public RichTextWidget MessageTextWidget { get; set; }

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x060008FC RID: 2300 RVA: 0x000199A0 File Offset: 0x00017BA0
		// (set) Token: 0x060008FD RID: 2301 RVA: 0x000199A8 File Offset: 0x00017BA8
		public float FadeInTime { get; set; } = 1f;

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x060008FE RID: 2302 RVA: 0x000199B1 File Offset: 0x00017BB1
		// (set) Token: 0x060008FF RID: 2303 RVA: 0x000199B9 File Offset: 0x00017BB9
		public float StayTime { get; set; } = 3f;

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06000900 RID: 2304 RVA: 0x000199C2 File Offset: 0x00017BC2
		// (set) Token: 0x06000901 RID: 2305 RVA: 0x000199CA File Offset: 0x00017BCA
		public float FadeOutTime { get; set; } = 0.5f;

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06000902 RID: 2306 RVA: 0x000199D3 File Offset: 0x00017BD3
		private float CurrentAlpha
		{
			get
			{
				return base.AlphaFactor;
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06000903 RID: 2307 RVA: 0x000199DB File Offset: 0x00017BDB
		// (set) Token: 0x06000904 RID: 2308 RVA: 0x000199E3 File Offset: 0x00017BE3
		public float TimeSinceCreation { get; private set; }

		// Token: 0x06000905 RID: 2309 RVA: 0x000199EC File Offset: 0x00017BEC
		public MultiplayerPersonalKillFeedItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x00019A49 File Offset: 0x00017C49
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this._initialized)
			{
				base.PositionYOffset = 0f;
			}
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x00019A68 File Offset: 0x00017C68
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.SetGlobalAlphaRecursively(0f);
				this.UpdateNotificationBackgroundWidget();
				this.UpdateNotificationTypeIconWidget();
				this.UpdateNotificationMessageWidget();
				this.UpdateNotificationAmountWidget();
				this.DetermineSoundEvent();
				this._initialized = true;
			}
			this.UpdateAlphaValues(dt);
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x00019ABB File Offset: 0x00017CBB
		private void DetermineSoundEvent()
		{
			if (this.ItemType == 6)
			{
				base.Context.TwoDimensionContext.PlaySound(this._goldGainedSound);
			}
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x00019ADC File Offset: 0x00017CDC
		private void UpdateAlphaValues(float dt)
		{
			float num = 0f;
			float num2 = 0f;
			this.TimeSinceCreation += dt;
			if (this._maxTargetAlpha == 0f)
			{
				base.EventFired("OnRemove", Array.Empty<object>());
				return;
			}
			if (this.TimeSinceCreation <= this.FadeInTime)
			{
				num = MathF.Min(1f, this._maxTargetAlpha);
				num2 = this.TimeSinceCreation / this.FadeInTime;
			}
			else if (this.TimeSinceCreation - this.FadeInTime <= this.StayTime)
			{
				num = MathF.Min(1f, this._maxTargetAlpha);
				num2 = 1f;
			}
			else if (this.TimeSinceCreation - (this.FadeInTime + this.StayTime) <= this.FadeOutTime)
			{
				num = 0f;
				num2 = (this.TimeSinceCreation - (this.FadeInTime + this.StayTime)) / this.FadeOutTime;
				if (this.CurrentAlpha <= 0.1f)
				{
					base.EventFired("OnRemove", Array.Empty<object>());
				}
			}
			else
			{
				base.EventFired("OnRemove", Array.Empty<object>());
			}
			this.SetGlobalAlphaRecursively(Mathf.Lerp(this.CurrentAlpha, num, num2));
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x00019C01 File Offset: 0x00017E01
		public void SetSpeedModifier(float newSpeed)
		{
			if (newSpeed > this._speedModifier)
			{
				this._speedModifier = newSpeed;
			}
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x00019C13 File Offset: 0x00017E13
		public void SetMaxAlphaValue(float newMaxAlpha)
		{
			if (newMaxAlpha < this._maxTargetAlpha)
			{
				this._maxTargetAlpha = newMaxAlpha;
			}
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x00019C28 File Offset: 0x00017E28
		private void UpdateNotificationTypeIconWidget()
		{
			if (this.ItemType == 0)
			{
				this.NotificationTypeIconWidget.IsVisible = false;
				return;
			}
			switch (this.ItemType)
			{
			case 1:
				this.NotificationTypeIconWidget.SetState("FriendlyFireDamage");
				return;
			case 2:
				this.NotificationTypeIconWidget.SetState("FriendlyFireKill");
				return;
			case 3:
				this.NotificationTypeIconWidget.SetState("MountDamage");
				return;
			case 4:
				this.NotificationTypeIconWidget.SetState("NormalKill");
				return;
			case 5:
				this.NotificationTypeIconWidget.SetState("Assist");
				return;
			case 6:
				this.NotificationTypeIconWidget.SetState("GoldChange");
				return;
			case 7:
				this.NotificationTypeIconWidget.SetState("NormalKillHeadshot");
				return;
			default:
				Debug.FailedAssert("Undefined personal feed notification type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Multiplayer\\KillFeed\\MultiplayerPersonalKillFeedItemWidget.cs", "UpdateNotificationTypeIconWidget", 177);
				this.NotificationTypeIconWidget.IsVisible = false;
				return;
			}
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x00019D14 File Offset: 0x00017F14
		private void UpdateNotificationMessageWidget()
		{
			this.MessageTextWidget.Text = this.Message;
			if (string.IsNullOrEmpty(this.Message))
			{
				this.MessageTextWidget.IsVisible = false;
				return;
			}
			switch (this.ItemType)
			{
			case 0:
			case 3:
			case 4:
			case 5:
			case 7:
				this.MessageTextWidget.SetState("Normal");
				return;
			case 1:
			case 2:
				this.MessageTextWidget.SetState("FriendlyFire");
				return;
			case 6:
				if (this.Amount >= 0)
				{
					this.MessageTextWidget.SetState("GoldChangePositive");
					return;
				}
				this.MessageTextWidget.SetState("GoldChangeNegative");
				return;
			default:
				Debug.FailedAssert("Undefined personal feed notification type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Multiplayer\\KillFeed\\MultiplayerPersonalKillFeedItemWidget.cs", "UpdateNotificationMessageWidget", 218);
				this.MessageTextWidget.IsVisible = false;
				return;
			}
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x00019DF0 File Offset: 0x00017FF0
		private void UpdateNotificationAmountWidget()
		{
			if (this.ItemType != 6 && this.Amount == -1)
			{
				this.AmountTextWidget.IsVisible = false;
				return;
			}
			switch (this.ItemType)
			{
			case 0:
			case 3:
			case 4:
			case 7:
				this.AmountTextWidget.SetState("Normal");
				this.AmountTextWidget.IntText = this.Amount;
				return;
			case 1:
			case 2:
				this.AmountTextWidget.SetState("FriendlyFire");
				this.AmountTextWidget.IntText = this.Amount;
				return;
			case 5:
				this.AmountTextWidget.IsVisible = false;
				return;
			case 6:
				if (this.Amount >= 0)
				{
					this.AmountTextWidget.SetState("GoldChangePositive");
					this.AmountTextWidget.Text = "+" + this.Amount.ToString();
					return;
				}
				this.AmountTextWidget.SetState("GoldChangeNegative");
				this.AmountTextWidget.Text = this.Amount.ToString();
				return;
			default:
				Debug.FailedAssert("Undefined personal feed notification type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Multiplayer\\KillFeed\\MultiplayerPersonalKillFeedItemWidget.cs", "UpdateNotificationAmountWidget", 264);
				this.AmountTextWidget.IsVisible = false;
				return;
			}
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x00019F2C File Offset: 0x0001812C
		private void UpdateNotificationBackgroundWidget()
		{
			switch (this.ItemType)
			{
			case 0:
			case 1:
			case 3:
				this.NotificationBackgroundWidget.SetState("Hidden");
				return;
			case 2:
				this.NotificationBackgroundWidget.SetState("FriendlyFire");
				return;
			case 4:
			case 7:
				this.NotificationBackgroundWidget.SetState("Normal");
				return;
			case 5:
				break;
			case 6:
				if (this.Amount >= 0)
				{
					this.NotificationBackgroundWidget.SetState("GoldChangePositive");
					return;
				}
				this.NotificationBackgroundWidget.SetState("GoldChangeNegative");
				return;
			default:
				Debug.FailedAssert("Undefined personal feed notification type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Multiplayer\\KillFeed\\MultiplayerPersonalKillFeedItemWidget.cs", "UpdateNotificationBackgroundWidget", 300);
				this.NotificationBackgroundWidget.SetState("Hidden");
				break;
			}
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x00019FF0 File Offset: 0x000181F0
		private float GetInitialVerticalPositionOfSelf()
		{
			float num = 0f;
			for (int i = 0; i < base.GetSiblingIndex(); i++)
			{
				num += base.ParentWidget.GetChild(i).Size.Y * base._inverseScaleToUse;
			}
			return num;
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06000911 RID: 2321 RVA: 0x0001A035 File Offset: 0x00018235
		// (set) Token: 0x06000912 RID: 2322 RVA: 0x0001A03D File Offset: 0x0001823D
		public bool IsDamage
		{
			get
			{
				return this._isDamage;
			}
			set
			{
				if (value != this._isDamage)
				{
					this._isDamage = value;
					base.OnPropertyChanged(value, "IsDamage");
				}
			}
		}

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06000913 RID: 2323 RVA: 0x0001A05B File Offset: 0x0001825B
		// (set) Token: 0x06000914 RID: 2324 RVA: 0x0001A063 File Offset: 0x00018263
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
				}
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06000915 RID: 2325 RVA: 0x0001A086 File Offset: 0x00018286
		// (set) Token: 0x06000916 RID: 2326 RVA: 0x0001A08E File Offset: 0x0001828E
		public int ItemType
		{
			get
			{
				return this._itemType;
			}
			set
			{
				if (value != this._itemType)
				{
					this._itemType = value;
					base.OnPropertyChanged(value, "ItemType");
				}
			}
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06000917 RID: 2327 RVA: 0x0001A0AC File Offset: 0x000182AC
		// (set) Token: 0x06000918 RID: 2328 RVA: 0x0001A0B4 File Offset: 0x000182B4
		public int Amount
		{
			get
			{
				return this._amount;
			}
			set
			{
				if (value != this._amount)
				{
					this._amount = value;
					base.OnPropertyChanged(value, "Amount");
				}
			}
		}

		// Token: 0x0400041D RID: 1053
		private float _speedModifier = 1f;

		// Token: 0x0400041F RID: 1055
		private float _maxTargetAlpha = 1f;

		// Token: 0x04000420 RID: 1056
		private bool _initialized;

		// Token: 0x04000421 RID: 1057
		private string _goldGainedSound = "multiplayer/coin_add";

		// Token: 0x04000422 RID: 1058
		private bool _isDamage;

		// Token: 0x04000423 RID: 1059
		private int _itemType;

		// Token: 0x04000424 RID: 1060
		private int _amount = -1;

		// Token: 0x04000425 RID: 1061
		private string _message;
	}
}
