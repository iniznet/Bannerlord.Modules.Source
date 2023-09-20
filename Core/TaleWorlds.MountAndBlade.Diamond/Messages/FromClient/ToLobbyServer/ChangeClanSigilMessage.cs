using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeClanSigilMessage : Message
	{
		public string NewSigil { get; private set; }

		public ChangeClanSigilMessage(string newSigil)
		{
			this.NewSigil = newSigil;
		}
	}
}
