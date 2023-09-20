using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	// Token: 0x0200001B RID: 27
	public class SelectableItemPropertyVM : ViewModel
	{
		// Token: 0x06000196 RID: 406 RVA: 0x0000F606 File Offset: 0x0000D806
		public SelectableItemPropertyVM(string name, string value, BasicTooltipViewModel hint = null)
		{
			this.Name = name;
			this.Value = value;
			this.Hint = hint;
			this.Type = 0;
			this.RefreshValues();
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000F630 File Offset: 0x0000D830
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ColonText = GameTexts.FindText("str_colon", null).ToString();
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000F64E File Offset: 0x0000D84E
		private void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000199 RID: 409 RVA: 0x0000F660 File Offset: 0x0000D860
		// (set) Token: 0x0600019A RID: 410 RVA: 0x0000F668 File Offset: 0x0000D868
		[DataSourceProperty]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value != this._type)
				{
					this._type = value;
					base.OnPropertyChangedWithValue(value, "Type");
				}
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600019B RID: 411 RVA: 0x0000F686 File Offset: 0x0000D886
		// (set) Token: 0x0600019C RID: 412 RVA: 0x0000F68E File Offset: 0x0000D88E
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600019D RID: 413 RVA: 0x0000F6B1 File Offset: 0x0000D8B1
		// (set) Token: 0x0600019E RID: 414 RVA: 0x0000F6B9 File Offset: 0x0000D8B9
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

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600019F RID: 415 RVA: 0x0000F6DC File Offset: 0x0000D8DC
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x0000F6E4 File Offset: 0x0000D8E4
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

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000F702 File Offset: 0x0000D902
		// (set) Token: 0x060001A2 RID: 418 RVA: 0x0000F70A File Offset: 0x0000D90A
		[DataSourceProperty]
		public string ColonText
		{
			get
			{
				return this._colonText;
			}
			set
			{
				if (value != this._colonText)
				{
					this._colonText = value;
					base.OnPropertyChangedWithValue<string>(value, "ColonText");
				}
			}
		}

		// Token: 0x040000B6 RID: 182
		private int _type;

		// Token: 0x040000B7 RID: 183
		private string _name;

		// Token: 0x040000B8 RID: 184
		private string _value;

		// Token: 0x040000B9 RID: 185
		private BasicTooltipViewModel _hint;

		// Token: 0x040000BA RID: 186
		private string _colonText;

		// Token: 0x02000156 RID: 342
		public enum PropertyType
		{
			// Token: 0x04000EA6 RID: 3750
			None,
			// Token: 0x04000EA7 RID: 3751
			Wall,
			// Token: 0x04000EA8 RID: 3752
			Garrison,
			// Token: 0x04000EA9 RID: 3753
			Militia,
			// Token: 0x04000EAA RID: 3754
			Prosperity,
			// Token: 0x04000EAB RID: 3755
			Food,
			// Token: 0x04000EAC RID: 3756
			Loyalty,
			// Token: 0x04000EAD RID: 3757
			Security
		}
	}
}
