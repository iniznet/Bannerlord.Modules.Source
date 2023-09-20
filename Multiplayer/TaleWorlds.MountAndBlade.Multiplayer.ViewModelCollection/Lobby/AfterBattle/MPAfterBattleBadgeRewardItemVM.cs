using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.AfterBattle
{
	public class MPAfterBattleBadgeRewardItemVM : MPAfterBattleRewardItemVM
	{
		public MPAfterBattleBadgeRewardItemVM(Badge badge)
		{
			base.Type = 1;
			base.Name = badge.Name.ToString();
			this.BadgeID = badge.StringId;
			this.RefreshValues();
		}

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

		private string _badgeID;
	}
}
