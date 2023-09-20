using System;
using System.Linq;
using Helpers;
using SandBox.BoardGames.AI;
using SandBox.Conversation;
using SandBox.Conversation.MissionLogics;
using SandBox.Objects.Usables;
using SandBox.Source.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;

namespace SandBox.BoardGames.MissionLogics
{
	// Token: 0x020000C1 RID: 193
	public class MissionBoardGameLogic : MissionLogic
	{
		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000B88 RID: 2952 RVA: 0x0005C5AC File Offset: 0x0005A7AC
		// (remove) Token: 0x06000B89 RID: 2953 RVA: 0x0005C5E4 File Offset: 0x0005A7E4
		public event Action GameStarted;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06000B8A RID: 2954 RVA: 0x0005C61C File Offset: 0x0005A81C
		// (remove) Token: 0x06000B8B RID: 2955 RVA: 0x0005C654 File Offset: 0x0005A854
		public event Action GameEnded;

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000B8C RID: 2956 RVA: 0x0005C689 File Offset: 0x0005A889
		// (set) Token: 0x06000B8D RID: 2957 RVA: 0x0005C691 File Offset: 0x0005A891
		public BoardGameBase Board { get; private set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000B8E RID: 2958 RVA: 0x0005C69A File Offset: 0x0005A89A
		// (set) Token: 0x06000B8F RID: 2959 RVA: 0x0005C6A2 File Offset: 0x0005A8A2
		public BoardGameAIBase AIOpponent { get; private set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000B90 RID: 2960 RVA: 0x0005C6AB File Offset: 0x0005A8AB
		public bool IsOpposingAgentMovingToPlayingChair
		{
			get
			{
				return BoardGameAgentBehavior.IsAgentMovingToChair(this.OpposingAgent);
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000B91 RID: 2961 RVA: 0x0005C6B8 File Offset: 0x0005A8B8
		// (set) Token: 0x06000B92 RID: 2962 RVA: 0x0005C6C0 File Offset: 0x0005A8C0
		public bool IsGameInProgress { get; private set; }

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000B93 RID: 2963 RVA: 0x0005C6C9 File Offset: 0x0005A8C9
		public BoardGameHelper.BoardGameState BoardGameFinalState
		{
			get
			{
				return this._boardGameState;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000B94 RID: 2964 RVA: 0x0005C6D1 File Offset: 0x0005A8D1
		// (set) Token: 0x06000B95 RID: 2965 RVA: 0x0005C6D9 File Offset: 0x0005A8D9
		public CultureObject.BoardGameType CurrentBoardGame { get; private set; }

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000B96 RID: 2966 RVA: 0x0005C6E2 File Offset: 0x0005A8E2
		// (set) Token: 0x06000B97 RID: 2967 RVA: 0x0005C6EA File Offset: 0x0005A8EA
		public BoardGameHelper.AIDifficulty Difficulty { get; private set; }

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000B98 RID: 2968 RVA: 0x0005C6F3 File Offset: 0x0005A8F3
		// (set) Token: 0x06000B99 RID: 2969 RVA: 0x0005C6FB File Offset: 0x0005A8FB
		public int BetAmount { get; private set; }

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000B9A RID: 2970 RVA: 0x0005C704 File Offset: 0x0005A904
		// (set) Token: 0x06000B9B RID: 2971 RVA: 0x0005C70C File Offset: 0x0005A90C
		public Agent OpposingAgent { get; private set; }

		// Token: 0x06000B9C RID: 2972 RVA: 0x0005C718 File Offset: 0x0005A918
		public override void AfterStart()
		{
			base.AfterStart();
			this._opposingChair = MBExtensions.CollectObjects<Chair>(base.Mission.Scene.FindEntityWithTag("gambler_npc")).FirstOrDefault<Chair>();
			this._playerChair = MBExtensions.CollectObjects<Chair>(base.Mission.Scene.FindEntityWithTag("gambler_player")).FirstOrDefault<Chair>();
			foreach (StandingPoint standingPoint in this._opposingChair.StandingPoints)
			{
				standingPoint.IsDisabledForPlayers = true;
			}
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x0005C7C0 File Offset: 0x0005A9C0
		public void SetStartingPlayer(bool playerOneStarts)
		{
			this._startingPlayer = (playerOneStarts ? PlayerTurn.PlayerOne : PlayerTurn.PlayerTwo);
		}

		// Token: 0x06000B9E RID: 2974 RVA: 0x0005C7CF File Offset: 0x0005A9CF
		public void StartBoardGame()
		{
			this._startingBoardGame = true;
		}

		// Token: 0x06000B9F RID: 2975 RVA: 0x0005C7D8 File Offset: 0x0005A9D8
		private void BoardGameInit(CultureObject.BoardGameType game)
		{
			if (this.Board == null)
			{
				switch (game)
				{
				case 0:
					this.Board = new BoardGameSeega(this, this._startingPlayer);
					this.AIOpponent = new BoardGameAISeega(this.Difficulty, this);
					break;
				case 1:
					this.Board = new BoardGamePuluc(this, this._startingPlayer);
					this.AIOpponent = new BoardGameAIPuluc(this.Difficulty, this);
					break;
				case 2:
					this.Board = new BoardGameKonane(this, this._startingPlayer);
					this.AIOpponent = new BoardGameAIKonane(this.Difficulty, this);
					break;
				case 3:
					this.Board = new BoardGameMuTorere(this, this._startingPlayer);
					this.AIOpponent = new BoardGameAIMuTorere(this.Difficulty, this);
					break;
				case 4:
					this.Board = new BoardGameTablut(this, this._startingPlayer);
					this.AIOpponent = new BoardGameAITablut(this.Difficulty, this);
					break;
				case 5:
					this.Board = new BoardGameBaghChal(this, this._startingPlayer);
					this.AIOpponent = new BoardGameAIBaghChal(this.Difficulty, this);
					break;
				default:
					Debug.FailedAssert("[DEBUG]No board with this name was found.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\MissionLogics\\MissionBoardGameLogic.cs", "BoardGameInit", 122);
					break;
				}
				this.Board.Initialize();
				if (this.AIOpponent != null)
				{
					this.AIOpponent.Initialize();
				}
			}
			else
			{
				this.Board.SetStartingPlayer(this._startingPlayer);
				this.Board.InitializeUnits();
				this.Board.InitializeCapturedUnitsZones();
				this.Board.Reset();
				if (this.AIOpponent != null)
				{
					this.AIOpponent.SetDifficulty(this.Difficulty);
					this.AIOpponent.Initialize();
				}
			}
			if (this.Handler != null)
			{
				this.Handler.Install();
			}
			this._boardGameState = 0;
			this.IsGameInProgress = true;
			this._isTavernGame = CampaignMission.Current.Location == Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("tavern");
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x0005C9D4 File Offset: 0x0005ABD4
		public override void OnMissionTick(float dt)
		{
			if (base.Mission.IsInPhotoMode)
			{
				return;
			}
			if (this._startingBoardGame)
			{
				this._startingBoardGame = false;
				this.BoardGameInit(this.CurrentBoardGame);
				Action gameStarted = this.GameStarted;
				if (gameStarted == null)
				{
					return;
				}
				gameStarted();
				return;
			}
			else
			{
				if (this.IsGameInProgress)
				{
					this.Board.Tick(dt);
					return;
				}
				if (this.OpposingAgent != null && this.OpposingAgent.IsHero && Hero.OneToOneConversationHero == null && this.CheckIfBothSidesAreSitting())
				{
					this.StartBoardGame();
				}
				return;
			}
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x0005CA5C File Offset: 0x0005AC5C
		public void DetectOpposingAgent()
		{
			foreach (Agent agent in Mission.Current.Agents)
			{
				if (agent == ConversationMission.OneToOneConversationAgent)
				{
					this.OpposingAgent = agent;
					if (agent.IsHero)
					{
						BoardGameAgentBehavior.AddTargetChair(this.OpposingAgent, this._opposingChair);
					}
					AgentNavigator agentNavigator = this.OpposingAgent.GetComponent<CampaignAgentComponent>().AgentNavigator;
					this._specialTagCacheOfOpposingHero = agentNavigator.SpecialTargetTag;
					agentNavigator.SpecialTargetTag = "gambler_npc";
					break;
				}
			}
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x0005CB00 File Offset: 0x0005AD00
		public bool CheckIfBothSidesAreSitting()
		{
			return Agent.Main != null && this.OpposingAgent != null && this._playerChair.IsAgentFullySitting(Agent.Main) && this._opposingChair.IsAgentFullySitting(this.OpposingAgent);
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x0005CB38 File Offset: 0x0005AD38
		public void PlayerOneWon(string message = "str_boardgame_victory_message")
		{
			Agent opposingAgent = this.OpposingAgent;
			this.SetGameOver(GameOverEnum.PlayerOneWon);
			this.ShowInquiry(message, opposingAgent);
		}

		// Token: 0x06000BA4 RID: 2980 RVA: 0x0005CB5C File Offset: 0x0005AD5C
		public void PlayerTwoWon(string message = "str_boardgame_defeat_message")
		{
			Agent opposingAgent = this.OpposingAgent;
			this.SetGameOver(GameOverEnum.PlayerTwoWon);
			this.ShowInquiry(message, opposingAgent);
		}

		// Token: 0x06000BA5 RID: 2981 RVA: 0x0005CB80 File Offset: 0x0005AD80
		public void GameWasDraw(string message = "str_boardgame_draw_message")
		{
			Agent opposingAgent = this.OpposingAgent;
			this.SetGameOver(GameOverEnum.Draw);
			this.ShowInquiry(message, opposingAgent);
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x0005CBA4 File Offset: 0x0005ADA4
		private void ShowInquiry(string message, Agent conversationAgent)
		{
			InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_boardgame", null).ToString(), GameTexts.FindText(message, null).ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), "", delegate
			{
				this.StartConversationWithOpponentAfterGameEnd(conversationAgent);
			}, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x0005CC1E File Offset: 0x0005AE1E
		private void StartConversationWithOpponentAfterGameEnd(Agent conversationAgent)
		{
			MissionConversationLogic.Current.StartConversation(conversationAgent, false, false);
			this._boardGameState = 0;
		}

		// Token: 0x06000BA8 RID: 2984 RVA: 0x0005CC34 File Offset: 0x0005AE34
		public void SetGameOver(GameOverEnum gameOverInfo)
		{
			base.Mission.MainAgent.ClearTargetFrame();
			if (this.Handler != null && gameOverInfo != GameOverEnum.PlayerCanceledTheGame)
			{
				this.Handler.Uninstall();
			}
			Hero hero = (this.OpposingAgent.IsHero ? ((CharacterObject)this.OpposingAgent.Character).HeroObject : null);
			switch (gameOverInfo)
			{
			case GameOverEnum.PlayerOneWon:
				this._boardGameState = 1;
				break;
			case GameOverEnum.PlayerTwoWon:
				this._boardGameState = 2;
				break;
			case GameOverEnum.Draw:
				this._boardGameState = 3;
				break;
			case GameOverEnum.PlayerCanceledTheGame:
				this._boardGameState = 0;
				break;
			}
			if (gameOverInfo != GameOverEnum.PlayerCanceledTheGame)
			{
				CampaignEventDispatcher.Instance.OnPlayerBoardGameOver(hero, this._boardGameState);
			}
			Action gameEnded = this.GameEnded;
			if (gameEnded != null)
			{
				gameEnded();
			}
			BoardGameAgentBehavior.RemoveBoardGameBehaviorOfAgent(this.OpposingAgent);
			this.OpposingAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SpecialTargetTag = this._specialTagCacheOfOpposingHero;
			this.OpposingAgent = null;
			this.IsGameInProgress = false;
			BoardGameAIBase aiopponent = this.AIOpponent;
			if (aiopponent == null)
			{
				return;
			}
			aiopponent.OnSetGameOver();
		}

		// Token: 0x06000BA9 RID: 2985 RVA: 0x0005CD38 File Offset: 0x0005AF38
		public void ForfeitGame()
		{
			this.Board.SetGameOverInfo(GameOverEnum.PlayerTwoWon);
			Agent opposingAgent = this.OpposingAgent;
			this.SetGameOver(this.Board.GameOverInfo);
			this.StartConversationWithOpponentAfterGameEnd(opposingAgent);
		}

		// Token: 0x06000BAA RID: 2986 RVA: 0x0005CD70 File Offset: 0x0005AF70
		public void AIForfeitGame()
		{
			this.Board.SetGameOverInfo(GameOverEnum.PlayerOneWon);
			this.SetGameOver(this.Board.GameOverInfo);
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x0005CD8F File Offset: 0x0005AF8F
		public void RollDice()
		{
			this.Board.RollDice();
		}

		// Token: 0x06000BAC RID: 2988 RVA: 0x0005CD9C File Offset: 0x0005AF9C
		public bool RequiresDiceRolling()
		{
			switch (this.CurrentBoardGame)
			{
			case 0:
				return false;
			case 1:
				return true;
			case 2:
				return false;
			case 3:
				return false;
			case 4:
				return false;
			case 5:
				return false;
			default:
				return false;
			}
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x0005CDDD File Offset: 0x0005AFDD
		public void SetBetAmount(int bet)
		{
			this.BetAmount = bet;
		}

		// Token: 0x06000BAE RID: 2990 RVA: 0x0005CDE6 File Offset: 0x0005AFE6
		public void SetCurrentDifficulty(BoardGameHelper.AIDifficulty difficulty)
		{
			this.Difficulty = difficulty;
		}

		// Token: 0x06000BAF RID: 2991 RVA: 0x0005CDEF File Offset: 0x0005AFEF
		public void SetBoardGame(CultureObject.BoardGameType game)
		{
			this.CurrentBoardGame = game;
		}

		// Token: 0x06000BB0 RID: 2992 RVA: 0x0005CDF8 File Offset: 0x0005AFF8
		public override InquiryData OnEndMissionRequest(out bool canLeave)
		{
			canLeave = true;
			return null;
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x0005CE00 File Offset: 0x0005B000
		public static bool IsBoardGameAvailable()
		{
			Mission mission = Mission.Current;
			MissionBoardGameLogic missionBoardGameLogic = ((mission != null) ? mission.GetMissionBehavior<MissionBoardGameLogic>() : null);
			Mission mission2 = Mission.Current;
			return ((mission2 != null) ? mission2.Scene : null) != null && missionBoardGameLogic != null && Mission.Current.Scene.FindEntityWithTag("boardgame") != null && missionBoardGameLogic.OpposingAgent == null;
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x0005CE64 File Offset: 0x0005B064
		public static bool IsThereActiveBoardGameWithHero(Hero hero)
		{
			Mission mission = Mission.Current;
			MissionBoardGameLogic missionBoardGameLogic = ((mission != null) ? mission.GetMissionBehavior<MissionBoardGameLogic>() : null);
			Mission mission2 = Mission.Current;
			if (((mission2 != null) ? mission2.Scene : null) != null && Mission.Current.Scene.FindEntityWithTag("boardgame") != null && missionBoardGameLogic != null)
			{
				Agent opposingAgent = missionBoardGameLogic.OpposingAgent;
				return ((opposingAgent != null) ? opposingAgent.Character : null) == hero.CharacterObject;
			}
			return false;
		}

		// Token: 0x04000416 RID: 1046
		private const string BoardGameEntityTag = "boardgame";

		// Token: 0x04000417 RID: 1047
		private const string SpecialTargetGamblerNpcTag = "gambler_npc";

		// Token: 0x0400041A RID: 1050
		public IBoardGameHandler Handler;

		// Token: 0x0400041B RID: 1051
		private PlayerTurn _startingPlayer = PlayerTurn.PlayerTwo;

		// Token: 0x0400041C RID: 1052
		private Chair _playerChair;

		// Token: 0x0400041D RID: 1053
		private Chair _opposingChair;

		// Token: 0x0400041E RID: 1054
		private string _specialTagCacheOfOpposingHero;

		// Token: 0x0400041F RID: 1055
		private bool _isTavernGame;

		// Token: 0x04000420 RID: 1056
		private bool _startingBoardGame;

		// Token: 0x04000421 RID: 1057
		private BoardGameHelper.BoardGameState _boardGameState;
	}
}
