using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RemoveClanOfficerRoleForPlayerMessage : Message
	{
		public PlayerId RemovedOfficerId { get; private set; }

		public RemoveClanOfficerRoleForPlayerMessage(PlayerId removedOfficerId)
		{
			this.RemovedOfficerId = removedOfficerId;
		}
	}
}
