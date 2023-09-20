using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace TaleWorlds.MountAndBlade
{
	public class MissionMultiplayerGameModeDuelClient : MissionMultiplayerGameModeBaseClient
	{
		public override bool IsGameModeUsingGold
		{
			get
			{
				return false;
			}
		}

		public override bool IsGameModeTactical
		{
			get
			{
				return false;
			}
		}

		public override bool IsGameModeUsingRoundCountdown
		{
			get
			{
				return false;
			}
		}

		public override bool IsGameModeUsingAllowCultureChange
		{
			get
			{
				return true;
			}
		}

		public override bool IsGameModeUsingAllowTroopChange
		{
			get
			{
				return true;
			}
		}

		public override MultiplayerGameType GameType
		{
			get
			{
				return MultiplayerGameType.Duel;
			}
		}

		public bool IsInDuel
		{
			get
			{
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				bool? flag;
				if (component == null)
				{
					flag = null;
				}
				else
				{
					Team team = component.Team;
					flag = ((team != null) ? new bool?(team.IsDefender) : null);
				}
				return flag ?? false;
			}
		}

		public DuelMissionRepresentative MyRepresentative { get; private set; }

		private void OnMyClientSynchronized()
		{
			this.MyRepresentative = GameNetwork.MyPeer.GetComponent<DuelMissionRepresentative>();
			Action onMyRepresentativeAssigned = this.OnMyRepresentativeAssigned;
			if (onMyRepresentativeAssigned != null)
			{
				onMyRepresentativeAssigned();
			}
			this.MyRepresentative.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
		}

		public override int GetGoldAmount()
		{
			return 0;
		}

		public override void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount)
		{
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			base.MissionNetworkComponent.OnMyClientSynchronized += this.OnMyClientSynchronized;
		}

		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			base.MissionNetworkComponent.OnMyClientSynchronized -= this.OnMyClientSynchronized;
			if (this.MyRepresentative != null)
			{
				this.MyRepresentative.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			if (this.MyRepresentative != null)
			{
				this.MyRepresentative.CheckHasRequestFromAndRemoveRequestIfNeeded(affectedAgent.MissionPeer);
			}
		}

		public override bool CanRequestCultureChange()
		{
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			return ((missionPeer != null) ? missionPeer.Team : null) != null && missionPeer.Team.IsAttacker;
		}

		public override bool CanRequestTroopChange()
		{
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			return ((missionPeer != null) ? missionPeer.Team : null) != null && missionPeer.Team.IsAttacker;
		}

		public Action OnMyRepresentativeAssigned;
	}
}
