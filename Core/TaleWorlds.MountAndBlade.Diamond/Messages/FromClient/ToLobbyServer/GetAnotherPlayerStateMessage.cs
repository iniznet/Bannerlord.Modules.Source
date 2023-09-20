﻿using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetAnotherPlayerStateMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public GetAnotherPlayerStateMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
