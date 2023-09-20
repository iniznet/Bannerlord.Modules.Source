using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.KillFeed
{
	public class MultiplayerPersonalKillFeedItemWidget : Widget
	{
		public Widget NotificationTypeIconWidget { get; set; }

		public Widget NotificationBackgroundWidget { get; set; }

		public TextWidget AmountTextWidget { get; set; }

		public RichTextWidget MessageTextWidget { get; set; }

		public float FadeInTime { get; set; } = 1f;

		public float StayTime { get; set; } = 3f;

		public float FadeOutTime { get; set; } = 0.5f;

		private float CurrentAlpha
		{
			get
			{
				return base.AlphaFactor;
			}
		}

		public float TimeSinceCreation { get; private set; }

		public MultiplayerPersonalKillFeedItemWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this._initialized)
			{
				base.PositionYOffset = 0f;
			}
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
				this.DetermineSoundEvent();
				this._initialized = true;
			}
			this.UpdateAlphaValues(dt);
		}

		private void DetermineSoundEvent()
		{
			if (this.ItemType == 6)
			{
				base.Context.TwoDimensionContext.PlaySound(this._goldGainedSound);
			}
		}

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

		public void SetSpeedModifier(float newSpeed)
		{
			if (newSpeed > this._speedModifier)
			{
				this._speedModifier = newSpeed;
			}
		}

		public void SetMaxAlphaValue(float newMaxAlpha)
		{
			if (newMaxAlpha < this._maxTargetAlpha)
			{
				this._maxTargetAlpha = newMaxAlpha;
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

		private float GetInitialVerticalPositionOfSelf()
		{
			float num = 0f;
			for (int i = 0; i < base.GetSiblingIndex(); i++)
			{
				num += base.ParentWidget.GetChild(i).Size.Y * base._inverseScaleToUse;
			}
			return num;
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

		private float _speedModifier = 1f;

		private float _maxTargetAlpha = 1f;

		private bool _initialized;

		private string _goldGainedSound = "multiplayer/coin_add";

		private bool _isDamage;

		private int _itemType;

		private int _amount = -1;

		private string _message;
	}
}
