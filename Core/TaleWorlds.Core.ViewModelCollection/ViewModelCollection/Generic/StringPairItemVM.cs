using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	// Token: 0x02000023 RID: 35
	public class StringPairItemVM : ViewModel
	{
		// Token: 0x06000190 RID: 400 RVA: 0x00005396 File Offset: 0x00003596
		public StringPairItemVM(string definition, string value, BasicTooltipViewModel hint = null)
		{
			this.Definition = definition;
			this.Value = value;
			this.Hint = hint;
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000191 RID: 401 RVA: 0x000053B3 File Offset: 0x000035B3
		// (set) Token: 0x06000192 RID: 402 RVA: 0x000053BB File Offset: 0x000035BB
		[DataSourceProperty]
		public string Definition
		{
			get
			{
				return this._definition;
			}
			set
			{
				if (value != this._definition)
				{
					this._definition = value;
					base.OnPropertyChangedWithValue<string>(value, "Definition");
				}
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000193 RID: 403 RVA: 0x000053DE File Offset: 0x000035DE
		// (set) Token: 0x06000194 RID: 404 RVA: 0x000053E6 File Offset: 0x000035E6
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

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000195 RID: 405 RVA: 0x00005409 File Offset: 0x00003609
		// (set) Token: 0x06000196 RID: 406 RVA: 0x00005411 File Offset: 0x00003611
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

		// Token: 0x040000A2 RID: 162
		private string _definition;

		// Token: 0x040000A3 RID: 163
		private string _value;

		// Token: 0x040000A4 RID: 164
		private BasicTooltipViewModel _hint;
	}
}
