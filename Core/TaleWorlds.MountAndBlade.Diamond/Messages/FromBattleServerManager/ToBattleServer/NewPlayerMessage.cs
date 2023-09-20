using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class NewPlayerMessage : Message
	{
		[JsonProperty]
		public PlayerBattleInfo PlayerBattleInfo { get; private set; }

		[JsonProperty]
		public PlayerData PlayerData { get; private set; }

		[JsonProperty]
		public Guid PlayerParty { get; private set; }

		[JsonProperty]
		public Dictionary<string, List<string>> UsedCosmetics { get; private set; }

		public NewPlayerMessage()
		{
		}

		public NewPlayerMessage(PlayerData playerData, PlayerBattleInfo playerBattleInfo, Guid playerParty, Dictionary<string, List<string>> usedCosmetics)
		{
			this.PlayerBattleInfo = playerBattleInfo;
			this.PlayerData = playerData;
			this.PlayerParty = playerParty;
			this.UsedCosmetics = usedCosmetics;
		}
	}
}
