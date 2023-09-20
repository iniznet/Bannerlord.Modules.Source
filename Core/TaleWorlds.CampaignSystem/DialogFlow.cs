using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	public class DialogFlow
	{
		private DialogFlow(string startingToken, int priority = 100)
		{
			this._currentToken = startingToken;
			this.Priority = priority;
		}

		private DialogFlow Line(TextObject text, bool byPlayer, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null, bool isRepeatable = false)
		{
			string text2 = Campaign.Current.ConversationManager.CreateToken();
			this.AddLine(text, this._currentToken, text2, byPlayer, speakerDelegate, listenerDelegate, isRepeatable, false);
			this._currentToken = text2;
			return this;
		}

		public DialogFlow Variation(string text, params object[] propertiesAndWeights)
		{
			return this.Variation(new TextObject(text, null), propertiesAndWeights);
		}

		public DialogFlow Variation(TextObject text, params object[] propertiesAndWeights)
		{
			for (int i = 0; i < propertiesAndWeights.Length; i += 2)
			{
				string text2 = (string)propertiesAndWeights[i];
				int num = Convert.ToInt32(propertiesAndWeights[i + 1]);
				List<GameTextManager.ChoiceTag> list = new List<GameTextManager.ChoiceTag>();
				list.Add(new GameTextManager.ChoiceTag(text2, num));
				this.Lines[this.Lines.Count - 1].AddVariation(text, list);
			}
			return this;
		}

		public DialogFlow NpcLine(string npcText, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.NpcLine(new TextObject(npcText, null), speakerDelegate, listenerDelegate);
		}

		public DialogFlow NpcLine(TextObject npcText, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.Line(npcText, false, speakerDelegate, listenerDelegate, false);
		}

		public DialogFlow NpcLineWithVariation(string npcText, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			DialogFlow dialogFlow = this.Line(TextObject.Empty, false, speakerDelegate, listenerDelegate, false);
			List<GameTextManager.ChoiceTag> list = new List<GameTextManager.ChoiceTag>();
			list.Add(new GameTextManager.ChoiceTag("DefaultTag", 1));
			this.Lines[this.Lines.Count - 1].AddVariation(new TextObject(npcText, null), list);
			return dialogFlow;
		}

		public DialogFlow NpcLineWithVariation(TextObject npcText, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			DialogFlow dialogFlow = this.Line(TextObject.Empty, false, speakerDelegate, listenerDelegate, false);
			List<GameTextManager.ChoiceTag> list = new List<GameTextManager.ChoiceTag>();
			list.Add(new GameTextManager.ChoiceTag("DefaultTag", 1));
			this.Lines[this.Lines.Count - 1].AddVariation(npcText, list);
			return dialogFlow;
		}

		public DialogFlow PlayerLine(string playerText, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.Line(new TextObject(playerText, null), true, null, listenerDelegate, false);
		}

		public DialogFlow PlayerLine(TextObject playerText, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.Line(playerText, true, null, listenerDelegate, false);
		}

		private DialogFlow BeginOptions(bool byPlayer)
		{
			this._curDialogFlowContext = new DialogFlowContext(this._currentToken, byPlayer, this._curDialogFlowContext);
			return this;
		}

		public DialogFlow BeginPlayerOptions()
		{
			return this.BeginOptions(true);
		}

		public DialogFlow BeginNpcOptions()
		{
			return this.BeginOptions(false);
		}

		private DialogFlow Option(TextObject text, bool byPlayer, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null, bool isRepeatable = false, bool isSpecialOption = false)
		{
			string text2 = Campaign.Current.ConversationManager.CreateToken();
			this.AddLine(text, this._curDialogFlowContext.Token, text2, byPlayer, speakerDelegate, listenerDelegate, isRepeatable, isSpecialOption);
			this._currentToken = text2;
			return this;
		}

		public DialogFlow PlayerOption(string text, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.PlayerOption(new TextObject(text, null), listenerDelegate);
		}

		public DialogFlow PlayerOption(TextObject text, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.Option(text, true, null, listenerDelegate, false, false);
			return this;
		}

		public DialogFlow PlayerSpecialOption(TextObject text, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.Option(text, true, null, listenerDelegate, false, true);
			return this;
		}

		public DialogFlow PlayerRepeatableOption(TextObject text, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.Option(text, true, null, listenerDelegate, true, false);
			return this;
		}

		public DialogFlow NpcOption(string text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.Option(new TextObject(text, null), false, speakerDelegate, listenerDelegate, false, false);
			this._lastLine.ConditionDelegate = conditionDelegate;
			return this;
		}

		public DialogFlow NpcOption(TextObject text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.Option(text, false, speakerDelegate, listenerDelegate, false, false);
			this._lastLine.ConditionDelegate = conditionDelegate;
			return this;
		}

		public DialogFlow NpcOptionWithVariation(string text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.NpcOptionWithVariation(new TextObject(text, null), conditionDelegate, speakerDelegate, listenerDelegate);
			return this;
		}

		public DialogFlow NpcOptionWithVariation(TextObject text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.Option(TextObject.Empty, false, speakerDelegate, listenerDelegate, false, false);
			List<GameTextManager.ChoiceTag> list = new List<GameTextManager.ChoiceTag>();
			list.Add(new GameTextManager.ChoiceTag("DefaultTag", 1));
			this._lastLine.AddVariation(text, list);
			this._lastLine.ConditionDelegate = conditionDelegate;
			return this;
		}

		private DialogFlow EndOptions(bool byPlayer)
		{
			this._curDialogFlowContext = this._curDialogFlowContext.Parent;
			return this;
		}

		public DialogFlow EndPlayerOptions()
		{
			return this.EndOptions(true);
		}

		public DialogFlow EndNpcOptions()
		{
			return this.EndOptions(false);
		}

		public DialogFlow Condition(ConversationSentence.OnConditionDelegate conditionDelegate)
		{
			this._lastLine.ConditionDelegate = conditionDelegate;
			return this;
		}

		public DialogFlow ClickableCondition(ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate)
		{
			this._lastLine.ClickableConditionDelegate = clickableConditionDelegate;
			return this;
		}

		public DialogFlow Consequence(ConversationSentence.OnConsequenceDelegate consequenceDelegate)
		{
			this._lastLine.ConsequenceDelegate = consequenceDelegate;
			return this;
		}

		public static DialogFlow CreateDialogFlow(string inputToken = null, int priority = 100)
		{
			return new DialogFlow(inputToken ?? Campaign.Current.ConversationManager.CreateToken(), priority);
		}

		private DialogFlowLine AddLine(TextObject text, string inputToken, string outputToken, bool byPlayer, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate, bool isRepeatable, bool isSpecialOption = false)
		{
			DialogFlowLine dialogFlowLine = new DialogFlowLine();
			dialogFlowLine.Text = text;
			dialogFlowLine.InputToken = inputToken;
			dialogFlowLine.OutputToken = outputToken;
			dialogFlowLine.ByPlayer = byPlayer;
			dialogFlowLine.SpeakerDelegate = speakerDelegate;
			dialogFlowLine.ListenerDelegate = listenerDelegate;
			dialogFlowLine.IsRepeatable = isRepeatable;
			dialogFlowLine.IsSpecialOption = isSpecialOption;
			this.Lines.Add(dialogFlowLine);
			this._lastLine = dialogFlowLine;
			return dialogFlowLine;
		}

		public DialogFlow NpcDefaultOption(string text)
		{
			return this.NpcOption(text, null, null, null);
		}

		public DialogFlow GotoDialogState(string input)
		{
			this._lastLine.OutputToken = input;
			this._currentToken = input;
			return this;
		}

		public DialogFlow GetOutputToken(out string oState)
		{
			oState = this._lastLine.OutputToken;
			return this;
		}

		public DialogFlow GoBackToDialogState(string iState)
		{
			this._currentToken = iState;
			return this;
		}

		public DialogFlow CloseDialog()
		{
			this.GotoDialogState("close_window");
			return this;
		}

		private ConversationSentence AddDialogLine(ConversationSentence dialogLine)
		{
			Campaign.Current.ConversationManager.AddDialogLine(dialogLine);
			return dialogLine;
		}

		public ConversationSentence AddPlayerLine(string id, string inputToken, string outputToken, string text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnConsequenceDelegate consequenceDelegate, object relatedObject, int priority = 100, ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate = null, ConversationSentence.OnPersuasionOptionDelegate persuasionOptionDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.AddDialogLine(new ConversationSentence(id, new TextObject(text, null), inputToken, outputToken, conditionDelegate, clickableConditionDelegate, consequenceDelegate, 1U, priority, 0, 0, relatedObject, false, speakerDelegate, listenerDelegate, persuasionOptionDelegate));
		}

		public ConversationSentence AddDialogLine(string id, string inputToken, string outputToken, string text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnConsequenceDelegate consequenceDelegate, object relatedObject, int priority = 100, ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.AddDialogLine(new ConversationSentence(id, new TextObject(text, null), inputToken, outputToken, conditionDelegate, clickableConditionDelegate, consequenceDelegate, 0U, priority, 0, 0, relatedObject, false, speakerDelegate, listenerDelegate, null));
		}

		internal readonly List<DialogFlowLine> Lines = new List<DialogFlowLine>();

		internal readonly int Priority;

		private string _currentToken;

		private DialogFlowLine _lastLine;

		private DialogFlowContext _curDialogFlowContext;
	}
}
