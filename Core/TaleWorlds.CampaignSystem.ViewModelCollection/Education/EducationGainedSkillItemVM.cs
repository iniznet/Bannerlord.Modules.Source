using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	public class EducationGainedSkillItemVM : ViewModel
	{
		public SkillObject SkillObj { get; private set; }

		public EducationGainedSkillItemVM(SkillObject skill)
		{
			this.FocusPointGainList = new MBBindingList<BoolItemWithActionVM>();
			this.SkillObj = skill;
			this.SkillId = this.SkillObj.StringId;
			this.Skill = new EncyclopediaSkillVM(skill, 0);
		}

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

		public void SetSkillValue(int gaintedFromOtherStages, int gainedFromCurrentStage)
		{
			this.SkillValueInt = gaintedFromOtherStages + gainedFromCurrentStage;
			this.HasSkillValueIncreasedInCurrentStage = gainedFromCurrentStage > 0;
		}

		internal void ResetValues()
		{
			this.SetFocusValue(0, 0);
			this.SetSkillValue(0, 0);
		}

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

		private string _skillId;

		private EncyclopediaSkillVM _skill;

		private bool _hasFocusIncreasedInCurrentStage;

		private bool _hasSkillValueIncreasedInCurrentStage;

		private int _skillValueInt;

		private MBBindingList<BoolItemWithActionVM> _focusPointGainList;
	}
}
