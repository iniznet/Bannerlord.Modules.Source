using System;

namespace TaleWorlds.Core
{
	public class DefaultSiegeEngineTypes
	{
		private static DefaultSiegeEngineTypes Instance
		{
			get
			{
				return Game.Current.DefaultSiegeEngineTypes;
			}
		}

		public static SiegeEngineType Preparations
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypePreparations;
			}
		}

		public static SiegeEngineType Ladder
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeLadder;
			}
		}

		public static SiegeEngineType Ballista
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeBallista;
			}
		}

		public static SiegeEngineType FireBallista
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeFireBallista;
			}
		}

		public static SiegeEngineType Ram
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeRam;
			}
		}

		public static SiegeEngineType ImprovedRam
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeImprovedRam;
			}
		}

		public static SiegeEngineType SiegeTower
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeSiegeTower;
			}
		}

		public static SiegeEngineType HeavySiegeTower
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeHeavySiegeTower;
			}
		}

		public static SiegeEngineType Catapult
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeCatapult;
			}
		}

		public static SiegeEngineType FireCatapult
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeFireCatapult;
			}
		}

		public static SiegeEngineType Onager
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeOnager;
			}
		}

		public static SiegeEngineType FireOnager
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeFireOnager;
			}
		}

		public static SiegeEngineType Bricole
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeBricole;
			}
		}

		public static SiegeEngineType Trebuchet
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeTrebuchet;
			}
		}

		public static SiegeEngineType FireTrebuchet
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeTrebuchet;
			}
		}

		public DefaultSiegeEngineTypes()
		{
			this.RegisterAll();
		}

		private void RegisterAll()
		{
			Game.Current.ObjectManager.LoadXML("SiegeEngines", false);
			this._siegeEngineTypePreparations = this.GetSiegeEngine("preparations");
			this._siegeEngineTypeLadder = this.GetSiegeEngine("ladder");
			this._siegeEngineTypeSiegeTower = this.GetSiegeEngine("siege_tower_level1");
			this._siegeEngineTypeHeavySiegeTower = this.GetSiegeEngine("siege_tower_level2");
			this._siegeEngineTypeBallista = this.GetSiegeEngine("ballista");
			this._siegeEngineTypeFireBallista = this.GetSiegeEngine("fire_ballista");
			this._siegeEngineTypeCatapult = this.GetSiegeEngine("catapult");
			this._siegeEngineTypeFireCatapult = this.GetSiegeEngine("fire_catapult");
			this._siegeEngineTypeOnager = this.GetSiegeEngine("onager");
			this._siegeEngineTypeFireOnager = this.GetSiegeEngine("fire_onager");
			this._siegeEngineTypeBricole = this.GetSiegeEngine("bricole");
			this._siegeEngineTypeTrebuchet = this.GetSiegeEngine("trebuchet");
			this._siegeEngineTypeFireTrebuchet = this.GetSiegeEngine("fire_trebuchet");
			this._siegeEngineTypeRam = this.GetSiegeEngine("ram");
			this._siegeEngineTypeImprovedRam = this.GetSiegeEngine("improved_ram");
		}

		private SiegeEngineType GetSiegeEngine(string id)
		{
			return Game.Current.ObjectManager.GetObject<SiegeEngineType>(id);
		}

		private SiegeEngineType _siegeEngineTypePreparations;

		private SiegeEngineType _siegeEngineTypeLadder;

		private SiegeEngineType _siegeEngineTypeBallista;

		private SiegeEngineType _siegeEngineTypeFireBallista;

		private SiegeEngineType _siegeEngineTypeRam;

		private SiegeEngineType _siegeEngineTypeImprovedRam;

		private SiegeEngineType _siegeEngineTypeSiegeTower;

		private SiegeEngineType _siegeEngineTypeHeavySiegeTower;

		private SiegeEngineType _siegeEngineTypeCatapult;

		private SiegeEngineType _siegeEngineTypeFireCatapult;

		private SiegeEngineType _siegeEngineTypeOnager;

		private SiegeEngineType _siegeEngineTypeFireOnager;

		private SiegeEngineType _siegeEngineTypeBricole;

		private SiegeEngineType _siegeEngineTypeTrebuchet;

		private SiegeEngineType _siegeEngineTypeFireTrebuchet;
	}
}
