using System;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	// Token: 0x02000126 RID: 294
	public class CharacterCreationGainGroupItemVM : ViewModel
	{
		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x06001C48 RID: 7240 RVA: 0x00065DC1 File Offset: 0x00063FC1
		// (set) Token: 0x06001C49 RID: 7241 RVA: 0x00065DC9 File Offset: 0x00063FC9
		public CharacterAttribute AttributeObj { get; private set; }

		// Token: 0x06001C4A RID: 7242 RVA: 0x00065DD4 File Offset: 0x00063FD4
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

		// Token: 0x06001C4B RID: 7243 RVA: 0x00065E70 File Offset: 0x00064070
		public void ResetValues()
		{
			this.Attribute.ResetValues();
			this.Skills.ApplyActionOnAllItems(delegate(CharacterCreationGainedSkillItemVM s)
			{
				s.ResetValues();
			});
		}

		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x06001C4C RID: 7244 RVA: 0x00065EA7 File Offset: 0x000640A7
		// (set) Token: 0x06001C4D RID: 7245 RVA: 0x00065EAF File Offset: 0x000640AF
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

		// Token: 0x170009B1 RID: 2481
		// (get) Token: 0x06001C4E RID: 7246 RVA: 0x00065ECD File Offset: 0x000640CD
		// (set) Token: 0x06001C4F RID: 7247 RVA: 0x00065ED5 File Offset: 0x000640D5
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

		// Token: 0x04000D57 RID: 3415
		private readonly CharacterCreation _characterCreation;

		// Token: 0x04000D58 RID: 3416
		private readonly int _currentIndex;

		// Token: 0x04000D59 RID: 3417
		private MBBindingList<CharacterCreationGainedSkillItemVM> _skills;

		// Token: 0x04000D5A RID: 3418
		private CharacterCreationGainedAttributeItemVM _attribute;
	}
}
