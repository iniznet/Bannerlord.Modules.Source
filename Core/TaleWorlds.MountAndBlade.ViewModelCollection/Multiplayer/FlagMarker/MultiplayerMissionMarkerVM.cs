using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FlagMarker.Targets;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FlagMarker
{
	public class MultiplayerMissionMarkerVM : ViewModel
	{
		public MultiplayerMissionMarkerVM(Camera missionCamera)
		{
			this._missionCamera = missionCamera;
			this.FlagTargets = new MBBindingList<MissionFlagMarkerTargetVM>();
			this.PeerTargets = new MBBindingList<MissionPeerMarkerTargetVM>();
			this.SiegeEngineTargets = new MBBindingList<MissionSiegeEngineMarkerTargetVM>();
			this.AlwaysVisibleTargets = new MBBindingList<MissionAlwaysVisibleMarkerTargetVM>();
			this._teammateDictionary = new Dictionary<MissionPeer, MissionPeerMarkerTargetVM>();
			this._distanceComparer = new MultiplayerMissionMarkerVM.MarkerDistanceComparer();
			this._commanderInfo = Mission.Current.GetMissionBehavior<ICommanderInfo>();
			if (this._commanderInfo != null)
			{
				this._commanderInfo.OnFlagNumberChangedEvent += this.OnFlagNumberChangedEvent;
				this._commanderInfo.OnCapturePointOwnerChangedEvent += this.OnCapturePointOwnerChangedEvent;
				this.OnFlagNumberChangedEvent();
				this._siegeClient = Mission.Current.GetMissionBehavior<MissionMultiplayerSiegeClient>();
				if (this._siegeClient != null)
				{
					this._siegeClient.OnCapturePointRemainingMoraleGainsChangedEvent += this.OnCapturePointRemainingMoraleGainsChanged;
				}
			}
			MissionPeer.OnTeamChanged += this.OnTeamChanged;
			this._friendIDs = new List<PlayerId>();
			foreach (IFriendListService friendListService in PlatformServices.Instance.GetFriendListServices())
			{
				this._friendIDs.AddRange(friendListService.GetAllFriends());
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			if (this._commanderInfo != null)
			{
				this._commanderInfo.OnFlagNumberChangedEvent -= this.OnFlagNumberChangedEvent;
				this._commanderInfo.OnCapturePointOwnerChangedEvent -= this.OnCapturePointOwnerChangedEvent;
				if (this._siegeClient != null)
				{
					this._siegeClient.OnCapturePointRemainingMoraleGainsChangedEvent -= this.OnCapturePointRemainingMoraleGainsChanged;
				}
			}
			MissionPeer.OnTeamChanged -= this.OnTeamChanged;
		}

		public void Tick(float dt)
		{
			this.OnRefreshPeerMarkers();
			this.UpdateAlwaysVisibleTargetScreenPosition();
			if (this.IsEnabled)
			{
				this.UpdateTargetScreenPositions();
				this._fadeOutTimerStarted = false;
				this._fadeOutTimer = 0f;
				this._prevEnabledState = this.IsEnabled;
			}
			else
			{
				if (this._prevEnabledState)
				{
					this._fadeOutTimerStarted = true;
				}
				if (this._fadeOutTimerStarted)
				{
					this._fadeOutTimer += dt;
				}
				if (this._fadeOutTimer < 2f)
				{
					this.UpdateTargetScreenPositions();
				}
				else
				{
					this._fadeOutTimerStarted = false;
				}
			}
			this._prevEnabledState = this.IsEnabled;
		}

		private void OnCapturePointRemainingMoraleGainsChanged(int[] remainingMoraleGainsArr)
		{
			foreach (MissionFlagMarkerTargetVM missionFlagMarkerTargetVM in this.FlagTargets)
			{
				int flagIndex = missionFlagMarkerTargetVM.TargetFlag.FlagIndex;
				if (flagIndex >= 0 && flagIndex < remainingMoraleGainsArr.Length)
				{
					missionFlagMarkerTargetVM.OnRemainingMoraleChanged(remainingMoraleGainsArr[flagIndex]);
				}
			}
			Debug.Print("OnCapturePointRemainingMoraleGainsChanged: " + remainingMoraleGainsArr.Length, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		private void OnTeamChanged(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (this._commanderInfo != null)
			{
				this.OnFlagNumberChangedEvent();
			}
			if (peer.IsMine)
			{
				this.SiegeEngineTargets.Clear();
				foreach (GameEntity gameEntity in Mission.Current.GetActiveEntitiesWithScriptComponentOfType<SiegeWeapon>())
				{
					SiegeWeapon firstScriptOfType = gameEntity.GetFirstScriptOfType<SiegeWeapon>();
					if (newTeam.Side == firstScriptOfType.Side)
					{
						this.SiegeEngineTargets.Add(new MissionSiegeEngineMarkerTargetVM(firstScriptOfType));
					}
				}
			}
		}

		private void UpdateTargetScreenPositions()
		{
			this.PeerTargets.ApplyActionOnAllItems(delegate(MissionPeerMarkerTargetVM pt)
			{
				pt.UpdateScreenPosition(this._missionCamera);
			});
			this.FlagTargets.ApplyActionOnAllItems(delegate(MissionFlagMarkerTargetVM ft)
			{
				ft.UpdateScreenPosition(this._missionCamera);
			});
			this.SiegeEngineTargets.ApplyActionOnAllItems(delegate(MissionSiegeEngineMarkerTargetVM st)
			{
				st.UpdateScreenPosition(this._missionCamera);
			});
			this.PeerTargets.Sort(this._distanceComparer);
			this.FlagTargets.Sort(this._distanceComparer);
			this.SiegeEngineTargets.Sort(this._distanceComparer);
		}

		private void UpdateAlwaysVisibleTargetScreenPosition()
		{
			foreach (MissionAlwaysVisibleMarkerTargetVM missionAlwaysVisibleMarkerTargetVM in this.AlwaysVisibleTargets)
			{
				missionAlwaysVisibleMarkerTargetVM.UpdateScreenPosition(this._missionCamera);
			}
		}

		private void OnFlagNumberChangedEvent()
		{
			this.ResetCapturePointLists();
			this.InitCapturePoints();
		}

		private void InitCapturePoints()
		{
			if (this._commanderInfo != null)
			{
				foreach (FlagCapturePoint flagCapturePoint in this._commanderInfo.AllCapturePoints.Where((FlagCapturePoint c) => !c.IsDeactivated).ToArray<FlagCapturePoint>())
				{
					MissionFlagMarkerTargetVM missionFlagMarkerTargetVM = new MissionFlagMarkerTargetVM(flagCapturePoint);
					this.FlagTargets.Add(missionFlagMarkerTargetVM);
					missionFlagMarkerTargetVM.OnOwnerChanged(this._commanderInfo.GetFlagOwner(flagCapturePoint));
				}
			}
		}

		private void ResetCapturePointLists()
		{
			this.FlagTargets.Clear();
		}

		private void OnCapturePointOwnerChangedEvent(FlagCapturePoint flag, Team team)
		{
			foreach (MissionFlagMarkerTargetVM missionFlagMarkerTargetVM in this.FlagTargets)
			{
				if (missionFlagMarkerTargetVM.TargetFlag == flag)
				{
					missionFlagMarkerTargetVM.OnOwnerChanged(team);
				}
			}
		}

		private void OnRefreshPeerMarkers()
		{
			if (GameNetwork.MyPeer == null)
			{
				return;
			}
			Agent controlledAgent = GameNetwork.MyPeer.ControlledAgent;
			BattleSideEnum battleSideEnum = ((controlledAgent != null) ? controlledAgent.Team.Side : BattleSideEnum.None);
			List<MissionPeerMarkerTargetVM> list = this.PeerTargets.ToList<MissionPeerMarkerTargetVM>();
			using (List<MissionPeer>.Enumerator enumerator = VirtualPlayer.Peers<MissionPeer>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MissionPeer missionPeer = enumerator.Current;
					MissionPeer missionPeer2 = missionPeer;
					if (((missionPeer2 != null) ? missionPeer2.Team : null) != null && !missionPeer.IsMine && missionPeer.Team.Side == battleSideEnum)
					{
						IEnumerable<MissionPeerMarkerTargetVM> enumerable = this.PeerTargets.Where(delegate(MissionPeerMarkerTargetVM t)
						{
							MissionPeer targetPeer2 = t.TargetPeer;
							return targetPeer2 != null && targetPeer2.Peer.Id.Equals(missionPeer.Peer.Id);
						});
						if (enumerable.Any<MissionPeerMarkerTargetVM>())
						{
							MissionPeerMarkerTargetVM currentMarker = enumerable.First<MissionPeerMarkerTargetVM>();
							IEnumerable<MissionAlwaysVisibleMarkerTargetVM> enumerable2 = this.AlwaysVisibleTargets.Where((MissionAlwaysVisibleMarkerTargetVM t) => t.TargetPeer.Peer.Id.Equals(currentMarker.TargetPeer.Peer.Id));
							if (BannerlordConfig.EnableDeathIcon && !missionPeer.IsControlledAgentActive)
							{
								if (enumerable2.Any<MissionAlwaysVisibleMarkerTargetVM>())
								{
									continue;
								}
								MissionPeer targetPeer = enumerable.First<MissionPeerMarkerTargetVM>().TargetPeer;
								if (((targetPeer != null) ? targetPeer.ControlledAgent : null) != null)
								{
									MissionAlwaysVisibleMarkerTargetVM missionAlwaysVisibleMarkerTargetVM = new MissionAlwaysVisibleMarkerTargetVM(currentMarker.TargetPeer, enumerable.First<MissionPeerMarkerTargetVM>().WorldPosition, new Action<MissionAlwaysVisibleMarkerTargetVM>(this.OnRemoveAlwaysVisibleMarker));
									missionAlwaysVisibleMarkerTargetVM.UpdateScreenPosition(this._missionCamera);
									this.AlwaysVisibleTargets.Add(missionAlwaysVisibleMarkerTargetVM);
									continue;
								}
								continue;
							}
						}
						if (!this._teammateDictionary.ContainsKey(missionPeer))
						{
							MissionPeerMarkerTargetVM missionPeerMarkerTargetVM = new MissionPeerMarkerTargetVM(missionPeer, this._friendIDs.Contains(missionPeer.Peer.Id));
							this.PeerTargets.Add(missionPeerMarkerTargetVM);
							this._teammateDictionary.Add(missionPeer, missionPeerMarkerTargetVM);
						}
						else
						{
							list.Remove(this._teammateDictionary[missionPeer]);
						}
					}
				}
			}
			using (List<MissionPeerMarkerTargetVM>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					MissionPeerMarkerTargetVM missionPeerMarkerTargetVM2;
					if ((missionPeerMarkerTargetVM2 = enumerator2.Current) != null)
					{
						this.PeerTargets.Remove(missionPeerMarkerTargetVM2);
						this._teammateDictionary.Remove(missionPeerMarkerTargetVM2.TargetPeer);
					}
				}
			}
		}

		public void OnRemoveAlwaysVisibleMarker(MissionAlwaysVisibleMarkerTargetVM marker)
		{
			this.AlwaysVisibleTargets.Remove(marker);
		}

		private void UpdateTargetStates(bool state)
		{
			this.PeerTargets.ApplyActionOnAllItems(delegate(MissionPeerMarkerTargetVM pt)
			{
				pt.IsEnabled = state;
			});
			this.FlagTargets.ApplyActionOnAllItems(delegate(MissionFlagMarkerTargetVM ft)
			{
				ft.IsEnabled = state;
			});
			this.SiegeEngineTargets.ApplyActionOnAllItems(delegate(MissionSiegeEngineMarkerTargetVM st)
			{
				st.IsEnabled = state;
			});
		}

		[DataSourceProperty]
		public MBBindingList<MissionFlagMarkerTargetVM> FlagTargets
		{
			get
			{
				return this._flagTargets;
			}
			set
			{
				if (value != this._flagTargets)
				{
					this._flagTargets = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionFlagMarkerTargetVM>>(value, "FlagTargets");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MissionPeerMarkerTargetVM> PeerTargets
		{
			get
			{
				return this._peerTargets;
			}
			set
			{
				if (value != this._peerTargets)
				{
					this._peerTargets = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionPeerMarkerTargetVM>>(value, "PeerTargets");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MissionSiegeEngineMarkerTargetVM> SiegeEngineTargets
		{
			get
			{
				return this._siegeEngineTargets;
			}
			set
			{
				if (value != this._siegeEngineTargets)
				{
					this._siegeEngineTargets = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionSiegeEngineMarkerTargetVM>>(value, "SiegeEngineTargets");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MissionAlwaysVisibleMarkerTargetVM> AlwaysVisibleTargets
		{
			get
			{
				return this._alwaysVisibleTargets;
			}
			set
			{
				if (value != this._alwaysVisibleTargets)
				{
					this._alwaysVisibleTargets = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionAlwaysVisibleMarkerTargetVM>>(value, "AlwaysVisibleTargets");
				}
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
					this.UpdateTargetStates(value);
				}
			}
		}

		private readonly Camera _missionCamera;

		private bool _prevEnabledState;

		private bool _fadeOutTimerStarted;

		private float _fadeOutTimer;

		private MultiplayerMissionMarkerVM.MarkerDistanceComparer _distanceComparer;

		private readonly ICommanderInfo _commanderInfo;

		private readonly Dictionary<MissionPeer, MissionPeerMarkerTargetVM> _teammateDictionary;

		private readonly MissionMultiplayerSiegeClient _siegeClient;

		private readonly List<PlayerId> _friendIDs;

		private MBBindingList<MissionFlagMarkerTargetVM> _flagTargets;

		private MBBindingList<MissionPeerMarkerTargetVM> _peerTargets;

		private MBBindingList<MissionSiegeEngineMarkerTargetVM> _siegeEngineTargets;

		private MBBindingList<MissionAlwaysVisibleMarkerTargetVM> _alwaysVisibleTargets;

		private bool _isEnabled;

		public class MarkerDistanceComparer : IComparer<MissionMarkerTargetVM>
		{
			public int Compare(MissionMarkerTargetVM x, MissionMarkerTargetVM y)
			{
				return y.Distance.CompareTo(x.Distance);
			}
		}
	}
}
