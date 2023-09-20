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
	public class BoardGameVM : ViewModel
	{
		public BoardGameVM()
		{
			this._missionBoardGameHandler = Mission.Current.GetMissionBehavior<MissionBoardGameLogic>();
			this.BoardGameType = this._missionBoardGameHandler.CurrentBoardGame.ToString();
			this.IsGameUsingDice = this._missionBoardGameHandler.RequiresDiceRolling();
			this.DiceResult = "-";
			this.Instructions = new BoardGameInstructionsVM(this._missionBoardGameHandler.CurrentBoardGame);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RollDiceText = GameTexts.FindText("str_roll_dice", null).ToString();
			this.CloseText = GameTexts.FindText("str_close", null).ToString();
			this.ForfeitText = GameTexts.FindText("str_forfeit", null).ToString();
		}

		public void Activate()
		{
			this.SwitchTurns();
		}

		public void DiceRoll(int roll)
		{
			this.DiceResult = roll.ToString();
		}

		public void SwitchTurns()
		{
			this.IsPlayersTurn = this._missionBoardGameHandler.Board.PlayerTurn == PlayerTurn.PlayerOne || this._missionBoardGameHandler.Board.PlayerTurn == PlayerTurn.PlayerOneWaiting;
			this.TurnOwnerText = (this.IsPlayersTurn ? GameTexts.FindText("str_your_turn", null).ToString() : GameTexts.FindText("str_opponents_turn", null).ToString());
			this.DiceResult = "-";
			this.CanRoll = this.IsPlayersTurn && this.IsGameUsingDice;
		}

		public void ExecuteRoll()
		{
			if (this.CanRoll)
			{
				this._missionBoardGameHandler.RollDice();
				this.CanRoll = false;
			}
		}

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

		public void SetRollDiceKey(HotKey key)
		{
			this.RollDiceKey = InputKeyItemVM.CreateFromHotKey(key, false);
		}

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

		private readonly MissionBoardGameLogic _missionBoardGameHandler;

		private BoardGameInstructionsVM _instructions;

		private string _turnOwnerText;

		private string _boardGameType;

		private bool _isGameUsingDice;

		private bool _isPlayersTurn;

		private bool _canRoll;

		private string _diceResult;

		private string _rollDiceText;

		private string _closeText;

		private string _forfeitText;

		private InputKeyItemVM _rollDiceKey;
	}
}
