using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard
{
	public class SPScoreboardSkillItemVM : ViewModel
	{
		public SPScoreboardSkillItemVM(SkillObject skill, int initialValue)
		{
			this.Skill = skill;
			this._initialValue = initialValue;
			this._newValue = initialValue;
			this.SkillId = skill.StringId;
			this.Description = "(" + initialValue + ")";
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Level = this.Skill.Name.ToString();
		}

		public void UpdateSkill(int newValue)
		{
			this._newValue = newValue;
			this.Description = string.Concat(new object[]
			{
				"+",
				newValue - this._initialValue,
				"(",
				newValue,
				")"
			});
		}

		public bool IsValid()
		{
			return this._newValue > this._initialValue;
		}

		[DataSourceProperty]
		public string Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (value != this._level)
				{
					this._level = value;
					base.OnPropertyChangedWithValue<string>(value, "Level");
				}
			}
		}

		[DataSourceProperty]
		public string SkillId
		{
			get
			{
				return this._imagePath;
			}
			set
			{
				if (value != this._imagePath)
				{
					this._imagePath = value;
					base.OnPropertyChangedWithValue<string>(value, "SkillId");
				}
			}
		}

		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		public SkillObject Skill;

		private readonly int _initialValue;

		private int _newValue;

		private string _level;

		private string _imagePath;

		private string _description;
	}
}
