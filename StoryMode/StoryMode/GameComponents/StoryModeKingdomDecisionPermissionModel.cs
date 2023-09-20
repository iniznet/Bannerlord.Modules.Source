using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.GameComponents
{
	public class StoryModeKingdomDecisionPermissionModel : DefaultKingdomDecisionPermissionModel
	{
		public override bool IsWarDecisionAllowedBetweenKingdoms(Kingdom kingdom1, Kingdom kingdom2, out TextObject reason)
		{
			if (StoryModeManager.Current.MainStoryLine.ThirdPhase != null)
			{
				MBReadOnlyList<Kingdom> oppositionKingdoms = StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms;
				if (oppositionKingdoms.IndexOf(kingdom1) >= 0 && oppositionKingdoms.IndexOf(kingdom2) >= 0)
				{
					reason = GameTexts.FindText("str_kingdom_diplomacy_war_truce_disabled_reason_story", null);
					return false;
				}
			}
			reason = TextObject.Empty;
			return true;
		}

		public override bool IsPeaceDecisionAllowedBetweenKingdoms(Kingdom kingdom1, Kingdom kingdom2, out TextObject reason)
		{
			if (StoryModeManager.Current.MainStoryLine.ThirdPhase != null)
			{
				MBReadOnlyList<Kingdom> oppositionKingdoms = StoryModeManager.Current.MainStoryLine.ThirdPhase.OppositionKingdoms;
				MBReadOnlyList<Kingdom> allyKingdoms = StoryModeManager.Current.MainStoryLine.ThirdPhase.AllyKingdoms;
				if ((oppositionKingdoms.IndexOf(kingdom1) >= 0 && allyKingdoms.IndexOf(kingdom2) >= 0) || (oppositionKingdoms.IndexOf(kingdom2) >= 0 && allyKingdoms.IndexOf(kingdom1) >= 0))
				{
					reason = GameTexts.FindText("str_kingdom_diplomacy_war_truce_disabled_reason_story", null);
					return false;
				}
			}
			reason = TextObject.Empty;
			return true;
		}
	}
}
