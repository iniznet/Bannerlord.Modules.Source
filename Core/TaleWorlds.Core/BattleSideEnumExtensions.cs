using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200001D RID: 29
	public static class BattleSideEnumExtensions
	{
		// Token: 0x0600016F RID: 367 RVA: 0x00006308 File Offset: 0x00004508
		public static bool IsValid(this BattleSideEnum battleSide)
		{
			return battleSide >= BattleSideEnum.Defender && battleSide < BattleSideEnum.NumSides;
		}
	}
}
