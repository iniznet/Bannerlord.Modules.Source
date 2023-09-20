using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Conversation.Persuasion
{
	// Token: 0x02000258 RID: 600
	public class Persuasion
	{
		// Token: 0x170007D6 RID: 2006
		// (get) Token: 0x06001F17 RID: 7959 RVA: 0x000882CB File Offset: 0x000864CB
		public float DifficultyMultiplier
		{
			get
			{
				return this._difficultyMultiplier;
			}
		}

		// Token: 0x170007D7 RID: 2007
		// (get) Token: 0x06001F18 RID: 7960 RVA: 0x000882D3 File Offset: 0x000864D3
		// (set) Token: 0x06001F19 RID: 7961 RVA: 0x000882DB File Offset: 0x000864DB
		public float Progress { get; private set; }

		// Token: 0x06001F1A RID: 7962 RVA: 0x000882E4 File Offset: 0x000864E4
		public Persuasion(float goalValue, float successValue, float failValue, float criticalSuccessValue, float criticalFailValue, float initialProgress, PersuasionDifficulty difficulty)
		{
			this._chosenOptions = new List<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>();
			this.GoalValue = Campaign.Current.Models.PersuasionModel.CalculatePersuasionGoalValue(CharacterObject.OneToOneConversationCharacter, goalValue);
			this.SuccessValue = successValue;
			this.FailValue = failValue;
			this.CriticalSuccessValue = criticalSuccessValue;
			this.CriticalFailValue = criticalFailValue;
			this._difficulty = difficulty;
			if (initialProgress < 0f)
			{
				this.Progress = Campaign.Current.Models.PersuasionModel.CalculateInitialPersuasionProgress(CharacterObject.OneToOneConversationCharacter, this.GoalValue, this.SuccessValue);
			}
			else
			{
				this.Progress = initialProgress;
			}
			this._difficultyMultiplier = Campaign.Current.Models.PersuasionModel.GetDifficulty(difficulty);
		}

		// Token: 0x06001F1B RID: 7963 RVA: 0x000883A4 File Offset: 0x000865A4
		public void CommitProgress(PersuasionOptionArgs persuasionOptionArgs)
		{
			PersuasionOptionResult persuasionOptionResult = this.GetResult(persuasionOptionArgs);
			persuasionOptionResult = this.CheckPerkEffectOnResult(persuasionOptionResult);
			Tuple<PersuasionOptionArgs, PersuasionOptionResult> tuple = new Tuple<PersuasionOptionArgs, PersuasionOptionResult>(persuasionOptionArgs, persuasionOptionResult);
			persuasionOptionArgs.BlockTheOption(true);
			this._chosenOptions.Add(tuple);
			this.Progress = MathF.Clamp(this.Progress + this.GetPersuasionOptionResultValue(persuasionOptionResult), 0f, this.GoalValue);
			CampaignEventDispatcher.Instance.OnPersuasionProgressCommitted(tuple);
		}

		// Token: 0x06001F1C RID: 7964 RVA: 0x0008840C File Offset: 0x0008660C
		private PersuasionOptionResult CheckPerkEffectOnResult(PersuasionOptionResult result)
		{
			PersuasionOptionResult persuasionOptionResult = result;
			if (result == PersuasionOptionResult.CriticalFailure && Hero.MainHero.GetPerkValue(DefaultPerks.Charm.ForgivableGrievances) && MBRandom.RandomFloat <= DefaultPerks.Charm.ForgivableGrievances.PrimaryBonus)
			{
				TextObject textObject = new TextObject("{=5IQriov5}You avoided critical failure because of {PERK_NAME}.", null);
				textObject.SetTextVariable("PERK_NAME", DefaultPerks.Charm.ForgivableGrievances.Name);
				InformationManager.DisplayMessage(new InformationMessage(textObject.ToString(), Color.White));
				persuasionOptionResult = PersuasionOptionResult.Failure;
			}
			return persuasionOptionResult;
		}

		// Token: 0x06001F1D RID: 7965 RVA: 0x00088478 File Offset: 0x00086678
		private float GetPersuasionOptionResultValue(PersuasionOptionResult result)
		{
			switch (result)
			{
			case PersuasionOptionResult.CriticalFailure:
				return -this.CriticalFailValue;
			case PersuasionOptionResult.Failure:
				return 0f;
			case PersuasionOptionResult.Success:
				return this.SuccessValue;
			case PersuasionOptionResult.CriticalSuccess:
				return this.CriticalSuccessValue;
			case PersuasionOptionResult.Miss:
				return 0f;
			default:
				return 0f;
			}
		}

		// Token: 0x06001F1E RID: 7966 RVA: 0x000884C8 File Offset: 0x000866C8
		private PersuasionOptionResult GetResult(PersuasionOptionArgs optionArgs)
		{
			float num;
			float num2;
			float num3;
			float num4;
			Campaign.Current.Models.PersuasionModel.GetChances(optionArgs, out num, out num2, out num3, out num4, this._difficultyMultiplier);
			float num5 = MBRandom.RandomFloat;
			if (num5 < num2)
			{
				return PersuasionOptionResult.CriticalSuccess;
			}
			num5 -= num2;
			if (num5 < num)
			{
				return PersuasionOptionResult.Success;
			}
			num5 -= num;
			if (num5 < num4)
			{
				return PersuasionOptionResult.Failure;
			}
			num5 -= num4;
			if (num5 < num3)
			{
				return PersuasionOptionResult.CriticalFailure;
			}
			return PersuasionOptionResult.Miss;
		}

		// Token: 0x06001F1F RID: 7967 RVA: 0x0008852E File Offset: 0x0008672E
		public IEnumerable<Tuple<PersuasionOptionArgs, PersuasionOptionResult>> GetChosenOptions()
		{
			return this._chosenOptions.AsReadOnly();
		}

		// Token: 0x040009FE RID: 2558
		public readonly float SuccessValue;

		// Token: 0x040009FF RID: 2559
		public readonly float FailValue;

		// Token: 0x04000A00 RID: 2560
		public readonly float CriticalSuccessValue;

		// Token: 0x04000A01 RID: 2561
		public readonly float CriticalFailValue;

		// Token: 0x04000A02 RID: 2562
		private readonly float _difficultyMultiplier;

		// Token: 0x04000A03 RID: 2563
		private readonly PersuasionDifficulty _difficulty;

		// Token: 0x04000A04 RID: 2564
		private readonly List<Tuple<PersuasionOptionArgs, PersuasionOptionResult>> _chosenOptions;

		// Token: 0x04000A05 RID: 2565
		public readonly float GoalValue;
	}
}
