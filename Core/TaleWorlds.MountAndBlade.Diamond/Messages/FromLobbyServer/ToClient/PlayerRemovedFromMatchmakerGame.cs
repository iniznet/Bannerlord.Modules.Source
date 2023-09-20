using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class PlayerRemovedFromMatchmakerGame : Message
	{
		public DisconnectType DisconnectType { get; private set; }

		public PlayerRemovedFromMatchmakerGame(DisconnectType disconnectType)
		{
			this.DisconnectType = disconnectType;
		}
	}
}
