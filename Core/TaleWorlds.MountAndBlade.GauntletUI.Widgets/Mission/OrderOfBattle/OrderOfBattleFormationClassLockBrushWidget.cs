using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	public class OrderOfBattleFormationClassLockBrushWidget : BrushWidget
	{
		public OrderOfBattleFormationClassLockBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void OnLockStateSet()
		{
			if (this.IsLocked)
			{
				base.Brush = this.LockedBrush;
			}
			else
			{
				base.Brush = this.UnlockedBrush;
			}
			this._isInitialStateSet = true;
		}

		[Editor(false)]
		public bool IsLocked
		{
			get
			{
				return this._isLocked;
			}
			set
			{
				if (value != this._isLocked || !this._isInitialStateSet)
				{
					this._isLocked = value;
					base.OnPropertyChanged(value, "IsLocked");
					this.OnLockStateSet();
				}
			}
		}

		[Editor(false)]
		public Brush LockedBrush
		{
			get
			{
				return this._lockedBrush;
			}
			set
			{
				if (value != this._lockedBrush)
				{
					this._lockedBrush = value;
					base.OnPropertyChanged<Brush>(value, "LockedBrush");
				}
			}
		}

		[Editor(false)]
		public Brush UnlockedBrush
		{
			get
			{
				return this._unlockedBrush;
			}
			set
			{
				if (value != this._unlockedBrush)
				{
					this._unlockedBrush = value;
					base.OnPropertyChanged<Brush>(value, "UnlockedBrush");
				}
			}
		}

		private bool _isInitialStateSet;

		private bool _isLocked;

		private Brush _lockedBrush;

		private Brush _unlockedBrush;
	}
}
