using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker
{
	// Token: 0x020000EA RID: 234
	public class MissionFormationMarkerTargetVM : ViewModel
	{
		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x060014F6 RID: 5366 RVA: 0x00044420 File Offset: 0x00042620
		// (set) Token: 0x060014F7 RID: 5367 RVA: 0x00044428 File Offset: 0x00042628
		public Formation Formation { get; private set; }

		// Token: 0x060014F8 RID: 5368 RVA: 0x00044434 File Offset: 0x00042634
		public MissionFormationMarkerTargetVM(Formation formation)
		{
			this.Formation = formation;
			this.TeamType = (this.Formation.Team.IsPlayerTeam ? 0 : (this.Formation.Team.IsPlayerAlly ? 1 : 2));
			this.FormationType = MissionFormationMarkerTargetVM.GetFormationType(this.Formation.InitialClass);
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x00044495 File Offset: 0x00042695
		public void Refresh()
		{
			this.Size = this.Formation.CountOfUnits;
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x000444A8 File Offset: 0x000426A8
		private static string GetFormationType(FormationClass formationType)
		{
			switch (formationType)
			{
			case FormationClass.Infantry:
				return TargetIconType.Infantry_Light.ToString();
			case FormationClass.Ranged:
				return TargetIconType.Archer_Light.ToString();
			case FormationClass.Cavalry:
				return TargetIconType.Cavalry_Light.ToString();
			case FormationClass.HorseArcher:
				return TargetIconType.HorseArcher_Light.ToString();
			case FormationClass.NumberOfDefaultFormations:
			case FormationClass.HeavyInfantry:
				return TargetIconType.Infantry_Heavy.ToString();
			case FormationClass.LightCavalry:
				return TargetIconType.Cavalry_Light.ToString();
			case FormationClass.HeavyCavalry:
				return TargetIconType.Cavalry_Heavy.ToString();
			case FormationClass.NumberOfRegularFormations:
			case FormationClass.Bodyguard:
			case FormationClass.NumberOfAllFormations:
				return TargetIconType.Infantry_Heavy.ToString();
			default:
				return TargetIconType.None.ToString();
			}
		}

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x060014FB RID: 5371 RVA: 0x0004457C File Offset: 0x0004277C
		// (set) Token: 0x060014FC RID: 5372 RVA: 0x00044584 File Offset: 0x00042784
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

		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x060014FD RID: 5373 RVA: 0x000445A2 File Offset: 0x000427A2
		// (set) Token: 0x060014FE RID: 5374 RVA: 0x000445AA File Offset: 0x000427AA
		[DataSourceProperty]
		public string FormationType
		{
			get
			{
				return this._formationType;
			}
			set
			{
				if (this._formationType != value)
				{
					this._formationType = value;
					base.OnPropertyChangedWithValue<string>(value, "FormationType");
				}
			}
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x060014FF RID: 5375 RVA: 0x000445CD File Offset: 0x000427CD
		// (set) Token: 0x06001500 RID: 5376 RVA: 0x000445D5 File Offset: 0x000427D5
		[DataSourceProperty]
		public int TeamType
		{
			get
			{
				return this._teamType;
			}
			set
			{
				if (this._teamType != value)
				{
					this._teamType = value;
					base.OnPropertyChangedWithValue(value, "TeamType");
				}
			}
		}

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x06001501 RID: 5377 RVA: 0x000445F3 File Offset: 0x000427F3
		// (set) Token: 0x06001502 RID: 5378 RVA: 0x000445FB File Offset: 0x000427FB
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

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x06001503 RID: 5379 RVA: 0x00044619 File Offset: 0x00042819
		// (set) Token: 0x06001504 RID: 5380 RVA: 0x00044621 File Offset: 0x00042821
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

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x06001505 RID: 5381 RVA: 0x0004465C File Offset: 0x0004285C
		// (set) Token: 0x06001506 RID: 5382 RVA: 0x00044664 File Offset: 0x00042864
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

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x06001507 RID: 5383 RVA: 0x0004468A File Offset: 0x0004288A
		// (set) Token: 0x06001508 RID: 5384 RVA: 0x00044692 File Offset: 0x00042892
		[DataSourceProperty]
		public int Size
		{
			get
			{
				return this._size;
			}
			set
			{
				if (this._size != value)
				{
					this._size = value;
					base.OnPropertyChangedWithValue(value, "Size");
				}
			}
		}

		// Token: 0x04000A04 RID: 2564
		private Vec2 _screenPosition;

		// Token: 0x04000A05 RID: 2565
		private float _distance;

		// Token: 0x04000A06 RID: 2566
		private bool _isEnabled;

		// Token: 0x04000A07 RID: 2567
		private bool _isBehind;

		// Token: 0x04000A08 RID: 2568
		private int _teamType;

		// Token: 0x04000A09 RID: 2569
		private int _size;

		// Token: 0x04000A0A RID: 2570
		private string _formationType;

		// Token: 0x02000235 RID: 565
		private enum TeamTypes
		{
			// Token: 0x04000EE1 RID: 3809
			PlayerTeam,
			// Token: 0x04000EE2 RID: 3810
			PlayerAllyTeam,
			// Token: 0x04000EE3 RID: 3811
			EnemyTeam
		}
	}
}
