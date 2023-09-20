using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001D8 RID: 472
	public static class CombatSoundContainer
	{
		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06001A74 RID: 6772 RVA: 0x0005D980 File Offset: 0x0005BB80
		// (set) Token: 0x06001A75 RID: 6773 RVA: 0x0005D987 File Offset: 0x0005BB87
		public static int SoundCodeMissionCombatBluntHigh { get; private set; }

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x06001A76 RID: 6774 RVA: 0x0005D98F File Offset: 0x0005BB8F
		// (set) Token: 0x06001A77 RID: 6775 RVA: 0x0005D996 File Offset: 0x0005BB96
		public static int SoundCodeMissionCombatBluntLow { get; private set; }

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06001A78 RID: 6776 RVA: 0x0005D99E File Offset: 0x0005BB9E
		// (set) Token: 0x06001A79 RID: 6777 RVA: 0x0005D9A5 File Offset: 0x0005BBA5
		public static int SoundCodeMissionCombatBluntMed { get; private set; }

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06001A7A RID: 6778 RVA: 0x0005D9AD File Offset: 0x0005BBAD
		// (set) Token: 0x06001A7B RID: 6779 RVA: 0x0005D9B4 File Offset: 0x0005BBB4
		public static int SoundCodeMissionCombatBoulderHigh { get; private set; }

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06001A7C RID: 6780 RVA: 0x0005D9BC File Offset: 0x0005BBBC
		// (set) Token: 0x06001A7D RID: 6781 RVA: 0x0005D9C3 File Offset: 0x0005BBC3
		public static int SoundCodeMissionCombatBoulderLow { get; private set; }

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06001A7E RID: 6782 RVA: 0x0005D9CB File Offset: 0x0005BBCB
		// (set) Token: 0x06001A7F RID: 6783 RVA: 0x0005D9D2 File Offset: 0x0005BBD2
		public static int SoundCodeMissionCombatBoulderMed { get; private set; }

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06001A80 RID: 6784 RVA: 0x0005D9DA File Offset: 0x0005BBDA
		// (set) Token: 0x06001A81 RID: 6785 RVA: 0x0005D9E1 File Offset: 0x0005BBE1
		public static int SoundCodeMissionCombatCutHigh { get; private set; }

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06001A82 RID: 6786 RVA: 0x0005D9E9 File Offset: 0x0005BBE9
		// (set) Token: 0x06001A83 RID: 6787 RVA: 0x0005D9F0 File Offset: 0x0005BBF0
		public static int SoundCodeMissionCombatCutLow { get; private set; }

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06001A84 RID: 6788 RVA: 0x0005D9F8 File Offset: 0x0005BBF8
		// (set) Token: 0x06001A85 RID: 6789 RVA: 0x0005D9FF File Offset: 0x0005BBFF
		public static int SoundCodeMissionCombatCutMed { get; private set; }

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06001A86 RID: 6790 RVA: 0x0005DA07 File Offset: 0x0005BC07
		// (set) Token: 0x06001A87 RID: 6791 RVA: 0x0005DA0E File Offset: 0x0005BC0E
		public static int SoundCodeMissionCombatMissileHigh { get; private set; }

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06001A88 RID: 6792 RVA: 0x0005DA16 File Offset: 0x0005BC16
		// (set) Token: 0x06001A89 RID: 6793 RVA: 0x0005DA1D File Offset: 0x0005BC1D
		public static int SoundCodeMissionCombatMissileLow { get; private set; }

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x06001A8A RID: 6794 RVA: 0x0005DA25 File Offset: 0x0005BC25
		// (set) Token: 0x06001A8B RID: 6795 RVA: 0x0005DA2C File Offset: 0x0005BC2C
		public static int SoundCodeMissionCombatMissileMed { get; private set; }

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x06001A8C RID: 6796 RVA: 0x0005DA34 File Offset: 0x0005BC34
		// (set) Token: 0x06001A8D RID: 6797 RVA: 0x0005DA3B File Offset: 0x0005BC3B
		public static int SoundCodeMissionCombatPierceHigh { get; private set; }

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06001A8E RID: 6798 RVA: 0x0005DA43 File Offset: 0x0005BC43
		// (set) Token: 0x06001A8F RID: 6799 RVA: 0x0005DA4A File Offset: 0x0005BC4A
		public static int SoundCodeMissionCombatPierceLow { get; private set; }

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06001A90 RID: 6800 RVA: 0x0005DA52 File Offset: 0x0005BC52
		// (set) Token: 0x06001A91 RID: 6801 RVA: 0x0005DA59 File Offset: 0x0005BC59
		public static int SoundCodeMissionCombatPierceMed { get; private set; }

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06001A92 RID: 6802 RVA: 0x0005DA61 File Offset: 0x0005BC61
		// (set) Token: 0x06001A93 RID: 6803 RVA: 0x0005DA68 File Offset: 0x0005BC68
		public static int SoundCodeMissionCombatPunchHigh { get; private set; }

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06001A94 RID: 6804 RVA: 0x0005DA70 File Offset: 0x0005BC70
		// (set) Token: 0x06001A95 RID: 6805 RVA: 0x0005DA77 File Offset: 0x0005BC77
		public static int SoundCodeMissionCombatPunchLow { get; private set; }

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06001A96 RID: 6806 RVA: 0x0005DA7F File Offset: 0x0005BC7F
		// (set) Token: 0x06001A97 RID: 6807 RVA: 0x0005DA86 File Offset: 0x0005BC86
		public static int SoundCodeMissionCombatPunchMed { get; private set; }

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06001A98 RID: 6808 RVA: 0x0005DA8E File Offset: 0x0005BC8E
		// (set) Token: 0x06001A99 RID: 6809 RVA: 0x0005DA95 File Offset: 0x0005BC95
		public static int SoundCodeMissionCombatThrowingAxeHigh { get; private set; }

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x06001A9A RID: 6810 RVA: 0x0005DA9D File Offset: 0x0005BC9D
		// (set) Token: 0x06001A9B RID: 6811 RVA: 0x0005DAA4 File Offset: 0x0005BCA4
		public static int SoundCodeMissionCombatThrowingAxeLow { get; private set; }

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x06001A9C RID: 6812 RVA: 0x0005DAAC File Offset: 0x0005BCAC
		// (set) Token: 0x06001A9D RID: 6813 RVA: 0x0005DAB3 File Offset: 0x0005BCB3
		public static int SoundCodeMissionCombatThrowingAxeMed { get; private set; }

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x06001A9E RID: 6814 RVA: 0x0005DABB File Offset: 0x0005BCBB
		// (set) Token: 0x06001A9F RID: 6815 RVA: 0x0005DAC2 File Offset: 0x0005BCC2
		public static int SoundCodeMissionCombatThrowingDaggerHigh { get; private set; }

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x06001AA0 RID: 6816 RVA: 0x0005DACA File Offset: 0x0005BCCA
		// (set) Token: 0x06001AA1 RID: 6817 RVA: 0x0005DAD1 File Offset: 0x0005BCD1
		public static int SoundCodeMissionCombatThrowingDaggerLow { get; private set; }

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x06001AA2 RID: 6818 RVA: 0x0005DAD9 File Offset: 0x0005BCD9
		// (set) Token: 0x06001AA3 RID: 6819 RVA: 0x0005DAE0 File Offset: 0x0005BCE0
		public static int SoundCodeMissionCombatThrowingDaggerMed { get; private set; }

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06001AA4 RID: 6820 RVA: 0x0005DAE8 File Offset: 0x0005BCE8
		// (set) Token: 0x06001AA5 RID: 6821 RVA: 0x0005DAEF File Offset: 0x0005BCEF
		public static int SoundCodeMissionCombatThrowingStoneHigh { get; private set; }

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x06001AA6 RID: 6822 RVA: 0x0005DAF7 File Offset: 0x0005BCF7
		// (set) Token: 0x06001AA7 RID: 6823 RVA: 0x0005DAFE File Offset: 0x0005BCFE
		public static int SoundCodeMissionCombatThrowingStoneLow { get; private set; }

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x06001AA8 RID: 6824 RVA: 0x0005DB06 File Offset: 0x0005BD06
		// (set) Token: 0x06001AA9 RID: 6825 RVA: 0x0005DB0D File Offset: 0x0005BD0D
		public static int SoundCodeMissionCombatThrowingStoneMed { get; private set; }

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x06001AAA RID: 6826 RVA: 0x0005DB15 File Offset: 0x0005BD15
		// (set) Token: 0x06001AAB RID: 6827 RVA: 0x0005DB1C File Offset: 0x0005BD1C
		public static int SoundCodeMissionCombatChargeDamage { get; private set; }

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x06001AAC RID: 6828 RVA: 0x0005DB24 File Offset: 0x0005BD24
		// (set) Token: 0x06001AAD RID: 6829 RVA: 0x0005DB2B File Offset: 0x0005BD2B
		public static int SoundCodeMissionCombatKick { get; private set; }

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x06001AAE RID: 6830 RVA: 0x0005DB33 File Offset: 0x0005BD33
		// (set) Token: 0x06001AAF RID: 6831 RVA: 0x0005DB3A File Offset: 0x0005BD3A
		public static int SoundCodeMissionCombatPlayerhit { get; private set; }

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x06001AB0 RID: 6832 RVA: 0x0005DB42 File Offset: 0x0005BD42
		// (set) Token: 0x06001AB1 RID: 6833 RVA: 0x0005DB49 File Offset: 0x0005BD49
		public static int SoundCodeMissionCombatWoodShieldBash { get; private set; }

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x06001AB2 RID: 6834 RVA: 0x0005DB51 File Offset: 0x0005BD51
		// (set) Token: 0x06001AB3 RID: 6835 RVA: 0x0005DB58 File Offset: 0x0005BD58
		public static int SoundCodeMissionCombatMetalShieldBash { get; private set; }

		// Token: 0x06001AB4 RID: 6836 RVA: 0x0005DB60 File Offset: 0x0005BD60
		static CombatSoundContainer()
		{
			CombatSoundContainer.UpdateMissionCombatSoundCodes();
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x0005DB68 File Offset: 0x0005BD68
		private static void UpdateMissionCombatSoundCodes()
		{
			CombatSoundContainer.SoundCodeMissionCombatBluntHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/blunt/high");
			CombatSoundContainer.SoundCodeMissionCombatBluntLow = SoundEvent.GetEventIdFromString("event:/mission/combat/blunt/low");
			CombatSoundContainer.SoundCodeMissionCombatBluntMed = SoundEvent.GetEventIdFromString("event:/mission/combat/blunt/med");
			CombatSoundContainer.SoundCodeMissionCombatBoulderHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/boulder/high");
			CombatSoundContainer.SoundCodeMissionCombatBoulderLow = SoundEvent.GetEventIdFromString("event:/mission/combat/boulder/low");
			CombatSoundContainer.SoundCodeMissionCombatBoulderMed = SoundEvent.GetEventIdFromString("event:/mission/combat/boulder/med");
			CombatSoundContainer.SoundCodeMissionCombatCutHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/cut/high");
			CombatSoundContainer.SoundCodeMissionCombatCutLow = SoundEvent.GetEventIdFromString("event:/mission/combat/cut/low");
			CombatSoundContainer.SoundCodeMissionCombatCutMed = SoundEvent.GetEventIdFromString("event:/mission/combat/cut/med");
			CombatSoundContainer.SoundCodeMissionCombatMissileHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/missile/high");
			CombatSoundContainer.SoundCodeMissionCombatMissileLow = SoundEvent.GetEventIdFromString("event:/mission/combat/missile/low");
			CombatSoundContainer.SoundCodeMissionCombatMissileMed = SoundEvent.GetEventIdFromString("event:/mission/combat/missile/med");
			CombatSoundContainer.SoundCodeMissionCombatPierceHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/pierce/high");
			CombatSoundContainer.SoundCodeMissionCombatPierceLow = SoundEvent.GetEventIdFromString("event:/mission/combat/pierce/low");
			CombatSoundContainer.SoundCodeMissionCombatPierceMed = SoundEvent.GetEventIdFromString("event:/mission/combat/pierce/med");
			CombatSoundContainer.SoundCodeMissionCombatPunchHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/punch/high");
			CombatSoundContainer.SoundCodeMissionCombatPunchLow = SoundEvent.GetEventIdFromString("event:/mission/combat/punch/low");
			CombatSoundContainer.SoundCodeMissionCombatPunchMed = SoundEvent.GetEventIdFromString("event:/mission/combat/punch/med");
			CombatSoundContainer.SoundCodeMissionCombatThrowingAxeHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/throwing/high");
			CombatSoundContainer.SoundCodeMissionCombatThrowingAxeLow = SoundEvent.GetEventIdFromString("event:/mission/combat/throwing/low");
			CombatSoundContainer.SoundCodeMissionCombatThrowingAxeMed = SoundEvent.GetEventIdFromString("event:/mission/combat/throwing/med");
			CombatSoundContainer.SoundCodeMissionCombatThrowingDaggerHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/throwing/high");
			CombatSoundContainer.SoundCodeMissionCombatThrowingDaggerLow = SoundEvent.GetEventIdFromString("event:/mission/combat/throwing/low");
			CombatSoundContainer.SoundCodeMissionCombatThrowingDaggerMed = SoundEvent.GetEventIdFromString("event:/mission/combat/throwing/med");
			CombatSoundContainer.SoundCodeMissionCombatThrowingStoneHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/throwingstone/high");
			CombatSoundContainer.SoundCodeMissionCombatThrowingStoneLow = SoundEvent.GetEventIdFromString("event:/mission/combat/throwingstone/low");
			CombatSoundContainer.SoundCodeMissionCombatThrowingStoneMed = SoundEvent.GetEventIdFromString("event:/mission/combat/throwingstone/med");
			CombatSoundContainer.SoundCodeMissionCombatChargeDamage = SoundEvent.GetEventIdFromString("event:/mission/combat/charge/damage");
			CombatSoundContainer.SoundCodeMissionCombatKick = SoundEvent.GetEventIdFromString("event:/mission/combat/kick");
			CombatSoundContainer.SoundCodeMissionCombatPlayerhit = SoundEvent.GetEventIdFromString("event:/mission/combat/playerHit");
			CombatSoundContainer.SoundCodeMissionCombatWoodShieldBash = SoundEvent.GetEventIdFromString("event:/mission/combat/shield/bash");
			CombatSoundContainer.SoundCodeMissionCombatMetalShieldBash = SoundEvent.GetEventIdFromString("event:/mission/combat/shield/metal_bash");
		}
	}
}
