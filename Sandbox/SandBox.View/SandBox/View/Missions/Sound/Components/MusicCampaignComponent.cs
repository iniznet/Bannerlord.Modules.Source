using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace SandBox.View.Missions.Sound.Components
{
	public class MusicCampaignComponent : MusicMissionPeacefulComponent
	{
		public MusicCampaignComponent()
		{
			this._playTime = 150f;
			this._stopTime = 120f;
		}

		public override void PreInitialize()
		{
			base.PreInitialize();
			base.PeacefulTracks.Add(1);
		}

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

		public override MusicPriority GetPriority()
		{
			return MusicPriority.Campaign;
		}

		public override bool IsActive()
		{
			return GameStateManager.Current.ActiveState is MapState;
		}

		private Timer _musicCheckTimer;

		private readonly float _playTime;

		private readonly float _stopTime;

		private bool _isPlaying;
	}
}
