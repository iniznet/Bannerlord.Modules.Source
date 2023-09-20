using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	// Token: 0x020000D1 RID: 209
	public class EducationGainedAttributeItemVM : ViewModel
	{
		// Token: 0x06001378 RID: 4984 RVA: 0x0004AB5C File Offset: 0x00048D5C
		public EducationGainedAttributeItemVM(CharacterAttribute attributeObj)
		{
			this._attributeObj = attributeObj;
			TextObject nameExtended = this._attributeObj.Name;
			TextObject desc = this._attributeObj.Description;
			this.Hint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("STR1", nameExtended);
				GameTexts.SetVariable("STR2", desc);
				return GameTexts.FindText("str_string_newline_string", null).ToString();
			});
			this.SetValue(0, 0);
		}

		// Token: 0x06001379 RID: 4985 RVA: 0x0004ABBD File Offset: 0x00048DBD
		internal void ResetValues()
		{
			this.SetValue(0, 0);
		}

		// Token: 0x0600137A RID: 4986 RVA: 0x0004ABC8 File Offset: 0x00048DC8
		public void SetValue(int gainedFromOtherStages, int gainedFromCurrentStage)
		{
			this.HasIncreasedInCurrentStage = gainedFromCurrentStage > 0;
			GameTexts.SetVariable("LEFT", this._attributeObj.Name);
			GameTexts.SetVariable("RIGHT", gainedFromOtherStages + gainedFromCurrentStage);
			this.NameText = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
		}

		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x0600137B RID: 4987 RVA: 0x0004AC17 File Offset: 0x00048E17
		// (set) Token: 0x0600137C RID: 4988 RVA: 0x0004AC1F File Offset: 0x00048E1F
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

		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x0600137D RID: 4989 RVA: 0x0004AC3D File Offset: 0x00048E3D
		// (set) Token: 0x0600137E RID: 4990 RVA: 0x0004AC45 File Offset: 0x00048E45
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x0600137F RID: 4991 RVA: 0x0004AC68 File Offset: 0x00048E68
		// (set) Token: 0x06001380 RID: 4992 RVA: 0x0004AC70 File Offset: 0x00048E70
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

		// Token: 0x04000904 RID: 2308
		private readonly CharacterAttribute _attributeObj;

		// Token: 0x04000905 RID: 2309
		private string _nameText;

		// Token: 0x04000906 RID: 2310
		private bool _hasIncreasedInCurrentStage;

		// Token: 0x04000907 RID: 2311
		private BasicTooltipViewModel _hint;
	}
}
