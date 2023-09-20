using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangePlayerSigilMessage : Message
	{
		[JsonProperty]
		public string SigilId { get; private set; }

		public ChangePlayerSigilMessage()
		{
		}

		public ChangePlayerSigilMessage(string sigilId)
		{
			this.SigilId = sigilId;
		}
	}
}
