using System;
using Newtonsoft.Json;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class ChatRoomInformationForClient
	{
		[JsonProperty]
		public Guid RoomId { get; private set; }

		[JsonProperty]
		public string Name { get; private set; }

		[JsonProperty]
		public string Endpoint { get; private set; }

		[JsonProperty]
		public string RoomColor { get; private set; }

		public ChatRoomInformationForClient()
		{
		}

		public ChatRoomInformationForClient(Guid roomId, string name, string endpoint, string color)
		{
			this.RoomId = roomId;
			this.Name = name;
			this.Endpoint = endpoint;
			this.RoomColor = color;
		}
	}
}
