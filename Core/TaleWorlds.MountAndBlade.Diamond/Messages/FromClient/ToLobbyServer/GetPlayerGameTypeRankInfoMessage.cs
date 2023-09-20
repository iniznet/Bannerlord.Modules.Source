﻿using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetPlayerGameTypeRankInfoMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		public GetPlayerGameTypeRankInfoMessage()
		{
		}

		public GetPlayerGameTypeRankInfoMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
