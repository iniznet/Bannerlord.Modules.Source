using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	// Token: 0x020001D5 RID: 469
	public class CharacterCreationCategory
	{
		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x06001BAF RID: 7087 RVA: 0x0007D754 File Offset: 0x0007B954
		// (set) Token: 0x06001BB0 RID: 7088 RVA: 0x0007D75C File Offset: 0x0007B95C
		public CharacterCreationOnCondition CategoryCondition { get; private set; }

		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x06001BB1 RID: 7089 RVA: 0x0007D765 File Offset: 0x0007B965
		// (set) Token: 0x06001BB2 RID: 7090 RVA: 0x0007D76D File Offset: 0x0007B96D
		public List<CharacterCreationOption> CharacterCreationOptions { get; private set; }

		// Token: 0x06001BB3 RID: 7091 RVA: 0x0007D776 File Offset: 0x0007B976
		public CharacterCreationCategory(CharacterCreationOnCondition condition = null)
		{
			this.CategoryCondition = condition;
			this.CharacterCreationOptions = new List<CharacterCreationOption>();
		}

		// Token: 0x06001BB4 RID: 7092 RVA: 0x0007D790 File Offset: 0x0007B990
		public void AddCategoryOption(TextObject text, MBList<SkillObject> effectedSkills, CharacterAttribute effectedAttribute, int focusToAdd, int skillLevelToAdd, int attributeLevelToAdd, CharacterCreationOnCondition optionCondition, CharacterCreationOnSelect onSelect, CharacterCreationApplyFinalEffects onApply, TextObject descriptionText = null, MBList<TraitObject> effectedTraits = null, int traitLevelToAdd = 0, int renownToAdd = 0, int goldToAdd = 0, int unspentFocusPoint = 0, int unspentAttributePoint = 0)
		{
			CharacterCreationOption characterCreationOption = new CharacterCreationOption(this.CharacterCreationOptions.Count + 1, effectedSkills, effectedAttribute, focusToAdd, skillLevelToAdd, attributeLevelToAdd, text, optionCondition, onSelect, onApply, descriptionText, effectedTraits, traitLevelToAdd, renownToAdd, goldToAdd, unspentFocusPoint, unspentAttributePoint);
			this.CharacterCreationOptions.Add(characterCreationOption);
		}
	}
}
