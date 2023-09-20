using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace StoryMode.GameComponents
{
	public class StoryModeCutsceneSelectionModel : DefaultCutsceneSelectionModel
	{
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
