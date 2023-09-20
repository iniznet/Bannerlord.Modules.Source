using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PlayerSuggestedToPartyMessage : Message
	{
		public PlayerId PlayerId { get; }

		public string PlayerName { get; }

		public PlayerId SuggestingPlayerId { get; }

		public string SuggestingPlayerName { get; }

		public PlayerSuggestedToPartyMessage(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName)
		{
			this.PlayerId = playerId;
			this.PlayerName = playerName;
			this.SuggestingPlayerId = suggestingPlayerId;
			this.SuggestingPlayerName = suggestingPlayerName;
		}
	}
}
