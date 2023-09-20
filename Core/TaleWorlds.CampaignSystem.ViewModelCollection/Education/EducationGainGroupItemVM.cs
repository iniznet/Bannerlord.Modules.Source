using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	public class EducationGainGroupItemVM : ViewModel
	{
		public CharacterAttribute AttributeObj { get; private set; }

		public EducationGainGroupItemVM(CharacterAttribute attributeObj)
		{
			this.AttributeObj = attributeObj;
			this.Skills = new MBBindingList<EducationGainedSkillItemVM>();
			this.Attribute = new EducationGainedAttributeItemVM(this.AttributeObj);
			foreach (SkillObject skillObject in this.AttributeObj.Skills)
			{
				this.Skills.Add(new EducationGainedSkillItemVM(skillObject));
			}
		}

		public void ResetValues()
		{
			this.Attribute.ResetValues();
			this.Skills.ApplyActionOnAllItems(delegate(EducationGainedSkillItemVM s)
			{
				s.ResetValues();
			});
		}

		[DataSourceProperty]
		public MBBindingList<EducationGainedSkillItemVM> Skills
		{
			get
			{
				return this._skills;
			}
			set
			{
				if (value != this._skills)
				{
					this._skills = value;
					base.OnPropertyChangedWithValue<MBBindingList<EducationGainedSkillItemVM>>(value, "Skills");
				}
			}
		}

		[DataSourceProperty]
		public EducationGainedAttributeItemVM Attribute
		{
			get
			{
				return this._attribute;
			}
			set
			{
				if (value != this._attribute)
				{
					this._attribute = value;
					base.OnPropertyChangedWithValue<EducationGainedAttributeItemVM>(value, "Attribute");
				}
			}
		}

		private MBBindingList<EducationGainedSkillItemVM> _skills;

		private EducationGainedAttributeItemVM _attribute;
	}
}
