using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	public class CharacterEquipmentItemVM : ViewModel
	{
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

		public virtual void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(ItemObject), new object[]
			{
				new EquipmentElement(this._item, null, null, false)
			});
		}

		public virtual void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

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

		private readonly ItemObject _item;

		private int _type;

		private bool _hasItem;
	}
}
