using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem.Load;

namespace SandBox
{
	public class SandBoxGameManager : MBGameManager
	{
		public SandBoxGameManager()
		{
			this._loadingSavedGame = false;
		}

		public SandBoxGameManager(LoadResult loadedGameResult)
		{
			this._loadingSavedGame = true;
			this._loadedGameResult = loadedGameResult;
		}

		public override void OnGameEnd(Game game)
		{
			MBDebug.SetErrorReportScene(null);
			base.OnGameEnd(game);
		}

		protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
		{
			nextStep = -1;
			switch (gameManagerLoadingStep)
			{
			case 0:
				nextStep = 1;
				return;
			case 1:
				MBGameManager.LoadModuleData(this._loadingSavedGame);
				nextStep = 2;
				return;
			case 2:
				if (!this._loadingSavedGame)
				{
					MBGameManager.StartNewGame();
				}
				nextStep = 3;
				return;
			case 3:
				MBGlobals.InitializeReferences();
				if (!this._loadingSavedGame)
				{
					MBDebug.Print("Initializing new game begin...", 0, 12, 17592186044416UL);
					Campaign campaign = new Campaign(1);
					Game.CreateGame(campaign, this);
					campaign.SetLoadingParameters(1);
					MBDebug.Print("Initializing new game end...", 0, 12, 17592186044416UL);
				}
				else
				{
					MBDebug.Print("Initializing saved game begin...", 0, 12, 17592186044416UL);
					((Campaign)Game.LoadSaveGame(this._loadedGameResult, this).GameType).SetLoadingParameters(2);
					this._loadedGameResult = null;
					Common.MemoryCleanupGC(false);
					MBDebug.Print("Initializing saved game end...", 0, 12, 17592186044416UL);
				}
				Game.Current.DoLoading();
				nextStep = 4;
				return;
			case 4:
			{
				bool flag = true;
				foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
				{
					flag = flag && mbsubModuleBase.DoLoading(Game.Current);
				}
				nextStep = (flag ? 5 : 4);
				return;
			}
			case 5:
				nextStep = (Game.Current.DoLoading() ? (-1) : 5);
				return;
			default:
				return;
			}
		}

		public override void OnLoadFinished()
		{
			if (!this._loadingSavedGame)
			{
				MBDebug.Print("Switching to menu window...", 0, 12, 17592186044416UL);
				if (!Game.Current.IsDevelopmentMode)
				{
					VideoPlaybackState videoPlaybackState = Game.Current.GameStateManager.CreateState<VideoPlaybackState>();
					string text = ModuleHelper.GetModuleFullPath("SandBox") + "Videos/CampaignIntro/";
					string text2 = text + "campaign_intro";
					string text3 = text + "campaign_intro.ivf";
					string text4 = text + "campaign_intro.ogg";
					videoPlaybackState.SetStartingParameters(text3, text4, text2, 30f, true);
					videoPlaybackState.SetOnVideoFinisedDelegate(new Action(this.LaunchSandboxCharacterCreation));
					Game.Current.GameStateManager.CleanAndPushState(videoPlaybackState, 0);
				}
				else
				{
					this.LaunchSandboxCharacterCreation();
				}
			}
			else
			{
				if (CampaignSiegeTestStatic.IsSiegeTestBuild)
				{
					CampaignSiegeTestStatic.DisableSiegeTest();
				}
				Game.Current.GameStateManager.OnSavedGameLoadFinished();
				Game.Current.GameStateManager.CleanAndPushState(Game.Current.GameStateManager.CreateState<MapState>(), 0);
				MapState mapState = Game.Current.GameStateManager.ActiveState as MapState;
				string text5 = ((mapState != null) ? mapState.GameMenuId : null);
				if (!string.IsNullOrEmpty(text5))
				{
					PlayerEncounter playerEncounter = PlayerEncounter.Current;
					if (playerEncounter != null)
					{
						playerEncounter.OnLoad();
					}
					Campaign.Current.GameMenuManager.SetNextMenu(text5);
				}
				IPartyVisual visuals = PartyBase.MainParty.Visuals;
				if (visuals != null)
				{
					visuals.SetMapIconAsDirty();
				}
				Campaign.Current.CampaignInformationManager.OnGameLoaded();
				foreach (Settlement settlement in Settlement.All)
				{
					settlement.Party.Visuals.RefreshLevelMask(settlement.Party);
				}
				CampaignEventDispatcher.Instance.OnGameLoadFinished();
				if (mapState != null)
				{
					mapState.OnLoadingFinished();
				}
			}
			base.IsLoaded = true;
		}

		private void LaunchSandboxCharacterCreation()
		{
			CharacterCreationState characterCreationState = Game.Current.GameStateManager.CreateState<CharacterCreationState>(new object[]
			{
				new SandboxCharacterCreationContent()
			});
			Game.Current.GameStateManager.CleanAndPushState(characterCreationState, 0);
		}

		[CrashInformationCollector.CrashInformationProvider]
		private static CrashInformationCollector.CrashInformation UsedModuleInfoCrashCallback()
		{
			Campaign campaign = Campaign.Current;
			if (((campaign != null) ? campaign.PreviouslyUsedModules : null) != null)
			{
				string[] moduleNames = SandBoxManager.Instance.ModuleManager.ModuleNames;
				MBList<ValueTuple<string, string>> mblist = new MBList<ValueTuple<string, string>>();
				using (List<string>.Enumerator enumerator = Campaign.Current.PreviouslyUsedModules.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string module = enumerator.Current;
						bool flag = Extensions.FindIndex<string>(moduleNames, (string x) => x == module) != -1;
						mblist.Add(new ValueTuple<string, string>(module, flag ? "1" : "0"));
					}
				}
				return new CrashInformationCollector.CrashInformation("Used Mods", mblist);
			}
			return null;
		}

		[CrashInformationCollector.CrashInformationProvider]
		private static CrashInformationCollector.CrashInformation UsedGameVersionsCallback()
		{
			Campaign campaign = Campaign.Current;
			if (((campaign != null) ? campaign.UsedGameVersions : null) != null)
			{
				MBList<ValueTuple<string, string>> mblist = new MBList<ValueTuple<string, string>>();
				for (int i = 0; i < Campaign.Current.UsedGameVersions.Count; i++)
				{
					string text = "";
					if (i < Campaign.Current.UsedGameVersions.Count - 1 && ApplicationVersion.FromString(Campaign.Current.UsedGameVersions[i], 21456) > ApplicationVersion.FromString(Campaign.Current.UsedGameVersions[i + 1], 21456))
					{
						text = "Error";
					}
					mblist.Add(new ValueTuple<string, string>(Campaign.Current.UsedGameVersions[i], text));
				}
				return new CrashInformationCollector.CrashInformation("Used Game Versions", mblist);
			}
			return null;
		}

		private bool _loadingSavedGame;

		private LoadResult _loadedGameResult;
	}
}
