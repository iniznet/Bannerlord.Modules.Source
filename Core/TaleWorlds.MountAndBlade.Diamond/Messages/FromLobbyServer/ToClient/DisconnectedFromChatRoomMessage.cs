using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace TaleWorlds.MountAndBlade.Diamond.Messages.FromLobbyServer.ToClient
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class DisconnectedFromChatRoomMessage : Message
	{
		[JsonProperty]
		public Guid RoomId { get; private set; }

		[JsonProperty]
		public string RoomName { get; private set; }

		public DisconnectedFromChatRoomMessage()
		{
		}

		public DisconnectedFromChatRoomMessage(Guid roomId, string roomName)
		{
			this.RoomId = roomId;
			this.RoomName = roomName;
		}
	}
}
