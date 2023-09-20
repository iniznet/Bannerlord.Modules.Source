using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class JoinGameData
	{
		public GameServerProperties GameServerProperties { get; }

		public int PeerIndex { get; }

		public int SessionKey { get; }

		public JoinGameData(GameServerProperties gameServerProperties, int peerIndex, int sessionKey)
		{
			this.GameServerProperties = gameServerProperties;
			this.PeerIndex = peerIndex;
			this.SessionKey = sessionKey;
		}
	}
}
