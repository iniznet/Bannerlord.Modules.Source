﻿using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class JoinGameData
	{
		public GameServerProperties GameServerProperties { get; set; }

		public int PeerIndex { get; set; }

		public int SessionKey { get; set; }

		public JoinGameData()
		{
		}

		public JoinGameData(GameServerProperties gameServerProperties, int peerIndex, int sessionKey)
		{
			this.GameServerProperties = gameServerProperties;
			this.PeerIndex = peerIndex;
			this.SessionKey = sessionKey;
		}
	}
}
