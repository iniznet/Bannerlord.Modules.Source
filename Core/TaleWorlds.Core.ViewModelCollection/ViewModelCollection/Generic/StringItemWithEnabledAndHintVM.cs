using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	// Token: 0x02000021 RID: 33
	public class StringItemWithEnabledAndHintVM : ViewModel
	{
		// Token: 0x06000183 RID: 387 RVA: 0x0000525B File Offset: 0x0000345B
		public StringItemWithEnabledAndHintVM(Action<object> onExecute, string item, bool enabled, object identifier, TextObject hintText = null)
		{
			this._onExecute = onExecute;
			this.Identifier = identifier;
			this.ActionText = item;
			this.IsEnabled = enabled;
			this.Hint = new HintViewModel(hintText ?? TextObject.Empty, null);
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00005297 File Offset: 0x00003497
		public void ExecuteAction()
		{
			if (this.IsEnabled)
			{
				this._onExecute(this.Identifier);
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000185 RID: 389 RVA: 0x000052B2 File Offset: 0x000034B2
		// (set) Token: 0x06000186 RID: 390 RVA: 0x000052BA File Offset: 0x000034BA
		[DataSourceProperty]
		public string ActionText
		{
			get
			{
				return this._actionText;
			}
			set
			{
				if (value != this._actionText)
				{
					this._actionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ActionText");
				}
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000187 RID: 391 RVA: 0x000052DD File Offset: 0x000034DD
		// (set) Token: 0x06000188 RID: 392 RVA: 0x000052E5 File Offset: 0x000034E5
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

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000189 RID: 393 RVA: 0x00005303 File Offset: 0x00003503
		// (set) Token: 0x0600018A RID: 394 RVA: 0x0000530B File Offset: 0x0000350B
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

		// Token: 0x0400009B RID: 155
		public object Identifier;

		// Token: 0x0400009C RID: 156
		protected Action<object> _onExecute;

		// Token: 0x0400009D RID: 157
		private HintViewModel _hint;

		// Token: 0x0400009E RID: 158
		private string _actionText;

		// Token: 0x0400009F RID: 159
		private bool _isEnabled;
	}
}
