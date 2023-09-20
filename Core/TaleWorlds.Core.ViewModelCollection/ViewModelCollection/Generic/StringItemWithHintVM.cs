using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	// Token: 0x02000022 RID: 34
	public class StringItemWithHintVM : ViewModel
	{
		// Token: 0x0600018B RID: 395 RVA: 0x00005329 File Offset: 0x00003529
		public StringItemWithHintVM(string text, TextObject hint)
		{
			this.Text = text;
			this.Hint = new HintViewModel(hint, null);
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600018C RID: 396 RVA: 0x00005345 File Offset: 0x00003545
		// (set) Token: 0x0600018D RID: 397 RVA: 0x0000534D File Offset: 0x0000354D
		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600018E RID: 398 RVA: 0x00005370 File Offset: 0x00003570
		// (set) Token: 0x0600018F RID: 399 RVA: 0x00005378 File Offset: 0x00003578
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

		// Token: 0x040000A0 RID: 160
		private string _text;

		// Token: 0x040000A1 RID: 161
		private HintViewModel _hint;
	}
}
