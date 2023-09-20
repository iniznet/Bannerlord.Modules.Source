using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public class SoundEvent
	{
		public int GetSoundId()
		{
			return this._soundId;
		}

		private SoundEvent(int soundId)
		{
			this._soundId = soundId;
		}

		public static SoundEvent CreateEventFromString(string eventId, Scene scene)
		{
			UIntPtr uintPtr = ((scene == null) ? UIntPtr.Zero : scene.Pointer);
			return new SoundEvent(EngineApplicationInterface.ISoundEvent.CreateEventFromString(eventId, uintPtr));
		}

		public void SetEventMinMaxDistance(Vec3 newRadius)
		{
			EngineApplicationInterface.ISoundEvent.SetEventMinMaxDistance(this._soundId, newRadius);
		}

		public static int GetEventIdFromString(string name)
		{
			return EngineApplicationInterface.ISoundEvent.GetEventIdFromString(name);
		}

		public static bool PlaySound2D(int soundCodeId)
		{
			return EngineApplicationInterface.ISoundEvent.PlaySound2D(soundCodeId);
		}

		public static bool PlaySound2D(string soundName)
		{
			return SoundEvent.PlaySound2D(SoundEvent.GetEventIdFromString(soundName));
		}

		public static int GetTotalEventCount()
		{
			return EngineApplicationInterface.ISoundEvent.GetTotalEventCount();
		}

		public static SoundEvent CreateEvent(int soundCodeId, Scene scene)
		{
			return new SoundEvent(EngineApplicationInterface.ISoundEvent.CreateEvent(soundCodeId, scene.Pointer));
		}

		public bool IsNullSoundEvent()
		{
			return this == SoundEvent.NullSoundEvent;
		}

		public bool IsValid
		{
			get
			{
				return this._soundId != -1 && EngineApplicationInterface.ISoundEvent.IsValid(this._soundId);
			}
		}

		public bool Play()
		{
			return EngineApplicationInterface.ISoundEvent.StartEvent(this._soundId);
		}

		public void Pause()
		{
			EngineApplicationInterface.ISoundEvent.PauseEvent(this._soundId);
		}

		public void Resume()
		{
			EngineApplicationInterface.ISoundEvent.ResumeEvent(this._soundId);
		}

		public void PlayExtraEvent(string eventName)
		{
			EngineApplicationInterface.ISoundEvent.PlayExtraEvent(this._soundId, eventName);
		}

		public void SetSwitch(string switchGroupName, string newSwitchStateName)
		{
			EngineApplicationInterface.ISoundEvent.SetSwitch(this._soundId, switchGroupName, newSwitchStateName);
		}

		public void TriggerCue()
		{
			EngineApplicationInterface.ISoundEvent.TriggerCue(this._soundId);
		}

		public bool PlayInPosition(Vec3 position)
		{
			return EngineApplicationInterface.ISoundEvent.StartEventInPosition(this._soundId, ref position);
		}

		public void Stop()
		{
			if (!this.IsValid)
			{
				return;
			}
			EngineApplicationInterface.ISoundEvent.StopEvent(this._soundId);
			this._soundId = -1;
		}

		public void SetParameter(string parameterName, float value)
		{
			EngineApplicationInterface.ISoundEvent.SetEventParameterFromString(this._soundId, parameterName, value);
		}

		public void SetParameter(int parameterIndex, float value)
		{
			EngineApplicationInterface.ISoundEvent.SetEventParameterAtIndex(this._soundId, parameterIndex, value);
		}

		public Vec3 GetEventMinMaxDistance()
		{
			return EngineApplicationInterface.ISoundEvent.GetEventMinMaxDistance(this._soundId);
		}

		public void SetPosition(Vec3 vec)
		{
			if (!this.IsValid)
			{
				return;
			}
			EngineApplicationInterface.ISoundEvent.SetEventPosition(this._soundId, ref vec);
		}

		public void SetVelocity(Vec3 vec)
		{
			if (!this.IsValid)
			{
				return;
			}
			EngineApplicationInterface.ISoundEvent.SetEventVelocity(this._soundId, ref vec);
		}

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

		public bool IsPlaying()
		{
			return EngineApplicationInterface.ISoundEvent.IsPlaying(this._soundId);
		}

		public bool IsPaused()
		{
			return EngineApplicationInterface.ISoundEvent.IsPaused(this._soundId);
		}

		public static SoundEvent CreateEventFromSoundBuffer(string eventId, byte[] soundData, Scene scene)
		{
			return new SoundEvent(EngineApplicationInterface.ISoundEvent.CreateEventFromSoundBuffer(eventId, soundData, (scene != null) ? scene.Pointer : UIntPtr.Zero));
		}

		public static SoundEvent CreateEventFromExternalFile(string programmerEventName, string soundFilePath, Scene scene)
		{
			return new SoundEvent(EngineApplicationInterface.ISoundEvent.CreateEventFromExternalFile(programmerEventName, soundFilePath, scene.Pointer));
		}

		private const int NullSoundId = -1;

		private static readonly SoundEvent NullSoundEvent = new SoundEvent(-1);

		private int _soundId;
	}
}
