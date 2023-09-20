using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticItem
{
	public class MPArmoryCosmeticTauntSlotVM : ViewModel
	{
		public static event Action<MPArmoryCosmeticTauntSlotVM, bool> OnFocusChanged;

		public static event Action<MPArmoryCosmeticTauntSlotVM> OnSelected;

		public static event Action<MPArmoryCosmeticTauntSlotVM> OnPreview;

		public static event Action<MPArmoryCosmeticTauntSlotVM, MPArmoryCosmeticTauntItemVM, bool> OnTauntEquipped;

		public MPArmoryCosmeticTauntSlotVM(int slotIndex)
		{
			this.IsEmpty = true;
			this.SlotIndex = slotIndex;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM selectKeyVisual = this.SelectKeyVisual;
			if (selectKeyVisual == null)
			{
				return;
			}
			selectKeyVisual.OnFinalize();
		}

		public void AssignTauntItem(MPArmoryCosmeticTauntItemVM tauntItem, bool isSwapping = false)
		{
			MPArmoryCosmeticTauntItemVM assignedTauntItem = this.AssignedTauntItem;
			this.AssignedTauntItem = tauntItem;
			this.IsEmpty = tauntItem == null;
			if (!isSwapping && assignedTauntItem != null)
			{
				assignedTauntItem.IsUsed = false;
			}
			if (this.AssignedTauntItem != null)
			{
				this.AssignedTauntItem.TauntCosmeticElement.UsageIndex = this.SlotIndex;
				this.AssignedTauntItem.IsUsed = true;
			}
			Action<MPArmoryCosmeticTauntSlotVM, MPArmoryCosmeticTauntItemVM, bool> onTauntEquipped = MPArmoryCosmeticTauntSlotVM.OnTauntEquipped;
			if (onTauntEquipped == null)
			{
				return;
			}
			onTauntEquipped(this, assignedTauntItem, isSwapping);
		}

		public void ExecuteSelect()
		{
			Action<MPArmoryCosmeticTauntSlotVM> onSelected = MPArmoryCosmeticTauntSlotVM.OnSelected;
			if (onSelected == null)
			{
				return;
			}
			onSelected(this);
		}

		public void ExecutePreview()
		{
			Action<MPArmoryCosmeticTauntSlotVM> onPreview = MPArmoryCosmeticTauntSlotVM.OnPreview;
			if (onPreview == null)
			{
				return;
			}
			onPreview(this);
		}

		public void ExecuteFocus()
		{
			Action<MPArmoryCosmeticTauntSlotVM, bool> onFocusChanged = MPArmoryCosmeticTauntSlotVM.OnFocusChanged;
			if (onFocusChanged == null)
			{
				return;
			}
			onFocusChanged(this, true);
		}

		public void ExecuteUnfocus()
		{
			Action<MPArmoryCosmeticTauntSlotVM, bool> onFocusChanged = MPArmoryCosmeticTauntSlotVM.OnFocusChanged;
			if (onFocusChanged == null)
			{
				return;
			}
			onFocusChanged(this, false);
		}

		public void SetSelectKeyVisual(HotKey hotKey)
		{
			this.SelectKeyVisual = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetEmptySlotKeyVisual(HotKey hotKey)
		{
			this.EmptySlotKeyVisual = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		[DataSourceProperty]
		public InputKeyItemVM SelectKeyVisual
		{
			get
			{
				return this._selectKeyVisual;
			}
			set
			{
				if (value != this._selectKeyVisual)
				{
					this._selectKeyVisual = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "SelectKeyVisual");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM EmptySlotKeyVisual
		{
			get
			{
				return this._emptySlotKeyVisual;
			}
			set
			{
				if (value != this._emptySlotKeyVisual)
				{
					this._emptySlotKeyVisual = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "EmptySlotKeyVisual");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAcceptingTaunts
		{
			get
			{
				return this._isAcceptingTaunts;
			}
			set
			{
				if (value != this._isAcceptingTaunts)
				{
					this._isAcceptingTaunts = value;
					base.OnPropertyChangedWithValue(value, "IsAcceptingTaunts");
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
				}
			}
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEmpty
		{
			get
			{
				return this._isEmpty;
			}
			set
			{
				if (value != this._isEmpty)
				{
					this._isEmpty = value;
					base.OnPropertyChangedWithValue(value, "IsEmpty");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFocused
		{
			get
			{
				return this._isFocused;
			}
			set
			{
				if (value != this._isFocused)
				{
					this._isFocused = value;
					base.OnPropertyChangedWithValue(value, "IsFocused");
				}
			}
		}

		[DataSourceProperty]
		public MPArmoryCosmeticTauntItemVM AssignedTauntItem
		{
			get
			{
				return this._assignedTauntItem;
			}
			set
			{
				if (value != this._assignedTauntItem)
				{
					this._assignedTauntItem = value;
					base.OnPropertyChangedWithValue<MPArmoryCosmeticTauntItemVM>(value, "AssignedTauntItem");
				}
			}
		}

		public readonly int SlotIndex;

		private InputKeyItemVM _selectKeyVisual;

		private InputKeyItemVM _emptySlotKeyVisual;

		private bool _isAcceptingTaunts;

		private bool _isSelected;

		private bool _isEnabled;

		private bool _isEmpty;

		private bool _isFocused;

		private MPArmoryCosmeticTauntItemVM _assignedTauntItem;
	}
}
