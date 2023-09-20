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
	// Token: 0x020000BC RID: 188
	public class MultiplayerMissionMarkerVM : ViewModel
	{
		// Token: 0x06001214 RID: 4628 RVA: 0x0003B694 File Offset: 0x00039894
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

		// Token: 0x06001215 RID: 4629 RVA: 0x0003B7B8 File Offset: 0x000399B8
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

		// Token: 0x06001216 RID: 4630 RVA: 0x0003B834 File Offset: 0x00039A34
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

		// Token: 0x06001217 RID: 4631 RVA: 0x0003B8C8 File Offset: 0x00039AC8
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

		// Token: 0x06001218 RID: 4632 RVA: 0x0003B950 File Offset: 0x00039B50
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

		// Token: 0x06001219 RID: 4633 RVA: 0x0003B9E0 File Offset: 0x00039BE0
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

		// Token: 0x0600121A RID: 4634 RVA: 0x0003BA68 File Offset: 0x00039C68
		private void UpdateAlwaysVisibleTargetScreenPosition()
		{
			foreach (MissionAlwaysVisibleMarkerTargetVM missionAlwaysVisibleMarkerTargetVM in this.AlwaysVisibleTargets)
			{
				missionAlwaysVisibleMarkerTargetVM.UpdateScreenPosition(this._missionCamera);
			}
		}

		// Token: 0x0600121B RID: 4635 RVA: 0x0003BAB8 File Offset: 0x00039CB8
		private void OnFlagNumberChangedEvent()
		{
			this.ResetCapturePointLists();
			this.InitCapturePoints();
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x0003BAC8 File Offset: 0x00039CC8
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

		// Token: 0x0600121D RID: 4637 RVA: 0x0003BB49 File Offset: 0x00039D49
		private void ResetCapturePointLists()
		{
			this.FlagTargets.Clear();
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x0003BB58 File Offset: 0x00039D58
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

		// Token: 0x0600121F RID: 4639 RVA: 0x0003BBB0 File Offset: 0x00039DB0
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

		// Token: 0x06001220 RID: 4640 RVA: 0x0003BE3C File Offset: 0x0003A03C
		public void OnRemoveAlwaysVisibleMarker(MissionAlwaysVisibleMarkerTargetVM marker)
		{
			this.AlwaysVisibleTargets.Remove(marker);
		}

		// Token: 0x06001221 RID: 4641 RVA: 0x0003BE4C File Offset: 0x0003A04C
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

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06001222 RID: 4642 RVA: 0x0003BEAB File Offset: 0x0003A0AB
		// (set) Token: 0x06001223 RID: 4643 RVA: 0x0003BEB3 File Offset: 0x0003A0B3
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

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06001224 RID: 4644 RVA: 0x0003BED1 File Offset: 0x0003A0D1
		// (set) Token: 0x06001225 RID: 4645 RVA: 0x0003BED9 File Offset: 0x0003A0D9
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

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x06001226 RID: 4646 RVA: 0x0003BEF7 File Offset: 0x0003A0F7
		// (set) Token: 0x06001227 RID: 4647 RVA: 0x0003BEFF File Offset: 0x0003A0FF
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

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x06001228 RID: 4648 RVA: 0x0003BF1D File Offset: 0x0003A11D
		// (set) Token: 0x06001229 RID: 4649 RVA: 0x0003BF25 File Offset: 0x0003A125
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

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x0600122A RID: 4650 RVA: 0x0003BF43 File Offset: 0x0003A143
		// (set) Token: 0x0600122B RID: 4651 RVA: 0x0003BF4B File Offset: 0x0003A14B
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

		// Token: 0x040008A1 RID: 2209
		private readonly Camera _missionCamera;

		// Token: 0x040008A2 RID: 2210
		private bool _prevEnabledState;

		// Token: 0x040008A3 RID: 2211
		private bool _fadeOutTimerStarted;

		// Token: 0x040008A4 RID: 2212
		private float _fadeOutTimer;

		// Token: 0x040008A5 RID: 2213
		private MultiplayerMissionMarkerVM.MarkerDistanceComparer _distanceComparer;

		// Token: 0x040008A6 RID: 2214
		private readonly ICommanderInfo _commanderInfo;

		// Token: 0x040008A7 RID: 2215
		private readonly Dictionary<MissionPeer, MissionPeerMarkerTargetVM> _teammateDictionary;

		// Token: 0x040008A8 RID: 2216
		private readonly MissionMultiplayerSiegeClient _siegeClient;

		// Token: 0x040008A9 RID: 2217
		private readonly List<PlayerId> _friendIDs;

		// Token: 0x040008AA RID: 2218
		private MBBindingList<MissionFlagMarkerTargetVM> _flagTargets;

		// Token: 0x040008AB RID: 2219
		private MBBindingList<MissionPeerMarkerTargetVM> _peerTargets;

		// Token: 0x040008AC RID: 2220
		private MBBindingList<MissionSiegeEngineMarkerTargetVM> _siegeEngineTargets;

		// Token: 0x040008AD RID: 2221
		private MBBindingList<MissionAlwaysVisibleMarkerTargetVM> _alwaysVisibleTargets;

		// Token: 0x040008AE RID: 2222
		private bool _isEnabled;

		// Token: 0x02000218 RID: 536
		public class MarkerDistanceComparer : IComparer<MissionMarkerTargetVM>
		{
			// Token: 0x06001AEE RID: 6894 RVA: 0x000572A4 File Offset: 0x000554A4
			public int Compare(MissionMarkerTargetVM x, MissionMarkerTargetVM y)
			{
				return y.Distance.CompareTo(x.Distance);
			}
		}
	}
}
