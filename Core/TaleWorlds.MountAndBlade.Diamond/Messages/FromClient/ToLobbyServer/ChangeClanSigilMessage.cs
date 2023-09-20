using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeClanSigilMessage : Message
	{
		[JsonProperty]
		public string NewSigil { get; private set; }

		public ChangeClanSigilMessage()
		{
		}

		public ChangeClanSigilMessage(string newSigil)
		{
			this.NewSigil = newSigil;
		}
	}
}
