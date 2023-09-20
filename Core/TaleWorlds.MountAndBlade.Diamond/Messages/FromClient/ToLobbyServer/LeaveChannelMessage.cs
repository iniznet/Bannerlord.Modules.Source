using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class LeaveChannelMessage : Message
	{
		[JsonProperty]
		public ChatChannelType Channel { get; private set; }

		public LeaveChannelMessage()
		{
		}

		public LeaveChannelMessage(ChatChannelType channel)
		{
			this.Channel = channel;
		}
	}
}
