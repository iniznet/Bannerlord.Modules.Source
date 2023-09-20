using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Friends;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Profile
{
	public class MPLobbyLeaderboardPlayerItemVM : MPLobbyPlayerBaseVM
	{
		public MPLobbyLeaderboardPlayerItemVM(int rank, PlayerLeaderboardData playerLeaderboardData, Action<MPLobbyLeaderboardPlayerItemVM> onActivatePlayerActions)
			: base(playerLeaderboardData.PlayerId, playerLeaderboardData.Name, null, null)
		{
			this.Rank = rank;
			base.Rating = playerLeaderboardData.Rating;
			base.RatingID = playerLeaderboardData.RankId;
			this._onActivatePlayerActions = onActivatePlayerActions;
			this.RefreshValues();
		}

		private void ExecuteActivatePlayerActions()
		{
			Action<MPLobbyLeaderboardPlayerItemVM> onActivatePlayerActions = this._onActivatePlayerActions;
			if (onActivatePlayerActions == null)
			{
				return;
			}
			onActivatePlayerActions(this);
		}

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

		public readonly MatchInfo MatchOfThePlayer;

		private readonly Action<MPLobbyLeaderboardPlayerItemVM> _onActivatePlayerActions;

		private int _rank;
	}
}
