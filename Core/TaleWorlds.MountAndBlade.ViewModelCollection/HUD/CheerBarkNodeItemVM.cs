using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class CheerBarkNodeItemVM : ViewModel
	{
		internal static event Action<CheerBarkNodeItemVM> OnSelection;

		internal static event Action<CheerBarkNodeItemVM> OnNodeFocused;

		public CheerBarkNodeItemVM(string tauntVisualName, TextObject nodeName, string nodeId, HotKey key, bool consoleOnlyShortcut = false, TauntUsageManager.TauntUsage.TauntUsageFlag disabledReason = TauntUsageManager.TauntUsage.TauntUsageFlag.None)
		{
			this._nodeName = nodeName;
			this.TauntVisualName = tauntVisualName;
			this.TypeAsString = nodeId;
			this.TauntUsageDisabledReason = disabledReason;
			this.IsDisabled = disabledReason != TauntUsageManager.TauntUsage.TauntUsageFlag.None && disabledReason != TauntUsageManager.TauntUsage.TauntUsageFlag.IsLeftStance;
			this.SubNodes = new MBBindingList<CheerBarkNodeItemVM>();
			if (key != null)
			{
				this.ShortcutKey = InputKeyItemVM.CreateFromHotKey(key, consoleOnlyShortcut);
			}
			this.RefreshValues();
		}

		public CheerBarkNodeItemVM(TextObject nodeName, string nodeId, HotKey key, bool consoleOnlyShortcut = false, TauntUsageManager.TauntUsage.TauntUsageFlag disabledReason = TauntUsageManager.TauntUsage.TauntUsageFlag.None)
		{
			this._nodeName = nodeName;
			this.TauntVisualName = string.Empty;
			this.TypeAsString = nodeId;
			this.TauntUsageDisabledReason = disabledReason;
			this.IsDisabled = disabledReason > TauntUsageManager.TauntUsage.TauntUsageFlag.None;
			this.SubNodes = new MBBindingList<CheerBarkNodeItemVM>();
			if (key != null)
			{
				this.ShortcutKey = InputKeyItemVM.CreateFromHotKey(key, consoleOnlyShortcut);
			}
			this.RefreshValues();
		}

		public void ClearSelectionRecursive()
		{
			this.IsSelected = false;
			for (int i = 0; i < this.SubNodes.Count; i++)
			{
				this.SubNodes[i].ClearSelectionRecursive();
			}
		}

		public void ExecuteFocused()
		{
			Action<CheerBarkNodeItemVM> onNodeFocused = CheerBarkNodeItemVM.OnNodeFocused;
			if (onNodeFocused == null)
			{
				return;
			}
			onNodeFocused(this);
		}

		public override void RefreshValues()
		{
			TextObject nodeName = this._nodeName;
			this.CheerNameText = ((nodeName != null) ? nodeName.ToString() : null);
		}

		public void AddSubNode(CheerBarkNodeItemVM subNode)
		{
			this.SubNodes.Add(subNode);
			this.HasSubNodes = true;
		}

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

		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (value != this._isDisabled)
				{
					this._isDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDisabled");
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
					if (this._isSelected)
					{
						Action<CheerBarkNodeItemVM> onSelection = CheerBarkNodeItemVM.OnSelection;
						if (onSelection == null)
						{
							return;
						}
						onSelection(this);
					}
				}
			}
		}

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

		[DataSourceProperty]
		public string TauntVisualName
		{
			get
			{
				return this._tauntVisualName;
			}
			set
			{
				if (value != this._tauntVisualName)
				{
					this._tauntVisualName = value;
					base.OnPropertyChangedWithValue<string>(value, "TauntVisualName");
				}
			}
		}

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

		public readonly TauntUsageManager.TauntUsage.TauntUsageFlag TauntUsageDisabledReason;

		private readonly TextObject _nodeName;

		private InputKeyItemVM _shortcutKey;

		private MBBindingList<CheerBarkNodeItemVM> _subNodes;

		private string _cheerNameText;

		private string _typeAsString;

		private string _tauntVisualName;

		private string _selectedNodeText;

		private bool _isDisabled;

		private bool _isSelected;

		private bool _hasSubNodes;
	}
}
