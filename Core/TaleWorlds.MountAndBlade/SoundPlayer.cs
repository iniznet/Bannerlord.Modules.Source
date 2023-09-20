using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200036A RID: 874
	public class SoundPlayer : ScriptComponentBehavior
	{
		// Token: 0x06002FAF RID: 12207 RVA: 0x000C3B4C File Offset: 0x000C1D4C
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

		// Token: 0x06002FB0 RID: 12208 RVA: 0x000C3BB2 File Offset: 0x000C1DB2
		public void UpdatePlaying()
		{
			this.Playing = this.SoundEvent != null && this.SoundEvent.IsValid && this.SoundEvent.IsPlaying();
		}

		// Token: 0x06002FB1 RID: 12209 RVA: 0x000C3BE0 File Offset: 0x000C1DE0
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

		// Token: 0x06002FB2 RID: 12210 RVA: 0x000C3C34 File Offset: 0x000C1E34
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

		// Token: 0x06002FB3 RID: 12211 RVA: 0x000C3C73 File Offset: 0x000C1E73
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

		// Token: 0x06002FB4 RID: 12212 RVA: 0x000C3CA5 File Offset: 0x000C1EA5
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

		// Token: 0x06002FB5 RID: 12213 RVA: 0x000C3CD7 File Offset: 0x000C1ED7
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

		// Token: 0x06002FB6 RID: 12214 RVA: 0x000C3D15 File Offset: 0x000C1F15
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		// Token: 0x06002FB7 RID: 12215 RVA: 0x000C3D1F File Offset: 0x000C1F1F
		protected internal override void OnTick(float dt)
		{
			this.UpdatePlaying();
			if (!this.Playing && this.AutoLoop)
			{
				this.ValidateSoundEvent();
				this.PlaySound();
			}
		}

		// Token: 0x06002FB8 RID: 12216 RVA: 0x000C3D43 File Offset: 0x000C1F43
		protected internal override bool MovesEntity()
		{
			return false;
		}

		// Token: 0x040013C8 RID: 5064
		private bool Playing;

		// Token: 0x040013C9 RID: 5065
		private int SoundCode = -1;

		// Token: 0x040013CA RID: 5066
		private SoundEvent SoundEvent;

		// Token: 0x040013CB RID: 5067
		public bool AutoLoop;

		// Token: 0x040013CC RID: 5068
		public bool AutoStart;

		// Token: 0x040013CD RID: 5069
		public string SoundName;
	}
}
