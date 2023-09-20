using System;
using System.Collections.ObjectModel;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class CrosshairVM : ViewModel
	{
		public CrosshairVM()
		{
			this.ReloadPhases = new MBBindingList<ReloadPhaseItemVM>();
		}

		public void SetProperties(double accuracy, double scale)
		{
			this.CrosshairAccuracy = accuracy;
			this.CrosshairScale = scale;
		}

		public void SetArrowProperties(double topArrowOpacity, double rightArrowOpacity, double bottomArrowOpacity, double leftArrowOpacity)
		{
			this.TopArrowOpacity = topArrowOpacity;
			this.BottomArrowOpacity = bottomArrowOpacity;
			this.RightArrowOpacity = rightArrowOpacity;
			this.LeftArrowOpacity = leftArrowOpacity;
		}

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

		public void ShowHitMarker(bool isVictimDead, bool isHumanoidHeadShot)
		{
			this.IsVictimDead = isVictimDead;
			this.IsHitMarkerVisible = false;
			this.IsHitMarkerVisible = true;
			this.IsHumanoidHeadshot = false;
			this.IsHumanoidHeadshot = isHumanoidHeadShot;
		}

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

		private bool _isVisible;

		private bool _isReloadPhasesVisible;

		private bool _isHitMarkerVisible;

		private bool _isVictimDead;

		private bool _isHumanoidHeadshot;

		private bool _isTargetInvalid;

		private MBBindingList<ReloadPhaseItemVM> _reloadPhases;

		private double _crosshairAccuracy;

		private double _crosshairScale;

		private double _topArrowOpacity;

		private double _bottomArrowOpacity;

		private double _rightArrowOpacity;

		private double _leftArrowOpacity;

		private int _crosshairType;
	}
}
