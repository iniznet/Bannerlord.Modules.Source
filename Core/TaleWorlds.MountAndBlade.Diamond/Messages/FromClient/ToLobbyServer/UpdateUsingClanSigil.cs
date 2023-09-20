using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateUsingClanSigil : Message
	{
		public bool IsUsed { get; private set; }

		public UpdateUsingClanSigil(bool isUsed)
		{
			this.IsUsed = isUsed;
		}
	}
}
