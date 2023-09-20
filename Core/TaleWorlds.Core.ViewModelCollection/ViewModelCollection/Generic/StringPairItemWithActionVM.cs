using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	// Token: 0x02000024 RID: 36
	public class StringPairItemWithActionVM : ViewModel
	{
		// Token: 0x06000197 RID: 407 RVA: 0x0000542F File Offset: 0x0000362F
		public StringPairItemWithActionVM(Action<object> onExecute, string definition, string value, object identifier)
		{
			this._onExecute = onExecute;
			this.Identifier = identifier;
			this.Definition = definition;
			this.Value = value;
			this.Hint = new HintViewModel();
			this.IsEnabled = true;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00005466 File Offset: 0x00003666
		public void ExecuteAction()
		{
			this._onExecute(this.Identifier);
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000199 RID: 409 RVA: 0x00005479 File Offset: 0x00003679
		// (set) Token: 0x0600019A RID: 410 RVA: 0x00005481 File Offset: 0x00003681
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

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600019B RID: 411 RVA: 0x000054A4 File Offset: 0x000036A4
		// (set) Token: 0x0600019C RID: 412 RVA: 0x000054AC File Offset: 0x000036AC
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

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x0600019D RID: 413 RVA: 0x000054CF File Offset: 0x000036CF
		// (set) Token: 0x0600019E RID: 414 RVA: 0x000054D7 File Offset: 0x000036D7
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

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x0600019F RID: 415 RVA: 0x000054F5 File Offset: 0x000036F5
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x000054FD File Offset: 0x000036FD
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x040000A5 RID: 165
		public object Identifier;

		// Token: 0x040000A6 RID: 166
		protected Action<object> _onExecute;

		// Token: 0x040000A7 RID: 167
		private string _definition;

		// Token: 0x040000A8 RID: 168
		private string _value;

		// Token: 0x040000A9 RID: 169
		private HintViewModel _hint;

		// Token: 0x040000AA RID: 170
		private bool _isEnabled;
	}
}
