﻿using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CancelBattleResponseMessage : Message
	{
		[JsonProperty]
		public bool Successful { get; private set; }

		public CancelBattleResponseMessage()
		{
		}

		public CancelBattleResponseMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
