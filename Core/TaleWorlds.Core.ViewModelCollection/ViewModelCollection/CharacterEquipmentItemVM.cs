using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	// Token: 0x02000008 RID: 8
	public class CharacterEquipmentItemVM : ViewModel
	{
		// Token: 0x0600002B RID: 43 RVA: 0x000022BC File Offset: 0x000004BC
		public CharacterEquipmentItemVM(ItemObject item)
		{
			this._item = item;
			if (this._item == null)
			{
				this.HasItem = false;
				this.Type = 0;
				return;
			}
			this.HasItem = true;
			this.Type = (int)this._item.Type;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000022FA File Offset: 0x000004FA
		public virtual void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(ItemObject), new object[]
			{
				new EquipmentElement(this._item, null, null, false)
			});
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002327 File Offset: 0x00000527
		public virtual void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002E RID: 46 RVA: 0x0000232E File Offset: 0x0000052E
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002336 File Offset: 0x00000536
		[DataSourceProperty]
		public int Type
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
					base.OnPropertyChangedWithValue(value, "Type");
				}
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002354 File Offset: 0x00000554
		// (set) Token: 0x06000031 RID: 49 RVA: 0x0000235C File Offset: 0x0000055C
		[DataSourceProperty]
		public bool HasItem
		{
			get
			{
				return this._hasItem;
			}
			set
			{
				if (value != this._hasItem)
				{
					this._hasItem = value;
					base.OnPropertyChangedWithValue(value, "HasItem");
				}
			}
		}

		// Token: 0x04000006 RID: 6
		private readonly ItemObject _item;

		// Token: 0x04000007 RID: 7
		private int _type;

		// Token: 0x04000008 RID: 8
		private bool _hasItem;
	}
}
