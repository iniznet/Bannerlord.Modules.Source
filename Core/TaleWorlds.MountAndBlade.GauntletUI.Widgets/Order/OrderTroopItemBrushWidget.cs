using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order
{
	// Token: 0x02000068 RID: 104
	public class OrderTroopItemBrushWidget : BrushWidget
	{
		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000580 RID: 1408 RVA: 0x00010A68 File Offset: 0x0000EC68
		// (set) Token: 0x06000581 RID: 1409 RVA: 0x00010A70 File Offset: 0x0000EC70
		public Widget SelectionFrameWidget { get; set; }

		// Token: 0x06000582 RID: 1410 RVA: 0x00010A79 File Offset: 0x0000EC79
		public OrderTroopItemBrushWidget(UIContext context)
			: base(context)
		{
			base.AddState("Selected");
			base.AddState("Disabled");
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x00010A9F File Offset: 0x0000EC9F
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.SelectionFrameWidget != null)
			{
				this.SelectionFrameWidget.IsVisible = base.EventManager.IsControllerActive && this.IsSelectionActive;
			}
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x00010AD1 File Offset: 0x0000ECD1
		private void SelectionStateChanged()
		{
			this.UpdateBackgroundState();
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x00010AD9 File Offset: 0x0000ECD9
		private void SelectableStateChanged()
		{
			this.UpdateBackgroundState();
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x00010AE1 File Offset: 0x0000ECE1
		private void CurrentMemberCountChanged()
		{
			this.UpdateBackgroundState();
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x00010AE9 File Offset: 0x0000ECE9
		private void UpdateBackgroundState()
		{
			if (this.CurrentMemberCount <= 0 || !this.IsSelectable)
			{
				this.SetState("Disabled");
				return;
			}
			this.SetState(this.IsSelected ? "Selected" : "Default");
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x00010B22 File Offset: 0x0000ED22
		private void UpdateBrush()
		{
			if (this.MeleeCardBrush == null || this.RangedCardBrush == null)
			{
				return;
			}
			if (this.HasAmmo)
			{
				base.Brush = this.RangedCardBrush;
				return;
			}
			base.Brush = this.MeleeCardBrush;
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000589 RID: 1417 RVA: 0x00010B56 File Offset: 0x0000ED56
		// (set) Token: 0x0600058A RID: 1418 RVA: 0x00010B5E File Offset: 0x0000ED5E
		[Editor(false)]
		public int CurrentMemberCount
		{
			get
			{
				return this._currentMemberCount;
			}
			set
			{
				if (this._currentMemberCount != value)
				{
					this._currentMemberCount = value;
					base.OnPropertyChanged(value, "CurrentMemberCount");
					this.CurrentMemberCountChanged();
				}
			}
		}

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x0600058B RID: 1419 RVA: 0x00010B82 File Offset: 0x0000ED82
		// (set) Token: 0x0600058C RID: 1420 RVA: 0x00010B8A File Offset: 0x0000ED8A
		[Editor(false)]
		public bool IsSelectable
		{
			get
			{
				return this._isSelectable;
			}
			set
			{
				if (this._isSelectable != value)
				{
					this._isSelectable = value;
					base.OnPropertyChanged(value, "IsSelectable");
					this.SelectableStateChanged();
				}
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x0600058D RID: 1421 RVA: 0x00010BAE File Offset: 0x0000EDAE
		// (set) Token: 0x0600058E RID: 1422 RVA: 0x00010BB6 File Offset: 0x0000EDB6
		[Editor(false)]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (this._isSelected != value)
				{
					this._isSelected = value;
					base.OnPropertyChanged(value, "IsSelected");
					this.SelectionStateChanged();
				}
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x0600058F RID: 1423 RVA: 0x00010BDA File Offset: 0x0000EDDA
		// (set) Token: 0x06000590 RID: 1424 RVA: 0x00010BE2 File Offset: 0x0000EDE2
		[Editor(false)]
		public bool IsSelectionActive
		{
			get
			{
				return this._isSelectionActive;
			}
			set
			{
				if (this._isSelectionActive != value)
				{
					this._isSelectionActive = value;
					base.OnPropertyChanged(value, "IsSelectionActive");
				}
			}
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000591 RID: 1425 RVA: 0x00010C00 File Offset: 0x0000EE00
		// (set) Token: 0x06000592 RID: 1426 RVA: 0x00010C08 File Offset: 0x0000EE08
		[Editor(false)]
		public bool HasAmmo
		{
			get
			{
				return this._hasAmmo;
			}
			set
			{
				if (this._hasAmmo != value)
				{
					this._hasAmmo = value;
					base.OnPropertyChanged(value, "HasAmmo");
					this.UpdateBrush();
				}
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000593 RID: 1427 RVA: 0x00010C2C File Offset: 0x0000EE2C
		// (set) Token: 0x06000594 RID: 1428 RVA: 0x00010C34 File Offset: 0x0000EE34
		[Editor(false)]
		public Brush RangedCardBrush
		{
			get
			{
				return this._rangedCardBrush;
			}
			set
			{
				if (value != this._rangedCardBrush)
				{
					this._rangedCardBrush = value;
					base.OnPropertyChanged<Brush>(value, "RangedCardBrush");
					this.UpdateBrush();
				}
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000595 RID: 1429 RVA: 0x00010C58 File Offset: 0x0000EE58
		// (set) Token: 0x06000596 RID: 1430 RVA: 0x00010C60 File Offset: 0x0000EE60
		[Editor(false)]
		public Brush MeleeCardBrush
		{
			get
			{
				return this._meleeCardBrush;
			}
			set
			{
				if (value != this._meleeCardBrush)
				{
					this._meleeCardBrush = value;
					base.OnPropertyChanged<Brush>(value, "MeleeCardBrush");
					this.UpdateBrush();
				}
			}
		}

		// Token: 0x04000262 RID: 610
		private int _currentMemberCount;

		// Token: 0x04000263 RID: 611
		private bool _isSelectable;

		// Token: 0x04000264 RID: 612
		private bool _isSelected;

		// Token: 0x04000265 RID: 613
		private bool _isSelectionActive;

		// Token: 0x04000266 RID: 614
		private bool _hasAmmo = true;

		// Token: 0x04000267 RID: 615
		private Brush _rangedCardBrush;

		// Token: 0x04000268 RID: 616
		private Brush _meleeCardBrush;
	}
}
