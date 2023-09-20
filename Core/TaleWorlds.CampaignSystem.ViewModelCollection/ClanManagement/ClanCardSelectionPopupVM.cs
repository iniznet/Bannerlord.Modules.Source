using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x020000FD RID: 253
	public class ClanCardSelectionPopupVM : ViewModel
	{
		// Token: 0x0600177E RID: 6014 RVA: 0x00056AEC File Offset: 0x00054CEC
		public ClanCardSelectionPopupVM()
		{
			this._titleText = TextObject.Empty;
			this.Items = new MBBindingList<ClanCardSelectionPopupItemVM>();
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x00056B0C File Offset: 0x00054D0C
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (!this._isMultiSelection)
			{
				ClanCardSelectionPopupItemVM lastSelectedItem = this._lastSelectedItem;
				string text;
				if (lastSelectedItem == null)
				{
					text = null;
				}
				else
				{
					TextObject actionResultText = lastSelectedItem.ActionResultText;
					text = ((actionResultText != null) ? actionResultText.ToString() : null);
				}
				this.ActionResult = text ?? string.Empty;
			}
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
			TextObject titleText = this._titleText;
			this.Title = ((titleText != null) ? titleText.ToString() : null) ?? string.Empty;
			this.Items.ApplyActionOnAllItems(delegate(ClanCardSelectionPopupItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x00056BB5 File Offset: 0x00054DB5
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey == null)
			{
				return;
			}
			doneInputKey.OnFinalize();
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x00056BCD File Offset: 0x00054DCD
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x00056BDC File Offset: 0x00054DDC
		public void Open(ClanCardSelectionInfo info)
		{
			this._isMultiSelection = info.IsMultiSelection;
			this._titleText = info.Title;
			this._onClosed = info.OnClosedAction;
			foreach (ClanCardSelectionItemInfo clanCardSelectionItemInfo in info.Items)
			{
				this.Items.Add(new ClanCardSelectionPopupItemVM(clanCardSelectionItemInfo, new Action<ClanCardSelectionPopupItemVM>(this.OnItemSelected)));
			}
			this.RefreshValues();
			this.IsVisible = true;
		}

		// Token: 0x06001783 RID: 6019 RVA: 0x00056C74 File Offset: 0x00054E74
		public void ExecuteCancel()
		{
			Action<List<object>, Action> onClosed = this._onClosed;
			if (onClosed != null)
			{
				onClosed(new List<object>(), null);
			}
			this.Close();
		}

		// Token: 0x06001784 RID: 6020 RVA: 0x00056C94 File Offset: 0x00054E94
		public void ExecuteDone()
		{
			List<object> selectedItems = new List<object>();
			this.Items.ApplyActionOnAllItems(delegate(ClanCardSelectionPopupItemVM x)
			{
				if (x.IsSelected)
				{
					selectedItems.Add(x.Identifier);
				}
			});
			Action<List<object>, Action> onClosed = this._onClosed;
			if (onClosed == null)
			{
				return;
			}
			onClosed(selectedItems, new Action(this.Close));
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x00056CEC File Offset: 0x00054EEC
		private void Close()
		{
			this.IsVisible = false;
			this._lastSelectedItem = null;
			this._titleText = TextObject.Empty;
			this.ActionResult = string.Empty;
			this.Title = string.Empty;
			this._onClosed = null;
			this.Items.Clear();
		}

		// Token: 0x06001786 RID: 6022 RVA: 0x00056D3C File Offset: 0x00054F3C
		private void OnItemSelected(ClanCardSelectionPopupItemVM item)
		{
			if (this._isMultiSelection)
			{
				item.IsSelected = !item.IsSelected;
			}
			else if (item != this._lastSelectedItem)
			{
				if (this._lastSelectedItem != null)
				{
					this._lastSelectedItem.IsSelected = false;
				}
				item.IsSelected = true;
				TextObject actionResultText = item.ActionResultText;
				this.ActionResult = ((actionResultText != null) ? actionResultText.ToString() : null) ?? string.Empty;
			}
			this._lastSelectedItem = item;
		}

		// Token: 0x170007F8 RID: 2040
		// (get) Token: 0x06001787 RID: 6023 RVA: 0x00056DAE File Offset: 0x00054FAE
		// (set) Token: 0x06001788 RID: 6024 RVA: 0x00056DB6 File Offset: 0x00054FB6
		[DataSourceProperty]
		public MBBindingList<ClanCardSelectionPopupItemVM> Items
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
					base.OnPropertyChangedWithValue<MBBindingList<ClanCardSelectionPopupItemVM>>(value, "Items");
				}
			}
		}

		// Token: 0x170007F9 RID: 2041
		// (get) Token: 0x06001789 RID: 6025 RVA: 0x00056DD4 File Offset: 0x00054FD4
		// (set) Token: 0x0600178A RID: 6026 RVA: 0x00056DDC File Offset: 0x00054FDC
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

		// Token: 0x170007FA RID: 2042
		// (get) Token: 0x0600178B RID: 6027 RVA: 0x00056DFA File Offset: 0x00054FFA
		// (set) Token: 0x0600178C RID: 6028 RVA: 0x00056E02 File Offset: 0x00055002
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

		// Token: 0x170007FB RID: 2043
		// (get) Token: 0x0600178D RID: 6029 RVA: 0x00056E25 File Offset: 0x00055025
		// (set) Token: 0x0600178E RID: 6030 RVA: 0x00056E2D File Offset: 0x0005502D
		[DataSourceProperty]
		public string ActionResult
		{
			get
			{
				return this._actionResult;
			}
			set
			{
				if (value != this._actionResult)
				{
					this._actionResult = value;
					base.OnPropertyChangedWithValue<string>(value, "ActionResult");
				}
			}
		}

		// Token: 0x170007FC RID: 2044
		// (get) Token: 0x0600178F RID: 6031 RVA: 0x00056E50 File Offset: 0x00055050
		// (set) Token: 0x06001790 RID: 6032 RVA: 0x00056E58 File Offset: 0x00055058
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

		// Token: 0x170007FD RID: 2045
		// (get) Token: 0x06001791 RID: 6033 RVA: 0x00056E7B File Offset: 0x0005507B
		// (set) Token: 0x06001792 RID: 6034 RVA: 0x00056E83 File Offset: 0x00055083
		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (value != this._isVisible)
				{
					this._isVisible = value;
					base.OnPropertyChangedWithValue(value, "IsVisible");
				}
			}
		}

		// Token: 0x04000B1C RID: 2844
		private TextObject _titleText;

		// Token: 0x04000B1D RID: 2845
		private bool _isMultiSelection;

		// Token: 0x04000B1E RID: 2846
		private ClanCardSelectionPopupItemVM _lastSelectedItem;

		// Token: 0x04000B1F RID: 2847
		private Action<List<object>, Action> _onClosed;

		// Token: 0x04000B20 RID: 2848
		private MBBindingList<ClanCardSelectionPopupItemVM> _items;

		// Token: 0x04000B21 RID: 2849
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000B22 RID: 2850
		private string _title;

		// Token: 0x04000B23 RID: 2851
		private string _actionResult;

		// Token: 0x04000B24 RID: 2852
		private string _doneLbl;

		// Token: 0x04000B25 RID: 2853
		private bool _isVisible;
	}
}
