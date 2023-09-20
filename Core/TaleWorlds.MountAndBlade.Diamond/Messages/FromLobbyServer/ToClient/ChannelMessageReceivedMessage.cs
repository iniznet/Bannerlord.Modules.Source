﻿using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ChannelMessageReceivedMessage : Message
	{
		[JsonProperty]
		public ChatChannelType Channel { get; private set; }

		[JsonProperty]
		public string PlayerName { get; private set; }

		[JsonProperty]
		public string Message { get; private set; }

		public ChannelMessageReceivedMessage()
		{
		}

		public ChannelMessageReceivedMessage(ChatChannelType channel, string playerName, string message)
		{
			this.Channel = channel;
			this.PlayerName = playerName;
			this.Message = message;
		}
	}
}
