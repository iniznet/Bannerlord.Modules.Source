using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Credits
{
	public class CreditsItemVM : ViewModel
	{
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

		public CreditsItemVM()
		{
			this._items = new MBBindingList<CreditsItemVM>();
			this.Type = "Entry";
			this.Text = "";
		}

		private string _text;

		private string _type;

		private MBBindingList<CreditsItemVM> _items;
	}
}
