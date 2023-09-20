using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class EnterCustomBattleWithPartyAnswer : Message
	{
		[JsonProperty]
		public bool Successful { get; private set; }

		public EnterCustomBattleWithPartyAnswer()
		{
		}

		public EnterCustomBattleWithPartyAnswer(bool successful)
		{
			this.Successful = successful;
		}
	}
}
