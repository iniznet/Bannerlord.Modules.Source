using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001C7 RID: 455
	public abstract class BattleCaptainModel : GameModel
	{
		// Token: 0x06001B67 RID: 7015
		public abstract float GetCaptainRatingForTroopClasses(Hero hero, TroopClassFlag flag, out List<PerkObject> compatiblePerks);
	}
}
