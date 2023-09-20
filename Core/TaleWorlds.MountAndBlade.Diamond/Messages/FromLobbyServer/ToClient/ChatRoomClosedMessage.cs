using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ChatRoomClosedMessage : Message
	{
		[JsonProperty]
		public Guid ChatRoomId { get; private set; }

		public ChatRoomClosedMessage()
		{
		}

		public ChatRoomClosedMessage(Guid chatRoomId)
		{
			this.ChatRoomId = chatRoomId;
		}
	}
}
