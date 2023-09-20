using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	// Token: 0x020000CA RID: 202
	public class EncyclopediaTraitItemVM : ViewModel
	{
		// Token: 0x0600132B RID: 4907 RVA: 0x00049AA8 File Offset: 0x00047CA8
		public EncyclopediaTraitItemVM(TraitObject traitObj, int value)
		{
			this._traitObj = traitObj;
			this.TraitId = traitObj.StringId;
			this.Value = value;
			string traitTooltipText = CampaignUIHelper.GetTraitTooltipText(traitObj, this.Value);
			this.Hint = new HintViewModel(new TextObject("{=!}" + traitTooltipText, null), null);
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x00049AFF File Offset: 0x00047CFF
		public EncyclopediaTraitItemVM(TraitObject traitObj, Hero hero)
			: this(traitObj, hero.GetTraitLevel(traitObj))
		{
		}

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x0600132D RID: 4909 RVA: 0x00049B0F File Offset: 0x00047D0F
		// (set) Token: 0x0600132E RID: 4910 RVA: 0x00049B17 File Offset: 0x00047D17
		[DataSourceProperty]
		public string TraitId
		{
			get
			{
				return this._traitId;
			}
			set
			{
				if (value != this._traitId)
				{
					this._traitId = value;
					base.OnPropertyChangedWithValue<string>(value, "TraitId");
				}
			}
		}

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x0600132F RID: 4911 RVA: 0x00049B3A File Offset: 0x00047D3A
		// (set) Token: 0x06001330 RID: 4912 RVA: 0x00049B42 File Offset: 0x00047D42
		[DataSourceProperty]
		public HintViewModel Hint
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
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06001331 RID: 4913 RVA: 0x00049B60 File Offset: 0x00047D60
		// (set) Token: 0x06001332 RID: 4914 RVA: 0x00049B68 File Offset: 0x00047D68
		[DataSourceProperty]
		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (value != this._value)
				{
					this._value = value;
					base.OnPropertyChangedWithValue(value, "Value");
				}
			}
		}

		// Token: 0x040008E1 RID: 2273
		private readonly TraitObject _traitObj;

		// Token: 0x040008E2 RID: 2274
		private string _traitId;

		// Token: 0x040008E3 RID: 2275
		private int _value;

		// Token: 0x040008E4 RID: 2276
		private HintViewModel _hint;
	}
}
