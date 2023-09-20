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
	// Token: 0x02000015 RID: 21
	[OverrideView(typeof(MissionConversationView))]
	public class MissionGauntletConversationView : MissionView, IConversationStateHandler
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00007AB6 File Offset: 0x00005CB6
		// (set) Token: 0x060000DC RID: 220 RVA: 0x00007ABE File Offset: 0x00005CBE
		public MissionConversationLogic ConversationHandler { get; private set; }

		// Token: 0x060000DD RID: 221 RVA: 0x00007AC7 File Offset: 0x00005CC7
		public MissionGauntletConversationView()
		{
			this.ViewOrderPriority = 49;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00007AD8 File Offset: 0x00005CD8
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

		// Token: 0x060000DF RID: 223 RVA: 0x00007C04 File Offset: 0x00005E04
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

		// Token: 0x060000E0 RID: 224 RVA: 0x00007C55 File Offset: 0x00005E55
		public override void EarlyStart()
		{
			base.EarlyStart();
			this.ConversationHandler = base.Mission.GetMissionBehavior<MissionConversationLogic>();
			this._conversationCameraView = base.Mission.GetMissionBehavior<MissionConversationCameraView>();
			Campaign.Current.ConversationManager.Handler = this;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00007C8F File Offset: 0x00005E8F
		public override void OnMissionScreenActivate()
		{
			base.OnMissionScreenActivate();
			if (this._dataSource != null)
			{
				base.MissionScreen.SetLayerCategoriesStateAndDeactivateOthers(new string[] { "Conversation", "SceneLayer" }, true);
				ScreenManager.TrySetFocus(this._gauntletLayer);
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00007CCC File Offset: 0x00005ECC
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

		// Token: 0x060000E3 RID: 227 RVA: 0x00007E2C File Offset: 0x0000602C
		public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
		{
			base.OnMissionModeChange(oldMissionMode, atStart);
			if (oldMissionMode == 5 && base.Mission.Mode == 1)
			{
				ScreenManager.TrySetFocus(this._gauntletLayer);
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00007E54 File Offset: 0x00006054
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

		// Token: 0x060000E5 RID: 229 RVA: 0x00007F14 File Offset: 0x00006114
		private string GetContinueKeyText()
		{
			if (TaleWorlds.InputSystem.Input.IsGamepadActive)
			{
				GameTexts.SetVariable("CONSOLE_KEY_NAME", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("ConversationHotKeyCategory", "ContinueKey")));
				return GameTexts.FindText("str_click_to_continue_console", null).ToString();
			}
			return GameTexts.FindText("str_click_to_continue", null).ToString();
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00007F67 File Offset: 0x00006167
		void IConversationStateHandler.OnConversationActivate()
		{
			base.MissionScreen.SetLayerCategoriesStateAndDeactivateOthers(new string[] { "Conversation", "SceneLayer" }, true);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00007F8B File Offset: 0x0000618B
		void IConversationStateHandler.OnConversationDeactivate()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00007F92 File Offset: 0x00006192
		void IConversationStateHandler.OnConversationContinue()
		{
			this._dataSource.OnConversationContinue();
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00007F9F File Offset: 0x0000619F
		void IConversationStateHandler.ExecuteConversationContinue()
		{
			this._dataSource.ExecuteContinue();
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00007FAC File Offset: 0x000061AC
		private bool IsGameKeyReleasedInAnyLayer(string hotKeyID, bool isDownAndReleased)
		{
			bool flag = this.IsReleasedInSceneLayer(hotKeyID, isDownAndReleased);
			bool flag2 = this.IsReleasedInGauntletLayer(hotKeyID, isDownAndReleased);
			return flag || flag2;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00007FCC File Offset: 0x000061CC
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

		// Token: 0x060000EC RID: 236 RVA: 0x0000800A File Offset: 0x0000620A
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

		// Token: 0x04000068 RID: 104
		private MissionConversationVM _dataSource;

		// Token: 0x04000069 RID: 105
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400006A RID: 106
		private ConversationManager _conversationManager;

		// Token: 0x0400006C RID: 108
		private MissionConversationCameraView _conversationCameraView;

		// Token: 0x0400006D RID: 109
		private MissionGauntletEscapeMenuBase _escapeView;

		// Token: 0x0400006E RID: 110
		private SpriteCategory _conversationCategory;
	}
}
