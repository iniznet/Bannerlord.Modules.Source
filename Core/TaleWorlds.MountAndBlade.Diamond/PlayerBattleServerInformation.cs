using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PlayerBattleServerInformation
	{
		public int PeerIndex { get; private set; }

		public int SessionKey { get; private set; }

		public PlayerBattleServerInformation(int peerIndex, int sessionKey)
		{
			this.PeerIndex = peerIndex;
			this.SessionKey = sessionKey;
		}
	}
}
