using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class GameMenuItemWidget : Widget
	{
		public Brush DefaultTextBrush { get; set; }

		public Brush HoveredTextBrush { get; set; }

		public Brush PressedTextBrush { get; set; }

		public Brush DisabledTextBrush { get; set; }

		public Brush NormalQuestBrush { get; set; }

		public Brush MainStoryQuestBrush { get; set; }

		public RichTextWidget ItemRichTextWidget { get; set; }

		public GameMenuItemWidget(UIContext context)
			: base(context)
		{
		}

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
			case 40:
				text = "Warehouse";
				break;
			}
			if (!string.IsNullOrEmpty(text) && type != 0)
			{
				this.LeaveTypeIcon.SetState(text);
				this.LeaveTypeIcon.IsVisible = true;
			}
		}

		private void SetLeaveTypeSound()
		{
			ButtonWidget parentButton = this.ParentButton;
			AudioProperty audioProperty = ((parentButton != null) ? parentButton.Brush.SoundProperties.GetEventAudioProperty("Click") : null);
			if (audioProperty != null)
			{
				audioProperty.AudioName = "default";
				int leaveType = this.LeaveType;
				if (leaveType <= 12)
				{
					if (leaveType != 1)
					{
						if (leaveType != 9)
						{
							if (leaveType != 12)
							{
								return;
							}
							if (this.GameMenuStringId == "encounter")
							{
								if (this.BattleSize < 50)
								{
									audioProperty.AudioName = "panels/battle/attack_small";
									return;
								}
								if (this.BattleSize < 100)
								{
									audioProperty.AudioName = "panels/battle/attack_medium";
									return;
								}
								audioProperty.AudioName = "panels/battle/attack_large";
								return;
							}
						}
						else if (this.GameMenuStringId == "encounter" || this.GameMenuStringId == "encounter_interrupted_siege_preparations" || this.GameMenuStringId == "menu_siege_strategies")
						{
							audioProperty.AudioName = "panels/battle/retreat";
							return;
						}
					}
					else if (this.GameMenuStringId == "menu_siege_strategies")
					{
						audioProperty.AudioName = "panels/siege/sally_out";
						return;
					}
				}
				else if (leaveType <= 25)
				{
					if (leaveType != 21)
					{
						if (leaveType - 24 > 1)
						{
							return;
						}
						audioProperty.AudioName = "panels/siege/raid";
						return;
					}
					else if (this.GameMenuStringId == "encounter")
					{
						audioProperty.AudioName = "panels/battle/retreat";
						return;
					}
				}
				else
				{
					if (leaveType == 34)
					{
						audioProperty.AudioName = "panels/siege/besiege";
						return;
					}
					if (leaveType != 36)
					{
						return;
					}
					audioProperty.AudioName = "panels/siege/lead_assault";
				}
			}
		}

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
					this.SetLeaveTypeSound();
				}
			}
		}

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
					base.OnPropertyChanged<string>(value, "GameMenuStringId");
					this.SetLeaveTypeSound();
				}
			}
		}

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
					base.OnPropertyChanged(value, "BattleSize");
					this.SetLeaveTypeSound();
				}
			}
		}

		public Action OnOptionStateChanged;

		private string _latestTextWidgetState = "";

		private bool _firstFrame = true;

		private int _itemType;

		private bool _isWaitActive;

		private bool _isMainStoryQuest;

		private BrushWidget _waitStateWidget;

		private BrushWidget _leaveTypeIcon;

		private int _leaveType = -1;

		private int _questType = -1;

		private int _issueType = -1;

		private BrushWidget _questIconWidget;

		private BrushWidget _issueIconWidget;

		private ButtonWidget _parentButton;

		private string _gameMenuStringId;

		private int _battleSize;
	}
}
