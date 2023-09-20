using System;

namespace TaleWorlds.Core
{
	// Token: 0x020000AF RID: 175
	public class MissionSiegeWeapon : IMissionSiegeWeapon
	{
		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06000871 RID: 2161 RVA: 0x0001CC55 File Offset: 0x0001AE55
		public int Index
		{
			get
			{
				return this._index;
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06000872 RID: 2162 RVA: 0x0001CC5D File Offset: 0x0001AE5D
		public SiegeEngineType Type
		{
			get
			{
				return this._type;
			}
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06000873 RID: 2163 RVA: 0x0001CC65 File Offset: 0x0001AE65
		public float Health
		{
			get
			{
				return this._health;
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06000874 RID: 2164 RVA: 0x0001CC6D File Offset: 0x0001AE6D
		public float InitialHealth
		{
			get
			{
				return this._initialHealth;
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06000875 RID: 2165 RVA: 0x0001CC75 File Offset: 0x0001AE75
		public float MaxHealth
		{
			get
			{
				return this._maxHealth;
			}
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x0001CC7D File Offset: 0x0001AE7D
		private MissionSiegeWeapon(int index, SiegeEngineType type, float health, float maxHealth)
		{
			this._index = index;
			this._type = type;
			this._initialHealth = health;
			this._health = this._initialHealth;
			this._maxHealth = maxHealth;
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x0001CCAE File Offset: 0x0001AEAE
		public static MissionSiegeWeapon CreateDefaultWeapon(SiegeEngineType type)
		{
			return new MissionSiegeWeapon(-1, type, (float)type.BaseHitPoints, (float)type.BaseHitPoints);
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x0001CCC5 File Offset: 0x0001AEC5
		public static MissionSiegeWeapon CreateCampaignWeapon(SiegeEngineType type, int index, float health, float maxHealth)
		{
			return new MissionSiegeWeapon(index, type, health, maxHealth);
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x0001CCD0 File Offset: 0x0001AED0
		public void SetHealth(float health)
		{
			this._health = health;
		}

		// Token: 0x040004D3 RID: 1235
		private float _health;

		// Token: 0x040004D4 RID: 1236
		private readonly int _index;

		// Token: 0x040004D5 RID: 1237
		private readonly SiegeEngineType _type;

		// Token: 0x040004D6 RID: 1238
		private readonly float _initialHealth;

		// Token: 0x040004D7 RID: 1239
		private readonly float _maxHealth;
	}
}
