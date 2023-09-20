using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.KillFeed.Personal
{
	public class MPPersonalKillNotificationItemVM : ViewModel
	{
		private MPPersonalKillNotificationItemVM.ItemTypes ItemTypeAsEnum
		{
			get
			{
				return this._itemTypeAsEnum;
			}
			set
			{
				this._itemType = (int)value;
				this._itemTypeAsEnum = value;
			}
		}

		public MPPersonalKillNotificationItemVM(int amount, bool isFatal, bool isMountDamage, bool isFriendlyFire, bool isHeadshot, string killedAgentName, Action<MPPersonalKillNotificationItemVM> onRemoveItem)
		{
			this._onRemoveItem = onRemoveItem;
			this.Amount = amount;
			if (isFriendlyFire)
			{
				this.ItemTypeAsEnum = (isFatal ? MPPersonalKillNotificationItemVM.ItemTypes.FriendlyFireKill : MPPersonalKillNotificationItemVM.ItemTypes.FriendlyFireDamage);
				this.Message = killedAgentName;
				return;
			}
			if (isMountDamage)
			{
				this.ItemTypeAsEnum = MPPersonalKillNotificationItemVM.ItemTypes.MountDamage;
				this.Message = GameTexts.FindText("str_damage_delivered_message", null).ToString();
				return;
			}
			if (isFatal)
			{
				this.ItemTypeAsEnum = (isHeadshot ? MPPersonalKillNotificationItemVM.ItemTypes.HeadshotKill : MPPersonalKillNotificationItemVM.ItemTypes.NormalKill);
				this.Message = killedAgentName;
				return;
			}
			this.ItemTypeAsEnum = MPPersonalKillNotificationItemVM.ItemTypes.NormalDamage;
			this.Message = GameTexts.FindText("str_damage_delivered_message", null).ToString();
		}

		public MPPersonalKillNotificationItemVM(int amount, GoldGainFlags reasonType, Action<MPPersonalKillNotificationItemVM> onRemoveItem)
		{
			this._onRemoveItem = onRemoveItem;
			this.ItemTypeAsEnum = MPPersonalKillNotificationItemVM.ItemTypes.GoldChange;
			if (reasonType <= 64)
			{
				if (reasonType <= 8)
				{
					switch (reasonType)
					{
					case 1:
						this.Message = GameTexts.FindText("str_gold_gain_first_ranged_kill", null).ToString();
						goto IL_200;
					case 2:
						this.Message = GameTexts.FindText("str_gold_gain_first_melee_kill", null).ToString();
						goto IL_200;
					case 3:
						break;
					case 4:
						this.Message = GameTexts.FindText("str_gold_gain_first_assist", null).ToString();
						goto IL_200;
					default:
						if (reasonType == 8)
						{
							this.Message = GameTexts.FindText("str_gold_gain_second_assist", null).ToString();
							goto IL_200;
						}
						break;
					}
				}
				else
				{
					if (reasonType == 16)
					{
						this.Message = GameTexts.FindText("str_gold_gain_third_assist", null).ToString();
						goto IL_200;
					}
					if (reasonType == 32)
					{
						this.Message = GameTexts.FindText("str_gold_gain_fifth_kill", null).ToString();
						goto IL_200;
					}
					if (reasonType == 64)
					{
						this.Message = GameTexts.FindText("str_gold_gain_tenth_kill", null).ToString();
						goto IL_200;
					}
				}
			}
			else if (reasonType <= 256)
			{
				if (reasonType == 128)
				{
					this.Message = GameTexts.FindText("str_gold_gain_default_kill", null).ToString();
					goto IL_200;
				}
				if (reasonType == 256)
				{
					this.Message = GameTexts.FindText("str_gold_gain_default_assist", null).ToString();
					goto IL_200;
				}
			}
			else
			{
				if (reasonType == 512)
				{
					this.Message = GameTexts.FindText("str_gold_gain_objective_completed", null).ToString();
					goto IL_200;
				}
				if (reasonType == 1024)
				{
					this.Message = GameTexts.FindText("str_gold_gain_objective_destroyed", null).ToString();
					goto IL_200;
				}
				if (reasonType == 2048)
				{
					this.Message = GameTexts.FindText("str_gold_gain_perk_bonus", null).ToString();
					goto IL_200;
				}
			}
			Debug.FailedAssert("Undefined gold change type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\KillFeed\\Personal\\MPPersonalKillNotificationItemVM.cs", ".ctor", 117);
			this.Message = "";
			IL_200:
			this.Amount = amount;
		}

		public MPPersonalKillNotificationItemVM(string victimAgentName, Action<MPPersonalKillNotificationItemVM> onRemoveItem)
		{
			this._onRemoveItem = onRemoveItem;
			this.Amount = -1;
			this.Message = victimAgentName;
			this.ItemTypeAsEnum = MPPersonalKillNotificationItemVM.ItemTypes.Assist;
		}

		public void ExecuteRemove()
		{
			this._onRemoveItem(this);
		}

		[DataSourceProperty]
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
					base.OnPropertyChangedWithValue<string>(value, "Message");
				}
			}
		}

		[DataSourceProperty]
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
					base.OnPropertyChangedWithValue(value, "ItemType");
				}
			}
		}

		[DataSourceProperty]
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
					base.OnPropertyChangedWithValue(value, "Amount");
				}
			}
		}

		private Action<MPPersonalKillNotificationItemVM> _onRemoveItem;

		private MPPersonalKillNotificationItemVM.ItemTypes _itemTypeAsEnum;

		private string _message;

		private int _amount;

		private int _itemType;

		private enum ItemTypes
		{
			NormalDamage,
			FriendlyFireDamage,
			FriendlyFireKill,
			MountDamage,
			NormalKill,
			Assist,
			GoldChange,
			HeadshotKill
		}
	}
}
