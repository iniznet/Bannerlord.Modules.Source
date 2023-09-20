using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class ControllerEquippedItemVM : EquipmentActionItemVM
	{
		public ControllerEquippedItemVM(string item, string itemTypeAsString, object identifier, HotKey key, Action<EquipmentActionItemVM> onSelection)
			: base(item, itemTypeAsString, identifier, onSelection, false)
		{
			if (key != null)
			{
				this.ShortcutKey = InputKeyItemVM.CreateFromHotKey(key, true);
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM shortcutKey = this.ShortcutKey;
			if (shortcutKey != null)
			{
				shortcutKey.OnFinalize();
			}
			this.ShortcutKey = null;
		}

		[DataSourceProperty]
		public InputKeyItemVM ShortcutKey
		{
			get
			{
				return this._shortcutKey;
			}
			set
			{
				if (value != this._shortcutKey)
				{
					this._shortcutKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ShortcutKey");
				}
			}
		}

		[DataSourceProperty]
		public float DropProgress
		{
			get
			{
				return this._dropProgress;
			}
			set
			{
				if (value != this._dropProgress)
				{
					this._dropProgress = value;
					base.OnPropertyChangedWithValue(value, "DropProgress");
				}
			}
		}

		private InputKeyItemVM _shortcutKey;

		private float _dropProgress;
	}
}
