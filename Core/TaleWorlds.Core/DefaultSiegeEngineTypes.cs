using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000050 RID: 80
	public class DefaultSiegeEngineTypes
	{
		// Token: 0x17000218 RID: 536
		// (get) Token: 0x060005E6 RID: 1510 RVA: 0x00015D84 File Offset: 0x00013F84
		private static DefaultSiegeEngineTypes Instance
		{
			get
			{
				return Game.Current.DefaultSiegeEngineTypes;
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x060005E7 RID: 1511 RVA: 0x00015D90 File Offset: 0x00013F90
		public static SiegeEngineType Preparations
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypePreparations;
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x060005E8 RID: 1512 RVA: 0x00015D9C File Offset: 0x00013F9C
		public static SiegeEngineType Ladder
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeLadder;
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x060005E9 RID: 1513 RVA: 0x00015DA8 File Offset: 0x00013FA8
		public static SiegeEngineType Ballista
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeBallista;
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x060005EA RID: 1514 RVA: 0x00015DB4 File Offset: 0x00013FB4
		public static SiegeEngineType FireBallista
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeFireBallista;
			}
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x060005EB RID: 1515 RVA: 0x00015DC0 File Offset: 0x00013FC0
		public static SiegeEngineType Ram
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeRam;
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x060005EC RID: 1516 RVA: 0x00015DCC File Offset: 0x00013FCC
		public static SiegeEngineType ImprovedRam
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeImprovedRam;
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x060005ED RID: 1517 RVA: 0x00015DD8 File Offset: 0x00013FD8
		public static SiegeEngineType SiegeTower
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeSiegeTower;
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x060005EE RID: 1518 RVA: 0x00015DE4 File Offset: 0x00013FE4
		public static SiegeEngineType HeavySiegeTower
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeHeavySiegeTower;
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x060005EF RID: 1519 RVA: 0x00015DF0 File Offset: 0x00013FF0
		public static SiegeEngineType Catapult
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeCatapult;
			}
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x060005F0 RID: 1520 RVA: 0x00015DFC File Offset: 0x00013FFC
		public static SiegeEngineType FireCatapult
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeFireCatapult;
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x060005F1 RID: 1521 RVA: 0x00015E08 File Offset: 0x00014008
		public static SiegeEngineType Onager
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeOnager;
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x060005F2 RID: 1522 RVA: 0x00015E14 File Offset: 0x00014014
		public static SiegeEngineType FireOnager
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeFireOnager;
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x060005F3 RID: 1523 RVA: 0x00015E20 File Offset: 0x00014020
		public static SiegeEngineType Bricole
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeBricole;
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x060005F4 RID: 1524 RVA: 0x00015E2C File Offset: 0x0001402C
		public static SiegeEngineType Trebuchet
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeTrebuchet;
			}
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x060005F5 RID: 1525 RVA: 0x00015E38 File Offset: 0x00014038
		public static SiegeEngineType FireTrebuchet
		{
			get
			{
				return DefaultSiegeEngineTypes.Instance._siegeEngineTypeTrebuchet;
			}
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x00015E44 File Offset: 0x00014044
		public DefaultSiegeEngineTypes()
		{
			this.RegisterAll();
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x00015E54 File Offset: 0x00014054
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

		// Token: 0x060005F8 RID: 1528 RVA: 0x00015F75 File Offset: 0x00014175
		private SiegeEngineType GetSiegeEngine(string id)
		{
			return Game.Current.ObjectManager.GetObject<SiegeEngineType>(id);
		}

		// Token: 0x040002F2 RID: 754
		private SiegeEngineType _siegeEngineTypePreparations;

		// Token: 0x040002F3 RID: 755
		private SiegeEngineType _siegeEngineTypeLadder;

		// Token: 0x040002F4 RID: 756
		private SiegeEngineType _siegeEngineTypeBallista;

		// Token: 0x040002F5 RID: 757
		private SiegeEngineType _siegeEngineTypeFireBallista;

		// Token: 0x040002F6 RID: 758
		private SiegeEngineType _siegeEngineTypeRam;

		// Token: 0x040002F7 RID: 759
		private SiegeEngineType _siegeEngineTypeImprovedRam;

		// Token: 0x040002F8 RID: 760
		private SiegeEngineType _siegeEngineTypeSiegeTower;

		// Token: 0x040002F9 RID: 761
		private SiegeEngineType _siegeEngineTypeHeavySiegeTower;

		// Token: 0x040002FA RID: 762
		private SiegeEngineType _siegeEngineTypeCatapult;

		// Token: 0x040002FB RID: 763
		private SiegeEngineType _siegeEngineTypeFireCatapult;

		// Token: 0x040002FC RID: 764
		private SiegeEngineType _siegeEngineTypeOnager;

		// Token: 0x040002FD RID: 765
		private SiegeEngineType _siegeEngineTypeFireOnager;

		// Token: 0x040002FE RID: 766
		private SiegeEngineType _siegeEngineTypeBricole;

		// Token: 0x040002FF RID: 767
		private SiegeEngineType _siegeEngineTypeTrebuchet;

		// Token: 0x04000300 RID: 768
		private SiegeEngineType _siegeEngineTypeFireTrebuchet;
	}
}
