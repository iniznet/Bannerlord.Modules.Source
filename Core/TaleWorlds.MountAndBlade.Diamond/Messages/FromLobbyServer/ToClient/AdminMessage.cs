﻿using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class AdminMessage : Message
	{
		[JsonProperty]
		public string Message { get; private set; }

		public AdminMessage()
		{
		}

		public AdminMessage(string message)
		{
			this.Message = message;
		}
	}
}
