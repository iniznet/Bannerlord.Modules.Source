using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class EquipmentActionItemVM : ViewModel
	{
		public EquipmentActionItemVM(string item, string itemTypeAsString, object identifier, Action<EquipmentActionItemVM> onSelection, bool isCurrentlyWielded = false)
		{
			this.Identifier = identifier;
			this.ActionText = item;
			this.TypeAsString = itemTypeAsString;
			this.IsWielded = isCurrentlyWielded;
			this._onSelection = onSelection;
		}

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

		private readonly Action<EquipmentActionItemVM> _onSelection;

		public object Identifier;

		private string _actionText;

		private string _typeAsString;

		private bool _isSelected;

		private bool _isWielded;
	}
}
