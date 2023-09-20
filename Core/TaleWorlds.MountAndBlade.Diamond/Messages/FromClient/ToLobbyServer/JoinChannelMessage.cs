using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class JoinChannelMessage : Message
	{
		[JsonProperty]
		public ChatChannelType Channel { get; private set; }

		public JoinChannelMessage()
		{
		}

		public JoinChannelMessage(ChatChannelType channel)
		{
			this.Channel = channel;
		}
	}
}
