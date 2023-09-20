using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class PlayerRemovedFromCustomGame : Message
	{
		public DisconnectType DisconnectType { get; private set; }

		public PlayerRemovedFromCustomGame(DisconnectType disconnectType)
		{
			this.DisconnectType = disconnectType;
		}
	}
}
