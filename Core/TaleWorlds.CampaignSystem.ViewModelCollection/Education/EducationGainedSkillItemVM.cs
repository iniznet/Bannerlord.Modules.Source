using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	// Token: 0x020000D0 RID: 208
	public class EducationGainedSkillItemVM : ViewModel
	{
		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x06001366 RID: 4966 RVA: 0x0004A9A3 File Offset: 0x00048BA3
		// (set) Token: 0x06001367 RID: 4967 RVA: 0x0004A9AB File Offset: 0x00048BAB
		public SkillObject SkillObj { get; private set; }

		// Token: 0x06001368 RID: 4968 RVA: 0x0004A9B4 File Offset: 0x00048BB4
		public EducationGainedSkillItemVM(SkillObject skill)
		{
			this.FocusPointGainList = new MBBindingList<BoolItemWithActionVM>();
			this.SkillObj = skill;
			this.SkillId = this.SkillObj.StringId;
			this.Skill = new EncyclopediaSkillVM(skill, 0);
		}

		// Token: 0x06001369 RID: 4969 RVA: 0x0004A9EC File Offset: 0x00048BEC
		public void SetFocusValue(int gainedFromOtherStages, int gainedFromCurrentStage)
		{
			this.FocusPointGainList.Clear();
			for (int i = 0; i < gainedFromOtherStages; i++)
			{
				this.FocusPointGainList.Add(new BoolItemWithActionVM(null, false, null));
			}
			for (int j = 0; j < gainedFromCurrentStage; j++)
			{
				this.FocusPointGainList.Add(new BoolItemWithActionVM(null, true, null));
			}
			this.HasFocusIncreasedInCurrentStage = gainedFromCurrentStage > 0;
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x0004AA4C File Offset: 0x00048C4C
		public void SetSkillValue(int gaintedFromOtherStages, int gainedFromCurrentStage)
		{
			this.SkillValueInt = gaintedFromOtherStages + gainedFromCurrentStage;
			this.HasSkillValueIncreasedInCurrentStage = gainedFromCurrentStage > 0;
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x0004AA61 File Offset: 0x00048C61
		internal void ResetValues()
		{
			this.SetFocusValue(0, 0);
			this.SetSkillValue(0, 0);
		}

		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x0600136C RID: 4972 RVA: 0x0004AA73 File Offset: 0x00048C73
		// (set) Token: 0x0600136D RID: 4973 RVA: 0x0004AA7B File Offset: 0x00048C7B
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

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x0600136E RID: 4974 RVA: 0x0004AA9E File Offset: 0x00048C9E
		// (set) Token: 0x0600136F RID: 4975 RVA: 0x0004AAA6 File Offset: 0x00048CA6
		[DataSourceProperty]
		public int SkillValueInt
		{
			get
			{
				return this._skillValueInt;
			}
			set
			{
				if (value != this._skillValueInt)
				{
					this._skillValueInt = value;
					base.OnPropertyChangedWithValue(value, "SkillValueInt");
				}
			}
		}

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x06001370 RID: 4976 RVA: 0x0004AAC4 File Offset: 0x00048CC4
		// (set) Token: 0x06001371 RID: 4977 RVA: 0x0004AACC File Offset: 0x00048CCC
		[DataSourceProperty]
		public EncyclopediaSkillVM Skill
		{
			get
			{
				return this._skill;
			}
			set
			{
				if (value != this._skill)
				{
					this._skill = value;
					base.OnPropertyChangedWithValue<EncyclopediaSkillVM>(value, "Skill");
				}
			}
		}

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x06001372 RID: 4978 RVA: 0x0004AAEA File Offset: 0x00048CEA
		// (set) Token: 0x06001373 RID: 4979 RVA: 0x0004AAF2 File Offset: 0x00048CF2
		[DataSourceProperty]
		public bool HasFocusIncreasedInCurrentStage
		{
			get
			{
				return this._hasFocusIncreasedInCurrentStage;
			}
			set
			{
				if (value != this._hasFocusIncreasedInCurrentStage)
				{
					this._hasFocusIncreasedInCurrentStage = value;
					base.OnPropertyChangedWithValue(value, "HasFocusIncreasedInCurrentStage");
				}
			}
		}

		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x06001374 RID: 4980 RVA: 0x0004AB10 File Offset: 0x00048D10
		// (set) Token: 0x06001375 RID: 4981 RVA: 0x0004AB18 File Offset: 0x00048D18
		[DataSourceProperty]
		public bool HasSkillValueIncreasedInCurrentStage
		{
			get
			{
				return this._hasSkillValueIncreasedInCurrentStage;
			}
			set
			{
				if (value != this._hasSkillValueIncreasedInCurrentStage)
				{
					this._hasSkillValueIncreasedInCurrentStage = value;
					base.OnPropertyChangedWithValue(value, "HasSkillValueIncreasedInCurrentStage");
				}
			}
		}

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x06001376 RID: 4982 RVA: 0x0004AB36 File Offset: 0x00048D36
		// (set) Token: 0x06001377 RID: 4983 RVA: 0x0004AB3E File Offset: 0x00048D3E
		[DataSourceProperty]
		public MBBindingList<BoolItemWithActionVM> FocusPointGainList
		{
			get
			{
				return this._focusPointGainList;
			}
			set
			{
				if (value != this._focusPointGainList)
				{
					this._focusPointGainList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BoolItemWithActionVM>>(value, "FocusPointGainList");
				}
			}
		}

		// Token: 0x040008FE RID: 2302
		private string _skillId;

		// Token: 0x040008FF RID: 2303
		private EncyclopediaSkillVM _skill;

		// Token: 0x04000900 RID: 2304
		private bool _hasFocusIncreasedInCurrentStage;

		// Token: 0x04000901 RID: 2305
		private bool _hasSkillValueIncreasedInCurrentStage;

		// Token: 0x04000902 RID: 2306
		private int _skillValueInt;

		// Token: 0x04000903 RID: 2307
		private MBBindingList<BoolItemWithActionVM> _focusPointGainList;
	}
}
