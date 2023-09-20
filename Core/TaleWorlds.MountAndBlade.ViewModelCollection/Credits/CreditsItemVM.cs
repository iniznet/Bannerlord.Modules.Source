using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Credits
{
	// Token: 0x0200010B RID: 267
	public class CreditsItemVM : ViewModel
	{
		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x060017E8 RID: 6120 RVA: 0x0004F160 File Offset: 0x0004D360
		// (set) Token: 0x060017E9 RID: 6121 RVA: 0x0004F168 File Offset: 0x0004D368
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

		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x060017EA RID: 6122 RVA: 0x0004F18B File Offset: 0x0004D38B
		// (set) Token: 0x060017EB RID: 6123 RVA: 0x0004F193 File Offset: 0x0004D393
		[DataSourceProperty]
		public string Type
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
					base.OnPropertyChangedWithValue<string>(value, "Type");
				}
			}
		}

		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x060017EC RID: 6124 RVA: 0x0004F1B6 File Offset: 0x0004D3B6
		// (set) Token: 0x060017ED RID: 6125 RVA: 0x0004F1BE File Offset: 0x0004D3BE
		[DataSourceProperty]
		public MBBindingList<CreditsItemVM> Items
		{
			get
			{
				return this._items;
			}
			set
			{
				if (value != this._items)
				{
					this._items = value;
					base.OnPropertyChangedWithValue<MBBindingList<CreditsItemVM>>(value, "Items");
				}
			}
		}

		// Token: 0x060017EE RID: 6126 RVA: 0x0004F1DC File Offset: 0x0004D3DC
		public CreditsItemVM()
		{
			this._items = new MBBindingList<CreditsItemVM>();
			this.Type = "Entry";
			this.Text = "";
		}

		// Token: 0x04000B72 RID: 2930
		private string _text;

		// Token: 0x04000B73 RID: 2931
		private string _type;

		// Token: 0x04000B74 RID: 2932
		private MBBindingList<CreditsItemVM> _items;
	}
}
