using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed.Personal
{
	// Token: 0x020000B0 RID: 176
	public class MPPersonalKillNotificationItemVM : ViewModel
	{
		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x060010B6 RID: 4278 RVA: 0x000373D5 File Offset: 0x000355D5
		// (set) Token: 0x060010B7 RID: 4279 RVA: 0x000373DD File Offset: 0x000355DD
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

		// Token: 0x060010B8 RID: 4280 RVA: 0x000373F0 File Offset: 0x000355F0
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

		// Token: 0x060010B9 RID: 4281 RVA: 0x00037484 File Offset: 0x00035684
		public MPPersonalKillNotificationItemVM(int amount, GoldGainFlags reasonType, Action<MPPersonalKillNotificationItemVM> onRemoveItem)
		{
			this._onRemoveItem = onRemoveItem;
			this.ItemTypeAsEnum = MPPersonalKillNotificationItemVM.ItemTypes.GoldChange;
			if (reasonType <= GoldGainFlags.TenthKill)
			{
				if (reasonType <= GoldGainFlags.SecondAssist)
				{
					switch (reasonType)
					{
					case GoldGainFlags.FirstRangedKill:
						this.Message = GameTexts.FindText("str_gold_gain_first_ranged_kill", null).ToString();
						goto IL_200;
					case GoldGainFlags.FirstMeleeKill:
						this.Message = GameTexts.FindText("str_gold_gain_first_melee_kill", null).ToString();
						goto IL_200;
					case GoldGainFlags.FirstRangedKill | GoldGainFlags.FirstMeleeKill:
						break;
					case GoldGainFlags.FirstAssist:
						this.Message = GameTexts.FindText("str_gold_gain_first_assist", null).ToString();
						goto IL_200;
					default:
						if (reasonType == GoldGainFlags.SecondAssist)
						{
							this.Message = GameTexts.FindText("str_gold_gain_second_assist", null).ToString();
							goto IL_200;
						}
						break;
					}
				}
				else
				{
					if (reasonType == GoldGainFlags.ThirdAssist)
					{
						this.Message = GameTexts.FindText("str_gold_gain_third_assist", null).ToString();
						goto IL_200;
					}
					if (reasonType == GoldGainFlags.FifthKill)
					{
						this.Message = GameTexts.FindText("str_gold_gain_fifth_kill", null).ToString();
						goto IL_200;
					}
					if (reasonType == GoldGainFlags.TenthKill)
					{
						this.Message = GameTexts.FindText("str_gold_gain_tenth_kill", null).ToString();
						goto IL_200;
					}
				}
			}
			else if (reasonType <= GoldGainFlags.DefaultAssist)
			{
				if (reasonType == GoldGainFlags.DefaultKill)
				{
					this.Message = GameTexts.FindText("str_gold_gain_default_kill", null).ToString();
					goto IL_200;
				}
				if (reasonType == GoldGainFlags.DefaultAssist)
				{
					this.Message = GameTexts.FindText("str_gold_gain_default_assist", null).ToString();
					goto IL_200;
				}
			}
			else
			{
				if (reasonType == GoldGainFlags.ObjectiveCompleted)
				{
					this.Message = GameTexts.FindText("str_gold_gain_objective_completed", null).ToString();
					goto IL_200;
				}
				if (reasonType == GoldGainFlags.ObjectiveDestroyed)
				{
					this.Message = GameTexts.FindText("str_gold_gain_objective_destroyed", null).ToString();
					goto IL_200;
				}
				if (reasonType == GoldGainFlags.PerkBonus)
				{
					this.Message = GameTexts.FindText("str_gold_gain_perk_bonus", null).ToString();
					goto IL_200;
				}
			}
			Debug.FailedAssert("Undefined gold change type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\KillFeed\\Personal\\MPPersonalKillNotificationItemVM.cs", ".ctor", 117);
			this.Message = "";
			IL_200:
			this.Amount = amount;
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x00037698 File Offset: 0x00035898
		public MPPersonalKillNotificationItemVM(string victimAgentName, Action<MPPersonalKillNotificationItemVM> onRemoveItem)
		{
			this._onRemoveItem = onRemoveItem;
			this.Amount = -1;
			this.Message = victimAgentName;
			this.ItemTypeAsEnum = MPPersonalKillNotificationItemVM.ItemTypes.Assist;
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x000376BC File Offset: 0x000358BC
		public void ExecuteRemove()
		{
			this._onRemoveItem(this);
		}

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x060010BC RID: 4284 RVA: 0x000376CA File Offset: 0x000358CA
		// (set) Token: 0x060010BD RID: 4285 RVA: 0x000376D2 File Offset: 0x000358D2
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

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x060010BE RID: 4286 RVA: 0x000376F5 File Offset: 0x000358F5
		// (set) Token: 0x060010BF RID: 4287 RVA: 0x000376FD File Offset: 0x000358FD
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

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x060010C0 RID: 4288 RVA: 0x0003771B File Offset: 0x0003591B
		// (set) Token: 0x060010C1 RID: 4289 RVA: 0x00037723 File Offset: 0x00035923
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

		// Token: 0x040007F2 RID: 2034
		private Action<MPPersonalKillNotificationItemVM> _onRemoveItem;

		// Token: 0x040007F3 RID: 2035
		private MPPersonalKillNotificationItemVM.ItemTypes _itemTypeAsEnum;

		// Token: 0x040007F4 RID: 2036
		private string _message;

		// Token: 0x040007F5 RID: 2037
		private int _amount;

		// Token: 0x040007F6 RID: 2038
		private int _itemType;

		// Token: 0x02000212 RID: 530
		private enum ItemTypes
		{
			// Token: 0x04000E72 RID: 3698
			NormalDamage,
			// Token: 0x04000E73 RID: 3699
			FriendlyFireDamage,
			// Token: 0x04000E74 RID: 3700
			FriendlyFireKill,
			// Token: 0x04000E75 RID: 3701
			MountDamage,
			// Token: 0x04000E76 RID: 3702
			NormalKill,
			// Token: 0x04000E77 RID: 3703
			Assist,
			// Token: 0x04000E78 RID: 3704
			GoldChange,
			// Token: 0x04000E79 RID: 3705
			HeadshotKill
		}
	}
}
