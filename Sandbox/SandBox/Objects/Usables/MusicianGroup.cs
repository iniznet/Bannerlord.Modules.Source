using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.AI;
using SandBox.Objects.AnimationPoints;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace SandBox.Objects.Usables
{
	public class MusicianGroup : UsableMachine
	{
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			return TextObject.Empty;
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return string.Empty;
		}

		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new UsablePlaceAI(this);
		}

		public void SetPlayList(List<SettlementMusicData> playList)
		{
			this._playList = playList.ToList<SettlementMusicData>();
		}

		protected override void OnInit()
		{
			base.OnInit();
			this._playList = new List<SettlementMusicData>();
			this._musicianPoints = base.StandingPoints.OfType<PlayMusicPoint>().ToList<PlayMusicPoint>();
			MBMusicManagerOld.StopMusic(false);
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2 | base.GetTickRequirement();
		}

		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			this.CheckNewTrackStart();
			this.CheckTrackEnd();
		}

		private void CheckNewTrackStart()
		{
			if (this._playList.Count > 0 && this._trackEvent == null && (this._gapTimer == null || this._gapTimer.ElapsedTime > 8f))
			{
				if (this._musicianPoints.Any((PlayMusicPoint x) => x.HasUser))
				{
					this._currentTrackIndex++;
					if (this._currentTrackIndex == this._playList.Count)
					{
						this._currentTrackIndex = 0;
					}
					this.SetupInstruments();
					this.StartTrack();
					this._gapTimer = null;
				}
			}
		}

		private void CheckTrackEnd()
		{
			if (this._trackEvent != null)
			{
				if (this._trackEvent.IsPlaying())
				{
					if (!this._musicianPoints.Any((PlayMusicPoint x) => x.HasUser))
					{
						this._trackEvent.Stop();
					}
				}
				if (this._trackEvent != null && !this._trackEvent.IsPlaying())
				{
					this._trackEvent.Release();
					this._trackEvent = null;
					this.StopMusicians();
					this._gapTimer = new BasicMissionTimer();
				}
			}
		}

		private void StopMusicians()
		{
			foreach (PlayMusicPoint playMusicPoint in this._musicianPoints)
			{
				if (playMusicPoint.HasUser)
				{
					playMusicPoint.EndLoop();
				}
			}
		}

		private void SetupInstruments()
		{
			List<PlayMusicPoint> list = this._musicianPoints.ToList<PlayMusicPoint>();
			Extensions.Shuffle<PlayMusicPoint>(list);
			SettlementMusicData settlementMusicData = this._playList[this._currentTrackIndex];
			using (List<InstrumentData>.Enumerator enumerator = settlementMusicData.Instruments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					InstrumentData instrumentData = enumerator.Current;
					PlayMusicPoint playMusicPoint = list.FirstOrDefault((PlayMusicPoint x) => x.GameEntity.Parent.Tags.Contains(instrumentData.Tag) || string.IsNullOrEmpty(instrumentData.Tag));
					if (playMusicPoint != null)
					{
						Tuple<InstrumentData, float> tuple = new Tuple<InstrumentData, float>(instrumentData, (float)settlementMusicData.Tempo / 120f);
						playMusicPoint.ChangeInstrument(tuple);
						list.Remove(playMusicPoint);
					}
				}
			}
			Tuple<InstrumentData, float> instrumentEmptyData = this.GetInstrumentEmptyData(settlementMusicData.Tempo);
			foreach (PlayMusicPoint playMusicPoint2 in list)
			{
				playMusicPoint2.ChangeInstrument(instrumentEmptyData);
			}
		}

		private Tuple<InstrumentData, float> GetInstrumentEmptyData(int tempo)
		{
			Tuple<InstrumentData, float> tuple;
			if (tempo > 130)
			{
				tuple = new Tuple<InstrumentData, float>(MBObjectManager.Instance.GetObject<InstrumentData>("cheerful"), 1f);
			}
			else if (tempo > 100)
			{
				tuple = new Tuple<InstrumentData, float>(MBObjectManager.Instance.GetObject<InstrumentData>("active"), 1f);
			}
			else
			{
				tuple = new Tuple<InstrumentData, float>(MBObjectManager.Instance.GetObject<InstrumentData>("calm"), 1f);
			}
			return tuple;
		}

		private void StartTrack()
		{
			int eventIdFromString = SoundEvent.GetEventIdFromString(this._playList[this._currentTrackIndex].MusicPath);
			this._trackEvent = SoundEvent.CreateEvent(eventIdFromString, Mission.Current.Scene);
			this._trackEvent.SetPosition(base.GameEntity.GetGlobalFrame().origin);
			this._trackEvent.Play();
			MBMusicManagerOld.StopMusic(true);
			foreach (PlayMusicPoint playMusicPoint in this._musicianPoints)
			{
				playMusicPoint.StartLoop(this._trackEvent);
			}
		}

		public const int GapBetweenTracks = 8;

		public const bool DisableAmbientMusic = true;

		private const int TempoMidValue = 120;

		private const int TempoSpeedUpLimit = 130;

		private const int TempoSlowDownLimit = 100;

		private List<PlayMusicPoint> _musicianPoints;

		private SoundEvent _trackEvent;

		private BasicMissionTimer _gapTimer;

		private List<SettlementMusicData> _playList;

		private int _currentTrackIndex = -1;
	}
}
