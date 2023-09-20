using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000295 RID: 661
	public class MissionScoreboardComponent : MissionNetwork
	{
		// Token: 0x14000040 RID: 64
		// (add) Token: 0x0600239F RID: 9119 RVA: 0x00083A6C File Offset: 0x00081C6C
		// (remove) Token: 0x060023A0 RID: 9120 RVA: 0x00083AA4 File Offset: 0x00081CA4
		public event Action OnRoundPropertiesChanged;

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x060023A1 RID: 9121 RVA: 0x00083ADC File Offset: 0x00081CDC
		// (remove) Token: 0x060023A2 RID: 9122 RVA: 0x00083B14 File Offset: 0x00081D14
		public event Action<BattleSideEnum> OnBotPropertiesChanged;

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x060023A3 RID: 9123 RVA: 0x00083B4C File Offset: 0x00081D4C
		// (remove) Token: 0x060023A4 RID: 9124 RVA: 0x00083B84 File Offset: 0x00081D84
		public event Action<Team, Team, MissionPeer> OnPlayerSideChanged;

		// Token: 0x14000043 RID: 67
		// (add) Token: 0x060023A5 RID: 9125 RVA: 0x00083BBC File Offset: 0x00081DBC
		// (remove) Token: 0x060023A6 RID: 9126 RVA: 0x00083BF4 File Offset: 0x00081DF4
		public event Action<BattleSideEnum, MissionPeer> OnPlayerPropertiesChanged;

		// Token: 0x14000044 RID: 68
		// (add) Token: 0x060023A7 RID: 9127 RVA: 0x00083C2C File Offset: 0x00081E2C
		// (remove) Token: 0x060023A8 RID: 9128 RVA: 0x00083C64 File Offset: 0x00081E64
		public event Action<MissionPeer, int> OnMVPSelected;

		// Token: 0x14000045 RID: 69
		// (add) Token: 0x060023A9 RID: 9129 RVA: 0x00083C9C File Offset: 0x00081E9C
		// (remove) Token: 0x060023AA RID: 9130 RVA: 0x00083CD4 File Offset: 0x00081ED4
		public event Action OnScoreboardInitialized;

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x060023AB RID: 9131 RVA: 0x00083D09 File Offset: 0x00081F09
		public bool IsOneSided
		{
			get
			{
				return this._scoreboardSides == MissionScoreboardComponent.ScoreboardSides.OneSide;
			}
		}

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x060023AC RID: 9132 RVA: 0x00083D14 File Offset: 0x00081F14
		public MissionScoreboardComponent.ScoreboardHeader[] Headers
		{
			get
			{
				return this._scoreboardData.GetScoreboardHeaders();
			}
		}

		// Token: 0x060023AD RID: 9133 RVA: 0x00083D21 File Offset: 0x00081F21
		public MissionScoreboardComponent(IScoreboardData scoreboardData)
		{
			this._scoreboardData = scoreboardData;
			this._spectators = new List<MissionPeer>();
			this._sides = new MissionScoreboardComponent.MissionScoreboardSide[2];
			this._roundWinnerList = new List<BattleSideEnum>();
			this._mvpCountPerPeer = new List<ValueTuple<MissionPeer, int>>();
		}

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x060023AE RID: 9134 RVA: 0x00083D5D File Offset: 0x00081F5D
		public IEnumerable<BattleSideEnum> RoundWinnerList
		{
			get
			{
				return this._roundWinnerList.AsReadOnly();
			}
		}

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x060023AF RID: 9135 RVA: 0x00083D6A File Offset: 0x00081F6A
		public MissionScoreboardComponent.MissionScoreboardSide[] Sides
		{
			get
			{
				return this._sides;
			}
		}

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x060023B0 RID: 9136 RVA: 0x00083D72 File Offset: 0x00081F72
		public List<MissionPeer> Spectators
		{
			get
			{
				return this._spectators;
			}
		}

		// Token: 0x060023B1 RID: 9137 RVA: 0x00083D7C File Offset: 0x00081F7C
		public override void AfterStart()
		{
			this.Clear();
			this._missionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._missionNetworkComponent = base.Mission.GetMissionBehavior<MissionNetworkComponent>();
			this._mpGameModeBase = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			if (this._missionLobbyComponent.MissionType == MissionLobbyComponent.MultiplayerGameType.FreeForAll || this._missionLobbyComponent.MissionType == MissionLobbyComponent.MultiplayerGameType.Duel)
			{
				this._scoreboardSides = MissionScoreboardComponent.ScoreboardSides.OneSide;
			}
			else
			{
				this._scoreboardSides = MissionScoreboardComponent.ScoreboardSides.TwoSides;
			}
			MissionPeer.OnTeamChanged += this.TeamChange;
			this._missionNetworkComponent.OnMyClientSynchronized += this.OnMyClientSynchronized;
			if (GameNetwork.IsServerOrRecorder && this._mpGameModeBase.RoundComponent != null)
			{
				this._mpGameModeBase.RoundComponent.OnRoundEnding += this.OnRoundEnding;
				this._mpGameModeBase.RoundComponent.OnPreRoundEnding += this.OnPreRoundEnding;
			}
			this.LateInitScoreboard();
		}

		// Token: 0x060023B2 RID: 9138 RVA: 0x00083E67 File Offset: 0x00082067
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register<UpdateRoundScores>(new GameNetworkMessage.ServerMessageHandlerDelegate<UpdateRoundScores>(this.HandleServerUpdateRoundScoresMessage));
				registerer.Register<SetRoundMVP>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetRoundMVP>(this.HandleServerSetRoundMVP));
				registerer.Register<BotData>(new GameNetworkMessage.ServerMessageHandlerDelegate<BotData>(this.HandleServerEventBotDataMessage));
			}
		}

		// Token: 0x060023B3 RID: 9139 RVA: 0x00083EA8 File Offset: 0x000820A8
		public override void OnRemoveBehavior()
		{
			this._spectators.Clear();
			for (int i = 0; i < 2; i++)
			{
				if (this._sides[i] != null)
				{
					this._sides[i].Clear();
				}
			}
			MissionPeer.OnTeamChanged -= this.TeamChange;
			this._missionNetworkComponent.OnMyClientSynchronized -= this.OnMyClientSynchronized;
			if (GameNetwork.IsServerOrRecorder && this._mpGameModeBase.RoundComponent != null)
			{
				this._mpGameModeBase.RoundComponent.OnRoundEnding -= this.OnRoundEnding;
			}
			base.OnRemoveBehavior();
		}

		// Token: 0x060023B4 RID: 9140 RVA: 0x00083F41 File Offset: 0x00082141
		public void Clear()
		{
			this._spectators.Clear();
		}

		// Token: 0x060023B5 RID: 9141 RVA: 0x00083F50 File Offset: 0x00082150
		public void ResetBotScores()
		{
			foreach (MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide in this._sides)
			{
				if (((missionScoreboardSide != null) ? missionScoreboardSide.BotScores : null) != null)
				{
					missionScoreboardSide.BotScores.ResetKillDeathAssist();
				}
			}
		}

		// Token: 0x060023B6 RID: 9142 RVA: 0x00083F90 File Offset: 0x00082190
		public void ChangeTeamScore(Team team, int scoreChange)
		{
			MissionScoreboardComponent.MissionScoreboardSide sideSafe = this.GetSideSafe(team.Side);
			sideSafe.SideScore += scoreChange;
			sideSafe.SideScore = MBMath.ClampInt(sideSafe.SideScore, -1023000, 1023000);
			if (GameNetwork.IsServer)
			{
				int num = ((this._scoreboardSides != MissionScoreboardComponent.ScoreboardSides.OneSide) ? this._sides[0].SideScore : 0);
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new UpdateRoundScores(this._sides[1].SideScore, num));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
			if (this.OnRoundPropertiesChanged != null)
			{
				this.OnRoundPropertiesChanged();
			}
		}

		// Token: 0x060023B7 RID: 9143 RVA: 0x00084028 File Offset: 0x00082228
		public void ClearScores()
		{
			this._sides[1].SideScore = 0;
			if (this._scoreboardSides == MissionScoreboardComponent.ScoreboardSides.TwoSides)
			{
				this._sides[0].SideScore = 0;
			}
			if (this.OnRoundPropertiesChanged != null)
			{
				this.OnRoundPropertiesChanged();
			}
		}

		// Token: 0x060023B8 RID: 9144 RVA: 0x00084064 File Offset: 0x00082264
		private void UpdateRoundScores()
		{
			foreach (MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide in this._sides)
			{
				if (missionScoreboardSide != null && missionScoreboardSide.Side == this.RoundWinner)
				{
					this._roundWinnerList.Add(this.RoundWinner);
					if (this.RoundWinner != BattleSideEnum.None)
					{
						this._sides[(int)this.RoundWinner].SideScore++;
					}
				}
			}
			if (this.OnRoundPropertiesChanged != null)
			{
				this.OnRoundPropertiesChanged();
			}
			if (GameNetwork.IsServer)
			{
				int num = ((this._scoreboardSides != MissionScoreboardComponent.ScoreboardSides.OneSide) ? this._sides[0].SideScore : 0);
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new UpdateRoundScores(this._sides[1].SideScore, num));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
		}

		// Token: 0x060023B9 RID: 9145 RVA: 0x00084126 File Offset: 0x00082326
		public MissionScoreboardComponent.MissionScoreboardSide GetSideSafe(BattleSideEnum battleSide)
		{
			if (this._scoreboardSides == MissionScoreboardComponent.ScoreboardSides.OneSide)
			{
				return this._sides[1];
			}
			return this._sides[(int)battleSide];
		}

		// Token: 0x060023BA RID: 9146 RVA: 0x00084141 File Offset: 0x00082341
		public int GetRoundScore(BattleSideEnum side)
		{
			if (side > (BattleSideEnum)this._sides.Length || side < BattleSideEnum.Defender)
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MissionScoreboardComponent.cs", "GetRoundScore", 448);
				return 0;
			}
			return this.GetSideSafe(side).SideScore;
		}

		// Token: 0x060023BB RID: 9147 RVA: 0x0008417C File Offset: 0x0008237C
		public void HandleServerUpdateRoundScoresMessage(UpdateRoundScores message)
		{
			this._sides[1].SideScore = message.AttackerTeamScore;
			if (this._scoreboardSides != MissionScoreboardComponent.ScoreboardSides.OneSide)
			{
				this._sides[0].SideScore = message.DefenderTeamScore;
			}
			if (this.OnRoundPropertiesChanged != null)
			{
				this.OnRoundPropertiesChanged();
			}
		}

		// Token: 0x060023BC RID: 9148 RVA: 0x000841CA File Offset: 0x000823CA
		public void HandleServerSetRoundMVP(SetRoundMVP message)
		{
			Action<MissionPeer, int> onMVPSelected = this.OnMVPSelected;
			if (onMVPSelected != null)
			{
				onMVPSelected(message.MVPPeer.GetComponent<MissionPeer>(), message.MVPCount);
			}
			this.PlayerPropertiesChanged(message.MVPPeer);
		}

		// Token: 0x060023BD RID: 9149 RVA: 0x000841FC File Offset: 0x000823FC
		public void CalculateTotalNumbers()
		{
			foreach (MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide in this._sides)
			{
				if (missionScoreboardSide != null)
				{
					int num = missionScoreboardSide.BotScores.DeathCount;
					int num2 = missionScoreboardSide.BotScores.AssistCount;
					int num3 = missionScoreboardSide.BotScores.KillCount;
					foreach (MissionPeer missionPeer in missionScoreboardSide.Players)
					{
						num2 += missionPeer.AssistCount;
						num += missionPeer.DeathCount;
						num3 += missionPeer.KillCount;
					}
				}
			}
		}

		// Token: 0x060023BE RID: 9150 RVA: 0x000842B4 File Offset: 0x000824B4
		private void TeamChange(NetworkCommunicator player, Team oldTeam, Team nextTeam)
		{
			if (oldTeam == null && MBNetwork.VirtualPlayers[player.VirtualPlayer.Index] != player.VirtualPlayer)
			{
				Debug.Print("Ignoring team change call for {}, dced peer.", 0, Debug.DebugColor.White, 17179869184UL);
				return;
			}
			MissionPeer component = player.GetComponent<MissionPeer>();
			if (oldTeam != null)
			{
				if (oldTeam == base.Mission.SpectatorTeam)
				{
					this._spectators.Remove(component);
				}
				else
				{
					this.GetSideSafe(oldTeam.Side).RemovePlayer(component);
				}
			}
			if (nextTeam != null)
			{
				if (nextTeam == base.Mission.SpectatorTeam)
				{
					this._spectators.Add(component);
				}
				else
				{
					Debug.Print(string.Format(">SBC => {0} is switching from {1} to {2}. Adding to scoreboard side {3}.", new object[]
					{
						player.UserName,
						(oldTeam == null) ? "NULL" : oldTeam.Side.ToString(),
						nextTeam.Side.ToString(),
						nextTeam.Side
					}), 0, Debug.DebugColor.Blue, 17179869184UL);
					this.GetSideSafe(nextTeam.Side).AddPlayer(component);
				}
			}
			if (this.OnPlayerSideChanged != null)
			{
				this.OnPlayerSideChanged(oldTeam, nextTeam, component);
			}
		}

		// Token: 0x060023BF RID: 9151 RVA: 0x000843EC File Offset: 0x000825EC
		public override void OnClearScene()
		{
			foreach (MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide in this.Sides)
			{
				if (missionScoreboardSide != null)
				{
					missionScoreboardSide.BotScores.AliveCount = 0;
				}
			}
		}

		// Token: 0x060023C0 RID: 9152 RVA: 0x00084424 File Offset: 0x00082624
		public override void OnPlayerConnectedToServer(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (component != null && component.Team != null)
			{
				this.TeamChange(networkPeer, null, component.Team);
			}
		}

		// Token: 0x060023C1 RID: 9153 RVA: 0x00084454 File Offset: 0x00082654
		public override void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
			MissionPeer missionPeer = networkPeer.GetComponent<MissionPeer>();
			if (missionPeer != null)
			{
				bool flag = this._spectators.Contains(missionPeer);
				bool flag2 = this._sides.Any((MissionScoreboardComponent.MissionScoreboardSide x) => x != null && x.Players.Contains(missionPeer));
				if (flag)
				{
					this._spectators.Remove(missionPeer);
					return;
				}
				if (flag2)
				{
					this.GetSideSafe(missionPeer.Team.Side).RemovePlayer(missionPeer);
					Formation controlledFormation = missionPeer.ControlledFormation;
					if (controlledFormation != null)
					{
						Team team = missionPeer.Team;
						BotData botScores = this.Sides[(int)team.Side].BotScores;
						botScores.AliveCount += controlledFormation.GetCountOfUnitsWithCondition((Agent agent) => agent.IsActive());
						this.BotPropertiesChanged(team.Side);
					}
					Action<Team, Team, MissionPeer> onPlayerSideChanged = this.OnPlayerSideChanged;
					if (onPlayerSideChanged == null)
					{
						return;
					}
					onPlayerSideChanged(missionPeer.Team, null, missionPeer);
				}
			}
		}

		// Token: 0x060023C2 RID: 9154 RVA: 0x0008456F File Offset: 0x0008276F
		private void BotsControlledChanged(NetworkCommunicator peer)
		{
			this.PlayerPropertiesChanged(peer);
		}

		// Token: 0x060023C3 RID: 9155 RVA: 0x00084578 File Offset: 0x00082778
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.IsActive() && !agent.IsMount)
			{
				if (agent.MissionPeer == null)
				{
					this.BotPropertiesChanged(agent.Team.Side);
					return;
				}
				if (agent.MissionPeer != null)
				{
					this.PlayerPropertiesChanged(agent.MissionPeer.GetNetworkPeer());
				}
			}
		}

		// Token: 0x060023C4 RID: 9156 RVA: 0x000845C8 File Offset: 0x000827C8
		public override void OnAssignPlayerAsSergeantOfFormation(Agent agent)
		{
			if (agent.MissionPeer != null)
			{
				this.PlayerPropertiesChanged(agent.MissionPeer.GetNetworkPeer());
			}
		}

		// Token: 0x060023C5 RID: 9157 RVA: 0x000845E3 File Offset: 0x000827E3
		public void BotPropertiesChanged(BattleSideEnum side)
		{
			if (this.OnBotPropertiesChanged != null)
			{
				this.OnBotPropertiesChanged(side);
			}
		}

		// Token: 0x060023C6 RID: 9158 RVA: 0x000845FC File Offset: 0x000827FC
		public void PlayerPropertiesChanged(NetworkCommunicator player)
		{
			if (GameNetwork.IsDedicatedServer)
			{
				return;
			}
			MissionPeer component = player.GetComponent<MissionPeer>();
			if (component != null)
			{
				this.PlayerPropertiesChanged(component);
			}
		}

		// Token: 0x060023C7 RID: 9159 RVA: 0x00084624 File Offset: 0x00082824
		public void PlayerPropertiesChanged(MissionPeer player)
		{
			if (GameNetwork.IsDedicatedServer)
			{
				return;
			}
			this.CalculateTotalNumbers();
			if (this.OnPlayerPropertiesChanged != null && player.Team != null && player.Team != Mission.Current.SpectatorTeam)
			{
				BattleSideEnum side = player.Team.Side;
				this.OnPlayerPropertiesChanged(side, player);
			}
		}

		// Token: 0x060023C8 RID: 9160 RVA: 0x0008467C File Offset: 0x0008287C
		protected override void HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			networkPeer.GetComponent<MissionPeer>();
			foreach (MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide in this._sides)
			{
				if (missionScoreboardSide != null && !networkPeer.IsServerPeer)
				{
					if (missionScoreboardSide.BotScores.IsAnyValid)
					{
						GameNetwork.BeginModuleEventAsServer(networkPeer);
						GameNetwork.WriteMessage(new BotData(missionScoreboardSide.Side, missionScoreboardSide.BotScores.KillCount, missionScoreboardSide.BotScores.AssistCount, missionScoreboardSide.BotScores.DeathCount, missionScoreboardSide.BotScores.AliveCount));
						GameNetwork.EndModuleEventAsServer();
					}
					if (this._mpGameModeBase != null)
					{
						int num = ((this._scoreboardSides != MissionScoreboardComponent.ScoreboardSides.OneSide) ? this._sides[0].SideScore : 0);
						GameNetwork.BeginModuleEventAsServer(networkPeer);
						GameNetwork.WriteMessage(new UpdateRoundScores(this._sides[1].SideScore, num));
						GameNetwork.EndModuleEventAsServer();
					}
				}
			}
			if (!networkPeer.IsServerPeer && this._mvpCountPerPeer != null)
			{
				foreach (ValueTuple<MissionPeer, int> valueTuple in this._mvpCountPerPeer)
				{
					GameNetwork.BeginModuleEventAsServer(networkPeer);
					GameNetwork.WriteMessage(new SetRoundMVP(valueTuple.Item1.GetNetworkPeer(), valueTuple.Item2));
					GameNetwork.EndModuleEventAsServer();
				}
			}
		}

		// Token: 0x060023C9 RID: 9161 RVA: 0x000847D4 File Offset: 0x000829D4
		public void HandleServerEventBotDataMessage(BotData message)
		{
			MissionScoreboardComponent.MissionScoreboardSide sideSafe = this.GetSideSafe(message.Side);
			sideSafe.BotScores.KillCount = message.KillCount;
			sideSafe.BotScores.AssistCount = message.AssistCount;
			sideSafe.BotScores.DeathCount = message.DeathCount;
			sideSafe.BotScores.AliveCount = message.AliveBotCount;
			this.BotPropertiesChanged(message.Side);
		}

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x060023CA RID: 9162 RVA: 0x0008483C File Offset: 0x00082A3C
		public BattleSideEnum RoundWinner
		{
			get
			{
				IRoundComponent roundComponent = this._mpGameModeBase.RoundComponent;
				if (roundComponent == null)
				{
					return BattleSideEnum.None;
				}
				return roundComponent.RoundWinner;
			}
		}

		// Token: 0x060023CB RID: 9163 RVA: 0x00084854 File Offset: 0x00082A54
		public void OnRoundEnding()
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				this.UpdateRoundScores();
			}
		}

		// Token: 0x060023CC RID: 9164 RVA: 0x00084863 File Offset: 0x00082A63
		public override void OnMissionRestart()
		{
			this.Clear();
			this.ClearScores();
		}

		// Token: 0x060023CD RID: 9165 RVA: 0x00084871 File Offset: 0x00082A71
		private void OnMyClientSynchronized()
		{
			this.LateInitializeHeaders();
		}

		// Token: 0x060023CE RID: 9166 RVA: 0x0008487C File Offset: 0x00082A7C
		private void LateInitScoreboard()
		{
			MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide = new MissionScoreboardComponent.MissionScoreboardSide(BattleSideEnum.Attacker);
			this._sides[1] = missionScoreboardSide;
			this._sides[1].BotScores = new BotData();
			if (this._scoreboardSides == MissionScoreboardComponent.ScoreboardSides.TwoSides)
			{
				MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide2 = new MissionScoreboardComponent.MissionScoreboardSide(BattleSideEnum.Defender);
				this._sides[0] = missionScoreboardSide2;
				this._sides[0].BotScores = new BotData();
			}
		}

		// Token: 0x060023CF RID: 9167 RVA: 0x000848D8 File Offset: 0x00082AD8
		private void LateInitializeHeaders()
		{
			if (this._isInitialized)
			{
				return;
			}
			this._isInitialized = true;
			foreach (MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide in this._sides)
			{
				if (missionScoreboardSide != null)
				{
					missionScoreboardSide.UpdateHeader(this.Headers);
				}
			}
			if (this.OnScoreboardInitialized != null)
			{
				this.OnScoreboardInitialized();
			}
		}

		// Token: 0x060023D0 RID: 9168 RVA: 0x00084930 File Offset: 0x00082B30
		public void OnMultiplayerGameClientBehaviorInitialized(ref Action<NetworkCommunicator> onBotsControlledChanged)
		{
			onBotsControlledChanged = (Action<NetworkCommunicator>)Delegate.Combine(onBotsControlledChanged, new Action<NetworkCommunicator>(this.BotsControlledChanged));
		}

		// Token: 0x060023D1 RID: 9169 RVA: 0x0008494C File Offset: 0x00082B4C
		public BattleSideEnum GetMatchWinnerSide()
		{
			List<int> scores = new List<int>();
			KeyValuePair<BattleSideEnum, int> keyValuePair = new KeyValuePair<BattleSideEnum, int>(BattleSideEnum.None, -1);
			for (int i = 0; i < 2; i++)
			{
				BattleSideEnum battleSideEnum = (BattleSideEnum)i;
				MissionScoreboardComponent.MissionScoreboardSide sideSafe = this.GetSideSafe(battleSideEnum);
				if (sideSafe.SideScore > keyValuePair.Value && sideSafe.CurrentPlayerCount > 0)
				{
					keyValuePair = new KeyValuePair<BattleSideEnum, int>(battleSideEnum, sideSafe.SideScore);
				}
				scores.Add(sideSafe.SideScore);
			}
			if (!scores.IsEmpty<int>() && scores.All((int s) => s == scores[0]))
			{
				return BattleSideEnum.None;
			}
			return keyValuePair.Key;
		}

		// Token: 0x060023D2 RID: 9170 RVA: 0x000849F4 File Offset: 0x00082BF4
		private void OnPreRoundEnding()
		{
			if (GameNetwork.IsServer)
			{
				KeyValuePair<MissionPeer, int> keyValuePair2;
				KeyValuePair<MissionPeer, int> keyValuePair4;
				foreach (MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide in this.Sides)
				{
					if (missionScoreboardSide.Side == BattleSideEnum.Attacker)
					{
						KeyValuePair<MissionPeer, int> keyValuePair = missionScoreboardSide.CalculateAndGetMVPScoreWithPeer();
						if (keyValuePair2.Key == null || keyValuePair2.Value < keyValuePair.Value)
						{
							keyValuePair2 = keyValuePair;
						}
					}
					else if (missionScoreboardSide.Side == BattleSideEnum.Defender)
					{
						KeyValuePair<MissionPeer, int> keyValuePair3 = missionScoreboardSide.CalculateAndGetMVPScoreWithPeer();
						if (keyValuePair4.Key == null || keyValuePair4.Value < keyValuePair3.Value)
						{
							keyValuePair4 = keyValuePair3;
						}
					}
				}
				if (keyValuePair2.Key != null)
				{
					this.SetPeerAsMVP(keyValuePair2.Key);
				}
				if (keyValuePair4.Key != null)
				{
					this.SetPeerAsMVP(keyValuePair4.Key);
				}
			}
		}

		// Token: 0x060023D3 RID: 9171 RVA: 0x00084AB0 File Offset: 0x00082CB0
		private void SetPeerAsMVP(MissionPeer peer)
		{
			int num = -1;
			for (int i = 0; i < this._mvpCountPerPeer.Count; i++)
			{
				if (peer == this._mvpCountPerPeer[i].Item1)
				{
					num = i;
					break;
				}
			}
			int num2 = 1;
			if (num != -1)
			{
				num2 = this._mvpCountPerPeer[num].Item2 + 1;
				this._mvpCountPerPeer.RemoveAt(num);
			}
			this._mvpCountPerPeer.Add(new ValueTuple<MissionPeer, int>(peer, num2));
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new SetRoundMVP(peer.GetNetworkPeer(), num2));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			Action<MissionPeer, int> onMVPSelected = this.OnMVPSelected;
			if (onMVPSelected == null)
			{
				return;
			}
			onMVPSelected(peer, num2);
		}

		// Token: 0x060023D4 RID: 9172 RVA: 0x00084B54 File Offset: 0x00082D54
		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			if (GameNetwork.IsServer && !isBlocked && damagedHp > 0f)
			{
				if (affectorAgent.IsMount)
				{
					affectorAgent = affectorAgent.RiderAgent;
				}
				if (affectorAgent != null)
				{
					MissionPeer missionPeer = affectorAgent.MissionPeer ?? ((affectorAgent.IsAIControlled && affectorAgent.OwningAgentMissionPeer != null) ? affectorAgent.OwningAgentMissionPeer : null);
					if (missionPeer != null)
					{
						int num = (int)damagedHp;
						if (affectedAgent.IsMount)
						{
							num = (int)(damagedHp * 0.35f);
							affectedAgent = affectedAgent.RiderAgent;
						}
						if (affectedAgent != null && affectorAgent != affectedAgent)
						{
							if (!affectorAgent.IsFriendOf(affectedAgent))
							{
								missionPeer.Score += num;
							}
							else
							{
								missionPeer.Score -= (int)((float)num * 1.5f);
							}
							GameNetwork.BeginBroadcastModuleEvent();
							GameNetwork.WriteMessage(new KillDeathCountChange(missionPeer.GetNetworkPeer(), null, missionPeer.KillCount, missionPeer.AssistCount, missionPeer.DeathCount, missionPeer.Score));
							GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
						}
					}
				}
			}
		}

		// Token: 0x04000D0A RID: 3338
		private const int TotalSideCount = 2;

		// Token: 0x04000D0B RID: 3339
		private MissionLobbyComponent _missionLobbyComponent;

		// Token: 0x04000D0C RID: 3340
		private MissionNetworkComponent _missionNetworkComponent;

		// Token: 0x04000D0D RID: 3341
		private MissionMultiplayerGameModeBaseClient _mpGameModeBase;

		// Token: 0x04000D0E RID: 3342
		private IScoreboardData _scoreboardData;

		// Token: 0x04000D15 RID: 3349
		private List<MissionPeer> _spectators;

		// Token: 0x04000D16 RID: 3350
		private MissionScoreboardComponent.MissionScoreboardSide[] _sides;

		// Token: 0x04000D17 RID: 3351
		private bool _isInitialized;

		// Token: 0x04000D18 RID: 3352
		private List<BattleSideEnum> _roundWinnerList;

		// Token: 0x04000D19 RID: 3353
		private MissionScoreboardComponent.ScoreboardSides _scoreboardSides;

		// Token: 0x04000D1A RID: 3354
		private List<ValueTuple<MissionPeer, int>> _mvpCountPerPeer;

		// Token: 0x020005AA RID: 1450
		private enum ScoreboardSides
		{
			// Token: 0x04001DD2 RID: 7634
			OneSide,
			// Token: 0x04001DD3 RID: 7635
			TwoSides
		}

		// Token: 0x020005AB RID: 1451
		public struct ScoreboardHeader
		{
			// Token: 0x06003B66 RID: 15206 RVA: 0x000EF41C File Offset: 0x000ED61C
			public ScoreboardHeader(string id, Func<MissionPeer, string> playerGetterFunc, Func<BotData, string> botGetterFunc)
			{
				this.Id = id;
				this.Name = GameTexts.FindText("str_scoreboard_header", id);
				this._playerGetterFunc = playerGetterFunc;
				this._botGetterFunc = botGetterFunc;
			}

			// Token: 0x06003B67 RID: 15207 RVA: 0x000EF444 File Offset: 0x000ED644
			public string GetValueOf(MissionPeer missionPeer)
			{
				if (this._playerGetterFunc == null)
				{
					return "";
				}
				return this._playerGetterFunc(missionPeer);
			}

			// Token: 0x06003B68 RID: 15208 RVA: 0x000EF460 File Offset: 0x000ED660
			public string GetValueOf(BotData botData)
			{
				if (this._botGetterFunc == null)
				{
					return "";
				}
				return this._botGetterFunc(botData);
			}

			// Token: 0x04001DD4 RID: 7636
			private readonly Func<MissionPeer, string> _playerGetterFunc;

			// Token: 0x04001DD5 RID: 7637
			private readonly Func<BotData, string> _botGetterFunc;

			// Token: 0x04001DD6 RID: 7638
			public readonly string Id;

			// Token: 0x04001DD7 RID: 7639
			public readonly TextObject Name;
		}

		// Token: 0x020005AC RID: 1452
		public class MissionScoreboardSide
		{
			// Token: 0x170009A0 RID: 2464
			// (get) Token: 0x06003B69 RID: 15209 RVA: 0x000EF47C File Offset: 0x000ED67C
			public int CurrentPlayerCount
			{
				get
				{
					return this._players.Count;
				}
			}

			// Token: 0x170009A1 RID: 2465
			// (get) Token: 0x06003B6A RID: 15210 RVA: 0x000EF489 File Offset: 0x000ED689
			public IEnumerable<MissionPeer> Players
			{
				get
				{
					return this._players;
				}
			}

			// Token: 0x06003B6B RID: 15211 RVA: 0x000EF491 File Offset: 0x000ED691
			public MissionScoreboardSide(BattleSideEnum side)
			{
				this.Side = side;
				this._players = new List<MissionPeer>();
				this._playerLastRoundScoreMap = new List<int>();
			}

			// Token: 0x06003B6C RID: 15212 RVA: 0x000EF4B6 File Offset: 0x000ED6B6
			public void AddPlayer(MissionPeer peer)
			{
				if (!this._players.Contains(peer))
				{
					this._players.Add(peer);
					this._playerLastRoundScoreMap.Add(0);
				}
			}

			// Token: 0x06003B6D RID: 15213 RVA: 0x000EF4E0 File Offset: 0x000ED6E0
			public void RemovePlayer(MissionPeer peer)
			{
				for (int i = 0; i < this._players.Count; i++)
				{
					if (this._players[i] == peer)
					{
						this._players.RemoveAt(i);
						this._playerLastRoundScoreMap.RemoveAt(i);
						return;
					}
				}
			}

			// Token: 0x06003B6E RID: 15214 RVA: 0x000EF52C File Offset: 0x000ED72C
			public string[] GetValuesOf(MissionPeer peer)
			{
				if (this._properties == null)
				{
					return new string[0];
				}
				string[] array = new string[this._properties.Length];
				if (peer == null)
				{
					for (int i = 0; i < this._properties.Length; i++)
					{
						array[i] = this._properties[i].GetValueOf(this.BotScores);
					}
					return array;
				}
				for (int j = 0; j < this._properties.Length; j++)
				{
					array[j] = this._properties[j].GetValueOf(peer);
				}
				return array;
			}

			// Token: 0x06003B6F RID: 15215 RVA: 0x000EF5B4 File Offset: 0x000ED7B4
			public string[] GetHeaderNames()
			{
				if (this._properties == null)
				{
					return new string[0];
				}
				string[] array = new string[this._properties.Length];
				for (int i = 0; i < this._properties.Length; i++)
				{
					array[i] = this._properties[i].Name.ToString();
				}
				return array;
			}

			// Token: 0x06003B70 RID: 15216 RVA: 0x000EF60C File Offset: 0x000ED80C
			public string[] GetHeaderIds()
			{
				if (this._properties == null)
				{
					return new string[0];
				}
				string[] array = new string[this._properties.Length];
				for (int i = 0; i < this._properties.Length; i++)
				{
					array[i] = this._properties[i].Id;
				}
				return array;
			}

			// Token: 0x06003B71 RID: 15217 RVA: 0x000EF660 File Offset: 0x000ED860
			public int GetScore(MissionPeer peer)
			{
				if (this._properties == null)
				{
					return 0;
				}
				string text;
				if (peer == null)
				{
					if (this._properties.Any((MissionScoreboardComponent.ScoreboardHeader p) => p.Id == "score"))
					{
						text = this._properties.FirstOrDefault((MissionScoreboardComponent.ScoreboardHeader x) => x.Id == "score").GetValueOf(this.BotScores);
					}
					else
					{
						text = string.Empty;
					}
				}
				else if (this._properties.Any((MissionScoreboardComponent.ScoreboardHeader p) => p.Id == "score"))
				{
					text = this._properties.Single((MissionScoreboardComponent.ScoreboardHeader x) => x.Id == "score").GetValueOf(peer);
				}
				else
				{
					text = string.Empty;
				}
				int num = 0;
				int.TryParse(text, out num);
				return num;
			}

			// Token: 0x06003B72 RID: 15218 RVA: 0x000EF75D File Offset: 0x000ED95D
			public void UpdateHeader(MissionScoreboardComponent.ScoreboardHeader[] headers)
			{
				this._properties = headers;
			}

			// Token: 0x06003B73 RID: 15219 RVA: 0x000EF766 File Offset: 0x000ED966
			public void Clear()
			{
				this._players.Clear();
			}

			// Token: 0x06003B74 RID: 15220 RVA: 0x000EF774 File Offset: 0x000ED974
			public KeyValuePair<MissionPeer, int> CalculateAndGetMVPScoreWithPeer()
			{
				KeyValuePair<MissionPeer, int> keyValuePair = default(KeyValuePair<MissionPeer, int>);
				for (int i = 0; i < this._players.Count; i++)
				{
					int num = this._players[i].Score - this._playerLastRoundScoreMap[i];
					this._playerLastRoundScoreMap[i] = this._players[i].Score;
					if (keyValuePair.Key == null || keyValuePair.Value < num)
					{
						keyValuePair = new KeyValuePair<MissionPeer, int>(this._players[i], num);
					}
				}
				return keyValuePair;
			}

			// Token: 0x04001DD8 RID: 7640
			public readonly BattleSideEnum Side;

			// Token: 0x04001DD9 RID: 7641
			private MissionScoreboardComponent.ScoreboardHeader[] _properties;

			// Token: 0x04001DDA RID: 7642
			public BotData BotScores;

			// Token: 0x04001DDB RID: 7643
			public int SideScore;

			// Token: 0x04001DDC RID: 7644
			private List<MissionPeer> _players;

			// Token: 0x04001DDD RID: 7645
			private List<int> _playerLastRoundScoreMap;
		}
	}
}
