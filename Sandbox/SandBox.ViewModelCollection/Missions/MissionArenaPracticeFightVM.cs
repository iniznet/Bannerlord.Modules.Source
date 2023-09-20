using System;
using SandBox.Missions.MissionLogics.Arena;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Missions
{
	public class MissionArenaPracticeFightVM : ViewModel
	{
		public MissionArenaPracticeFightVM(ArenaPracticeFightMissionController practiceMissionController)
		{
			this._practiceMissionController = practiceMissionController;
			this._mission = practiceMissionController.Mission;
		}

		public void Tick()
		{
			this.IsPlayerPracticing = this._practiceMissionController.IsPlayerPracticing;
			Agent mainAgent = this._mission.MainAgent;
			if (mainAgent != null && mainAgent.IsActive())
			{
				int killCount = this._mission.MainAgent.KillCount;
				GameTexts.SetVariable("BEATEN_OPPONENT_COUNT", killCount);
				this.OpponentsBeatenText = GameTexts.FindText("str_beaten_opponent", null).ToString();
			}
			int remainingOpponentCount = this._practiceMissionController.RemainingOpponentCount;
			GameTexts.SetVariable("REMAINING_OPPONENT_COUNT", remainingOpponentCount);
			this.OpponentsRemainingText = GameTexts.FindText("str_remaining_opponent", null).ToString();
			this.UpdatePrizeText();
		}

		public void UpdatePrizeText()
		{
			bool remainingOpponentCount = this._practiceMissionController.RemainingOpponentCount != 0;
			int opponentCountBeatenByPlayer = this._practiceMissionController.OpponentCountBeatenByPlayer;
			int num = 0;
			if (!remainingOpponentCount)
			{
				num = 250;
			}
			else if (opponentCountBeatenByPlayer >= 3)
			{
				if (opponentCountBeatenByPlayer < 6)
				{
					num = 5;
				}
				else if (opponentCountBeatenByPlayer < 10)
				{
					num = 10;
				}
				else if (opponentCountBeatenByPlayer < 20)
				{
					num = 25;
				}
				else
				{
					num = 60;
				}
			}
			GameTexts.SetVariable("DENAR_AMOUNT", num);
			GameTexts.SetVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
			this.PrizeText = GameTexts.FindText("str_earned_denar", null).ToString();
		}

		[DataSourceProperty]
		public string OpponentsBeatenText
		{
			get
			{
				return this._opponentsBeatenText;
			}
			set
			{
				if (this._opponentsBeatenText != value)
				{
					this._opponentsBeatenText = value;
					base.OnPropertyChangedWithValue<string>(value, "OpponentsBeatenText");
				}
			}
		}

		[DataSourceProperty]
		public string PrizeText
		{
			get
			{
				return this._prizeText;
			}
			set
			{
				if (this._prizeText != value)
				{
					this._prizeText = value;
					base.OnPropertyChangedWithValue<string>(value, "PrizeText");
				}
			}
		}

		[DataSourceProperty]
		public string OpponentsRemainingText
		{
			get
			{
				return this._opponentsRemainingText;
			}
			set
			{
				if (this._opponentsRemainingText != value)
				{
					this._opponentsRemainingText = value;
					base.OnPropertyChangedWithValue<string>(value, "OpponentsRemainingText");
				}
			}
		}

		public bool IsPlayerPracticing
		{
			get
			{
				return this._isPlayerPracticing;
			}
			set
			{
				if (this._isPlayerPracticing != value)
				{
					this._isPlayerPracticing = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerPracticing");
				}
			}
		}

		private readonly Mission _mission;

		private readonly ArenaPracticeFightMissionController _practiceMissionController;

		private string _opponentsBeatenText;

		private string _opponentsRemainingText;

		private bool _isPlayerPracticing;

		private string _prizeText;
	}
}
