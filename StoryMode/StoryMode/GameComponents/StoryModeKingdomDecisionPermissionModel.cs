using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.GameComponents
{
	// Token: 0x02000042 RID: 66
	public class StoryModeKingdomDecisionPermissionModel : DefaultKingdomDecisionPermissionModel
	{
		// Token: 0x060003BC RID: 956 RVA: 0x00017390 File Offset: 0x00015590
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

		// Token: 0x060003BD RID: 957 RVA: 0x000173F0 File Offset: 0x000155F0
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
