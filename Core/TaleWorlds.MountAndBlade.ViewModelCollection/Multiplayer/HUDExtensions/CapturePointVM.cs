using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.Compass;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.HUDExtensions
{
	// Token: 0x020000B7 RID: 183
	public class CapturePointVM : CompassTargetVM
	{
		// Token: 0x06001140 RID: 4416 RVA: 0x00038D64 File Offset: 0x00036F64
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

		// Token: 0x06001141 RID: 4417 RVA: 0x00038DF3 File Offset: 0x00036FF3
		public override void Refresh(float circleX, float x, float distance)
		{
			base.Refresh(circleX, x, distance);
			this.FlagProgress = this.Target.GetFlagProgress();
		}

		// Token: 0x06001142 RID: 4418 RVA: 0x00038E10 File Offset: 0x00037010
		public void OnOwnerChanged(Team newTeam)
		{
			uint num = ((newTeam != null) ? newTeam.Color : 4284111450U);
			uint num2 = ((newTeam != null) ? newTeam.Color2 : uint.MaxValue);
			base.RefreshColor(num, num2);
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x00038E43 File Offset: 0x00037043
		public void ResetFlag()
		{
			this.OnOwnerChanged(null);
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x00038E4C File Offset: 0x0003704C
		internal void OnRemainingMoraleChanged(int remainingMorale)
		{
			if (this.RemainingRemovalTime != remainingMorale && remainingMorale != 90)
			{
				this.RemainingRemovalTime = (int)((float)remainingMorale / 1f);
			}
		}

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x06001145 RID: 4421 RVA: 0x00038E6B File Offset: 0x0003706B
		// (set) Token: 0x06001146 RID: 4422 RVA: 0x00038E73 File Offset: 0x00037073
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

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x06001147 RID: 4423 RVA: 0x00038E91 File Offset: 0x00037091
		// (set) Token: 0x06001148 RID: 4424 RVA: 0x00038E99 File Offset: 0x00037099
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

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x06001149 RID: 4425 RVA: 0x00038EB7 File Offset: 0x000370B7
		// (set) Token: 0x0600114A RID: 4426 RVA: 0x00038EBF File Offset: 0x000370BF
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

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x0600114B RID: 4427 RVA: 0x00038EDD File Offset: 0x000370DD
		// (set) Token: 0x0600114C RID: 4428 RVA: 0x00038EE5 File Offset: 0x000370E5
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

		// Token: 0x04000836 RID: 2102
		public readonly FlagCapturePoint Target;

		// Token: 0x04000837 RID: 2103
		private float _flagProgress;

		// Token: 0x04000838 RID: 2104
		private int _remainingRemovalTime = -1;

		// Token: 0x04000839 RID: 2105
		private bool _isKeepFlag;

		// Token: 0x0400083A RID: 2106
		private bool _isSpawnAffectorFlag;
	}
}
