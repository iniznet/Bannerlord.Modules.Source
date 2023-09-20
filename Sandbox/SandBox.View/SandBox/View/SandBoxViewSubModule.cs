﻿using System;
using System.Collections.Generic;
using SandBox.View.Conversation;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.ScreenSystem;

namespace SandBox.View
{
	public class SandBoxViewSubModule : MBSubModuleBase
	{
		public static ConversationViewManager ConversationViewManager
		{
			get
			{
				return SandBoxViewSubModule._instance._conversationViewManager;
			}
		}

		public static IMapConversationDataProvider MapConversationDataProvider
		{
			get
			{
				return SandBoxViewSubModule._instance._mapConversationDataProvider;
			}
		}

		internal static Dictionary<UIntPtr, IPartyVisual> VisualsOfEntities
		{
			get
			{
				return SandBoxViewSubModule._instance._visualsOfEntities;
			}
		}

		internal static Dictionary<UIntPtr, Tuple<MatrixFrame, PartyVisual>> FrameAndVisualOfEngines
		{
			get
			{
				return SandBoxViewSubModule._instance._frameAndVisualOfEngines;
			}
		}

		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			SandBoxViewSubModule._instance = this;
			SandBoxSaveHelper.OnStateChange += SandBoxViewSubModule.OnSaveHelperStateChange;
			PropertyBasedTooltipVMExtensions.FillCampaignTooltipTypes();
			TextObject coreContentDisabledReason = new TextObject("{=V8BXjyYq}Disabled during installation.", null);
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("CampaignResumeGame", new TextObject("{=6mN03uTP}Saved Games", null), 0, delegate
			{
				ScreenManager.PushScreen(SandBoxViewCreator.CreateSaveLoadScreen(false));
			}, () => new ValueTuple<bool, TextObject>(Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason)));
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("ContinueCampaign", new TextObject("{=0tJ1oarX}Continue Campaign", null), 1, new Action(this.ContinueCampaign), () => this.IsContinueCampaignDisabled()));
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("SandBoxNewGame", new TextObject("{=171fTtIN}SandBox", null), 3, delegate
			{
				MBGameManager.StartNewGame(new SandBoxGameManager());
			}, () => new ValueTuple<bool, TextObject>(Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason)));
			Module.CurrentModule.ImguiProfilerTick += this.OnImguiProfilerTick;
			this._mapConversationDataProvider = new DefaultMapConversationDataProvider();
		}

		protected override void OnSubModuleUnloaded()
		{
			Module.CurrentModule.ImguiProfilerTick -= this.OnImguiProfilerTick;
			SandBoxSaveHelper.OnStateChange -= SandBoxViewSubModule.OnSaveHelperStateChange;
			SandBoxViewSubModule._instance = null;
			base.OnSubModuleUnloaded();
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
			if (!this._isInitialized)
			{
				CampaignOptionsManager.Initialize();
				this._isInitialized = true;
			}
		}

		public override void OnCampaignStart(Game game, object starterObject)
		{
			base.OnCampaignStart(game, starterObject);
			if (Campaign.Current != null)
			{
				this._conversationViewManager = new ConversationViewManager();
			}
		}

		public override void OnGameLoaded(Game game, object initializerObject)
		{
			this._conversationViewManager = new ConversationViewManager();
		}

		public override void BeginGameStart(Game game)
		{
			base.BeginGameStart(game);
			if (Campaign.Current != null)
			{
				this._visualsOfEntities = new Dictionary<UIntPtr, IPartyVisual>();
				this._frameAndVisualOfEngines = new Dictionary<UIntPtr, Tuple<MatrixFrame, PartyVisual>>();
				Campaign.Current.VisualCreator.PartyVisualCreator = new PartyVisualCreator();
				Campaign.Current.SaveHandler.MainHeroVisualSupplier = new MainHeroSaveVisualSupplier();
				TableauCacheManager.InitializeSandboxValues();
			}
		}

		public override void OnGameEnd(Game game)
		{
			if (this._visualsOfEntities != null)
			{
				foreach (IPartyVisual partyVisual in this._visualsOfEntities.Values)
				{
					partyVisual.ReleaseResources();
				}
			}
			this._visualsOfEntities = null;
			this._frameAndVisualOfEngines = null;
			this._conversationViewManager = null;
			if (Campaign.Current != null)
			{
				Campaign.Current.SaveHandler.MainHeroVisualSupplier = null;
				TableauCacheManager.ReleaseSandboxValues();
			}
		}

		private ValueTuple<bool, TextObject> IsContinueCampaignDisabled()
		{
			if (Module.CurrentModule.IsOnlyCoreContentEnabled)
			{
				return new ValueTuple<bool, TextObject>(true, new TextObject("{=V8BXjyYq}Disabled during installation.", null));
			}
			if (string.IsNullOrEmpty(BannerlordConfig.LatestSaveGameName))
			{
				return new ValueTuple<bool, TextObject>(true, new TextObject("{=aWMZQKXZ}Save the game at least once to continue", null));
			}
			SaveGameFileInfo saveFileWithName = MBSaveLoad.GetSaveFileWithName(BannerlordConfig.LatestSaveGameName);
			if (saveFileWithName == null)
			{
				return new ValueTuple<bool, TextObject>(true, new TextObject("{=60LTq0tQ}Can't find the save file for the latest save game.", null));
			}
			if (saveFileWithName.IsCorrupted)
			{
				return new ValueTuple<bool, TextObject>(true, new TextObject("{=t6W3UjG0}Save game file appear to be corrupted. Try starting a new campaign or load another one from Saved Games menu.", null));
			}
			return new ValueTuple<bool, TextObject>(false, TextObject.Empty);
		}

		private void ContinueCampaign()
		{
			SaveGameFileInfo saveFileWithName = MBSaveLoad.GetSaveFileWithName(BannerlordConfig.LatestSaveGameName);
			if (saveFileWithName != null && !saveFileWithName.IsCorrupted)
			{
				SandBoxSaveHelper.TryLoadSave(saveFileWithName, new Action<LoadResult>(this.StartGame), null);
				return;
			}
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=oZrVNUOk}Error", null).ToString(), new TextObject("{=t6W3UjG0}Save game file appear to be corrupted. Try starting a new campaign or load another one from Saved Games menu.", null).ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
		}

		private void StartGame(LoadResult loadResult)
		{
			MBGameManager.StartNewGame(new SandBoxGameManager(loadResult));
		}

		private void OnImguiProfilerTick()
		{
			if (Campaign.Current == null)
			{
				return;
			}
			List<MobileParty> all = MobileParty.All;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (MobileParty mobileParty in all)
			{
				if (mobileParty.IsVisible)
				{
					num++;
				}
				if (((PartyVisual)mobileParty.Party.Visuals).HumanAgentVisuals != null)
				{
					num2++;
				}
				if (((PartyVisual)mobileParty.Party.Visuals).MountAgentVisuals != null)
				{
					num2++;
				}
				if (((PartyVisual)mobileParty.Party.Visuals).CaravanMountAgentVisuals != null)
				{
					num2++;
				}
				num3++;
			}
			Imgui.BeginMainThreadScope();
			Imgui.Begin("Bannerlord Campaign Statistics");
			Imgui.Columns(2, "", true);
			Imgui.Text("Name");
			Imgui.NextColumn();
			Imgui.Text("Count");
			Imgui.NextColumn();
			Imgui.Separator();
			Imgui.Text("Total Mobile Party");
			Imgui.NextColumn();
			Imgui.Text(num3.ToString());
			Imgui.NextColumn();
			Imgui.Text("Visible Mobile Party");
			Imgui.NextColumn();
			Imgui.Text(num.ToString());
			Imgui.NextColumn();
			Imgui.Text("Total Agent Visuals");
			Imgui.NextColumn();
			Imgui.Text(num2.ToString());
			Imgui.NextColumn();
			Imgui.End();
			Imgui.EndMainThreadScope();
		}

		public static void SetMapConversationDataProvider(IMapConversationDataProvider mapConversationDataProvider)
		{
			SandBoxViewSubModule._instance._mapConversationDataProvider = mapConversationDataProvider;
		}

		private static void OnSaveHelperStateChange(SandBoxSaveHelper.SaveHelperState currentState)
		{
			switch (currentState)
			{
			case SandBoxSaveHelper.SaveHelperState.Start:
			case SandBoxSaveHelper.SaveHelperState.LoadGame:
				LoadingWindow.EnableGlobalLoadingWindow();
				return;
			case SandBoxSaveHelper.SaveHelperState.Inquiry:
				LoadingWindow.DisableGlobalLoadingWindow();
				return;
			default:
				Debug.FailedAssert("Undefined save state for listener!", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\SandBoxViewSubModule.cs", "OnSaveHelperStateChange", 294);
				return;
			}
		}

		private bool _isInitialized;

		private ConversationViewManager _conversationViewManager;

		private IMapConversationDataProvider _mapConversationDataProvider;

		private Dictionary<UIntPtr, IPartyVisual> _visualsOfEntities;

		private Dictionary<UIntPtr, Tuple<MatrixFrame, PartyVisual>> _frameAndVisualOfEngines;

		private static SandBoxViewSubModule _instance;
	}
}
