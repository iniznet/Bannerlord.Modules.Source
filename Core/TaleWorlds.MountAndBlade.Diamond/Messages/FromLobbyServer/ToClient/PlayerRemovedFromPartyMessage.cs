using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PlayerRemovedFromPartyMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public PartyRemoveReason Reason { get; private set; }

		public PlayerRemovedFromPartyMessage(PlayerId playerId, PartyRemoveReason reason)
		{
			this.PlayerId = playerId;
			this.Reason = reason;
		}
	}
}
