using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001D9 RID: 473
	public static class ItemPhysicsSoundContainer
	{
		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x06001AB6 RID: 6838 RVA: 0x0005DD55 File Offset: 0x0005BF55
		// (set) Token: 0x06001AB7 RID: 6839 RVA: 0x0005DD5C File Offset: 0x0005BF5C
		public static int SoundCodePhysicsBoulderDefault { get; private set; }

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x06001AB8 RID: 6840 RVA: 0x0005DD64 File Offset: 0x0005BF64
		// (set) Token: 0x06001AB9 RID: 6841 RVA: 0x0005DD6B File Offset: 0x0005BF6B
		public static int SoundCodePhysicsArrowlikeDefault { get; private set; }

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06001ABA RID: 6842 RVA: 0x0005DD73 File Offset: 0x0005BF73
		// (set) Token: 0x06001ABB RID: 6843 RVA: 0x0005DD7A File Offset: 0x0005BF7A
		public static int SoundCodePhysicsBowlikeDefault { get; private set; }

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x06001ABC RID: 6844 RVA: 0x0005DD82 File Offset: 0x0005BF82
		// (set) Token: 0x06001ABD RID: 6845 RVA: 0x0005DD89 File Offset: 0x0005BF89
		public static int SoundCodePhysicsDaggerlikeDefault { get; private set; }

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x06001ABE RID: 6846 RVA: 0x0005DD91 File Offset: 0x0005BF91
		// (set) Token: 0x06001ABF RID: 6847 RVA: 0x0005DD98 File Offset: 0x0005BF98
		public static int SoundCodePhysicsGreatswordlikeDefault { get; private set; }

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06001AC0 RID: 6848 RVA: 0x0005DDA0 File Offset: 0x0005BFA0
		// (set) Token: 0x06001AC1 RID: 6849 RVA: 0x0005DDA7 File Offset: 0x0005BFA7
		public static int SoundCodePhysicsShieldlikeDefault { get; private set; }

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x06001AC2 RID: 6850 RVA: 0x0005DDAF File Offset: 0x0005BFAF
		// (set) Token: 0x06001AC3 RID: 6851 RVA: 0x0005DDB6 File Offset: 0x0005BFB6
		public static int SoundCodePhysicsSpearlikeDefault { get; private set; }

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06001AC4 RID: 6852 RVA: 0x0005DDBE File Offset: 0x0005BFBE
		// (set) Token: 0x06001AC5 RID: 6853 RVA: 0x0005DDC5 File Offset: 0x0005BFC5
		public static int SoundCodePhysicsSwordlikeDefault { get; private set; }

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06001AC6 RID: 6854 RVA: 0x0005DDCD File Offset: 0x0005BFCD
		// (set) Token: 0x06001AC7 RID: 6855 RVA: 0x0005DDD4 File Offset: 0x0005BFD4
		public static int SoundCodePhysicsBoulderWood { get; private set; }

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06001AC8 RID: 6856 RVA: 0x0005DDDC File Offset: 0x0005BFDC
		// (set) Token: 0x06001AC9 RID: 6857 RVA: 0x0005DDE3 File Offset: 0x0005BFE3
		public static int SoundCodePhysicsArrowlikeWood { get; private set; }

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06001ACA RID: 6858 RVA: 0x0005DDEB File Offset: 0x0005BFEB
		// (set) Token: 0x06001ACB RID: 6859 RVA: 0x0005DDF2 File Offset: 0x0005BFF2
		public static int SoundCodePhysicsBowlikeWood { get; private set; }

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x06001ACC RID: 6860 RVA: 0x0005DDFA File Offset: 0x0005BFFA
		// (set) Token: 0x06001ACD RID: 6861 RVA: 0x0005DE01 File Offset: 0x0005C001
		public static int SoundCodePhysicsDaggerlikeWood { get; private set; }

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x06001ACE RID: 6862 RVA: 0x0005DE09 File Offset: 0x0005C009
		// (set) Token: 0x06001ACF RID: 6863 RVA: 0x0005DE10 File Offset: 0x0005C010
		public static int SoundCodePhysicsGreatswordlikeWood { get; private set; }

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x06001AD0 RID: 6864 RVA: 0x0005DE18 File Offset: 0x0005C018
		// (set) Token: 0x06001AD1 RID: 6865 RVA: 0x0005DE1F File Offset: 0x0005C01F
		public static int SoundCodePhysicsShieldlikeWood { get; private set; }

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x06001AD2 RID: 6866 RVA: 0x0005DE27 File Offset: 0x0005C027
		// (set) Token: 0x06001AD3 RID: 6867 RVA: 0x0005DE2E File Offset: 0x0005C02E
		public static int SoundCodePhysicsSpearlikeWood { get; private set; }

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x06001AD4 RID: 6868 RVA: 0x0005DE36 File Offset: 0x0005C036
		// (set) Token: 0x06001AD5 RID: 6869 RVA: 0x0005DE3D File Offset: 0x0005C03D
		public static int SoundCodePhysicsSwordlikeWood { get; private set; }

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06001AD6 RID: 6870 RVA: 0x0005DE45 File Offset: 0x0005C045
		// (set) Token: 0x06001AD7 RID: 6871 RVA: 0x0005DE4C File Offset: 0x0005C04C
		public static int SoundCodePhysicsBoulderStone { get; private set; }

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06001AD8 RID: 6872 RVA: 0x0005DE54 File Offset: 0x0005C054
		// (set) Token: 0x06001AD9 RID: 6873 RVA: 0x0005DE5B File Offset: 0x0005C05B
		public static int SoundCodePhysicsArrowlikeStone { get; private set; }

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06001ADA RID: 6874 RVA: 0x0005DE63 File Offset: 0x0005C063
		// (set) Token: 0x06001ADB RID: 6875 RVA: 0x0005DE6A File Offset: 0x0005C06A
		public static int SoundCodePhysicsBowlikeStone { get; private set; }

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x06001ADC RID: 6876 RVA: 0x0005DE72 File Offset: 0x0005C072
		// (set) Token: 0x06001ADD RID: 6877 RVA: 0x0005DE79 File Offset: 0x0005C079
		public static int SoundCodePhysicsDaggerlikeStone { get; private set; }

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x06001ADE RID: 6878 RVA: 0x0005DE81 File Offset: 0x0005C081
		// (set) Token: 0x06001ADF RID: 6879 RVA: 0x0005DE88 File Offset: 0x0005C088
		public static int SoundCodePhysicsGreatswordlikeStone { get; private set; }

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06001AE0 RID: 6880 RVA: 0x0005DE90 File Offset: 0x0005C090
		// (set) Token: 0x06001AE1 RID: 6881 RVA: 0x0005DE97 File Offset: 0x0005C097
		public static int SoundCodePhysicsShieldlikeStone { get; private set; }

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06001AE2 RID: 6882 RVA: 0x0005DE9F File Offset: 0x0005C09F
		// (set) Token: 0x06001AE3 RID: 6883 RVA: 0x0005DEA6 File Offset: 0x0005C0A6
		public static int SoundCodePhysicsSpearlikeStone { get; private set; }

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06001AE4 RID: 6884 RVA: 0x0005DEAE File Offset: 0x0005C0AE
		// (set) Token: 0x06001AE5 RID: 6885 RVA: 0x0005DEB5 File Offset: 0x0005C0B5
		public static int SoundCodePhysicsSwordlikeStone { get; private set; }

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06001AE6 RID: 6886 RVA: 0x0005DEBD File Offset: 0x0005C0BD
		// (set) Token: 0x06001AE7 RID: 6887 RVA: 0x0005DEC4 File Offset: 0x0005C0C4
		public static int SoundCodePhysicsWater { get; private set; }

		// Token: 0x06001AE8 RID: 6888 RVA: 0x0005DECC File Offset: 0x0005C0CC
		static ItemPhysicsSoundContainer()
		{
			ItemPhysicsSoundContainer.UpdateItemPhysicsSoundCodes();
		}

		// Token: 0x06001AE9 RID: 6889 RVA: 0x0005DED4 File Offset: 0x0005C0D4
		private static void UpdateItemPhysicsSoundCodes()
		{
			ItemPhysicsSoundContainer.SoundCodePhysicsBoulderDefault = SoundEvent.GetEventIdFromString("event:/physics/boulder/default");
			ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/arrowlike/default");
			ItemPhysicsSoundContainer.SoundCodePhysicsBowlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/bowlike/default");
			ItemPhysicsSoundContainer.SoundCodePhysicsDaggerlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/daggerlike/default");
			ItemPhysicsSoundContainer.SoundCodePhysicsGreatswordlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/greatswordlike/default");
			ItemPhysicsSoundContainer.SoundCodePhysicsShieldlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/shieldlike/default");
			ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/spearlike/default");
			ItemPhysicsSoundContainer.SoundCodePhysicsSwordlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/swordlike/default");
			ItemPhysicsSoundContainer.SoundCodePhysicsBoulderWood = SoundEvent.GetEventIdFromString("event:/physics/boulder/wood");
			ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeWood = SoundEvent.GetEventIdFromString("event:/physics/arrowlike/wood");
			ItemPhysicsSoundContainer.SoundCodePhysicsBowlikeWood = SoundEvent.GetEventIdFromString("event:/physics/bowlike/wood");
			ItemPhysicsSoundContainer.SoundCodePhysicsDaggerlikeWood = SoundEvent.GetEventIdFromString("event:/physics/daggerlike/wood");
			ItemPhysicsSoundContainer.SoundCodePhysicsGreatswordlikeWood = SoundEvent.GetEventIdFromString("event:/physics/greatswordlike/wood");
			ItemPhysicsSoundContainer.SoundCodePhysicsShieldlikeWood = SoundEvent.GetEventIdFromString("event:/physics/shieldlike/wood");
			ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeWood = SoundEvent.GetEventIdFromString("event:/physics/spearlike/wood");
			ItemPhysicsSoundContainer.SoundCodePhysicsSwordlikeWood = SoundEvent.GetEventIdFromString("event:/physics/swordlike/wood");
			ItemPhysicsSoundContainer.SoundCodePhysicsBoulderStone = SoundEvent.GetEventIdFromString("event:/physics/boulder/stone");
			ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeStone = SoundEvent.GetEventIdFromString("event:/physics/arrowlike/stone");
			ItemPhysicsSoundContainer.SoundCodePhysicsBowlikeStone = SoundEvent.GetEventIdFromString("event:/physics/bowlike/stone");
			ItemPhysicsSoundContainer.SoundCodePhysicsDaggerlikeStone = SoundEvent.GetEventIdFromString("event:/physics/daggerlike/stone");
			ItemPhysicsSoundContainer.SoundCodePhysicsGreatswordlikeStone = SoundEvent.GetEventIdFromString("event:/physics/greatswordlike/stone");
			ItemPhysicsSoundContainer.SoundCodePhysicsShieldlikeStone = SoundEvent.GetEventIdFromString("event:/physics/shieldlike/stone");
			ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeStone = SoundEvent.GetEventIdFromString("event:/physics/spearlike/stone");
			ItemPhysicsSoundContainer.SoundCodePhysicsSwordlikeStone = SoundEvent.GetEventIdFromString("event:/physics/swordlike/stone");
			ItemPhysicsSoundContainer.SoundCodePhysicsWater = SoundEvent.GetEventIdFromString("event:/physics/water");
		}
	}
}
