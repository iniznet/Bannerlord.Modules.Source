using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200004D RID: 77
	public class CampaignGameStarter : IGameStarter
	{
		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000763 RID: 1891 RVA: 0x00020502 File Offset: 0x0001E702
		public ICollection<CampaignBehaviorBase> CampaignBehaviors
		{
			get
			{
				return this._campaignBehaviors;
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000764 RID: 1892 RVA: 0x0002050A File Offset: 0x0001E70A
		public IEnumerable<GameModel> Models
		{
			get
			{
				return this._models;
			}
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x00020512 File Offset: 0x0001E712
		public CampaignGameStarter(GameMenuManager gameMenuManager, ConversationManager conversationManager, GameTextManager gameTextManager)
		{
			this._conversationManager = conversationManager;
			this._gameTextManager = gameTextManager;
			this._gameMenuManager = gameMenuManager;
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x00020545 File Offset: 0x0001E745
		public void UnregisterNonReadyObjects()
		{
			Game.Current.ObjectManager.UnregisterNonReadyObjects();
			this._gameMenuManager.UnregisterNonReadyObjects();
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x00020561 File Offset: 0x0001E761
		public void AddBehavior(CampaignBehaviorBase campaignBehavior)
		{
			if (campaignBehavior != null)
			{
				this._campaignBehaviors.Add(campaignBehavior);
			}
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x00020574 File Offset: 0x0001E774
		public void RemoveBehaviors<T>() where T : CampaignBehaviorBase
		{
			for (int i = this._campaignBehaviors.Count - 1; i >= 0; i--)
			{
				if (this._campaignBehaviors[i] is T)
				{
					this._campaignBehaviors.RemoveAt(i);
				}
			}
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x000205B8 File Offset: 0x0001E7B8
		public bool RemoveBehavior<T>(T behavior) where T : CampaignBehaviorBase
		{
			return this._campaignBehaviors.Remove(behavior);
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x000205CC File Offset: 0x0001E7CC
		public void AddModel(GameModel model)
		{
			if (model != null)
			{
				if (this._models.FindIndex((GameModel x) => x.GetType() == model.GetType()) >= 0)
				{
					throw new ArgumentException();
				}
				this._models.Add(model);
			}
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x0002061F File Offset: 0x0001E81F
		public void AddGameMenu(string menuId, string menuText, OnInitDelegate initDelegate, GameOverlays.MenuOverlayType overlay = GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags menuFlags = GameMenu.MenuFlags.None, object relatedObject = null)
		{
			this.GetPresumedGameMenu(menuId).Initialize(new TextObject(menuText, null), initDelegate, overlay, menuFlags, relatedObject);
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x0002063C File Offset: 0x0001E83C
		public void AddWaitGameMenu(string idString, string text, OnInitDelegate initDelegate, OnConditionDelegate condition, OnConsequenceDelegate consequence, OnTickDelegate tick, GameMenu.MenuAndOptionType type, GameOverlays.MenuOverlayType overlay = GameOverlays.MenuOverlayType.None, float targetWaitHours = 0f, GameMenu.MenuFlags flags = GameMenu.MenuFlags.None, object relatedObject = null)
		{
			this.GetPresumedGameMenu(idString).Initialize(new TextObject(text, null), initDelegate, condition, consequence, tick, type, overlay, targetWaitHours, flags, relatedObject);
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x00020670 File Offset: 0x0001E870
		public void AddGameMenuOption(string menuId, string optionId, string optionText, GameMenuOption.OnConditionDelegate condition, GameMenuOption.OnConsequenceDelegate consequence, bool isLeave = false, int index = -1, bool isRepeatable = false, object relatedObject = null)
		{
			this.GetPresumedGameMenu(menuId).AddOption(optionId, new TextObject(optionText, null), condition, consequence, index, isLeave, isRepeatable, relatedObject);
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x000206A0 File Offset: 0x0001E8A0
		public GameMenu GetPresumedGameMenu(string stringId)
		{
			GameMenu gameMenu = this._gameMenuManager.GetGameMenu(stringId);
			if (gameMenu == null)
			{
				gameMenu = new GameMenu(stringId);
				this._gameMenuManager.AddGameMenu(gameMenu);
			}
			return gameMenu;
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x000206D1 File Offset: 0x0001E8D1
		private ConversationSentence AddDialogLine(ConversationSentence dialogLine)
		{
			this._conversationManager.AddDialogLine(dialogLine);
			return dialogLine;
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x000206E4 File Offset: 0x0001E8E4
		public ConversationSentence AddPlayerLine(string id, string inputToken, string outputToken, string text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnConsequenceDelegate consequenceDelegate, int priority = 100, ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate = null, ConversationSentence.OnPersuasionOptionDelegate persuasionOptionDelegate = null)
		{
			return this.AddDialogLine(new ConversationSentence(id, new TextObject(text, null), inputToken, outputToken, conditionDelegate, clickableConditionDelegate, consequenceDelegate, 1U, priority, 0, 0, null, false, null, null, persuasionOptionDelegate));
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x00020718 File Offset: 0x0001E918
		public ConversationSentence AddRepeatablePlayerLine(string id, string inputToken, string outputToken, string text, string continueListingRepeatedObjectsText, string continueListingOptionOutputToken, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnConsequenceDelegate consequenceDelegate, int priority = 100, ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate = null)
		{
			ConversationSentence conversationSentence = this.AddDialogLine(new ConversationSentence(id, new TextObject(text, null), inputToken, outputToken, conditionDelegate, clickableConditionDelegate, consequenceDelegate, 3U, priority, 0, 0, null, false, null, null, null));
			this.AddDialogLine(new ConversationSentence(id + "_continue", new TextObject(continueListingRepeatedObjectsText, null), inputToken, continueListingOptionOutputToken, new ConversationSentence.OnConditionDelegate(ConversationManager.IsThereMultipleRepeatablePages), null, new ConversationSentence.OnConsequenceDelegate(ConversationManager.DialogRepeatContinueListing), 1U, priority, 0, 0, null, false, null, null, null));
			return conversationSentence;
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x00020790 File Offset: 0x0001E990
		public ConversationSentence AddDialogLineWithVariation(string id, string inputToken, string outputToken, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnConsequenceDelegate consequenceDelegate, int priority = 100, string idleActionId = "", string idleFaceAnimId = "", string reactionId = "", string reactionFaceAnimId = "", ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate = null)
		{
			return this.AddDialogLine(new ConversationSentence(id, new TextObject("{=7AyjDt96}{VARIATION_TEXT_TAGGED_LINE}", null), inputToken, outputToken, conditionDelegate, clickableConditionDelegate, consequenceDelegate, 0U, priority, 0, 0, null, true, null, null, null));
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x000207C8 File Offset: 0x0001E9C8
		public ConversationSentence AddDialogLine(string id, string inputToken, string outputToken, string text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnConsequenceDelegate consequenceDelegate, int priority = 100, ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate = null)
		{
			return this.AddDialogLine(new ConversationSentence(id, new TextObject(text, null), inputToken, outputToken, conditionDelegate, clickableConditionDelegate, consequenceDelegate, 0U, priority, 0, 0, null, false, null, null, null));
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x000207FC File Offset: 0x0001E9FC
		public ConversationSentence AddDialogLineMultiAgent(string id, string inputToken, string outputToken, TextObject text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnConsequenceDelegate consequenceDelegate, int agentIndex, int nextAgentIndex, int priority = 100, ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate = null)
		{
			return this.AddDialogLine(new ConversationSentence(id, text, inputToken, outputToken, conditionDelegate, clickableConditionDelegate, consequenceDelegate, 0U, priority, agentIndex, nextAgentIndex, null, false, null, null, null));
		}

		// Token: 0x0400028C RID: 652
		private readonly GameMenuManager _gameMenuManager;

		// Token: 0x0400028D RID: 653
		private readonly GameTextManager _gameTextManager;

		// Token: 0x0400028E RID: 654
		private readonly ConversationManager _conversationManager;

		// Token: 0x0400028F RID: 655
		private readonly List<CampaignBehaviorBase> _campaignBehaviors = new List<CampaignBehaviorBase>();

		// Token: 0x04000290 RID: 656
		private readonly List<GameModel> _models = new List<GameModel>();
	}
}
