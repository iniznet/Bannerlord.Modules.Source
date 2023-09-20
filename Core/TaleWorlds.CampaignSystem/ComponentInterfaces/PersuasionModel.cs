using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200017A RID: 378
	public abstract class PersuasionModel : GameModel
	{
		// Token: 0x06001929 RID: 6441
		public abstract int GetSkillXpFromPersuasion(PersuasionDifficulty difficulty, int argumentDifficultyBonusCoefficient);

		// Token: 0x0600192A RID: 6442
		public abstract void GetChances(PersuasionOptionArgs optionArgs, out float successChance, out float critSuccessChance, out float critFailChance, out float failChance, float difficultyMultiplier);

		// Token: 0x0600192B RID: 6443
		public abstract void GetEffectChances(PersuasionOptionArgs option, out float moveToNextStageChance, out float blockRandomOptionChance, float difficultyMultiplier);

		// Token: 0x0600192C RID: 6444
		public abstract PersuasionArgumentStrength GetArgumentStrengthBasedOnTargetTraits(CharacterObject character, Tuple<TraitObject, int>[] traitCorrelation);

		// Token: 0x0600192D RID: 6445
		public abstract float GetDifficulty(PersuasionDifficulty difficulty);

		// Token: 0x0600192E RID: 6446
		public abstract float CalculateInitialPersuasionProgress(CharacterObject character, float goalValue, float successValue);

		// Token: 0x0600192F RID: 6447
		public abstract float CalculatePersuasionGoalValue(CharacterObject oneToOneConversationCharacter, float successValue);
	}
}
