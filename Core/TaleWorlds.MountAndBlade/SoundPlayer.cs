using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class SoundPlayer : ScriptComponentBehavior
	{
		private void ValidateSoundEvent()
		{
			if ((this.SoundEvent == null || !this.SoundEvent.IsValid) && this.SoundName.Length > 0)
			{
				if (this.SoundCode == -1)
				{
					this.SoundCode = SoundManager.GetEventGlobalIndex(this.SoundName);
				}
				this.SoundEvent = SoundEvent.CreateEvent(this.SoundCode, base.GameEntity.Scene);
			}
		}

		public void UpdatePlaying()
		{
			this.Playing = this.SoundEvent != null && this.SoundEvent.IsValid && this.SoundEvent.IsPlaying();
		}

		public void PlaySound()
		{
			if (this.Playing)
			{
				return;
			}
			if (this.SoundEvent != null && this.SoundEvent.IsValid)
			{
				this.SoundEvent.SetPosition(base.GameEntity.GlobalPosition);
				this.SoundEvent.Play();
				this.Playing = true;
			}
		}

		public void ResumeSound()
		{
			if (this.Playing)
			{
				return;
			}
			if (this.SoundEvent != null && this.SoundEvent.IsValid && this.SoundEvent.IsPaused())
			{
				this.SoundEvent.Resume();
				this.Playing = true;
			}
		}

		public void PauseSound()
		{
			if (!this.Playing)
			{
				return;
			}
			if (this.SoundEvent != null && this.SoundEvent.IsValid)
			{
				this.SoundEvent.Pause();
				this.Playing = false;
			}
		}

		public void StopSound()
		{
			if (!this.Playing)
			{
				return;
			}
			if (this.SoundEvent != null && this.SoundEvent.IsValid)
			{
				this.SoundEvent.Stop();
				this.Playing = false;
			}
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			MBDebug.Print("SoundPlayer : OnInit called.", 0, Debug.DebugColor.Yellow, 17592186044416UL);
			this.ValidateSoundEvent();
			if (this.AutoStart)
			{
				this.PlaySound();
			}
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		protected internal override void OnTick(float dt)
		{
			this.UpdatePlaying();
			if (!this.Playing && this.AutoLoop)
			{
				this.ValidateSoundEvent();
				this.PlaySound();
			}
		}

		protected internal override bool MovesEntity()
		{
			return false;
		}

		private bool Playing;

		private int SoundCode = -1;

		private SoundEvent SoundEvent;

		public bool AutoLoop;

		public bool AutoStart;

		public string SoundName;
	}
}
