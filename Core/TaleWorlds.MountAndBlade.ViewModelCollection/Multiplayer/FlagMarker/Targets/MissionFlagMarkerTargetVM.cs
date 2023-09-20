using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FlagMarker.Targets
{
	// Token: 0x020000BE RID: 190
	public class MissionFlagMarkerTargetVM : MissionMarkerTargetVM
	{
		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x06001235 RID: 4661 RVA: 0x0003BFEB File Offset: 0x0003A1EB
		// (set) Token: 0x06001236 RID: 4662 RVA: 0x0003BFF3 File Offset: 0x0003A1F3
		public FlagCapturePoint TargetFlag { get; private set; }

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x06001237 RID: 4663 RVA: 0x0003BFFC File Offset: 0x0003A1FC
		public override Vec3 WorldPosition
		{
			get
			{
				if (this.TargetFlag != null)
				{
					return this.TargetFlag.Position;
				}
				Debug.FailedAssert("No target found!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\FlagMarker\\Targets\\MissionFlagMarkerTargetVM.cs", "WorldPosition", 24);
				return Vec3.One;
			}
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06001238 RID: 4664 RVA: 0x0003C02D File Offset: 0x0003A22D
		protected override float HeightOffset
		{
			get
			{
				return 2f;
			}
		}

		// Token: 0x06001239 RID: 4665 RVA: 0x0003C034 File Offset: 0x0003A234
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

		// Token: 0x0600123A RID: 4666 RVA: 0x0003C0D8 File Offset: 0x0003A2D8
		private Vec3 Vector3Maxamize(Vec3 vector)
		{
			float num = 0f;
			num = ((vector.x > num) ? vector.x : num);
			num = ((vector.y > num) ? vector.y : num);
			num = ((vector.z > num) ? vector.z : num);
			return vector / num;
		}

		// Token: 0x0600123B RID: 4667 RVA: 0x0003C12C File Offset: 0x0003A32C
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

		// Token: 0x0600123C RID: 4668 RVA: 0x0003C288 File Offset: 0x0003A488
		public void OnOwnerChanged(Team team)
		{
			bool flag = team == null || team.TeamIndex == -1;
			uint num = (flag ? 4284111450U : team.Color);
			uint num2 = (flag ? uint.MaxValue : team.Color2);
			base.RefreshColor(num, num2);
		}

		// Token: 0x0600123D RID: 4669 RVA: 0x0003C2C9 File Offset: 0x0003A4C9
		public void OnRemainingMoraleChanged(int remainingMorale)
		{
			if (this.RemainingRemovalTime != remainingMorale && remainingMorale != 90)
			{
				this.RemainingRemovalTime = (int)((float)remainingMorale / 1f);
			}
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x0600123E RID: 4670 RVA: 0x0003C2E8 File Offset: 0x0003A4E8
		// (set) Token: 0x0600123F RID: 4671 RVA: 0x0003C2F0 File Offset: 0x0003A4F0
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

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x06001240 RID: 4672 RVA: 0x0003C30E File Offset: 0x0003A50E
		// (set) Token: 0x06001241 RID: 4673 RVA: 0x0003C316 File Offset: 0x0003A516
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

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x06001242 RID: 4674 RVA: 0x0003C334 File Offset: 0x0003A534
		// (set) Token: 0x06001243 RID: 4675 RVA: 0x0003C33C File Offset: 0x0003A53C
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

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x06001244 RID: 4676 RVA: 0x0003C35A File Offset: 0x0003A55A
		// (set) Token: 0x06001245 RID: 4677 RVA: 0x0003C362 File Offset: 0x0003A562
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

		// Token: 0x040008B3 RID: 2227
		private bool _isKeepFlag;

		// Token: 0x040008B4 RID: 2228
		private bool _isSpawnAffectorFlag;

		// Token: 0x040008B5 RID: 2229
		private float _flagProgress;

		// Token: 0x040008B6 RID: 2230
		private int _remainingRemovalTime = -1;
	}
}
