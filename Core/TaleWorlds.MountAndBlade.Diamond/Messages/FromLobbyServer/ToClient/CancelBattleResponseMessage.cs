using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CancelBattleResponseMessage : Message
	{
		public bool Successful { get; private set; }

		public CancelBattleResponseMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
