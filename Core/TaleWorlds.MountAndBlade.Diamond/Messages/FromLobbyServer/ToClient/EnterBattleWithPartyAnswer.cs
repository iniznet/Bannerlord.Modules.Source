using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class EnterBattleWithPartyAnswer : Message
	{
		[JsonProperty]
		public bool Successful { get; private set; }

		[JsonProperty]
		public string[] SelectedAndEnabledGameTypes { get; private set; }

		public EnterBattleWithPartyAnswer()
		{
		}

		public EnterBattleWithPartyAnswer(bool successful, string[] selectedAndEnabledGameTypes)
		{
			this.Successful = successful;
			this.SelectedAndEnabledGameTypes = selectedAndEnabledGameTypes;
		}
	}
}
