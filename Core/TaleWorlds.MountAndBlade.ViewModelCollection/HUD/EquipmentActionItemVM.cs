using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000E2 RID: 226
	public class EquipmentActionItemVM : ViewModel
	{
		// Token: 0x060014A6 RID: 5286 RVA: 0x00043A53 File Offset: 0x00041C53
		public EquipmentActionItemVM(string item, string itemTypeAsString, object identifier, Action<EquipmentActionItemVM> onSelection, bool isCurrentlyWielded = false)
		{
			this.Identifier = identifier;
			this.ActionText = item;
			this.TypeAsString = itemTypeAsString;
			this.IsWielded = isCurrentlyWielded;
			this._onSelection = onSelection;
		}

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x060014A7 RID: 5287 RVA: 0x00043A80 File Offset: 0x00041C80
		// (set) Token: 0x060014A8 RID: 5288 RVA: 0x00043A88 File Offset: 0x00041C88
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

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x060014A9 RID: 5289 RVA: 0x00043AAB File Offset: 0x00041CAB
		// (set) Token: 0x060014AA RID: 5290 RVA: 0x00043AB3 File Offset: 0x00041CB3
		[DataSourceProperty]
		public bool IsWielded
		{
			get
			{
				return this._isWielded;
			}
			set
			{
				if (value != this._isWielded)
				{
					this._isWielded = value;
					base.OnPropertyChangedWithValue(value, "IsWielded");
				}
			}
		}

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x060014AB RID: 5291 RVA: 0x00043AD1 File Offset: 0x00041CD1
		// (set) Token: 0x060014AC RID: 5292 RVA: 0x00043AD9 File Offset: 0x00041CD9
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
					if (value)
					{
						this._onSelection(this);
					}
				}
			}
		}

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x060014AD RID: 5293 RVA: 0x00043B06 File Offset: 0x00041D06
		// (set) Token: 0x060014AE RID: 5294 RVA: 0x00043B0E File Offset: 0x00041D0E
		[DataSourceProperty]
		public string TypeAsString
		{
			get
			{
				return this._typeAsString;
			}
			set
			{
				if (value != this._typeAsString)
				{
					this._typeAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "TypeAsString");
				}
			}
		}

		// Token: 0x040009E0 RID: 2528
		private readonly Action<EquipmentActionItemVM> _onSelection;

		// Token: 0x040009E1 RID: 2529
		public object Identifier;

		// Token: 0x040009E2 RID: 2530
		private string _actionText;

		// Token: 0x040009E3 RID: 2531
		private string _typeAsString;

		// Token: 0x040009E4 RID: 2532
		private bool _isSelected;

		// Token: 0x040009E5 RID: 2533
		private bool _isWielded;
	}
}
