using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChannelMessage : Message
	{
		public ChatChannelType Channel { get; private set; }

		public string Message { get; private set; }

		public ChannelMessage(ChatChannelType channel, string message)
		{
			this.Channel = channel;
			this.Message = message;
		}
	}
}
