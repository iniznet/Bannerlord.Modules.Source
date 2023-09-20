using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public class BattlePeer
	{
		public int Index { get; private set; }

		public string Name { get; private set; }

		public PlayerId PlayerId
		{
			get
			{
				return this.PlayerData.PlayerId;
			}
		}

		public int TeamNo { get; private set; }

		public BattleJoinType BattleJoinType { get; private set; }

		public bool Quit
		{
			get
			{
				return this.QuitType > BattlePeerQuitType.None;
			}
		}

		public PlayerData PlayerData { get; private set; }

		public Dictionary<string, List<string>> UsedCosmetics { get; private set; }

		public int SessionKey { get; private set; }

		public BattlePeerQuitType QuitType { get; private set; }

		public BattlePeer(string name, PlayerData playerData, Dictionary<string, List<string>> usedCosmetics, int teamNo, BattleJoinType battleJoinType)
		{
			this.Index = -1;
			this.Name = name;
			this.PlayerData = playerData;
			this.UsedCosmetics = usedCosmetics;
			this.TeamNo = teamNo;
			this.BattleJoinType = battleJoinType;
		}

		internal void Flee()
		{
			this.QuitType = BattlePeerQuitType.Fled;
			this.Index = -1;
			this.SessionKey = 0;
		}

		internal void SetPlayerDisconnectdFromLobby()
		{
			this.QuitType = BattlePeerQuitType.DisconnectedFromLobby;
			this.Index = -1;
			this.SessionKey = 0;
		}

		internal void SetPlayerDisconnectdFromGameSession()
		{
			this.QuitType = BattlePeerQuitType.DisconnectedFromGameSession;
			this.Index = -1;
			this.SessionKey = 0;
		}

		public void Rejoin(int teamNo)
		{
			this.QuitType = BattlePeerQuitType.None;
			this.TeamNo = teamNo;
		}

		public void InitializeSession(int index, int sessionKey)
		{
			this.Index = index;
			this.SessionKey = sessionKey;
		}

		internal void SetPlayerKickedDueToFriendlyDamage()
		{
			this.QuitType = BattlePeerQuitType.KickedDueToFriendlyDamage;
			this.Index = -1;
			this.SessionKey = 0;
		}
	}
}
