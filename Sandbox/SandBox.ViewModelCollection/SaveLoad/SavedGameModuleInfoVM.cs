using System;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.SaveLoad
{
	// Token: 0x0200000F RID: 15
	public class SavedGameModuleInfoVM : ViewModel
	{
		// Token: 0x06000134 RID: 308 RVA: 0x000077F8 File Offset: 0x000059F8
		public SavedGameModuleInfoVM(string definition, string seperator, string value)
		{
			this.Definition = definition;
			this.Seperator = seperator;
			this.Value = value;
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000135 RID: 309 RVA: 0x00007815 File Offset: 0x00005A15
		// (set) Token: 0x06000136 RID: 310 RVA: 0x0000781D File Offset: 0x00005A1D
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

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000137 RID: 311 RVA: 0x00007840 File Offset: 0x00005A40
		// (set) Token: 0x06000138 RID: 312 RVA: 0x00007848 File Offset: 0x00005A48
		[DataSourceProperty]
		public string Seperator
		{
			get
			{
				return this._seperator;
			}
			set
			{
				if (value != this._seperator)
				{
					this._seperator = value;
					base.OnPropertyChangedWithValue<string>(value, "Seperator");
				}
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000139 RID: 313 RVA: 0x0000786B File Offset: 0x00005A6B
		// (set) Token: 0x0600013A RID: 314 RVA: 0x00007873 File Offset: 0x00005A73
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

		// Token: 0x04000077 RID: 119
		private string _definition;

		// Token: 0x04000078 RID: 120
		private string _seperator;

		// Token: 0x04000079 RID: 121
		private string _value;
	}
}
