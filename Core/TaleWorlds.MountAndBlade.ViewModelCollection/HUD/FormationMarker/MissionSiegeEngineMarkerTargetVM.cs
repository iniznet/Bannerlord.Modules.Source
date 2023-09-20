using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker
{
	// Token: 0x020000ED RID: 237
	public class MissionSiegeEngineMarkerTargetVM : ViewModel
	{
		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x06001521 RID: 5409 RVA: 0x00044FBF File Offset: 0x000431BF
		// (set) Token: 0x06001522 RID: 5410 RVA: 0x00044FC7 File Offset: 0x000431C7
		public SiegeWeapon Engine { get; private set; }

		// Token: 0x06001523 RID: 5411 RVA: 0x00044FD0 File Offset: 0x000431D0
		public MissionSiegeEngineMarkerTargetVM(SiegeWeapon engine, bool isEnemy)
		{
			this.Engine = engine;
			this.EngineType = this.Engine.GetSiegeEngineType().StringId;
			this.IsEnemy = isEnemy;
		}

		// Token: 0x06001524 RID: 5412 RVA: 0x00044FFC File Offset: 0x000431FC
		public void Refresh()
		{
			this.HitPoints = MathF.Ceiling(this.Engine.DestructionComponent.HitPoint / this.Engine.DestructionComponent.MaxHitPoint * 100f);
		}

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x06001525 RID: 5413 RVA: 0x00045030 File Offset: 0x00043230
		// (set) Token: 0x06001526 RID: 5414 RVA: 0x00045038 File Offset: 0x00043238
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (this._isEnabled != value)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x06001527 RID: 5415 RVA: 0x00045056 File Offset: 0x00043256
		// (set) Token: 0x06001528 RID: 5416 RVA: 0x0004505E File Offset: 0x0004325E
		[DataSourceProperty]
		public bool IsEnemy
		{
			get
			{
				return this._isEnemy;
			}
			set
			{
				if (this._isEnemy != value)
				{
					this._isEnemy = value;
					base.OnPropertyChangedWithValue(value, "IsEnemy");
				}
			}
		}

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x06001529 RID: 5417 RVA: 0x0004507C File Offset: 0x0004327C
		// (set) Token: 0x0600152A RID: 5418 RVA: 0x00045084 File Offset: 0x00043284
		[DataSourceProperty]
		public string EngineType
		{
			get
			{
				return this._engineType;
			}
			set
			{
				if (this._engineType != value)
				{
					this._engineType = value;
					base.OnPropertyChangedWithValue<string>(value, "EngineType");
				}
			}
		}

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x0600152B RID: 5419 RVA: 0x000450A7 File Offset: 0x000432A7
		// (set) Token: 0x0600152C RID: 5420 RVA: 0x000450AF File Offset: 0x000432AF
		[DataSourceProperty]
		public bool IsBehind
		{
			get
			{
				return this._isBehind;
			}
			set
			{
				if (this._isBehind != value)
				{
					this._isBehind = value;
					base.OnPropertyChangedWithValue(value, "IsBehind");
				}
			}
		}

		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x0600152D RID: 5421 RVA: 0x000450CD File Offset: 0x000432CD
		// (set) Token: 0x0600152E RID: 5422 RVA: 0x000450D5 File Offset: 0x000432D5
		[DataSourceProperty]
		public Vec2 ScreenPosition
		{
			get
			{
				return this._screenPosition;
			}
			set
			{
				if (value.x != this._screenPosition.x || value.y != this._screenPosition.y)
				{
					this._screenPosition = value;
					base.OnPropertyChangedWithValue(value, "ScreenPosition");
				}
			}
		}

		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x0600152F RID: 5423 RVA: 0x00045110 File Offset: 0x00043310
		// (set) Token: 0x06001530 RID: 5424 RVA: 0x00045118 File Offset: 0x00043318
		[DataSourceProperty]
		public float Distance
		{
			get
			{
				return this._distance;
			}
			set
			{
				if (this._distance != value && !float.IsNaN(value))
				{
					this._distance = value;
					base.OnPropertyChangedWithValue(value, "Distance");
				}
			}
		}

		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x06001531 RID: 5425 RVA: 0x0004513E File Offset: 0x0004333E
		// (set) Token: 0x06001532 RID: 5426 RVA: 0x00045146 File Offset: 0x00043346
		[DataSourceProperty]
		public int HitPoints
		{
			get
			{
				return this._hitPoints;
			}
			set
			{
				if (this._hitPoints != value)
				{
					this._hitPoints = value;
					base.OnPropertyChangedWithValue(value, "HitPoints");
				}
			}
		}

		// Token: 0x04000A20 RID: 2592
		private Vec2 _screenPosition;

		// Token: 0x04000A21 RID: 2593
		private float _distance;

		// Token: 0x04000A22 RID: 2594
		private bool _isEnabled;

		// Token: 0x04000A23 RID: 2595
		private bool _isBehind;

		// Token: 0x04000A24 RID: 2596
		private bool _isEnemy;

		// Token: 0x04000A25 RID: 2597
		private string _engineType;

		// Token: 0x04000A26 RID: 2598
		private int _hitPoints;
	}
}
