using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class ClanCreationPlayerData
	{
		public PlayerSessionId PlayerSessionId { get; private set; }

		public ClanCreationAnswer ClanCreationAnswer { get; private set; }

		public ClanCreationPlayerData(PlayerSessionId playerSessionId, ClanCreationAnswer answer)
		{
			this.PlayerSessionId = playerSessionId;
			this.ClanCreationAnswer = answer;
		}
	}
}
