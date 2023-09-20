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
	// Token: 0x02000125 RID: 293
	public class CharacterCreationGainedPropertiesVM : ViewModel
	{
		// Token: 0x06001C3D RID: 7229 RVA: 0x000656B4 File Offset: 0x000638B4
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

		// Token: 0x06001C3E RID: 7230 RVA: 0x00065764 File Offset: 0x00063964
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

		// Token: 0x06001C3F RID: 7231 RVA: 0x00065894 File Offset: 0x00063A94
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

		// Token: 0x06001C40 RID: 7232 RVA: 0x000659D8 File Offset: 0x00063BD8
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

		// Token: 0x06001C41 RID: 7233 RVA: 0x00065BC0 File Offset: 0x00063DC0
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

		// Token: 0x06001C42 RID: 7234 RVA: 0x00065CEC File Offset: 0x00063EEC
		private CharacterCreationGainedAttributeItemVM GetItemFromAttribute(CharacterAttribute attribute)
		{
			CharacterCreationGainGroupItemVM characterCreationGainGroupItemVM = this.GainGroups.SingleOrDefault((CharacterCreationGainGroupItemVM g) => g.AttributeObj == attribute);
			if (characterCreationGainGroupItemVM == null)
			{
				return null;
			}
			return characterCreationGainGroupItemVM.Attribute;
		}

		// Token: 0x06001C43 RID: 7235 RVA: 0x00065D28 File Offset: 0x00063F28
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

		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x06001C44 RID: 7236 RVA: 0x00065D75 File Offset: 0x00063F75
		// (set) Token: 0x06001C45 RID: 7237 RVA: 0x00065D7D File Offset: 0x00063F7D
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

		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x06001C46 RID: 7238 RVA: 0x00065D9B File Offset: 0x00063F9B
		// (set) Token: 0x06001C47 RID: 7239 RVA: 0x00065DA3 File Offset: 0x00063FA3
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

		// Token: 0x04000D50 RID: 3408
		private readonly CharacterCreation _characterCreation;

		// Token: 0x04000D51 RID: 3409
		private readonly int _currentIndex;

		// Token: 0x04000D52 RID: 3410
		private readonly Dictionary<CharacterAttribute, Tuple<int, int>> _affectedAttributesMap;

		// Token: 0x04000D53 RID: 3411
		private readonly Dictionary<SkillObject, Tuple<int, int>> _affectedSkillMap;

		// Token: 0x04000D54 RID: 3412
		private MBBindingList<CharacterCreationGainGroupItemVM> _gainGroups;

		// Token: 0x04000D55 RID: 3413
		private MBBindingList<EncyclopediaTraitItemVM> _gainedTraits;
	}
}
