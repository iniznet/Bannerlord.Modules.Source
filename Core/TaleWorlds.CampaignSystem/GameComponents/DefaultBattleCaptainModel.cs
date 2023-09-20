using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultBattleCaptainModel : BattleCaptainModel
	{
		public override float GetCaptainRatingForTroopClasses(Hero hero, TroopClassFlag flag, out List<PerkObject> compatiblePerks)
		{
			float num = 0f;
			compatiblePerks = new List<PerkObject>();
			foreach (PerkObject perkObject in PerkHelper.GetCaptainPerksForTroopClasses(flag))
			{
				if (hero.GetPerkValue(perkObject))
				{
					num += perkObject.RequiredSkillValue;
					compatiblePerks.Add(perkObject);
				}
			}
			num /= 1650f;
			return num;
		}
	}
}
