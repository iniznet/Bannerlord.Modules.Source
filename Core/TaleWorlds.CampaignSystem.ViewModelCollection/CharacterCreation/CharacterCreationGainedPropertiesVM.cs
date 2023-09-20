using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	public class CharacterCreationGainedPropertiesVM : ViewModel
	{
		public CharacterCreationGainedPropertiesVM(CharacterCreation characterCreation, int currentIndex)
		{
			this._characterCreation = characterCreation;
			this._currentIndex = currentIndex;
			this._affectedAttributesMap = new Dictionary<CharacterAttribute, Tuple<int, int>>();
			this._affectedSkillMap = new Dictionary<SkillObject, Tuple<int, int>>();
			this.GainGroups = new MBBindingList<CharacterCreationGainGroupItemVM>();
			foreach (CharacterAttribute characterAttribute in Attributes.All)
			{
				this.GainGroups.Add(new CharacterCreationGainGroupItemVM(characterAttribute, this._characterCreation, this._currentIndex));
			}
			this.GainedTraits = new MBBindingList<EncyclopediaTraitItemVM>();
			this.UpdateValues();
		}

		public void UpdateValues()
		{
			this._affectedAttributesMap.Clear();
			this._affectedSkillMap.Clear();
			this.GainGroups.ApplyActionOnAllItems(delegate(CharacterCreationGainGroupItemVM g)
			{
				g.ResetValues();
			});
			this.PopulateInitialValues();
			this.PopulateGainedAttributeValues();
			this.PopulateGainedTraitValues();
			foreach (KeyValuePair<CharacterAttribute, Tuple<int, int>> keyValuePair in this._affectedAttributesMap)
			{
				this.GetItemFromAttribute(keyValuePair.Key).SetValue(keyValuePair.Value.Item1, keyValuePair.Value.Item2);
			}
			foreach (KeyValuePair<SkillObject, Tuple<int, int>> keyValuePair2 in this._affectedSkillMap)
			{
				this.GetItemFromSkill(keyValuePair2.Key).SetValue(keyValuePair2.Value.Item1, keyValuePair2.Value.Item2);
			}
		}

		private void PopulateInitialValues()
		{
			foreach (SkillObject skillObject in Skills.All)
			{
				int focus = Hero.MainHero.HeroDeveloper.GetFocus(skillObject);
				if (this._affectedSkillMap.ContainsKey(skillObject))
				{
					Tuple<int, int> tuple = this._affectedSkillMap[skillObject];
					this._affectedSkillMap[skillObject] = new Tuple<int, int>(tuple.Item1 + focus, 0);
				}
				else
				{
					this._affectedSkillMap.Add(skillObject, new Tuple<int, int>(focus, 0));
				}
			}
			foreach (CharacterAttribute characterAttribute in Attributes.All)
			{
				int attributeValue = Hero.MainHero.GetAttributeValue(characterAttribute);
				if (this._affectedAttributesMap.ContainsKey(characterAttribute))
				{
					Tuple<int, int> tuple2 = this._affectedAttributesMap[characterAttribute];
					this._affectedAttributesMap[characterAttribute] = new Tuple<int, int>(tuple2.Item1 + attributeValue, 0);
				}
				else
				{
					this._affectedAttributesMap.Add(characterAttribute, new Tuple<int, int>(attributeValue, 0));
				}
			}
		}

		private void PopulateGainedAttributeValues()
		{
			for (int i = 0; i < this._characterCreation.CharacterCreationMenuCount; i++)
			{
				int selectedOptionId = (this._characterCreation.GetSelectedOptions(i).Any<int>() ? this._characterCreation.GetSelectedOptions(i).First<int>() : (-1));
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				if (selectedOptionId != -1)
				{
					CharacterCreationOption characterCreationOption = this._characterCreation.GetCurrentMenuOptions(i).SingleOrDefault((CharacterCreationOption o) => o.Id == selectedOptionId);
					if (characterCreationOption != null)
					{
						if (i == this._currentIndex)
						{
							num3 = characterCreationOption.AttributeLevelToAdd;
						}
						else
						{
							num4 += characterCreationOption.AttributeLevelToAdd;
						}
						if (characterCreationOption.EffectedAttribute != null)
						{
							if (this._affectedAttributesMap.ContainsKey(characterCreationOption.EffectedAttribute))
							{
								Tuple<int, int> tuple = this._affectedAttributesMap[characterCreationOption.EffectedAttribute];
								this._affectedAttributesMap[characterCreationOption.EffectedAttribute] = new Tuple<int, int>(tuple.Item1 + num4, tuple.Item2 + num3);
							}
							else
							{
								this._affectedAttributesMap.Add(characterCreationOption.EffectedAttribute, new Tuple<int, int>(num4, num3));
							}
						}
						if (i == this._currentIndex)
						{
							num = characterCreationOption.FocusToAdd;
						}
						else
						{
							num2 += characterCreationOption.FocusToAdd;
						}
						foreach (SkillObject skillObject in characterCreationOption.AffectedSkills)
						{
							if (this._affectedSkillMap.ContainsKey(skillObject))
							{
								Tuple<int, int> tuple2 = this._affectedSkillMap[skillObject];
								this._affectedSkillMap[skillObject] = new Tuple<int, int>(tuple2.Item1 + num2, tuple2.Item2 + num);
							}
							else
							{
								this._affectedSkillMap.Add(skillObject, new Tuple<int, int>(num2, num));
							}
						}
					}
				}
			}
		}

		private void PopulateGainedTraitValues()
		{
			this.GainedTraits.Clear();
			for (int i = 0; i < this._characterCreation.CharacterCreationMenuCount; i++)
			{
				int selectedOptionId = (this._characterCreation.GetSelectedOptions(i).Any<int>() ? this._characterCreation.GetSelectedOptions(i).First<int>() : (-1));
				if (selectedOptionId != -1)
				{
					CharacterCreationOption characterCreationOption = this._characterCreation.GetCurrentMenuOptions(i).SingleOrDefault((CharacterCreationOption o) => o.Id == selectedOptionId);
					if (((characterCreationOption != null) ? characterCreationOption.AffectedTraits : null) != null && characterCreationOption.AffectedTraits.Count > 0)
					{
						using (List<TraitObject>.Enumerator enumerator = characterCreationOption.AffectedTraits.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								TraitObject effectedTrait = enumerator.Current;
								if (this.GainedTraits.FirstOrDefault((EncyclopediaTraitItemVM t) => t.TraitId == effectedTrait.StringId) == null)
								{
									this.GainedTraits.Add(new EncyclopediaTraitItemVM(effectedTrait, 1));
								}
							}
						}
					}
				}
			}
		}

		private CharacterCreationGainedAttributeItemVM GetItemFromAttribute(CharacterAttribute attribute)
		{
			CharacterCreationGainGroupItemVM characterCreationGainGroupItemVM = this.GainGroups.SingleOrDefault((CharacterCreationGainGroupItemVM g) => g.AttributeObj == attribute);
			if (characterCreationGainGroupItemVM == null)
			{
				return null;
			}
			return characterCreationGainGroupItemVM.Attribute;
		}

		private CharacterCreationGainedSkillItemVM GetItemFromSkill(SkillObject skill)
		{
			Func<CharacterCreationGainedSkillItemVM, bool> <>9__2;
			CharacterCreationGainGroupItemVM characterCreationGainGroupItemVM = this.GainGroups.SingleOrDefault(delegate(CharacterCreationGainGroupItemVM g)
			{
				IEnumerable<CharacterCreationGainedSkillItemVM> skills = g.Skills;
				Func<CharacterCreationGainedSkillItemVM, bool> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (CharacterCreationGainedSkillItemVM s) => s.SkillObj == skill);
				}
				return skills.SingleOrDefault(func) != null;
			});
			if (characterCreationGainGroupItemVM == null)
			{
				return null;
			}
			return characterCreationGainGroupItemVM.Skills.SingleOrDefault((CharacterCreationGainedSkillItemVM s) => s.SkillObj == skill);
		}

		[DataSourceProperty]
		public MBBindingList<CharacterCreationGainGroupItemVM> GainGroups
		{
			get
			{
				return this._gainGroups;
			}
			set
			{
				if (value != this._gainGroups)
				{
					this._gainGroups = value;
					base.OnPropertyChangedWithValue<MBBindingList<CharacterCreationGainGroupItemVM>>(value, "GainGroups");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<EncyclopediaTraitItemVM> GainedTraits
		{
			get
			{
				return this._gainedTraits;
			}
			set
			{
				if (value != this._gainedTraits)
				{
					this._gainedTraits = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaTraitItemVM>>(value, "GainedTraits");
				}
			}
		}

		private readonly CharacterCreation _characterCreation;

		private readonly int _currentIndex;

		private readonly Dictionary<CharacterAttribute, Tuple<int, int>> _affectedAttributesMap;

		private readonly Dictionary<SkillObject, Tuple<int, int>> _affectedSkillMap;

		private MBBindingList<CharacterCreationGainGroupItemVM> _gainGroups;

		private MBBindingList<EncyclopediaTraitItemVM> _gainedTraits;
	}
}
