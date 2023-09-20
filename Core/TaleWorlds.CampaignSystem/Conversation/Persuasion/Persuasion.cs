using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Conversation.Persuasion
{
	public class Persuasion
	{
		public float DifficultyMultiplier
		{
			get
			{
				return this._difficultyMultiplier;
			}
		}

		public float Progress { get; private set; }

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

		public IEnumerable<Tuple<PersuasionOptionArgs, PersuasionOptionResult>> GetChosenOptions()
		{
			return this._chosenOptions.AsReadOnly();
		}

		public readonly float SuccessValue;

		public readonly float FailValue;

		public readonly float CriticalSuccessValue;

		public readonly float CriticalFailValue;

		private readonly float _difficultyMultiplier;

		private readonly PersuasionDifficulty _difficulty;

		private readonly List<Tuple<PersuasionOptionArgs, PersuasionOptionResult>> _chosenOptions;

		public readonly float GoalValue;
	}
}
