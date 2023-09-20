using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class CosmeticItemInfo
	{
		public string TroopId { get; private set; }

		public string CosmeticIndex { get; private set; }

		public bool IsEquipped { get; private set; }

		public CosmeticItemInfo(string troopId, string cosmeticIndex, bool isEquipped)
		{
			this.TroopId = troopId;
			this.CosmeticIndex = cosmeticIndex;
			this.IsEquipped = isEquipped;
		}
	}
}
