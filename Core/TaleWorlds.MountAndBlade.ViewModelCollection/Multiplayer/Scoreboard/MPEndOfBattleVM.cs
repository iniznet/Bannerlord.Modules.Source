using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
	// Token: 0x02000055 RID: 85
	public class MPEndOfBattleVM : ViewModel
	{
		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000725 RID: 1829 RVA: 0x0001CAFE File Offset: 0x0001ACFE
		private MissionRepresentativeBase missionRep
		{
			get
			{
				NetworkCommunicator myPeer = GameNetwork.MyPeer;
				if (myPeer == null)
				{
					return null;
				}
				VirtualPlayer virtualPlayer = myPeer.VirtualPlayer;
				if (virtualPlayer == null)
				{
					return null;
				}
				return virtualPlayer.GetComponent<MissionRepresentativeBase>();
			}
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x0001CB1C File Offset: 0x0001AD1C
		public MPEndOfBattleVM(Mission mission, MissionScoreboardComponent missionScoreboardComponent, bool isSingleTeam)
		{
			this._missionScoreboardComponent = missionScoreboardComponent;
			this._gameMode = mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this._lobbyComponent = mission.GetMissionBehavior<MissionLobbyComponent>();
			this._lobbyComponent.OnPostMatchEnded += this.OnPostMatchEnded;
			this._isSingleTeam = isSingleTeam;
			this.RefreshValues();
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x0001CB74 File Offset: 0x0001AD74
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CountdownTitle = new TextObject("{=wGjQgQlY}Next Game begins in:", null).ToString();
			this.Header = new TextObject("{=HXxNfncd}End of Battle", null).ToString();
			MPEndOfBattleSideVM allySide = this.AllySide;
			if (allySide != null)
			{
				allySide.RefreshValues();
			}
			MPEndOfBattleSideVM enemySide = this.EnemySide;
			if (enemySide == null)
			{
				return;
			}
			enemySide.RefreshValues();
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x0001CBD4 File Offset: 0x0001ADD4
		public override void OnFinalize()
		{
			base.OnFinalize();
			this._lobbyComponent.OnPostMatchEnded -= this.OnPostMatchEnded;
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x0001CBF3 File Offset: 0x0001ADF3
		public void Tick(float dt)
		{
			this.Countdown = MathF.Ceiling(this._gameMode.RemainingTime);
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x0001CC0B File Offset: 0x0001AE0B
		private void OnPostMatchEnded()
		{
			this.OnFinalRoundEnded();
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x0001CC14 File Offset: 0x0001AE14
		private void OnFinalRoundEnded()
		{
			if (this._isSingleTeam)
			{
				return;
			}
			this.IsAvailable = true;
			this.InitSides();
			MissionScoreboardComponent missionScoreboardComponent = this._missionScoreboardComponent;
			BattleSideEnum battleSideEnum = ((missionScoreboardComponent != null) ? missionScoreboardComponent.GetMatchWinnerSide() : BattleSideEnum.None);
			if (battleSideEnum == this._enemyBattleSide)
			{
				this.BattleResult = 0;
				this.ResultText = GameTexts.FindText("str_defeat", null).ToString();
				return;
			}
			if (battleSideEnum == this._allyBattleSide)
			{
				this.BattleResult = 1;
				this.ResultText = GameTexts.FindText("str_victory", null).ToString();
				return;
			}
			this.BattleResult = 2;
			this.ResultText = GameTexts.FindText("str_draw", null).ToString();
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x0001CCB8 File Offset: 0x0001AEB8
		private void InitSides()
		{
			this._allyBattleSide = BattleSideEnum.Attacker;
			this._enemyBattleSide = BattleSideEnum.Defender;
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			if (missionPeer != null)
			{
				Team team = missionPeer.Team;
				if (team != null && team.Side == BattleSideEnum.Defender)
				{
					this._allyBattleSide = BattleSideEnum.Defender;
					this._enemyBattleSide = BattleSideEnum.Attacker;
				}
			}
			MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide = this._missionScoreboardComponent.Sides.FirstOrDefault((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == this._allyBattleSide);
			if (missionScoreboardSide != null)
			{
				string text = ((missionScoreboardSide.Side == BattleSideEnum.Attacker) ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
				this.AllySide = new MPEndOfBattleSideVM(this._missionScoreboardComponent, missionScoreboardSide, MBObjectManager.Instance.GetObject<BasicCultureObject>(text));
			}
			missionScoreboardSide = this._missionScoreboardComponent.Sides.FirstOrDefault((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == this._enemyBattleSide);
			if (missionScoreboardSide != null)
			{
				string text2 = ((missionScoreboardSide.Side == BattleSideEnum.Attacker) ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
				this.EnemySide = new MPEndOfBattleSideVM(this._missionScoreboardComponent, missionScoreboardSide, MBObjectManager.Instance.GetObject<BasicCultureObject>(text2));
			}
			if (this.EnemySide.Side.Side == BattleSideEnum.Defender)
			{
				this.EnemySide.UseSecondary = true;
				return;
			}
			if (this.AllySide.Side.Side == BattleSideEnum.Defender)
			{
				this.AllySide.UseSecondary = true;
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x0600072D RID: 1837 RVA: 0x0001CDF9 File Offset: 0x0001AFF9
		// (set) Token: 0x0600072E RID: 1838 RVA: 0x0001CE01 File Offset: 0x0001B001
		[DataSourceProperty]
		public bool IsAvailable
		{
			get
			{
				return this._isAvailable;
			}
			set
			{
				if (value != this._isAvailable)
				{
					this._isAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsAvailable");
				}
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x0600072F RID: 1839 RVA: 0x0001CE1F File Offset: 0x0001B01F
		// (set) Token: 0x06000730 RID: 1840 RVA: 0x0001CE27 File Offset: 0x0001B027
		[DataSourceProperty]
		public string CountdownTitle
		{
			get
			{
				return this._countdownTitle;
			}
			set
			{
				if (value != this._countdownTitle)
				{
					this._countdownTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "CountdownTitle");
				}
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06000731 RID: 1841 RVA: 0x0001CE4A File Offset: 0x0001B04A
		// (set) Token: 0x06000732 RID: 1842 RVA: 0x0001CE52 File Offset: 0x0001B052
		[DataSourceProperty]
		public int Countdown
		{
			get
			{
				return this._countdown;
			}
			set
			{
				if (value != this._countdown)
				{
					this._countdown = value;
					base.OnPropertyChangedWithValue(value, "Countdown");
				}
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06000733 RID: 1843 RVA: 0x0001CE70 File Offset: 0x0001B070
		// (set) Token: 0x06000734 RID: 1844 RVA: 0x0001CE78 File Offset: 0x0001B078
		[DataSourceProperty]
		public string Header
		{
			get
			{
				return this._header;
			}
			set
			{
				if (value != this._header)
				{
					this._header = value;
					base.OnPropertyChangedWithValue<string>(value, "Header");
				}
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06000735 RID: 1845 RVA: 0x0001CE9B File Offset: 0x0001B09B
		// (set) Token: 0x06000736 RID: 1846 RVA: 0x0001CEA3 File Offset: 0x0001B0A3
		[DataSourceProperty]
		public int BattleResult
		{
			get
			{
				return this._battleResult;
			}
			set
			{
				if (value != this._battleResult)
				{
					this._battleResult = value;
					base.OnPropertyChangedWithValue(value, "BattleResult");
				}
			}
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06000737 RID: 1847 RVA: 0x0001CEC1 File Offset: 0x0001B0C1
		// (set) Token: 0x06000738 RID: 1848 RVA: 0x0001CEC9 File Offset: 0x0001B0C9
		[DataSourceProperty]
		public string ResultText
		{
			get
			{
				return this._resultText;
			}
			set
			{
				if (value != this._resultText)
				{
					this._resultText = value;
					base.OnPropertyChangedWithValue<string>(value, "ResultText");
				}
			}
		}

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000739 RID: 1849 RVA: 0x0001CEEC File Offset: 0x0001B0EC
		// (set) Token: 0x0600073A RID: 1850 RVA: 0x0001CEF4 File Offset: 0x0001B0F4
		[DataSourceProperty]
		public MPEndOfBattleSideVM AllySide
		{
			get
			{
				return this._allySide;
			}
			set
			{
				if (value != this._allySide)
				{
					this._allySide = value;
					base.OnPropertyChangedWithValue<MPEndOfBattleSideVM>(value, "AllySide");
				}
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x0600073B RID: 1851 RVA: 0x0001CF12 File Offset: 0x0001B112
		// (set) Token: 0x0600073C RID: 1852 RVA: 0x0001CF1A File Offset: 0x0001B11A
		[DataSourceProperty]
		public MPEndOfBattleSideVM EnemySide
		{
			get
			{
				return this._enemySide;
			}
			set
			{
				if (value != this._enemySide)
				{
					this._enemySide = value;
					base.OnPropertyChangedWithValue<MPEndOfBattleSideVM>(value, "EnemySide");
				}
			}
		}

		// Token: 0x040003A2 RID: 930
		private MissionScoreboardComponent _missionScoreboardComponent;

		// Token: 0x040003A3 RID: 931
		private MissionMultiplayerGameModeBaseClient _gameMode;

		// Token: 0x040003A4 RID: 932
		private MissionLobbyComponent _lobbyComponent;

		// Token: 0x040003A5 RID: 933
		private bool _isSingleTeam;

		// Token: 0x040003A6 RID: 934
		private BattleSideEnum _allyBattleSide;

		// Token: 0x040003A7 RID: 935
		private BattleSideEnum _enemyBattleSide;

		// Token: 0x040003A8 RID: 936
		private bool _isAvailable;

		// Token: 0x040003A9 RID: 937
		private string _countdownTitle;

		// Token: 0x040003AA RID: 938
		private int _countdown;

		// Token: 0x040003AB RID: 939
		private string _header;

		// Token: 0x040003AC RID: 940
		private int _battleResult;

		// Token: 0x040003AD RID: 941
		private string _resultText;

		// Token: 0x040003AE RID: 942
		private MPEndOfBattleSideVM _allySide;

		// Token: 0x040003AF RID: 943
		private MPEndOfBattleSideVM _enemySide;
	}
}
