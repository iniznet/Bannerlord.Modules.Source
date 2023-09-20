using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.Compass;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.HUDExtensions
{
	public class CapturePointVM : CompassTargetVM
	{
		public CapturePointVM(FlagCapturePoint target, TargetIconType iconType)
			: base(iconType, 0U, 0U, null, false, false)
		{
			this.Target = target;
			foreach (string text in this.Target.GameEntity.Tags)
			{
				if (text.StartsWith("enable_") || text.StartsWith("disable_"))
				{
					this.IsSpawnAffectorFlag = true;
				}
			}
			if (this.Target.GameEntity.HasTag("keep_capture_point"))
			{
				this.IsKeepFlag = true;
			}
			this.ResetFlag();
		}

		public override void Refresh(float circleX, float x, float distance)
		{
			base.Refresh(circleX, x, distance);
			this.FlagProgress = this.Target.GetFlagProgress();
		}

		public void OnOwnerChanged(Team newTeam)
		{
			uint num = ((newTeam != null) ? newTeam.Color : 4284111450U);
			uint num2 = ((newTeam != null) ? newTeam.Color2 : uint.MaxValue);
			base.RefreshColor(num, num2);
		}

		public void ResetFlag()
		{
			this.OnOwnerChanged(null);
		}

		internal void OnRemainingMoraleChanged(int remainingMorale)
		{
			if (this.RemainingRemovalTime != remainingMorale && remainingMorale != 90)
			{
				this.RemainingRemovalTime = (int)((float)remainingMorale / 1f);
			}
		}

		[DataSourceProperty]
		public float FlagProgress
		{
			get
			{
				return this._flagProgress;
			}
			set
			{
				if (value != this._flagProgress)
				{
					this._flagProgress = value;
					base.OnPropertyChangedWithValue(value, "FlagProgress");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSpawnAffectorFlag
		{
			get
			{
				return this._isSpawnAffectorFlag;
			}
			set
			{
				if (value != this._isSpawnAffectorFlag)
				{
					this._isSpawnAffectorFlag = value;
					base.OnPropertyChangedWithValue(value, "IsSpawnAffectorFlag");
				}
			}
		}

		[DataSourceProperty]
		public bool IsKeepFlag
		{
			get
			{
				return this._isKeepFlag;
			}
			set
			{
				if (value != this._isKeepFlag)
				{
					this._isKeepFlag = value;
					base.OnPropertyChangedWithValue(value, "IsKeepFlag");
				}
			}
		}

		[DataSourceProperty]
		public int RemainingRemovalTime
		{
			get
			{
				return this._remainingRemovalTime;
			}
			set
			{
				if (value != this._remainingRemovalTime)
				{
					this._remainingRemovalTime = value;
					base.OnPropertyChangedWithValue(value, "RemainingRemovalTime");
				}
			}
		}

		public readonly FlagCapturePoint Target;

		private float _flagProgress;

		private int _remainingRemovalTime = -1;

		private bool _isKeepFlag;

		private bool _isSpawnAffectorFlag;
	}
}
