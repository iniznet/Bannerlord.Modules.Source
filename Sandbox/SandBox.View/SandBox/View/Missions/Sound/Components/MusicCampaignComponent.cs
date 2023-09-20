using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace SandBox.View.Missions.Sound.Components
{
	// Token: 0x02000028 RID: 40
	public class MusicCampaignComponent : MusicMissionPeacefulComponent
	{
		// Token: 0x0600014F RID: 335 RVA: 0x00010C84 File Offset: 0x0000EE84
		public MusicCampaignComponent()
		{
			this._playTime = 150f;
			this._stopTime = 120f;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00010CA2 File Offset: 0x0000EEA2
		public override void PreInitialize()
		{
			base.PreInitialize();
			base.PeacefulTracks.Add(1);
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00010CB8 File Offset: 0x0000EEB8
		public override void Tick(float dt)
		{
			base.Tick(dt);
			if (this._musicCheckTimer.Check(Game.Current.ApplicationTime))
			{
				if (this._isPlaying)
				{
					this._musicCheckTimer.Reset(Game.Current.ApplicationTime, this._stopTime);
					this.CurrentMood = -1;
				}
				else
				{
					this._musicCheckTimer.Reset(Game.Current.ApplicationTime, this._playTime);
					this.CurrentMood = base.SelectNewPeacefulTrack();
				}
				this._isPlaying = !this._isPlaying;
			}
			if (MBMusicManagerOld.GetCurrentMood() != this.CurrentMood)
			{
				if (this.CurrentMood == -1)
				{
					MBMusicManagerOld.StopMusic(false);
					return;
				}
				MBMusicManagerOld.SetMood(this.CurrentMood, 0.1f, true, false);
			}
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00010D74 File Offset: 0x0000EF74
		public override void OnActived()
		{
			base.OnActived();
			MBMusicManagerOld.LeaveMenuMode();
			this._isPlaying = true;
			float applicationTime = Game.Current.ApplicationTime;
			this._musicCheckTimer = new Timer(applicationTime, this._playTime, true);
			this._musicCheckTimer.Check(applicationTime);
			this.CurrentMood = base.SelectNewPeacefulTrack();
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00010DCA File Offset: 0x0000EFCA
		public override MusicPriority GetPriority()
		{
			return MusicPriority.Campaign;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00010DCD File Offset: 0x0000EFCD
		public override bool IsActive()
		{
			return GameStateManager.Current.ActiveState is MapState;
		}

		// Token: 0x040000C9 RID: 201
		private Timer _musicCheckTimer;

		// Token: 0x040000CA RID: 202
		private readonly float _playTime;

		// Token: 0x040000CB RID: 203
		private readonly float _stopTime;

		// Token: 0x040000CC RID: 204
		private bool _isPlaying;
	}
}
