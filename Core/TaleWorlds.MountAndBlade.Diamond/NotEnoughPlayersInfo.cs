using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class NotEnoughPlayersInfo
	{
		public int CurrentPlayerCount { get; private set; }

		public int RequiredPlayerCount { get; private set; }

		public NotEnoughPlayersInfo(int currentPlayerCount, int requiredPlayerCount)
		{
			this.CurrentPlayerCount = currentPlayerCount;
			this.RequiredPlayerCount = requiredPlayerCount;
		}
	}
}
