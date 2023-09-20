using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class AnotherPlayerData
	{
		public AnotherPlayerState PlayerState { get; private set; }

		public int Experience { get; private set; }

		public AnotherPlayerData(AnotherPlayerState anotherPlayerState, int anotherPlayerExperience)
		{
			this.PlayerState = anotherPlayerState;
			this.Experience = anotherPlayerExperience;
		}
	}
}
