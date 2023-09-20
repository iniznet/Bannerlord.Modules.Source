using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.HUDExtensions
{
	// Token: 0x020000B9 RID: 185
	public class DuelMatchVM : ViewModel
	{
		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x06001188 RID: 4488 RVA: 0x00039D14 File Offset: 0x00037F14
		// (set) Token: 0x06001189 RID: 4489 RVA: 0x00039D1C File Offset: 0x00037F1C
		public MissionPeer FirstPlayerPeer { get; private set; }

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x0600118A RID: 4490 RVA: 0x00039D25 File Offset: 0x00037F25
		// (set) Token: 0x0600118B RID: 4491 RVA: 0x00039D2D File Offset: 0x00037F2D
		public MissionPeer SecondPlayerPeer { get; private set; }

		// Token: 0x0600118C RID: 4492 RVA: 0x00039D36 File Offset: 0x00037F36
		public DuelMatchVM()
		{
			this.IsEnabled = false;
			this._duelCountdownText = new TextObject("{=cO2FDHCa}Duel with {OPPONENT_NAME} is starting in {DUEL_REMAINING_TIME} seconds.", null);
			this.RefreshValues();
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x00039D5C File Offset: 0x00037F5C
		public void OnDuelPrepStarted(MissionPeer opponentPeer, int prepDuration)
		{
			this._prepTimeRemaining = (float)prepDuration;
			GameTexts.SetVariable("OPPONENT_NAME", opponentPeer.DisplayedName);
			this.IsPreparing = true;
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x00039D80 File Offset: 0x00037F80
		public void Tick(float dt)
		{
			if (this._prepTimeRemaining > 0f)
			{
				GameTexts.SetVariable("DUEL_REMAINING_TIME", (float)MathF.Ceiling(this._prepTimeRemaining));
				this.CountdownMessage = this._duelCountdownText.ToString();
				this._prepTimeRemaining -= dt;
				return;
			}
			this.IsPreparing = false;
		}

		// Token: 0x0600118F RID: 4495 RVA: 0x00039DD8 File Offset: 0x00037FD8
		public void OnDuelStarted(MissionPeer firstPeer, MissionPeer secondPeer, int arenaType)
		{
			this.FirstPlayerPeer = firstPeer;
			this.SecondPlayerPeer = secondPeer;
			this.FirstPlayerScore = 0;
			this.SecondPlayerScore = 0;
			this.FirstPlayer = new MPPlayerVM(firstPeer);
			this.SecondPlayer = new MPPlayerVM(secondPeer);
			this.FirstPlayer.RefreshDivision(true);
			this.SecondPlayer.RefreshDivision(true);
			this.ArenaType = arenaType;
			this.UpdateScore();
			this.IsEnabled = true;
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x00039E45 File Offset: 0x00038045
		public void OnDuelEnded()
		{
			this.FirstPlayerPeer = null;
			this.SecondPlayerPeer = null;
			this.IsEnabled = false;
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x00039E5C File Offset: 0x0003805C
		public void OnPeerScored(MissionPeer peer)
		{
			if (peer == this.FirstPlayerPeer)
			{
				int num = this.FirstPlayerScore;
				this.FirstPlayerScore = num + 1;
			}
			else if (peer == this.SecondPlayerPeer)
			{
				int num = this.SecondPlayerScore;
				this.SecondPlayerScore = num + 1;
			}
			this.UpdateScore();
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x00039EA3 File Offset: 0x000380A3
		private void UpdateScore()
		{
			GameTexts.SetVariable("LEFT", this.FirstPlayerScore);
			GameTexts.SetVariable("RIGHT", this.SecondPlayerScore);
			this.Score = GameTexts.FindText("str_LEFT_dash_RIGHT", null).ToString();
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x06001193 RID: 4499 RVA: 0x00039EDB File Offset: 0x000380DB
		// (set) Token: 0x06001194 RID: 4500 RVA: 0x00039EE3 File Offset: 0x000380E3
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x06001195 RID: 4501 RVA: 0x00039F01 File Offset: 0x00038101
		// (set) Token: 0x06001196 RID: 4502 RVA: 0x00039F09 File Offset: 0x00038109
		[DataSourceProperty]
		public bool IsPreparing
		{
			get
			{
				return this._isPreparing;
			}
			set
			{
				if (value != this._isPreparing)
				{
					this._isPreparing = value;
					base.OnPropertyChangedWithValue(value, "IsPreparing");
				}
			}
		}

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x06001197 RID: 4503 RVA: 0x00039F27 File Offset: 0x00038127
		// (set) Token: 0x06001198 RID: 4504 RVA: 0x00039F2F File Offset: 0x0003812F
		[DataSourceProperty]
		public string CountdownMessage
		{
			get
			{
				return this._countdownMessage;
			}
			set
			{
				if (value != this._countdownMessage)
				{
					this._countdownMessage = value;
					base.OnPropertyChangedWithValue<string>(value, "CountdownMessage");
				}
			}
		}

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x06001199 RID: 4505 RVA: 0x00039F52 File Offset: 0x00038152
		// (set) Token: 0x0600119A RID: 4506 RVA: 0x00039F5A File Offset: 0x0003815A
		[DataSourceProperty]
		public string Score
		{
			get
			{
				return this._score;
			}
			set
			{
				if (value != this._score)
				{
					this._score = value;
					base.OnPropertyChangedWithValue<string>(value, "Score");
				}
			}
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x0600119B RID: 4507 RVA: 0x00039F7D File Offset: 0x0003817D
		// (set) Token: 0x0600119C RID: 4508 RVA: 0x00039F85 File Offset: 0x00038185
		[DataSourceProperty]
		public int ArenaType
		{
			get
			{
				return this._arenaType;
			}
			set
			{
				if (value != this._arenaType)
				{
					this._arenaType = value;
					base.OnPropertyChangedWithValue(value, "ArenaType");
				}
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x0600119D RID: 4509 RVA: 0x00039FA3 File Offset: 0x000381A3
		// (set) Token: 0x0600119E RID: 4510 RVA: 0x00039FAB File Offset: 0x000381AB
		[DataSourceProperty]
		public int FirstPlayerScore
		{
			get
			{
				return this._firstPlayerScore;
			}
			set
			{
				if (value != this._firstPlayerScore)
				{
					this._firstPlayerScore = value;
					base.OnPropertyChangedWithValue(value, "FirstPlayerScore");
				}
			}
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x0600119F RID: 4511 RVA: 0x00039FC9 File Offset: 0x000381C9
		// (set) Token: 0x060011A0 RID: 4512 RVA: 0x00039FD1 File Offset: 0x000381D1
		[DataSourceProperty]
		public int SecondPlayerScore
		{
			get
			{
				return this._secondPlayerScore;
			}
			set
			{
				if (value != this._secondPlayerScore)
				{
					this._secondPlayerScore = value;
					base.OnPropertyChangedWithValue(value, "SecondPlayerScore");
				}
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x060011A1 RID: 4513 RVA: 0x00039FEF File Offset: 0x000381EF
		// (set) Token: 0x060011A2 RID: 4514 RVA: 0x00039FF7 File Offset: 0x000381F7
		[DataSourceProperty]
		public MPPlayerVM FirstPlayer
		{
			get
			{
				return this._firstPlayer;
			}
			set
			{
				if (value != this._firstPlayer)
				{
					this._firstPlayer = value;
					base.OnPropertyChangedWithValue<MPPlayerVM>(value, "FirstPlayer");
				}
			}
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x060011A3 RID: 4515 RVA: 0x0003A015 File Offset: 0x00038215
		// (set) Token: 0x060011A4 RID: 4516 RVA: 0x0003A01D File Offset: 0x0003821D
		[DataSourceProperty]
		public MPPlayerVM SecondPlayer
		{
			get
			{
				return this._secondPlayer;
			}
			set
			{
				if (value != this._secondPlayer)
				{
					this._secondPlayer = value;
					base.OnPropertyChangedWithValue<MPPlayerVM>(value, "SecondPlayer");
				}
			}
		}

		// Token: 0x0400085B RID: 2139
		private float _prepTimeRemaining;

		// Token: 0x0400085C RID: 2140
		private TextObject _duelCountdownText;

		// Token: 0x0400085D RID: 2141
		private bool _isEnabled;

		// Token: 0x0400085E RID: 2142
		private bool _isPreparing;

		// Token: 0x0400085F RID: 2143
		private string _countdownMessage;

		// Token: 0x04000860 RID: 2144
		private string _score;

		// Token: 0x04000861 RID: 2145
		private int _arenaType;

		// Token: 0x04000862 RID: 2146
		private int _firstPlayerScore;

		// Token: 0x04000863 RID: 2147
		private int _secondPlayerScore;

		// Token: 0x04000864 RID: 2148
		private MPPlayerVM _firstPlayer;

		// Token: 0x04000865 RID: 2149
		private MPPlayerVM _secondPlayer;
	}
}
