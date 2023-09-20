using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	// Token: 0x020000C9 RID: 201
	public class EncyclopediaSkillVM : ViewModel
	{
		// Token: 0x06001323 RID: 4899 RVA: 0x000499AB File Offset: 0x00047BAB
		public EncyclopediaSkillVM(SkillObject skill, int skillValue)
		{
			this._skill = skill;
			this.SkillValue = skillValue;
			this.SkillId = skill.StringId;
			this.RefreshValues();
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x000499D4 File Offset: 0x00047BD4
		public override void RefreshValues()
		{
			base.RefreshValues();
			string name = this._skill.Name.ToString();
			string desc = this._skill.Description.ToString();
			this.Hint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("STR1", name);
				GameTexts.SetVariable("STR2", desc);
				return GameTexts.FindText("str_string_newline_string", null).ToString();
			});
		}

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x06001325 RID: 4901 RVA: 0x00049A30 File Offset: 0x00047C30
		// (set) Token: 0x06001326 RID: 4902 RVA: 0x00049A38 File Offset: 0x00047C38
		[DataSourceProperty]
		public BasicTooltipViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06001327 RID: 4903 RVA: 0x00049A56 File Offset: 0x00047C56
		// (set) Token: 0x06001328 RID: 4904 RVA: 0x00049A5E File Offset: 0x00047C5E
		[DataSourceProperty]
		public int SkillValue
		{
			get
			{
				return this._skillValue;
			}
			set
			{
				if (value != this._skillValue)
				{
					this._skillValue = value;
					base.OnPropertyChangedWithValue(value, "SkillValue");
				}
			}
		}

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06001329 RID: 4905 RVA: 0x00049A7C File Offset: 0x00047C7C
		// (set) Token: 0x0600132A RID: 4906 RVA: 0x00049A84 File Offset: 0x00047C84
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

		// Token: 0x040008DD RID: 2269
		private readonly SkillObject _skill;

		// Token: 0x040008DE RID: 2270
		private string _skillId;

		// Token: 0x040008DF RID: 2271
		private int _skillValue;

		// Token: 0x040008E0 RID: 2272
		private BasicTooltipViewModel _hint;
	}
}
