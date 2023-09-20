using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public static class MBSoundEvent
	{
		public static bool PlaySound(int soundCodeId, ref Vec3 position)
		{
			return MBAPI.IMBSoundEvent.PlaySound(soundCodeId, ref position);
		}

		public static bool PlaySound(int soundCodeId, Vec3 position)
		{
			Vec3 vec = position;
			return MBAPI.IMBSoundEvent.PlaySound(soundCodeId, ref vec);
		}

		public static bool PlaySound(int soundCodeId, ref SoundEventParameter parameter, Vec3 position)
		{
			Vec3 vec = position;
			return MBSoundEvent.PlaySound(soundCodeId, ref parameter, ref vec);
		}

		public static bool PlaySound(string soundPath, ref SoundEventParameter parameter, Vec3 position)
		{
			int eventIdFromString = SoundEvent.GetEventIdFromString(soundPath);
			Vec3 vec = position;
			return MBSoundEvent.PlaySound(eventIdFromString, ref parameter, ref vec);
		}

		public static bool PlaySound(int soundCodeId, ref SoundEventParameter parameter, ref Vec3 position)
		{
			return MBAPI.IMBSoundEvent.PlaySoundWithParam(soundCodeId, parameter, ref position);
		}

		public static void PlayEventFromSoundBuffer(string eventId, byte[] soundData, Scene scene)
		{
			MBAPI.IMBSoundEvent.CreateEventFromSoundBuffer(eventId, soundData, (scene != null) ? scene.Pointer : UIntPtr.Zero);
		}

		public static void CreateEventFromExternalFile(string programmerEventName, string soundFilePath, Scene scene)
		{
			MBAPI.IMBSoundEvent.CreateEventFromExternalFile(programmerEventName, soundFilePath, (scene != null) ? scene.Pointer : UIntPtr.Zero);
		}
	}
}
