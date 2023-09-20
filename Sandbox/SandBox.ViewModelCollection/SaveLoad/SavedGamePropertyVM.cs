using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.ViewModelCollection.SaveLoad
{
	// Token: 0x02000010 RID: 16
	public class SavedGamePropertyVM : ViewModel
	{
		// Token: 0x0600013B RID: 315 RVA: 0x00007896 File Offset: 0x00005A96
		public SavedGamePropertyVM(SavedGamePropertyVM.SavedGameProperty type, TextObject value, TextObject hint)
		{
			this.PropertyType = type.ToString();
			this._valueText = value;
			this.Hint = new HintViewModel(hint, null);
			this.RefreshValues();
		}

		// Token: 0x0600013C RID: 316 RVA: 0x000078D6 File Offset: 0x00005AD6
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Value = this._valueText.ToString();
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600013D RID: 317 RVA: 0x000078EF File Offset: 0x00005AEF
		// (set) Token: 0x0600013E RID: 318 RVA: 0x000078F7 File Offset: 0x00005AF7
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

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600013F RID: 319 RVA: 0x00007915 File Offset: 0x00005B15
		// (set) Token: 0x06000140 RID: 320 RVA: 0x0000791D File Offset: 0x00005B1D
		[DataSourceProperty]
		public string PropertyType
		{
			get
			{
				return this._propertyType;
			}
			set
			{
				if (value != this._propertyType)
				{
					this._propertyType = value;
					base.OnPropertyChangedWithValue<string>(value, "PropertyType");
				}
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000141 RID: 321 RVA: 0x00007940 File Offset: 0x00005B40
		// (set) Token: 0x06000142 RID: 322 RVA: 0x00007948 File Offset: 0x00005B48
		[DataSourceProperty]
		public string Value
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
					base.OnPropertyChangedWithValue<string>(value, "Value");
				}
			}
		}

		// Token: 0x0400007A RID: 122
		private TextObject _valueText = TextObject.Empty;

		// Token: 0x0400007B RID: 123
		private HintViewModel _hint;

		// Token: 0x0400007C RID: 124
		private string _propertyType;

		// Token: 0x0400007D RID: 125
		private string _value;

		// Token: 0x02000055 RID: 85
		public enum SavedGameProperty
		{
			// Token: 0x040002A2 RID: 674
			None = -1,
			// Token: 0x040002A3 RID: 675
			Health,
			// Token: 0x040002A4 RID: 676
			Gold,
			// Token: 0x040002A5 RID: 677
			Influence,
			// Token: 0x040002A6 RID: 678
			PartySize,
			// Token: 0x040002A7 RID: 679
			Food,
			// Token: 0x040002A8 RID: 680
			Fiefs
		}
	}
}
