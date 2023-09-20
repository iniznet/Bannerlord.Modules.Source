using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class AssignAsClanOfficerMessage : Message
	{
		[JsonProperty]
		public PlayerId AssignedPlayerId { get; private set; }

		[JsonProperty]
		public bool DontUseNameForUnknownPlayer { get; private set; }

		public AssignAsClanOfficerMessage()
		{
		}

		public AssignAsClanOfficerMessage(PlayerId assignedPlayerId, bool dontUseNameForUnknownPlayer)
		{
			this.AssignedPlayerId = assignedPlayerId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
		}
	}
}
