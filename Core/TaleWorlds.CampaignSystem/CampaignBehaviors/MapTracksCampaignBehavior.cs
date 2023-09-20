using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003AB RID: 939
	public class MapTracksCampaignBehavior : CampaignBehaviorBase, IMapTracksCampaignBehavior, ICampaignBehavior
	{
		// Token: 0x17000CD3 RID: 3283
		// (get) Token: 0x06003824 RID: 14372 RVA: 0x000FEACD File Offset: 0x000FCCCD
		public MBReadOnlyList<Track> DetectedTracks
		{
			get
			{
				return this._detectedTracksCache;
			}
		}

		// Token: 0x06003825 RID: 14373 RVA: 0x000FEAD8 File Offset: 0x000FCCD8
		public MapTracksCampaignBehavior()
		{
			this._trackPool = new MapTracksCampaignBehavior.TrackPool(2048);
		}

		// Token: 0x06003826 RID: 14374 RVA: 0x000FEB30 File Offset: 0x000FCD30
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.GameLoadFinished));
			CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnHourlyTickParty));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
		}

		// Token: 0x06003827 RID: 14375 RVA: 0x000FEBB0 File Offset: 0x000FCDB0
		private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			if (this._trackDataDictionary.ContainsKey(mobileParty))
			{
				this._trackDataDictionary.Remove(mobileParty);
			}
		}

		// Token: 0x06003828 RID: 14376 RVA: 0x000FEBCD File Offset: 0x000FCDCD
		private void OnNewGameCreated(CampaignGameStarter gameStarted)
		{
			this._trackDataDictionary = new Dictionary<MobileParty, Vec2>();
			this.AddEventHandler();
		}

		// Token: 0x06003829 RID: 14377 RVA: 0x000FEBE0 File Offset: 0x000FCDE0
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<Track>>("_allTracks", ref this._allTracks);
			dataStore.SyncData<Dictionary<MobileParty, Vec2>>("_trackDataDictionary", ref this._trackDataDictionary);
		}

		// Token: 0x0600382A RID: 14378 RVA: 0x000FEC08 File Offset: 0x000FCE08
		private void OnHourlyTickParty(MobileParty mobileParty)
		{
			if (Campaign.Current.Models.MapTrackModel.CanPartyLeaveTrack(mobileParty))
			{
				Vec2 vec = Vec2.Zero;
				if (this._trackDataDictionary.ContainsKey(mobileParty))
				{
					vec = this._trackDataDictionary[mobileParty];
				}
				if (vec.DistanceSquared(mobileParty.Position2D) > 5f && this.IsTrackDropped(mobileParty))
				{
					Vec2 position2D = mobileParty.Position2D;
					Vec2 vec2 = mobileParty.Position2D - vec;
					vec2.Normalize();
					this.AddTrack(mobileParty, position2D, vec2);
					this._trackDataDictionary[mobileParty] = position2D;
				}
			}
		}

		// Token: 0x0600382B RID: 14379 RVA: 0x000FEC9C File Offset: 0x000FCE9C
		private void OnHourlyTick()
		{
			this.RemoveExpiredTracks();
		}

		// Token: 0x0600382C RID: 14380 RVA: 0x000FECA4 File Offset: 0x000FCEA4
		private void GameLoadFinished()
		{
			this._allTracks.RemoveAll((Track x) => x.IsExpired);
			this._detectedTracksCache = this._allTracks.Where((Track x) => x.IsDetected).ToMBList<Track>();
			this.AddEventHandler();
			foreach (Track track in this._allTracks)
			{
				this._trackLocator.UpdateLocator(track);
			}
			foreach (MobileParty mobileParty in this._trackDataDictionary.Keys.ToList<MobileParty>())
			{
				if (!mobileParty.IsActive)
				{
					this._trackDataDictionary.Remove(mobileParty);
				}
			}
		}

		// Token: 0x0600382D RID: 14381 RVA: 0x000FEDC0 File Offset: 0x000FCFC0
		private void AddEventHandler()
		{
			this._quarterHourlyTick = CampaignPeriodicEventManager.CreatePeriodicEvent(CampaignTime.Hours(0.25f), CampaignTime.Hours(0.1f));
			this._quarterHourlyTick.AddHandler(new MBCampaignEvent.CampaignEventDelegate(this.QuarterHourlyTick));
		}

		// Token: 0x0600382E RID: 14382 RVA: 0x000FEDF8 File Offset: 0x000FCFF8
		private void QuarterHourlyTick(MBCampaignEvent campaignEvent, object[] delegateParams)
		{
			if (!PartyBase.MainParty.IsValid)
			{
				return;
			}
			int num = ((MobileParty.MainParty.EffectiveScout != null) ? MobileParty.MainParty.EffectiveScout.GetSkillValue(DefaultSkills.Scouting) : 0);
			if (num != 0)
			{
				float maxTrackSpottingDistanceForMainParty = Campaign.Current.Models.MapTrackModel.GetMaxTrackSpottingDistanceForMainParty();
				LocatableSearchData<Track> locatableSearchData = this._trackLocator.StartFindingLocatablesAroundPosition(MobileParty.MainParty.Position2D, maxTrackSpottingDistanceForMainParty);
				for (Track track = this._trackLocator.FindNextLocatable(ref locatableSearchData); track != null; track = this._trackLocator.FindNextLocatable(ref locatableSearchData))
				{
					if (!track.IsDetected && this._allTracks.Contains(track) && Campaign.Current.Models.MapTrackModel.GetTrackDetectionDifficultyForMainParty(track, maxTrackSpottingDistanceForMainParty) < (float)num)
					{
						this.TrackDetected(track);
					}
				}
			}
		}

		// Token: 0x0600382F RID: 14383 RVA: 0x000FEEC0 File Offset: 0x000FD0C0
		private void RemoveExpiredTracks()
		{
			for (int i = this._allTracks.Count - 1; i >= 0; i--)
			{
				Track track = this._allTracks[i];
				if (track.IsExpired)
				{
					this._allTracks.Remove(track);
					if (this._detectedTracksCache.Contains(track))
					{
						this._detectedTracksCache.Remove(track);
						CampaignEventDispatcher.Instance.TrackLost(track);
					}
					this._trackLocator.RemoveLocatable(track);
					this._trackPool.ReleaseTrack(track);
				}
			}
		}

		// Token: 0x06003830 RID: 14384 RVA: 0x000FEF45 File Offset: 0x000FD145
		private void TrackDetected(Track track)
		{
			track.IsDetected = true;
			this._detectedTracksCache.Add(track);
			CampaignEventDispatcher.Instance.TrackDetected(track);
			SkillLevelingManager.OnTrackDetected(track);
		}

		// Token: 0x06003831 RID: 14385 RVA: 0x000FEF6C File Offset: 0x000FD16C
		public bool IsTrackDropped(MobileParty mobileParty)
		{
			float skipTrackChance = Campaign.Current.Models.MapTrackModel.GetSkipTrackChance(mobileParty);
			if (MBRandom.RandomFloat < skipTrackChance)
			{
				return false;
			}
			float num = mobileParty.Position2D.DistanceSquared(MobileParty.MainParty.Position2D);
			float num2 = MobileParty.MainParty.Speed * Campaign.Current.Models.MapTrackModel.MaxTrackLife;
			return num2 * num2 > num;
		}

		// Token: 0x06003832 RID: 14386 RVA: 0x000FEFDC File Offset: 0x000FD1DC
		public void AddTrack(MobileParty party, Vec2 trackPosition, Vec2 trackDirection)
		{
			Track track = this._trackPool.RequestTrack(party, trackPosition, trackDirection);
			this._allTracks.Add(track);
			this._trackLocator.UpdateLocator(track);
		}

		// Token: 0x06003833 RID: 14387 RVA: 0x000FF014 File Offset: 0x000FD214
		public void AddMapArrow(TextObject pointerName, Vec2 trackPosition, Vec2 trackDirection, float life)
		{
			Track track = this._trackPool.RequestMapArrow(pointerName, trackPosition, trackDirection, life);
			this._allTracks.Add(track);
			this._trackLocator.UpdateLocator(track);
			this.TrackDetected(track);
		}

		// Token: 0x04001196 RID: 4502
		private const float PartyTrackPositionDelta = 5f;

		// Token: 0x04001197 RID: 4503
		private List<Track> _allTracks = new List<Track>();

		// Token: 0x04001198 RID: 4504
		private MBList<Track> _detectedTracksCache = new MBList<Track>();

		// Token: 0x04001199 RID: 4505
		private Dictionary<MobileParty, Vec2> _trackDataDictionary = new Dictionary<MobileParty, Vec2>();

		// Token: 0x0400119A RID: 4506
		private MBCampaignEvent _quarterHourlyTick;

		// Token: 0x0400119B RID: 4507
		private LocatorGrid<Track> _trackLocator = new LocatorGrid<Track>(5f, 32, 32);

		// Token: 0x0400119C RID: 4508
		private MapTracksCampaignBehavior.TrackPool _trackPool;

		// Token: 0x02000705 RID: 1797
		private class TrackPool
		{
			// Token: 0x17001363 RID: 4963
			// (get) Token: 0x06005564 RID: 21860 RVA: 0x0016CF35 File Offset: 0x0016B135
			private int MaxSize { get; }

			// Token: 0x17001364 RID: 4964
			// (get) Token: 0x06005565 RID: 21861 RVA: 0x0016CF3D File Offset: 0x0016B13D
			public int Size
			{
				get
				{
					Stack<Track> stack = this._stack;
					if (stack == null)
					{
						return 0;
					}
					return stack.Count;
				}
			}

			// Token: 0x06005566 RID: 21862 RVA: 0x0016CF50 File Offset: 0x0016B150
			public TrackPool(int size)
			{
				this.MaxSize = size;
				this._stack = new Stack<Track>();
				for (int i = 0; i < size; i++)
				{
					this._stack.Push(new Track());
				}
			}

			// Token: 0x06005567 RID: 21863 RVA: 0x0016CF94 File Offset: 0x0016B194
			public Track RequestTrack(MobileParty party, Vec2 trackPosition, Vec2 trackDirection)
			{
				Track track = ((this._stack.Count > 0) ? this._stack.Pop() : new Track());
				int num = party.Party.NumberOfAllMembers;
				int num2 = party.Party.NumberOfHealthyMembers;
				int num3 = party.Party.NumberOfMenWithHorse;
				int num4 = party.Party.NumberOfMenWithoutHorse;
				int num5 = party.Party.NumberOfPackAnimals;
				int num6 = party.Party.NumberOfPrisoners;
				TextObject textObject = party.Name;
				if (party.Army != null && party.Army.LeaderParty == party)
				{
					textObject = party.ArmyName;
					foreach (MobileParty mobileParty in party.Army.LeaderParty.AttachedParties)
					{
						num += mobileParty.Party.NumberOfAllMembers;
						num2 += mobileParty.Party.NumberOfHealthyMembers;
						num3 += mobileParty.Party.NumberOfMenWithHorse;
						num4 += mobileParty.Party.NumberOfMenWithoutHorse;
						num5 += mobileParty.Party.NumberOfPackAnimals;
						num6 += mobileParty.Party.NumberOfPrisoners;
					}
				}
				track.Position = trackPosition;
				track.Direction = trackDirection.RotationInRadians;
				track.PartyType = Track.GetPartyTypeEnum(party);
				track.PartyName = textObject;
				track.Culture = party.Party.Culture;
				if (track.Culture == null)
				{
					string text = string.Format("Track culture is null for {0}", party.Name);
					Debug.Print(text, 0, Debug.DebugColor.White, 17592186044416UL);
					Debug.FailedAssert(text, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CampaignBehaviors\\MapTracksCampaignBehavior.cs", "RequestTrack", 62);
				}
				track.Speed = party.Speed;
				track.Life = (float)Campaign.Current.Models.MapTrackModel.GetTrackLife(party);
				track.IsEnemy = FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, party.MapFaction);
				track.NumberOfAllMembers = num;
				track.NumberOfHealthyMembers = num2;
				track.NumberOfMenWithHorse = num3;
				track.NumberOfMenWithoutHorse = num4;
				track.NumberOfPackAnimals = num5;
				track.NumberOfPrisoners = num6;
				track.IsPointer = false;
				track.IsDetected = false;
				track.CreationTime = CampaignTime.Now;
				return track;
			}

			// Token: 0x06005568 RID: 21864 RVA: 0x0016D1E4 File Offset: 0x0016B3E4
			public Track RequestMapArrow(TextObject pointerName, Vec2 trackPosition, Vec2 trackDirection, float life)
			{
				Track track = ((this._stack.Count > 0) ? this._stack.Pop() : new Track());
				track.Position = trackPosition;
				track.Direction = trackDirection.RotationInRadians;
				track.PartyName = pointerName;
				track.Life = life;
				track.IsPointer = true;
				track.IsDetected = true;
				track.CreationTime = CampaignTime.Now;
				return track;
			}

			// Token: 0x06005569 RID: 21865 RVA: 0x0016D24D File Offset: 0x0016B44D
			public void ReleaseTrack(Track track)
			{
				track.Reset();
				if (this._stack.Count < this.MaxSize)
				{
					this._stack.Push(track);
				}
			}

			// Token: 0x0600556A RID: 21866 RVA: 0x0016D274 File Offset: 0x0016B474
			public override string ToString()
			{
				return string.Format("TrackPool: {0}", this.Size);
			}

			// Token: 0x04001CDB RID: 7387
			private Stack<Track> _stack;
		}
	}
}
