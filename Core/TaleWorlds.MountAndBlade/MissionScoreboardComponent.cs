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
	public class MissionScoreboardComponent : MissionNetwork
	{
		public event Action OnRoundPropertiesChanged;

		public event Action<BattleSideEnum> OnBotPropertiesChanged;

		public event Action<Team, Team, MissionPeer> OnPlayerSideChanged;

		public event Action<BattleSideEnum, MissionPeer> OnPlayerPropertiesChanged;

		public event Action<MissionPeer, int> OnMVPSelected;

		public event Action OnScoreboardInitialized;

		public bool IsOneSided
		{
			get
			{
				return this._scoreboardSides == MissionScoreboardComponent.ScoreboardSides.OneSide;
			}
		}

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

		public MissionScoreboardComponent.ScoreboardHeader[] Headers
		{
			get
			{
				return this._scoreboardData.GetScoreboardHeaders();
			}
		}

		public MissionScoreboardComponent(IScoreboardData scoreboardData)
		{
			this._scoreboardData = scoreboardData;
			this._spectators = new List<MissionPeer>();
			this._sides = new MissionScoreboardComponent.MissionScoreboardSide[2];
			this._roundWinnerList = new List<BattleSideEnum>();
			this._mvpCountPerPeer = new List<ValueTuple<MissionPeer, int>>();
		}

		public IEnumerable<BattleSideEnum> RoundWinnerList
		{
			get
			{
				return this._roundWinnerList.AsReadOnly();
			}
		}

		public MissionScoreboardComponent.MissionScoreboardSide[] Sides
		{
			get
			{
				return this._sides;
			}
		}

		public List<MissionPeer> Spectators
		{
			get
			{
				return this._spectators;
			}
		}

		public override void AfterStart()
		{
			this._spectators.Clear();
			this._missionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._missionNetworkComponent = base.Mission.GetMissionBehavior<MissionNetworkComponent>();
			this._mpGameModeBase = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			if (this._missionLobbyComponent.MissionType == MultiplayerGameType.FreeForAll || this._missionLobbyComponent.MissionType == MultiplayerGameType.Duel)
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

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.RegisterBaseHandler<UpdateRoundScores>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerUpdateRoundScoresMessage));
				registerer.RegisterBaseHandler<SetRoundMVP>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerSetRoundMVP));
				registerer.RegisterBaseHandler<BotData>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventBotDataMessage));
			}
		}

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

		public MissionScoreboardComponent.MissionScoreboardSide GetSideSafe(BattleSideEnum battleSide)
		{
			if (this._scoreboardSides == MissionScoreboardComponent.ScoreboardSides.OneSide)
			{
				return this._sides[1];
			}
			return this._sides[(int)battleSide];
		}

		public int GetRoundScore(BattleSideEnum side)
		{
			if (side > (BattleSideEnum)this._sides.Length || side < BattleSideEnum.Defender)
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MissionScoreboardComponent.cs", "GetRoundScore", 432);
				return 0;
			}
			return this.GetSideSafe(side).SideScore;
		}

		public void HandleServerUpdateRoundScoresMessage(GameNetworkMessage baseMessage)
		{
			UpdateRoundScores updateRoundScores = (UpdateRoundScores)baseMessage;
			this._sides[1].SideScore = updateRoundScores.AttackerTeamScore;
			if (this._scoreboardSides != MissionScoreboardComponent.ScoreboardSides.OneSide)
			{
				this._sides[0].SideScore = updateRoundScores.DefenderTeamScore;
			}
			if (this.OnRoundPropertiesChanged != null)
			{
				this.OnRoundPropertiesChanged();
			}
		}

		public void HandleServerSetRoundMVP(GameNetworkMessage baseMessage)
		{
			SetRoundMVP setRoundMVP = (SetRoundMVP)baseMessage;
			Action<MissionPeer, int> onMVPSelected = this.OnMVPSelected;
			if (onMVPSelected != null)
			{
				onMVPSelected(setRoundMVP.MVPPeer.GetComponent<MissionPeer>(), setRoundMVP.MVPCount);
			}
			this.PlayerPropertiesChanged(setRoundMVP.MVPPeer);
		}

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

		private void TeamChange(NetworkCommunicator player, Team oldTeam, Team nextTeam)
		{
			if (oldTeam == null && GameNetwork.VirtualPlayers[player.VirtualPlayer.Index] != player.VirtualPlayer)
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

		public override void OnClearScene()
		{
			if (this._mpGameModeBase.RoundComponent == null && GameNetwork.IsServer)
			{
				this.ClearSideScores();
			}
			foreach (MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide in this.Sides)
			{
				if (missionScoreboardSide != null)
				{
					missionScoreboardSide.BotScores.AliveCount = 0;
				}
			}
		}

		public override void OnPlayerConnectedToServer(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (component != null && component.Team != null)
			{
				this.TeamChange(networkPeer, null, component.Team);
			}
		}

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

		private void BotsControlledChanged(NetworkCommunicator peer)
		{
			this.PlayerPropertiesChanged(peer);
		}

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

		public override void OnAssignPlayerAsSergeantOfFormation(Agent agent)
		{
			if (agent.MissionPeer != null)
			{
				this.PlayerPropertiesChanged(agent.MissionPeer.GetNetworkPeer());
			}
		}

		public void BotPropertiesChanged(BattleSideEnum side)
		{
			if (this.OnBotPropertiesChanged != null)
			{
				this.OnBotPropertiesChanged(side);
			}
		}

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

		public void HandleServerEventBotDataMessage(GameNetworkMessage baseMessage)
		{
			BotData botData = (BotData)baseMessage;
			MissionScoreboardComponent.MissionScoreboardSide sideSafe = this.GetSideSafe(botData.Side);
			sideSafe.BotScores.KillCount = botData.KillCount;
			sideSafe.BotScores.AssistCount = botData.AssistCount;
			sideSafe.BotScores.DeathCount = botData.DeathCount;
			sideSafe.BotScores.AliveCount = botData.AliveBotCount;
			this.BotPropertiesChanged(botData.Side);
		}

		private void ClearSideScores()
		{
			this._sides[1].SideScore = 0;
			if (this._scoreboardSides == MissionScoreboardComponent.ScoreboardSides.TwoSides)
			{
				this._sides[0].SideScore = 0;
			}
			if (GameNetwork.IsServer)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new UpdateRoundScores(0, 0));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
			if (this.OnRoundPropertiesChanged != null)
			{
				this.OnRoundPropertiesChanged();
			}
		}

		public void OnRoundEnding()
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				this.UpdateRoundScores();
			}
		}

		private void OnMyClientSynchronized()
		{
			this.LateInitializeHeaders();
		}

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

		public void OnMultiplayerGameClientBehaviorInitialized(ref Action<NetworkCommunicator> onBotsControlledChanged)
		{
			onBotsControlledChanged = (Action<NetworkCommunicator>)Delegate.Combine(onBotsControlledChanged, new Action<NetworkCommunicator>(this.BotsControlledChanged));
		}

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

		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			if (affectorAgent != null && GameNetwork.IsServer && !isBlocked && damagedHp > 0f)
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

		private const int TotalSideCount = 2;

		private MissionLobbyComponent _missionLobbyComponent;

		private MissionNetworkComponent _missionNetworkComponent;

		private MissionMultiplayerGameModeBaseClient _mpGameModeBase;

		private IScoreboardData _scoreboardData;

		private List<MissionPeer> _spectators;

		private MissionScoreboardComponent.MissionScoreboardSide[] _sides;

		private bool _isInitialized;

		private List<BattleSideEnum> _roundWinnerList;

		private MissionScoreboardComponent.ScoreboardSides _scoreboardSides;

		private List<ValueTuple<MissionPeer, int>> _mvpCountPerPeer;

		private enum ScoreboardSides
		{
			OneSide,
			TwoSides
		}

		public struct ScoreboardHeader
		{
			public ScoreboardHeader(string id, Func<MissionPeer, string> playerGetterFunc, Func<BotData, string> botGetterFunc)
			{
				this.Id = id;
				this.Name = GameTexts.FindText("str_scoreboard_header", id);
				this._playerGetterFunc = playerGetterFunc;
				this._botGetterFunc = botGetterFunc;
			}

			public string GetValueOf(MissionPeer missionPeer)
			{
				if (this._playerGetterFunc == null)
				{
					return "";
				}
				return this._playerGetterFunc(missionPeer);
			}

			public string GetValueOf(BotData botData)
			{
				if (this._botGetterFunc == null)
				{
					return "";
				}
				return this._botGetterFunc(botData);
			}

			private readonly Func<MissionPeer, string> _playerGetterFunc;

			private readonly Func<BotData, string> _botGetterFunc;

			public readonly string Id;

			public readonly TextObject Name;
		}

		public class MissionScoreboardSide
		{
			public int CurrentPlayerCount
			{
				get
				{
					return this._players.Count;
				}
			}

			public IEnumerable<MissionPeer> Players
			{
				get
				{
					return this._players;
				}
			}

			public MissionScoreboardSide(BattleSideEnum side)
			{
				this.Side = side;
				this._players = new List<MissionPeer>();
				this._playerLastRoundScoreMap = new List<int>();
			}

			public void AddPlayer(MissionPeer peer)
			{
				if (!this._players.Contains(peer))
				{
					this._players.Add(peer);
					this._playerLastRoundScoreMap.Add(0);
				}
			}

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

			public void UpdateHeader(MissionScoreboardComponent.ScoreboardHeader[] headers)
			{
				this._properties = headers;
			}

			public void Clear()
			{
				this._players.Clear();
			}

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

			public readonly BattleSideEnum Side;

			private MissionScoreboardComponent.ScoreboardHeader[] _properties;

			public BotData BotScores;

			public int SideScore;

			private List<MissionPeer> _players;

			private List<int> _playerLastRoundScoreMap;
		}
	}
}
