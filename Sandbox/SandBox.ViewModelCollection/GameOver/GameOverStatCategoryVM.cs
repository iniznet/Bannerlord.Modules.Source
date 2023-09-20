using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.GameOver
{
	// Token: 0x02000035 RID: 53
	public class GameOverStatCategoryVM : ViewModel
	{
		// Token: 0x06000401 RID: 1025 RVA: 0x00012525 File Offset: 0x00010725
		public GameOverStatCategoryVM(StatCategory category, Action<GameOverStatCategoryVM> onSelect)
		{
			this._category = category;
			this._onSelect = onSelect;
			this.Items = new MBBindingList<GameOverStatItemVM>();
			this.ID = category.ID;
			this.RefreshValues();
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x00012558 File Offset: 0x00010758
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Items.Clear();
			this.Name = GameTexts.FindText("str_game_over_stat_category", this._category.ID).ToString();
			foreach (StatItem statItem in this._category.Items)
			{
				this.Items.Add(new GameOverStatItemVM(statItem));
			}
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x000125E8 File Offset: 0x000107E8
		public void ExecuteSelectCategory()
		{
			Action<GameOverStatCategoryVM> onSelect = this._onSelect;
			if (onSelect == null)
			{
				return;
			}
			Common.DynamicInvokeWithLog(onSelect, new object[] { this });
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000404 RID: 1028 RVA: 0x00012605 File Offset: 0x00010805
		// (set) Token: 0x06000405 RID: 1029 RVA: 0x0001260D File Offset: 0x0001080D
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000406 RID: 1030 RVA: 0x00012630 File Offset: 0x00010830
		// (set) Token: 0x06000407 RID: 1031 RVA: 0x00012638 File Offset: 0x00010838
		[DataSourceProperty]
		public string ID
		{
			get
			{
				return this._id;
			}
			set
			{
				if (value != this._id)
				{
					this._id = value;
					base.OnPropertyChangedWithValue<string>(value, "ID");
				}
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000408 RID: 1032 RVA: 0x0001265B File Offset: 0x0001085B
		// (set) Token: 0x06000409 RID: 1033 RVA: 0x00012663 File Offset: 0x00010863
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

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x0600040A RID: 1034 RVA: 0x00012681 File Offset: 0x00010881
		// (set) Token: 0x0600040B RID: 1035 RVA: 0x00012689 File Offset: 0x00010889
		[DataSourceProperty]
		public MBBindingList<GameOverStatItemVM> Items
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
					base.OnPropertyChangedWithValue<MBBindingList<GameOverStatItemVM>>(value, "Items");
				}
			}
		}

		// Token: 0x04000213 RID: 531
		private readonly StatCategory _category;

		// Token: 0x04000214 RID: 532
		private readonly Action<GameOverStatCategoryVM> _onSelect;

		// Token: 0x04000215 RID: 533
		private string _name;

		// Token: 0x04000216 RID: 534
		private string _id;

		// Token: 0x04000217 RID: 535
		private bool _isSelected;

		// Token: 0x04000218 RID: 536
		private MBBindingList<GameOverStatItemVM> _items;
	}
}
