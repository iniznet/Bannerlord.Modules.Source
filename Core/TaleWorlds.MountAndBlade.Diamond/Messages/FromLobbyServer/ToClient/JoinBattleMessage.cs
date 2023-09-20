﻿using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class JoinBattleMessage : Message
	{
		[JsonProperty]
		public BattleServerInformationForClient BattleServerInformation { get; private set; }

		public JoinBattleMessage()
		{
		}

		public JoinBattleMessage(BattleServerInformationForClient battleServerInformation)
		{
			this.BattleServerInformation = battleServerInformation;
		}
	}
}
