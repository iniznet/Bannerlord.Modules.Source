using System;
using SandBox;
using StoryMode.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;

namespace StoryMode
{
	// Token: 0x02000013 RID: 19
	public class StoryModeGameManager : SandBoxGameManager
	{
		// Token: 0x0600008E RID: 142 RVA: 0x00004B30 File Offset: 0x00002D30
		protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
		{
			if (gameManagerLoadingStep != 3)
			{
				base.DoLoadingForGameManager(gameManagerLoadingStep, out nextStep);
				return;
			}
			MBGlobals.InitializeReferences();
			MBDebug.Print("Initializing new game begin...", 0, 12, 17592186044416UL);
			CampaignStoryMode campaignStoryMode = new CampaignStoryMode(1);
			Game.CreateGame(campaignStoryMode, this);
			campaignStoryMode.SetLoadingParameters(1);
			MBDebug.Print("Initializing new game end...", 0, 12, 17592186044416UL);
			Game.Current.DoLoading();
			nextStep = 4;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00004BA0 File Offset: 0x00002DA0
		public override void OnLoadFinished()
		{
			VideoPlaybackState videoPlaybackState = Game.Current.GameStateManager.CreateState<VideoPlaybackState>();
			string text = ModuleHelper.GetModuleFullPath("SandBox") + "Videos/CampaignIntro/";
			string text2 = text + "campaign_intro";
			string text3 = text + "campaign_intro.ivf";
			string text4 = text + "campaign_intro.ogg";
			videoPlaybackState.SetStartingParameters(text3, text4, text2, 30f, true);
			videoPlaybackState.SetOnVideoFinisedDelegate(new Action(this.LaunchStoryModeCharacterCreation));
			Game.Current.GameStateManager.CleanAndPushState(videoPlaybackState, 0);
			base.IsLoaded = true;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00004C30 File Offset: 0x00002E30
		private void LaunchStoryModeCharacterCreation()
		{
			CharacterCreationState characterCreationState = Game.Current.GameStateManager.CreateState<CharacterCreationState>(new object[]
			{
				new StoryModeCharacterCreationContent()
			});
			Game.Current.GameStateManager.CleanAndPushState(characterCreationState, 0);
		}
	}
}
