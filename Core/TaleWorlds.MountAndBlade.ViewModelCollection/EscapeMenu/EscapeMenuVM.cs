using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu
{
	public class EscapeMenuVM : ViewModel
	{
		public EscapeMenuVM(IEnumerable<EscapeMenuItemVM> items, TextObject title = null)
		{
			this._titleObj = title;
			this.MenuItems = new MBBindingList<EscapeMenuItemVM>();
			if (items != null)
			{
				foreach (EscapeMenuItemVM escapeMenuItemVM in items)
				{
					this.MenuItems.Add(escapeMenuItemVM);
				}
			}
			this.Tips = new GameTipsVM(true, true);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject titleObj = this._titleObj;
			this.Title = ((titleObj != null) ? titleObj.ToString() : null) ?? "";
			this.MenuItems.ApplyActionOnAllItems(delegate(EscapeMenuItemVM x)
			{
				x.RefreshValues();
			});
			this.Tips.RefreshValues();
		}

		public virtual void Tick(float dt)
		{
		}

		public void RefreshItems(IEnumerable<EscapeMenuItemVM> items)
		{
			this.MenuItems.Clear();
			foreach (EscapeMenuItemVM escapeMenuItemVM in items)
			{
				this.MenuItems.Add(escapeMenuItemVM);
			}
		}

		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<EscapeMenuItemVM> MenuItems
		{
			get
			{
				return this._menuItems;
			}
			set
			{
				if (value != this._menuItems)
				{
					this._menuItems = value;
					base.OnPropertyChangedWithValue<MBBindingList<EscapeMenuItemVM>>(value, "MenuItems");
				}
			}
		}

		[DataSourceProperty]
		public GameTipsVM Tips
		{
			get
			{
				return this._tips;
			}
			set
			{
				if (value != this._tips)
				{
					this._tips = value;
					base.OnPropertyChangedWithValue<GameTipsVM>(value, "Tips");
				}
			}
		}

		private readonly TextObject _titleObj;

		private string _title;

		private MBBindingList<EscapeMenuItemVM> _menuItems;

		private GameTipsVM _tips;
	}
}
