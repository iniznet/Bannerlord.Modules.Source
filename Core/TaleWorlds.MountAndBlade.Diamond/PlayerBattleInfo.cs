using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000136 RID: 310
	[Serializable]
	public class PlayerBattleInfo
	{
		// Token: 0x17000287 RID: 647
		// (get) Token: 0x0600075C RID: 1884 RVA: 0x0000C038 File Offset: 0x0000A238
		// (set) Token: 0x0600075D RID: 1885 RVA: 0x0000C040 File Offset: 0x0000A240
		public PlayerId PlayerId { get; private set; }

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x0600075E RID: 1886 RVA: 0x0000C049 File Offset: 0x0000A249
		// (set) Token: 0x0600075F RID: 1887 RVA: 0x0000C051 File Offset: 0x0000A251
		public string Name { get; private set; }

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06000760 RID: 1888 RVA: 0x0000C05A File Offset: 0x0000A25A
		// (set) Token: 0x06000761 RID: 1889 RVA: 0x0000C062 File Offset: 0x0000A262
		public int TeamNo { get; private set; }

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06000762 RID: 1890 RVA: 0x0000C06B File Offset: 0x0000A26B
		public bool Fled
		{
			get
			{
				return this._state == PlayerBattleInfo.State.Fled;
			}
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06000763 RID: 1891 RVA: 0x0000C076 File Offset: 0x0000A276
		public bool Disconnected
		{
			get
			{
				return this._state == PlayerBattleInfo.State.Disconnected;
			}
		}

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x06000764 RID: 1892 RVA: 0x0000C081 File Offset: 0x0000A281
		// (set) Token: 0x06000765 RID: 1893 RVA: 0x0000C089 File Offset: 0x0000A289
		public BattleJoinType JoinType { get; private set; }

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06000766 RID: 1894 RVA: 0x0000C092 File Offset: 0x0000A292
		// (set) Token: 0x06000767 RID: 1895 RVA: 0x0000C09A File Offset: 0x0000A29A
		public int PeerIndex { get; private set; }

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06000768 RID: 1896 RVA: 0x0000C0A3 File Offset: 0x0000A2A3
		public PlayerBattleInfo.State CurrentState
		{
			get
			{
				return this._state;
			}
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x0000C0AB File Offset: 0x0000A2AB
		public PlayerBattleInfo(PlayerId playerId, string name, int teamNo)
		{
			this.PlayerId = playerId;
			this.Name = name;
			this.TeamNo = teamNo;
			this.PeerIndex = -1;
			this._state = PlayerBattleInfo.State.AssignedToBattle;
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x0000C0D6 File Offset: 0x0000A2D6
		public PlayerBattleInfo(PlayerId playerId, string name, int teamNo, int peerIndex, PlayerBattleInfo.State state)
		{
			this.PlayerId = playerId;
			this.Name = name;
			this.TeamNo = teamNo;
			this.PeerIndex = peerIndex;
			this._state = state;
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x0000C103 File Offset: 0x0000A303
		public void Flee()
		{
			if (this._state != PlayerBattleInfo.State.Disconnected && this._state != PlayerBattleInfo.State.AtBattle)
			{
				throw new Exception("PlayerBattleInfo incorrect state, expected AtBattle or Disconnected; got " + this._state);
			}
			this._state = PlayerBattleInfo.State.Fled;
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x0000C139 File Offset: 0x0000A339
		public void Disconnect()
		{
			if (this._state != PlayerBattleInfo.State.AtBattle)
			{
				throw new Exception("PlayerBattleInfo incorrect state, expected AtBattle got " + this._state);
			}
			this._state = PlayerBattleInfo.State.Disconnected;
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x0000C166 File Offset: 0x0000A366
		public void Initialize(int peerIndex)
		{
			if (this._state != PlayerBattleInfo.State.AssignedToBattle)
			{
				throw new Exception("PlayerBattleInfo incorrect state, expected AssignedToBattle got " + this._state);
			}
			this.PeerIndex = peerIndex;
			this._state = PlayerBattleInfo.State.AtBattle;
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x0000C19A File Offset: 0x0000A39A
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

		// Token: 0x0600076F RID: 1903 RVA: 0x0000C1D5 File Offset: 0x0000A3D5
		public PlayerBattleInfo Clone()
		{
			return new PlayerBattleInfo(this.PlayerId, this.Name, this.TeamNo, this.PeerIndex, this._state);
		}

		// Token: 0x04000368 RID: 872
		private PlayerBattleInfo.State _state;

		// Token: 0x020001B6 RID: 438
		public enum State
		{
			// Token: 0x04000644 RID: 1604
			Created,
			// Token: 0x04000645 RID: 1605
			AssignedToBattle,
			// Token: 0x04000646 RID: 1606
			AtBattle,
			// Token: 0x04000647 RID: 1607
			Disconnected,
			// Token: 0x04000648 RID: 1608
			Fled
		}
	}
}
