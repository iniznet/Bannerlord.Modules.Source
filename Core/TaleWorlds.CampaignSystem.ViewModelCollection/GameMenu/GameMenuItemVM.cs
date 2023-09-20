using System;
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
	// Token: 0x02000086 RID: 134
	public class GameMenuItemVM : BindingListStringItem
	{
		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06000D33 RID: 3379 RVA: 0x000358B6 File Offset: 0x00033AB6
		public string OptionID { get; }

		// Token: 0x06000D34 RID: 3380 RVA: 0x000358C0 File Offset: 0x00033AC0
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

		// Token: 0x06000D35 RID: 3381 RVA: 0x000359C4 File Offset: 0x00033BC4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Refresh();
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x000359D2 File Offset: 0x00033BD2
		public void UpdateMenuContext(MenuContext newMenuContext)
		{
			this._menuContext = newMenuContext;
			this.Refresh();
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x000359E1 File Offset: 0x00033BE1
		public void ExecuteAction()
		{
			MenuContext menuContext = this._menuContext;
			if (menuContext == null)
			{
				return;
			}
			menuContext.InvokeConsequence(this.Index);
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x000359F9 File Offset: 0x00033BF9
		public override void OnFinalize()
		{
			base.OnFinalize();
			if (this.ShortcutKey != null)
			{
				this.ShortcutKey.OnFinalize();
			}
		}

		// Token: 0x06000D39 RID: 3385 RVA: 0x00035A14 File Offset: 0x00033C14
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
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06000D3A RID: 3386 RVA: 0x00035A93 File Offset: 0x00033C93
		// (set) Token: 0x06000D3B RID: 3387 RVA: 0x00035A9B File Offset: 0x00033C9B
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

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06000D3C RID: 3388 RVA: 0x00035AB9 File Offset: 0x00033CB9
		// (set) Token: 0x06000D3D RID: 3389 RVA: 0x00035AC1 File Offset: 0x00033CC1
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

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06000D3E RID: 3390 RVA: 0x00035ADF File Offset: 0x00033CDF
		// (set) Token: 0x06000D3F RID: 3391 RVA: 0x00035AE7 File Offset: 0x00033CE7
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

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06000D40 RID: 3392 RVA: 0x00035B05 File Offset: 0x00033D05
		// (set) Token: 0x06000D41 RID: 3393 RVA: 0x00035B0D File Offset: 0x00033D0D
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

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06000D42 RID: 3394 RVA: 0x00035B4C File Offset: 0x00033D4C
		// (set) Token: 0x06000D43 RID: 3395 RVA: 0x00035B54 File Offset: 0x00033D54
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

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06000D44 RID: 3396 RVA: 0x00035B72 File Offset: 0x00033D72
		// (set) Token: 0x06000D45 RID: 3397 RVA: 0x00035B7A File Offset: 0x00033D7A
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

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06000D46 RID: 3398 RVA: 0x00035B98 File Offset: 0x00033D98
		// (set) Token: 0x06000D47 RID: 3399 RVA: 0x00035BA0 File Offset: 0x00033DA0
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

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06000D48 RID: 3400 RVA: 0x00035BBE File Offset: 0x00033DBE
		// (set) Token: 0x06000D49 RID: 3401 RVA: 0x00035BC6 File Offset: 0x00033DC6
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

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06000D4A RID: 3402 RVA: 0x00035BE4 File Offset: 0x00033DE4
		// (set) Token: 0x06000D4B RID: 3403 RVA: 0x00035BEC File Offset: 0x00033DEC
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

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06000D4C RID: 3404 RVA: 0x00035C0A File Offset: 0x00033E0A
		// (set) Token: 0x06000D4D RID: 3405 RVA: 0x00035C12 File Offset: 0x00033E12
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

		// Token: 0x04000613 RID: 1555
		private MenuContext _menuContext;

		// Token: 0x04000614 RID: 1556
		public int Index;

		// Token: 0x04000615 RID: 1557
		private TextObject _nonWaitText;

		// Token: 0x04000616 RID: 1558
		private TextObject _waitText;

		// Token: 0x04000617 RID: 1559
		private TextObject _tooltip;

		// Token: 0x04000619 RID: 1561
		private readonly GameMenuOption _gameMenuOption;

		// Token: 0x0400061A RID: 1562
		private MBBindingList<QuestMarkerVM> _quests;

		// Token: 0x0400061B RID: 1563
		private int _itemType = -1;

		// Token: 0x0400061C RID: 1564
		private bool _isWaitActive;

		// Token: 0x0400061D RID: 1565
		private bool _isEnabled;

		// Token: 0x0400061E RID: 1566
		private HintViewModel _itemHint;

		// Token: 0x0400061F RID: 1567
		private HintViewModel _questHint;

		// Token: 0x04000620 RID: 1568
		private HintViewModel _issueHint;

		// Token: 0x04000621 RID: 1569
		private bool _isHighlightEnabled;

		// Token: 0x04000622 RID: 1570
		private int _optionLeaveType = -1;

		// Token: 0x04000623 RID: 1571
		private InputKeyItemVM _shortcutKey;
	}
}
