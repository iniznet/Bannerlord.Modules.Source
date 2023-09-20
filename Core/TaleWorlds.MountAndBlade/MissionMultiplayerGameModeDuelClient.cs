using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200029A RID: 666
	public class MissionMultiplayerGameModeDuelClient : MissionMultiplayerGameModeBaseClient
	{
		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x06002413 RID: 9235 RVA: 0x00085265 File Offset: 0x00083465
		public override bool IsGameModeUsingGold
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x06002414 RID: 9236 RVA: 0x00085268 File Offset: 0x00083468
		public override bool IsGameModeTactical
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x06002415 RID: 9237 RVA: 0x0008526B File Offset: 0x0008346B
		public override bool IsGameModeUsingRoundCountdown
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x06002416 RID: 9238 RVA: 0x0008526E File Offset: 0x0008346E
		public override bool IsGameModeUsingAllowCultureChange
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x06002417 RID: 9239 RVA: 0x00085271 File Offset: 0x00083471
		public override bool IsGameModeUsingAllowTroopChange
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x06002418 RID: 9240 RVA: 0x00085274 File Offset: 0x00083474
		public override MissionLobbyComponent.MultiplayerGameType GameType
		{
			get
			{
				return MissionLobbyComponent.MultiplayerGameType.Duel;
			}
		}

		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x06002419 RID: 9241 RVA: 0x00085278 File Offset: 0x00083478
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

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x0600241A RID: 9242 RVA: 0x000852CF File Offset: 0x000834CF
		// (set) Token: 0x0600241B RID: 9243 RVA: 0x000852D7 File Offset: 0x000834D7
		public DuelMissionRepresentative MyRepresentative { get; private set; }

		// Token: 0x0600241C RID: 9244 RVA: 0x000852E0 File Offset: 0x000834E0
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

		// Token: 0x0600241D RID: 9245 RVA: 0x0008530F File Offset: 0x0008350F
		public override int GetGoldAmount()
		{
			return 0;
		}

		// Token: 0x0600241E RID: 9246 RVA: 0x00085312 File Offset: 0x00083512
		public override void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount)
		{
		}

		// Token: 0x0600241F RID: 9247 RVA: 0x00085314 File Offset: 0x00083514
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			base.MissionNetworkComponent.OnMyClientSynchronized += this.OnMyClientSynchronized;
		}

		// Token: 0x06002420 RID: 9248 RVA: 0x00085333 File Offset: 0x00083533
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			base.MissionNetworkComponent.OnMyClientSynchronized -= this.OnMyClientSynchronized;
			if (this.MyRepresentative != null)
			{
				this.MyRepresentative.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
			}
		}

		// Token: 0x06002421 RID: 9249 RVA: 0x00085366 File Offset: 0x00083566
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			if (this.MyRepresentative != null)
			{
				this.MyRepresentative.CheckHasRequestFromAndRemoveRequestIfNeeded(affectedAgent.MissionPeer);
			}
		}

		// Token: 0x06002422 RID: 9250 RVA: 0x00085390 File Offset: 0x00083590
		public override bool CanRequestCultureChange()
		{
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			return ((missionPeer != null) ? missionPeer.Team : null) != null && missionPeer.Team.IsAttacker;
		}

		// Token: 0x06002423 RID: 9251 RVA: 0x000853CC File Offset: 0x000835CC
		public override bool CanRequestTroopChange()
		{
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			return ((missionPeer != null) ? missionPeer.Team : null) != null && missionPeer.Team.IsAttacker;
		}

		// Token: 0x04000D2A RID: 3370
		public Action OnMyRepresentativeAssigned;
	}
}
