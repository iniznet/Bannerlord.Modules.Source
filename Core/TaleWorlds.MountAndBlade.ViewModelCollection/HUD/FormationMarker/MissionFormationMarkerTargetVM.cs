using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker
{
	public class MissionFormationMarkerTargetVM : ViewModel
	{
		public Formation Formation { get; private set; }

		public MissionFormationMarkerTargetVM(Formation formation)
		{
			this.Formation = formation;
			this.TeamType = (this.Formation.Team.IsPlayerTeam ? 0 : (this.Formation.Team.IsPlayerAlly ? 1 : 2));
			this.FormationType = MissionFormationMarkerTargetVM.GetFormationType(this.Formation.InitialClass);
		}

		public void Refresh()
		{
			this.Size = this.Formation.CountOfUnits;
		}

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

		private Vec2 _screenPosition;

		private float _distance;

		private bool _isEnabled;

		private bool _isBehind;

		private int _teamType;

		private int _size;

		private string _formationType;

		private enum TeamTypes
		{
			PlayerTeam,
			PlayerAllyTeam,
			EnemyTeam
		}
	}
}
