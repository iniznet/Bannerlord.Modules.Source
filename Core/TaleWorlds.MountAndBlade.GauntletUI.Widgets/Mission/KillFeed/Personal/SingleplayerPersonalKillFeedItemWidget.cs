using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.KillFeed.Personal
{
	// Token: 0x020000DE RID: 222
	public class SingleplayerPersonalKillFeedItemWidget : Widget
	{
		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06000B74 RID: 2932 RVA: 0x0001FDEB File Offset: 0x0001DFEB
		// (set) Token: 0x06000B75 RID: 2933 RVA: 0x0001FDF3 File Offset: 0x0001DFF3
		public Widget NotificationTypeIconWidget { get; set; }

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06000B76 RID: 2934 RVA: 0x0001FDFC File Offset: 0x0001DFFC
		// (set) Token: 0x06000B77 RID: 2935 RVA: 0x0001FE04 File Offset: 0x0001E004
		public Widget NotificationBackgroundWidget { get; set; }

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06000B78 RID: 2936 RVA: 0x0001FE0D File Offset: 0x0001E00D
		// (set) Token: 0x06000B79 RID: 2937 RVA: 0x0001FE15 File Offset: 0x0001E015
		public TextWidget AmountTextWidget { get; set; }

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06000B7A RID: 2938 RVA: 0x0001FE1E File Offset: 0x0001E01E
		// (set) Token: 0x06000B7B RID: 2939 RVA: 0x0001FE26 File Offset: 0x0001E026
		public RichTextWidget MessageTextWidget { get; set; }

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06000B7C RID: 2940 RVA: 0x0001FE2F File Offset: 0x0001E02F
		// (set) Token: 0x06000B7D RID: 2941 RVA: 0x0001FE37 File Offset: 0x0001E037
		public float FadeInTime { get; set; } = 0.2f;

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06000B7E RID: 2942 RVA: 0x0001FE40 File Offset: 0x0001E040
		// (set) Token: 0x06000B7F RID: 2943 RVA: 0x0001FE48 File Offset: 0x0001E048
		public float StayTime { get; set; } = 2f;

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06000B80 RID: 2944 RVA: 0x0001FE51 File Offset: 0x0001E051
		// (set) Token: 0x06000B81 RID: 2945 RVA: 0x0001FE59 File Offset: 0x0001E059
		public float FadeOutTime { get; set; } = 0.2f;

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06000B82 RID: 2946 RVA: 0x0001FE62 File Offset: 0x0001E062
		private float CurrentAlpha
		{
			get
			{
				return base.AlphaFactor;
			}
		}

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06000B83 RID: 2947 RVA: 0x0001FE6A File Offset: 0x0001E06A
		// (set) Token: 0x06000B84 RID: 2948 RVA: 0x0001FE72 File Offset: 0x0001E072
		public float TimeSinceCreation { get; private set; }

		// Token: 0x06000B85 RID: 2949 RVA: 0x0001FE7B File Offset: 0x0001E07B
		public SingleplayerPersonalKillFeedItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x0001FEA8 File Offset: 0x0001E0A8
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
				this.UpdateTroopTypeVisualWidget();
				this._initialized = true;
			}
			this.UpdateAlphaValues(dt);
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x0001FEFC File Offset: 0x0001E0FC
		private void UpdateAlphaValues(float dt)
		{
			this.TimeSinceCreation += dt;
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

		// Token: 0x06000B88 RID: 2952 RVA: 0x0001FFE5 File Offset: 0x0001E1E5
		public void SetSpeedModifier(float newSpeed)
		{
			if (newSpeed > this._speedModifier)
			{
				this._speedModifier = newSpeed;
			}
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x0001FFF8 File Offset: 0x0001E1F8
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
				this.NotificationTypeIconWidget.SetState("MakeUnconscious");
				return;
			case 7:
				this.NotificationTypeIconWidget.SetState("NormalKillHeadshot");
				return;
			case 8:
				this.NotificationTypeIconWidget.SetState("MakeUnconsciousHeadshot");
				return;
			default:
				Debug.FailedAssert("Undefined personal feed notification type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Mission\\KillFeed\\Personal\\SingleplayerPersonalKillFeedItemWidget.cs", "UpdateNotificationTypeIconWidget", 126);
				this.NotificationTypeIconWidget.IsVisible = false;
				return;
			}
		}

		// Token: 0x06000B8A RID: 2954 RVA: 0x000200F8 File Offset: 0x0001E2F8
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
			case 6:
			case 7:
			case 8:
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
			default:
				Debug.FailedAssert("Undefined personal feed notification type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Mission\\KillFeed\\Personal\\SingleplayerPersonalKillFeedItemWidget.cs", "UpdateNotificationAmountWidget", 163);
				this.AmountTextWidget.IsVisible = false;
				return;
			}
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x000201D0 File Offset: 0x0001E3D0
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
			case 6:
			case 7:
			case 8:
				this.MessageTextWidget.SetState("Normal");
				return;
			case 1:
			case 2:
				this.MessageTextWidget.SetState("FriendlyFire");
				return;
			default:
				Debug.FailedAssert("Undefined personal feed notification type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Mission\\KillFeed\\Personal\\SingleplayerPersonalKillFeedItemWidget.cs", "UpdateNotificationMessageWidget", 196);
				this.MessageTextWidget.IsVisible = false;
				return;
			}
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x00020284 File Offset: 0x0001E484
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
			case 6:
			case 7:
			case 8:
				this.NotificationBackgroundWidget.SetState("Normal");
				return;
			case 5:
				break;
			default:
				Debug.FailedAssert("Undefined personal feed notification type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Mission\\KillFeed\\Personal\\SingleplayerPersonalKillFeedItemWidget.cs", "UpdateNotificationBackgroundWidget", 224);
				this.NotificationBackgroundWidget.SetState("Hidden");
				break;
			}
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x00020320 File Offset: 0x0001E520
		private void UpdateTroopTypeVisualWidget()
		{
			if (this._troopTypeWidget != null)
			{
				if (string.IsNullOrEmpty(this.TypeID))
				{
					this._troopTypeWidget.IsVisible = false;
					return;
				}
				this._troopTypeWidget.Sprite = this._troopTypeWidget.Context.SpriteData.GetSprite("General\\compass\\" + this._typeID);
			}
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06000B8E RID: 2958 RVA: 0x0002037F File Offset: 0x0001E57F
		// (set) Token: 0x06000B8F RID: 2959 RVA: 0x00020387 File Offset: 0x0001E587
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

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06000B90 RID: 2960 RVA: 0x000203A5 File Offset: 0x0001E5A5
		// (set) Token: 0x06000B91 RID: 2961 RVA: 0x000203AD File Offset: 0x0001E5AD
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

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06000B92 RID: 2962 RVA: 0x000203CB File Offset: 0x0001E5CB
		// (set) Token: 0x06000B93 RID: 2963 RVA: 0x000203D3 File Offset: 0x0001E5D3
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

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06000B94 RID: 2964 RVA: 0x000203F1 File Offset: 0x0001E5F1
		// (set) Token: 0x06000B95 RID: 2965 RVA: 0x000203F9 File Offset: 0x0001E5F9
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

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06000B96 RID: 2966 RVA: 0x0002041C File Offset: 0x0001E61C
		// (set) Token: 0x06000B97 RID: 2967 RVA: 0x00020424 File Offset: 0x0001E624
		public string TypeID
		{
			get
			{
				return this._typeID;
			}
			set
			{
				if (value != this._typeID)
				{
					this._typeID = value;
					base.OnPropertyChanged<string>(value, "TypeID");
				}
			}
		}

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06000B98 RID: 2968 RVA: 0x00020447 File Offset: 0x0001E647
		// (set) Token: 0x06000B99 RID: 2969 RVA: 0x00020450 File Offset: 0x0001E650
		public Widget TroopTypeWidget
		{
			get
			{
				return this._troopTypeWidget;
			}
			set
			{
				if (value != this._troopTypeWidget)
				{
					this._troopTypeWidget = value;
					if (!string.IsNullOrEmpty(this._typeID))
					{
						this._troopTypeWidget.Sprite = this._troopTypeWidget.Context.SpriteData.GetSprite("General\\compass\\" + this._typeID);
					}
				}
			}
		}

		// Token: 0x04000546 RID: 1350
		private bool _initialized;

		// Token: 0x04000547 RID: 1351
		private float _speedModifier;

		// Token: 0x04000548 RID: 1352
		private int _itemType;

		// Token: 0x04000549 RID: 1353
		private int _amount;

		// Token: 0x0400054A RID: 1354
		private string _typeID;

		// Token: 0x0400054B RID: 1355
		private string _message;

		// Token: 0x0400054C RID: 1356
		private Widget _troopTypeWidget;

		// Token: 0x0400054D RID: 1357
		private bool _isDamage;
	}
}
