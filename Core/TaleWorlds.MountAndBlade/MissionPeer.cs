using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public class MissionPeer : PeerComponent
	{
		public static event MissionPeer.OnUpdateEquipmentSetIndexEventDelegate OnEquipmentIndexRefreshed;

		public static event MissionPeer.OnPerkUpdateEventDelegate OnPerkSelectionUpdated;

		public static event MissionPeer.OnTeamChangedDelegate OnPreTeamChanged;

		public static event MissionPeer.OnTeamChangedDelegate OnTeamChanged;

		private event MissionPeer.OnCultureChangedDelegate OnCultureChanged;

		public static event MissionPeer.OnPlayerKilledDelegate OnPlayerKilled;

		public DateTime JoinTime { get; internal set; }

		public bool EquipmentUpdatingExpired { get; set; }

		public bool TeamInitialPerkInfoReady { get; private set; }

		public bool HasSpawnedAgentVisuals { get; set; }

		public int SelectedTroopIndex
		{
			get
			{
				return this._selectedTroopIndex;
			}
			set
			{
				if (this._selectedTroopIndex != value)
				{
					this._selectedTroopIndex = value;
					this.ResetSelectedPerks();
					MissionPeer.OnUpdateEquipmentSetIndexEventDelegate onEquipmentIndexRefreshed = MissionPeer.OnEquipmentIndexRefreshed;
					if (onEquipmentIndexRefreshed == null)
					{
						return;
					}
					onEquipmentIndexRefreshed(this, value);
				}
			}
		}

		public int NextSelectedTroopIndex { get; set; }

		public MissionRepresentativeBase Representative
		{
			get
			{
				if (this._representative == null)
				{
					this._representative = base.Peer.GetComponent<MissionRepresentativeBase>();
				}
				return this._representative;
			}
		}

		public MBReadOnlyList<int[]> Perks
		{
			get
			{
				return this._perks;
			}
		}

		public string DisplayedName
		{
			get
			{
				if (GameNetwork.IsDedicatedServer)
				{
					return base.Name;
				}
				if (NetworkMain.CommunityClient.IsInGame)
				{
					return base.Name;
				}
				if (NetworkMain.GameClient.HasUserGeneratedContentPrivilege && (NetworkMain.GameClient.IsKnownPlayer(base.Peer.Id) || !BannerlordConfig.EnableGenericNames))
				{
					VirtualPlayer peer = base.Peer;
					return ((peer != null) ? peer.UserName : null) ?? "";
				}
				if (this.Culture == null || MultiplayerClassDivisions.GetMPHeroClassForPeer(this, false) == null)
				{
					return new TextObject("{=RN6zHak0}Player", null).ToString();
				}
				return MultiplayerClassDivisions.GetMPHeroClassForPeer(this, false).TroopName.ToString();
			}
		}

		public MBReadOnlyList<MPPerkObject> SelectedPerks
		{
			get
			{
				if (this.SelectedTroopIndex < 0 || this.Team == null || this.Team.Side == BattleSideEnum.None)
				{
					return new MBList<MPPerkObject>();
				}
				if ((this._selectedPerks.Item2 == null || this.SelectedTroopIndex != this._selectedPerks.Item1 || this._selectedPerks.Item2.Count < 3) && !this.RefreshSelectedPerks())
				{
					return new MBReadOnlyList<MPPerkObject>();
				}
				return this._selectedPerks.Item2;
			}
		}

		public MissionPeer()
		{
			this.SpawnTimer = new Timer(Mission.Current.CurrentTime, 3f, false);
			this._selectedPerks = new ValueTuple<int, MBList<MPPerkObject>>(0, null);
			this._perks = new MBList<int[]>();
			for (int i = 0; i < 16; i++)
			{
				int[] array = new int[3];
				this._perks.Add(array);
			}
		}

		public Timer SpawnTimer { get; internal set; }

		public bool HasSpawnTimerExpired { get; set; }

		public BasicCultureObject VotedForBan { get; private set; }

		public BasicCultureObject VotedForSelection { get; private set; }

		public bool WantsToSpawnAsBot { get; set; }

		public int SpawnCountThisRound { get; set; }

		public int RequestedKickPollCount { get; private set; }

		public int KillCount
		{
			get
			{
				return this._killCount;
			}
			internal set
			{
				this._killCount = MBMath.ClampInt(value, -1000, 100000);
			}
		}

		public int AssistCount
		{
			get
			{
				return this._assistCount;
			}
			internal set
			{
				this._assistCount = MBMath.ClampInt(value, -1000, 100000);
			}
		}

		public int DeathCount
		{
			get
			{
				return this._deathCount;
			}
			internal set
			{
				this._deathCount = MBMath.ClampInt(value, -1000, 100000);
			}
		}

		public int Score
		{
			get
			{
				return this._score;
			}
			internal set
			{
				this._score = MBMath.ClampInt(value, -1000000, 1000000);
			}
		}

		public int BotsUnderControlAlive
		{
			get
			{
				return this._botsUnderControlAlive;
			}
			set
			{
				if (this._botsUnderControlAlive != value)
				{
					this._botsUnderControlAlive = value;
					MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(this);
					if (perkHandler == null)
					{
						return;
					}
					perkHandler.OnEvent(MPPerkCondition.PerkEventFlags.AliveBotCountChange);
				}
			}
		}

		public int BotsUnderControlTotal { get; internal set; }

		public bool IsControlledAgentActive
		{
			get
			{
				return this.ControlledAgent != null && this.ControlledAgent.IsActive();
			}
		}

		public Agent ControlledAgent
		{
			get
			{
				return this.GetNetworkPeer().ControlledAgent;
			}
			set
			{
				NetworkCommunicator networkPeer = this.GetNetworkPeer();
				if (networkPeer.ControlledAgent != value)
				{
					this.ResetSelectedPerks();
					Agent controlledAgent = networkPeer.ControlledAgent;
					networkPeer.ControlledAgent = value;
					if (controlledAgent != null && controlledAgent.MissionPeer == this && controlledAgent.IsActive())
					{
						controlledAgent.MissionPeer = null;
					}
					if (networkPeer.ControlledAgent != null && networkPeer.ControlledAgent.MissionPeer != this)
					{
						networkPeer.ControlledAgent.MissionPeer = this;
					}
					MissionRepresentativeBase component = networkPeer.VirtualPlayer.GetComponent<MissionRepresentativeBase>();
					if (component != null)
					{
						component.SetAgent(value);
					}
					if (value != null)
					{
						MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(this);
						if (perkHandler == null)
						{
							return;
						}
						perkHandler.OnEvent(value, MPPerkCondition.PerkEventFlags.PeerControlledAgentChange);
					}
				}
			}
		}

		public Agent FollowedAgent
		{
			get
			{
				return this._followedAgent;
			}
			set
			{
				if (this._followedAgent != value)
				{
					this._followedAgent = value;
					if (GameNetwork.IsClient)
					{
						GameNetwork.BeginModuleEventAsClient();
						Agent followedAgent = this._followedAgent;
						GameNetwork.WriteMessage(new SetFollowedAgent((followedAgent != null) ? followedAgent.Index : (-1)));
						GameNetwork.EndModuleEventAsClient();
					}
				}
			}
		}

		public Team Team
		{
			get
			{
				return this._team;
			}
			set
			{
				if (this._team != value)
				{
					if (MissionPeer.OnPreTeamChanged != null)
					{
						MissionPeer.OnPreTeamChanged(this.GetNetworkPeer(), this._team, value);
					}
					Team team = this._team;
					this._team = value;
					string text = "Set the team to: ";
					Team team2 = this._team;
					Debug.Print(text + (((team2 != null) ? team2.Side.ToString() : null) ?? "null") + ", for peer: " + base.Name, 0, Debug.DebugColor.White, 17592186044416UL);
					this._controlledFormation = null;
					if (this._team != null)
					{
						if (GameNetwork.IsServer)
						{
							MBAPI.IMBPeer.SetTeam(base.Peer.Index, this._team.MBTeam.Index);
							GameNetwork.BeginBroadcastModuleEvent();
							GameNetwork.WriteMessage(new SetPeerTeam(this.GetNetworkPeer(), this._team.TeamIndex));
							GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
						}
						if (MissionPeer.OnTeamChanged != null)
						{
							MissionPeer.OnTeamChanged(this.GetNetworkPeer(), team, this._team);
							return;
						}
					}
					else if (GameNetwork.IsServer)
					{
						MBAPI.IMBPeer.SetTeam(base.Peer.Index, -1);
					}
				}
			}
		}

		public BasicCultureObject Culture
		{
			get
			{
				return this._culture;
			}
			set
			{
				BasicCultureObject culture = this._culture;
				this._culture = value;
				if (GameNetwork.IsServerOrRecorder)
				{
					this.TeamInitialPerkInfoReady = base.Peer.IsMine;
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new ChangeCulture(this, this._culture));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				if (this.OnCultureChanged != null)
				{
					this.OnCultureChanged(this._culture);
				}
			}
		}

		public Formation ControlledFormation
		{
			get
			{
				return this._controlledFormation;
			}
			set
			{
				if (this._controlledFormation != value)
				{
					this._controlledFormation = value;
				}
			}
		}

		public bool IsAgentAliveForChatting
		{
			get
			{
				MissionPeer component = base.GetComponent<MissionPeer>();
				return component != null && (this.IsControlledAgentActive || component.HasSpawnedAgentVisuals);
			}
		}

		public bool IsMutedFromPlatform { get; private set; }

		public bool IsMuted { get; private set; }

		public bool IsMutedFromGameOrPlatform
		{
			get
			{
				return this.IsMutedFromPlatform || this.IsMuted;
			}
		}

		public void SetMutedFromPlatform(bool isMuted)
		{
			this.IsMutedFromPlatform = isMuted;
		}

		public void SetMuted(bool isMuted)
		{
			this.IsMuted = isMuted;
		}

		public void ResetRequestedKickPollCount()
		{
			this.RequestedKickPollCount = 0;
		}

		public void IncrementRequestedKickPollCount()
		{
			int requestedKickPollCount = this.RequestedKickPollCount;
			this.RequestedKickPollCount = requestedKickPollCount + 1;
		}

		public int GetSelectedPerkIndexWithPerkListIndex(int troopIndex, int perkListIndex)
		{
			return this._perks[troopIndex][perkListIndex];
		}

		public bool SelectPerk(int perkListIndex, int perkIndex, int enforcedSelectedTroopIndex = -1)
		{
			if (this.SelectedTroopIndex >= 0 && enforcedSelectedTroopIndex >= 0 && this.SelectedTroopIndex != enforcedSelectedTroopIndex)
			{
				Debug.Print("SelectedTroopIndex < 0 || enforcedSelectedTroopIndex < 0 || SelectedTroopIndex == enforcedSelectedTroopIndex", 0, Debug.DebugColor.White, 17179869184UL);
				Debug.Print(string.Format("SelectedTroopIndex: {0} enforcedSelectedTroopIndex: {1}", this.SelectedTroopIndex, enforcedSelectedTroopIndex), 0, Debug.DebugColor.White, 17179869184UL);
			}
			int num = ((enforcedSelectedTroopIndex >= 0) ? enforcedSelectedTroopIndex : this.SelectedTroopIndex);
			if (perkIndex != this._perks[num][perkListIndex])
			{
				this._perks[num][perkListIndex] = perkIndex;
				if (this.GetNetworkPeer().IsMine)
				{
					List<MultiplayerClassDivisions.MPHeroClass> list = MultiplayerClassDivisions.GetMPHeroClasses(this.Culture).ToList<MultiplayerClassDivisions.MPHeroClass>();
					int count = list.Count;
					for (int i = 0; i < count; i++)
					{
						if (num == i)
						{
							MultiplayerClassDivisions.MPHeroClass mpheroClass = list[i];
							List<MPPerkSelectionManager.MPPerkSelection> list2 = new List<MPPerkSelectionManager.MPPerkSelection>();
							for (int j = 0; j < 3; j++)
							{
								list2.Add(new MPPerkSelectionManager.MPPerkSelection(this._perks[i][j], j));
							}
							MPPerkSelectionManager.Instance.SetSelectionsForHeroClassTemporarily(mpheroClass, list2);
							break;
						}
					}
				}
				if (num == this.SelectedTroopIndex)
				{
					this.ResetSelectedPerks();
				}
				MissionPeer.OnPerkUpdateEventDelegate onPerkSelectionUpdated = MissionPeer.OnPerkSelectionUpdated;
				if (onPerkSelectionUpdated != null)
				{
					onPerkSelectionUpdated(this);
				}
				return true;
			}
			return false;
		}

		public void HandleVoteChange(CultureVoteTypes voteType, BasicCultureObject culture)
		{
			if (voteType != CultureVoteTypes.Ban)
			{
				if (voteType == CultureVoteTypes.Select)
				{
					this.VotedForSelection = culture;
				}
			}
			else
			{
				this.VotedForBan = culture;
			}
			if (GameNetwork.IsServer)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new CultureVoteServer(this.GetNetworkPeer(), voteType, (voteType == CultureVoteTypes.Ban) ? this.VotedForBan : this.VotedForSelection));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			if (base.IsMine)
			{
				MPPerkSelectionManager.Instance.TryToApplyAndSavePendingChanges();
			}
			this.ResetKillRegistry();
			if (this.HasSpawnedAgentVisuals && Mission.Current != null)
			{
				MultiplayerMissionAgentVisualSpawnComponent missionBehavior = Mission.Current.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
				if (missionBehavior != null)
				{
					missionBehavior.RemoveAgentVisuals(this, false);
				}
				this.HasSpawnedAgentVisuals = false;
				this.OnCultureChanged -= this.CultureChanged;
			}
		}

		public override void OnInitialize()
		{
			base.OnInitialize();
			this.OnCultureChanged += this.CultureChanged;
		}

		public int GetAmountOfAgentVisualsForPeer()
		{
			return this._visuals.Count((PeerVisualsHolder v) => v != null);
		}

		public PeerVisualsHolder GetVisuals(int visualIndex)
		{
			if (this._visuals.Count <= 0)
			{
				return null;
			}
			return this._visuals[visualIndex];
		}

		public void ClearVisuals(int visualIndex)
		{
			if (visualIndex < this._visuals.Count && this._visuals[visualIndex] != null)
			{
				if (!GameNetwork.IsDedicatedServer)
				{
					MBAgentVisuals visuals = this._visuals[visualIndex].AgentVisuals.GetVisuals();
					visuals.ClearVisualComponents(true);
					visuals.ClearAllWeaponMeshes();
					visuals.Reset();
					if (this._visuals[visualIndex].MountAgentVisuals != null)
					{
						MBAgentVisuals visuals2 = this._visuals[visualIndex].MountAgentVisuals.GetVisuals();
						visuals2.ClearVisualComponents(true);
						visuals2.ClearAllWeaponMeshes();
						visuals2.Reset();
					}
				}
				this._visuals[visualIndex] = null;
			}
		}

		public void ClearAllVisuals(bool freeResources = false)
		{
			if (this._visuals != null)
			{
				for (int i = this._visuals.Count - 1; i >= 0; i--)
				{
					if (this._visuals[i] != null)
					{
						this.ClearVisuals(i);
					}
				}
				if (freeResources)
				{
					this._visuals = null;
				}
			}
		}

		public void OnVisualsSpawned(PeerVisualsHolder visualsHolder, int visualIndex)
		{
			if (visualIndex >= this._visuals.Count)
			{
				int num = visualIndex - this._visuals.Count;
				for (int i = 0; i < num + 1; i++)
				{
					this._visuals.Add(null);
				}
			}
			this._visuals[visualIndex] = visualsHolder;
		}

		public IEnumerable<IAgentVisual> GetAllAgentVisualsForPeer()
		{
			int count = this.GetAmountOfAgentVisualsForPeer();
			int num;
			for (int i = 0; i < count; i = num + 1)
			{
				yield return this.GetVisuals(i).AgentVisuals;
				num = i;
			}
			yield break;
		}

		public IAgentVisual GetAgentVisualForPeer(int visualsIndex)
		{
			IAgentVisual agentVisual;
			return this.GetAgentVisualForPeer(visualsIndex, out agentVisual);
		}

		public IAgentVisual GetAgentVisualForPeer(int visualsIndex, out IAgentVisual mountAgentVisuals)
		{
			PeerVisualsHolder visuals = this.GetVisuals(visualsIndex);
			mountAgentVisuals = ((visuals != null) ? visuals.MountAgentVisuals : null);
			if (visuals == null)
			{
				return null;
			}
			return visuals.AgentVisuals;
		}

		public void TickInactivityStatus()
		{
			NetworkCommunicator networkPeer = this.GetNetworkPeer();
			if (!networkPeer.IsMine)
			{
				if (this.ControlledAgent != null && this.ControlledAgent.IsActive())
				{
					if (this._lastActiveTime == MissionTime.Zero)
					{
						this._lastActiveTime = MissionTime.Now;
						this._previousActivityStatus = ValueTuple.Create<Agent.MovementControlFlag, Vec2, Vec3>(this.ControlledAgent.MovementFlags, this.ControlledAgent.MovementInputVector, this.ControlledAgent.LookDirection);
						this._inactiveWarningGiven = false;
						return;
					}
					ValueTuple<Agent.MovementControlFlag, Vec2, Vec3> valueTuple = ValueTuple.Create<Agent.MovementControlFlag, Vec2, Vec3>(this.ControlledAgent.MovementFlags, this.ControlledAgent.MovementInputVector, this.ControlledAgent.LookDirection);
					if (this._previousActivityStatus.Item1 != valueTuple.Item1 || this._previousActivityStatus.Item2.DistanceSquared(valueTuple.Item2) > 1E-05f || this._previousActivityStatus.Item3.DistanceSquared(valueTuple.Item3) > 1E-05f)
					{
						this._lastActiveTime = MissionTime.Now;
						this._previousActivityStatus = valueTuple;
						this._inactiveWarningGiven = false;
					}
					if (this._lastActiveTime.ElapsedSeconds > 180f)
					{
						DisconnectInfo disconnectInfo = networkPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
						disconnectInfo.Type = DisconnectType.Inactivity;
						networkPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", disconnectInfo);
						GameNetwork.AddNetworkPeerToDisconnectAsServer(networkPeer);
						return;
					}
					if (this._lastActiveTime.ElapsedSeconds > 120f && !this._inactiveWarningGiven)
					{
						MultiplayerGameNotificationsComponent missionBehavior = Mission.Current.GetMissionBehavior<MultiplayerGameNotificationsComponent>();
						if (missionBehavior != null)
						{
							missionBehavior.PlayerIsInactive(this.GetNetworkPeer());
						}
						this._inactiveWarningGiven = true;
						return;
					}
				}
				else
				{
					this._lastActiveTime = MissionTime.Now;
					this._inactiveWarningGiven = false;
				}
			}
		}

		public void OnKillAnotherPeer(MissionPeer victimPeer)
		{
			if (victimPeer != null)
			{
				if (!this._numberOfTimesPeerKilledPerPeer.ContainsKey(victimPeer))
				{
					this._numberOfTimesPeerKilledPerPeer.Add(victimPeer, 1);
				}
				else
				{
					Dictionary<MissionPeer, int> numberOfTimesPeerKilledPerPeer = this._numberOfTimesPeerKilledPerPeer;
					int num = numberOfTimesPeerKilledPerPeer[victimPeer];
					numberOfTimesPeerKilledPerPeer[victimPeer] = num + 1;
				}
				MissionPeer.OnPlayerKilledDelegate onPlayerKilled = MissionPeer.OnPlayerKilled;
				if (onPlayerKilled == null)
				{
					return;
				}
				onPlayerKilled(this, victimPeer);
			}
		}

		public void OverrideCultureWithTeamCulture()
		{
			MultiplayerOptions.OptionType optionType = ((this.Team.Side == BattleSideEnum.Attacker) ? MultiplayerOptions.OptionType.CultureTeam1 : MultiplayerOptions.OptionType.CultureTeam2);
			this.Culture = MBObjectManager.Instance.GetObject<BasicCultureObject>(optionType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
		}

		public int GetNumberOfTimesPeerKilledPeer(MissionPeer killedPeer)
		{
			if (this._numberOfTimesPeerKilledPerPeer.ContainsKey(killedPeer))
			{
				return this._numberOfTimesPeerKilledPerPeer[killedPeer];
			}
			return 0;
		}

		public void ResetKillRegistry()
		{
			this._numberOfTimesPeerKilledPerPeer.Clear();
		}

		public bool RefreshSelectedPerks()
		{
			MBList<MPPerkObject> mblist = new MBList<MPPerkObject>();
			List<List<IReadOnlyPerkObject>> availablePerksForPeer = MultiplayerClassDivisions.GetAvailablePerksForPeer(this);
			if (availablePerksForPeer.Count == 3)
			{
				for (int i = 0; i < 3; i++)
				{
					int num = this._perks[this.SelectedTroopIndex][i];
					if (availablePerksForPeer[i].Count > 0)
					{
						mblist.Add(availablePerksForPeer[i][(num >= 0 && num < availablePerksForPeer[i].Count) ? num : 0].Clone(this));
					}
				}
				this._selectedPerks = new ValueTuple<int, MBList<MPPerkObject>>(this.SelectedTroopIndex, mblist);
				return true;
			}
			return false;
		}

		private void ResetSelectedPerks()
		{
			if (this._selectedPerks.Item2 != null)
			{
				foreach (MPPerkObject mpperkObject in this._selectedPerks.Item2)
				{
					mpperkObject.Reset();
				}
			}
		}

		private void CultureChanged(BasicCultureObject newCulture)
		{
			List<MultiplayerClassDivisions.MPHeroClass> list = MultiplayerClassDivisions.GetMPHeroClasses(newCulture).ToList<MultiplayerClassDivisions.MPHeroClass>();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				MultiplayerClassDivisions.MPHeroClass mpheroClass = list[i];
				List<MPPerkSelectionManager.MPPerkSelection> selectionsForHeroClass = MPPerkSelectionManager.Instance.GetSelectionsForHeroClass(mpheroClass);
				if (selectionsForHeroClass != null)
				{
					int count2 = selectionsForHeroClass.Count;
					for (int j = 0; j < count2; j++)
					{
						MPPerkSelectionManager.MPPerkSelection mpperkSelection = selectionsForHeroClass[j];
						this._perks[i][mpperkSelection.ListIndex] = mpperkSelection.Index;
					}
				}
				else
				{
					for (int k = 0; k < 3; k++)
					{
						this._perks[i][k] = 0;
					}
				}
			}
			if (base.IsMine && GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new TeamInitialPerkInfoMessage(this._perks[this.SelectedTroopIndex]));
				GameNetwork.EndModuleEventAsClient();
			}
		}

		public void OnTeamInitialPerkInfoReceived(int[] perks)
		{
			for (int i = 0; i < 3; i++)
			{
				this.SelectPerk(i, perks[i], -1);
			}
			this.TeamInitialPerkInfoReady = true;
		}

		public const int NumberOfPerkLists = 3;

		public const int MaxNumberOfTroopTypesPerCulture = 16;

		private const float InactivityKickInSeconds = 180f;

		private const float InactivityWarnInSeconds = 120f;

		public const int MinKDACount = -1000;

		public const int MaxKDACount = 100000;

		public const int MinScore = -1000000;

		public const int MaxScore = 1000000;

		public const int MinSpawnTimer = 3;

		public int CaptainBeingDetachedThreshold = 125;

		private List<PeerVisualsHolder> _visuals = new List<PeerVisualsHolder>();

		private Dictionary<MissionPeer, int> _numberOfTimesPeerKilledPerPeer = new Dictionary<MissionPeer, int>();

		private MissionTime _lastActiveTime = MissionTime.Zero;

		private ValueTuple<Agent.MovementControlFlag, Vec2, Vec3> _previousActivityStatus;

		private bool _inactiveWarningGiven;

		private int _selectedTroopIndex;

		private Agent _followedAgent;

		private Team _team;

		private BasicCultureObject _culture;

		private Formation _controlledFormation;

		private MissionRepresentativeBase _representative;

		private readonly MBList<int[]> _perks;

		private int _killCount;

		private int _assistCount;

		private int _deathCount;

		private int _score;

		private ValueTuple<int, MBList<MPPerkObject>> _selectedPerks;

		private int _botsUnderControlAlive;

		public delegate void OnUpdateEquipmentSetIndexEventDelegate(MissionPeer lobbyPeer, int equipmentSetIndex);

		public delegate void OnPerkUpdateEventDelegate(MissionPeer peer);

		public delegate void OnTeamChangedDelegate(NetworkCommunicator peer, Team previousTeam, Team newTeam);

		public delegate void OnCultureChangedDelegate(BasicCultureObject newCulture);

		public delegate void OnPlayerKilledDelegate(MissionPeer killerPeer, MissionPeer killedPeer);
	}
}
