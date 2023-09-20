using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	public class ClanCardSelectionPopupVM : ViewModel
	{
		public ClanCardSelectionPopupVM()
		{
			this._titleText = TextObject.Empty;
			this.Items = new MBBindingList<ClanCardSelectionPopupItemVM>();
		}

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

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

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

		public void ExecuteCancel()
		{
			Action<List<object>, Action> onClosed = this._onClosed;
			if (onClosed != null)
			{
				onClosed(new List<object>(), null);
			}
			this.Close();
		}

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

		private TextObject _titleText;

		private bool _isMultiSelection;

		private ClanCardSelectionPopupItemVM _lastSelectedItem;

		private Action<List<object>, Action> _onClosed;

		private MBBindingList<ClanCardSelectionPopupItemVM> _items;

		private InputKeyItemVM _doneInputKey;

		private string _title;

		private string _actionResult;

		private string _doneLbl;

		private bool _isVisible;
	}
}
