using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper
{
	// Token: 0x02000117 RID: 279
	public class AttributeBoundSkillItemVM : ViewModel
	{
		// Token: 0x06001AC5 RID: 6853 RVA: 0x000611C4 File Offset: 0x0005F3C4
		public AttributeBoundSkillItemVM(SkillObject skill)
		{
			this.Name = skill.Name.ToString();
			this.SkillId = skill.StringId;
		}

		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x06001AC6 RID: 6854 RVA: 0x000611E9 File Offset: 0x0005F3E9
		// (set) Token: 0x06001AC7 RID: 6855 RVA: 0x000611F1 File Offset: 0x0005F3F1
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x1700092B RID: 2347
		// (get) Token: 0x06001AC8 RID: 6856 RVA: 0x00061214 File Offset: 0x0005F414
		// (set) Token: 0x06001AC9 RID: 6857 RVA: 0x0006121C File Offset: 0x0005F41C
		[DataSourceProperty]
		public string SkillId
		{
			get
			{
				return this._skillId;
			}
			set
			{
				if (value != this._skillId)
				{
					this._skillId = value;
					base.OnPropertyChangedWithValue<string>(value, "SkillId");
				}
			}
		}

		// Token: 0x04000CAF RID: 3247
		private string _name;

		// Token: 0x04000CB0 RID: 3248
		private string _skillId;
	}
}
