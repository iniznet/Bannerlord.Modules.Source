using System;

namespace TaleWorlds.Core
{
	public static class BattleSideEnumExtensions
	{
		public static bool IsValid(this BattleSideEnum battleSide)
		{
			return battleSide >= BattleSideEnum.Defender && battleSide < BattleSideEnum.NumSides;
		}
	}
}
