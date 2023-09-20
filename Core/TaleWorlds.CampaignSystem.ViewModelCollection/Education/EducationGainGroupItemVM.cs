using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	// Token: 0x020000CF RID: 207
	public class EducationGainGroupItemVM : ViewModel
	{
		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x0600135E RID: 4958 RVA: 0x0004A883 File Offset: 0x00048A83
		// (set) Token: 0x0600135F RID: 4959 RVA: 0x0004A88B File Offset: 0x00048A8B
		public CharacterAttribute AttributeObj { get; private set; }

		// Token: 0x06001360 RID: 4960 RVA: 0x0004A894 File Offset: 0x00048A94
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

		// Token: 0x06001361 RID: 4961 RVA: 0x0004A920 File Offset: 0x00048B20
		public void ResetValues()
		{
			this.Attribute.ResetValues();
			this.Skills.ApplyActionOnAllItems(delegate(EducationGainedSkillItemVM s)
			{
				s.ResetValues();
			});
		}

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x06001362 RID: 4962 RVA: 0x0004A957 File Offset: 0x00048B57
		// (set) Token: 0x06001363 RID: 4963 RVA: 0x0004A95F File Offset: 0x00048B5F
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

		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x06001364 RID: 4964 RVA: 0x0004A97D File Offset: 0x00048B7D
		// (set) Token: 0x06001365 RID: 4965 RVA: 0x0004A985 File Offset: 0x00048B85
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

		// Token: 0x040008FB RID: 2299
		private MBBindingList<EducationGainedSkillItemVM> _skills;

		// Token: 0x040008FC RID: 2300
		private EducationGainedAttributeItemVM _attribute;
	}
}
