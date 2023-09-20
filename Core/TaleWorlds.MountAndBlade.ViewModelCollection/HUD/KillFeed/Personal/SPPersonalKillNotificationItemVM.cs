using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.Personal
{
	// Token: 0x020000E6 RID: 230
	public class SPPersonalKillNotificationItemVM : ViewModel
	{
		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x060014D4 RID: 5332 RVA: 0x00043E7A File Offset: 0x0004207A
		// (set) Token: 0x060014D5 RID: 5333 RVA: 0x00043E82 File Offset: 0x00042082
		private SPPersonalKillNotificationItemVM.ItemTypes ItemTypeAsEnum
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

		// Token: 0x060014D6 RID: 5334 RVA: 0x00043E94 File Offset: 0x00042094
		public SPPersonalKillNotificationItemVM(int damageAmount, bool isMountDamage, bool isFriendlyFire, bool isHeadshot, string killedAgentName, bool isUnconscious, Action<SPPersonalKillNotificationItemVM> onRemoveItem)
		{
			this._onRemoveItem = onRemoveItem;
			this.Amount = damageAmount;
			if (isFriendlyFire)
			{
				this.ItemTypeAsEnum = SPPersonalKillNotificationItemVM.ItemTypes.FriendlyFireKill;
				this.Message = killedAgentName;
				return;
			}
			if (isMountDamage)
			{
				this.ItemTypeAsEnum = SPPersonalKillNotificationItemVM.ItemTypes.MountDamage;
				this.Message = GameTexts.FindText("str_damage_delivered_message", null).ToString();
				return;
			}
			this.ItemTypeAsEnum = (isUnconscious ? (isHeadshot ? SPPersonalKillNotificationItemVM.ItemTypes.MakeUnconsciousHeadshot : SPPersonalKillNotificationItemVM.ItemTypes.MakeUnconscious) : (isHeadshot ? SPPersonalKillNotificationItemVM.ItemTypes.NormalKillHeadshot : SPPersonalKillNotificationItemVM.ItemTypes.NormalKill));
			this.Message = killedAgentName;
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x00043F10 File Offset: 0x00042110
		public SPPersonalKillNotificationItemVM(int amount, bool isMountDamage, bool isFriendlyFire, string killedAgentName, Action<SPPersonalKillNotificationItemVM> onRemoveItem)
		{
			this._onRemoveItem = onRemoveItem;
			this.Amount = amount;
			if (isFriendlyFire)
			{
				this.ItemTypeAsEnum = SPPersonalKillNotificationItemVM.ItemTypes.FriendlyFireDamage;
				this.Message = killedAgentName;
				return;
			}
			if (isMountDamage)
			{
				this.ItemTypeAsEnum = SPPersonalKillNotificationItemVM.ItemTypes.MountDamage;
				this.Message = GameTexts.FindText("str_damage_delivered_message", null).ToString();
				return;
			}
			this.ItemTypeAsEnum = SPPersonalKillNotificationItemVM.ItemTypes.NormalDamage;
			this.Message = GameTexts.FindText("str_damage_delivered_message", null).ToString();
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x00043F83 File Offset: 0x00042183
		public SPPersonalKillNotificationItemVM(string victimAgentName, Action<SPPersonalKillNotificationItemVM> onRemoveItem)
		{
			this._onRemoveItem = onRemoveItem;
			this.Amount = -1;
			this.Message = victimAgentName;
			this.ItemTypeAsEnum = SPPersonalKillNotificationItemVM.ItemTypes.Assist;
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x00043FA7 File Offset: 0x000421A7
		public void ExecuteRemove()
		{
			this._onRemoveItem(this);
		}

		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x060014DA RID: 5338 RVA: 0x00043FB5 File Offset: 0x000421B5
		// (set) Token: 0x060014DB RID: 5339 RVA: 0x00043FBD File Offset: 0x000421BD
		[DataSourceProperty]
		public string VictimType
		{
			get
			{
				return this._victimType;
			}
			set
			{
				if (value != this._victimType)
				{
					this._victimType = value;
					base.OnPropertyChangedWithValue<string>(value, "VictimType");
				}
			}
		}

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x060014DC RID: 5340 RVA: 0x00043FE0 File Offset: 0x000421E0
		// (set) Token: 0x060014DD RID: 5341 RVA: 0x00043FE8 File Offset: 0x000421E8
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

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x060014DE RID: 5342 RVA: 0x0004400B File Offset: 0x0004220B
		// (set) Token: 0x060014DF RID: 5343 RVA: 0x00044013 File Offset: 0x00042213
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

		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x060014E0 RID: 5344 RVA: 0x00044031 File Offset: 0x00042231
		// (set) Token: 0x060014E1 RID: 5345 RVA: 0x00044039 File Offset: 0x00042239
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

		// Token: 0x040009F4 RID: 2548
		private Action<SPPersonalKillNotificationItemVM> _onRemoveItem;

		// Token: 0x040009F5 RID: 2549
		private SPPersonalKillNotificationItemVM.ItemTypes _itemTypeAsEnum;

		// Token: 0x040009F6 RID: 2550
		private string _message;

		// Token: 0x040009F7 RID: 2551
		private string _victimType;

		// Token: 0x040009F8 RID: 2552
		private int _amount;

		// Token: 0x040009F9 RID: 2553
		private int _itemType;

		// Token: 0x02000234 RID: 564
		private enum ItemTypes
		{
			// Token: 0x04000ED7 RID: 3799
			NormalDamage,
			// Token: 0x04000ED8 RID: 3800
			FriendlyFireDamage,
			// Token: 0x04000ED9 RID: 3801
			FriendlyFireKill,
			// Token: 0x04000EDA RID: 3802
			MountDamage,
			// Token: 0x04000EDB RID: 3803
			NormalKill,
			// Token: 0x04000EDC RID: 3804
			Assist,
			// Token: 0x04000EDD RID: 3805
			MakeUnconscious,
			// Token: 0x04000EDE RID: 3806
			NormalKillHeadshot,
			// Token: 0x04000EDF RID: 3807
			MakeUnconsciousHeadshot
		}
	}
}
