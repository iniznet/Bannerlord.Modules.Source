using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	public class EducationGainedPropertiesVM : ViewModel
	{
		public EducationGainedPropertiesVM(Hero child, int pageCount)
		{
			this._child = child;
			this._pageCount = pageCount;
			this._educationBehavior = Campaign.Current.GetCampaignBehavior<IEducationLogic>();
			this._affectedSkillFocusMap = new Dictionary<SkillObject, Tuple<int, int>>();
			this._affectedSkillValueMap = new Dictionary<SkillObject, Tuple<int, int>>();
			this._affectedAttributesMap = new Dictionary<CharacterAttribute, Tuple<int, int>>();
			this.GainGroups = new MBBindingList<EducationGainGroupItemVM>();
			foreach (CharacterAttribute characterAttribute in Attributes.All)
			{
				this.GainGroups.Add(new EducationGainGroupItemVM(characterAttribute));
			}
			this.UpdateWithSelections(new List<string>(), -1);
		}

		internal void UpdateWithSelections(List<string> selectedOptions, int currentPageIndex)
		{
			this._affectedAttributesMap.Clear();
			this._affectedSkillFocusMap.Clear();
			this._affectedSkillValueMap.Clear();
			this.GainGroups.ApplyActionOnAllItems(delegate(EducationGainGroupItemVM g)
			{
				g.ResetValues();
			});
			this.PopulateInitialValues();
			this.PopulateGainedAttributeValues(selectedOptions, currentPageIndex);
			foreach (KeyValuePair<CharacterAttribute, Tuple<int, int>> keyValuePair in this._affectedAttributesMap)
			{
				this.GetItemFromAttribute(keyValuePair.Key).SetValue(keyValuePair.Value.Item1, keyValuePair.Value.Item2);
			}
			foreach (KeyValuePair<SkillObject, Tuple<int, int>> keyValuePair2 in this._affectedSkillFocusMap)
			{
				this.GetItemFromSkill(keyValuePair2.Key).SetFocusValue(keyValuePair2.Value.Item1, keyValuePair2.Value.Item2);
			}
			foreach (KeyValuePair<SkillObject, Tuple<int, int>> keyValuePair3 in this._affectedSkillValueMap)
			{
				this.GetItemFromSkill(keyValuePair3.Key).SetSkillValue(keyValuePair3.Value.Item1, keyValuePair3.Value.Item2);
			}
		}

		private void PopulateInitialValues()
		{
			foreach (SkillObject skillObject in Skills.All)
			{
				int focus = this._child.HeroDeveloper.GetFocus(skillObject);
				if (this._affectedSkillFocusMap.ContainsKey(skillObject))
				{
					Tuple<int, int> tuple = this._affectedSkillFocusMap[skillObject];
					this._affectedSkillFocusMap[skillObject] = new Tuple<int, int>(tuple.Item1 + focus, 0);
				}
				else
				{
					this._affectedSkillFocusMap.Add(skillObject, new Tuple<int, int>(focus, 0));
				}
				int skillValue = this._child.GetSkillValue(skillObject);
				if (this._affectedSkillValueMap.ContainsKey(skillObject))
				{
					Tuple<int, int> tuple2 = this._affectedSkillValueMap[skillObject];
					this._affectedSkillValueMap[skillObject] = new Tuple<int, int>(tuple2.Item1 + skillValue, 0);
				}
				else
				{
					this._affectedSkillValueMap.Add(skillObject, new Tuple<int, int>(skillValue, 0));
				}
			}
			foreach (CharacterAttribute characterAttribute in Attributes.All)
			{
				int attributeValue = this._child.GetAttributeValue(characterAttribute);
				if (this._affectedAttributesMap.ContainsKey(characterAttribute))
				{
					Tuple<int, int> tuple3 = this._affectedAttributesMap[characterAttribute];
					this._affectedAttributesMap[characterAttribute] = new Tuple<int, int>(tuple3.Item1 + attributeValue, 0);
				}
				else
				{
					this._affectedAttributesMap.Add(characterAttribute, new Tuple<int, int>(attributeValue, 0));
				}
			}
		}

		private void PopulateGainedAttributeValues(List<string> selectedOptions, int currentPageIndex)
		{
			bool flag = currentPageIndex == this._pageCount - 1;
			for (int i = 0; i < selectedOptions.Count; i++)
			{
				string text = selectedOptions[i];
				TextObject textObject;
				TextObject textObject2;
				TextObject textObject3;
				ValueTuple<CharacterAttribute, int>[] array;
				ValueTuple<SkillObject, int>[] array2;
				ValueTuple<SkillObject, int>[] array3;
				EducationCampaignBehavior.EducationCharacterProperties[] array4;
				this._educationBehavior.GetOptionProperties(this._child, text, selectedOptions, out textObject, out textObject2, out textObject3, out array, out array2, out array3, out array4);
				bool flag2 = i == currentPageIndex;
				if (array != null)
				{
					foreach (ValueTuple<CharacterAttribute, int> valueTuple in array)
					{
						Tuple<int, int> tuple = this._affectedAttributesMap[valueTuple.Item1];
						int num = (flag2 ? valueTuple.Item2 : (flag ? (tuple.Item2 + valueTuple.Item2) : 0));
						int num2 = (flag2 ? tuple.Item1 : (flag ? tuple.Item1 : (tuple.Item1 + valueTuple.Item2)));
						this._affectedAttributesMap[valueTuple.Item1] = new Tuple<int, int>(num2, num);
					}
				}
				if (array2 != null)
				{
					foreach (ValueTuple<SkillObject, int> valueTuple2 in array2)
					{
						Tuple<int, int> tuple2 = this._affectedSkillValueMap[valueTuple2.Item1];
						int num3 = (flag2 ? valueTuple2.Item2 : (flag ? (tuple2.Item2 + valueTuple2.Item2) : 0));
						int num4 = (flag2 ? tuple2.Item1 : (flag ? tuple2.Item1 : (tuple2.Item1 + valueTuple2.Item2)));
						this._affectedSkillValueMap[valueTuple2.Item1] = new Tuple<int, int>(num4, num3);
					}
				}
				if (array3 != null)
				{
					foreach (ValueTuple<SkillObject, int> valueTuple3 in array3)
					{
						Tuple<int, int> tuple3 = this._affectedSkillFocusMap[valueTuple3.Item1];
						int num5 = (flag2 ? valueTuple3.Item2 : (flag ? (tuple3.Item2 + valueTuple3.Item2) : 0));
						int num6 = (flag2 ? tuple3.Item1 : (flag ? tuple3.Item1 : (tuple3.Item1 + valueTuple3.Item2)));
						this._affectedSkillFocusMap[valueTuple3.Item1] = new Tuple<int, int>(num6, num5);
					}
				}
			}
		}

		private EducationGainedAttributeItemVM GetItemFromAttribute(CharacterAttribute attribute)
		{
			EducationGainGroupItemVM educationGainGroupItemVM = this.GainGroups.SingleOrDefault((EducationGainGroupItemVM g) => g.AttributeObj == attribute);
			if (educationGainGroupItemVM == null)
			{
				return null;
			}
			return educationGainGroupItemVM.Attribute;
		}

		private EducationGainedSkillItemVM GetItemFromSkill(SkillObject skill)
		{
			Func<EducationGainedSkillItemVM, bool> <>9__2;
			EducationGainGroupItemVM educationGainGroupItemVM = this.GainGroups.SingleOrDefault(delegate(EducationGainGroupItemVM g)
			{
				IEnumerable<EducationGainedSkillItemVM> skills = g.Skills;
				Func<EducationGainedSkillItemVM, bool> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (EducationGainedSkillItemVM s) => s.SkillObj == skill);
				}
				return skills.SingleOrDefault(func) != null;
			});
			if (educationGainGroupItemVM == null)
			{
				return null;
			}
			return educationGainGroupItemVM.Skills.SingleOrDefault((EducationGainedSkillItemVM s) => s.SkillObj == skill);
		}

		[DataSourceProperty]
		public MBBindingList<EducationGainGroupItemVM> GainGroups
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
					base.OnPropertyChangedWithValue<MBBindingList<EducationGainGroupItemVM>>(value, "GainGroups");
				}
			}
		}

		private readonly Hero _child;

		private readonly int _pageCount;

		private readonly IEducationLogic _educationBehavior;

		private readonly Dictionary<CharacterAttribute, Tuple<int, int>> _affectedAttributesMap;

		private readonly Dictionary<SkillObject, Tuple<int, int>> _affectedSkillFocusMap;

		private readonly Dictionary<SkillObject, Tuple<int, int>> _affectedSkillValueMap;

		private MBBindingList<EducationGainGroupItemVM> _gainGroups;
	}
}
