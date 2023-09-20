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
			this.FormationType = MissionFormationMarkerTargetVM.GetFormationType(this.Formation.RepresentativeClass);
		}

		public void Refresh()
		{
			this.Size = this.Formation.CountOfUnits;
		}

		public void SetTargetedState(bool isFocused, bool isTargetingAFormation)
		{
			this.IsCenterOfFocus = isFocused;
			this.IsTargetingAFormation = isTargetingAFormation;
		}

		public static string GetFormationType(FormationClass formationType)
		{
			switch (formationType)
			{
			case FormationClass.Infantry:
				return "Infantry_Light";
			case FormationClass.Ranged:
				return "Archer_Light";
			case FormationClass.Cavalry:
				return "Cavalry_Light";
			case FormationClass.HorseArcher:
				return "HorseArcher_Light";
			case FormationClass.NumberOfDefaultFormations:
			case FormationClass.HeavyInfantry:
			case FormationClass.NumberOfRegularFormations:
			case FormationClass.Bodyguard:
			case FormationClass.NumberOfAllFormations:
				return "Infantry_Heavy";
			case FormationClass.LightCavalry:
				return "Cavalry_Light";
			case FormationClass.HeavyCavalry:
				return "Cavalry_Heavy";
			default:
				return "None";
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
		public bool IsCenterOfFocus
		{
			get
			{
				return this._isCenterOfFocus;
			}
			set
			{
				if (this._isCenterOfFocus != value)
				{
					this._isCenterOfFocus = value;
					base.OnPropertyChangedWithValue(value, "IsCenterOfFocus");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFormationTargetRelevant
		{
			get
			{
				return this._isFormationTargetRelevant;
			}
			set
			{
				if (this._isFormationTargetRelevant != value)
				{
					this._isFormationTargetRelevant = value;
					base.OnPropertyChangedWithValue(value, "IsFormationTargetRelevant");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTargetingAFormation
		{
			get
			{
				return this._isTargetingAFormation;
			}
			set
			{
				if (this._isTargetingAFormation != value)
				{
					this._isTargetingAFormation = value;
					base.OnPropertyChangedWithValue(value, "IsTargetingAFormation");
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
		public bool IsInsideScreenBoundaries
		{
			get
			{
				return this._isInsideScreenBoundaries;
			}
			set
			{
				if (this._isInsideScreenBoundaries != value)
				{
					this._isInsideScreenBoundaries = value;
					base.OnPropertyChangedWithValue(value, "IsInsideScreenBoundaries");
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

		[DataSourceProperty]
		public int WSign
		{
			get
			{
				return this._wSign;
			}
			set
			{
				if (this._wSign != value)
				{
					this._wSign = value;
					base.OnPropertyChangedWithValue(value, "WSign");
				}
			}
		}

		private Vec2 _screenPosition;

		private float _distance;

		private bool _isEnabled;

		private bool _isInsideScreenBoundaries;

		private bool _isCenterOfFocus;

		private bool _isFormationTargetRelevant;

		private bool _isTargetingAFormation;

		private int _teamType;

		private int _size;

		private int _wSign;

		private string _formationType;

		public enum TeamTypes
		{
			PlayerTeam,
			PlayerAllyTeam,
			EnemyTeam
		}
	}
}
