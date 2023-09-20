using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	// Token: 0x02000127 RID: 295
	public class CharacterCreationGainedSkillItemVM : ViewModel
	{
		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x06001C50 RID: 7248 RVA: 0x00065EF3 File Offset: 0x000640F3
		// (set) Token: 0x06001C51 RID: 7249 RVA: 0x00065EFB File Offset: 0x000640FB
		public SkillObject SkillObj { get; private set; }

		// Token: 0x06001C52 RID: 7250 RVA: 0x00065F04 File Offset: 0x00064104
		public CharacterCreationGainedSkillItemVM(SkillObject skill)
		{
			this.FocusPointGainList = new MBBindingList<BoolItemWithActionVM>();
			this.SkillObj = skill;
			this.SkillId = this.SkillObj.StringId;
			this.Skill = new EncyclopediaSkillVM(skill, 0);
		}

		// Token: 0x06001C53 RID: 7251 RVA: 0x00065F3C File Offset: 0x0006413C
		public void SetValue(int gainedFromOtherStages, int gainedFromCurrentStage)
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
			this.HasIncreasedInCurrentStage = gainedFromCurrentStage > 0;
		}

		// Token: 0x06001C54 RID: 7252 RVA: 0x00065F9C File Offset: 0x0006419C
		internal void ResetValues()
		{
			this.SetValue(0, 0);
		}

		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x06001C55 RID: 7253 RVA: 0x00065FA6 File Offset: 0x000641A6
		// (set) Token: 0x06001C56 RID: 7254 RVA: 0x00065FAE File Offset: 0x000641AE
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

		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x06001C57 RID: 7255 RVA: 0x00065FD1 File Offset: 0x000641D1
		// (set) Token: 0x06001C58 RID: 7256 RVA: 0x00065FD9 File Offset: 0x000641D9
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

		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x06001C59 RID: 7257 RVA: 0x00065FF7 File Offset: 0x000641F7
		// (set) Token: 0x06001C5A RID: 7258 RVA: 0x00065FFF File Offset: 0x000641FF
		[DataSourceProperty]
		public bool HasIncreasedInCurrentStage
		{
			get
			{
				return this._hasIncreasedInCurrentStage;
			}
			set
			{
				if (value != this._hasIncreasedInCurrentStage)
				{
					this._hasIncreasedInCurrentStage = value;
					base.OnPropertyChangedWithValue(value, "HasIncreasedInCurrentStage");
				}
			}
		}

		// Token: 0x170009B6 RID: 2486
		// (get) Token: 0x06001C5B RID: 7259 RVA: 0x0006601D File Offset: 0x0006421D
		// (set) Token: 0x06001C5C RID: 7260 RVA: 0x00066025 File Offset: 0x00064225
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

		// Token: 0x04000D5C RID: 3420
		private string _skillId;

		// Token: 0x04000D5D RID: 3421
		private EncyclopediaSkillVM _skill;

		// Token: 0x04000D5E RID: 3422
		private bool _hasIncreasedInCurrentStage;

		// Token: 0x04000D5F RID: 3423
		private MBBindingList<BoolItemWithActionVM> _focusPointGainList;
	}
}
