using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	// Token: 0x020001D6 RID: 470
	public class CharacterCreationMenu
	{
		// Token: 0x17000721 RID: 1825
		// (get) Token: 0x06001BB5 RID: 7093 RVA: 0x0007D7D9 File Offset: 0x0007B9D9
		public MBReadOnlyList<CharacterCreationCategory> CharacterCreationCategories
		{
			get
			{
				return this._characterCreationCategories;
			}
		}

		// Token: 0x06001BB6 RID: 7094 RVA: 0x0007D7E4 File Offset: 0x0007B9E4
		public CharacterCreationCategory AddMenuCategory(CharacterCreationOnCondition condition = null)
		{
			CharacterCreationCategory characterCreationCategory = new CharacterCreationCategory(condition);
			this._characterCreationCategories.Add(characterCreationCategory);
			return characterCreationCategory;
		}

		// Token: 0x06001BB7 RID: 7095 RVA: 0x0007D805 File Offset: 0x0007BA05
		public CharacterCreationMenu(TextObject title, TextObject text, CharacterCreationOnInit onInit, CharacterCreationMenu.MenuTypes menuType = CharacterCreationMenu.MenuTypes.MultipleChoice)
		{
			this.Title = title;
			this.Text = text;
			this.OnInit = onInit;
			this.SelectedOptions = new List<int>();
			this._characterCreationCategories = new MBList<CharacterCreationCategory>();
			this.MenuType = menuType;
		}

		// Token: 0x06001BB8 RID: 7096 RVA: 0x0007D840 File Offset: 0x0007BA40
		public void ApplyFinalEffect(CharacterCreation characterCreation)
		{
			using (List<int>.Enumerator enumerator = this.SelectedOptions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int selectedOption = enumerator.Current;
					Predicate<CharacterCreationOption> <>9__0;
					foreach (CharacterCreationCategory characterCreationCategory in this.CharacterCreationCategories)
					{
						if (characterCreationCategory.CategoryCondition == null || characterCreationCategory.CategoryCondition())
						{
							List<CharacterCreationOption> characterCreationOptions = characterCreationCategory.CharacterCreationOptions;
							Predicate<CharacterCreationOption> predicate;
							if ((predicate = <>9__0) == null)
							{
								predicate = (<>9__0 = (CharacterCreationOption o) => o.Id == selectedOption);
							}
							CharacterCreationOption characterCreationOption = characterCreationOptions.Find(predicate);
							if (characterCreationOption.ApplyFinalEffects != null)
							{
								MBReadOnlyList<SkillObject> affectedSkills = characterCreationOption.AffectedSkills;
								List<SkillObject> list = ((affectedSkills != null) ? affectedSkills.ToList<SkillObject>() : null);
								MBReadOnlyList<TraitObject> affectedTraits = characterCreationOption.AffectedTraits;
								List<TraitObject> list2 = ((affectedTraits != null) ? affectedTraits.ToList<TraitObject>() : null);
								CharacterCreationContentBase.Instance.ApplySkillAndAttributeEffects(list, characterCreationOption.FocusToAdd, characterCreationOption.SkillLevelToAdd, characterCreationOption.EffectedAttribute, characterCreationOption.AttributeLevelToAdd, list2, characterCreationOption.TraitLevelToAdd, characterCreationOption.RenownToAdd, characterCreationOption.GoldToAdd, characterCreationOption.UnspentFocusToAdd, characterCreationOption.UnspentAttributeToAdd);
								characterCreationOption.ApplyFinalEffects(characterCreation);
							}
						}
					}
				}
			}
		}

		// Token: 0x040008C4 RID: 2244
		public readonly CharacterCreationMenu.MenuTypes MenuType;

		// Token: 0x040008C5 RID: 2245
		public readonly TextObject Title;

		// Token: 0x040008C6 RID: 2246
		public readonly TextObject Text;

		// Token: 0x040008C7 RID: 2247
		public readonly CharacterCreationOnInit OnInit;

		// Token: 0x040008C8 RID: 2248
		private readonly MBList<CharacterCreationCategory> _characterCreationCategories;

		// Token: 0x040008C9 RID: 2249
		public readonly List<int> SelectedOptions;

		// Token: 0x0200055E RID: 1374
		public enum MenuTypes
		{
			// Token: 0x040016B1 RID: 5809
			MultipleChoice,
			// Token: 0x040016B2 RID: 5810
			SelectAllThatApply
		}
	}
}
