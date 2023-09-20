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
	// Token: 0x02000027 RID: 39
	public class MusicianGroup : UsableMachine
	{
		// Token: 0x060001C6 RID: 454 RVA: 0x0000C68F File Offset: 0x0000A88F
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			return TextObject.Empty;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000C696 File Offset: 0x0000A896
		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return string.Empty;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000C69D File Offset: 0x0000A89D
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new UsablePlaceAI(this);
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000C6A5 File Offset: 0x0000A8A5
		public void SetPlayList(List<SettlementMusicData> playList)
		{
			this._playList = playList.ToList<SettlementMusicData>();
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000C6B3 File Offset: 0x0000A8B3
		protected override void OnInit()
		{
			base.OnInit();
			this._playList = new List<SettlementMusicData>();
			this._musicianPoints = base.StandingPoints.OfType<PlayMusicPoint>().ToList<PlayMusicPoint>();
			MBMusicManagerOld.StopMusic(false);
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000C6E2 File Offset: 0x0000A8E2
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2 | base.GetTickRequirement();
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000C6EC File Offset: 0x0000A8EC
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			this.CheckNewTrackStart();
			this.CheckTrackEnd();
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000C704 File Offset: 0x0000A904
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

		// Token: 0x060001CE RID: 462 RVA: 0x0000C7B0 File Offset: 0x0000A9B0
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

		// Token: 0x060001CF RID: 463 RVA: 0x0000C844 File Offset: 0x0000AA44
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

		// Token: 0x060001D0 RID: 464 RVA: 0x0000C8A0 File Offset: 0x0000AAA0
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

		// Token: 0x060001D1 RID: 465 RVA: 0x0000C9AC File Offset: 0x0000ABAC
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

		// Token: 0x060001D2 RID: 466 RVA: 0x0000CA1C File Offset: 0x0000AC1C
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

		// Token: 0x040000B9 RID: 185
		public const int GapBetweenTracks = 8;

		// Token: 0x040000BA RID: 186
		public const bool DisableAmbientMusic = true;

		// Token: 0x040000BB RID: 187
		private const int TempoMidValue = 120;

		// Token: 0x040000BC RID: 188
		private const int TempoSpeedUpLimit = 130;

		// Token: 0x040000BD RID: 189
		private const int TempoSlowDownLimit = 100;

		// Token: 0x040000BE RID: 190
		private List<PlayMusicPoint> _musicianPoints;

		// Token: 0x040000BF RID: 191
		private SoundEvent _trackEvent;

		// Token: 0x040000C0 RID: 192
		private BasicMissionTimer _gapTimer;

		// Token: 0x040000C1 RID: 193
		private List<SettlementMusicData> _playList;

		// Token: 0x040000C2 RID: 194
		private int _currentTrackIndex = -1;
	}
}
