using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000E0 RID: 224
	public class ControllerEquippedItemVM : EquipmentActionItemVM
	{
		// Token: 0x06001482 RID: 5250 RVA: 0x00042FB8 File Offset: 0x000411B8
		public ControllerEquippedItemVM(string item, string itemTypeAsString, object identifier, HotKey key, Action<EquipmentActionItemVM> onSelection)
			: base(item, itemTypeAsString, identifier, onSelection, false)
		{
			if (key != null)
			{
				this.ShortcutKey = InputKeyItemVM.CreateFromHotKey(key, true);
			}
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x00042FD8 File Offset: 0x000411D8
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

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x06001484 RID: 5252 RVA: 0x00042FF8 File Offset: 0x000411F8
		// (set) Token: 0x06001485 RID: 5253 RVA: 0x00043000 File Offset: 0x00041200
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

		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x06001486 RID: 5254 RVA: 0x0004301E File Offset: 0x0004121E
		// (set) Token: 0x06001487 RID: 5255 RVA: 0x00043026 File Offset: 0x00041226
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

		// Token: 0x040009D0 RID: 2512
		private InputKeyItemVM _shortcutKey;

		// Token: 0x040009D1 RID: 2513
		private float _dropProgress;
	}
}
