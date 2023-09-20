using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000079 RID: 121
	internal class DialogFlowLine
	{
		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06000F23 RID: 3875 RVA: 0x00046B81 File Offset: 0x00044D81
		// (set) Token: 0x06000F22 RID: 3874 RVA: 0x00046B78 File Offset: 0x00044D78
		public List<KeyValuePair<TextObject, List<GameTextManager.ChoiceTag>>> Variations { get; private set; }

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06000F24 RID: 3876 RVA: 0x00046B89 File Offset: 0x00044D89
		public bool HasVariation
		{
			get
			{
				return this.Variations.Count > 0;
			}
		}

		// Token: 0x06000F25 RID: 3877 RVA: 0x00046B99 File Offset: 0x00044D99
		internal DialogFlowLine()
		{
			this.Variations = new List<KeyValuePair<TextObject, List<GameTextManager.ChoiceTag>>>();
		}

		// Token: 0x06000F26 RID: 3878 RVA: 0x00046BAC File Offset: 0x00044DAC
		public void AddVariation(TextObject text, List<GameTextManager.ChoiceTag> list)
		{
			this.Variations.Add(new KeyValuePair<TextObject, List<GameTextManager.ChoiceTag>>(text, list));
		}

		// Token: 0x04000524 RID: 1316
		internal TextObject Text;

		// Token: 0x04000525 RID: 1317
		internal string InputToken;

		// Token: 0x04000526 RID: 1318
		internal string OutputToken;

		// Token: 0x04000527 RID: 1319
		internal bool ByPlayer;

		// Token: 0x04000528 RID: 1320
		internal ConversationSentence.OnConditionDelegate ConditionDelegate;

		// Token: 0x04000529 RID: 1321
		internal ConversationSentence.OnClickableConditionDelegate ClickableConditionDelegate;

		// Token: 0x0400052A RID: 1322
		internal ConversationSentence.OnConsequenceDelegate ConsequenceDelegate;

		// Token: 0x0400052B RID: 1323
		internal ConversationSentence.OnMultipleConversationConsequenceDelegate SpeakerDelegate;

		// Token: 0x0400052C RID: 1324
		internal ConversationSentence.OnMultipleConversationConsequenceDelegate ListenerDelegate;

		// Token: 0x0400052D RID: 1325
		internal bool IsRepeatable;

		// Token: 0x0400052E RID: 1326
		internal bool IsSpecialOption;
	}
}
