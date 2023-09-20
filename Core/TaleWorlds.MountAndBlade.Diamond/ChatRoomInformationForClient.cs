using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class ChatRoomInformationForClient
	{
		public Guid RoomId { get; private set; }

		public string Name { get; private set; }

		public string Endpoint { get; private set; }

		public string RoomColor { get; private set; }

		public ChatRoomInformationForClient(Guid roomId, string name, string endpoint, string color)
		{
			this.RoomId = roomId;
			this.Name = name;
			this.Endpoint = endpoint;
			this.RoomColor = color;
		}
	}
}
