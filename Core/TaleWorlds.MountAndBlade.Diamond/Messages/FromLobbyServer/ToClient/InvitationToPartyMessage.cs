﻿using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class InvitationToPartyMessage : Message
	{
		[JsonProperty]
		public string InviterPlayerName { get; private set; }

		[JsonProperty]
		public PlayerId InviterPlayerId { get; private set; }

		public InvitationToPartyMessage()
		{
		}

		public InvitationToPartyMessage(string inviterPlayerName, PlayerId inviterPlayerId)
		{
			this.InviterPlayerName = inviterPlayerName;
			this.InviterPlayerId = inviterPlayerId;
		}
	}
}
