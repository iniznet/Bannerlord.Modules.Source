using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateUsingClanSigil : Message
	{
		[JsonProperty]
		public bool IsUsed { get; private set; }

		public UpdateUsingClanSigil()
		{
		}

		public UpdateUsingClanSigil(bool isUsed)
		{
			this.IsUsed = isUsed;
		}
	}
}
