using System;

namespace TaleWorlds.MountAndBlade
{
	public class BaseNetworkComponentData : UdpNetworkComponent
	{
		public int CurrentBattleIndex { get; private set; }

		public void UpdateCurrentBattleIndex(int currentBattleIndex)
		{
			this.CurrentBattleIndex = currentBattleIndex;
		}

		public const float MaxIntermissionStateTime = 240f;
	}
}
