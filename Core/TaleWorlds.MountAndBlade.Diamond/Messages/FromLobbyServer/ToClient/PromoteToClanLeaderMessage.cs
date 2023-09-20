using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PromoteToClanLeaderMessage : Message
	{
		public PlayerId PromotedPlayerId { get; private set; }

		public bool DontUseNameForUnknownPlayer { get; private set; }

		public PromoteToClanLeaderMessage(PlayerId promotedPlayerId, bool dontUseNameForUnknownPlayer)
		{
			this.PromotedPlayerId = promotedPlayerId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
		}
	}
}
