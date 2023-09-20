using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public static class MiscSoundContainer
	{
		public static int SoundCodeMovementFoleyDoorOpen { get; private set; }

		public static int SoundCodeMovementFoleyDoorClose { get; private set; }

		public static int SoundCodeAmbientNodeSiegeBallistaFire { get; private set; }

		public static int SoundCodeAmbientNodeSiegeMangonelFire { get; private set; }

		public static int SoundCodeAmbientNodeSiegeTrebuchetFire { get; private set; }

		public static int SoundCodeAmbientNodeSiegeBallistaHit { get; private set; }

		public static int SoundCodeAmbientNodeSiegeBoulderHit { get; private set; }

		static MiscSoundContainer()
		{
			MiscSoundContainer.UpdateMiscSoundCodes();
		}

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
