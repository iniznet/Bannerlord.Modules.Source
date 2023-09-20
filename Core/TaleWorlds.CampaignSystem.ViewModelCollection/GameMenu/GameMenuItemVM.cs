using System;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem.ViewModelCollection.Quests;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu
{
	public class GameMenuItemVM : BindingListStringItem
	{
		public string OptionID { get; }

		public GameMenuItemVM(MenuContext menuContext, int index, TextObject text, TextObject text2, TextObject tooltip, GameMenu.MenuAndOptionType type, GameMenuOption gameMenuOption, GameKey shortcutKey)
			: base(text.ToString())
		{
			this._gameMenuOption = gameMenuOption;
			this.ItemHint = new HintViewModel();
			this.Index = index;
			this._menuContext = menuContext;
			this._itemType = (int)type;
			this._tooltip = tooltip;
			this._nonWaitText = text;
			this._waitText = text2;
			base.Item = this._nonWaitText.ToString();
			this.ItemHint.HintText = this._tooltip;
			this.OptionLeaveType = (int)gameMenuOption.OptionLeaveType;
			this.OptionID = gameMenuOption.IdString;
			this.Quests = new MBBindingList<QuestMarkerVM>();
			foreach (GameMenuOption.IssueQuestFlags issueQuestFlags in GameMenuOption.IssueQuestFlagsValues)
			{
				if (issueQuestFlags != GameMenuOption.IssueQuestFlags.None && (gameMenuOption.OptionQuestData & issueQuestFlags) != GameMenuOption.IssueQuestFlags.None)
				{
					CampaignUIHelper.IssueQuestFlags issueQuestFlags2 = (CampaignUIHelper.IssueQuestFlags)issueQuestFlags;
					this.Quests.Add(new QuestMarkerVM(issueQuestFlags2, null, null));
				}
			}
			this.ShortcutKey = ((shortcutKey != null) ? InputKeyItemVM.CreateFromGameKey(shortcutKey, true) : null);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Refresh();
		}

		public void UpdateMenuContext(MenuContext newMenuContext)
		{
			this._menuContext = newMenuContext;
			this.Refresh();
		}

		public void ExecuteAction()
		{
			MenuContext menuContext = this._menuContext;
			if (menuContext == null)
			{
				return;
			}
			menuContext.InvokeConsequence(this.Index);
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			if (this.ShortcutKey != null)
			{
				this.ShortcutKey.OnFinalize();
			}
		}

		public void Refresh()
		{
			int itemType = this._itemType;
			if (itemType != 0)
			{
				int num = itemType - 1;
			}
			this.IsWaitActive = Campaign.Current.GameMenuManager.GetVirtualMenuIsWaitActive(this._menuContext);
			this.IsEnabled = Campaign.Current.GameMenuManager.GetVirtualMenuOptionIsEnabled(this._menuContext, this.Index);
			this.ItemHint.HintText = Campaign.Current.GameMenuManager.GetVirtualMenuOptionTooltip(this._menuContext, this.Index);
			this.GameMenuStringId = this._menuContext.GameMenu.StringId;
			if (PlayerEncounter.Battle != null)
			{
				this.BattleSize = PlayerEncounter.Battle.AttackerSide.TroopCount + PlayerEncounter.Battle.DefenderSide.TroopCount;
				return;
			}
			this.BattleSize = -1;
		}

		[DataSourceProperty]
		public MBBindingList<QuestMarkerVM> Quests
		{
			get
			{
				return this._quests;
			}
			set
			{
				if (value != this._quests)
				{
					this._quests = value;
					base.OnPropertyChangedWithValue<MBBindingList<QuestMarkerVM>>(value, "Quests");
				}
			}
		}

		[DataSourceProperty]
		public int OptionLeaveType
		{
			get
			{
				return this._optionLeaveType;
			}
			set
			{
				if (value != this._optionLeaveType)
				{
					this._optionLeaveType = value;
					base.OnPropertyChangedWithValue(value, "OptionLeaveType");
				}
			}
		}

		[DataSourceProperty]
		public int ItemType
		{
			get
			{
				return this._itemType;
			}
			set
			{
				if (value != this._itemType)
				{
					this._itemType = value;
					base.OnPropertyChangedWithValue(value, "ItemType");
				}
			}
		}

		[DataSourceProperty]
		public bool IsWaitActive
		{
			get
			{
				return this._isWaitActive;
			}
			set
			{
				if (value != this._isWaitActive)
				{
					this._isWaitActive = value;
					base.OnPropertyChangedWithValue(value, "IsWaitActive");
					base.Item = (value ? this._waitText.ToString() : this._nonWaitText.ToString());
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
		public bool IsHighlightEnabled
		{
			get
			{
				return this._isHighlightEnabled;
			}
			set
			{
				if (value != this._isHighlightEnabled)
				{
					this._isHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsHighlightEnabled");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ItemHint
		{
			get
			{
				return this._itemHint;
			}
			set
			{
				if (value != this._itemHint)
				{
					this._itemHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ItemHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel QuestHint
		{
			get
			{
				return this._questHint;
			}
			set
			{
				if (value != this._questHint)
				{
					this._questHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "QuestHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel IssueHint
		{
			get
			{
				return this._issueHint;
			}
			set
			{
				if (value != this._issueHint)
				{
					this._issueHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "IssueHint");
				}
			}
		}

		[DataSourceProperty]
		public string GameMenuStringId
		{
			get
			{
				return this._gameMenuStringId;
			}
			set
			{
				if (value != this._gameMenuStringId)
				{
					this._gameMenuStringId = value;
					base.OnPropertyChangedWithValue<string>(value, "GameMenuStringId");
				}
			}
		}

		[DataSourceProperty]
		public int BattleSize
		{
			get
			{
				return this._battleSize;
			}
			set
			{
				if (value != this._battleSize)
				{
					this._battleSize = value;
					base.OnPropertyChangedWithValue(value, "BattleSize");
				}
			}
		}

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

		private MenuContext _menuContext;

		public int Index;

		private TextObject _nonWaitText;

		private TextObject _waitText;

		private TextObject _tooltip;

		private readonly GameMenuOption _gameMenuOption;

		private MBBindingList<QuestMarkerVM> _quests;

		private int _itemType = -1;

		private bool _isWaitActive;

		private bool _isEnabled;

		private HintViewModel _itemHint;

		private HintViewModel _questHint;

		private HintViewModel _issueHint;

		private bool _isHighlightEnabled;

		private int _optionLeaveType = -1;

		private string _gameMenuStringId;

		private int _battleSize = -1;

		private InputKeyItemVM _shortcutKey;
	}
}
