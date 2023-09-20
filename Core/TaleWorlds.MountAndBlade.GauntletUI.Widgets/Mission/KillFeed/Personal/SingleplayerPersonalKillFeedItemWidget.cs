using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.KillFeed.Personal
{
	public class SingleplayerPersonalKillFeedItemWidget : Widget
	{
		public Widget NotificationTypeIconWidget { get; set; }

		public Widget NotificationBackgroundWidget { get; set; }

		public TextWidget AmountTextWidget { get; set; }

		public RichTextWidget MessageTextWidget { get; set; }

		public float FadeInTime { get; set; } = 0.2f;

		public float StayTime { get; set; } = 2f;

		public float FadeOutTime { get; set; } = 0.2f;

		private float CurrentAlpha
		{
			get
			{
				return base.AlphaFactor;
			}
		}

		public float TimeSinceCreation { get; private set; }

		public SingleplayerPersonalKillFeedItemWidget(UIContext context)
			: base(context)
		{
		}

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

		public void SetSpeedModifier(float newSpeed)
		{
			if (newSpeed > this._speedModifier)
			{
				this._speedModifier = newSpeed;
			}
		}

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

		private bool _initialized;

		private float _speedModifier;

		private int _itemType;

		private int _amount;

		private string _typeID;

		private string _message;

		private Widget _troopTypeWidget;

		private bool _isDamage;
	}
}
