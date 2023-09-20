using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PlayerBattleInfo
	{
		public PlayerId PlayerId { get; set; }

		public string Name { get; set; }

		public int TeamNo { get; set; }

		public bool Fled
		{
			get
			{
				return this._state == PlayerBattleInfo.State.Fled;
			}
		}

		public bool Disconnected
		{
			get
			{
				return this._state == PlayerBattleInfo.State.Disconnected;
			}
		}

		public BattleJoinType JoinType { get; set; }

		public int PeerIndex { get; set; }

		public PlayerBattleInfo.State CurrentState
		{
			get
			{
				return this._state;
			}
		}

		public PlayerBattleInfo()
		{
		}

		public PlayerBattleInfo(PlayerId playerId, string name, int teamNo)
		{
			this.PlayerId = playerId;
			this.Name = name;
			this.TeamNo = teamNo;
			this.PeerIndex = -1;
			this._state = PlayerBattleInfo.State.AssignedToBattle;
		}

		public PlayerBattleInfo(PlayerId playerId, string name, int teamNo, int peerIndex, PlayerBattleInfo.State state)
		{
			this.PlayerId = playerId;
			this.Name = name;
			this.TeamNo = teamNo;
			this.PeerIndex = peerIndex;
			this._state = state;
		}

		public void Flee()
		{
			if (this._state != PlayerBattleInfo.State.Disconnected && this._state != PlayerBattleInfo.State.AtBattle)
			{
				throw new Exception("PlayerBattleInfo incorrect state, expected AtBattle or Disconnected; got " + this._state);
			}
			this._state = PlayerBattleInfo.State.Fled;
		}

		public void Disconnect()
		{
			if (this._state != PlayerBattleInfo.State.AtBattle)
			{
				throw new Exception("PlayerBattleInfo incorrect state, expected AtBattle got " + this._state);
			}
			this._state = PlayerBattleInfo.State.Disconnected;
		}

		public void Initialize(int peerIndex)
		{
			if (this._state != PlayerBattleInfo.State.AssignedToBattle)
			{
				throw new Exception("PlayerBattleInfo incorrect state, expected AssignedToBattle got " + this._state);
			}
			this.PeerIndex = peerIndex;
			this._state = PlayerBattleInfo.State.AtBattle;
		}

		public void RejoinBattle(int teamNo)
		{
			if (this._state != PlayerBattleInfo.State.Disconnected)
			{
				throw new Exception("PlayerBattleInfo incorrect state, expected Fled got " + this._state);
			}
			this.TeamNo = teamNo;
			this.PeerIndex = -1;
			this._state = PlayerBattleInfo.State.AssignedToBattle;
		}

		public PlayerBattleInfo Clone()
		{
			return new PlayerBattleInfo(this.PlayerId, this.Name, this.TeamNo, this.PeerIndex, this._state);
		}

		private PlayerBattleInfo.State _state;

		public enum State
		{
			Created,
			AssignedToBattle,
			AtBattle,
			Disconnected,
			Fled
		}
	}
}
