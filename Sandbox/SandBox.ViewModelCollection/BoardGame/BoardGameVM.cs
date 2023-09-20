using System;
using SandBox.BoardGames;
using SandBox.BoardGames.MissionLogics;
using SandBox.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.BoardGame
{
	// Token: 0x0200003D RID: 61
	public class BoardGameVM : ViewModel
	{
		// Token: 0x06000450 RID: 1104 RVA: 0x000130B4 File Offset: 0x000112B4
		public BoardGameVM()
		{
			this._missionBoardGameHandler = Mission.Current.GetMissionBehavior<MissionBoardGameLogic>();
			this.BoardGameType = this._missionBoardGameHandler.CurrentBoardGame.ToString();
			this.IsGameUsingDice = this._missionBoardGameHandler.RequiresDiceRolling();
			this.DiceResult = "-";
			this.Instructions = new BoardGameInstructionsVM(this._missionBoardGameHandler.CurrentBoardGame);
			this.RefreshValues();
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x00013130 File Offset: 0x00011330
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RollDiceText = GameTexts.FindText("str_roll_dice", null).ToString();
			this.CloseText = GameTexts.FindText("str_close", null).ToString();
			this.ForfeitText = GameTexts.FindText("str_forfeit", null).ToString();
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00013185 File Offset: 0x00011385
		public void Activate()
		{
			this.SwitchTurns();
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x0001318D File Offset: 0x0001138D
		public void DiceRoll(int roll)
		{
			this.DiceResult = roll.ToString();
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0001319C File Offset: 0x0001139C
		public void SwitchTurns()
		{
			this.IsPlayersTurn = this._missionBoardGameHandler.Board.PlayerTurn == PlayerTurn.PlayerOne || this._missionBoardGameHandler.Board.PlayerTurn == PlayerTurn.PlayerOneWaiting;
			this.TurnOwnerText = (this.IsPlayersTurn ? GameTexts.FindText("str_your_turn", null).ToString() : GameTexts.FindText("str_opponents_turn", null).ToString());
			this.DiceResult = "-";
			this.CanRoll = this.IsPlayersTurn && this.IsGameUsingDice;
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x00013229 File Offset: 0x00011429
		public void ExecuteRoll()
		{
			if (this.CanRoll)
			{
				this._missionBoardGameHandler.RollDice();
				this.CanRoll = false;
			}
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00013248 File Offset: 0x00011448
		public void ExecuteForfeit()
		{
			if (this._missionBoardGameHandler.Board.IsReady && this._missionBoardGameHandler.IsGameInProgress)
			{
				TextObject textObject = new TextObject("{=azJulvrp}{?IS_BETTING}You are going to lose {BET_AMOUNT}{GOLD_ICON} if you forfeit.{newline}{?}{\\?}Do you really want to forfeit?", null);
				textObject.SetTextVariable("IS_BETTING", (this._missionBoardGameHandler.BetAmount > 0) ? 1 : 0);
				textObject.SetTextVariable("BET_AMOUNT", this._missionBoardGameHandler.BetAmount);
				textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
				textObject.SetTextVariable("newline", "{=!}\n");
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_forfeit", null).ToString(), textObject.ToString(), true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), new Action(this._missionBoardGameHandler.ForfeitGame), null, "", 0f, null, null, null), true, false);
			}
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00013340 File Offset: 0x00011540
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM rollDiceKey = this.RollDiceKey;
			if (rollDiceKey == null)
			{
				return;
			}
			rollDiceKey.OnFinalize();
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000458 RID: 1112 RVA: 0x00013358 File Offset: 0x00011558
		// (set) Token: 0x06000459 RID: 1113 RVA: 0x00013360 File Offset: 0x00011560
		[DataSourceProperty]
		public BoardGameInstructionsVM Instructions
		{
			get
			{
				return this._instructions;
			}
			set
			{
				if (value != this._instructions)
				{
					this._instructions = value;
					base.OnPropertyChangedWithValue<BoardGameInstructionsVM>(value, "Instructions");
				}
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x0600045A RID: 1114 RVA: 0x0001337E File Offset: 0x0001157E
		// (set) Token: 0x0600045B RID: 1115 RVA: 0x00013386 File Offset: 0x00011586
		[DataSourceProperty]
		public bool CanRoll
		{
			get
			{
				return this._canRoll;
			}
			set
			{
				if (value != this._canRoll)
				{
					this._canRoll = value;
					base.OnPropertyChangedWithValue(value, "CanRoll");
				}
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x0600045C RID: 1116 RVA: 0x000133A4 File Offset: 0x000115A4
		// (set) Token: 0x0600045D RID: 1117 RVA: 0x000133AC File Offset: 0x000115AC
		[DataSourceProperty]
		public bool IsPlayersTurn
		{
			get
			{
				return this._isPlayersTurn;
			}
			set
			{
				if (value != this._isPlayersTurn)
				{
					this._isPlayersTurn = value;
					base.OnPropertyChangedWithValue(value, "IsPlayersTurn");
				}
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600045E RID: 1118 RVA: 0x000133CA File Offset: 0x000115CA
		// (set) Token: 0x0600045F RID: 1119 RVA: 0x000133D2 File Offset: 0x000115D2
		[DataSourceProperty]
		public bool IsGameUsingDice
		{
			get
			{
				return this._isGameUsingDice;
			}
			set
			{
				if (value != this._isGameUsingDice)
				{
					this._isGameUsingDice = value;
					base.OnPropertyChangedWithValue(value, "IsGameUsingDice");
				}
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000460 RID: 1120 RVA: 0x000133F0 File Offset: 0x000115F0
		// (set) Token: 0x06000461 RID: 1121 RVA: 0x000133F8 File Offset: 0x000115F8
		[DataSourceProperty]
		public string DiceResult
		{
			get
			{
				return this._diceResult;
			}
			set
			{
				if (value != this._diceResult)
				{
					this._diceResult = value;
					base.OnPropertyChangedWithValue<string>(value, "DiceResult");
				}
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x0001341B File Offset: 0x0001161B
		// (set) Token: 0x06000463 RID: 1123 RVA: 0x00013423 File Offset: 0x00011623
		[DataSourceProperty]
		public string RollDiceText
		{
			get
			{
				return this._rollDiceText;
			}
			set
			{
				if (value != this._rollDiceText)
				{
					this._rollDiceText = value;
					base.OnPropertyChangedWithValue<string>(value, "RollDiceText");
				}
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x00013446 File Offset: 0x00011646
		// (set) Token: 0x06000465 RID: 1125 RVA: 0x0001344E File Offset: 0x0001164E
		[DataSourceProperty]
		public string TurnOwnerText
		{
			get
			{
				return this._turnOwnerText;
			}
			set
			{
				if (value != this._turnOwnerText)
				{
					this._turnOwnerText = value;
					base.OnPropertyChangedWithValue<string>(value, "TurnOwnerText");
				}
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x00013471 File Offset: 0x00011671
		// (set) Token: 0x06000467 RID: 1127 RVA: 0x00013479 File Offset: 0x00011679
		[DataSourceProperty]
		public string BoardGameType
		{
			get
			{
				return this._boardGameType;
			}
			set
			{
				if (value != this._boardGameType)
				{
					this._boardGameType = value;
					base.OnPropertyChangedWithValue<string>(value, "BoardGameType");
				}
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x0001349C File Offset: 0x0001169C
		// (set) Token: 0x06000469 RID: 1129 RVA: 0x000134A4 File Offset: 0x000116A4
		[DataSourceProperty]
		public string CloseText
		{
			get
			{
				return this._closeText;
			}
			set
			{
				if (value != this._closeText)
				{
					this._closeText = value;
					base.OnPropertyChangedWithValue<string>(value, "CloseText");
				}
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x000134C7 File Offset: 0x000116C7
		// (set) Token: 0x0600046B RID: 1131 RVA: 0x000134CF File Offset: 0x000116CF
		[DataSourceProperty]
		public string ForfeitText
		{
			get
			{
				return this._forfeitText;
			}
			set
			{
				if (value != this._forfeitText)
				{
					this._forfeitText = value;
					base.OnPropertyChangedWithValue<string>(value, "ForfeitText");
				}
			}
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x000134F2 File Offset: 0x000116F2
		public void SetRollDiceKey(HotKey key)
		{
			this.RollDiceKey = InputKeyItemVM.CreateFromHotKey(key, false);
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x0600046D RID: 1133 RVA: 0x00013501 File Offset: 0x00011701
		// (set) Token: 0x0600046E RID: 1134 RVA: 0x00013509 File Offset: 0x00011709
		[DataSourceProperty]
		public InputKeyItemVM RollDiceKey
		{
			get
			{
				return this._rollDiceKey;
			}
			set
			{
				if (value != this._rollDiceKey)
				{
					this._rollDiceKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "RollDiceKey");
				}
			}
		}

		// Token: 0x0400023E RID: 574
		private readonly MissionBoardGameLogic _missionBoardGameHandler;

		// Token: 0x0400023F RID: 575
		private BoardGameInstructionsVM _instructions;

		// Token: 0x04000240 RID: 576
		private string _turnOwnerText;

		// Token: 0x04000241 RID: 577
		private string _boardGameType;

		// Token: 0x04000242 RID: 578
		private bool _isGameUsingDice;

		// Token: 0x04000243 RID: 579
		private bool _isPlayersTurn;

		// Token: 0x04000244 RID: 580
		private bool _canRoll;

		// Token: 0x04000245 RID: 581
		private string _diceResult;

		// Token: 0x04000246 RID: 582
		private string _rollDiceText;

		// Token: 0x04000247 RID: 583
		private string _closeText;

		// Token: 0x04000248 RID: 584
		private string _forfeitText;

		// Token: 0x04000249 RID: 585
		private InputKeyItemVM _rollDiceKey;
	}
}
