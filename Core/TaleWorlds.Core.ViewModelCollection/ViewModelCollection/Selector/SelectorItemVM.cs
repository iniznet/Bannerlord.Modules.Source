using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Selector
{
	// Token: 0x02000011 RID: 17
	public class SelectorItemVM : ViewModel
	{
		// Token: 0x060000BA RID: 186 RVA: 0x0000326A File Offset: 0x0000146A
		public SelectorItemVM(TextObject s)
		{
			this._s = s;
			this.RefreshValues();
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00003286 File Offset: 0x00001486
		public SelectorItemVM(string s)
		{
			this._stringItem = s;
			this.RefreshValues();
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000032A2 File Offset: 0x000014A2
		public SelectorItemVM(TextObject s, TextObject hint)
		{
			this._s = s;
			this._hintObj = hint;
			this.RefreshValues();
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000032C5 File Offset: 0x000014C5
		public SelectorItemVM(string s, TextObject hint)
		{
			this._stringItem = s;
			this._hintObj = hint;
			this.RefreshValues();
		}

		// Token: 0x060000BE RID: 190 RVA: 0x000032E8 File Offset: 0x000014E8
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._s != null)
			{
				this._stringItem = this._s.ToString();
			}
			if (this._hintObj != null)
			{
				if (this._hint == null)
				{
					this._hint = new HintViewModel(this._hintObj, null);
					return;
				}
				this._hint.HintText = this._hintObj;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000BF RID: 191 RVA: 0x00003348 File Offset: 0x00001548
		// (set) Token: 0x060000C0 RID: 192 RVA: 0x00003350 File Offset: 0x00001550
		[DataSourceProperty]
		public string StringItem
		{
			get
			{
				return this._stringItem;
			}
			set
			{
				if (value != this._stringItem)
				{
					this._stringItem = value;
					base.OnPropertyChangedWithValue<string>(value, "StringItem");
				}
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x00003373 File Offset: 0x00001573
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x0000337B File Offset: 0x0000157B
		[DataSourceProperty]
		public bool CanBeSelected
		{
			get
			{
				return this._canBeSelected;
			}
			set
			{
				if (value != this._canBeSelected)
				{
					this._canBeSelected = value;
					base.OnPropertyChangedWithValue(value, "CanBeSelected");
				}
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00003399 File Offset: 0x00001599
		// (set) Token: 0x060000C4 RID: 196 RVA: 0x000033A1 File Offset: 0x000015A1
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

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x000033BF File Offset: 0x000015BF
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x000033C7 File Offset: 0x000015C7
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x04000049 RID: 73
		private TextObject _s;

		// Token: 0x0400004A RID: 74
		private TextObject _hintObj;

		// Token: 0x0400004B RID: 75
		private string _stringItem;

		// Token: 0x0400004C RID: 76
		private HintViewModel _hint;

		// Token: 0x0400004D RID: 77
		private bool _canBeSelected = true;

		// Token: 0x0400004E RID: 78
		private bool _isSelected;
	}
}
