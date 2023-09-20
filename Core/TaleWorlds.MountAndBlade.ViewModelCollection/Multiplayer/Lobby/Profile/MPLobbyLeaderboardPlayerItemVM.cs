using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	// Token: 0x0200006A RID: 106
	public class MPLobbyLeaderboardPlayerItemVM : MPLobbyPlayerBaseVM
	{
		// Token: 0x060009D4 RID: 2516 RVA: 0x000240CC File Offset: 0x000222CC
		public MPLobbyLeaderboardPlayerItemVM(int rank, PlayerLeaderboardData playerLeaderboardData, Action<MPLobbyLeaderboardPlayerItemVM> onActivatePlayerActions)
			: base(playerLeaderboardData.PlayerId, playerLeaderboardData.Name, null, null)
		{
			this.Rank = rank;
			base.Rating = playerLeaderboardData.Rating;
			base.RatingID = playerLeaderboardData.RankId;
			this._onActivatePlayerActions = onActivatePlayerActions;
			this.RefreshValues();
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x00024119 File Offset: 0x00022319
		private void ExecuteActivatePlayerActions()
		{
			Action<MPLobbyLeaderboardPlayerItemVM> onActivatePlayerActions = this._onActivatePlayerActions;
			if (onActivatePlayerActions == null)
			{
				return;
			}
			onActivatePlayerActions(this);
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x060009D6 RID: 2518 RVA: 0x0002412C File Offset: 0x0002232C
		// (set) Token: 0x060009D7 RID: 2519 RVA: 0x00024134 File Offset: 0x00022334
		[DataSourceProperty]
		public int Rank
		{
			get
			{
				return this._rank;
			}
			set
			{
				if (value != this._rank)
				{
					this._rank = value;
					base.OnPropertyChangedWithValue(value, "Rank");
				}
			}
		}

		// Token: 0x040004CA RID: 1226
		public readonly MatchInfo MatchOfThePlayer;

		// Token: 0x040004CB RID: 1227
		private readonly Action<MPLobbyLeaderboardPlayerItemVM> _onActivatePlayerActions;

		// Token: 0x040004CC RID: 1228
		private int _rank;
	}
}
