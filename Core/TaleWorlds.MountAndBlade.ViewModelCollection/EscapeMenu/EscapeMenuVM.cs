using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu
{
	// Token: 0x02000109 RID: 265
	public class EscapeMenuVM : ViewModel
	{
		// Token: 0x060017D3 RID: 6099 RVA: 0x0004ECD4 File Offset: 0x0004CED4
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

		// Token: 0x060017D4 RID: 6100 RVA: 0x0004ED50 File Offset: 0x0004CF50
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

		// Token: 0x060017D5 RID: 6101 RVA: 0x0004EDB9 File Offset: 0x0004CFB9
		public virtual void Tick(float dt)
		{
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x0004EDBC File Offset: 0x0004CFBC
		public void RefreshItems(IEnumerable<EscapeMenuItemVM> items)
		{
			this.MenuItems.Clear();
			foreach (EscapeMenuItemVM escapeMenuItemVM in items)
			{
				this.MenuItems.Add(escapeMenuItemVM);
			}
		}

		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x060017D7 RID: 6103 RVA: 0x0004EE14 File Offset: 0x0004D014
		// (set) Token: 0x060017D8 RID: 6104 RVA: 0x0004EE1C File Offset: 0x0004D01C
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

		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x060017D9 RID: 6105 RVA: 0x0004EE3F File Offset: 0x0004D03F
		// (set) Token: 0x060017DA RID: 6106 RVA: 0x0004EE47 File Offset: 0x0004D047
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

		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x060017DB RID: 6107 RVA: 0x0004EE65 File Offset: 0x0004D065
		// (set) Token: 0x060017DC RID: 6108 RVA: 0x0004EE6D File Offset: 0x0004D06D
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

		// Token: 0x04000B66 RID: 2918
		private readonly TextObject _titleObj;

		// Token: 0x04000B67 RID: 2919
		private string _title;

		// Token: 0x04000B68 RID: 2920
		private MBBindingList<EscapeMenuItemVM> _menuItems;

		// Token: 0x04000B69 RID: 2921
		private GameTipsVM _tips;
	}
}
