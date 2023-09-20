using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class JoinChannelMessage : Message
	{
		public ChatChannelType Channel { get; private set; }

		public JoinChannelMessage(ChatChannelType channel)
		{
			this.Channel = channel;
		}
	}
}
