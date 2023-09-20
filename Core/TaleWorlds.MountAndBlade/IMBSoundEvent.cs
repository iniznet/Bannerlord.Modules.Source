using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200019C RID: 412
	[ScriptingInterfaceBase]
	internal interface IMBSoundEvent
	{
		// Token: 0x060016DE RID: 5854
		[EngineMethod("create_event_from_external_file", false)]
		int CreateEventFromExternalFile(string programmerSoundEventName, string filePath, UIntPtr scene);

		// Token: 0x060016DF RID: 5855
		[EngineMethod("create_event_from_sound_buffer", false)]
		int CreateEventFromSoundBuffer(string programmerSoundEventName, byte[] soundBuffer, UIntPtr scene);

		// Token: 0x060016E0 RID: 5856
		[EngineMethod("play_sound", false)]
		bool PlaySound(int fmodEventIndex, ref Vec3 position);

		// Token: 0x060016E1 RID: 5857
		[EngineMethod("play_sound_with_int_param", false)]
		bool PlaySoundWithIntParam(int fmodEventIndex, int paramIndex, float paramVal, ref Vec3 position);

		// Token: 0x060016E2 RID: 5858
		[EngineMethod("play_sound_with_str_param", false)]
		bool PlaySoundWithStrParam(int fmodEventIndex, string paramName, float paramVal, ref Vec3 position);

		// Token: 0x060016E3 RID: 5859
		[EngineMethod("play_sound_with_param", false)]
		bool PlaySoundWithParam(int soundCodeId, SoundEventParameter parameter, ref Vec3 position);
	}
}
