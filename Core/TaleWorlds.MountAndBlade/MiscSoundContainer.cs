using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001DA RID: 474
	public static class MiscSoundContainer
	{
		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x06001AEA RID: 6890 RVA: 0x0005E058 File Offset: 0x0005C258
		// (set) Token: 0x06001AEB RID: 6891 RVA: 0x0005E05F File Offset: 0x0005C25F
		public static int SoundCodeMovementFoleyDoorOpen { get; private set; }

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06001AEC RID: 6892 RVA: 0x0005E067 File Offset: 0x0005C267
		// (set) Token: 0x06001AED RID: 6893 RVA: 0x0005E06E File Offset: 0x0005C26E
		public static int SoundCodeMovementFoleyDoorClose { get; private set; }

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06001AEE RID: 6894 RVA: 0x0005E076 File Offset: 0x0005C276
		// (set) Token: 0x06001AEF RID: 6895 RVA: 0x0005E07D File Offset: 0x0005C27D
		public static int SoundCodeAmbientNodeSiegeBallistaFire { get; private set; }

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x06001AF0 RID: 6896 RVA: 0x0005E085 File Offset: 0x0005C285
		// (set) Token: 0x06001AF1 RID: 6897 RVA: 0x0005E08C File Offset: 0x0005C28C
		public static int SoundCodeAmbientNodeSiegeMangonelFire { get; private set; }

		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x06001AF2 RID: 6898 RVA: 0x0005E094 File Offset: 0x0005C294
		// (set) Token: 0x06001AF3 RID: 6899 RVA: 0x0005E09B File Offset: 0x0005C29B
		public static int SoundCodeAmbientNodeSiegeTrebuchetFire { get; private set; }

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x06001AF4 RID: 6900 RVA: 0x0005E0A3 File Offset: 0x0005C2A3
		// (set) Token: 0x06001AF5 RID: 6901 RVA: 0x0005E0AA File Offset: 0x0005C2AA
		public static int SoundCodeAmbientNodeSiegeBallistaHit { get; private set; }

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x06001AF6 RID: 6902 RVA: 0x0005E0B2 File Offset: 0x0005C2B2
		// (set) Token: 0x06001AF7 RID: 6903 RVA: 0x0005E0B9 File Offset: 0x0005C2B9
		public static int SoundCodeAmbientNodeSiegeBoulderHit { get; private set; }

		// Token: 0x06001AF8 RID: 6904 RVA: 0x0005E0C1 File Offset: 0x0005C2C1
		static MiscSoundContainer()
		{
			MiscSoundContainer.UpdateMiscSoundCodes();
		}

		// Token: 0x06001AF9 RID: 6905 RVA: 0x0005E0C8 File Offset: 0x0005C2C8
		private static void UpdateMiscSoundCodes()
		{
			MiscSoundContainer.SoundCodeMovementFoleyDoorOpen = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/door_open");
			MiscSoundContainer.SoundCodeMovementFoleyDoorClose = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/door_close");
			MiscSoundContainer.SoundCodeAmbientNodeSiegeBallistaFire = SoundEvent.GetEventIdFromString("event:/map/ambient/node/siege/ballista_fire");
			MiscSoundContainer.SoundCodeAmbientNodeSiegeMangonelFire = SoundEvent.GetEventIdFromString("event:/map/ambient/node/siege/mangonel_fire");
			MiscSoundContainer.SoundCodeAmbientNodeSiegeTrebuchetFire = SoundEvent.GetEventIdFromString("event:/map/ambient/node/siege/trebuchet_fire");
			MiscSoundContainer.SoundCodeAmbientNodeSiegeBallistaHit = SoundEvent.GetEventIdFromString("event:/map/ambient/node/siege/ballista_hit");
			MiscSoundContainer.SoundCodeAmbientNodeSiegeBoulderHit = SoundEvent.GetEventIdFromString("event:/map/ambient/node/siege/boulder_hit");
		}
	}
}
