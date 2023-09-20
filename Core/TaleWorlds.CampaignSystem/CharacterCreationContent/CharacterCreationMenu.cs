using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	public class CharacterCreationMenu
	{
		public MBReadOnlyList<CharacterCreationCategory> CharacterCreationCategories
		{
			get
			{
				return this._characterCreationCategories;
			}
		}

		public CharacterCreationCategory AddMenuCategory(CharacterCreationOnCondition condition = null)
		{
			CharacterCreationCategory characterCreationCategory = new CharacterCreationCategory(condition);
			this._characterCreationCategories.Add(characterCreationCategory);
			return characterCreationCategory;
		}

		public CharacterCreationMenu(TextObject title, TextObject text, CharacterCreationOnInit onInit, CharacterCreationMenu.MenuTypes menuType = CharacterCreationMenu.MenuTypes.MultipleChoice)
		{
			this.Title = title;
			this.Text = text;
			this.OnInit = onInit;
			this.SelectedOptions = new List<int>();
			this._characterCreationCategories = new MBList<CharacterCreationCategory>();
			this.MenuType = menuType;
		}

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

		public readonly CharacterCreationMenu.MenuTypes MenuType;

		public readonly TextObject Title;

		public readonly TextObject Text;

		public readonly CharacterCreationOnInit OnInit;

		private readonly MBList<CharacterCreationCategory> _characterCreationCategories;

		public readonly List<int> SelectedOptions;

		public enum MenuTypes
		{
			MultipleChoice,
			SelectAllThatApply
		}
	}
}
