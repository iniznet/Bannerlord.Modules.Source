using System;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	public class MPLobbyRecentGamePlayerItemVM : MPLobbyPlayerBaseVM
	{
		public MPLobbyRecentGamePlayerItemVM(PlayerId playerId, MatchInfo matchOfThePlayer, Action<MPLobbyRecentGamePlayerItemVM> onActivatePlayerActions)
			: base(playerId, "", null, null)
		{
			this.MatchOfThePlayer = matchOfThePlayer;
			this._onActivatePlayerActions = onActivatePlayerActions;
			PlayerInfo playerInfo = this.MatchOfThePlayer.Players.FirstOrDefault((PlayerInfo p) => p.PlayerId == playerId.ToString());
			if (playerInfo != null)
			{
				this.KillCount = playerInfo.Kill;
				this.DeathCount = playerInfo.Death;
				this.AssistCount = playerInfo.Assist;
			}
			this.RefreshValues();
		}

		private void ExecuteActivatePlayerActions()
		{
			Action<MPLobbyRecentGamePlayerItemVM> onActivatePlayerActions = this._onActivatePlayerActions;
			if (onActivatePlayerActions == null)
			{
				return;
			}
			onActivatePlayerActions(this);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		[DataSourceProperty]
		public int KillCount
		{
			get
			{
				return this._killCount;
			}
			set
			{
				if (value != this._killCount)
				{
					this._killCount = value;
					base.OnPropertyChangedWithValue(value, "KillCount");
				}
			}
		}

		[DataSourceProperty]
		public int DeathCount
		{
			get
			{
				return this._deathCount;
			}
			set
			{
				if (value != this._deathCount)
				{
					this._deathCount = value;
					base.OnPropertyChangedWithValue(value, "DeathCount");
				}
			}
		}

		[DataSourceProperty]
		public int AssistCount
		{
			get
			{
				return this._assistCount;
			}
			set
			{
				if (value != this._assistCount)
				{
					this._assistCount = value;
					base.OnPropertyChangedWithValue(value, "AssistCount");
				}
			}
		}

		public readonly MatchInfo MatchOfThePlayer;

		private readonly Action<MPLobbyRecentGamePlayerItemVM> _onActivatePlayerActions;

		private int _killCount;

		private int _deathCount;

		private int _assistCount;
	}
}
