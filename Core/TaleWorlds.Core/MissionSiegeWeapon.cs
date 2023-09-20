using System;

namespace TaleWorlds.Core
{
	public class MissionSiegeWeapon : IMissionSiegeWeapon
	{
		public int Index
		{
			get
			{
				return this._index;
			}
		}

		public SiegeEngineType Type
		{
			get
			{
				return this._type;
			}
		}

		public float Health
		{
			get
			{
				return this._health;
			}
		}

		public float InitialHealth
		{
			get
			{
				return this._initialHealth;
			}
		}

		public float MaxHealth
		{
			get
			{
				return this._maxHealth;
			}
		}

		private MissionSiegeWeapon(int index, SiegeEngineType type, float health, float maxHealth)
		{
			this._index = index;
			this._type = type;
			this._initialHealth = health;
			this._health = this._initialHealth;
			this._maxHealth = maxHealth;
		}

		public static MissionSiegeWeapon CreateDefaultWeapon(SiegeEngineType type)
		{
			return new MissionSiegeWeapon(-1, type, (float)type.BaseHitPoints, (float)type.BaseHitPoints);
		}

		public static MissionSiegeWeapon CreateCampaignWeapon(SiegeEngineType type, int index, float health, float maxHealth)
		{
			return new MissionSiegeWeapon(index, type, health, maxHealth);
		}

		public void SetHealth(float health)
		{
			this._health = health;
		}

		private float _health;

		private readonly int _index;

		private readonly SiegeEngineType _type;

		private readonly float _initialHealth;

		private readonly float _maxHealth;
	}
}
