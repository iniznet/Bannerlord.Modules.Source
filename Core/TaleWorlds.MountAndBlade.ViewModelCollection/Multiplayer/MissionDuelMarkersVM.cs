using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000036 RID: 54
	public class MissionDuelMarkersVM : ViewModel
	{
		// Token: 0x0600045E RID: 1118 RVA: 0x00013F54 File Offset: 0x00012154
		public MissionDuelMarkersVM(Camera missionCamera, MissionMultiplayerGameModeDuelClient client)
		{
			this._missionCamera = missionCamera;
			this._client = client;
			List<GameEntity> list = new List<GameEntity>();
			list.AddRange(Mission.Current.Scene.FindEntitiesWithTag("duel_zone_landmark"));
			this.Landmarks = new MBBindingList<MissionDuelLandmarkMarkerVM>();
			foreach (GameEntity gameEntity in list)
			{
				this.Landmarks.Add(new MissionDuelLandmarkMarkerVM(gameEntity));
			}
			this.Targets = new MBBindingList<MissionDuelPeerMarkerVM>();
			this._targetPeersToMarkersDictionary = new Dictionary<MissionPeer, MissionDuelPeerMarkerVM>();
			this._targetPeersInDuelDictionary = new Dictionary<MissionPeer, bool>();
			this._distanceComparer = new MissionDuelMarkersVM.PeerMarkerDistanceComparer();
			this.UpdateScreenCenter();
			this.RefreshValues();
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x00014024 File Offset: 0x00012224
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Targets.ApplyActionOnAllItems(delegate(MissionDuelPeerMarkerVM t)
			{
				t.RefreshValues();
			});
			this.Landmarks.ApplyActionOnAllItems(delegate(MissionDuelLandmarkMarkerVM l)
			{
				l.RefreshValues();
			});
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x0001408B File Offset: 0x0001228B
		public void UpdateScreenCenter()
		{
			this._screenCenter = new Vec2(Screen.RealScreenResolutionWidth / 2f, Screen.RealScreenResolutionHeight / 2f);
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x000140AE File Offset: 0x000122AE
		public void Tick(float dt)
		{
			if (this._hasEnteredLobby && GameNetwork.MyPeer != null)
			{
				this.OnRefreshPeerMarkers();
				this.UpdateTargets(dt);
			}
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x000140CC File Offset: 0x000122CC
		public void RegisterEvents()
		{
			DuelMissionRepresentative myRepresentative = this._client.MyRepresentative;
			myRepresentative.OnDuelRequestSentEvent = (Action<MissionPeer>)Delegate.Combine(myRepresentative.OnDuelRequestSentEvent, new Action<MissionPeer>(this.OnDuelRequestSent));
			DuelMissionRepresentative myRepresentative2 = this._client.MyRepresentative;
			myRepresentative2.OnDuelRequestedEvent = (Action<MissionPeer, TroopType>)Delegate.Combine(myRepresentative2.OnDuelRequestedEvent, new Action<MissionPeer, TroopType>(this.OnDuelRequested));
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x00014134 File Offset: 0x00012334
		public void UnregisterEvents()
		{
			DuelMissionRepresentative myRepresentative = this._client.MyRepresentative;
			myRepresentative.OnDuelRequestSentEvent = (Action<MissionPeer>)Delegate.Remove(myRepresentative.OnDuelRequestSentEvent, new Action<MissionPeer>(this.OnDuelRequestSent));
			DuelMissionRepresentative myRepresentative2 = this._client.MyRepresentative;
			myRepresentative2.OnDuelRequestedEvent = (Action<MissionPeer, TroopType>)Delegate.Remove(myRepresentative2.OnDuelRequestedEvent, new Action<MissionPeer, TroopType>(this.OnDuelRequested));
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x0001419C File Offset: 0x0001239C
		private void UpdateTargets(float dt)
		{
			if (this._currentFocusTarget != null)
			{
				this._previousFocusTarget = this._currentFocusTarget;
				this._currentFocusTarget = null;
				if (this._isPlayerFocused)
				{
					this._previousFocusTarget.IsFocused = false;
				}
			}
			if (this._currentLandmarkTarget != null)
			{
				this._previousLandmarkTarget = this._currentLandmarkTarget;
				this._currentLandmarkTarget = null;
				if (this._isPlayerFocused)
				{
					this._previousLandmarkTarget.IsFocused = false;
				}
			}
			DuelMissionRepresentative myRepresentative = this._client.MyRepresentative;
			if (((myRepresentative != null) ? myRepresentative.MissionPeer.ControlledAgent : null) != null)
			{
				float num = float.MaxValue;
				foreach (MissionDuelPeerMarkerVM missionDuelPeerMarkerVM in this.Targets)
				{
					missionDuelPeerMarkerVM.OnTick(dt);
					if (missionDuelPeerMarkerVM.IsEnabled)
					{
						if (!missionDuelPeerMarkerVM.HasSentDuelRequest && !missionDuelPeerMarkerVM.HasDuelRequestForPlayer && missionDuelPeerMarkerVM.TargetPeer.ControlledAgent != null)
						{
							missionDuelPeerMarkerVM.PreferredArenaType = this._playerPreferredArenaType;
						}
						missionDuelPeerMarkerVM.UpdateScreenPosition(this._missionCamera);
						missionDuelPeerMarkerVM.HasDuelRequestForPlayer = this._client.MyRepresentative.CheckHasRequestFromAndRemoveRequestIfNeeded(missionDuelPeerMarkerVM.TargetPeer);
						float num2 = missionDuelPeerMarkerVM.ScreenPosition.Distance(this._screenCenter);
						if (!this._isPlayerFocused && missionDuelPeerMarkerVM.WSign >= 0 && num2 < 350f && num2 < num)
						{
							num = num2;
							this._currentFocusTarget = missionDuelPeerMarkerVM;
						}
					}
				}
				this.Targets.Sort(this._distanceComparer);
				if (this._client.MyRepresentative != null)
				{
					if (this._currentFocusTarget != null && this._currentFocusTarget.TargetPeer.ControlledAgent != null)
					{
						this._client.MyRepresentative.OnObjectFocused(this._currentFocusTarget.TargetPeer.ControlledAgent);
						if (this._previousFocusTarget != null && this._currentFocusTarget.TargetPeer != this._previousFocusTarget.TargetPeer)
						{
							this._previousFocusTarget.IsFocused = false;
						}
						this._currentFocusTarget.IsFocused = true;
						if (this._previousLandmarkTarget != null)
						{
							this._previousLandmarkTarget.IsFocused = false;
							return;
						}
					}
					else
					{
						if (this._previousFocusTarget != null)
						{
							this._previousFocusTarget.IsFocused = false;
						}
						foreach (MissionDuelLandmarkMarkerVM missionDuelLandmarkMarkerVM in this.Landmarks)
						{
							if (Agent.Main != null)
							{
								missionDuelLandmarkMarkerVM.UpdateScreenPosition(this._missionCamera);
								if (!this._isPlayerFocused && missionDuelLandmarkMarkerVM.IsInScreenBoundaries && Agent.Main.GetWorldPosition().GetGroundVec3().DistanceSquared(missionDuelLandmarkMarkerVM.Entity.GlobalPosition) < 500f)
								{
									missionDuelLandmarkMarkerVM.IsFocused = true;
									this._currentLandmarkTarget = missionDuelLandmarkMarkerVM;
									if (this._previousLandmarkTarget != missionDuelLandmarkMarkerVM)
									{
										if (this._previousLandmarkTarget != null)
										{
											this._previousLandmarkTarget.IsFocused = false;
										}
										this._currentLandmarkTarget.IsFocused = true;
									}
									this._client.MyRepresentative.OnObjectFocused(missionDuelLandmarkMarkerVM.FocusableComponent);
									break;
								}
							}
						}
						if (this._currentLandmarkTarget == null && this._previousLandmarkTarget != null)
						{
							this._previousLandmarkTarget.IsFocused = false;
						}
						if (this._currentFocusTarget == null && this._currentLandmarkTarget == null)
						{
							this._client.MyRepresentative.OnObjectFocusLost();
						}
					}
				}
			}
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x000144FC File Offset: 0x000126FC
		private void OnRefreshPeerMarkers()
		{
			List<MissionDuelPeerMarkerVM> list = this.Targets.ToList<MissionDuelPeerMarkerVM>();
			foreach (MissionPeer missionPeer in VirtualPlayer.Peers<MissionPeer>())
			{
				if (((missionPeer != null) ? missionPeer.Team : null) != null && missionPeer.IsControlledAgentActive && !missionPeer.IsMine)
				{
					if (!this._targetPeersToMarkersDictionary.ContainsKey(missionPeer))
					{
						MissionDuelPeerMarkerVM missionDuelPeerMarkerVM = new MissionDuelPeerMarkerVM(missionPeer);
						this.Targets.Add(missionDuelPeerMarkerVM);
						this._targetPeersToMarkersDictionary.Add(missionPeer, missionDuelPeerMarkerVM);
						this.OnPeerEquipmentRefreshed(missionPeer);
						if (this._targetPeersInDuelDictionary.ContainsKey(missionPeer))
						{
							missionDuelPeerMarkerVM.UpdateCurentDuelStatus(this._targetPeersInDuelDictionary[missionPeer]);
						}
					}
					else
					{
						list.Remove(this._targetPeersToMarkersDictionary[missionPeer]);
					}
					if (!this._targetPeersInDuelDictionary.ContainsKey(missionPeer))
					{
						this._targetPeersInDuelDictionary.Add(missionPeer, false);
					}
				}
			}
			foreach (MissionDuelPeerMarkerVM missionDuelPeerMarkerVM2 in list)
			{
				this.Targets.Remove(missionDuelPeerMarkerVM2);
				this._targetPeersToMarkersDictionary.Remove(missionDuelPeerMarkerVM2.TargetPeer);
			}
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x00014660 File Offset: 0x00012860
		private void UpdateTargetsEnabled(bool isEnabled)
		{
			foreach (MissionDuelPeerMarkerVM missionDuelPeerMarkerVM in this.Targets)
			{
				missionDuelPeerMarkerVM.IsEnabled = !missionDuelPeerMarkerVM.IsInDuel && isEnabled;
			}
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x000146B8 File Offset: 0x000128B8
		private void OnDuelRequestSent(MissionPeer targetPeer)
		{
			foreach (MissionDuelPeerMarkerVM missionDuelPeerMarkerVM in this.Targets)
			{
				if (missionDuelPeerMarkerVM.TargetPeer == targetPeer)
				{
					missionDuelPeerMarkerVM.HasSentDuelRequest = true;
				}
			}
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x00014710 File Offset: 0x00012910
		private void OnDuelRequested(MissionPeer targetPeer, TroopType troopType)
		{
			MissionDuelPeerMarkerVM missionDuelPeerMarkerVM = this.Targets.FirstOrDefault((MissionDuelPeerMarkerVM t) => t.TargetPeer == targetPeer);
			if (missionDuelPeerMarkerVM != null)
			{
				missionDuelPeerMarkerVM.HasDuelRequestForPlayer = true;
				missionDuelPeerMarkerVM.PreferredArenaType = (int)troopType;
			}
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x00014753 File Offset: 0x00012953
		public void OnAgentSpawnedWithoutDuel()
		{
			this._hasEnteredLobby = true;
			this.IsEnabled = true;
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00014763 File Offset: 0x00012963
		public void OnAgentBuiltForTheFirstTime()
		{
			this._playerPreferredArenaType = (int)MultiplayerDuelVM.GetAgentDefaultPreferredArenaType(Agent.Main);
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00014778 File Offset: 0x00012978
		public void OnDuelStarted(MissionPeer firstPeer, MissionPeer secondPeer)
		{
			if (this._client.MyRepresentative.MissionPeer == firstPeer || this._client.MyRepresentative.MissionPeer == secondPeer)
			{
				this.IsEnabled = false;
			}
			foreach (MissionDuelPeerMarkerVM missionDuelPeerMarkerVM in this.Targets)
			{
				if (missionDuelPeerMarkerVM.TargetPeer == firstPeer || missionDuelPeerMarkerVM.TargetPeer == secondPeer)
				{
					missionDuelPeerMarkerVM.OnDuelStarted();
				}
			}
			this._targetPeersInDuelDictionary[firstPeer] = true;
			this._targetPeersInDuelDictionary[secondPeer] = true;
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x00014820 File Offset: 0x00012A20
		public void SetMarkerOfPeerEnabled(MissionPeer peer, bool isEnabled)
		{
			if (peer != null)
			{
				if (this._targetPeersToMarkersDictionary.ContainsKey(peer))
				{
					this._targetPeersToMarkersDictionary[peer].UpdateCurentDuelStatus(!isEnabled);
					this._targetPeersToMarkersDictionary[peer].UpdateBounty();
				}
				if (this._targetPeersInDuelDictionary.ContainsKey(peer))
				{
					this._targetPeersInDuelDictionary[peer] = !isEnabled;
				}
			}
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x00014882 File Offset: 0x00012A82
		public void OnPlayerPreferredZoneChanged(int playerPrefferedArenaType)
		{
			this._playerPreferredArenaType = playerPrefferedArenaType;
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0001488B File Offset: 0x00012A8B
		public void OnFocusGained()
		{
			this._isPlayerFocused = true;
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00014894 File Offset: 0x00012A94
		public void OnFocusLost()
		{
			this._isPlayerFocused = false;
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x0001489D File Offset: 0x00012A9D
		public void OnPeerEquipmentRefreshed(MissionPeer peer)
		{
			if (this._targetPeersToMarkersDictionary.ContainsKey(peer))
			{
				this._targetPeersToMarkersDictionary[peer].RefreshPerkSelection();
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06000471 RID: 1137 RVA: 0x000148BE File Offset: 0x00012ABE
		// (set) Token: 0x06000472 RID: 1138 RVA: 0x000148C6 File Offset: 0x00012AC6
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
					this.UpdateTargetsEnabled(value);
				}
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000473 RID: 1139 RVA: 0x000148EB File Offset: 0x00012AEB
		// (set) Token: 0x06000474 RID: 1140 RVA: 0x000148F3 File Offset: 0x00012AF3
		[DataSourceProperty]
		public MBBindingList<MissionDuelPeerMarkerVM> Targets
		{
			get
			{
				return this._targets;
			}
			set
			{
				if (value != this._targets)
				{
					this._targets = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionDuelPeerMarkerVM>>(value, "Targets");
				}
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000475 RID: 1141 RVA: 0x00014911 File Offset: 0x00012B11
		// (set) Token: 0x06000476 RID: 1142 RVA: 0x00014919 File Offset: 0x00012B19
		[DataSourceProperty]
		public MBBindingList<MissionDuelLandmarkMarkerVM> Landmarks
		{
			get
			{
				return this._landmarks;
			}
			set
			{
				if (value != this._landmarks)
				{
					this._landmarks = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionDuelLandmarkMarkerVM>>(value, "Landmarks");
				}
			}
		}

		// Token: 0x04000231 RID: 561
		private const string ZoneLandmarkTag = "duel_zone_landmark";

		// Token: 0x04000232 RID: 562
		private const float FocusScreenDistanceThreshold = 350f;

		// Token: 0x04000233 RID: 563
		private const float LandmarkFocusDistanceThrehsold = 500f;

		// Token: 0x04000234 RID: 564
		private bool _hasEnteredLobby;

		// Token: 0x04000235 RID: 565
		private Camera _missionCamera;

		// Token: 0x04000236 RID: 566
		private MissionDuelPeerMarkerVM _previousFocusTarget;

		// Token: 0x04000237 RID: 567
		private MissionDuelPeerMarkerVM _currentFocusTarget;

		// Token: 0x04000238 RID: 568
		private MissionDuelLandmarkMarkerVM _previousLandmarkTarget;

		// Token: 0x04000239 RID: 569
		private MissionDuelLandmarkMarkerVM _currentLandmarkTarget;

		// Token: 0x0400023A RID: 570
		private MissionDuelMarkersVM.PeerMarkerDistanceComparer _distanceComparer;

		// Token: 0x0400023B RID: 571
		private readonly Dictionary<MissionPeer, MissionDuelPeerMarkerVM> _targetPeersToMarkersDictionary;

		// Token: 0x0400023C RID: 572
		private readonly MissionMultiplayerGameModeDuelClient _client;

		// Token: 0x0400023D RID: 573
		private Vec2 _screenCenter;

		// Token: 0x0400023E RID: 574
		private Dictionary<MissionPeer, bool> _targetPeersInDuelDictionary;

		// Token: 0x0400023F RID: 575
		private int _playerPreferredArenaType;

		// Token: 0x04000240 RID: 576
		private bool _isPlayerFocused;

		// Token: 0x04000241 RID: 577
		private bool _isEnabled;

		// Token: 0x04000242 RID: 578
		private MBBindingList<MissionDuelPeerMarkerVM> _targets;

		// Token: 0x04000243 RID: 579
		private MBBindingList<MissionDuelLandmarkMarkerVM> _landmarks;

		// Token: 0x0200015D RID: 349
		private class PeerMarkerDistanceComparer : IComparer<MissionDuelPeerMarkerVM>
		{
			// Token: 0x06001920 RID: 6432 RVA: 0x0005110C File Offset: 0x0004F30C
			public int Compare(MissionDuelPeerMarkerVM x, MissionDuelPeerMarkerVM y)
			{
				return y.Distance.CompareTo(x.Distance);
			}
		}
	}
}
