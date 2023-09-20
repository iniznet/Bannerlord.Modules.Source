using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.GauntletUI.GamepadNavigation;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	// Token: 0x0200005C RID: 92
	public class PartyScreenWidget : Widget
	{
		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060004C0 RID: 1216 RVA: 0x0000EA94 File Offset: 0x0000CC94
		public PartyTroopTupleButtonWidget CurrentMainTuple
		{
			get
			{
				return this._currentMainTuple;
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060004C1 RID: 1217 RVA: 0x0000EA9C File Offset: 0x0000CC9C
		public PartyTroopTupleButtonWidget CurrentOtherTuple
		{
			get
			{
				return this._currentOtherTuple;
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060004C2 RID: 1218 RVA: 0x0000EAA4 File Offset: 0x0000CCA4
		// (set) Token: 0x060004C3 RID: 1219 RVA: 0x0000EAAC File Offset: 0x0000CCAC
		public ScrollablePanel MainScrollPanel { get; set; }

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x060004C4 RID: 1220 RVA: 0x0000EAB5 File Offset: 0x0000CCB5
		// (set) Token: 0x060004C5 RID: 1221 RVA: 0x0000EABD File Offset: 0x0000CCBD
		public ScrollablePanel OtherScrollPanel { get; set; }

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x060004C6 RID: 1222 RVA: 0x0000EAC6 File Offset: 0x0000CCC6
		// (set) Token: 0x060004C7 RID: 1223 RVA: 0x0000EACE File Offset: 0x0000CCCE
		public InputKeyVisualWidget TransferInputKeyVisual { get; set; }

		// Token: 0x060004C8 RID: 1224 RVA: 0x0000EAD7 File Offset: 0x0000CCD7
		public PartyScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x0000EAE0 File Offset: 0x0000CCE0
		protected override void OnConnectedToRoot()
		{
			base.Context.EventManager.OnDragStarted += this.OnDragStarted;
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x0000EAFE File Offset: 0x0000CCFE
		protected override void OnDisconnectedFromRoot()
		{
			base.Context.EventManager.OnDragStarted -= this.OnDragStarted;
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x0000EB1C File Offset: 0x0000CD1C
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

		// Token: 0x060004CC RID: 1228 RVA: 0x0000EB8C File Offset: 0x0000CD8C
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

		// Token: 0x060004CD RID: 1229 RVA: 0x0000EC74 File Offset: 0x0000CE74
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

		// Token: 0x060004CE RID: 1230 RVA: 0x0000ED50 File Offset: 0x0000CF50
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

		// Token: 0x060004CF RID: 1231 RVA: 0x0000EF38 File Offset: 0x0000D138
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

		// Token: 0x060004D0 RID: 1232 RVA: 0x0000EFB4 File Offset: 0x0000D1B4
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

		// Token: 0x060004D1 RID: 1233 RVA: 0x0000F033 File Offset: 0x0000D233
		private void OnDragStarted()
		{
			this.RemoveZeroCountItems();
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0000F03B File Offset: 0x0000D23B
		private void RemoveZeroCountItems()
		{
			base.EventFired("RemoveZeroCounts", Array.Empty<object>());
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0000F050 File Offset: 0x0000D250
		private void UpdateScrollTarget()
		{
			PartyTroopTupleButtonWidget currentOtherTuple = this._currentOtherTuple;
			if (((currentOtherTuple != null) ? currentOtherTuple.ParentWidget : null) != null)
			{
				(this._currentOtherTuple.IsTupleLeftSide ? this.OtherScrollPanel : this.MainScrollPanel).ScrollToChild(this._currentOtherTuple, -1f, 1f, 0, 400, 0.35f, 0f);
			}
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0000F0B1 File Offset: 0x0000D2B1
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

		// Token: 0x060004D5 RID: 1237 RVA: 0x0000F0DC File Offset: 0x0000D2DC
		private void OnNewTroopAdded(Widget parent, Widget addedChild)
		{
			PartyTroopTupleButtonWidget partyTroopTupleButtonWidget;
			if (this._currentMainTuple != null && (partyTroopTupleButtonWidget = addedChild as PartyTroopTupleButtonWidget) != null)
			{
				this._newAddedTroop = partyTroopTupleButtonWidget;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x060004D6 RID: 1238 RVA: 0x0000F102 File Offset: 0x0000D302
		// (set) Token: 0x060004D7 RID: 1239 RVA: 0x0000F10A File Offset: 0x0000D30A
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

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x060004D8 RID: 1240 RVA: 0x0000F11C File Offset: 0x0000D31C
		// (set) Token: 0x060004D9 RID: 1241 RVA: 0x0000F124 File Offset: 0x0000D324
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

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x060004DA RID: 1242 RVA: 0x0000F136 File Offset: 0x0000D336
		// (set) Token: 0x060004DB RID: 1243 RVA: 0x0000F140 File Offset: 0x0000D340
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

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x060004DC RID: 1244 RVA: 0x0000F19F File Offset: 0x0000D39F
		// (set) Token: 0x060004DD RID: 1245 RVA: 0x0000F1A8 File Offset: 0x0000D3A8
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

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x060004DE RID: 1246 RVA: 0x0000F207 File Offset: 0x0000D407
		// (set) Token: 0x060004DF RID: 1247 RVA: 0x0000F20F File Offset: 0x0000D40F
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

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x060004E0 RID: 1248 RVA: 0x0000F22D File Offset: 0x0000D42D
		// (set) Token: 0x060004E1 RID: 1249 RVA: 0x0000F235 File Offset: 0x0000D435
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

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x060004E2 RID: 1250 RVA: 0x0000F259 File Offset: 0x0000D459
		// (set) Token: 0x060004E3 RID: 1251 RVA: 0x0000F261 File Offset: 0x0000D461
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

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x060004E4 RID: 1252 RVA: 0x0000F285 File Offset: 0x0000D485
		// (set) Token: 0x060004E5 RID: 1253 RVA: 0x0000F28D File Offset: 0x0000D48D
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

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x060004E6 RID: 1254 RVA: 0x0000F2B1 File Offset: 0x0000D4B1
		// (set) Token: 0x060004E7 RID: 1255 RVA: 0x0000F2B9 File Offset: 0x0000D4B9
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

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x060004E8 RID: 1256 RVA: 0x0000F2DD File Offset: 0x0000D4DD
		// (set) Token: 0x060004E9 RID: 1257 RVA: 0x0000F2E5 File Offset: 0x0000D4E5
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

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x060004EA RID: 1258 RVA: 0x0000F309 File Offset: 0x0000D509
		// (set) Token: 0x060004EB RID: 1259 RVA: 0x0000F311 File Offset: 0x0000D511
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

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x060004EC RID: 1260 RVA: 0x0000F335 File Offset: 0x0000D535
		// (set) Token: 0x060004ED RID: 1261 RVA: 0x0000F33D File Offset: 0x0000D53D
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

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x060004EE RID: 1262 RVA: 0x0000F366 File Offset: 0x0000D566
		// (set) Token: 0x060004EF RID: 1263 RVA: 0x0000F36E File Offset: 0x0000D56E
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

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x060004F0 RID: 1264 RVA: 0x0000F397 File Offset: 0x0000D597
		// (set) Token: 0x060004F1 RID: 1265 RVA: 0x0000F39F File Offset: 0x0000D59F
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

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x060004F2 RID: 1266 RVA: 0x0000F3C8 File Offset: 0x0000D5C8
		// (set) Token: 0x060004F3 RID: 1267 RVA: 0x0000F3D0 File Offset: 0x0000D5D0
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

		// Token: 0x060004F4 RID: 1268 RVA: 0x0000F3FC File Offset: 0x0000D5FC
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

		// Token: 0x04000210 RID: 528
		private PartyTroopTupleButtonWidget _currentMainTuple;

		// Token: 0x04000211 RID: 529
		private PartyTroopTupleButtonWidget _currentOtherTuple;

		// Token: 0x04000214 RID: 532
		private InputKeyVisualWidget _takeAllPrisonersInputKeyVisual;

		// Token: 0x04000215 RID: 533
		private InputKeyVisualWidget _dismissAllPrisonersInputKeyVisual;

		// Token: 0x04000217 RID: 535
		private PartyTroopTupleButtonWidget _newAddedTroop;

		// Token: 0x04000218 RID: 536
		private Widget _upgradePopupParent;

		// Token: 0x04000219 RID: 537
		private Widget _recruitPopupParent;

		// Token: 0x0400021A RID: 538
		private Widget _takeAllPrisonersInputKeyVisualParent;

		// Token: 0x0400021B RID: 539
		private Widget _dismissAllPrisonersInputKeyVisualParent;

		// Token: 0x0400021C RID: 540
		private int _mainPartyTroopSize;

		// Token: 0x0400021D RID: 541
		private bool _isPrisonerWarningEnabled;

		// Token: 0x0400021E RID: 542
		private bool _isTroopWarningEnabled;

		// Token: 0x0400021F RID: 543
		private bool _isOtherTroopWarningEnabled;

		// Token: 0x04000220 RID: 544
		private TextWidget _troopLabel;

		// Token: 0x04000221 RID: 545
		private TextWidget _prisonerLabel;

		// Token: 0x04000222 RID: 546
		private TextWidget _otherTroopLabel;

		// Token: 0x04000223 RID: 547
		private ListPanel _otherMemberList;

		// Token: 0x04000224 RID: 548
		private ListPanel _otherPrisonerList;

		// Token: 0x04000225 RID: 549
		private ListPanel _mainMemberList;

		// Token: 0x04000226 RID: 550
		private ListPanel _mainPrisonerList;
	}
}
