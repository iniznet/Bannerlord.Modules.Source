using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class EnterBattleWithPartyAnswer : Message
	{
		public bool Successful { get; private set; }

		public string[] SelectedAndEnabledGameTypes { get; private set; }

		public EnterBattleWithPartyAnswer(bool successful, string[] selectedAndEnabledGameTypes)
		{
			this.Successful = successful;
			this.SelectedAndEnabledGameTypes = selectedAndEnabledGameTypes;
		}
	}
}
