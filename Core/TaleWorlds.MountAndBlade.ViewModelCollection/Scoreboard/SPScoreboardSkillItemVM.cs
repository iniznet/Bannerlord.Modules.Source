using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard
{
	// Token: 0x02000011 RID: 17
	public class SPScoreboardSkillItemVM : ViewModel
	{
		// Token: 0x0600014E RID: 334 RVA: 0x00005E9C File Offset: 0x0000409C
		public SPScoreboardSkillItemVM(SkillObject skill, int initialValue)
		{
			this.Skill = skill;
			this._initialValue = initialValue;
			this._newValue = initialValue;
			this.SkillId = skill.StringId;
			this.Description = "(" + initialValue + ")";
			this.RefreshValues();
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00005EF1 File Offset: 0x000040F1
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Level = this.Skill.Name.ToString();
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00005F10 File Offset: 0x00004110
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

		// Token: 0x06000151 RID: 337 RVA: 0x00005F66 File Offset: 0x00004166
		public bool IsValid()
		{
			return this._newValue > this._initialValue;
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00005F76 File Offset: 0x00004176
		// (set) Token: 0x06000153 RID: 339 RVA: 0x00005F7E File Offset: 0x0000417E
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

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00005FA1 File Offset: 0x000041A1
		// (set) Token: 0x06000155 RID: 341 RVA: 0x00005FA9 File Offset: 0x000041A9
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

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00005FCC File Offset: 0x000041CC
		// (set) Token: 0x06000157 RID: 343 RVA: 0x00005FD4 File Offset: 0x000041D4
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

		// Token: 0x04000093 RID: 147
		public SkillObject Skill;

		// Token: 0x04000094 RID: 148
		private readonly int _initialValue;

		// Token: 0x04000095 RID: 149
		private int _newValue;

		// Token: 0x04000096 RID: 150
		private string _level;

		// Token: 0x04000097 RID: 151
		private string _imagePath;

		// Token: 0x04000098 RID: 152
		private string _description;
	}
}
