using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200003E RID: 62
	[ApplicationInterfaceBase]
	internal interface ISoundEvent
	{
		// Token: 0x06000574 RID: 1396
		[EngineMethod("create_event_from_string", false)]
		int CreateEventFromString(string eventName, UIntPtr scene);

		// Token: 0x06000575 RID: 1397
		[EngineMethod("get_event_id_from_string", false)]
		int GetEventIdFromString(string eventName);

		// Token: 0x06000576 RID: 1398
		[EngineMethod("play_sound_2d", false)]
		bool PlaySound2D(int fmodEventIndex);

		// Token: 0x06000577 RID: 1399
		[EngineMethod("get_total_event_count", false)]
		int GetTotalEventCount();

		// Token: 0x06000578 RID: 1400
		[EngineMethod("set_event_min_max_distance", false)]
		void SetEventMinMaxDistance(int fmodEventIndex, Vec3 radius);

		// Token: 0x06000579 RID: 1401
		[EngineMethod("create_event", false)]
		int CreateEvent(int fmodEventIndex, UIntPtr scene);

		// Token: 0x0600057A RID: 1402
		[EngineMethod("release_event", false)]
		void ReleaseEvent(int eventId);

		// Token: 0x0600057B RID: 1403
		[EngineMethod("set_event_parameter_from_string", false)]
		void SetEventParameterFromString(int eventId, string name, float value);

		// Token: 0x0600057C RID: 1404
		[EngineMethod("get_event_min_max_distance", false)]
		Vec3 GetEventMinMaxDistance(int eventId);

		// Token: 0x0600057D RID: 1405
		[EngineMethod("set_event_position", false)]
		void SetEventPosition(int eventId, ref Vec3 position);

		// Token: 0x0600057E RID: 1406
		[EngineMethod("set_event_velocity", false)]
		void SetEventVelocity(int eventId, ref Vec3 velocity);

		// Token: 0x0600057F RID: 1407
		[EngineMethod("start_event", false)]
		bool StartEvent(int eventId);

		// Token: 0x06000580 RID: 1408
		[EngineMethod("start_event_in_position", false)]
		bool StartEventInPosition(int eventId, ref Vec3 position);

		// Token: 0x06000581 RID: 1409
		[EngineMethod("stop_event", false)]
		void StopEvent(int eventId);

		// Token: 0x06000582 RID: 1410
		[EngineMethod("pause_event", false)]
		void PauseEvent(int eventId);

		// Token: 0x06000583 RID: 1411
		[EngineMethod("resume_event", false)]
		void ResumeEvent(int eventId);

		// Token: 0x06000584 RID: 1412
		[EngineMethod("play_extra_event", false)]
		void PlayExtraEvent(int soundId, string eventName);

		// Token: 0x06000585 RID: 1413
		[EngineMethod("set_switch", false)]
		void SetSwitch(int soundId, string switchGroupName, string newSwitchStateName);

		// Token: 0x06000586 RID: 1414
		[EngineMethod("trigger_cue", false)]
		void TriggerCue(int eventId);

		// Token: 0x06000587 RID: 1415
		[EngineMethod("set_event_parameter_at_index", false)]
		void SetEventParameterAtIndex(int soundId, int parameterIndex, float value);

		// Token: 0x06000588 RID: 1416
		[EngineMethod("is_playing", false)]
		bool IsPlaying(int eventId);

		// Token: 0x06000589 RID: 1417
		[EngineMethod("is_paused", false)]
		bool IsPaused(int eventId);

		// Token: 0x0600058A RID: 1418
		[EngineMethod("is_valid", false)]
		bool IsValid(int eventId);

		// Token: 0x0600058B RID: 1419
		[EngineMethod("create_event_from_external_file", false)]
		int CreateEventFromExternalFile(string programmerSoundEventName, string filePath, UIntPtr scene);

		// Token: 0x0600058C RID: 1420
		[EngineMethod("create_event_from_sound_buffer", false)]
		int CreateEventFromSoundBuffer(string programmerSoundEventName, byte[] soundBuffer, UIntPtr scene);
	}
}
