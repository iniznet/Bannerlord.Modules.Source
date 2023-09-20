using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x0200003B RID: 59
	public class MPDeathCardVM : ViewModel
	{
		// Token: 0x06000522 RID: 1314 RVA: 0x000166C0 File Offset: 0x000148C0
		public MPDeathCardVM(MissionLobbyComponent.MultiplayerGameType gameType)
		{
			this.KillCountsEnabled = gameType != MissionLobbyComponent.MultiplayerGameType.Captain;
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x00016735 File Offset: 0x00014935
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.YouText = GameTexts.FindText("str_death_card_you", null).ToString();
			this.Deactivate();
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x0001675C File Offset: 0x0001495C
		public void OnMainAgentRemoved(Agent affectorAgent, KillingBlow blow)
		{
			this.ResetProperties();
			bool flag = affectorAgent.IsMount && affectorAgent.RiderAgent == null;
			if (affectorAgent == Agent.Main)
			{
				this.TitleText = this._killedSelfText.ToString();
				this.IsSelfInflicted = true;
			}
			else if (flag)
			{
				this._killedByStrayHorse.SetTextVariable("MOUNT_NAME", affectorAgent.Name);
				this.TitleText = this._killedByStrayHorse.ToString();
				this.IsSelfInflicted = true;
			}
			else
			{
				this.IsSelfInflicted = false;
				this.TitleText = this._killedByText.ToString();
			}
			Team team = ((affectorAgent != null) ? affectorAgent.Team : null);
			Agent main = Agent.Main;
			this.KillerText = ((team == ((main != null) ? main.Team : null)) ? this._allyText.ToString() : this._enemyText.ToString());
			if (this.IsSelfInflicted)
			{
				this.PlayerProperties = new MPPlayerVM(GameNetwork.MyPeer.GetComponent<MissionPeer>());
				this.PlayerProperties.RefreshDivision(false);
			}
			else
			{
				this.KillerName = affectorAgent.Name.ToString();
				if (blow.WeaponItemKind >= 0)
				{
					this.UsedWeaponName = ItemObject.GetItemFromWeaponKind(blow.WeaponItemKind).Name.ToString();
				}
				else
				{
					this.UsedWeaponName = new TextObject("{=GAZ5QLZi}Unarmed", null).ToString();
				}
				bool isServerOrRecorder = GameNetwork.IsServerOrRecorder;
				if (affectorAgent.MissionPeer != null)
				{
					this.PlayerProperties = new MPPlayerVM(affectorAgent.MissionPeer);
					this.PlayerProperties.RefreshDivision(false);
					this.NumOfTimesPlayerKilled = Agent.Main.MissionPeer.GetNumberOfTimesPeerKilledPeer(affectorAgent.MissionPeer);
					this.NumOfTimesPlayerGotKilled = affectorAgent.MissionPeer.GetNumberOfTimesPeerKilledPeer(Agent.Main.MissionPeer) + (isServerOrRecorder ? 0 : 1);
				}
				else if (affectorAgent.OwningAgentMissionPeer != null)
				{
					this.PlayerProperties = new MPPlayerVM(affectorAgent.OwningAgentMissionPeer);
					this.PlayerProperties.RefreshDivision(false);
				}
				else
				{
					this.PlayerProperties = new MPPlayerVM(affectorAgent);
				}
			}
			this.IsActive = true;
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0001694F File Offset: 0x00014B4F
		private void ResetProperties()
		{
			this.IsActive = false;
			this.TitleText = "";
			this.UsedWeaponName = "";
			this.BodyPartHit = -1;
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x00016975 File Offset: 0x00014B75
		public void Deactivate()
		{
			this.IsActive = false;
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000527 RID: 1319 RVA: 0x0001697E File Offset: 0x00014B7E
		// (set) Token: 0x06000528 RID: 1320 RVA: 0x00016986 File Offset: 0x00014B86
		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
				}
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000529 RID: 1321 RVA: 0x000169A4 File Offset: 0x00014BA4
		// (set) Token: 0x0600052A RID: 1322 RVA: 0x000169AC File Offset: 0x00014BAC
		[DataSourceProperty]
		public bool IsSelfInflicted
		{
			get
			{
				return this._isSelfInflicted;
			}
			set
			{
				if (value != this._isSelfInflicted)
				{
					this._isSelfInflicted = value;
					base.OnPropertyChangedWithValue(value, "IsSelfInflicted");
				}
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x0600052B RID: 1323 RVA: 0x000169CA File Offset: 0x00014BCA
		// (set) Token: 0x0600052C RID: 1324 RVA: 0x000169D2 File Offset: 0x00014BD2
		[DataSourceProperty]
		public bool KillCountsEnabled
		{
			get
			{
				return this._killCountsEnabled;
			}
			set
			{
				if (value != this._killCountsEnabled)
				{
					this._killCountsEnabled = value;
					base.OnPropertyChangedWithValue(value, "KillCountsEnabled");
				}
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x0600052D RID: 1325 RVA: 0x000169F0 File Offset: 0x00014BF0
		// (set) Token: 0x0600052E RID: 1326 RVA: 0x000169F8 File Offset: 0x00014BF8
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

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x0600052F RID: 1327 RVA: 0x00016A1B File Offset: 0x00014C1B
		// (set) Token: 0x06000530 RID: 1328 RVA: 0x00016A23 File Offset: 0x00014C23
		[DataSourceProperty]
		public string UsedWeaponName
		{
			get
			{
				return this._usedWeaponName;
			}
			set
			{
				if (value != this._usedWeaponName)
				{
					this._usedWeaponName = value;
					base.OnPropertyChangedWithValue<string>(value, "UsedWeaponName");
				}
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000531 RID: 1329 RVA: 0x00016A46 File Offset: 0x00014C46
		// (set) Token: 0x06000532 RID: 1330 RVA: 0x00016A4E File Offset: 0x00014C4E
		[DataSourceProperty]
		public string KillerName
		{
			get
			{
				return this._killerName;
			}
			set
			{
				if (value != this._killerName)
				{
					this._killerName = value;
					base.OnPropertyChangedWithValue<string>(value, "KillerName");
				}
			}
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000533 RID: 1331 RVA: 0x00016A71 File Offset: 0x00014C71
		// (set) Token: 0x06000534 RID: 1332 RVA: 0x00016A79 File Offset: 0x00014C79
		[DataSourceProperty]
		public string KillerText
		{
			get
			{
				return this._killerText;
			}
			set
			{
				if (value != this._killerText)
				{
					this._killerText = value;
					base.OnPropertyChangedWithValue<string>(value, "KillerText");
				}
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000535 RID: 1333 RVA: 0x00016A9C File Offset: 0x00014C9C
		// (set) Token: 0x06000536 RID: 1334 RVA: 0x00016AA4 File Offset: 0x00014CA4
		[DataSourceProperty]
		public string YouText
		{
			get
			{
				return this._youText;
			}
			set
			{
				if (value != this._youText)
				{
					this._youText = value;
					base.OnPropertyChangedWithValue<string>(value, "YouText");
				}
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000537 RID: 1335 RVA: 0x00016AC7 File Offset: 0x00014CC7
		// (set) Token: 0x06000538 RID: 1336 RVA: 0x00016ACF File Offset: 0x00014CCF
		[DataSourceProperty]
		public MPPlayerVM PlayerProperties
		{
			get
			{
				return this._playerProperties;
			}
			set
			{
				if (value != this._playerProperties)
				{
					this._playerProperties = value;
					base.OnPropertyChangedWithValue<MPPlayerVM>(value, "PlayerProperties");
				}
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000539 RID: 1337 RVA: 0x00016AED File Offset: 0x00014CED
		// (set) Token: 0x0600053A RID: 1338 RVA: 0x00016AF5 File Offset: 0x00014CF5
		[DataSourceProperty]
		public int BodyPartHit
		{
			get
			{
				return this._bodyPartHit;
			}
			set
			{
				if (value != this._bodyPartHit)
				{
					this._bodyPartHit = value;
					base.OnPropertyChangedWithValue(value, "BodyPartHit");
				}
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x0600053B RID: 1339 RVA: 0x00016B13 File Offset: 0x00014D13
		// (set) Token: 0x0600053C RID: 1340 RVA: 0x00016B1B File Offset: 0x00014D1B
		[DataSourceProperty]
		public int NumOfTimesPlayerKilled
		{
			get
			{
				return this._numOfTimesPlayerKilled;
			}
			set
			{
				if (value != this._numOfTimesPlayerKilled)
				{
					this._numOfTimesPlayerKilled = value;
					base.OnPropertyChangedWithValue(value, "NumOfTimesPlayerKilled");
				}
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x0600053D RID: 1341 RVA: 0x00016B39 File Offset: 0x00014D39
		// (set) Token: 0x0600053E RID: 1342 RVA: 0x00016B41 File Offset: 0x00014D41
		[DataSourceProperty]
		public int NumOfTimesPlayerGotKilled
		{
			get
			{
				return this._numOfTimesPlayerGotKilled;
			}
			set
			{
				if (value != this._numOfTimesPlayerGotKilled)
				{
					this._numOfTimesPlayerGotKilled = value;
					base.OnPropertyChangedWithValue(value, "NumOfTimesPlayerGotKilled");
				}
			}
		}

		// Token: 0x04000297 RID: 663
		private readonly TextObject _killedByStrayHorse = GameTexts.FindText("str_killed_by_stray_horse", null);

		// Token: 0x04000298 RID: 664
		private readonly TextObject _killedSelfText = GameTexts.FindText("str_killed_self", null);

		// Token: 0x04000299 RID: 665
		private readonly TextObject _killedByText = GameTexts.FindText("str_killed_by", null);

		// Token: 0x0400029A RID: 666
		private readonly TextObject _enemyText = GameTexts.FindText("str_death_card_enemy", null);

		// Token: 0x0400029B RID: 667
		private readonly TextObject _allyText = GameTexts.FindText("str_death_card_ally", null);

		// Token: 0x0400029C RID: 668
		private bool _isActive;

		// Token: 0x0400029D RID: 669
		private bool _isSelfInflicted;

		// Token: 0x0400029E RID: 670
		private bool _killCountsEnabled;

		// Token: 0x0400029F RID: 671
		private int _numOfTimesPlayerKilled;

		// Token: 0x040002A0 RID: 672
		private int _numOfTimesPlayerGotKilled;

		// Token: 0x040002A1 RID: 673
		private string _titleText;

		// Token: 0x040002A2 RID: 674
		private string _usedWeaponName;

		// Token: 0x040002A3 RID: 675
		private string _killerName;

		// Token: 0x040002A4 RID: 676
		private string _killerText;

		// Token: 0x040002A5 RID: 677
		private string _youText;

		// Token: 0x040002A6 RID: 678
		private MPPlayerVM _playerProperties;

		// Token: 0x040002A7 RID: 679
		private int _bodyPartHit;
	}
}
