using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class LeaveChannelMessage : Message
	{
		public ChatChannelType Channel { get; private set; }

		public LeaveChannelMessage(ChatChannelType channel)
		{
			this.Channel = channel;
		}
	}
}
