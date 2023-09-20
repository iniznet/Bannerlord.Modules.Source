using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	// Token: 0x020000D1 RID: 209
	public class OrderOfBattleFormationClassLockBrushWidget : BrushWidget
	{
		// Token: 0x06000AA2 RID: 2722 RVA: 0x0001DC5C File Offset: 0x0001BE5C
		public OrderOfBattleFormationClassLockBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x0001DC65 File Offset: 0x0001BE65
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

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06000AA4 RID: 2724 RVA: 0x0001DC90 File Offset: 0x0001BE90
		// (set) Token: 0x06000AA5 RID: 2725 RVA: 0x0001DC98 File Offset: 0x0001BE98
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

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06000AA6 RID: 2726 RVA: 0x0001DCC4 File Offset: 0x0001BEC4
		// (set) Token: 0x06000AA7 RID: 2727 RVA: 0x0001DCCC File Offset: 0x0001BECC
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

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x06000AA8 RID: 2728 RVA: 0x0001DCEA File Offset: 0x0001BEEA
		// (set) Token: 0x06000AA9 RID: 2729 RVA: 0x0001DCF2 File Offset: 0x0001BEF2
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

		// Token: 0x040004D9 RID: 1241
		private bool _isInitialStateSet;

		// Token: 0x040004DA RID: 1242
		private bool _isLocked;

		// Token: 0x040004DB RID: 1243
		private Brush _lockedBrush;

		// Token: 0x040004DC RID: 1244
		private Brush _unlockedBrush;
	}
}
