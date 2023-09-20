using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.FlagMarker.Targets
{
	public class MissionFlagMarkerTargetVM : MissionMarkerTargetVM
	{
		public FlagCapturePoint TargetFlag { get; private set; }

		public override Vec3 WorldPosition
		{
			get
			{
				if (this.TargetFlag != null)
				{
					return this.TargetFlag.Position;
				}
				Debug.FailedAssert("No target found!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\FlagMarker\\Targets\\MissionFlagMarkerTargetVM.cs", "WorldPosition", 24);
				return Vec3.One;
			}
		}

		protected override float HeightOffset
		{
			get
			{
				return 2f;
			}
		}

		public MissionFlagMarkerTargetVM(FlagCapturePoint flag)
			: base(MissionMarkerType.Flag)
		{
			this.TargetFlag = flag;
			base.Name = Convert.ToChar(flag.FlagChar).ToString();
			foreach (string text in this.TargetFlag.GameEntity.Tags)
			{
				if (text.StartsWith("enable_") || text.StartsWith("disable_"))
				{
					this.IsSpawnAffectorFlag = true;
				}
			}
			if (this.TargetFlag.GameEntity.HasTag("keep_capture_point"))
			{
				this.IsKeepFlag = true;
			}
			this.OnOwnerChanged(null);
		}

		private Vec3 Vector3Maxamize(Vec3 vector)
		{
			float num = 0f;
			num = ((vector.x > num) ? vector.x : num);
			num = ((vector.y > num) ? vector.y : num);
			num = ((vector.z > num) ? vector.z : num);
			return vector / num;
		}

		public override void UpdateScreenPosition(Camera missionCamera)
		{
			Vec3 worldPosition = this.WorldPosition;
			worldPosition.z += this.HeightOffset;
			Vec3 vec = missionCamera.WorldPointToViewPortPoint(ref worldPosition);
			vec.y = 1f - vec.y;
			if (vec.z < 0f)
			{
				vec.x = 1f - vec.x;
				vec.y = 1f - vec.y;
				vec.z = 0f;
				vec = this.Vector3Maxamize(vec);
			}
			if (float.IsPositiveInfinity(vec.x))
			{
				vec.x = 1f;
			}
			else if (float.IsNegativeInfinity(vec.x))
			{
				vec.x = 0f;
			}
			if (float.IsPositiveInfinity(vec.y))
			{
				vec.y = 1f;
			}
			else if (float.IsNegativeInfinity(vec.y))
			{
				vec.y = 0f;
			}
			vec.x = MathF.Clamp(vec.x, 0f, 1f) * Screen.RealScreenResolutionWidth;
			vec.y = MathF.Clamp(vec.y, 0f, 1f) * Screen.RealScreenResolutionHeight;
			base.ScreenPosition = new Vec2(vec.x, vec.y);
			this.FlagProgress = this.TargetFlag.GetFlagProgress();
		}

		public void OnOwnerChanged(Team team)
		{
			bool flag = team == null || team.TeamIndex == -1;
			uint num = (flag ? 4284111450U : team.Color);
			uint num2 = (flag ? uint.MaxValue : team.Color2);
			base.RefreshColor(num, num2);
		}

		public void OnRemainingMoraleChanged(int remainingMorale)
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

		private bool _isKeepFlag;

		private bool _isSpawnAffectorFlag;

		private float _flagProgress;

		private int _remainingRemovalTime = -1;
	}
}
