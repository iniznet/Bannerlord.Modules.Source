using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.AfterBattle
{
	// Token: 0x020000AD RID: 173
	public abstract class MPAfterBattleRewardItemVM : ViewModel
	{
		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x06001094 RID: 4244 RVA: 0x00037014 File Offset: 0x00035214
		// (set) Token: 0x06001095 RID: 4245 RVA: 0x0003701C File Offset: 0x0003521C
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value != this._type)
				{
					this._type = value;
					base.OnPropertyChangedWithValue(value, "Type");
				}
			}
		}

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x06001096 RID: 4246 RVA: 0x0003703A File Offset: 0x0003523A
		// (set) Token: 0x06001097 RID: 4247 RVA: 0x00037042 File Offset: 0x00035242
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x040007E5 RID: 2021
		private int _type;

		// Token: 0x040007E6 RID: 2022
		private string _name;

		// Token: 0x02000211 RID: 529
		public enum RewardType
		{
			// Token: 0x04000E6F RID: 3695
			Loot,
			// Token: 0x04000E70 RID: 3696
			Badge
		}
	}
}
