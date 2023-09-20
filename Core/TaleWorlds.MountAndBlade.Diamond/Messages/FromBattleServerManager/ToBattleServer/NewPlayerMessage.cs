using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class NewPlayerMessage : Message
	{
		public PlayerBattleInfo PlayerBattleInfo { get; private set; }

		public PlayerData PlayerData { get; private set; }

		public Guid PlayerParty { get; private set; }

		public Dictionary<string, List<string>> UsedCosmetics { get; private set; }

		public NewPlayerMessage(PlayerData playerData, PlayerBattleInfo playerBattleInfo, Guid playerParty, Dictionary<string, List<string>> usedCosmetics)
		{
			this.PlayerBattleInfo = playerBattleInfo;
			this.PlayerData = playerData;
			this.PlayerParty = playerParty;
			this.UsedCosmetics = usedCosmetics;
		}
	}
}
