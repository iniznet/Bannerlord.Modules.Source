using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.GauntletUI.GamepadNavigation;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	public class PartyScreenWidget : Widget
	{
		public PartyTroopTupleButtonWidget CurrentMainTuple
		{
			get
			{
				return this._currentMainTuple;
			}
		}

		public PartyTroopTupleButtonWidget CurrentOtherTuple
		{
			get
			{
				return this._currentOtherTuple;
			}
		}

		public ScrollablePanel MainScrollPanel { get; set; }

		public ScrollablePanel OtherScrollPanel { get; set; }

		public InputKeyVisualWidget TransferInputKeyVisual { get; set; }

		public PartyScreenWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnConnectedToRoot()
		{
			base.Context.EventManager.OnDragStarted += this.OnDragStarted;
		}

		protected override void OnDisconnectedFromRoot()
		{
			base.Context.EventManager.OnDragStarted -= this.OnDragStarted;
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			Widget latestMouseUpWidget = base.EventManager.LatestMouseUpWidget;
			if (this._currentMainTuple != null && latestMouseUpWidget != null && !(latestMouseUpWidget is PartyTroopTupleButtonWidget) && !this._currentMainTuple.CheckIsMyChildRecursive(latestMouseUpWidget))
			{
				PartyTroopTupleButtonWidget currentOtherTuple = this._currentOtherTuple;
				if (currentOtherTuple != null && !currentOtherTuple.CheckIsMyChildRecursive(latestMouseUpWidget) && this.IsWidgetChildOfType<PartyFormationDropdownWidget>(latestMouseUpWidget) == null)
				{
					this.SetCurrentTuple(null, false);
				}
			}
			this.UpdateInputKeyVisualsVisibility();
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._newAddedTroop != null)
			{
				string characterID = this._newAddedTroop.CharacterID;
				PartyTroopTupleButtonWidget currentMainTuple = this._currentMainTuple;
				if (characterID == ((currentMainTuple != null) ? currentMainTuple.CharacterID : null))
				{
					bool isPrisoner = this._newAddedTroop.IsPrisoner;
					PartyTroopTupleButtonWidget currentMainTuple2 = this._currentMainTuple;
					bool? flag = ((currentMainTuple2 != null) ? new bool?(currentMainTuple2.IsPrisoner) : null);
					if ((isPrisoner == flag.GetValueOrDefault()) & (flag != null))
					{
						bool isTupleLeftSide = this._newAddedTroop.IsTupleLeftSide;
						PartyTroopTupleButtonWidget currentMainTuple3 = this._currentMainTuple;
						flag = ((currentMainTuple3 != null) ? new bool?(currentMainTuple3.IsTupleLeftSide) : null);
						if (!((isTupleLeftSide == flag.GetValueOrDefault()) & (flag != null)))
						{
							this._currentOtherTuple = this._newAddedTroop;
							this._currentOtherTuple.IsSelected = true;
							this.UpdateScrollTarget();
						}
					}
				}
				this._newAddedTroop = null;
			}
		}

		public void SetCurrentTuple(PartyTroopTupleButtonWidget tuple, bool isLeftSide)
		{
			if (this._currentMainTuple != null && this._currentMainTuple != tuple)
			{
				this._currentMainTuple.IsSelected = false;
				if (this._currentOtherTuple != null)
				{
					this._currentOtherTuple.IsSelected = false;
					this._currentOtherTuple = null;
				}
			}
			if (tuple == null)
			{
				this._currentMainTuple = null;
				this.RemoveZeroCountItems();
				if (this._currentOtherTuple != null)
				{
					this._currentOtherTuple.IsSelected = false;
					this._currentOtherTuple = null;
					return;
				}
			}
			else
			{
				if (tuple == this._currentMainTuple || tuple == this._currentOtherTuple)
				{
					this.SetCurrentTuple(null, false);
					return;
				}
				this._currentMainTuple = tuple;
				this._currentOtherTuple = this.FindTupleWithTroopIDInList(this._currentMainTuple.CharacterID, this._currentMainTuple.IsTupleLeftSide, this._currentMainTuple.IsPrisoner);
				if (this._currentOtherTuple != null)
				{
					this._currentOtherTuple.IsSelected = true;
					this.UpdateScrollTarget();
				}
			}
		}

		private void UpdateInputKeyVisualsVisibility()
		{
			if (base.EventManager.IsControllerActive)
			{
				bool flag = false;
				PartyTroopTupleButtonWidget partyTroopTupleButtonWidget;
				if ((partyTroopTupleButtonWidget = base.EventManager.HoveredView as PartyTroopTupleButtonWidget) != null)
				{
					this.TransferInputKeyVisual.IsVisible = partyTroopTupleButtonWidget.IsTransferable;
					flag = true;
					if (partyTroopTupleButtonWidget.IsTupleLeftSide)
					{
						this.TransferInputKeyVisual.KeyID = this._takeAllPrisonersInputKeyVisual.KeyID;
						this.TransferInputKeyVisual.ScaledPositionXOffset = partyTroopTupleButtonWidget.GlobalPosition.X + partyTroopTupleButtonWidget.Size.X - 65f * base._scaleToUse - base.EventManager.LeftUsableAreaStart;
						this.TransferInputKeyVisual.ScaledPositionYOffset = partyTroopTupleButtonWidget.GlobalPosition.Y - 13f * base._scaleToUse - base.EventManager.TopUsableAreaStart;
					}
					else
					{
						this.TransferInputKeyVisual.KeyID = this._dismissAllPrisonersInputKeyVisual.KeyID;
						this.TransferInputKeyVisual.ScaledPositionXOffset = partyTroopTupleButtonWidget.GlobalPosition.X + 5f * base._scaleToUse - base.EventManager.LeftUsableAreaStart;
						this.TransferInputKeyVisual.ScaledPositionYOffset = partyTroopTupleButtonWidget.GlobalPosition.Y - 13f * base._scaleToUse - base.EventManager.TopUsableAreaStart;
					}
				}
				else
				{
					this.TransferInputKeyVisual.IsVisible = false;
					this.TransferInputKeyVisual.KeyID = "";
				}
				bool flag2 = !this.IsAnyPopupOpen() && !flag && !this.MainScrollPanel.InnerPanel.IsHovered && !this.OtherScrollPanel.InnerPanel.IsHovered && !GauntletGamepadNavigationManager.Instance.IsCursorMovingForNavigation;
				this.TakeAllPrisonersInputKeyVisualParent.IsVisible = flag2;
				this.DismissAllPrisonersInputKeyVisualParent.IsVisible = flag2;
				return;
			}
			this.TransferInputKeyVisual.IsVisible = false;
			this.TakeAllPrisonersInputKeyVisualParent.IsVisible = true;
			this.DismissAllPrisonersInputKeyVisualParent.IsVisible = true;
		}

		private void RefreshWarningStatuses()
		{
			TextWidget prisonerLabel = this.PrisonerLabel;
			if (prisonerLabel != null)
			{
				prisonerLabel.SetState(this.IsPrisonerWarningEnabled ? "OverLimit" : "Default");
			}
			TextWidget troopLabel = this.TroopLabel;
			if (troopLabel != null)
			{
				troopLabel.SetState(this.IsTroopWarningEnabled ? "OverLimit" : "Default");
			}
			TextWidget otherTroopLabel = this.OtherTroopLabel;
			if (otherTroopLabel == null)
			{
				return;
			}
			otherTroopLabel.SetState(this.IsOtherTroopWarningEnabled ? "OverLimit" : "Default");
		}

		private PartyTroopTupleButtonWidget FindTupleWithTroopIDInList(string troopID, bool searchMainList, bool isPrisoner)
		{
			IEnumerable<PartyTroopTupleButtonWidget> enumerable;
			if (searchMainList)
			{
				enumerable = (isPrisoner ? this.MainPrisonerList.Children.Cast<PartyTroopTupleButtonWidget>() : this.MainMemberList.Children.Cast<PartyTroopTupleButtonWidget>());
			}
			else
			{
				enumerable = (isPrisoner ? this.OtherPrisonerList.Children.Cast<PartyTroopTupleButtonWidget>() : this.OtherMemberList.Children.Cast<PartyTroopTupleButtonWidget>());
			}
			return enumerable.SingleOrDefault((PartyTroopTupleButtonWidget i) => i.CharacterID == troopID);
		}

		private void OnDragStarted()
		{
			this.RemoveZeroCountItems();
		}

		private void RemoveZeroCountItems()
		{
			base.EventFired("RemoveZeroCounts", Array.Empty<object>());
		}

		private void UpdateScrollTarget()
		{
			PartyTroopTupleButtonWidget currentOtherTuple = this._currentOtherTuple;
			if (((currentOtherTuple != null) ? currentOtherTuple.ParentWidget : null) != null)
			{
				(this._currentOtherTuple.IsTupleLeftSide ? this.OtherScrollPanel : this.MainScrollPanel).ScrollToChild(this._currentOtherTuple, -1f, 1f, 0, 400, 0.35f, 0f);
			}
		}

		private bool IsAnyPopupOpen()
		{
			Widget recruitPopupParent = this.RecruitPopupParent;
			if (recruitPopupParent == null || !recruitPopupParent.IsVisible)
			{
				Widget upgradePopupParent = this.UpgradePopupParent;
				return upgradePopupParent != null && upgradePopupParent.IsVisible;
			}
			return true;
		}

		private void OnNewTroopAdded(Widget parent, Widget addedChild)
		{
			PartyTroopTupleButtonWidget partyTroopTupleButtonWidget;
			if (this._currentMainTuple != null && (partyTroopTupleButtonWidget = addedChild as PartyTroopTupleButtonWidget) != null)
			{
				this._newAddedTroop = partyTroopTupleButtonWidget;
			}
		}

		public Widget UpgradePopupParent
		{
			get
			{
				return this._upgradePopupParent;
			}
			set
			{
				if (value != this._upgradePopupParent)
				{
					this._upgradePopupParent = value;
				}
			}
		}

		public Widget RecruitPopupParent
		{
			get
			{
				return this._recruitPopupParent;
			}
			set
			{
				if (value != this._recruitPopupParent)
				{
					this._recruitPopupParent = value;
				}
			}
		}

		public Widget TakeAllPrisonersInputKeyVisualParent
		{
			get
			{
				return this._takeAllPrisonersInputKeyVisualParent;
			}
			set
			{
				if (value != this._takeAllPrisonersInputKeyVisualParent)
				{
					this._takeAllPrisonersInputKeyVisualParent = value;
					if (this._takeAllPrisonersInputKeyVisualParent != null)
					{
						this._takeAllPrisonersInputKeyVisual = this._takeAllPrisonersInputKeyVisualParent.Children.FirstOrDefault((Widget x) => x is InputKeyVisualWidget) as InputKeyVisualWidget;
					}
				}
			}
		}

		public Widget DismissAllPrisonersInputKeyVisualParent
		{
			get
			{
				return this._dismissAllPrisonersInputKeyVisualParent;
			}
			set
			{
				if (value != this._dismissAllPrisonersInputKeyVisualParent)
				{
					this._dismissAllPrisonersInputKeyVisualParent = value;
					if (this._dismissAllPrisonersInputKeyVisualParent != null)
					{
						this._dismissAllPrisonersInputKeyVisual = this._dismissAllPrisonersInputKeyVisualParent.Children.FirstOrDefault((Widget x) => x is InputKeyVisualWidget) as InputKeyVisualWidget;
					}
				}
			}
		}

		[Editor(false)]
		public int MainPartyTroopSize
		{
			get
			{
				return this._mainPartyTroopSize;
			}
			set
			{
				if (this._mainPartyTroopSize != value)
				{
					this._mainPartyTroopSize = value;
					base.OnPropertyChanged(value, "MainPartyTroopSize");
				}
			}
		}

		[Editor(false)]
		public bool IsPrisonerWarningEnabled
		{
			get
			{
				return this._isPrisonerWarningEnabled;
			}
			set
			{
				if (this._isPrisonerWarningEnabled != value)
				{
					this._isPrisonerWarningEnabled = value;
					base.OnPropertyChanged(value, "IsPrisonerWarningEnabled");
					this.RefreshWarningStatuses();
				}
			}
		}

		[Editor(false)]
		public bool IsOtherTroopWarningEnabled
		{
			get
			{
				return this._isOtherTroopWarningEnabled;
			}
			set
			{
				if (this._isOtherTroopWarningEnabled != value)
				{
					this._isOtherTroopWarningEnabled = value;
					base.OnPropertyChanged(value, "IsOtherTroopWarningEnabled");
					this.RefreshWarningStatuses();
				}
			}
		}

		[Editor(false)]
		public bool IsTroopWarningEnabled
		{
			get
			{
				return this._isTroopWarningEnabled;
			}
			set
			{
				if (this._isTroopWarningEnabled != value)
				{
					this._isTroopWarningEnabled = value;
					base.OnPropertyChanged(value, "IsTroopWarningEnabled");
					this.RefreshWarningStatuses();
				}
			}
		}

		[Editor(false)]
		public TextWidget TroopLabel
		{
			get
			{
				return this._troopLabel;
			}
			set
			{
				if (this._troopLabel != value)
				{
					this._troopLabel = value;
					base.OnPropertyChanged<TextWidget>(value, "TroopLabel");
					this.RefreshWarningStatuses();
				}
			}
		}

		[Editor(false)]
		public TextWidget PrisonerLabel
		{
			get
			{
				return this._prisonerLabel;
			}
			set
			{
				if (this._prisonerLabel != value)
				{
					this._prisonerLabel = value;
					base.OnPropertyChanged<TextWidget>(value, "PrisonerLabel");
					this.RefreshWarningStatuses();
				}
			}
		}

		[Editor(false)]
		public TextWidget OtherTroopLabel
		{
			get
			{
				return this._otherTroopLabel;
			}
			set
			{
				if (this._otherTroopLabel != value)
				{
					this._otherTroopLabel = value;
					base.OnPropertyChanged<TextWidget>(value, "OtherTroopLabel");
					this.RefreshWarningStatuses();
				}
			}
		}

		[Editor(false)]
		public ListPanel OtherMemberList
		{
			get
			{
				return this._otherMemberList;
			}
			set
			{
				if (this._otherMemberList != value)
				{
					this._otherMemberList = value;
					value.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnNewTroopAdded));
				}
			}
		}

		[Editor(false)]
		public ListPanel OtherPrisonerList
		{
			get
			{
				return this._otherPrisonerList;
			}
			set
			{
				if (this._otherPrisonerList != value)
				{
					this._otherPrisonerList = value;
					value.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnNewTroopAdded));
				}
			}
		}

		[Editor(false)]
		public ListPanel MainMemberList
		{
			get
			{
				return this._mainMemberList;
			}
			set
			{
				if (this._mainMemberList != value)
				{
					this._mainMemberList = value;
					value.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnNewTroopAdded));
				}
			}
		}

		[Editor(false)]
		public ListPanel MainPrisonerList
		{
			get
			{
				return this._mainPrisonerList;
			}
			set
			{
				if (this._mainPrisonerList != value)
				{
					this._mainPrisonerList = value;
					value.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnNewTroopAdded));
				}
			}
		}

		private T IsWidgetChildOfType<T>(Widget currentWidget) where T : Widget
		{
			while (currentWidget != null)
			{
				T t;
				if ((t = currentWidget as T) != null)
				{
					return t;
				}
				currentWidget = currentWidget.ParentWidget;
			}
			return default(T);
		}

		private PartyTroopTupleButtonWidget _currentMainTuple;

		private PartyTroopTupleButtonWidget _currentOtherTuple;

		private InputKeyVisualWidget _takeAllPrisonersInputKeyVisual;

		private InputKeyVisualWidget _dismissAllPrisonersInputKeyVisual;

		private PartyTroopTupleButtonWidget _newAddedTroop;

		private Widget _upgradePopupParent;

		private Widget _recruitPopupParent;

		private Widget _takeAllPrisonersInputKeyVisualParent;

		private Widget _dismissAllPrisonersInputKeyVisualParent;

		private int _mainPartyTroopSize;

		private bool _isPrisonerWarningEnabled;

		private bool _isTroopWarningEnabled;

		private bool _isOtherTroopWarningEnabled;

		private TextWidget _troopLabel;

		private TextWidget _prisonerLabel;

		private TextWidget _otherTroopLabel;

		private ListPanel _otherMemberList;

		private ListPanel _otherPrisonerList;

		private ListPanel _mainMemberList;

		private ListPanel _mainPrisonerList;
	}
}
