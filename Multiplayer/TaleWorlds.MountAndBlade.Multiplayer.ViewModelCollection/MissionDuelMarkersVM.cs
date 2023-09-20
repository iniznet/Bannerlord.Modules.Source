using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection
{
	public class MissionDuelMarkersVM : ViewModel
	{
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

		public void UpdateScreenCenter()
		{
			this._screenCenter = new Vec2(Screen.RealScreenResolutionWidth / 2f, Screen.RealScreenResolutionHeight / 2f);
		}

		public void Tick(float dt)
		{
			if (this._hasEnteredLobby && GameNetwork.MyPeer != null)
			{
				this.OnRefreshPeerMarkers();
				this.UpdateTargets(dt);
			}
		}

		public void RegisterEvents()
		{
			DuelMissionRepresentative myRepresentative = this._client.MyRepresentative;
			myRepresentative.OnDuelRequestSentEvent = (Action<MissionPeer>)Delegate.Combine(myRepresentative.OnDuelRequestSentEvent, new Action<MissionPeer>(this.OnDuelRequestSent));
			DuelMissionRepresentative myRepresentative2 = this._client.MyRepresentative;
			myRepresentative2.OnDuelRequestedEvent = (Action<MissionPeer, TroopType>)Delegate.Combine(myRepresentative2.OnDuelRequestedEvent, new Action<MissionPeer, TroopType>(this.OnDuelRequested));
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionsChanged));
		}

		public void UnregisterEvents()
		{
			DuelMissionRepresentative myRepresentative = this._client.MyRepresentative;
			myRepresentative.OnDuelRequestSentEvent = (Action<MissionPeer>)Delegate.Remove(myRepresentative.OnDuelRequestSentEvent, new Action<MissionPeer>(this.OnDuelRequestSent));
			DuelMissionRepresentative myRepresentative2 = this._client.MyRepresentative;
			myRepresentative2.OnDuelRequestedEvent = (Action<MissionPeer, TroopType>)Delegate.Remove(myRepresentative2.OnDuelRequestedEvent, new Action<MissionPeer, TroopType>(this.OnDuelRequested));
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionsChanged));
		}

		private void OnManagedOptionsChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 32)
			{
				this.Targets.ApplyActionOnAllItems(delegate(MissionDuelPeerMarkerVM t)
				{
					t.RefreshValues();
				});
			}
		}

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

		public void RefreshPeerEquipments()
		{
			foreach (MissionPeer missionPeer in VirtualPlayer.Peers<MissionPeer>())
			{
				this.OnPeerEquipmentRefreshed(missionPeer);
			}
		}

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

		private void UpdateTargetsEnabled(bool isEnabled)
		{
			foreach (MissionDuelPeerMarkerVM missionDuelPeerMarkerVM in this.Targets)
			{
				missionDuelPeerMarkerVM.IsEnabled = !missionDuelPeerMarkerVM.IsInDuel && isEnabled;
			}
		}

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

		private void OnDuelRequested(MissionPeer targetPeer, TroopType troopType)
		{
			MissionDuelPeerMarkerVM missionDuelPeerMarkerVM = this.Targets.FirstOrDefault((MissionDuelPeerMarkerVM t) => t.TargetPeer == targetPeer);
			if (missionDuelPeerMarkerVM != null)
			{
				missionDuelPeerMarkerVM.HasDuelRequestForPlayer = true;
				missionDuelPeerMarkerVM.PreferredArenaType = troopType;
			}
		}

		public void OnAgentSpawnedWithoutDuel()
		{
			this._hasEnteredLobby = true;
			this.IsEnabled = true;
		}

		public void OnAgentBuiltForTheFirstTime()
		{
			this._playerPreferredArenaType = MultiplayerDuelVM.GetAgentDefaultPreferredArenaType(Agent.Main);
		}

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

		public void OnPlayerPreferredZoneChanged(int playerPrefferedArenaType)
		{
			this._playerPreferredArenaType = playerPrefferedArenaType;
		}

		public void OnFocusGained()
		{
			this._isPlayerFocused = true;
		}

		public void OnFocusLost()
		{
			this._isPlayerFocused = false;
		}

		public void OnPeerEquipmentRefreshed(MissionPeer peer)
		{
			MissionDuelPeerMarkerVM missionDuelPeerMarkerVM;
			if (this._targetPeersToMarkersDictionary.TryGetValue(peer, out missionDuelPeerMarkerVM))
			{
				missionDuelPeerMarkerVM.RefreshPerkSelection();
			}
		}

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

		private const string ZoneLandmarkTag = "duel_zone_landmark";

		private const float FocusScreenDistanceThreshold = 350f;

		private const float LandmarkFocusDistanceThrehsold = 500f;

		private bool _hasEnteredLobby;

		private Camera _missionCamera;

		private MissionDuelPeerMarkerVM _previousFocusTarget;

		private MissionDuelPeerMarkerVM _currentFocusTarget;

		private MissionDuelLandmarkMarkerVM _previousLandmarkTarget;

		private MissionDuelLandmarkMarkerVM _currentLandmarkTarget;

		private MissionDuelMarkersVM.PeerMarkerDistanceComparer _distanceComparer;

		private readonly Dictionary<MissionPeer, MissionDuelPeerMarkerVM> _targetPeersToMarkersDictionary;

		private readonly MissionMultiplayerGameModeDuelClient _client;

		private Vec2 _screenCenter;

		private Dictionary<MissionPeer, bool> _targetPeersInDuelDictionary;

		private int _playerPreferredArenaType;

		private bool _isPlayerFocused;

		private bool _isEnabled;

		private MBBindingList<MissionDuelPeerMarkerVM> _targets;

		private MBBindingList<MissionDuelLandmarkMarkerVM> _landmarks;

		private class PeerMarkerDistanceComparer : IComparer<MissionDuelPeerMarkerVM>
		{
			public int Compare(MissionDuelPeerMarkerVM x, MissionDuelPeerMarkerVM y)
			{
				return y.Distance.CompareTo(x.Distance);
			}
		}
	}
}
