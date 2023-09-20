using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200016E RID: 366
	public abstract class CharacterDevelopmentModel : GameModel
	{
		// Token: 0x060018D9 RID: 6361
		public abstract List<Tuple<SkillObject, int>> GetSkillsDerivedFromTraits(Hero hero = null, CharacterObject templateCharacter = null, bool isByNaturalGrowth = false);

		// Token: 0x060018DA RID: 6362
		public abstract int SkillsRequiredForLevel(int level);

		// Token: 0x060018DB RID: 6363
		public abstract int GetMaxSkillPoint();

		// Token: 0x060018DC RID: 6364
		public abstract int GetXpRequiredForSkillLevel(int skillLevel);

		// Token: 0x060018DD RID: 6365
		public abstract int GetSkillLevelChange(Hero hero, SkillObject skill, float skillXp);

		// Token: 0x060018DE RID: 6366
		public abstract int GetXpAmountForSkillLevelChange(Hero hero, SkillObject skill, int skillLevelChange);

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x060018DF RID: 6367
		public abstract int MaxAttribute { get; }

		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x060018E0 RID: 6368
		public abstract int MaxFocusPerSkill { get; }

		// Token: 0x060018E1 RID: 6369
		public abstract void GetTraitLevelForTraitXp(Hero hero, TraitObject trait, int newValue, out int traitLevel, out int traitXp);

		// Token: 0x060018E2 RID: 6370
		public abstract int GetTraitXpRequiredForTraitLevel(TraitObject trait, int traitLevel);

		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x060018E3 RID: 6371
		public abstract int FocusPointsPerLevel { get; }

		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x060018E4 RID: 6372
		public abstract int FocusPointsAtStart { get; }

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x060018E5 RID: 6373
		public abstract int AttributePointsAtStart { get; }

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x060018E6 RID: 6374
		public abstract int LevelsPerAttributePoint { get; }

		// Token: 0x060018E7 RID: 6375
		public abstract ExplainedNumber CalculateLearningLimit(int attributeValue, int focusValue, TextObject attributeName, bool includeDescriptions = false);

		// Token: 0x060018E8 RID: 6376
		public abstract float CalculateLearningRate(Hero hero, SkillObject skill);

		// Token: 0x060018E9 RID: 6377
		public abstract ExplainedNumber CalculateLearningRate(int attributeValue, int focusValue, int skillValue, int characterLevel, TextObject attributeName, bool includeDescriptions = false);
	}
}
