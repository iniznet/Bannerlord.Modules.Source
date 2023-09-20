using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	// Token: 0x02000128 RID: 296
	public class CharacterCreationGainedAttributeItemVM : ViewModel
	{
		// Token: 0x06001C5D RID: 7261 RVA: 0x00066044 File Offset: 0x00064244
		public CharacterCreationGainedAttributeItemVM(CharacterAttribute attributeObj)
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

		// Token: 0x06001C5E RID: 7262 RVA: 0x000660A5 File Offset: 0x000642A5
		internal void ResetValues()
		{
			this.SetValue(0, 0);
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x000660B0 File Offset: 0x000642B0
		public void SetValue(int gainedFromOtherStages, int gainedFromCurrentStage)
		{
			this.HasIncreasedInCurrentStage = gainedFromCurrentStage > 0;
			GameTexts.SetVariable("LEFT", this._attributeObj.Name);
			GameTexts.SetVariable("RIGHT", gainedFromOtherStages + gainedFromCurrentStage);
			this.NameText = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
		}

		// Token: 0x170009B7 RID: 2487
		// (get) Token: 0x06001C60 RID: 7264 RVA: 0x000660FF File Offset: 0x000642FF
		// (set) Token: 0x06001C61 RID: 7265 RVA: 0x00066107 File Offset: 0x00064307
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

		// Token: 0x170009B8 RID: 2488
		// (get) Token: 0x06001C62 RID: 7266 RVA: 0x00066125 File Offset: 0x00064325
		// (set) Token: 0x06001C63 RID: 7267 RVA: 0x0006612D File Offset: 0x0006432D
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

		// Token: 0x170009B9 RID: 2489
		// (get) Token: 0x06001C64 RID: 7268 RVA: 0x00066150 File Offset: 0x00064350
		// (set) Token: 0x06001C65 RID: 7269 RVA: 0x00066158 File Offset: 0x00064358
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

		// Token: 0x04000D60 RID: 3424
		private readonly CharacterAttribute _attributeObj;

		// Token: 0x04000D61 RID: 3425
		private string _nameText;

		// Token: 0x04000D62 RID: 3426
		private bool _hasIncreasedInCurrentStage;

		// Token: 0x04000D63 RID: 3427
		private BasicTooltipViewModel _hint;
	}
}
