using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200007B RID: 123
	public class DialogFlow
	{
		// Token: 0x06000F28 RID: 3880 RVA: 0x00046BDD File Offset: 0x00044DDD
		private DialogFlow(string startingToken, int priority = 100)
		{
			this._currentToken = startingToken;
			this.Priority = priority;
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x00046C00 File Offset: 0x00044E00
		private DialogFlow Line(TextObject text, bool byPlayer, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null, bool isRepeatable = false)
		{
			string text2 = Campaign.Current.ConversationManager.CreateToken();
			this.AddLine(text, this._currentToken, text2, byPlayer, speakerDelegate, listenerDelegate, isRepeatable, false);
			this._currentToken = text2;
			return this;
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x00046C3B File Offset: 0x00044E3B
		public DialogFlow Variation(string text, params object[] propertiesAndWeights)
		{
			return this.Variation(new TextObject(text, null), propertiesAndWeights);
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x00046C4C File Offset: 0x00044E4C
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

		// Token: 0x06000F2C RID: 3884 RVA: 0x00046CAE File Offset: 0x00044EAE
		public DialogFlow NpcLine(string npcText, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.NpcLine(new TextObject(npcText, null), speakerDelegate, listenerDelegate);
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x00046CBF File Offset: 0x00044EBF
		public DialogFlow NpcLine(TextObject npcText, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.Line(npcText, false, speakerDelegate, listenerDelegate, false);
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x00046CCC File Offset: 0x00044ECC
		public DialogFlow NpcLineWithVariation(string npcText, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			DialogFlow dialogFlow = this.Line(TextObject.Empty, false, speakerDelegate, listenerDelegate, false);
			List<GameTextManager.ChoiceTag> list = new List<GameTextManager.ChoiceTag>();
			list.Add(new GameTextManager.ChoiceTag("DefaultTag", 1));
			this.Lines[this.Lines.Count - 1].AddVariation(new TextObject(npcText, null), list);
			return dialogFlow;
		}

		// Token: 0x06000F2F RID: 3887 RVA: 0x00046D24 File Offset: 0x00044F24
		public DialogFlow NpcLineWithVariation(TextObject npcText, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			DialogFlow dialogFlow = this.Line(TextObject.Empty, false, speakerDelegate, listenerDelegate, false);
			List<GameTextManager.ChoiceTag> list = new List<GameTextManager.ChoiceTag>();
			list.Add(new GameTextManager.ChoiceTag("DefaultTag", 1));
			this.Lines[this.Lines.Count - 1].AddVariation(npcText, list);
			return dialogFlow;
		}

		// Token: 0x06000F30 RID: 3888 RVA: 0x00046D76 File Offset: 0x00044F76
		public DialogFlow PlayerLine(string playerText, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.Line(new TextObject(playerText, null), true, null, listenerDelegate, false);
		}

		// Token: 0x06000F31 RID: 3889 RVA: 0x00046D89 File Offset: 0x00044F89
		public DialogFlow PlayerLine(TextObject playerText, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.Line(playerText, true, null, listenerDelegate, false);
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x00046D96 File Offset: 0x00044F96
		private DialogFlow BeginOptions(bool byPlayer)
		{
			this._curDialogFlowContext = new DialogFlowContext(this._currentToken, byPlayer, this._curDialogFlowContext);
			return this;
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x00046DB1 File Offset: 0x00044FB1
		public DialogFlow BeginPlayerOptions()
		{
			return this.BeginOptions(true);
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x00046DBA File Offset: 0x00044FBA
		public DialogFlow BeginNpcOptions()
		{
			return this.BeginOptions(false);
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x00046DC4 File Offset: 0x00044FC4
		private DialogFlow Option(TextObject text, bool byPlayer, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null, bool isRepeatable = false, bool isSpecialOption = false)
		{
			string text2 = Campaign.Current.ConversationManager.CreateToken();
			this.AddLine(text, this._curDialogFlowContext.Token, text2, byPlayer, speakerDelegate, listenerDelegate, isRepeatable, isSpecialOption);
			this._currentToken = text2;
			return this;
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x00046E05 File Offset: 0x00045005
		public DialogFlow PlayerOption(string text, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.PlayerOption(new TextObject(text, null), listenerDelegate);
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x00046E15 File Offset: 0x00045015
		public DialogFlow PlayerOption(TextObject text, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.Option(text, true, null, listenerDelegate, false, false);
			return this;
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x00046E25 File Offset: 0x00045025
		public DialogFlow PlayerSpecialOption(TextObject text, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.Option(text, true, null, listenerDelegate, false, true);
			return this;
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x00046E35 File Offset: 0x00045035
		public DialogFlow PlayerRepeatableOption(TextObject text, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.Option(text, true, null, listenerDelegate, true, false);
			return this;
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x00046E45 File Offset: 0x00045045
		public DialogFlow NpcOption(string text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.Option(new TextObject(text, null), false, speakerDelegate, listenerDelegate, false, false);
			this._lastLine.ConditionDelegate = conditionDelegate;
			return this;
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x00046E68 File Offset: 0x00045068
		public DialogFlow NpcOption(TextObject text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.Option(text, false, speakerDelegate, listenerDelegate, false, false);
			this._lastLine.ConditionDelegate = conditionDelegate;
			return this;
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x00046E85 File Offset: 0x00045085
		public DialogFlow NpcOptionWithVariation(string text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.NpcOptionWithVariation(new TextObject(text, null), conditionDelegate, speakerDelegate, listenerDelegate);
			return this;
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x00046E9C File Offset: 0x0004509C
		public DialogFlow NpcOptionWithVariation(TextObject text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			this.Option(TextObject.Empty, false, speakerDelegate, listenerDelegate, false, false);
			List<GameTextManager.ChoiceTag> list = new List<GameTextManager.ChoiceTag>();
			list.Add(new GameTextManager.ChoiceTag("DefaultTag", 1));
			this._lastLine.AddVariation(text, list);
			this._lastLine.ConditionDelegate = conditionDelegate;
			return this;
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x00046EEC File Offset: 0x000450EC
		private DialogFlow EndOptions(bool byPlayer)
		{
			this._curDialogFlowContext = this._curDialogFlowContext.Parent;
			return this;
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x00046F00 File Offset: 0x00045100
		public DialogFlow EndPlayerOptions()
		{
			return this.EndOptions(true);
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x00046F09 File Offset: 0x00045109
		public DialogFlow EndNpcOptions()
		{
			return this.EndOptions(false);
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x00046F12 File Offset: 0x00045112
		public DialogFlow Condition(ConversationSentence.OnConditionDelegate conditionDelegate)
		{
			this._lastLine.ConditionDelegate = conditionDelegate;
			return this;
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x00046F21 File Offset: 0x00045121
		public DialogFlow ClickableCondition(ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate)
		{
			this._lastLine.ClickableConditionDelegate = clickableConditionDelegate;
			return this;
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x00046F30 File Offset: 0x00045130
		public DialogFlow Consequence(ConversationSentence.OnConsequenceDelegate consequenceDelegate)
		{
			this._lastLine.ConsequenceDelegate = consequenceDelegate;
			return this;
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x00046F3F File Offset: 0x0004513F
		public static DialogFlow CreateDialogFlow(string inputToken = null, int priority = 100)
		{
			return new DialogFlow(inputToken ?? Campaign.Current.ConversationManager.CreateToken(), priority);
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x00046F5C File Offset: 0x0004515C
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

		// Token: 0x06000F46 RID: 3910 RVA: 0x00046FC0 File Offset: 0x000451C0
		public DialogFlow NpcDefaultOption(string text)
		{
			return this.NpcOption(text, null, null, null);
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x00046FCC File Offset: 0x000451CC
		public DialogFlow GotoDialogState(string input)
		{
			this._lastLine.OutputToken = input;
			this._currentToken = input;
			return this;
		}

		// Token: 0x06000F48 RID: 3912 RVA: 0x00046FE2 File Offset: 0x000451E2
		public DialogFlow GetOutputToken(out string oState)
		{
			oState = this._lastLine.OutputToken;
			return this;
		}

		// Token: 0x06000F49 RID: 3913 RVA: 0x00046FF2 File Offset: 0x000451F2
		public DialogFlow GoBackToDialogState(string iState)
		{
			this._currentToken = iState;
			return this;
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x00046FFC File Offset: 0x000451FC
		public DialogFlow CloseDialog()
		{
			this.GotoDialogState("close_window");
			return this;
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x0004700B File Offset: 0x0004520B
		private ConversationSentence AddDialogLine(ConversationSentence dialogLine)
		{
			Campaign.Current.ConversationManager.AddDialogLine(dialogLine);
			return dialogLine;
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x00047020 File Offset: 0x00045220
		public ConversationSentence AddPlayerLine(string id, string inputToken, string outputToken, string text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnConsequenceDelegate consequenceDelegate, object relatedObject, int priority = 100, ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate = null, ConversationSentence.OnPersuasionOptionDelegate persuasionOptionDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.AddDialogLine(new ConversationSentence(id, new TextObject(text, null), inputToken, outputToken, conditionDelegate, clickableConditionDelegate, consequenceDelegate, 1U, priority, 0, 0, relatedObject, false, speakerDelegate, listenerDelegate, persuasionOptionDelegate));
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x0004705C File Offset: 0x0004525C
		public ConversationSentence AddDialogLine(string id, string inputToken, string outputToken, string text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnConsequenceDelegate consequenceDelegate, object relatedObject, int priority = 100, ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null)
		{
			return this.AddDialogLine(new ConversationSentence(id, new TextObject(text, null), inputToken, outputToken, conditionDelegate, clickableConditionDelegate, consequenceDelegate, 0U, priority, 0, 0, relatedObject, false, speakerDelegate, listenerDelegate, null));
		}

		// Token: 0x04000533 RID: 1331
		internal readonly List<DialogFlowLine> Lines = new List<DialogFlowLine>();

		// Token: 0x04000534 RID: 1332
		internal readonly int Priority;

		// Token: 0x04000535 RID: 1333
		private string _currentToken;

		// Token: 0x04000536 RID: 1334
		private DialogFlowLine _lastLine;

		// Token: 0x04000537 RID: 1335
		private DialogFlowContext _curDialogFlowContext;
	}
}
