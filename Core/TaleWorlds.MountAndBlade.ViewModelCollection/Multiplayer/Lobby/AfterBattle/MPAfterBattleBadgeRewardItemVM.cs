using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.AfterBattle
{
	// Token: 0x020000AA RID: 170
	public class MPAfterBattleBadgeRewardItemVM : MPAfterBattleRewardItemVM
	{
		// Token: 0x06001051 RID: 4177 RVA: 0x0003634A File Offset: 0x0003454A
		public MPAfterBattleBadgeRewardItemVM(Badge badge)
		{
			base.Type = 1;
			base.Name = badge.Name.ToString();
			this.BadgeID = badge.StringId;
			this.RefreshValues();
		}

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06001052 RID: 4178 RVA: 0x0003637C File Offset: 0x0003457C
		// (set) Token: 0x06001053 RID: 4179 RVA: 0x00036384 File Offset: 0x00034584
		[DataSourceProperty]
		public string BadgeID
		{
			get
			{
				return this._badgeID;
			}
			set
			{
				if (value != this._badgeID)
				{
					this._badgeID = value;
					base.OnPropertyChangedWithValue<string>(value, "BadgeID");
				}
			}
		}

		// Token: 0x040007B6 RID: 1974
		private string _badgeID;
	}
}
