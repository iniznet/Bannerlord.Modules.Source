using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Conversation
{
	public class ConversationSentence
	{
		public TextObject Text { get; private set; }

		public int Index { get; internal set; }

		public string Id { get; private set; }

		public bool IsPlayer
		{
			get
			{
				return this.GetFlags(ConversationSentence.DialogLineFlags.PlayerLine);
			}
			internal set
			{
				this.set_flags(value, ConversationSentence.DialogLineFlags.PlayerLine);
			}
		}

		public bool IsRepeatable
		{
			get
			{
				return this.GetFlags(ConversationSentence.DialogLineFlags.RepeatForObjects);
			}
			internal set
			{
				this.set_flags(value, ConversationSentence.DialogLineFlags.RepeatForObjects);
			}
		}

		public bool IsSpecial
		{
			get
			{
				return this.GetFlags(ConversationSentence.DialogLineFlags.SpecialLine);
			}
			internal set
			{
				this.set_flags(value, ConversationSentence.DialogLineFlags.SpecialLine);
			}
		}

		private bool GetFlags(ConversationSentence.DialogLineFlags flag)
		{
			return (this._flags & (uint)flag) > 0U;
		}

		private void set_flags(bool val, ConversationSentence.DialogLineFlags newFlag)
		{
			if (val)
			{
				this._flags |= (uint)newFlag;
				return;
			}
			this._flags &= (uint)(~(uint)newFlag);
		}

		public int Priority { get; private set; }

		public int InputToken { get; private set; }

		public int OutputToken { get; private set; }

		public object RelatedObject { get; private set; }

		public bool IsWithVariation { get; private set; }

		public PersuasionOptionArgs PersuationOptionArgs { get; private set; }

		public bool HasPersuasion
		{
			get
			{
				return this._onPersuasionOption != null;
			}
		}

		public string SkillName
		{
			get
			{
				if (!this.HasPersuasion)
				{
					return "";
				}
				return this.PersuationOptionArgs.SkillUsed.ToString();
			}
		}

		public string TraitName
		{
			get
			{
				if (!this.HasPersuasion)
				{
					return "";
				}
				if (this.PersuationOptionArgs.TraitUsed == null)
				{
					return "";
				}
				return this.PersuationOptionArgs.TraitUsed.ToString();
			}
		}

		internal ConversationSentence(string idString, TextObject text, string inputToken, string outputToken, ConversationSentence.OnConditionDelegate conditionDelegate, ConversationSentence.OnClickableConditionDelegate clickableConditionDelegate, ConversationSentence.OnConsequenceDelegate consequenceDelegate, uint flags = 0U, int priority = 100, int agentIndex = 0, int nextAgentIndex = 0, object relatedObject = null, bool withVariation = false, ConversationSentence.OnMultipleConversationConsequenceDelegate speakerDelegate = null, ConversationSentence.OnMultipleConversationConsequenceDelegate listenerDelegate = null, ConversationSentence.OnPersuasionOptionDelegate persuasionOptionDelegate = null)
		{
			this.Index = Campaign.Current.ConversationManager.CreateConversationSentenceIndex();
			this.Id = idString;
			this.Text = text;
			this.InputToken = Campaign.Current.ConversationManager.GetStateIndex(inputToken);
			this.OutputToken = Campaign.Current.ConversationManager.GetStateIndex(outputToken);
			this.OnCondition = conditionDelegate;
			this.OnClickableCondition = clickableConditionDelegate;
			this.OnConsequence = consequenceDelegate;
			this._flags = flags;
			this.Priority = priority;
			this.AgentIndex = agentIndex;
			this.NextAgentIndex = nextAgentIndex;
			this.RelatedObject = relatedObject;
			this.IsWithVariation = withVariation;
			this.IsSpeaker = speakerDelegate;
			this.IsListener = listenerDelegate;
			this._onPersuasionOption = persuasionOptionDelegate;
		}

		internal ConversationSentence(int index)
		{
			this.Index = index;
		}

		public ConversationSentence Variation(params object[] list)
		{
			Game.Current.GameTextManager.AddGameText(this.Id).AddVariation((string)list[0], list.Skip(1).ToArray<object>());
			return this;
		}

		internal void RunConsequence(Game game)
		{
			if (this.OnConsequence != null)
			{
				this.OnConsequence();
			}
			Campaign.Current.ConversationManager.OnConsequence(this);
			if (this.HasPersuasion)
			{
				ConversationManager.PersuasionCommitProgress(this.PersuationOptionArgs);
			}
		}

		internal bool RunCondition()
		{
			bool flag = true;
			if (this.OnCondition != null)
			{
				flag = this.OnCondition();
			}
			if (flag && this.HasPersuasion)
			{
				this.PersuationOptionArgs = this._onPersuasionOption();
			}
			Campaign.Current.ConversationManager.OnCondition(this);
			return flag;
		}

		internal bool RunClickableCondition()
		{
			bool flag = true;
			if (this.OnClickableCondition != null)
			{
				flag = this.OnClickableCondition(out this.HintText);
			}
			Campaign.Current.ConversationManager.OnClickableCondition(this);
			return flag;
		}

		public void Deserialize(XmlNode node, Type typeOfConversationCallbacks, ConversationManager conversationManager, int defaultPriority)
		{
			if (node.Attributes == null)
			{
				throw new TWXmlLoadException("node.Attributes != null");
			}
			this.Id = node.Attributes["id"].Value;
			XmlNode xmlNode = node.Attributes["on_condition"];
			if (xmlNode != null)
			{
				string innerText = xmlNode.InnerText;
				this._methodOnCondition = typeOfConversationCallbacks.GetMethod(innerText);
				if (this._methodOnCondition == null)
				{
					throw new MBMethodNameNotFoundException(innerText);
				}
				this.OnCondition = Delegate.CreateDelegate(typeof(ConversationSentence.OnConditionDelegate), null, this._methodOnCondition) as ConversationSentence.OnConditionDelegate;
			}
			XmlNode xmlNode2 = node.Attributes["on_clickable_condition"];
			if (xmlNode2 != null)
			{
				string innerText2 = xmlNode2.InnerText;
				this._methodOnClickableCondition = typeOfConversationCallbacks.GetMethod(innerText2);
				if (this._methodOnClickableCondition == null)
				{
					throw new MBMethodNameNotFoundException(innerText2);
				}
				this.OnClickableCondition = Delegate.CreateDelegate(typeof(ConversationSentence.OnClickableConditionDelegate), null, this._methodOnClickableCondition) as ConversationSentence.OnClickableConditionDelegate;
			}
			XmlNode xmlNode3 = node.Attributes["on_consequence"];
			if (xmlNode3 != null)
			{
				string innerText3 = xmlNode3.InnerText;
				this._methodOnConsequence = typeOfConversationCallbacks.GetMethod(innerText3);
				if (this._methodOnConsequence == null)
				{
					throw new MBMethodNameNotFoundException(innerText3);
				}
				this.OnConsequence = Delegate.CreateDelegate(typeof(ConversationSentence.OnConsequenceDelegate), null, this._methodOnConsequence) as ConversationSentence.OnConsequenceDelegate;
			}
			XmlNode xmlNode4 = node.Attributes["is_player"];
			if (xmlNode4 != null)
			{
				string innerText4 = xmlNode4.InnerText;
				this.IsPlayer = Convert.ToBoolean(innerText4);
			}
			XmlNode xmlNode5 = node.Attributes["is_repeatable"];
			if (xmlNode5 != null)
			{
				string innerText5 = xmlNode5.InnerText;
				this.IsRepeatable = Convert.ToBoolean(innerText5);
			}
			XmlNode xmlNode6 = node.Attributes["is_speacial_option"];
			if (xmlNode6 != null)
			{
				string innerText6 = xmlNode6.InnerText;
				this.IsSpecial = Convert.ToBoolean(innerText6);
			}
			XmlNode xmlNode7 = node.Attributes["text"];
			if (xmlNode7 != null)
			{
				this.Text = new TextObject(xmlNode7.InnerText, null);
			}
			XmlNode xmlNode8 = node.Attributes["istate"];
			if (xmlNode8 != null)
			{
				this.InputToken = conversationManager.GetStateIndex(xmlNode8.InnerText);
			}
			XmlNode xmlNode9 = node.Attributes["ostate"];
			if (xmlNode9 != null)
			{
				this.OutputToken = conversationManager.GetStateIndex(xmlNode9.InnerText);
			}
			XmlNode xmlNode10 = node.Attributes["priority"];
			this.Priority = ((xmlNode10 != null) ? int.Parse(xmlNode10.InnerText) : defaultPriority);
		}

		public static object CurrentProcessedRepeatObject
		{
			get
			{
				return Campaign.Current.ConversationManager.GetCurrentProcessedRepeatObject();
			}
		}

		public static object SelectedRepeatObject
		{
			get
			{
				return Campaign.Current.ConversationManager.GetSelectedRepeatObject();
			}
		}

		public static TextObject SelectedRepeatLine
		{
			get
			{
				return Campaign.Current.ConversationManager.GetCurrentDialogLine();
			}
		}

		public static void SetObjectsToRepeatOver(IReadOnlyList<object> objectsToRepeatOver, int maxRepeatedDialogsInConversation = 5)
		{
			Campaign.Current.ConversationManager.SetDialogRepeatCount(objectsToRepeatOver, maxRepeatedDialogsInConversation);
		}

		public const int DefaultPriority = 100;

		public int AgentIndex;

		public int NextAgentIndex;

		public bool IsClickable = true;

		public TextObject HintText;

		private MethodInfo _methodOnCondition;

		public ConversationSentence.OnConditionDelegate OnCondition;

		private MethodInfo _methodOnClickableCondition;

		public ConversationSentence.OnClickableConditionDelegate OnClickableCondition;

		private MethodInfo _methodOnConsequence;

		public ConversationSentence.OnConsequenceDelegate OnConsequence;

		public ConversationSentence.OnMultipleConversationConsequenceDelegate IsSpeaker;

		public ConversationSentence.OnMultipleConversationConsequenceDelegate IsListener;

		private uint _flags;

		private ConversationSentence.OnPersuasionOptionDelegate _onPersuasionOption;

		public enum DialogLineFlags
		{
			PlayerLine = 1,
			RepeatForObjects,
			SpecialLine = 4
		}

		public delegate bool OnConditionDelegate();

		public delegate bool OnClickableConditionDelegate(out TextObject explanation);

		public delegate PersuasionOptionArgs OnPersuasionOptionDelegate();

		public delegate void OnConsequenceDelegate();

		public delegate bool OnMultipleConversationConsequenceDelegate(IAgent agent);
	}
}
