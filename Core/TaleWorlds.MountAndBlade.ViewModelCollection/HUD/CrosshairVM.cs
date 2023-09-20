using System;
using System.Collections.ObjectModel;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000D8 RID: 216
	public class CrosshairVM : ViewModel
	{
		// Token: 0x060013FE RID: 5118 RVA: 0x00041996 File Offset: 0x0003FB96
		public CrosshairVM()
		{
			this.ReloadPhases = new MBBindingList<ReloadPhaseItemVM>();
		}

		// Token: 0x060013FF RID: 5119 RVA: 0x000419A9 File Offset: 0x0003FBA9
		public void SetProperties(double accuracy, double scale)
		{
			this.CrosshairAccuracy = accuracy;
			this.CrosshairScale = scale;
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x000419B9 File Offset: 0x0003FBB9
		public void SetArrowProperties(double topArrowOpacity, double rightArrowOpacity, double bottomArrowOpacity, double leftArrowOpacity)
		{
			this.TopArrowOpacity = topArrowOpacity;
			this.BottomArrowOpacity = bottomArrowOpacity;
			this.RightArrowOpacity = rightArrowOpacity;
			this.LeftArrowOpacity = leftArrowOpacity;
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x000419D8 File Offset: 0x0003FBD8
		public void SetReloadProperties(in StackArray.StackArray10FloatFloatTuple reloadPhases, int reloadPhaseCount)
		{
			if (reloadPhaseCount == 0)
			{
				this.IsReloadPhasesVisible = false;
			}
			else
			{
				for (int i = 0; i < reloadPhaseCount; i++)
				{
					StackArray.StackArray10FloatFloatTuple stackArray10FloatFloatTuple = reloadPhases;
					if (stackArray10FloatFloatTuple[i].Item1 < 1f)
					{
						this.IsReloadPhasesVisible = true;
						break;
					}
				}
			}
			this.PopulateReloadPhases(reloadPhases, reloadPhaseCount);
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x00041A2C File Offset: 0x0003FC2C
		private void PopulateReloadPhases(in StackArray.StackArray10FloatFloatTuple reloadPhases, int reloadPhaseCount)
		{
			if (reloadPhaseCount != this.ReloadPhases.Count)
			{
				this.ReloadPhases.Clear();
				for (int i = 0; i < reloadPhaseCount; i++)
				{
					Collection<ReloadPhaseItemVM> reloadPhases2 = this.ReloadPhases;
					StackArray.StackArray10FloatFloatTuple stackArray10FloatFloatTuple = reloadPhases;
					float item = stackArray10FloatFloatTuple[i].Item1;
					stackArray10FloatFloatTuple = reloadPhases;
					reloadPhases2.Add(new ReloadPhaseItemVM(item, stackArray10FloatFloatTuple[i].Item2));
				}
				return;
			}
			for (int j = 0; j < reloadPhaseCount; j++)
			{
				ReloadPhaseItemVM reloadPhaseItemVM = this.ReloadPhases[j];
				StackArray.StackArray10FloatFloatTuple stackArray10FloatFloatTuple = reloadPhases;
				float item2 = stackArray10FloatFloatTuple[j].Item1;
				stackArray10FloatFloatTuple = reloadPhases;
				reloadPhaseItemVM.Update(item2, stackArray10FloatFloatTuple[j].Item2);
			}
		}

		// Token: 0x06001403 RID: 5123 RVA: 0x00041ADC File Offset: 0x0003FCDC
		public void ShowHitMarker(bool isVictimDead, bool isHumanoidHeadShot)
		{
			this.IsVictimDead = isVictimDead;
			this.IsHitMarkerVisible = false;
			this.IsHitMarkerVisible = true;
			this.IsHumanoidHeadshot = false;
			this.IsHumanoidHeadshot = isHumanoidHeadShot;
		}

		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x06001404 RID: 5124 RVA: 0x00041B01 File Offset: 0x0003FD01
		// (set) Token: 0x06001405 RID: 5125 RVA: 0x00041B09 File Offset: 0x0003FD09
		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (value != this._isVisible)
				{
					this._isVisible = value;
					base.OnPropertyChangedWithValue(value, "IsVisible");
				}
			}
		}

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x06001406 RID: 5126 RVA: 0x00041B27 File Offset: 0x0003FD27
		// (set) Token: 0x06001407 RID: 5127 RVA: 0x00041B2F File Offset: 0x0003FD2F
		[DataSourceProperty]
		public bool IsReloadPhasesVisible
		{
			get
			{
				return this._isReloadPhasesVisible;
			}
			set
			{
				if (value != this._isReloadPhasesVisible)
				{
					this._isReloadPhasesVisible = value;
					base.OnPropertyChangedWithValue(value, "IsReloadPhasesVisible");
				}
			}
		}

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x06001408 RID: 5128 RVA: 0x00041B4D File Offset: 0x0003FD4D
		// (set) Token: 0x06001409 RID: 5129 RVA: 0x00041B55 File Offset: 0x0003FD55
		[DataSourceProperty]
		public bool IsHitMarkerVisible
		{
			get
			{
				return this._isHitMarkerVisible;
			}
			set
			{
				if (value != this._isHitMarkerVisible)
				{
					this._isHitMarkerVisible = value;
					base.OnPropertyChangedWithValue(value, "IsHitMarkerVisible");
				}
			}
		}

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x0600140A RID: 5130 RVA: 0x00041B73 File Offset: 0x0003FD73
		// (set) Token: 0x0600140B RID: 5131 RVA: 0x00041B7B File Offset: 0x0003FD7B
		[DataSourceProperty]
		public bool IsVictimDead
		{
			get
			{
				return this._isVictimDead;
			}
			set
			{
				if (value != this._isVictimDead)
				{
					this._isVictimDead = value;
					base.OnPropertyChangedWithValue(value, "IsVictimDead");
				}
			}
		}

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x0600140C RID: 5132 RVA: 0x00041B99 File Offset: 0x0003FD99
		// (set) Token: 0x0600140D RID: 5133 RVA: 0x00041BA1 File Offset: 0x0003FDA1
		[DataSourceProperty]
		public bool IsHumanoidHeadshot
		{
			get
			{
				return this._isHumanoidHeadshot;
			}
			set
			{
				if (value != this._isHumanoidHeadshot)
				{
					this._isHumanoidHeadshot = value;
					base.OnPropertyChangedWithValue(value, "IsHumanoidHeadshot");
				}
			}
		}

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x0600140E RID: 5134 RVA: 0x00041BBF File Offset: 0x0003FDBF
		// (set) Token: 0x0600140F RID: 5135 RVA: 0x00041BC7 File Offset: 0x0003FDC7
		[DataSourceProperty]
		public double TopArrowOpacity
		{
			get
			{
				return this._topArrowOpacity;
			}
			set
			{
				if (value != this._topArrowOpacity)
				{
					this._topArrowOpacity = value;
					base.OnPropertyChangedWithValue(value, "TopArrowOpacity");
				}
			}
		}

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x06001410 RID: 5136 RVA: 0x00041BE5 File Offset: 0x0003FDE5
		// (set) Token: 0x06001411 RID: 5137 RVA: 0x00041BED File Offset: 0x0003FDED
		[DataSourceProperty]
		public MBBindingList<ReloadPhaseItemVM> ReloadPhases
		{
			get
			{
				return this._reloadPhases;
			}
			set
			{
				if (value != this._reloadPhases)
				{
					this._reloadPhases = value;
					base.OnPropertyChangedWithValue<MBBindingList<ReloadPhaseItemVM>>(value, "ReloadPhases");
				}
			}
		}

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x06001412 RID: 5138 RVA: 0x00041C0B File Offset: 0x0003FE0B
		// (set) Token: 0x06001413 RID: 5139 RVA: 0x00041C13 File Offset: 0x0003FE13
		[DataSourceProperty]
		public double BottomArrowOpacity
		{
			get
			{
				return this._bottomArrowOpacity;
			}
			set
			{
				if (value != this._bottomArrowOpacity)
				{
					this._bottomArrowOpacity = value;
					base.OnPropertyChangedWithValue(value, "BottomArrowOpacity");
				}
			}
		}

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x06001414 RID: 5140 RVA: 0x00041C31 File Offset: 0x0003FE31
		// (set) Token: 0x06001415 RID: 5141 RVA: 0x00041C39 File Offset: 0x0003FE39
		[DataSourceProperty]
		public double RightArrowOpacity
		{
			get
			{
				return this._rightArrowOpacity;
			}
			set
			{
				if (value != this._rightArrowOpacity)
				{
					this._rightArrowOpacity = value;
					base.OnPropertyChangedWithValue(value, "RightArrowOpacity");
				}
			}
		}

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x06001416 RID: 5142 RVA: 0x00041C57 File Offset: 0x0003FE57
		// (set) Token: 0x06001417 RID: 5143 RVA: 0x00041C5F File Offset: 0x0003FE5F
		[DataSourceProperty]
		public double LeftArrowOpacity
		{
			get
			{
				return this._leftArrowOpacity;
			}
			set
			{
				if (value != this._leftArrowOpacity)
				{
					this._leftArrowOpacity = value;
					base.OnPropertyChangedWithValue(value, "LeftArrowOpacity");
				}
			}
		}

		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x06001418 RID: 5144 RVA: 0x00041C7D File Offset: 0x0003FE7D
		// (set) Token: 0x06001419 RID: 5145 RVA: 0x00041C85 File Offset: 0x0003FE85
		[DataSourceProperty]
		public bool IsTargetInvalid
		{
			get
			{
				return this._isTargetInvalid;
			}
			set
			{
				if (value != this._isTargetInvalid)
				{
					this._isTargetInvalid = value;
					base.OnPropertyChangedWithValue(value, "IsTargetInvalid");
				}
			}
		}

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x0600141A RID: 5146 RVA: 0x00041CA3 File Offset: 0x0003FEA3
		// (set) Token: 0x0600141B RID: 5147 RVA: 0x00041CAB File Offset: 0x0003FEAB
		[DataSourceProperty]
		public double CrosshairAccuracy
		{
			get
			{
				return this._crosshairAccuracy;
			}
			set
			{
				if (value != this._crosshairAccuracy)
				{
					this._crosshairAccuracy = value;
					base.OnPropertyChangedWithValue(value, "CrosshairAccuracy");
				}
			}
		}

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x0600141C RID: 5148 RVA: 0x00041CC9 File Offset: 0x0003FEC9
		// (set) Token: 0x0600141D RID: 5149 RVA: 0x00041CD1 File Offset: 0x0003FED1
		[DataSourceProperty]
		public double CrosshairScale
		{
			get
			{
				return this._crosshairScale;
			}
			set
			{
				if (value != this._crosshairScale)
				{
					this._crosshairScale = value;
					base.OnPropertyChangedWithValue(value, "CrosshairScale");
				}
			}
		}

		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x0600141E RID: 5150 RVA: 0x00041CEF File Offset: 0x0003FEEF
		// (set) Token: 0x0600141F RID: 5151 RVA: 0x00041CF7 File Offset: 0x0003FEF7
		[DataSourceProperty]
		public int CrosshairType
		{
			get
			{
				return this._crosshairType;
			}
			set
			{
				if (value != this._crosshairType)
				{
					this._crosshairType = value;
					base.OnPropertyChangedWithValue(value, "CrosshairType");
				}
			}
		}

		// Token: 0x04000995 RID: 2453
		private bool _isVisible;

		// Token: 0x04000996 RID: 2454
		private bool _isReloadPhasesVisible;

		// Token: 0x04000997 RID: 2455
		private bool _isHitMarkerVisible;

		// Token: 0x04000998 RID: 2456
		private bool _isVictimDead;

		// Token: 0x04000999 RID: 2457
		private bool _isHumanoidHeadshot;

		// Token: 0x0400099A RID: 2458
		private bool _isTargetInvalid;

		// Token: 0x0400099B RID: 2459
		private MBBindingList<ReloadPhaseItemVM> _reloadPhases;

		// Token: 0x0400099C RID: 2460
		private double _crosshairAccuracy;

		// Token: 0x0400099D RID: 2461
		private double _crosshairScale;

		// Token: 0x0400099E RID: 2462
		private double _topArrowOpacity;

		// Token: 0x0400099F RID: 2463
		private double _bottomArrowOpacity;

		// Token: 0x040009A0 RID: 2464
		private double _rightArrowOpacity;

		// Token: 0x040009A1 RID: 2465
		private double _leftArrowOpacity;

		// Token: 0x040009A2 RID: 2466
		private int _crosshairType;
	}
}
