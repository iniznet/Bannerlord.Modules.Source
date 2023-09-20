using System;
using SandBox.GauntletUI.Map;
using SandBox.View.Map;
using StoryMode.GameComponents.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View;

namespace StoryMode.GauntletUI.Map
{
	[OverrideView(typeof(MapCheatsView))]
	internal class GauntletStoryModeMapCheatsView : GauntletMapCheatsView
	{
		protected override void CreateLayout()
		{
			base.CreateLayout();
			AchievementsCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<AchievementsCampaignBehavior>();
			TextObject textObject;
			if (campaignBehavior == null || !campaignBehavior.CheckAchievementSystemActivity(out textObject))
			{
				this.EnableCheatMenu();
				return;
			}
			this._layerAsGauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=4Ygn4OGE}Enable Cheats", null).ToString(), new TextObject("{=YkbOfPRU}Enabling cheats will disable the achievements this game. Do you want to proceed?", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.EnableCheatMenu), new Action(this.RemoveCheatMenu), "", 0f, null, null, null), false, false);
		}

		private void EnableCheatMenu()
		{
			this._layerAsGauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			AchievementsCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<AchievementsCampaignBehavior>();
			TextObject textObject;
			if (campaignBehavior != null && campaignBehavior.CheckAchievementSystemActivity(out textObject) && campaignBehavior != null)
			{
				campaignBehavior.DeactivateAchievements(new TextObject("{=sO8Zh3ZH}Achievements are disabled due to cheat usage.", null), true, false);
			}
		}

		private void RemoveCheatMenu()
		{
			base.MapScreen.CloseGameplayCheats();
		}
	}
}
