using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Conversation.Persuasion
{
	// Token: 0x02000259 RID: 601
	public class PersuasionTask
	{
		// Token: 0x06001F20 RID: 7968 RVA: 0x0008853B File Offset: 0x0008673B
		public PersuasionTask(int reservationType)
		{
			this.Options = new MBList<PersuasionOptionArgs>();
			this.ReservationType = reservationType;
		}

		// Token: 0x06001F21 RID: 7969 RVA: 0x00088555 File Offset: 0x00086755
		public void AddOptionToTask(PersuasionOptionArgs option)
		{
			this.Options.Add(option);
		}

		// Token: 0x06001F22 RID: 7970 RVA: 0x00088564 File Offset: 0x00086764
		public void BlockAllOptions()
		{
			foreach (PersuasionOptionArgs persuasionOptionArgs in this.Options)
			{
				persuasionOptionArgs.BlockTheOption(true);
			}
		}

		// Token: 0x06001F23 RID: 7971 RVA: 0x000885B8 File Offset: 0x000867B8
		public void UnblockAllOptions()
		{
			foreach (PersuasionOptionArgs persuasionOptionArgs in this.Options)
			{
				persuasionOptionArgs.BlockTheOption(false);
			}
		}

		// Token: 0x06001F24 RID: 7972 RVA: 0x0008860C File Offset: 0x0008680C
		public void ApplyEffects(float moveToNextStageChance, float blockRandomOptionChance)
		{
			if (moveToNextStageChance > MBRandom.RandomFloat)
			{
				this.BlockAllOptions();
				return;
			}
			if (blockRandomOptionChance > MBRandom.RandomFloat)
			{
				PersuasionOptionArgs randomElementWithPredicate = this.Options.GetRandomElementWithPredicate((PersuasionOptionArgs x) => !x.IsBlocked);
				if (randomElementWithPredicate == null)
				{
					return;
				}
				randomElementWithPredicate.BlockTheOption(true);
			}
		}

		// Token: 0x04000A07 RID: 2567
		public readonly MBList<PersuasionOptionArgs> Options;

		// Token: 0x04000A08 RID: 2568
		public TextObject SpokenLine;

		// Token: 0x04000A09 RID: 2569
		public TextObject ImmediateFailLine;

		// Token: 0x04000A0A RID: 2570
		public TextObject FinalFailLine;

		// Token: 0x04000A0B RID: 2571
		public TextObject TryLaterLine;

		// Token: 0x04000A0C RID: 2572
		public readonly int ReservationType;
	}
}
