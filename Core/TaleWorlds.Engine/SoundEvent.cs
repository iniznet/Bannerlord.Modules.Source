using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000087 RID: 135
	public class SoundEvent
	{
		// Token: 0x06000A44 RID: 2628 RVA: 0x0000B372 File Offset: 0x00009572
		public int GetSoundId()
		{
			return this._soundId;
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x0000B37A File Offset: 0x0000957A
		private SoundEvent(int soundId)
		{
			this._soundId = soundId;
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x0000B38C File Offset: 0x0000958C
		public static SoundEvent CreateEventFromString(string eventId, Scene scene)
		{
			UIntPtr uintPtr = ((scene == null) ? UIntPtr.Zero : scene.Pointer);
			return new SoundEvent(EngineApplicationInterface.ISoundEvent.CreateEventFromString(eventId, uintPtr));
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x0000B3C1 File Offset: 0x000095C1
		public void SetEventMinMaxDistance(Vec3 newRadius)
		{
			EngineApplicationInterface.ISoundEvent.SetEventMinMaxDistance(this._soundId, newRadius);
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x0000B3D4 File Offset: 0x000095D4
		public static int GetEventIdFromString(string name)
		{
			return EngineApplicationInterface.ISoundEvent.GetEventIdFromString(name);
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x0000B3E1 File Offset: 0x000095E1
		public static bool PlaySound2D(int soundCodeId)
		{
			return EngineApplicationInterface.ISoundEvent.PlaySound2D(soundCodeId);
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x0000B3EE File Offset: 0x000095EE
		public static bool PlaySound2D(string soundName)
		{
			return SoundEvent.PlaySound2D(SoundEvent.GetEventIdFromString(soundName));
		}

		// Token: 0x06000A4B RID: 2635 RVA: 0x0000B3FB File Offset: 0x000095FB
		public static int GetTotalEventCount()
		{
			return EngineApplicationInterface.ISoundEvent.GetTotalEventCount();
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x0000B407 File Offset: 0x00009607
		public static SoundEvent CreateEvent(int soundCodeId, Scene scene)
		{
			return new SoundEvent(EngineApplicationInterface.ISoundEvent.CreateEvent(soundCodeId, scene.Pointer));
		}

		// Token: 0x06000A4D RID: 2637 RVA: 0x0000B41F File Offset: 0x0000961F
		public bool IsNullSoundEvent()
		{
			return this == SoundEvent.NullSoundEvent;
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000A4E RID: 2638 RVA: 0x0000B429 File Offset: 0x00009629
		public bool IsValid
		{
			get
			{
				return this._soundId != -1 && EngineApplicationInterface.ISoundEvent.IsValid(this._soundId);
			}
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x0000B446 File Offset: 0x00009646
		public bool Play()
		{
			return EngineApplicationInterface.ISoundEvent.StartEvent(this._soundId);
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x0000B458 File Offset: 0x00009658
		public void Pause()
		{
			EngineApplicationInterface.ISoundEvent.PauseEvent(this._soundId);
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x0000B46A File Offset: 0x0000966A
		public void Resume()
		{
			EngineApplicationInterface.ISoundEvent.ResumeEvent(this._soundId);
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x0000B47C File Offset: 0x0000967C
		public void PlayExtraEvent(string eventName)
		{
			EngineApplicationInterface.ISoundEvent.PlayExtraEvent(this._soundId, eventName);
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x0000B48F File Offset: 0x0000968F
		public void SetSwitch(string switchGroupName, string newSwitchStateName)
		{
			EngineApplicationInterface.ISoundEvent.SetSwitch(this._soundId, switchGroupName, newSwitchStateName);
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x0000B4A3 File Offset: 0x000096A3
		public void TriggerCue()
		{
			EngineApplicationInterface.ISoundEvent.TriggerCue(this._soundId);
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x0000B4B5 File Offset: 0x000096B5
		public bool PlayInPosition(Vec3 position)
		{
			return EngineApplicationInterface.ISoundEvent.StartEventInPosition(this._soundId, ref position);
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x0000B4C9 File Offset: 0x000096C9
		public void Stop()
		{
			if (!this.IsValid)
			{
				return;
			}
			EngineApplicationInterface.ISoundEvent.StopEvent(this._soundId);
			this._soundId = -1;
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x0000B4EB File Offset: 0x000096EB
		public void SetParameter(string parameterName, float value)
		{
			EngineApplicationInterface.ISoundEvent.SetEventParameterFromString(this._soundId, parameterName, value);
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x0000B4FF File Offset: 0x000096FF
		public void SetParameter(int parameterIndex, float value)
		{
			EngineApplicationInterface.ISoundEvent.SetEventParameterAtIndex(this._soundId, parameterIndex, value);
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x0000B513 File Offset: 0x00009713
		public Vec3 GetEventMinMaxDistance()
		{
			return EngineApplicationInterface.ISoundEvent.GetEventMinMaxDistance(this._soundId);
		}

		// Token: 0x06000A5A RID: 2650 RVA: 0x0000B525 File Offset: 0x00009725
		public void SetPosition(Vec3 vec)
		{
			if (!this.IsValid)
			{
				return;
			}
			EngineApplicationInterface.ISoundEvent.SetEventPosition(this._soundId, ref vec);
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x0000B542 File Offset: 0x00009742
		public void SetVelocity(Vec3 vec)
		{
			if (!this.IsValid)
			{
				return;
			}
			EngineApplicationInterface.ISoundEvent.SetEventVelocity(this._soundId, ref vec);
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x0000B560 File Offset: 0x00009760
		public void Release()
		{
			MBDebug.Print("Release Sound Event " + this._soundId, 0, Debug.DebugColor.Red, 17592186044416UL);
			if (this.IsValid)
			{
				if (this.IsPlaying())
				{
					this.Stop();
				}
				EngineApplicationInterface.ISoundEvent.ReleaseEvent(this._soundId);
			}
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x0000B5B8 File Offset: 0x000097B8
		public bool IsPlaying()
		{
			return EngineApplicationInterface.ISoundEvent.IsPlaying(this._soundId);
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x0000B5CA File Offset: 0x000097CA
		public bool IsPaused()
		{
			return EngineApplicationInterface.ISoundEvent.IsPaused(this._soundId);
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x0000B5DC File Offset: 0x000097DC
		public static SoundEvent CreateEventFromSoundBuffer(string eventId, byte[] soundData, Scene scene)
		{
			return new SoundEvent(EngineApplicationInterface.ISoundEvent.CreateEventFromSoundBuffer(eventId, soundData, (scene != null) ? scene.Pointer : UIntPtr.Zero));
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x0000B605 File Offset: 0x00009805
		public static SoundEvent CreateEventFromExternalFile(string programmerEventName, string soundFilePath, Scene scene)
		{
			return new SoundEvent(EngineApplicationInterface.ISoundEvent.CreateEventFromExternalFile(programmerEventName, soundFilePath, scene.Pointer));
		}

		// Token: 0x040001A4 RID: 420
		private const int NullSoundId = -1;

		// Token: 0x040001A5 RID: 421
		private static readonly SoundEvent NullSoundEvent = new SoundEvent(-1);

		// Token: 0x040001A6 RID: 422
		private int _soundId;
	}
}
