using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangePlayerSigilMessage : Message
	{
		public string SigilId { get; private set; }

		public ChangePlayerSigilMessage(string sigilId)
		{
			this.SigilId = sigilId;
		}
	}
}
