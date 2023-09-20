using System;

namespace TaleWorlds.CampaignSystem.Party
{
	public struct TroopTradeDifference
	{
		public CharacterObject Troop { get; set; }

		public bool IsPrisoner { get; set; }

		public int FromCount { get; set; }

		public int ToCount { get; set; }

		public int DifferenceCount
		{
			get
			{
				return this.FromCount - this.ToCount;
			}
		}

		public bool IsEmpty { get; private set; }

		public static TroopTradeDifference Empty
		{
			get
			{
				return new TroopTradeDifference
				{
					IsEmpty = true
				};
			}
		}
	}
}
