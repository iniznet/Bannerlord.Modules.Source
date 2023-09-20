using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
	// Token: 0x0200001D RID: 29
	public class TroopTypeSelectionPopUpVM : ViewModel
	{
		// Token: 0x06000177 RID: 375 RVA: 0x00009C5C File Offset: 0x00007E5C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
			this.CancelLbl = GameTexts.FindText("str_cancel", null).ToString();
			this.SelectAllLbl = GameTexts.FindText("str_custom_battle_select_all", null).ToString();
			this.BackToDefaultLbl = GameTexts.FindText("str_custom_battle_back_to_default", null).ToString();
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00009CC7 File Offset: 0x00007EC7
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.DoneInputKey.OnFinalize();
			this.CancelInputKey.OnFinalize();
			this.ResetInputKey.OnFinalize();
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00009CF0 File Offset: 0x00007EF0
		public void OpenPopUp(string title, MBBindingList<CustomBattleTroopTypeVM> troops)
		{
			this._itemSelectionsBackUp = new List<bool>();
			foreach (CustomBattleTroopTypeVM customBattleTroopTypeVM in troops)
			{
				this._itemSelectionsBackUp.Add(customBattleTroopTypeVM.IsSelected);
			}
			this._selectedItemCount = troops.Count((CustomBattleTroopTypeVM x) => x.IsSelected);
			this.Title = title;
			this.Items = troops;
			this.IsOpen = true;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00009D90 File Offset: 0x00007F90
		public void OnItemSelectionToggled(CustomBattleTroopTypeVM item)
		{
			if (this._selectedItemCount > 1 || !item.IsSelected)
			{
				item.IsSelected = !item.IsSelected;
				this._selectedItemCount += (item.IsSelected ? 1 : (-1));
			}
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00009DDC File Offset: 0x00007FDC
		public void ExecuteSelectAll()
		{
			this.Items.ApplyActionOnAllItems(delegate(CustomBattleTroopTypeVM x)
			{
				x.IsSelected = true;
			});
			this._selectedItemCount = this.Items.Count;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00009E1C File Offset: 0x0000801C
		public void ExecuteBackToDefault()
		{
			this.Items.ApplyActionOnAllItems(delegate(CustomBattleTroopTypeVM x)
			{
				x.IsSelected = x.IsDefault;
			});
			this._selectedItemCount = this.Items.Count((CustomBattleTroopTypeVM x) => x.IsSelected);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00009E83 File Offset: 0x00008083
		public void ExecuteCancel()
		{
			this.ExecuteReset();
			Action onPopUpClosed = this.OnPopUpClosed;
			if (onPopUpClosed != null)
			{
				onPopUpClosed();
			}
			this.IsOpen = false;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00009EA3 File Offset: 0x000080A3
		public void ExecuteDone()
		{
			this.IsOpen = false;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00009EAC File Offset: 0x000080AC
		public void ExecuteReset()
		{
			int count = this._itemSelectionsBackUp.Count;
			if (count != this.Items.Count)
			{
				Debug.FailedAssert("Backup troop count does not match with the actual troop count.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.CustomBattle\\CustomBattle\\TroopTypeSelectionPopUpVM.cs", "ExecuteReset", 100);
				return;
			}
			for (int i = 0; i < count; i++)
			{
				this.Items[i].IsSelected = this._itemSelectionsBackUp[i];
			}
			this._selectedItemCount = this.Items.Count((CustomBattleTroopTypeVM x) => x.IsSelected);
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00009F43 File Offset: 0x00008143
		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00009F52 File Offset: 0x00008152
		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00009F61 File Offset: 0x00008161
		public void SetResetInputKey(HotKey hotkey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000183 RID: 387 RVA: 0x00009F70 File Offset: 0x00008170
		// (set) Token: 0x06000184 RID: 388 RVA: 0x00009F78 File Offset: 0x00008178
		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00009F96 File Offset: 0x00008196
		// (set) Token: 0x06000186 RID: 390 RVA: 0x00009F9E File Offset: 0x0000819E
		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000187 RID: 391 RVA: 0x00009FBC File Offset: 0x000081BC
		// (set) Token: 0x06000188 RID: 392 RVA: 0x00009FC4 File Offset: 0x000081C4
		[DataSourceProperty]
		public InputKeyItemVM ResetInputKey
		{
			get
			{
				return this._resetInputKey;
			}
			set
			{
				if (value != this._resetInputKey)
				{
					this._resetInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ResetInputKey");
				}
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000189 RID: 393 RVA: 0x00009FE2 File Offset: 0x000081E2
		// (set) Token: 0x0600018A RID: 394 RVA: 0x00009FEA File Offset: 0x000081EA
		[DataSourceProperty]
		public MBBindingList<CustomBattleTroopTypeVM> Items
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
					base.OnPropertyChangedWithValue<MBBindingList<CustomBattleTroopTypeVM>>(value, "Items");
				}
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600018B RID: 395 RVA: 0x0000A008 File Offset: 0x00008208
		// (set) Token: 0x0600018C RID: 396 RVA: 0x0000A010 File Offset: 0x00008210
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

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600018D RID: 397 RVA: 0x0000A033 File Offset: 0x00008233
		// (set) Token: 0x0600018E RID: 398 RVA: 0x0000A03B File Offset: 0x0000823B
		[DataSourceProperty]
		public string DoneLbl
		{
			get
			{
				return this._doneLbl;
			}
			set
			{
				if (value != this._doneLbl)
				{
					this._doneLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneLbl");
				}
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600018F RID: 399 RVA: 0x0000A05E File Offset: 0x0000825E
		// (set) Token: 0x06000190 RID: 400 RVA: 0x0000A066 File Offset: 0x00008266
		[DataSourceProperty]
		public string CancelLbl
		{
			get
			{
				return this._cancelLbl;
			}
			set
			{
				if (value != this._cancelLbl)
				{
					this._cancelLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelLbl");
				}
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000191 RID: 401 RVA: 0x0000A089 File Offset: 0x00008289
		// (set) Token: 0x06000192 RID: 402 RVA: 0x0000A091 File Offset: 0x00008291
		[DataSourceProperty]
		public string SelectAllLbl
		{
			get
			{
				return this._selectAllLbl;
			}
			set
			{
				if (value != this._selectAllLbl)
				{
					this._selectAllLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectAllLbl");
				}
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000193 RID: 403 RVA: 0x0000A0B4 File Offset: 0x000082B4
		// (set) Token: 0x06000194 RID: 404 RVA: 0x0000A0BC File Offset: 0x000082BC
		[DataSourceProperty]
		public string BackToDefaultLbl
		{
			get
			{
				return this._backToDefaultLbl;
			}
			set
			{
				if (value != this._backToDefaultLbl)
				{
					this._backToDefaultLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "BackToDefaultLbl");
				}
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000195 RID: 405 RVA: 0x0000A0DF File Offset: 0x000082DF
		// (set) Token: 0x06000196 RID: 406 RVA: 0x0000A0E7 File Offset: 0x000082E7
		[DataSourceProperty]
		public bool IsOpen
		{
			get
			{
				return this._isOpen;
			}
			set
			{
				if (value != this._isOpen)
				{
					this._isOpen = value;
					base.OnPropertyChangedWithValue(value, "IsOpen");
				}
			}
		}

		// Token: 0x040000FE RID: 254
		public Action OnPopUpClosed;

		// Token: 0x040000FF RID: 255
		private List<bool> _itemSelectionsBackUp;

		// Token: 0x04000100 RID: 256
		private int _selectedItemCount;

		// Token: 0x04000101 RID: 257
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000102 RID: 258
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000103 RID: 259
		private InputKeyItemVM _resetInputKey;

		// Token: 0x04000104 RID: 260
		private MBBindingList<CustomBattleTroopTypeVM> _items;

		// Token: 0x04000105 RID: 261
		private string _title;

		// Token: 0x04000106 RID: 262
		private string _doneLbl;

		// Token: 0x04000107 RID: 263
		private string _cancelLbl;

		// Token: 0x04000108 RID: 264
		private string _selectAllLbl;

		// Token: 0x04000109 RID: 265
		private string _backToDefaultLbl;

		// Token: 0x0400010A RID: 266
		private bool _isOpen;
	}
}
