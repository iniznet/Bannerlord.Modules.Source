using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Conversation.Persuasion
{
	public class PersuasionTask
	{
		public PersuasionTask(int reservationType)
		{
			this.Options = new MBList<PersuasionOptionArgs>();
			this.ReservationType = reservationType;
		}

		public void AddOptionToTask(PersuasionOptionArgs option)
		{
			this.Options.Add(option);
		}

		public void BlockAllOptions()
		{
			foreach (PersuasionOptionArgs persuasionOptionArgs in this.Options)
			{
				persuasionOptionArgs.BlockTheOption(true);
			}
		}

		public void UnblockAllOptions()
		{
			foreach (PersuasionOptionArgs persuasionOptionArgs in this.Options)
			{
				persuasionOptionArgs.BlockTheOption(false);
			}
		}

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

		public readonly MBList<PersuasionOptionArgs> Options;

		public TextObject SpokenLine;

		public TextObject ImmediateFailLine;

		public TextObject FinalFailLine;

		public TextObject TryLaterLine;

		public readonly int ReservationType;
	}
}
