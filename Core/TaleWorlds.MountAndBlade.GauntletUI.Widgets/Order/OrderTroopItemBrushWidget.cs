using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order
{
	public class OrderTroopItemBrushWidget : BrushWidget
	{
		public Widget SelectionFrameWidget { get; set; }

		public OrderTroopItemBrushWidget(UIContext context)
			: base(context)
		{
			base.AddState("Selected");
			base.AddState("Disabled");
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.SelectionFrameWidget != null)
			{
				this.SelectionFrameWidget.IsVisible = base.EventManager.IsControllerActive && this.IsSelectionActive;
			}
		}

		private void SelectionStateChanged()
		{
			this.UpdateBackgroundState();
		}

		private void SelectableStateChanged()
		{
			this.UpdateBackgroundState();
		}

		private void CurrentMemberCountChanged()
		{
			this.UpdateBackgroundState();
		}

		private void UpdateBackgroundState()
		{
			if (this.CurrentMemberCount <= 0 || !this.IsSelectable)
			{
				this.SetState("Disabled");
				return;
			}
			this.SetState(this.IsSelected ? "Selected" : "Default");
		}

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

		private int _currentMemberCount;

		private bool _isSelectable;

		private bool _isSelected;

		private bool _isSelectionActive;

		private bool _hasAmmo = true;

		private Brush _rangedCardBrush;

		private Brush _meleeCardBrush;
	}
}
