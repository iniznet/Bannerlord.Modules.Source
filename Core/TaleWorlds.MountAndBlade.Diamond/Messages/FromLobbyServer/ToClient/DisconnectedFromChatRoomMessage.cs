using System;
using TaleWorlds.Diamond;

namespace TaleWorlds.MountAndBlade.Diamond.Messages.FromLobbyServer.ToClient
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class DisconnectedFromChatRoomMessage : Message
	{
		public Guid RoomId { get; private set; }

		public string RoomName { get; private set; }

		public DisconnectedFromChatRoomMessage(Guid roomId, string roomName)
		{
			this.RoomId = roomId;
			this.RoomName = roomName;
		}
	}
}
