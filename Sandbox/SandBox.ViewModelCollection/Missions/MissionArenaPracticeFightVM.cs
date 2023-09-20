using System;
using SandBox.Missions.MissionLogics.Arena;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Missions
{
	// Token: 0x02000024 RID: 36
	public class MissionArenaPracticeFightVM : ViewModel
	{
		// Token: 0x060002DA RID: 730 RVA: 0x0000E0B1 File Offset: 0x0000C2B1
		public MissionArenaPracticeFightVM(ArenaPracticeFightMissionController practiceMissionController)
		{
			this._practiceMissionController = practiceMissionController;
			this._mission = practiceMissionController.Mission;
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000E0CC File Offset: 0x0000C2CC
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

		// Token: 0x060002DC RID: 732 RVA: 0x0000E168 File Offset: 0x0000C368
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

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x060002DD RID: 733 RVA: 0x0000E1EB File Offset: 0x0000C3EB
		// (set) Token: 0x060002DE RID: 734 RVA: 0x0000E1F3 File Offset: 0x0000C3F3
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

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x060002DF RID: 735 RVA: 0x0000E216 File Offset: 0x0000C416
		// (set) Token: 0x060002E0 RID: 736 RVA: 0x0000E21E File Offset: 0x0000C41E
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

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x0000E241 File Offset: 0x0000C441
		// (set) Token: 0x060002E2 RID: 738 RVA: 0x0000E249 File Offset: 0x0000C449
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

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x0000E26C File Offset: 0x0000C46C
		// (set) Token: 0x060002E4 RID: 740 RVA: 0x0000E274 File Offset: 0x0000C474
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

		// Token: 0x0400016B RID: 363
		private readonly Mission _mission;

		// Token: 0x0400016C RID: 364
		private readonly ArenaPracticeFightMissionController _practiceMissionController;

		// Token: 0x0400016D RID: 365
		private string _opponentsBeatenText;

		// Token: 0x0400016E RID: 366
		private string _opponentsRemainingText;

		// Token: 0x0400016F RID: 367
		private bool _isPlayerPracticing;

		// Token: 0x04000170 RID: 368
		private string _prizeText;
	}
}
