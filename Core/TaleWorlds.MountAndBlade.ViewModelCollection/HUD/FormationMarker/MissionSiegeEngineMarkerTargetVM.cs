using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker
{
	public class MissionSiegeEngineMarkerTargetVM : ViewModel
	{
		public SiegeWeapon Engine { get; private set; }

		public MissionSiegeEngineMarkerTargetVM(SiegeWeapon engine, bool isEnemy)
		{
			this.Engine = engine;
			this.EngineType = this.Engine.GetSiegeEngineType().StringId;
			this.IsEnemy = isEnemy;
		}

		public void Refresh()
		{
			this.HitPoints = MathF.Ceiling(this.Engine.DestructionComponent.HitPoint / this.Engine.DestructionComponent.MaxHitPoint * 100f);
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

		private Vec2 _screenPosition;

		private float _distance;

		private bool _isEnabled;

		private bool _isBehind;

		private bool _isEnemy;

		private string _engineType;

		private int _hitPoints;
	}
}
