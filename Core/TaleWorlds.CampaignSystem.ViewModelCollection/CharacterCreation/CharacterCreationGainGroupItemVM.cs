using System;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	public class CharacterCreationGainGroupItemVM : ViewModel
	{
		public CharacterAttribute AttributeObj { get; private set; }

		public CharacterCreationGainGroupItemVM(CharacterAttribute attributeObj, CharacterCreation characterCreation, int currentIndex)
		{
			this.AttributeObj = attributeObj;
			this._characterCreation = characterCreation;
			this._currentIndex = currentIndex;
			this.Skills = new MBBindingList<CharacterCreationGainedSkillItemVM>();
			this.Attribute = new CharacterCreationGainedAttributeItemVM(this.AttributeObj);
			foreach (SkillObject skillObject in this.AttributeObj.Skills)
			{
				this.Skills.Add(new CharacterCreationGainedSkillItemVM(skillObject));
			}
		}

		public void ResetValues()
		{
			this.Attribute.ResetValues();
			this.Skills.ApplyActionOnAllItems(delegate(CharacterCreationGainedSkillItemVM s)
			{
				s.ResetValues();
			});
		}

		[DataSourceProperty]
		public MBBindingList<CharacterCreationGainedSkillItemVM> Skills
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
					base.OnPropertyChangedWithValue<MBBindingList<CharacterCreationGainedSkillItemVM>>(value, "Skills");
				}
			}
		}

		[DataSourceProperty]
		public CharacterCreationGainedAttributeItemVM Attribute
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
					base.OnPropertyChangedWithValue<CharacterCreationGainedAttributeItemVM>(value, "Attribute");
				}
			}
		}

		private readonly CharacterCreation _characterCreation;

		private readonly int _currentIndex;

		private MBBindingList<CharacterCreationGainedSkillItemVM> _skills;

		private CharacterCreationGainedAttributeItemVM _attribute;
	}
}
