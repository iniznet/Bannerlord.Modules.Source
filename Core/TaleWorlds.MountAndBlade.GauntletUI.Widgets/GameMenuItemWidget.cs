using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200001B RID: 27
	public class GameMenuItemWidget : Widget
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600012B RID: 299 RVA: 0x0000550D File Offset: 0x0000370D
		// (set) Token: 0x0600012C RID: 300 RVA: 0x00005515 File Offset: 0x00003715
		public Brush DefaultTextBrush { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600012D RID: 301 RVA: 0x0000551E File Offset: 0x0000371E
		// (set) Token: 0x0600012E RID: 302 RVA: 0x00005526 File Offset: 0x00003726
		public Brush HoveredTextBrush { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600012F RID: 303 RVA: 0x0000552F File Offset: 0x0000372F
		// (set) Token: 0x06000130 RID: 304 RVA: 0x00005537 File Offset: 0x00003737
		public Brush PressedTextBrush { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00005540 File Offset: 0x00003740
		// (set) Token: 0x06000132 RID: 306 RVA: 0x00005548 File Offset: 0x00003748
		public Brush DisabledTextBrush { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00005551 File Offset: 0x00003751
		// (set) Token: 0x06000134 RID: 308 RVA: 0x00005559 File Offset: 0x00003759
		public Brush NormalQuestBrush { get; set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000135 RID: 309 RVA: 0x00005562 File Offset: 0x00003762
		// (set) Token: 0x06000136 RID: 310 RVA: 0x0000556A File Offset: 0x0000376A
		public Brush MainStoryQuestBrush { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000137 RID: 311 RVA: 0x00005573 File Offset: 0x00003773
		// (set) Token: 0x06000138 RID: 312 RVA: 0x0000557B File Offset: 0x0000377B
		public RichTextWidget ItemRichTextWidget { get; set; }

		// Token: 0x06000139 RID: 313 RVA: 0x00005584 File Offset: 0x00003784
		public GameMenuItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600013A RID: 314 RVA: 0x000055B4 File Offset: 0x000037B4
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._firstFrame)
			{
				base.GamepadNavigationIndex = base.GetSiblingIndex();
				this._firstFrame = false;
			}
			if (this._latestTextWidgetState != this.ItemRichTextWidget.CurrentState)
			{
				if (this.ItemRichTextWidget.CurrentState == "Default")
				{
					this.ItemRichTextWidget.Brush = this.DefaultTextBrush;
				}
				else if (this.ItemRichTextWidget.CurrentState == "Hovered")
				{
					this.ItemRichTextWidget.Brush = this.HoveredTextBrush;
				}
				else if (this.ItemRichTextWidget.CurrentState == "Pressed")
				{
					this.ItemRichTextWidget.Brush = this.PressedTextBrush;
				}
				else if (this.ItemRichTextWidget.CurrentState == "Disabled")
				{
					this.ItemRichTextWidget.Brush = this.DisabledTextBrush;
				}
				this._latestTextWidgetState = this.ItemRichTextWidget.CurrentState;
			}
		}

		// Token: 0x0600013B RID: 315 RVA: 0x000056B8 File Offset: 0x000038B8
		private void SetLeaveTypeIcon(int type)
		{
			string text = string.Empty;
			switch (type)
			{
			case 0:
				text = "Default";
				break;
			case 1:
				text = "Mission";
				break;
			case 2:
				text = "SubMenu";
				break;
			case 3:
				text = "BribeAndEscape";
				break;
			case 4:
				text = "Escape";
				break;
			case 5:
				text = "Craft";
				break;
			case 6:
				text = "ForceToGiveGoods";
				break;
			case 7:
				text = "ForceToGiveTroops";
				break;
			case 8:
				text = "Bribe";
				break;
			case 9:
				text = "LeaveTroopsAndFlee";
				break;
			case 10:
				text = "OrderTroopsToAttack";
				break;
			case 11:
				text = "Raid";
				break;
			case 12:
				text = "HostileAction";
				break;
			case 13:
				text = "Recruit";
				break;
			case 14:
				text = "Trade";
				break;
			case 15:
				text = "Wait";
				break;
			case 16:
				text = "Leave";
				break;
			case 17:
				text = "Continue";
				break;
			case 18:
				text = "Manage";
				break;
			case 19:
				text = "ManageHideoutTroops";
				break;
			case 20:
				text = "WaitQuest";
				break;
			case 21:
				text = "Surrender";
				break;
			case 22:
				text = "Conversation";
				break;
			case 23:
				text = "DefendAction";
				break;
			case 24:
				text = "Devastate";
				break;
			case 25:
				text = "Pillage";
				break;
			case 26:
				text = "ShowMercy";
				break;
			case 27:
				text = "Leaderboard";
				break;
			case 28:
				text = "OpenStash";
				break;
			case 29:
				text = "ManageGarrison";
				break;
			case 30:
				text = "StagePrisonBreak";
				break;
			case 31:
				text = "ManagePrisoners";
				break;
			case 32:
				text = "Ransom";
				break;
			case 33:
				text = "PracticeFight";
				break;
			case 34:
				text = "BesiegeTown";
				break;
			case 35:
				text = "SneakIn";
				break;
			case 36:
				text = "LeadAssault";
				break;
			case 37:
				text = "DonateTroops";
				break;
			case 38:
				text = "DonatePrisoners";
				break;
			case 39:
				text = "SiegeAmbush";
				break;
			}
			if (!string.IsNullOrEmpty(text) && type != 0)
			{
				this.LeaveTypeIcon.SetState(text);
				this.LeaveTypeIcon.IsVisible = true;
			}
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000591C File Offset: 0x00003B1C
		private void SetProgressIconType(int type, Widget progressWidget)
		{
			string text = string.Empty;
			switch (type)
			{
			case 0:
				text = "Default";
				break;
			case 1:
				text = "Available";
				break;
			case 2:
				text = "Active";
				break;
			case 3:
				text = "Completed";
				break;
			default:
				text = "";
				break;
			}
			if (progressWidget == this.QuestIconWidget)
			{
				this.QuestIconWidget.Brush = (this.IsMainStoryQuest ? this.MainStoryQuestBrush : this.NormalQuestBrush);
			}
			if (!string.IsNullOrEmpty(text) && type != 0)
			{
				progressWidget.SetState(text);
				progressWidget.IsVisible = true;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600013D RID: 317 RVA: 0x000059B0 File Offset: 0x00003BB0
		// (set) Token: 0x0600013E RID: 318 RVA: 0x000059B8 File Offset: 0x00003BB8
		public int ItemType
		{
			get
			{
				return this._itemType;
			}
			set
			{
				if (this._itemType != value)
				{
					this._itemType = value;
					base.OnPropertyChanged(value, "ItemType");
				}
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600013F RID: 319 RVA: 0x000059D6 File Offset: 0x00003BD6
		// (set) Token: 0x06000140 RID: 320 RVA: 0x000059DE File Offset: 0x00003BDE
		public BrushWidget QuestIconWidget
		{
			get
			{
				return this._questIconWidget;
			}
			set
			{
				if (this._questIconWidget != value)
				{
					this._questIconWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "QuestIconWidget");
				}
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000141 RID: 321 RVA: 0x000059FC File Offset: 0x00003BFC
		// (set) Token: 0x06000142 RID: 322 RVA: 0x00005A04 File Offset: 0x00003C04
		public BrushWidget IssueIconWidget
		{
			get
			{
				return this._issueIconWidget;
			}
			set
			{
				if (this._issueIconWidget != value)
				{
					this._issueIconWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "IssueIconWidget");
				}
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000143 RID: 323 RVA: 0x00005A22 File Offset: 0x00003C22
		// (set) Token: 0x06000144 RID: 324 RVA: 0x00005A2A File Offset: 0x00003C2A
		public int LeaveType
		{
			get
			{
				return this._leaveType;
			}
			set
			{
				if (this._leaveType != value)
				{
					this._leaveType = value;
					base.OnPropertyChanged(value, "LeaveType");
					this.SetLeaveTypeIcon(value);
				}
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000145 RID: 325 RVA: 0x00005A4F File Offset: 0x00003C4F
		// (set) Token: 0x06000146 RID: 326 RVA: 0x00005A57 File Offset: 0x00003C57
		public bool IsMainStoryQuest
		{
			get
			{
				return this._isMainStoryQuest;
			}
			set
			{
				if (this._isMainStoryQuest != value)
				{
					this._isMainStoryQuest = value;
					base.OnPropertyChanged(value, "IsMainStoryQuest");
					this.SetProgressIconType(this.QuestType, this.QuestIconWidget);
				}
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000147 RID: 327 RVA: 0x00005A87 File Offset: 0x00003C87
		// (set) Token: 0x06000148 RID: 328 RVA: 0x00005A8F File Offset: 0x00003C8F
		public int QuestType
		{
			get
			{
				return this._questType;
			}
			set
			{
				if (this._questType != value)
				{
					this._questType = value;
					base.OnPropertyChanged(value, "QuestType");
					this.SetProgressIconType(value, this.QuestIconWidget);
				}
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00005ABA File Offset: 0x00003CBA
		// (set) Token: 0x0600014A RID: 330 RVA: 0x00005AC2 File Offset: 0x00003CC2
		public int IssueType
		{
			get
			{
				return this._issueType;
			}
			set
			{
				if (this._issueType != value)
				{
					this._issueType = value;
					base.OnPropertyChanged(value, "IssueType");
					this.SetProgressIconType(value, this.IssueIconWidget);
				}
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600014B RID: 331 RVA: 0x00005AED File Offset: 0x00003CED
		// (set) Token: 0x0600014C RID: 332 RVA: 0x00005AF5 File Offset: 0x00003CF5
		public bool IsWaitActive
		{
			get
			{
				return this._isWaitActive;
			}
			set
			{
				if (this._isWaitActive != value)
				{
					this._isWaitActive = value;
					base.OnPropertyChanged(value, "IsWaitActive");
				}
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600014D RID: 333 RVA: 0x00005B13 File Offset: 0x00003D13
		// (set) Token: 0x0600014E RID: 334 RVA: 0x00005B1B File Offset: 0x00003D1B
		public BrushWidget LeaveTypeIcon
		{
			get
			{
				return this._leaveTypeIcon;
			}
			set
			{
				if (this._leaveTypeIcon != value)
				{
					this._leaveTypeIcon = value;
					base.OnPropertyChanged<BrushWidget>(value, "LeaveTypeIcon");
					if (value != null)
					{
						this.LeaveTypeIcon.IsVisible = false;
					}
				}
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600014F RID: 335 RVA: 0x00005B48 File Offset: 0x00003D48
		// (set) Token: 0x06000150 RID: 336 RVA: 0x00005B50 File Offset: 0x00003D50
		public BrushWidget WaitStateWidget
		{
			get
			{
				return this._waitStateWidget;
			}
			set
			{
				if (this._waitStateWidget != value)
				{
					this._waitStateWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "WaitStateWidget");
				}
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000151 RID: 337 RVA: 0x00005B6E File Offset: 0x00003D6E
		// (set) Token: 0x06000152 RID: 338 RVA: 0x00005B76 File Offset: 0x00003D76
		public ButtonWidget ParentButton
		{
			get
			{
				return this._parentButton;
			}
			set
			{
				if (value != this._parentButton)
				{
					this._parentButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ParentButton");
					this._parentButton.boolPropertyChanged += this.ParentButton_PropertyChanged;
				}
			}
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00005BAB File Offset: 0x00003DAB
		private void ParentButton_PropertyChanged(PropertyOwnerObject widget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsDisabled" || propertyName == "IsHighlightEnabled")
			{
				Action onOptionStateChanged = this.OnOptionStateChanged;
				if (onOptionStateChanged == null)
				{
					return;
				}
				onOptionStateChanged();
			}
		}

		// Token: 0x0400008E RID: 142
		public Action OnOptionStateChanged;

		// Token: 0x04000096 RID: 150
		private string _latestTextWidgetState = "";

		// Token: 0x04000097 RID: 151
		private bool _firstFrame = true;

		// Token: 0x04000098 RID: 152
		private int _itemType;

		// Token: 0x04000099 RID: 153
		private bool _isWaitActive;

		// Token: 0x0400009A RID: 154
		private bool _isMainStoryQuest;

		// Token: 0x0400009B RID: 155
		private BrushWidget _waitStateWidget;

		// Token: 0x0400009C RID: 156
		private BrushWidget _leaveTypeIcon;

		// Token: 0x0400009D RID: 157
		private int _leaveType = -1;

		// Token: 0x0400009E RID: 158
		private int _questType = -1;

		// Token: 0x0400009F RID: 159
		private int _issueType = -1;

		// Token: 0x040000A0 RID: 160
		private BrushWidget _questIconWidget;

		// Token: 0x040000A1 RID: 161
		private BrushWidget _issueIconWidget;

		// Token: 0x040000A2 RID: 162
		private ButtonWidget _parentButton;
	}
}
