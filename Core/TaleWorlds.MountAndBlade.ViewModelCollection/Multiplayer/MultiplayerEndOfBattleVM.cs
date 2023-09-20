using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000043 RID: 67
	public class MultiplayerEndOfBattleVM : ViewModel
	{
		// Token: 0x060005AC RID: 1452 RVA: 0x00018555 File Offset: 0x00016755
		public MultiplayerEndOfBattleVM()
		{
			this._activeDelay = MissionLobbyComponent.PostMatchWaitDuration / 2f;
			this._gameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this.RefreshValues();
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x00018584 File Offset: 0x00016784
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=GPfkMajw}Battle Ended", null).ToString();
			this.DescriptionText = new TextObject("{=ADPaaX8R}Best Players of This Battle", null).ToString();
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x000185B8 File Offset: 0x000167B8
		public void OnTick(float dt)
		{
			if (this._isBattleEnded)
			{
				this._activateTimeElapsed += dt;
				if (this._activateTimeElapsed >= this._activeDelay)
				{
					this._isBattleEnded = false;
					this.OnEnabled();
				}
			}
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x000185EC File Offset: 0x000167EC
		private void OnEnabled()
		{
			MissionScoreboardComponent missionBehavior = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
			List<MissionPeer> list = new List<MissionPeer>();
			foreach (MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide in missionBehavior.Sides.Where((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side != BattleSideEnum.None))
			{
				foreach (MissionPeer missionPeer in missionScoreboardSide.Players)
				{
					list.Add(missionPeer);
				}
			}
			list.Sort((MissionPeer p1, MissionPeer p2) => this.GetPeerScore(p2).CompareTo(this.GetPeerScore(p1)));
			if (list.Count > 0)
			{
				this.HasFirstPlace = true;
				MissionPeer missionPeer2 = list[0];
				this.FirstPlacePlayer = new MPEndOfBattlePlayerVM(missionPeer2, this.GetPeerScore(missionPeer2), 1);
			}
			if (list.Count > 1)
			{
				this.HasSecondPlace = true;
				MissionPeer missionPeer3 = list[1];
				this.SecondPlacePlayer = new MPEndOfBattlePlayerVM(missionPeer3, this.GetPeerScore(missionPeer3), 2);
			}
			if (list.Count > 2)
			{
				this.HasThirdPlace = true;
				MissionPeer missionPeer4 = list[2];
				this.ThirdPlacePlayer = new MPEndOfBattlePlayerVM(missionPeer4, this.GetPeerScore(missionPeer4), 3);
			}
			this.IsEnabled = true;
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x00018748 File Offset: 0x00016948
		public void OnBattleEnded()
		{
			this._isBattleEnded = true;
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x00018751 File Offset: 0x00016951
		private int GetPeerScore(MissionPeer peer)
		{
			if (peer == null)
			{
				return 0;
			}
			if (this._gameMode.GameType != MissionLobbyComponent.MultiplayerGameType.Duel)
			{
				return peer.Score;
			}
			DuelMissionRepresentative component = peer.GetComponent<DuelMissionRepresentative>();
			if (component == null)
			{
				return 0;
			}
			return component.Score;
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060005B2 RID: 1458 RVA: 0x0001877E File Offset: 0x0001697E
		// (set) Token: 0x060005B3 RID: 1459 RVA: 0x00018786 File Offset: 0x00016986
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

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060005B4 RID: 1460 RVA: 0x000187A4 File Offset: 0x000169A4
		// (set) Token: 0x060005B5 RID: 1461 RVA: 0x000187AC File Offset: 0x000169AC
		[DataSourceProperty]
		public bool HasFirstPlace
		{
			get
			{
				return this._hasFirstPlace;
			}
			set
			{
				if (value != this._hasFirstPlace)
				{
					this._hasFirstPlace = value;
					base.OnPropertyChangedWithValue(value, "HasFirstPlace");
				}
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060005B6 RID: 1462 RVA: 0x000187CA File Offset: 0x000169CA
		// (set) Token: 0x060005B7 RID: 1463 RVA: 0x000187D2 File Offset: 0x000169D2
		[DataSourceProperty]
		public bool HasSecondPlace
		{
			get
			{
				return this._hasSecondPlace;
			}
			set
			{
				if (value != this._hasSecondPlace)
				{
					this._hasSecondPlace = value;
					base.OnPropertyChangedWithValue(value, "HasSecondPlace");
				}
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x060005B8 RID: 1464 RVA: 0x000187F0 File Offset: 0x000169F0
		// (set) Token: 0x060005B9 RID: 1465 RVA: 0x000187F8 File Offset: 0x000169F8
		[DataSourceProperty]
		public bool HasThirdPlace
		{
			get
			{
				return this._hasThirdPlace;
			}
			set
			{
				if (value != this._hasThirdPlace)
				{
					this._hasThirdPlace = value;
					base.OnPropertyChangedWithValue(value, "HasThirdPlace");
				}
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x00018816 File Offset: 0x00016A16
		// (set) Token: 0x060005BB RID: 1467 RVA: 0x0001881E File Offset: 0x00016A1E
		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x00018841 File Offset: 0x00016A41
		// (set) Token: 0x060005BD RID: 1469 RVA: 0x00018849 File Offset: 0x00016A49
		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x060005BE RID: 1470 RVA: 0x0001886C File Offset: 0x00016A6C
		// (set) Token: 0x060005BF RID: 1471 RVA: 0x00018874 File Offset: 0x00016A74
		[DataSourceProperty]
		public MPEndOfBattlePlayerVM FirstPlacePlayer
		{
			get
			{
				return this._firstPlacePlayer;
			}
			set
			{
				if (value != this._firstPlacePlayer)
				{
					this._firstPlacePlayer = value;
					base.OnPropertyChangedWithValue<MPEndOfBattlePlayerVM>(value, "FirstPlacePlayer");
				}
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x060005C0 RID: 1472 RVA: 0x00018892 File Offset: 0x00016A92
		// (set) Token: 0x060005C1 RID: 1473 RVA: 0x0001889A File Offset: 0x00016A9A
		[DataSourceProperty]
		public MPEndOfBattlePlayerVM SecondPlacePlayer
		{
			get
			{
				return this._secondPlacePlayer;
			}
			set
			{
				if (value != this._secondPlacePlayer)
				{
					this._secondPlacePlayer = value;
					base.OnPropertyChangedWithValue<MPEndOfBattlePlayerVM>(value, "SecondPlacePlayer");
				}
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x060005C2 RID: 1474 RVA: 0x000188B8 File Offset: 0x00016AB8
		// (set) Token: 0x060005C3 RID: 1475 RVA: 0x000188C0 File Offset: 0x00016AC0
		[DataSourceProperty]
		public MPEndOfBattlePlayerVM ThirdPlacePlayer
		{
			get
			{
				return this._thirdPlacePlayer;
			}
			set
			{
				if (value != this._thirdPlacePlayer)
				{
					this._thirdPlacePlayer = value;
					base.OnPropertyChangedWithValue<MPEndOfBattlePlayerVM>(value, "ThirdPlacePlayer");
				}
			}
		}

		// Token: 0x040002E5 RID: 741
		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		// Token: 0x040002E6 RID: 742
		private readonly float _activeDelay;

		// Token: 0x040002E7 RID: 743
		private bool _isBattleEnded;

		// Token: 0x040002E8 RID: 744
		private float _activateTimeElapsed;

		// Token: 0x040002E9 RID: 745
		private bool _isEnabled;

		// Token: 0x040002EA RID: 746
		private bool _hasFirstPlace;

		// Token: 0x040002EB RID: 747
		private bool _hasSecondPlace;

		// Token: 0x040002EC RID: 748
		private bool _hasThirdPlace;

		// Token: 0x040002ED RID: 749
		private string _titleText;

		// Token: 0x040002EE RID: 750
		private string _descriptionText;

		// Token: 0x040002EF RID: 751
		private MPEndOfBattlePlayerVM _firstPlacePlayer;

		// Token: 0x040002F0 RID: 752
		private MPEndOfBattlePlayerVM _secondPlacePlayer;

		// Token: 0x040002F1 RID: 753
		private MPEndOfBattlePlayerVM _thirdPlacePlayer;
	}
}
