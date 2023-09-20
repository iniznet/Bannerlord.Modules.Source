using System;
using SandBox.Conversation.MissionLogics;
using SandBox.View.Missions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.ViewModelCollection.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Mission;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Missions
{
	[OverrideView(typeof(MissionConversationView))]
	public class MissionGauntletConversationView : MissionView, IConversationStateHandler
	{
		public MissionConversationLogic ConversationHandler { get; private set; }

		public MissionGauntletConversationView()
		{
			this.ViewOrderPriority = 49;
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			MissionGauntletEscapeMenuBase escapeView = this._escapeView;
			if ((escapeView == null || !escapeView.IsActive) && this._gauntletLayer != null)
			{
				MissionConversationVM dataSource = this._dataSource;
				if (dataSource != null && dataSource.AnswerList.Count <= 0 && base.Mission.Mode != 5)
				{
					if (!this.IsReleasedInSceneLayer("ContinueClick", false))
					{
						if (!this.IsReleasedInGauntletLayer("ContinueKey", true))
						{
							goto IL_9D;
						}
						MissionConversationVM dataSource2 = this._dataSource;
						if (dataSource2 == null || dataSource2.SelectedAnOptionOrLinkThisFrame)
						{
							goto IL_9D;
						}
					}
					MissionConversationVM dataSource3 = this._dataSource;
					if (dataSource3 != null)
					{
						dataSource3.ExecuteContinue();
					}
				}
				IL_9D:
				if (this._gauntletLayer != null && this.IsGameKeyReleasedInAnyLayer("ToggleEscapeMenu", true))
				{
					base.MissionScreen.OnEscape();
				}
				if (this._dataSource != null)
				{
					this._dataSource.SelectedAnOptionOrLinkThisFrame = false;
				}
				if (base.MissionScreen.SceneLayer.Input.IsKeyDown(225))
				{
					GauntletLayer gauntletLayer = this._gauntletLayer;
					if (gauntletLayer == null)
					{
						return;
					}
					gauntletLayer.InputRestrictions.SetMouseVisibility(false);
					return;
				}
				else
				{
					GauntletLayer gauntletLayer2 = this._gauntletLayer;
					if (gauntletLayer2 == null)
					{
						return;
					}
					gauntletLayer2.InputRestrictions.SetInputRestrictions(true, 7);
				}
			}
		}

		public override void OnMissionScreenFinalize()
		{
			Campaign.Current.ConversationManager.Handler = null;
			if (this._dataSource != null)
			{
				MissionConversationVM dataSource = this._dataSource;
				if (dataSource != null)
				{
					dataSource.OnFinalize();
				}
				this._dataSource = null;
			}
			this._gauntletLayer = null;
			this.ConversationHandler = null;
			base.OnMissionScreenFinalize();
		}

		public override void EarlyStart()
		{
			base.EarlyStart();
			this.ConversationHandler = base.Mission.GetMissionBehavior<MissionConversationLogic>();
			this._conversationCameraView = base.Mission.GetMissionBehavior<MissionConversationCameraView>();
			Campaign.Current.ConversationManager.Handler = this;
		}

		public override void OnMissionScreenActivate()
		{
			base.OnMissionScreenActivate();
			if (this._dataSource != null)
			{
				base.MissionScreen.SetLayerCategoriesStateAndDeactivateOthers(new string[] { "Conversation", "SceneLayer" }, true);
				ScreenManager.TrySetFocus(this._gauntletLayer);
			}
		}

		void IConversationStateHandler.OnConversationInstall()
		{
			base.MissionScreen.SetConversationActive(true);
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._conversationCategory = spriteData.SpriteCategories["ui_conversation"];
			this._conversationCategory.Load(resourceContext, uiresourceDepot);
			this._dataSource = new MissionConversationVM(new Func<string>(this.GetContinueKeyText), false);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "Conversation", false);
			this._gauntletLayer.LoadMovie("SPConversation", this._dataSource);
			GameKeyContext category = HotKeyManager.GetCategory("ConversationHotKeyCategory");
			this._gauntletLayer.Input.RegisterHotKeyCategory(category);
			if (!base.MissionScreen.SceneLayer.Input.IsCategoryRegistered(category))
			{
				base.MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(category);
			}
			this._gauntletLayer.IsFocusLayer = true;
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._escapeView = base.Mission.GetMissionBehavior<MissionGauntletEscapeMenuBase>();
			base.MissionScreen.AddLayer(this._gauntletLayer);
			base.MissionScreen.SetLayerCategoriesStateAndDeactivateOthers(new string[] { "Conversation", "SceneLayer" }, true);
			ScreenManager.TrySetFocus(this._gauntletLayer);
			this._conversationManager = Campaign.Current.ConversationManager;
			InformationManager.ClearAllMessages();
		}

		public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
		{
			base.OnMissionModeChange(oldMissionMode, atStart);
			if (oldMissionMode == 5 && base.Mission.Mode == 1)
			{
				ScreenManager.TrySetFocus(this._gauntletLayer);
			}
		}

		void IConversationStateHandler.OnConversationUninstall()
		{
			base.MissionScreen.SetConversationActive(false);
			if (this._dataSource != null)
			{
				MissionConversationVM dataSource = this._dataSource;
				if (dataSource != null)
				{
					dataSource.OnFinalize();
				}
				this._dataSource = null;
			}
			this._conversationCategory.Unload();
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			base.MissionScreen.SetLayerCategoriesStateAndToggleOthers(new string[] { "Conversation" }, false);
			base.MissionScreen.SetLayerCategoriesState(new string[] { "SceneLayer" }, true);
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._escapeView = null;
		}

		private string GetContinueKeyText()
		{
			if (TaleWorlds.InputSystem.Input.IsGamepadActive)
			{
				GameTexts.SetVariable("CONSOLE_KEY_NAME", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("ConversationHotKeyCategory", "ContinueKey")));
				return GameTexts.FindText("str_click_to_continue_console", null).ToString();
			}
			return GameTexts.FindText("str_click_to_continue", null).ToString();
		}

		void IConversationStateHandler.OnConversationActivate()
		{
			base.MissionScreen.SetLayerCategoriesStateAndDeactivateOthers(new string[] { "Conversation", "SceneLayer" }, true);
		}

		void IConversationStateHandler.OnConversationDeactivate()
		{
			MBInformationManager.HideInformations();
		}

		void IConversationStateHandler.OnConversationContinue()
		{
			this._dataSource.OnConversationContinue();
		}

		void IConversationStateHandler.ExecuteConversationContinue()
		{
			this._dataSource.ExecuteContinue();
		}

		private bool IsGameKeyReleasedInAnyLayer(string hotKeyID, bool isDownAndReleased)
		{
			bool flag = this.IsReleasedInSceneLayer(hotKeyID, isDownAndReleased);
			bool flag2 = this.IsReleasedInGauntletLayer(hotKeyID, isDownAndReleased);
			return flag || flag2;
		}

		private bool IsReleasedInSceneLayer(string hotKeyID, bool isDownAndReleased)
		{
			if (isDownAndReleased)
			{
				SceneLayer sceneLayer = base.MissionScreen.SceneLayer;
				return sceneLayer != null && sceneLayer.Input.IsHotKeyDownAndReleased(hotKeyID);
			}
			SceneLayer sceneLayer2 = base.MissionScreen.SceneLayer;
			return sceneLayer2 != null && sceneLayer2.Input.IsHotKeyReleased(hotKeyID);
		}

		private bool IsReleasedInGauntletLayer(string hotKeyID, bool isDownAndReleased)
		{
			if (isDownAndReleased)
			{
				GauntletLayer gauntletLayer = this._gauntletLayer;
				return gauntletLayer != null && gauntletLayer.Input.IsHotKeyDownAndReleased(hotKeyID);
			}
			GauntletLayer gauntletLayer2 = this._gauntletLayer;
			return gauntletLayer2 != null && gauntletLayer2.Input.IsHotKeyReleased(hotKeyID);
		}

		private MissionConversationVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private ConversationManager _conversationManager;

		private MissionConversationCameraView _conversationCameraView;

		private MissionGauntletEscapeMenuBase _escapeView;

		private SpriteCategory _conversationCategory;
	}
}
