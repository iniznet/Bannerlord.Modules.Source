using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001C7 RID: 455
	public static class MBSoundEvent
	{
		// Token: 0x060019F1 RID: 6641 RVA: 0x0005C31F File Offset: 0x0005A51F
		public static bool PlaySound(int soundCodeId, ref Vec3 position)
		{
			return MBAPI.IMBSoundEvent.PlaySound(soundCodeId, ref position);
		}

		// Token: 0x060019F2 RID: 6642 RVA: 0x0005C330 File Offset: 0x0005A530
		public static bool PlaySound(int soundCodeId, Vec3 position)
		{
			Vec3 vec = position;
			return MBAPI.IMBSoundEvent.PlaySound(soundCodeId, ref vec);
		}

		// Token: 0x060019F3 RID: 6643 RVA: 0x0005C34C File Offset: 0x0005A54C
		public static bool PlaySound(int soundCodeId, ref SoundEventParameter parameter, Vec3 position)
		{
			Vec3 vec = position;
			return MBSoundEvent.PlaySound(soundCodeId, ref parameter, ref vec);
		}

		// Token: 0x060019F4 RID: 6644 RVA: 0x0005C364 File Offset: 0x0005A564
		public static bool PlaySound(string soundPath, ref SoundEventParameter parameter, Vec3 position)
		{
			int eventIdFromString = SoundEvent.GetEventIdFromString(soundPath);
			Vec3 vec = position;
			return MBSoundEvent.PlaySound(eventIdFromString, ref parameter, ref vec);
		}

		// Token: 0x060019F5 RID: 6645 RVA: 0x0005C381 File Offset: 0x0005A581
		public static bool PlaySound(int soundCodeId, ref SoundEventParameter parameter, ref Vec3 position)
		{
			return MBAPI.IMBSoundEvent.PlaySoundWithParam(soundCodeId, parameter, ref position);
		}

		// Token: 0x060019F6 RID: 6646 RVA: 0x0005C395 File Offset: 0x0005A595
		public static void PlayEventFromSoundBuffer(string eventId, byte[] soundData, Scene scene)
		{
			MBAPI.IMBSoundEvent.CreateEventFromSoundBuffer(eventId, soundData, (scene != null) ? scene.Pointer : UIntPtr.Zero);
		}

		// Token: 0x060019F7 RID: 6647 RVA: 0x0005C3BA File Offset: 0x0005A5BA
		public static void CreateEventFromExternalFile(string programmerEventName, string soundFilePath, Scene scene)
		{
			MBAPI.IMBSoundEvent.CreateEventFromExternalFile(programmerEventName, soundFilePath, (scene != null) ? scene.Pointer : UIntPtr.Zero);
		}
	}
}
