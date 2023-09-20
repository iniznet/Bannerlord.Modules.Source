using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class RejoinBattleRequestAnswerMessage : Message
	{
		public bool IsRejoinAccepted { get; set; }

		public bool IsSuccessful { get; set; }

		public RejoinBattleRequestAnswerMessage()
		{
		}

		public RejoinBattleRequestAnswerMessage(bool isRejoinAccepted, bool isSuccessful)
		{
			this.IsRejoinAccepted = isRejoinAccepted;
			this.IsSuccessful = isSuccessful;
		}
	}
}
