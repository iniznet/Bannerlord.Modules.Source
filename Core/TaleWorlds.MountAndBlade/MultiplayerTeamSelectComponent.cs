using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerTeamSelectComponent : MissionNetwork
	{
		public event MultiplayerTeamSelectComponent.OnSelectingTeamDelegate OnSelectingTeam;

		public event Action OnMyTeamChange;

		public event Action OnUpdateTeams;

		public event Action OnUpdateFriendsPerTeam;

		public bool TeamSelectionEnabled { get; private set; }

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionNetworkComponent = base.Mission.GetMissionBehavior<MissionNetworkComponent>();
			this._gameModeServer = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBase>();
			if (BannerlordNetwork.LobbyMissionType == LobbyMissionType.Matchmaker)
			{
				this.TeamSelectionEnabled = false;
				return;
			}
			this.TeamSelectionEnabled = true;
		}

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsServer)
			{
				registerer.Register<TeamChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<TeamChange>(this.HandleClientEventTeamChange));
			}
		}

		private void OnMyClientSynchronized()
		{
			base.Mission.GetMissionBehavior<MissionNetworkComponent>().OnMyClientSynchronized -= this.OnMyClientSynchronized;
			if (Mission.Current.GetMissionBehavior<MissionLobbyComponent>().CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending && GameNetwork.MyPeer.GetComponent<MissionPeer>().Team == null)
			{
				this.SelectTeam();
			}
		}

		public override void AfterStart()
		{
			this._platformFriends = new HashSet<PlayerId>();
			foreach (PlayerId playerId in FriendListService.GetAllFriendsInAllPlatforms())
			{
				this._platformFriends.Add(playerId);
			}
			this._friendsPerTeam = new Dictionary<Team, IEnumerable<VirtualPlayer>>();
			MissionPeer.OnTeamChanged += this.UpdateTeams;
			if (GameNetwork.IsClient)
			{
				MissionNetworkComponent missionBehavior = base.Mission.GetMissionBehavior<MissionNetworkComponent>();
				if (this.TeamSelectionEnabled)
				{
					missionBehavior.OnMyClientSynchronized += this.OnMyClientSynchronized;
				}
			}
		}

		public override void OnRemoveBehavior()
		{
			MissionPeer.OnTeamChanged -= this.UpdateTeams;
			this.OnMyTeamChange = null;
			base.OnRemoveBehavior();
		}

		private bool HandleClientEventTeamChange(NetworkCommunicator peer, TeamChange message)
		{
			if (this.TeamSelectionEnabled)
			{
				if (message.AutoAssign)
				{
					this.AutoAssignTeam(peer);
				}
				else
				{
					this.ChangeTeamServer(peer, message.Team);
				}
			}
			return true;
		}

		public void SelectTeam()
		{
			if (this.OnSelectingTeam != null)
			{
				List<Team> disabledTeams = this.GetDisabledTeams();
				this.OnSelectingTeam(disabledTeams);
			}
		}

		public void UpdateTeams(NetworkCommunicator peer, Team oldTeam, Team newTeam)
		{
			if (this.OnUpdateTeams != null)
			{
				this.OnUpdateTeams();
			}
			if (GameNetwork.IsMyPeerReady)
			{
				this.CacheFriendsForTeams();
			}
			if (newTeam.Side != BattleSideEnum.None)
			{
				MissionPeer component = peer.GetComponent<MissionPeer>();
				component.SelectedTroopIndex = 0;
				component.NextSelectedTroopIndex = 0;
				component.OverrideCultureWithTeamCulture();
			}
		}

		public static int GetAutoTeamBalanceDifference(AutoTeamBalanceLimits limit)
		{
			switch (limit)
			{
			case AutoTeamBalanceLimits.Off:
				return 0;
			case AutoTeamBalanceLimits.LimitTo2:
				return 2;
			case AutoTeamBalanceLimits.LimitTo3:
				return 3;
			case AutoTeamBalanceLimits.LimitTo5:
				return 5;
			case AutoTeamBalanceLimits.LimitTo10:
				return 10;
			case AutoTeamBalanceLimits.LimitTo20:
				return 20;
			default:
				Debug.FailedAssert("Unknown auto team balance limit!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MultiplayerTeamSelectComponent.cs", "GetAutoTeamBalanceDifference", 195);
				return 0;
			}
		}

		public List<Team> GetDisabledTeams()
		{
			List<Team> list = new List<Team>();
			if (MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) == 0)
			{
				return list;
			}
			Team myTeam = (GameNetwork.IsMyPeerReady ? GameNetwork.MyPeer.GetComponent<MissionPeer>().Team : null);
			Team[] array = base.Mission.Teams.Where((Team q) => q != this.Mission.SpectatorTeam).OrderBy(delegate(Team q)
			{
				if (myTeam == null)
				{
					return this.GetPlayerCountForTeam(q);
				}
				if (q != myTeam)
				{
					return this.GetPlayerCountForTeam(q);
				}
				return this.GetPlayerCountForTeam(q) - 1;
			}).ToArray<Team>();
			foreach (Team team in array)
			{
				int num = this.GetPlayerCountForTeam(team);
				int num2 = this.GetPlayerCountForTeam(array[0]);
				if (myTeam == team)
				{
					num--;
				}
				if (myTeam == array[0])
				{
					num2--;
				}
				if (num - num2 >= MultiplayerTeamSelectComponent.GetAutoTeamBalanceDifference((AutoTeamBalanceLimits)MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)))
				{
					list.Add(team);
				}
			}
			return list;
		}

		public void ChangeTeamServer(NetworkCommunicator networkPeer, Team team)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			Team team2 = component.Team;
			if (team2 != null && team2 != base.Mission.SpectatorTeam && team2 != team && component.ControlledAgent != null)
			{
				Blow blow = new Blow(component.ControlledAgent.Index);
				blow.DamageType = DamageTypes.Invalid;
				blow.BaseMagnitude = 10000f;
				blow.Position = component.ControlledAgent.Position;
				blow.DamagedPercentage = 1f;
				component.ControlledAgent.Die(blow, Agent.KillInfo.TeamSwitch);
			}
			component.Team = team;
			BasicCultureObject basicCultureObject = (component.Team.IsAttacker ? MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)) : MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)));
			component.Culture = basicCultureObject;
			if (team != team2)
			{
				if (component.HasSpawnedAgentVisuals)
				{
					component.HasSpawnedAgentVisuals = false;
					MBDebug.Print("HasSpawnedAgentVisuals = false for peer: " + component.Name + " because he just changed his team", 0, Debug.DebugColor.White, 17592186044416UL);
					component.SpawnCountThisRound = 0;
					Mission.Current.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component, true);
				}
				if (!this._gameModeServer.IsGameModeHidingAllAgentVisuals && !networkPeer.IsServerPeer)
				{
					MissionNetworkComponent missionNetworkComponent = this._missionNetworkComponent;
					if (missionNetworkComponent != null)
					{
						missionNetworkComponent.OnPeerSelectedTeam(component);
					}
				}
				this._gameModeServer.OnPeerChangedTeam(networkPeer, team2, team);
				component.SpawnTimer.Reset(Mission.Current.CurrentTime, 0.1f);
				component.WantsToSpawnAsBot = false;
				component.HasSpawnTimerExpired = false;
			}
			this.UpdateTeams(networkPeer, team2, team);
		}

		public void ChangeTeam(Team team)
		{
			if (team != GameNetwork.MyPeer.GetComponent<MissionPeer>().Team)
			{
				if (GameNetwork.IsServer)
				{
					Mission.Current.PlayerTeam = team;
					this.ChangeTeamServer(GameNetwork.MyPeer, team);
				}
				else
				{
					foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
					{
						MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
						if (component != null)
						{
							component.ClearAllVisuals(false);
						}
					}
					GameNetwork.BeginModuleEventAsClient();
					GameNetwork.WriteMessage(new TeamChange(false, team));
					GameNetwork.EndModuleEventAsClient();
				}
				if (this.OnMyTeamChange != null)
				{
					this.OnMyTeamChange();
				}
			}
		}

		public int GetPlayerCountForTeam(Team team)
		{
			int num = 0;
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (((component != null) ? component.Team : null) != null && component.Team == team)
				{
					num++;
				}
			}
			return num;
		}

		private void CacheFriendsForTeams()
		{
			this._friendsPerTeam.Clear();
			if (this._platformFriends.Count > 0)
			{
				List<MissionPeer> list = new List<MissionPeer>();
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null && this._platformFriends.Contains(networkCommunicator.VirtualPlayer.Id))
					{
						list.Add(component);
					}
				}
				using (List<Team>.Enumerator enumerator2 = base.Mission.Teams.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Team team = enumerator2.Current;
						if (team != null)
						{
							this._friendsPerTeam.Add(team, from x in list
								where x.Team == team
								select x.Peer);
						}
					}
				}
				if (this.OnUpdateFriendsPerTeam != null)
				{
					this.OnUpdateFriendsPerTeam();
				}
			}
		}

		public IEnumerable<VirtualPlayer> GetFriendsForTeam(Team team)
		{
			if (this._friendsPerTeam.ContainsKey(team))
			{
				return this._friendsPerTeam[team];
			}
			return new List<VirtualPlayer>();
		}

		public void BalanceTeams()
		{
			if (MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) != 0)
			{
				int i = this.GetPlayerCountForTeam(Mission.Current.AttackerTeam);
				int j = this.GetPlayerCountForTeam(Mission.Current.DefenderTeam);
				while (i > j + MultiplayerTeamSelectComponent.GetAutoTeamBalanceDifference((AutoTeamBalanceLimits)MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)))
				{
					MissionPeer missionPeer = null;
					foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
					{
						if (networkCommunicator.IsSynchronized)
						{
							MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
							if (((component != null) ? component.Team : null) != null && component.Team == base.Mission.AttackerTeam && (missionPeer == null || component.JoinTime >= missionPeer.JoinTime))
							{
								missionPeer = component;
							}
						}
					}
					this.ChangeTeamServer(missionPeer.GetNetworkPeer(), Mission.Current.DefenderTeam);
					i--;
					j++;
				}
				while (j > i + MultiplayerTeamSelectComponent.GetAutoTeamBalanceDifference((AutoTeamBalanceLimits)MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)))
				{
					MissionPeer missionPeer2 = null;
					foreach (NetworkCommunicator networkCommunicator2 in GameNetwork.NetworkPeers)
					{
						if (networkCommunicator2.IsSynchronized)
						{
							MissionPeer component2 = networkCommunicator2.GetComponent<MissionPeer>();
							if (((component2 != null) ? component2.Team : null) != null && component2.Team == base.Mission.DefenderTeam && (missionPeer2 == null || component2.JoinTime >= missionPeer2.JoinTime))
							{
								missionPeer2 = component2;
							}
						}
					}
					this.ChangeTeamServer(missionPeer2.GetNetworkPeer(), Mission.Current.AttackerTeam);
					i++;
					j--;
				}
			}
		}

		public void AutoAssignTeam(NetworkCommunicator peer)
		{
			if (!GameNetwork.IsServer)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new TeamChange(true, null));
				GameNetwork.EndModuleEventAsClient();
				if (this.OnMyTeamChange != null)
				{
					this.OnMyTeamChange();
				}
				return;
			}
			List<Team> disabledTeams = this.GetDisabledTeams();
			List<Team> list = base.Mission.Teams.Where((Team x) => !disabledTeams.Contains(x) && x.Side != BattleSideEnum.None).ToList<Team>();
			Team team;
			if (list.Count > 1)
			{
				int[] array = new int[list.Count];
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (((component != null) ? component.Team : null) != null)
					{
						for (int i = 0; i < list.Count; i++)
						{
							if (component.Team == list[i])
							{
								array[i]++;
							}
						}
					}
				}
				int num = -1;
				int num2 = -1;
				for (int j = 0; j < array.Length; j++)
				{
					if (num2 < 0 || array[j] < num)
					{
						num2 = j;
						num = array[j];
					}
				}
				team = list[num2];
			}
			else
			{
				team = list[0];
			}
			if (!peer.IsMine)
			{
				this.ChangeTeamServer(peer, team);
				return;
			}
			this.ChangeTeam(team);
		}

		private MissionNetworkComponent _missionNetworkComponent;

		private MissionMultiplayerGameModeBase _gameModeServer;

		private HashSet<PlayerId> _platformFriends;

		private Dictionary<Team, IEnumerable<VirtualPlayer>> _friendsPerTeam;

		public delegate void OnSelectingTeamDelegate(List<Team> disableTeams);
	}
}
