using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Helpers;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Conversation.Tags;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Conversation
{
	// Token: 0x020001EF RID: 495
	public class ConversationManager
	{
		// Token: 0x06001D46 RID: 7494 RVA: 0x000848B2 File Offset: 0x00082AB2
		public int CreateConversationSentenceIndex()
		{
			int numConversationSentencesCreated = this._numConversationSentencesCreated;
			this._numConversationSentencesCreated++;
			return numConversationSentencesCreated;
		}

		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x06001D47 RID: 7495 RVA: 0x000848C8 File Offset: 0x00082AC8
		public string CurrentSentenceText
		{
			get
			{
				TextObject textObject = this._currentSentenceText;
				if (this.OneToOneConversationCharacter != null)
				{
					textObject = this.FindMatchingTextOrNull(textObject.GetID(), this.OneToOneConversationCharacter);
					if (textObject == null)
					{
						textObject = this._currentSentenceText;
					}
				}
				return MBTextManager.DiscardAnimationTags(textObject.CopyTextObject().ToString());
			}
		}

		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x06001D48 RID: 7496 RVA: 0x00084911 File Offset: 0x00082B11
		private int DialogRepeatCount
		{
			get
			{
				if (this._dialogRepeatObjects.Count > 0)
				{
					return this._dialogRepeatObjects[this._currentRepeatedDialogSetIndex].Count;
				}
				return 1;
			}
		}

		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x06001D49 RID: 7497 RVA: 0x00084939 File Offset: 0x00082B39
		public bool IsConversationFlowActive
		{
			get
			{
				return this._isActive;
			}
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x00084944 File Offset: 0x00082B44
		public ConversationManager()
		{
			this._sentences = new List<ConversationSentence>();
			this.stateMap = new Dictionary<string, int>();
			this.stateMap.Add("start", 0);
			this.stateMap.Add("event_triggered", 1);
			this.stateMap.Add("member_chat", 2);
			this.stateMap.Add("prisoner_chat", 3);
			this.stateMap.Add("close_window", 4);
			this._numberOfStateIndices = 5;
			this._isActive = false;
			this._executeDoOptionContinue = false;
			this.InitializeTags();
			this.ConversationAnimationManager = new ConversationAnimationManager();
		}

		// Token: 0x1700075A RID: 1882
		// (get) Token: 0x06001D4B RID: 7499 RVA: 0x00084A14 File Offset: 0x00082C14
		// (set) Token: 0x06001D4C RID: 7500 RVA: 0x00084A1C File Offset: 0x00082C1C
		public List<ConversationSentenceOption> CurOptions { get; protected set; }

		// Token: 0x06001D4D RID: 7501 RVA: 0x00084A28 File Offset: 0x00082C28
		public void StartNew(int startingToken, bool setActionsInstantly)
		{
			this.ActiveToken = startingToken;
			this._currentSentence = -1;
			this.ResetRepeatedDialogSystem();
			this._lastSelectedDialogObject = null;
			Debug.Print("--------------- Conversation Start --------------- ", 5, Debug.DebugColor.White, 4503599627370496UL);
			Debug.Print(string.Concat(new object[]
			{
				"Conversation character name: ",
				this.OneToOneConversationCharacter.Name,
				"\nid: ",
				this.OneToOneConversationCharacter.StringId,
				"\nculture:",
				this.OneToOneConversationCharacter.Culture,
				"\npersona:",
				this.OneToOneConversationCharacter.GetPersona().Name
			}), 5, Debug.DebugColor.White, 17592186044416UL);
			if (CampaignMission.Current != null)
			{
				foreach (IAgent agent in this.ConversationAgents)
				{
					CampaignMission.Current.OnConversationStart(agent, setActionsInstantly);
				}
			}
			this.ProcessPartnerSentence();
		}

		// Token: 0x06001D4E RID: 7502 RVA: 0x00084B34 File Offset: 0x00082D34
		private bool ProcessPartnerSentence()
		{
			List<ConversationSentenceOption> sentenceOptions = this.GetSentenceOptions(false, false);
			bool flag = false;
			if (sentenceOptions.Count > 0)
			{
				this.ProcessSentence(sentenceOptions[0]);
				flag = true;
			}
			IConversationStateHandler handler = this.Handler;
			if (handler != null)
			{
				handler.OnConversationContinue();
			}
			return flag;
		}

		// Token: 0x06001D4F RID: 7503 RVA: 0x00084B78 File Offset: 0x00082D78
		public void ProcessSentence(ConversationSentenceOption conversationSentenceOption)
		{
			ConversationSentence conversationSentence = this._sentences[conversationSentenceOption.SentenceNo];
			Debug.Print(conversationSentenceOption.DebugInfo, 0, Debug.DebugColor.White, 4503599627370496UL);
			this.ActiveToken = conversationSentence.OutputToken;
			this.UpdateSpeakerAndListenerAgents(conversationSentence);
			if (CampaignMission.Current != null)
			{
				CampaignMission.Current.OnProcessSentence();
			}
			this._lastSelectedDialogObject = conversationSentenceOption.RepeatObject;
			this._currentSentence = conversationSentenceOption.SentenceNo;
			if (Game.Current == null)
			{
				throw new MBNullParameterException("Game");
			}
			this.UpdateCurrentSentenceText();
			int count = this._sentences.Count;
			conversationSentence.RunConsequence(Game.Current);
			if (CampaignMission.Current != null)
			{
				string[] conversationAnimations = MBTextManager.GetConversationAnimations(this._currentSentenceText);
				string text = "";
				VoiceObject voiceObject;
				if (MBTextManager.TryGetVoiceObject(this._currentSentenceText, out voiceObject))
				{
					text = Campaign.Current.Models.VoiceOverModel.GetSoundPathForCharacter((CharacterObject)this.SpeakerAgent.Character, voiceObject);
				}
				CampaignMission.Current.OnConversationPlay(conversationAnimations[0], conversationAnimations[1], conversationAnimations[2], conversationAnimations[3], text);
			}
			if (0 > this._currentSentence || this._currentSentence >= count)
			{
				Debug.FailedAssert("CurrentSentence is not valid.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Conversation\\ConversationManager.cs", "ProcessSentence", 390);
			}
		}

		// Token: 0x06001D50 RID: 7504 RVA: 0x00084CAC File Offset: 0x00082EAC
		private void UpdateSpeakerAndListenerAgents(ConversationSentence sentence)
		{
			if (sentence.IsSpeaker != null)
			{
				if (sentence.IsSpeaker(this._mainAgent))
				{
					this.SetSpeakerAgent(this._mainAgent);
					goto IL_8B;
				}
				using (IEnumerator<IAgent> enumerator = this.ConversationAgents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IAgent agent = enumerator.Current;
						if (sentence.IsSpeaker(agent))
						{
							this.SetSpeakerAgent(agent);
							break;
						}
					}
					goto IL_8B;
				}
			}
			this.SetSpeakerAgent((!sentence.IsPlayer) ? this.ConversationAgents[0] : this._mainAgent);
			IL_8B:
			if (sentence.IsListener != null)
			{
				if (sentence.IsListener(this._mainAgent))
				{
					this.SetListenerAgent(this._mainAgent);
					return;
				}
				using (IEnumerator<IAgent> enumerator = this.ConversationAgents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IAgent agent2 = enumerator.Current;
						if (sentence.IsListener(agent2))
						{
							this.SetListenerAgent(agent2);
							break;
						}
					}
					return;
				}
			}
			this.SetListenerAgent((!sentence.IsPlayer) ? this._mainAgent : this.ConversationAgents[0]);
		}

		// Token: 0x06001D51 RID: 7505 RVA: 0x00084DEC File Offset: 0x00082FEC
		private void SetSpeakerAgent(IAgent agent)
		{
			if (this._speakerAgent != agent)
			{
				this._speakerAgent = agent;
				if (this._speakerAgent != null && this._speakerAgent.Character is CharacterObject)
				{
					StringHelpers.SetCharacterProperties("SPEAKER", agent.Character as CharacterObject, null, false);
				}
			}
		}

		// Token: 0x06001D52 RID: 7506 RVA: 0x00084E3C File Offset: 0x0008303C
		private void SetListenerAgent(IAgent agent)
		{
			if (this._listenerAgent != agent)
			{
				this._listenerAgent = agent;
				if (this._listenerAgent != null && this._listenerAgent.Character is CharacterObject)
				{
					StringHelpers.SetCharacterProperties("LISTENER", agent.Character as CharacterObject, null, false);
				}
			}
		}

		// Token: 0x06001D53 RID: 7507 RVA: 0x00084E8C File Offset: 0x0008308C
		public void UpdateCurrentSentenceText()
		{
			TextObject textObject;
			if (this._currentSentence >= 0)
			{
				textObject = this._sentences[this._currentSentence].Text;
			}
			else
			{
				if (Campaign.Current == null)
				{
					throw new MBNullParameterException("Campaign");
				}
				textObject = GameTexts.FindText("str_error_string", null);
			}
			this._currentSentenceText = textObject;
		}

		// Token: 0x06001D54 RID: 7508 RVA: 0x00084EE0 File Offset: 0x000830E0
		public bool IsConversationEnded()
		{
			return this.ActiveToken == 4;
		}

		// Token: 0x06001D55 RID: 7509 RVA: 0x00084EEB File Offset: 0x000830EB
		public void ClearCurrentOptions()
		{
			if (this.CurOptions == null)
			{
				this.CurOptions = new List<ConversationSentenceOption>();
			}
			this.CurOptions.Clear();
		}

		// Token: 0x06001D56 RID: 7510 RVA: 0x00084F0C File Offset: 0x0008310C
		public void AddToCurrentOptions(TextObject text, string id, bool isClickable, TextObject hintText)
		{
			ConversationSentenceOption conversationSentenceOption = new ConversationSentenceOption
			{
				SentenceNo = 0,
				Text = text,
				Id = id,
				RepeatObject = null,
				DebugInfo = null,
				IsClickable = isClickable,
				HintText = hintText
			};
			this.CurOptions.Add(conversationSentenceOption);
		}

		// Token: 0x06001D57 RID: 7511 RVA: 0x00084F68 File Offset: 0x00083168
		public void GetPlayerSentenceOptions()
		{
			this.CurOptions = this.GetSentenceOptions(true, true);
			if (this.CurOptions.Count > 0)
			{
				ConversationSentenceOption conversationSentenceOption = this.CurOptions[0];
				foreach (ConversationSentenceOption conversationSentenceOption2 in this.CurOptions)
				{
					if (this._sentences[conversationSentenceOption2.SentenceNo].IsListener != null)
					{
						conversationSentenceOption = conversationSentenceOption2;
						break;
					}
				}
				this.UpdateSpeakerAndListenerAgents(this._sentences[conversationSentenceOption.SentenceNo]);
			}
		}

		// Token: 0x06001D58 RID: 7512 RVA: 0x00085010 File Offset: 0x00083210
		public int GetStateIndex(string str)
		{
			int num;
			if (this.stateMap.ContainsKey(str))
			{
				num = this.stateMap[str];
			}
			else
			{
				num = this._numberOfStateIndices;
				Dictionary<string, int> dictionary = this.stateMap;
				int numberOfStateIndices = this._numberOfStateIndices;
				this._numberOfStateIndices = numberOfStateIndices + 1;
				dictionary.Add(str, numberOfStateIndices);
			}
			return num;
		}

		// Token: 0x06001D59 RID: 7513 RVA: 0x0008505F File Offset: 0x0008325F
		internal void Build()
		{
			this.SortSentences();
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x00085067 File Offset: 0x00083267
		public void DisableSentenceSort()
		{
			this._sortSentenceIsDisabled = true;
		}

		// Token: 0x06001D5B RID: 7515 RVA: 0x00085070 File Offset: 0x00083270
		public void EnableSentenceSort()
		{
			this._sortSentenceIsDisabled = false;
			this.SortSentences();
		}

		// Token: 0x06001D5C RID: 7516 RVA: 0x0008507F File Offset: 0x0008327F
		private void SortSentences()
		{
			this._sentences = this._sentences.OrderByDescending((ConversationSentence pair) => pair.Priority).ToList<ConversationSentence>();
		}

		// Token: 0x06001D5D RID: 7517 RVA: 0x000850B8 File Offset: 0x000832B8
		private void SortLastSentence()
		{
			int num = this._sentences.Count - 1;
			ConversationSentence conversationSentence = this._sentences[num];
			int priority = conversationSentence.Priority;
			int num2 = num - 1;
			while (num2 >= 0 && this._sentences[num2].Priority < priority)
			{
				this._sentences[num2 + 1] = this._sentences[num2];
				num = num2;
				num2--;
			}
			this._sentences[num] = conversationSentence;
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x00085134 File Offset: 0x00083334
		private List<ConversationSentenceOption> GetSentenceOptions(bool onlyPlayer, bool processAfterOneOption)
		{
			List<ConversationSentenceOption> list = new List<ConversationSentenceOption>();
			ConversationManager.SetupTextVariables();
			for (int i = 0; i < this._sentences.Count; i++)
			{
				if (this.GetSentenceMatch(i, onlyPlayer))
				{
					ConversationSentence conversationSentence = this._sentences[i];
					int num = 1;
					this._dialogRepeatLines.Clear();
					this._currentRepeatIndex = 0;
					if (conversationSentence.IsRepeatable)
					{
						num = this.DialogRepeatCount;
					}
					for (int j = 0; j < num; j++)
					{
						this._dialogRepeatLines.Add(conversationSentence.Text.CopyTextObject());
						if (conversationSentence.RunCondition())
						{
							conversationSentence.IsClickable = conversationSentence.RunClickableCondition();
							if (conversationSentence.IsWithVariation)
							{
								TextObject textObject = this.FindMatchingTextOrNull(conversationSentence.Id, this.OneToOneConversationCharacter);
								GameTexts.SetVariable("VARIATION_TEXT_TAGGED_LINE", textObject);
							}
							string text = (conversationSentence.IsPlayer ? "P  -> (" : "AI -> (") + conversationSentence.Id + ") - ";
							ConversationSentenceOption conversationSentenceOption = new ConversationSentenceOption
							{
								SentenceNo = i,
								Text = this.GetCurrentDialogLine(),
								Id = conversationSentence.Id,
								RepeatObject = this.GetCurrentProcessedRepeatObject(),
								DebugInfo = text,
								IsClickable = conversationSentence.IsClickable,
								HasPersuasion = conversationSentence.HasPersuasion,
								SkillName = conversationSentence.SkillName,
								TraitName = conversationSentence.TraitName,
								IsSpecial = conversationSentence.IsSpecial,
								HintText = conversationSentence.HintText,
								PersuationOptionArgs = conversationSentence.PersuationOptionArgs
							};
							list.Add(conversationSentenceOption);
							if (conversationSentence.IsRepeatable)
							{
								this._currentRepeatIndex++;
							}
							if (!processAfterOneOption)
							{
								return list;
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x00085300 File Offset: 0x00083500
		private bool GetSentenceMatch(int sentenceIndex, bool onlyPlayer)
		{
			if (0 > sentenceIndex || sentenceIndex >= this._sentences.Count)
			{
				throw new MBOutOfRangeException("Sentence index is not valid.");
			}
			bool flag = this._sentences[sentenceIndex].InputToken != this.ActiveToken;
			if (!flag && onlyPlayer)
			{
				flag = !this._sentences[sentenceIndex].IsPlayer;
			}
			return !flag;
		}

		// Token: 0x06001D60 RID: 7520 RVA: 0x00085368 File Offset: 0x00083568
		internal object GetCurrentProcessedRepeatObject()
		{
			if (this._dialogRepeatObjects.Count <= 0)
			{
				return null;
			}
			return this._dialogRepeatObjects[this._currentRepeatedDialogSetIndex][this._currentRepeatIndex];
		}

		// Token: 0x06001D61 RID: 7521 RVA: 0x00085396 File Offset: 0x00083596
		internal TextObject GetCurrentDialogLine()
		{
			if (this._dialogRepeatLines.Count <= this._currentRepeatIndex)
			{
				return null;
			}
			return this._dialogRepeatLines[this._currentRepeatIndex];
		}

		// Token: 0x06001D62 RID: 7522 RVA: 0x000853BE File Offset: 0x000835BE
		internal object GetSelectedRepeatObject()
		{
			return this._lastSelectedDialogObject;
		}

		// Token: 0x06001D63 RID: 7523 RVA: 0x000853C8 File Offset: 0x000835C8
		internal void SetDialogRepeatCount(IReadOnlyList<object> dialogRepeatObjects, int maxRepeatedDialogsInConversation)
		{
			this._dialogRepeatObjects.Clear();
			bool flag = dialogRepeatObjects.Count > maxRepeatedDialogsInConversation + 1;
			List<object> list = new List<object>(maxRepeatedDialogsInConversation);
			for (int i = 0; i < dialogRepeatObjects.Count; i++)
			{
				object obj = dialogRepeatObjects[i];
				if (flag && i % maxRepeatedDialogsInConversation == 0)
				{
					list = new List<object>(maxRepeatedDialogsInConversation);
					this._dialogRepeatObjects.Add(list);
				}
				list.Add(obj);
			}
			if (!flag && !list.IsEmpty<object>())
			{
				this._dialogRepeatObjects.Add(list);
			}
			this._currentRepeatedDialogSetIndex = 0;
			this._currentRepeatIndex = 0;
		}

		// Token: 0x06001D64 RID: 7524 RVA: 0x00085454 File Offset: 0x00083654
		internal static void DialogRepeatContinueListing()
		{
			Campaign campaign = Campaign.Current;
			ConversationManager conversationManager = ((campaign != null) ? campaign.ConversationManager : null);
			if (conversationManager != null)
			{
				conversationManager._currentRepeatedDialogSetIndex++;
				if (conversationManager._currentRepeatedDialogSetIndex >= conversationManager._dialogRepeatObjects.Count)
				{
					conversationManager._currentRepeatedDialogSetIndex = 0;
				}
				conversationManager._currentRepeatIndex = 0;
			}
		}

		// Token: 0x06001D65 RID: 7525 RVA: 0x000854A8 File Offset: 0x000836A8
		internal static bool IsThereMultipleRepeatablePages()
		{
			Campaign campaign = Campaign.Current;
			if (campaign == null)
			{
				return false;
			}
			ConversationManager conversationManager = campaign.ConversationManager;
			int? num = ((conversationManager != null) ? new int?(conversationManager._dialogRepeatObjects.Count) : null);
			int num2 = 1;
			return (num.GetValueOrDefault() > num2) & (num != null);
		}

		// Token: 0x06001D66 RID: 7526 RVA: 0x000854F8 File Offset: 0x000836F8
		private void ResetRepeatedDialogSystem()
		{
			this._currentRepeatedDialogSetIndex = 0;
			this._currentRepeatIndex = 0;
			this._dialogRepeatObjects.Clear();
			this._dialogRepeatLines.Clear();
		}

		// Token: 0x06001D67 RID: 7527 RVA: 0x0008551E File Offset: 0x0008371E
		internal ConversationSentence AddDialogLine(ConversationSentence dialogLine)
		{
			this._sentences.Add(dialogLine);
			if (!this._sortSentenceIsDisabled)
			{
				this.SortLastSentence();
			}
			return dialogLine;
		}

		// Token: 0x06001D68 RID: 7528 RVA: 0x0008553C File Offset: 0x0008373C
		public void AddDialogFlow(DialogFlow dialogFlow, object relatedObject = null)
		{
			foreach (DialogFlowLine dialogFlowLine in dialogFlow.Lines)
			{
				string text = this.CreateId();
				uint num = (dialogFlowLine.ByPlayer ? 1U : 0U) | (dialogFlowLine.IsRepeatable ? 2U : 0U) | (dialogFlowLine.IsSpecialOption ? 4U : 0U);
				this.AddDialogLine(new ConversationSentence(text, dialogFlowLine.HasVariation ? new TextObject("{=7AyjDt96}{VARIATION_TEXT_TAGGED_LINE}", null) : dialogFlowLine.Text, dialogFlowLine.InputToken, dialogFlowLine.OutputToken, dialogFlowLine.ConditionDelegate, dialogFlowLine.ClickableConditionDelegate, dialogFlowLine.ConsequenceDelegate, num, dialogFlow.Priority, 0, 0, relatedObject, dialogFlowLine.HasVariation, dialogFlowLine.SpeakerDelegate, dialogFlowLine.ListenerDelegate, null));
				GameText gameText = Game.Current.GameTextManager.AddGameText(text);
				foreach (KeyValuePair<TextObject, List<GameTextManager.ChoiceTag>> keyValuePair in dialogFlowLine.Variations)
				{
					gameText.AddVariationWithId("", keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		// Token: 0x06001D69 RID: 7529 RVA: 0x000856A4 File Offset: 0x000838A4
		public ConversationSentence AddDialogLineMultiAgent(string id, string inputToken, string outputToken, TextObject text, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnConsequenceDelegate consequenceDelegate, int agentIndex, int nextAgentIndex, int priority = 100, ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate = null)
		{
			return this.AddDialogLine(new ConversationSentence(id, text, inputToken, outputToken, conditionDelegate, clickableConditionDelegate, consequenceDelegate, 0U, priority, agentIndex, nextAgentIndex, null, false, null, null, null));
		}

		// Token: 0x06001D6A RID: 7530 RVA: 0x000856D3 File Offset: 0x000838D3
		internal string CreateToken()
		{
			string text = string.Format("atk:{0}", this._autoToken);
			this._autoToken++;
			return text;
		}

		// Token: 0x06001D6B RID: 7531 RVA: 0x000856F8 File Offset: 0x000838F8
		private string CreateId()
		{
			string text = string.Format("adg:{0}", this._autoId);
			this._autoId++;
			return text;
		}

		// Token: 0x06001D6C RID: 7532 RVA: 0x0008571D File Offset: 0x0008391D
		internal void SetupGameStringsForConversation()
		{
			StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, null, false);
		}

		// Token: 0x06001D6D RID: 7533 RVA: 0x00085736 File Offset: 0x00083936
		internal void OnConsequence(ConversationSentence sentence)
		{
			Action<ConversationSentence> consequenceRunned = this.ConsequenceRunned;
			if (consequenceRunned == null)
			{
				return;
			}
			consequenceRunned(sentence);
		}

		// Token: 0x06001D6E RID: 7534 RVA: 0x00085749 File Offset: 0x00083949
		internal void OnCondition(ConversationSentence sentence)
		{
			Action<ConversationSentence> conditionRunned = this.ConditionRunned;
			if (conditionRunned == null)
			{
				return;
			}
			conditionRunned(sentence);
		}

		// Token: 0x06001D6F RID: 7535 RVA: 0x0008575C File Offset: 0x0008395C
		internal void OnClickableCondition(ConversationSentence sentence)
		{
			Action<ConversationSentence> clickableConditionRunned = this.ClickableConditionRunned;
			if (clickableConditionRunned == null)
			{
				return;
			}
			clickableConditionRunned(sentence);
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06001D70 RID: 7536 RVA: 0x00085770 File Offset: 0x00083970
		// (remove) Token: 0x06001D71 RID: 7537 RVA: 0x000857A8 File Offset: 0x000839A8
		public event Action<ConversationSentence> ConsequenceRunned;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06001D72 RID: 7538 RVA: 0x000857E0 File Offset: 0x000839E0
		// (remove) Token: 0x06001D73 RID: 7539 RVA: 0x00085818 File Offset: 0x00083A18
		public event Action<ConversationSentence> ConditionRunned;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06001D74 RID: 7540 RVA: 0x00085850 File Offset: 0x00083A50
		// (remove) Token: 0x06001D75 RID: 7541 RVA: 0x00085888 File Offset: 0x00083A88
		public event Action<ConversationSentence> ClickableConditionRunned;

		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x06001D76 RID: 7542 RVA: 0x000858BD File Offset: 0x00083ABD
		public IReadOnlyList<IAgent> ConversationAgents
		{
			get
			{
				return this._conversationAgents;
			}
		}

		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x06001D77 RID: 7543 RVA: 0x000858C5 File Offset: 0x00083AC5
		public IAgent OneToOneConversationAgent
		{
			get
			{
				if (this.ConversationAgents.IsEmpty<IAgent>() || this.ConversationAgents.Count > 1)
				{
					return null;
				}
				return this.ConversationAgents[0];
			}
		}

		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x06001D78 RID: 7544 RVA: 0x000858F0 File Offset: 0x00083AF0
		public IAgent SpeakerAgent
		{
			get
			{
				if (this.ConversationAgents != null)
				{
					return this._speakerAgent;
				}
				return null;
			}
		}

		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x06001D79 RID: 7545 RVA: 0x00085902 File Offset: 0x00083B02
		public IAgent ListenerAgent
		{
			get
			{
				if (this.ConversationAgents != null)
				{
					return this._listenerAgent;
				}
				return null;
			}
		}

		// Token: 0x1700075F RID: 1887
		// (get) Token: 0x06001D7A RID: 7546 RVA: 0x00085914 File Offset: 0x00083B14
		// (set) Token: 0x06001D7B RID: 7547 RVA: 0x0008591C File Offset: 0x00083B1C
		public bool IsConversationInProgress { get; private set; }

		// Token: 0x17000760 RID: 1888
		// (get) Token: 0x06001D7C RID: 7548 RVA: 0x00085925 File Offset: 0x00083B25
		public Hero OneToOneConversationHero
		{
			get
			{
				if (this.OneToOneConversationCharacter != null)
				{
					return this.OneToOneConversationCharacter.HeroObject;
				}
				return null;
			}
		}

		// Token: 0x17000761 RID: 1889
		// (get) Token: 0x06001D7D RID: 7549 RVA: 0x0008593C File Offset: 0x00083B3C
		public CharacterObject OneToOneConversationCharacter
		{
			get
			{
				if (this.OneToOneConversationAgent != null)
				{
					return (CharacterObject)this.OneToOneConversationAgent.Character;
				}
				return null;
			}
		}

		// Token: 0x17000762 RID: 1890
		// (get) Token: 0x06001D7E RID: 7550 RVA: 0x00085958 File Offset: 0x00083B58
		public IEnumerable<CharacterObject> ConversationCharacters
		{
			get
			{
				new List<CharacterObject>();
				foreach (IAgent agent in this.ConversationAgents)
				{
					yield return (CharacterObject)agent.Character;
				}
				IEnumerator<IAgent> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x06001D7F RID: 7551 RVA: 0x00085968 File Offset: 0x00083B68
		public bool IsAgentInConversation(IAgent agent)
		{
			return this.ConversationAgents.Contains(agent);
		}

		// Token: 0x17000763 RID: 1891
		// (get) Token: 0x06001D80 RID: 7552 RVA: 0x00085976 File Offset: 0x00083B76
		public MobileParty ConversationParty
		{
			get
			{
				return this._conversationParty;
			}
		}

		// Token: 0x17000764 RID: 1892
		// (get) Token: 0x06001D81 RID: 7553 RVA: 0x0008597E File Offset: 0x00083B7E
		// (set) Token: 0x06001D82 RID: 7554 RVA: 0x00085986 File Offset: 0x00083B86
		public bool NeedsToActivateForMapConversation { get; private set; }

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06001D83 RID: 7555 RVA: 0x00085990 File Offset: 0x00083B90
		// (remove) Token: 0x06001D84 RID: 7556 RVA: 0x000859C8 File Offset: 0x00083BC8
		public event Action ConversationSetup;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06001D85 RID: 7557 RVA: 0x00085A00 File Offset: 0x00083C00
		// (remove) Token: 0x06001D86 RID: 7558 RVA: 0x00085A38 File Offset: 0x00083C38
		public event Action ConversationBegin;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06001D87 RID: 7559 RVA: 0x00085A70 File Offset: 0x00083C70
		// (remove) Token: 0x06001D88 RID: 7560 RVA: 0x00085AA8 File Offset: 0x00083CA8
		public event Action ConversationEnd;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06001D89 RID: 7561 RVA: 0x00085AE0 File Offset: 0x00083CE0
		// (remove) Token: 0x06001D8A RID: 7562 RVA: 0x00085B18 File Offset: 0x00083D18
		public event Action ConversationEndOneShot;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06001D8B RID: 7563 RVA: 0x00085B50 File Offset: 0x00083D50
		// (remove) Token: 0x06001D8C RID: 7564 RVA: 0x00085B88 File Offset: 0x00083D88
		public event Action ConversationContinued;

		// Token: 0x06001D8D RID: 7565 RVA: 0x00085BBD File Offset: 0x00083DBD
		private void SetupConversation()
		{
			this.IsConversationInProgress = true;
			IConversationStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnConversationInstall();
		}

		// Token: 0x06001D8E RID: 7566 RVA: 0x00085BD6 File Offset: 0x00083DD6
		public void BeginConversation()
		{
			this.IsConversationInProgress = true;
			if (this.ConversationSetup != null)
			{
				this.ConversationSetup();
			}
			if (this.ConversationBegin != null)
			{
				this.ConversationBegin();
			}
			this.NeedsToActivateForMapConversation = false;
		}

		// Token: 0x06001D8F RID: 7567 RVA: 0x00085C0C File Offset: 0x00083E0C
		public void EndConversation()
		{
			Debug.Print("--------------- Conversation End --------------- ", 0, Debug.DebugColor.White, 4503599627370496UL);
			if (CampaignMission.Current != null)
			{
				foreach (IAgent agent in this.ConversationAgents)
				{
					CampaignMission.Current.OnConversationEnd(agent);
				}
			}
			this._conversationParty = null;
			if (this.ConversationEndOneShot != null)
			{
				this.ConversationEndOneShot();
				this.ConversationEndOneShot = null;
			}
			if (this.ConversationEnd != null)
			{
				this.ConversationEnd();
			}
			this.IsConversationInProgress = false;
			foreach (IAgent agent2 in this.ConversationAgents)
			{
				agent2.SetAsConversationAgent(false);
			}
			Campaign.Current.CurrentConversationContext = ConversationContext.Default;
			CampaignEventDispatcher.Instance.OnConversationEnded(this.ConversationCharacters);
			if (ConversationManager.GetPersuasionIsActive())
			{
				ConversationManager.EndPersuasion();
			}
			this._conversationAgents.Clear();
			this._speakerAgent = null;
			this._listenerAgent = null;
			this._mainAgent = null;
			if (this.IsConversationFlowActive)
			{
				this.OnConversationDeactivate();
			}
			IConversationStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnConversationUninstall();
		}

		// Token: 0x06001D90 RID: 7568 RVA: 0x00085D54 File Offset: 0x00083F54
		public void DoOption(int optionIndex)
		{
			this.LastSelectedButtonIndex = optionIndex;
			this.ProcessSentence(this.CurOptions[optionIndex]);
			if (this._isActive)
			{
				this.DoOptionContinue();
				return;
			}
			this._executeDoOptionContinue = true;
		}

		// Token: 0x06001D91 RID: 7569 RVA: 0x00085D88 File Offset: 0x00083F88
		public void DoOption(string optionID)
		{
			this.LastSelectedDialog = optionID;
			int count = Campaign.Current.ConversationManager.CurOptions.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.CurOptions[i].Id == optionID)
				{
					this.ProcessSentence(this.CurOptions[i]);
				}
			}
			if (this._isActive)
			{
				this.DoOptionContinue();
				return;
			}
			this._executeDoOptionContinue = true;
		}

		// Token: 0x06001D92 RID: 7570 RVA: 0x00085DFE File Offset: 0x00083FFE
		public void DoConversationContinuedCallback()
		{
			if (this.ConversationContinued != null)
			{
				this.ConversationContinued();
			}
		}

		// Token: 0x06001D93 RID: 7571 RVA: 0x00085E13 File Offset: 0x00084013
		public void DoOptionContinue()
		{
			if (this.IsConversationEnded() && this._sentences[this._currentSentence].IsPlayer)
			{
				this.EndConversation();
				return;
			}
			this.ProcessPartnerSentence();
			this.DoConversationContinuedCallback();
		}

		// Token: 0x06001D94 RID: 7572 RVA: 0x00085E4C File Offset: 0x0008404C
		public void ContinueConversation()
		{
			if (this.CurOptions.Count <= 1)
			{
				if (this.IsConversationEnded())
				{
					this.EndConversation();
					return;
				}
				if (!this.ProcessPartnerSentence() && this.ListenerAgent.Character == Hero.MainHero.CharacterObject)
				{
					this.EndConversation();
					return;
				}
				this.DoConversationContinuedCallback();
				if (CampaignMission.Current != null)
				{
					CampaignMission.Current.OnConversationContinue();
				}
			}
		}

		// Token: 0x06001D95 RID: 7573 RVA: 0x00085EB4 File Offset: 0x000840B4
		public void SetupAndStartMissionConversation(IAgent agent, IAgent mainAgent, bool setActionsInstantly)
		{
			CampaignEvents.SetupPreConversation();
			this.SetupConversation();
			this._mainAgent = mainAgent;
			this._conversationAgents.Clear();
			this.AddConversationAgent(agent);
			this._conversationParty = null;
			this.StartNew(0, setActionsInstantly);
			if (!this.IsConversationFlowActive)
			{
				this.OnConversationActivate();
			}
			this.BeginConversation();
		}

		// Token: 0x06001D96 RID: 7574 RVA: 0x00085F08 File Offset: 0x00084108
		public void SetupAndStartMissionConversationWithMultipleAgents(IEnumerable<IAgent> agents, IAgent mainAgent)
		{
			this.SetupConversation();
			this._mainAgent = mainAgent;
			this._conversationAgents.Clear();
			this.AddConversationAgents(agents, true);
			this._conversationParty = null;
			this.StartNew(0, true);
			if (!this.IsConversationFlowActive)
			{
				this.OnConversationActivate();
			}
			this.BeginConversation();
		}

		// Token: 0x06001D97 RID: 7575 RVA: 0x00085F58 File Offset: 0x00084158
		public void SetupAndStartMapConversation(MobileParty party, IAgent agent, IAgent mainAgent)
		{
			this._conversationParty = party;
			CampaignEvents.SetupPreConversation();
			this._mainAgent = mainAgent;
			this._conversationAgents.Clear();
			this.AddConversationAgent(agent);
			this.SetupConversation();
			this.StartNew(0, true);
			this.NeedsToActivateForMapConversation = true;
			if (!this.IsConversationFlowActive)
			{
				this.OnConversationActivate();
			}
		}

		// Token: 0x06001D98 RID: 7576 RVA: 0x00085FB0 File Offset: 0x000841B0
		public void AddConversationAgents(IEnumerable<IAgent> agents, bool setActionsInstantly)
		{
			foreach (IAgent agent in agents)
			{
				if (agent.IsActive() && !this.ConversationAgents.Contains(agent))
				{
					this.AddConversationAgent(agent);
					CampaignMission.Current.OnConversationStart(agent, setActionsInstantly);
				}
			}
		}

		// Token: 0x06001D99 RID: 7577 RVA: 0x0008601C File Offset: 0x0008421C
		private void AddConversationAgent(IAgent agent)
		{
			this._conversationAgents.Add(agent);
			agent.SetAsConversationAgent(true);
			CampaignEventDispatcher.Instance.OnAgentJoinedConversation(agent);
		}

		// Token: 0x06001D9A RID: 7578 RVA: 0x0008603C File Offset: 0x0008423C
		public bool IsConversationAgent(IAgent agent)
		{
			return this.ConversationAgents != null && this.ConversationAgents.Contains(agent);
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x00086054 File Offset: 0x00084254
		public void RemoveRelatedLines(object o)
		{
			this._sentences.RemoveAll((ConversationSentence s) => s.RelatedObject == o);
		}

		// Token: 0x17000765 RID: 1893
		// (get) Token: 0x06001D9C RID: 7580 RVA: 0x00086086 File Offset: 0x00084286
		// (set) Token: 0x06001D9D RID: 7581 RVA: 0x0008608E File Offset: 0x0008428E
		public IConversationStateHandler Handler { get; set; }

		// Token: 0x06001D9E RID: 7582 RVA: 0x00086097 File Offset: 0x00084297
		public void OnConversationDeactivate()
		{
			this._isActive = false;
			IConversationStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnConversationDeactivate();
		}

		// Token: 0x06001D9F RID: 7583 RVA: 0x000860B0 File Offset: 0x000842B0
		public void OnConversationActivate()
		{
			this._isActive = true;
			if (this._executeDoOptionContinue)
			{
				this._executeDoOptionContinue = false;
				this.DoOptionContinue();
			}
			IConversationStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnConversationActivate();
		}

		// Token: 0x06001DA0 RID: 7584 RVA: 0x000860E0 File Offset: 0x000842E0
		public TextObject FindMatchingTextOrNull(string id, CharacterObject character)
		{
			float num = -2.1474836E+09f;
			TextObject textObject = null;
			GameText gameText = Game.Current.GameTextManager.GetGameText(id);
			if (gameText != null)
			{
				foreach (GameText.GameTextVariation gameTextVariation in gameText.Variations)
				{
					float num2 = this.FindMatchingScore(character, gameTextVariation.Tags);
					if (num2 > num)
					{
						textObject = gameTextVariation.Text;
						num = num2;
					}
				}
			}
			return textObject;
		}

		// Token: 0x06001DA1 RID: 7585 RVA: 0x00086164 File Offset: 0x00084364
		private float FindMatchingScore(CharacterObject character, GameTextManager.ChoiceTag[] choiceTags)
		{
			float num = 0f;
			foreach (GameTextManager.ChoiceTag choiceTag in choiceTags)
			{
				if (choiceTag.TagName != "DefaultTag")
				{
					if (this.IsTagApplicable(choiceTag.TagName, character) == choiceTag.IsTagReversed)
					{
						return -2.1474836E+09f;
					}
					uint weight = choiceTag.Weight;
					num += weight;
				}
			}
			return num;
		}

		// Token: 0x06001DA2 RID: 7586 RVA: 0x000861D0 File Offset: 0x000843D0
		private void InitializeTags()
		{
			this._tags = new Dictionary<string, ConversationTag>();
			string name = typeof(ConversationTag).Assembly.GetName().Name;
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
				bool flag = false;
				if (name == assembly.GetName().Name)
				{
					flag = true;
				}
				else
				{
					AssemblyName[] array = referencedAssemblies;
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].Name == name)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					foreach (Type type in assembly.GetTypes())
					{
						if (type.IsSubclassOf(typeof(ConversationTag)))
						{
							ConversationTag conversationTag = Activator.CreateInstance(type) as ConversationTag;
							this._tags.Add(conversationTag.StringId, conversationTag);
						}
					}
				}
			}
		}

		// Token: 0x06001DA3 RID: 7587 RVA: 0x000862D4 File Offset: 0x000844D4
		private static void SetupTextVariables()
		{
			StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, null, false);
			int num = 1;
			foreach (CharacterObject characterObject in CharacterObject.ConversationCharacters)
			{
				string text = ((num == 1) ? "" : ("_" + num));
				StringHelpers.SetCharacterProperties("CONVERSATION_CHARACTER" + text, characterObject, null, false);
			}
			MBTextManager.SetTextVariable("CURRENT_SETTLEMENT_NAME", (Settlement.CurrentSettlement == null) ? TextObject.Empty : Settlement.CurrentSettlement.Name, false);
			ConversationHelper.ConversationTroopCommentShown = false;
		}

		// Token: 0x06001DA4 RID: 7588 RVA: 0x0008638C File Offset: 0x0008458C
		public IEnumerable<string> GetApplicableTagNames(CharacterObject character)
		{
			foreach (ConversationTag conversationTag in this._tags.Values)
			{
				if (conversationTag.IsApplicableTo(character))
				{
					yield return conversationTag.StringId;
				}
			}
			Dictionary<string, ConversationTag>.ValueCollection.Enumerator enumerator = default(Dictionary<string, ConversationTag>.ValueCollection.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06001DA5 RID: 7589 RVA: 0x000863A4 File Offset: 0x000845A4
		public bool IsTagApplicable(string tagId, CharacterObject character)
		{
			ConversationTag conversationTag;
			if (this._tags.TryGetValue(tagId, out conversationTag))
			{
				return conversationTag.IsApplicableTo(character);
			}
			Debug.FailedAssert("asking for a nonexistent tag", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Conversation\\ConversationManager.cs", "IsTagApplicable", 1443);
			return false;
		}

		// Token: 0x06001DA6 RID: 7590 RVA: 0x000863E4 File Offset: 0x000845E4
		public void OpenMapConversation(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData)
		{
			GameStateManager gameStateManager = GameStateManager.Current;
			(((gameStateManager != null) ? gameStateManager.ActiveState : null) as MapState).OnMapConversationStarts(playerCharacterData, conversationPartnerData);
			PartyBase party = conversationPartnerData.Party;
			this.SetupAndStartMapConversation((party != null) ? party.MobileParty : null, new MapConversationAgent(conversationPartnerData.Character), new MapConversationAgent(CharacterObject.PlayerCharacter));
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x0008643B File Offset: 0x0008463B
		public static void StartPersuasion(float goalValue, float successValue, float failValue, float criticalSuccessValue, float criticalFailValue, float initialProgress = -1f, PersuasionDifficulty difficulty = PersuasionDifficulty.Medium)
		{
			ConversationManager._persuasion = new Persuasion(goalValue, successValue, failValue, criticalSuccessValue, criticalFailValue, initialProgress, difficulty);
		}

		// Token: 0x06001DA8 RID: 7592 RVA: 0x00086451 File Offset: 0x00084651
		public static void EndPersuasion()
		{
			ConversationManager._persuasion = null;
		}

		// Token: 0x06001DA9 RID: 7593 RVA: 0x00086459 File Offset: 0x00084659
		public static void PersuasionCommitProgress(PersuasionOptionArgs persuasionOptionArgs)
		{
			ConversationManager._persuasion.CommitProgress(persuasionOptionArgs);
		}

		// Token: 0x06001DAA RID: 7594 RVA: 0x00086466 File Offset: 0x00084666
		public static void Clear()
		{
			ConversationManager._persuasion = null;
		}

		// Token: 0x06001DAB RID: 7595 RVA: 0x0008646E File Offset: 0x0008466E
		public void GetPersuasionChanceValues(out float successValue, out float critSuccessValue, out float critFailValue)
		{
			successValue = ConversationManager._persuasion.SuccessValue;
			critSuccessValue = ConversationManager._persuasion.CriticalSuccessValue;
			critFailValue = ConversationManager._persuasion.CriticalFailValue;
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x00086494 File Offset: 0x00084694
		public static bool GetPersuasionIsActive()
		{
			return ConversationManager._persuasion != null;
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x0008649E File Offset: 0x0008469E
		public static bool GetPersuasionProgressSatisfied()
		{
			return ConversationManager._persuasion.Progress >= ConversationManager._persuasion.GoalValue;
		}

		// Token: 0x06001DAE RID: 7598 RVA: 0x000864B9 File Offset: 0x000846B9
		public static bool GetPersuasionIsFailure()
		{
			return ConversationManager._persuasion.Progress < 0f;
		}

		// Token: 0x06001DAF RID: 7599 RVA: 0x000864CC File Offset: 0x000846CC
		public static float GetPersuasionProgress()
		{
			return ConversationManager._persuasion.Progress;
		}

		// Token: 0x06001DB0 RID: 7600 RVA: 0x000864D8 File Offset: 0x000846D8
		public static float GetPersuasionGoalValue()
		{
			return ConversationManager._persuasion.GoalValue;
		}

		// Token: 0x06001DB1 RID: 7601 RVA: 0x000864E4 File Offset: 0x000846E4
		public static IEnumerable<Tuple<PersuasionOptionArgs, PersuasionOptionResult>> GetPersuasionChosenOptions()
		{
			return ConversationManager._persuasion.GetChosenOptions();
		}

		// Token: 0x06001DB2 RID: 7602 RVA: 0x000864F0 File Offset: 0x000846F0
		public void GetPersuasionChances(ConversationSentenceOption conversationSentenceOption, out float successChance, out float critSuccessChance, out float critFailChance, out float failChance)
		{
			ConversationSentence conversationSentence = this._sentences[conversationSentenceOption.SentenceNo];
			if (conversationSentenceOption.HasPersuasion)
			{
				Campaign.Current.Models.PersuasionModel.GetChances(conversationSentence.PersuationOptionArgs, out successChance, out critSuccessChance, out critFailChance, out failChance, ConversationManager._persuasion.DifficultyMultiplier);
				return;
			}
			successChance = 0f;
			critSuccessChance = 0f;
			critFailChance = 0f;
			failChance = 0f;
		}

		// Token: 0x04000927 RID: 2343
		private int _currentRepeatedDialogSetIndex;

		// Token: 0x04000928 RID: 2344
		private int _currentRepeatIndex;

		// Token: 0x04000929 RID: 2345
		private int _autoId;

		// Token: 0x0400092A RID: 2346
		private int _autoToken;

		// Token: 0x0400092B RID: 2347
		private int _numConversationSentencesCreated;

		// Token: 0x0400092C RID: 2348
		private List<ConversationSentence> _sentences;

		// Token: 0x0400092D RID: 2349
		private int _numberOfStateIndices;

		// Token: 0x0400092E RID: 2350
		public int ActiveToken;

		// Token: 0x0400092F RID: 2351
		private int _currentSentence;

		// Token: 0x04000930 RID: 2352
		private TextObject _currentSentenceText;

		// Token: 0x04000931 RID: 2353
		public List<Tuple<string, CharacterObject>> DetailedDebugLog = new List<Tuple<string, CharacterObject>>();

		// Token: 0x04000932 RID: 2354
		public string CurrentFaceAnimationRecord;

		// Token: 0x04000933 RID: 2355
		private object _lastSelectedDialogObject;

		// Token: 0x04000934 RID: 2356
		private readonly List<List<object>> _dialogRepeatObjects = new List<List<object>>();

		// Token: 0x04000935 RID: 2357
		private readonly List<TextObject> _dialogRepeatLines = new List<TextObject>();

		// Token: 0x04000936 RID: 2358
		private bool _isActive;

		// Token: 0x04000937 RID: 2359
		private bool _executeDoOptionContinue;

		// Token: 0x04000938 RID: 2360
		public int LastSelectedButtonIndex;

		// Token: 0x04000939 RID: 2361
		public string LastSelectedDialog;

		// Token: 0x0400093A RID: 2362
		public ConversationAnimationManager ConversationAnimationManager;

		// Token: 0x0400093B RID: 2363
		private IAgent _mainAgent;

		// Token: 0x0400093C RID: 2364
		private IAgent _speakerAgent;

		// Token: 0x0400093D RID: 2365
		private IAgent _listenerAgent;

		// Token: 0x0400093E RID: 2366
		private Dictionary<string, ConversationTag> _tags;

		// Token: 0x0400093F RID: 2367
		private bool _sortSentenceIsDisabled;

		// Token: 0x04000940 RID: 2368
		private Dictionary<string, int> stateMap;

		// Token: 0x04000945 RID: 2373
		private List<IAgent> _conversationAgents = new List<IAgent>();

		// Token: 0x04000947 RID: 2375
		public bool CurrentConversationIsFirst;

		// Token: 0x04000948 RID: 2376
		private MobileParty _conversationParty;

		// Token: 0x04000950 RID: 2384
		private static Persuasion _persuasion;

		// Token: 0x02000567 RID: 1383
		public class TaggedString
		{
			// Token: 0x040016D0 RID: 5840
			public TextObject Text;

			// Token: 0x040016D1 RID: 5841
			public List<GameTextManager.ChoiceTag> ChoiceTags = new List<GameTextManager.ChoiceTag>();

			// Token: 0x040016D2 RID: 5842
			public int FacialAnimation;
		}
	}
}
