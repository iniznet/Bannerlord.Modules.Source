using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ChatRoomClosedMessage : Message
	{
		public Guid ChatRoomId { get; private set; }

		public ChatRoomClosedMessage(Guid chatRoomId)
		{
			this.ChatRoomId = chatRoomId;
		}
	}
}
