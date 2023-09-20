﻿using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PlayerAssignedPartyLeaderMessage : Message
	{
		[JsonProperty]
		public PlayerId PartyLeaderId { get; private set; }

		public PlayerAssignedPartyLeaderMessage()
		{
		}

		public PlayerAssignedPartyLeaderMessage(PlayerId partyLeaderId)
		{
			this.PartyLeaderId = partyLeaderId;
		}
	}
}
