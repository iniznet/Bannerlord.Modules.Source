using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class BattleCaptainModel : GameModel
	{
		public abstract float GetCaptainRatingForTroopClasses(Hero hero, TroopClassFlag flag, out List<PerkObject> compatiblePerks);
	}
}
