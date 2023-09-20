using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class EnterCustomBattleWithPartyAnswer : Message
	{
		public bool Successful { get; private set; }

		public EnterCustomBattleWithPartyAnswer(bool successful)
		{
			this.Successful = successful;
		}
	}
}
