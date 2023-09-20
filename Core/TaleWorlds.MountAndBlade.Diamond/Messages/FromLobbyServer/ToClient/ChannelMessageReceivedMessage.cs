using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ChannelMessageReceivedMessage : Message
	{
		public ChatChannelType Channel { get; private set; }

		public string PlayerName { get; private set; }

		public string Message { get; private set; }

		public ChannelMessageReceivedMessage(ChatChannelType channel, string playerName, string message)
		{
			this.Channel = channel;
			this.PlayerName = playerName;
			this.Message = message;
		}
	}
}
