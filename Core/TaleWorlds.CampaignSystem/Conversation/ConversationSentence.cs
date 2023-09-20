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
	// Token: 0x020001F2 RID: 498
	public class ConversationSentence
	{
		// Token: 0x17000766 RID: 1894
		// (get) Token: 0x06001DB9 RID: 7609 RVA: 0x00086560 File Offset: 0x00084760
		// (set) Token: 0x06001DBA RID: 7610 RVA: 0x00086568 File Offset: 0x00084768
		public TextObject Text { get; private set; }

		// Token: 0x17000767 RID: 1895
		// (get) Token: 0x06001DBB RID: 7611 RVA: 0x00086571 File Offset: 0x00084771
		// (set) Token: 0x06001DBC RID: 7612 RVA: 0x00086579 File Offset: 0x00084779
		public int Index { get; internal set; }

		// Token: 0x17000768 RID: 1896
		// (get) Token: 0x06001DBD RID: 7613 RVA: 0x00086582 File Offset: 0x00084782
		// (set) Token: 0x06001DBE RID: 7614 RVA: 0x0008658A File Offset: 0x0008478A
		public string Id { get; private set; }

		// Token: 0x17000769 RID: 1897
		// (get) Token: 0x06001DBF RID: 7615 RVA: 0x00086593 File Offset: 0x00084793
		// (set) Token: 0x06001DC0 RID: 7616 RVA: 0x0008659C File Offset: 0x0008479C
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

		// Token: 0x1700076A RID: 1898
		// (get) Token: 0x06001DC1 RID: 7617 RVA: 0x000865A6 File Offset: 0x000847A6
		// (set) Token: 0x06001DC2 RID: 7618 RVA: 0x000865AF File Offset: 0x000847AF
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

		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x06001DC3 RID: 7619 RVA: 0x000865B9 File Offset: 0x000847B9
		// (set) Token: 0x06001DC4 RID: 7620 RVA: 0x000865C2 File Offset: 0x000847C2
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

		// Token: 0x06001DC5 RID: 7621 RVA: 0x000865CC File Offset: 0x000847CC
		private bool GetFlags(ConversationSentence.DialogLineFlags flag)
		{
			return (this._flags & (uint)flag) > 0U;
		}

		// Token: 0x06001DC6 RID: 7622 RVA: 0x000865D9 File Offset: 0x000847D9
		private void set_flags(bool val, ConversationSentence.DialogLineFlags newFlag)
		{
			if (val)
			{
				this._flags |= (uint)newFlag;
				return;
			}
			this._flags &= (uint)(~(uint)newFlag);
		}

		// Token: 0x1700076C RID: 1900
		// (get) Token: 0x06001DC7 RID: 7623 RVA: 0x000865FC File Offset: 0x000847FC
		// (set) Token: 0x06001DC8 RID: 7624 RVA: 0x00086604 File Offset: 0x00084804
		public int Priority { get; private set; }

		// Token: 0x1700076D RID: 1901
		// (get) Token: 0x06001DC9 RID: 7625 RVA: 0x0008660D File Offset: 0x0008480D
		// (set) Token: 0x06001DCA RID: 7626 RVA: 0x00086615 File Offset: 0x00084815
		public int InputToken { get; private set; }

		// Token: 0x1700076E RID: 1902
		// (get) Token: 0x06001DCB RID: 7627 RVA: 0x0008661E File Offset: 0x0008481E
		// (set) Token: 0x06001DCC RID: 7628 RVA: 0x00086626 File Offset: 0x00084826
		public int OutputToken { get; private set; }

		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x06001DCD RID: 7629 RVA: 0x0008662F File Offset: 0x0008482F
		// (set) Token: 0x06001DCE RID: 7630 RVA: 0x00086637 File Offset: 0x00084837
		public object RelatedObject { get; private set; }

		// Token: 0x17000770 RID: 1904
		// (get) Token: 0x06001DD0 RID: 7632 RVA: 0x00086649 File Offset: 0x00084849
		// (set) Token: 0x06001DCF RID: 7631 RVA: 0x00086640 File Offset: 0x00084840
		public bool IsWithVariation { get; private set; }

		// Token: 0x17000771 RID: 1905
		// (get) Token: 0x06001DD1 RID: 7633 RVA: 0x00086651 File Offset: 0x00084851
		// (set) Token: 0x06001DD2 RID: 7634 RVA: 0x00086659 File Offset: 0x00084859
		public PersuasionOptionArgs PersuationOptionArgs { get; private set; }

		// Token: 0x17000772 RID: 1906
		// (get) Token: 0x06001DD3 RID: 7635 RVA: 0x00086662 File Offset: 0x00084862
		public bool HasPersuasion
		{
			get
			{
				return this._onPersuasionOption != null;
			}
		}

		// Token: 0x17000773 RID: 1907
		// (get) Token: 0x06001DD4 RID: 7636 RVA: 0x0008666D File Offset: 0x0008486D
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

		// Token: 0x17000774 RID: 1908
		// (get) Token: 0x06001DD5 RID: 7637 RVA: 0x0008668D File Offset: 0x0008488D
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

		// Token: 0x06001DD6 RID: 7638 RVA: 0x000866C0 File Offset: 0x000848C0
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

		// Token: 0x06001DD7 RID: 7639 RVA: 0x0008678A File Offset: 0x0008498A
		internal ConversationSentence(int index)
		{
			this.Index = index;
		}

		// Token: 0x06001DD8 RID: 7640 RVA: 0x000867A0 File Offset: 0x000849A0
		public ConversationSentence Variation(params object[] list)
		{
			Game.Current.GameTextManager.AddGameText(this.Id).AddVariation((string)list[0], list.Skip(1).ToArray<object>());
			return this;
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x000867D1 File Offset: 0x000849D1
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

		// Token: 0x06001DDA RID: 7642 RVA: 0x0008680C File Offset: 0x00084A0C
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

		// Token: 0x06001DDB RID: 7643 RVA: 0x0008685C File Offset: 0x00084A5C
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

		// Token: 0x06001DDC RID: 7644 RVA: 0x00086898 File Offset: 0x00084A98
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

		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x06001DDD RID: 7645 RVA: 0x00086B1E File Offset: 0x00084D1E
		public static object CurrentProcessedRepeatObject
		{
			get
			{
				return Campaign.Current.ConversationManager.GetCurrentProcessedRepeatObject();
			}
		}

		// Token: 0x17000776 RID: 1910
		// (get) Token: 0x06001DDE RID: 7646 RVA: 0x00086B2F File Offset: 0x00084D2F
		public static object SelectedRepeatObject
		{
			get
			{
				return Campaign.Current.ConversationManager.GetSelectedRepeatObject();
			}
		}

		// Token: 0x17000777 RID: 1911
		// (get) Token: 0x06001DDF RID: 7647 RVA: 0x00086B40 File Offset: 0x00084D40
		public static TextObject SelectedRepeatLine
		{
			get
			{
				return Campaign.Current.ConversationManager.GetCurrentDialogLine();
			}
		}

		// Token: 0x06001DE0 RID: 7648 RVA: 0x00086B51 File Offset: 0x00084D51
		public static void SetObjectsToRepeatOver(IReadOnlyList<object> objectsToRepeatOver, int maxRepeatedDialogsInConversation = 5)
		{
			Campaign.Current.ConversationManager.SetDialogRepeatCount(objectsToRepeatOver, maxRepeatedDialogsInConversation);
		}

		// Token: 0x0400095D RID: 2397
		public const int DefaultPriority = 100;

		// Token: 0x04000961 RID: 2401
		public int AgentIndex;

		// Token: 0x04000962 RID: 2402
		public int NextAgentIndex;

		// Token: 0x04000963 RID: 2403
		public bool IsClickable = true;

		// Token: 0x04000964 RID: 2404
		public TextObject HintText;

		// Token: 0x04000969 RID: 2409
		private MethodInfo _methodOnCondition;

		// Token: 0x0400096A RID: 2410
		public ConversationSentence.OnConditionDelegate OnCondition;

		// Token: 0x0400096B RID: 2411
		private MethodInfo _methodOnClickableCondition;

		// Token: 0x0400096C RID: 2412
		public ConversationSentence.OnClickableConditionDelegate OnClickableCondition;

		// Token: 0x0400096D RID: 2413
		private MethodInfo _methodOnConsequence;

		// Token: 0x0400096E RID: 2414
		public ConversationSentence.OnConsequenceDelegate OnConsequence;

		// Token: 0x0400096F RID: 2415
		public ConversationSentence.OnMultipleConversationConsequenceDelegate IsSpeaker;

		// Token: 0x04000970 RID: 2416
		public ConversationSentence.OnMultipleConversationConsequenceDelegate IsListener;

		// Token: 0x04000971 RID: 2417
		private uint _flags;

		// Token: 0x04000973 RID: 2419
		private ConversationSentence.OnPersuasionOptionDelegate _onPersuasionOption;

		// Token: 0x0200056C RID: 1388
		public enum DialogLineFlags
		{
			// Token: 0x040016E3 RID: 5859
			PlayerLine = 1,
			// Token: 0x040016E4 RID: 5860
			RepeatForObjects,
			// Token: 0x040016E5 RID: 5861
			SpecialLine = 4
		}

		// Token: 0x0200056D RID: 1389
		// (Invoke) Token: 0x060043BC RID: 17340
		public delegate bool OnConditionDelegate();

		// Token: 0x0200056E RID: 1390
		// (Invoke) Token: 0x060043C0 RID: 17344
		public delegate bool OnClickableConditionDelegate(out TextObject explanation);

		// Token: 0x0200056F RID: 1391
		// (Invoke) Token: 0x060043C4 RID: 17348
		public delegate PersuasionOptionArgs OnPersuasionOptionDelegate();

		// Token: 0x02000570 RID: 1392
		// (Invoke) Token: 0x060043C8 RID: 17352
		public delegate void OnConsequenceDelegate();

		// Token: 0x02000571 RID: 1393
		// (Invoke) Token: 0x060043CC RID: 17356
		public delegate bool OnMultipleConversationConsequenceDelegate(IAgent agent);
	}
}
