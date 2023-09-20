using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000DE RID: 222
	public class CheerBarkNodeItemVM : ViewModel
	{
		// Token: 0x06001456 RID: 5206 RVA: 0x0004280B File Offset: 0x00040A0B
		public CheerBarkNodeItemVM(TextObject nodeName, string nodeId, HotKey key, Action<CheerBarkNodeItemVM> onSelection, bool consoleOnlyShortcut = false)
		{
			this.SubNodes = new MBBindingList<CheerBarkNodeItemVM>();
			this._nodeName = nodeName;
			this.TypeAsString = nodeId;
			this._onSelection = onSelection;
			if (key != null)
			{
				this.ShortcutKey = InputKeyItemVM.CreateFromHotKey(key, consoleOnlyShortcut);
			}
			this.RefreshValues();
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x0004284B File Offset: 0x00040A4B
		public override void RefreshValues()
		{
			this.CheerNameText = this._nodeName.ToString();
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x0004285E File Offset: 0x00040A5E
		public void AddSubNode(CheerBarkNodeItemVM subNode)
		{
			this.SubNodes.Add(subNode);
			this.HasSubNodes = true;
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x00042874 File Offset: 0x00040A74
		public override void OnFinalize()
		{
			base.OnFinalize();
			MBBindingList<CheerBarkNodeItemVM> subNodes = this.SubNodes;
			if (subNodes != null)
			{
				subNodes.ApplyActionOnAllItems(delegate(CheerBarkNodeItemVM n)
				{
					n.OnFinalize();
				});
			}
			InputKeyItemVM shortcutKey = this.ShortcutKey;
			if (shortcutKey != null)
			{
				shortcutKey.OnFinalize();
			}
			this.ShortcutKey = null;
		}

		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x0600145A RID: 5210 RVA: 0x000428CF File Offset: 0x00040ACF
		// (set) Token: 0x0600145B RID: 5211 RVA: 0x000428D7 File Offset: 0x00040AD7
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

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x0600145C RID: 5212 RVA: 0x000428F5 File Offset: 0x00040AF5
		// (set) Token: 0x0600145D RID: 5213 RVA: 0x000428FD File Offset: 0x00040AFD
		[DataSourceProperty]
		public MBBindingList<CheerBarkNodeItemVM> SubNodes
		{
			get
			{
				return this._subNodes;
			}
			set
			{
				if (value != this._subNodes)
				{
					this._subNodes = value;
					base.OnPropertyChangedWithValue<MBBindingList<CheerBarkNodeItemVM>>(value, "SubNodes");
				}
			}
		}

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x0600145E RID: 5214 RVA: 0x0004291B File Offset: 0x00040B1B
		// (set) Token: 0x0600145F RID: 5215 RVA: 0x00042923 File Offset: 0x00040B23
		[DataSourceProperty]
		public string CheerNameText
		{
			get
			{
				return this._cheerNameText;
			}
			set
			{
				if (value != this._cheerNameText)
				{
					this._cheerNameText = value;
					base.OnPropertyChangedWithValue<string>(value, "CheerNameText");
				}
			}
		}

		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x06001460 RID: 5216 RVA: 0x00042946 File Offset: 0x00040B46
		// (set) Token: 0x06001461 RID: 5217 RVA: 0x0004294E File Offset: 0x00040B4E
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
					if (value && this._onSelection != null)
					{
						this._onSelection(this);
					}
				}
			}
		}

		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x06001462 RID: 5218 RVA: 0x00042983 File Offset: 0x00040B83
		// (set) Token: 0x06001463 RID: 5219 RVA: 0x0004298B File Offset: 0x00040B8B
		[DataSourceProperty]
		public bool HasSubNodes
		{
			get
			{
				return this._hasSubNodes;
			}
			set
			{
				if (value != this._hasSubNodes)
				{
					this._hasSubNodes = value;
					base.OnPropertyChanged("HasSubNodes");
				}
			}
		}

		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x06001464 RID: 5220 RVA: 0x000429A8 File Offset: 0x00040BA8
		// (set) Token: 0x06001465 RID: 5221 RVA: 0x000429B0 File Offset: 0x00040BB0
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

		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x06001466 RID: 5222 RVA: 0x000429D3 File Offset: 0x00040BD3
		// (set) Token: 0x06001467 RID: 5223 RVA: 0x000429DB File Offset: 0x00040BDB
		[DataSourceProperty]
		public string SelectedNodeText
		{
			get
			{
				return this._selectedNodeText;
			}
			set
			{
				if (value != this._selectedNodeText)
				{
					this._selectedNodeText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectedNodeText");
				}
			}
		}

		// Token: 0x040009C0 RID: 2496
		private readonly Action<CheerBarkNodeItemVM> _onSelection;

		// Token: 0x040009C1 RID: 2497
		private readonly TextObject _nodeName;

		// Token: 0x040009C2 RID: 2498
		private MBBindingList<CheerBarkNodeItemVM> _subNodes;

		// Token: 0x040009C3 RID: 2499
		private string _cheerNameText;

		// Token: 0x040009C4 RID: 2500
		private string _typeAsString;

		// Token: 0x040009C5 RID: 2501
		private string _selectedNodeText;

		// Token: 0x040009C6 RID: 2502
		private bool _isSelected;

		// Token: 0x040009C7 RID: 2503
		private bool _hasSubNodes;

		// Token: 0x040009C8 RID: 2504
		private InputKeyItemVM _shortcutKey;
	}
}
