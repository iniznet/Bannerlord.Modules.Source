using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace StoryMode.GameComponents
{
	// Token: 0x0200003E RID: 62
	public class StoryModeCutsceneSelectionModel : DefaultCutsceneSelectionModel
	{
		// Token: 0x060003B4 RID: 948 RVA: 0x00017270 File Offset: 0x00015470
		public override SceneNotificationData GetKingdomDestroyedSceneNotification(Kingdom kingdom)
		{
			SceneNotificationData sceneNotificationData;
			if (StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom == kingdom)
			{
				sceneNotificationData = new SupportedFactionDefeatedSceneNotificationItem(kingdom, StoryModeManager.Current.MainStoryLine.IsOnImperialQuestLine);
			}
			else
			{
				sceneNotificationData = new KingdomDestroyedSceneNotificationItem(kingdom);
			}
			return sceneNotificationData;
		}
	}
}
