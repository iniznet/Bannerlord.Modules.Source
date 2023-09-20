using System;
using TaleWorlds.Avatar.PlayerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.Compass;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Armory;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x0200003E RID: 62
	public class MPPlayerVM : ViewModel
	{
		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000546 RID: 1350 RVA: 0x00016CBC File Offset: 0x00014EBC
		// (set) Token: 0x06000547 RID: 1351 RVA: 0x00016CC4 File Offset: 0x00014EC4
		public MissionPeer Peer { get; private set; }

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000548 RID: 1352 RVA: 0x00016CD0 File Offset: 0x00014ED0
		private Team _playerTeam
		{
			get
			{
				if (!GameNetwork.IsMyPeerReady)
				{
					return null;
				}
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				if (component.Team == null || component.Team.Side == BattleSideEnum.None)
				{
					return null;
				}
				return component.Team;
			}
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x00016D10 File Offset: 0x00014F10
		public MPPlayerVM(Agent agent)
		{
			if (agent != null)
			{
				MultiplayerClassDivisions.MPHeroClass mpheroClassForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character);
				TargetIconType targetIconType = ((mpheroClassForCharacter != null) ? mpheroClassForCharacter.IconType : TargetIconType.None);
				Team team = agent.Team;
				uint num = ((team != null) ? team.Color : 0U);
				Team team2 = agent.Team;
				uint num2 = ((team2 != null) ? team2.Color2 : 0U);
				BannerCode bannerCode = BannerCode.CreateFrom(new Banner(agent.Team.Banner.Serialize(), num, num2));
				this.CompassElement = new MPTeammateCompassTargetVM(targetIconType, num, num2, bannerCode, false);
				return;
			}
			this.CompassElement = new MPTeammateCompassTargetVM(TargetIconType.Monster, 0U, 0U, BannerCode.CreateFrom(Banner.CreateOneColoredEmptyBanner(0)), false);
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x00016DCC File Offset: 0x00014FCC
		public MPPlayerVM(MissionPeer peer)
		{
			this.Peer = peer;
			this._gameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this._missionRepresentative = peer.GetComponent<MissionRepresentativeBase>();
			if (this.Peer == null)
			{
				this.CompassElement = new MPTeammateCompassTargetVM(TargetIconType.None, 0U, 0U, null, false);
				return;
			}
			this._isInParty = NetworkMain.GameClient.IsInParty;
			this._isKnownPlayer = NetworkMain.GameClient.IsKnownPlayer(this.Peer.Peer.Id);
			this.RefreshAvatar();
			this.Name = peer.DisplayedName;
			this.ActivePerks = new MBBindingList<MPPerkVM>();
			this.RefreshValues();
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x00016E8B File Offset: 0x0001508B
		public void UpdateDisabled()
		{
			this.IsDead = !this.Peer.IsControlledAgentActive;
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x00016EA4 File Offset: 0x000150A4
		public void RefreshDivision(bool useCultureColors = false)
		{
			if (this.Peer == null || this.Peer.Culture == null)
			{
				return;
			}
			MultiplayerClassDivisions.MPHeroClass mpheroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(this.Peer, false);
			TargetIconType targetIconType = ((mpheroClassForPeer != null) ? mpheroClassForPeer.IconType : TargetIconType.None);
			if (this._cachedClass == null || this._cachedClass != mpheroClassForPeer || this._cachedCulture == null || this._cachedCulture != this.Peer.Culture)
			{
				this._cachedClass = mpheroClassForPeer;
				this._cachedCulture = this.Peer.Culture;
				Team team = this.Peer.Team;
				uint num = ((team != null) ? team.Color : 0U);
				Team team2 = this.Peer.Team;
				uint num2 = ((team2 != null) ? team2.Color2 : 0U);
				if (useCultureColors)
				{
					BasicCultureObject culture = this.Peer.Culture;
					num = ((culture != null) ? culture.ForegroundColor1 : 0U);
					BasicCultureObject culture2 = this.Peer.Culture;
					num2 = ((culture2 != null) ? culture2.BackgroundColor1 : 0U);
				}
				BannerCode bannerCode = BannerCode.CreateFrom(new Banner(this.Peer.Peer.BannerCode, num, num2));
				TargetIconType targetIconType2 = targetIconType;
				uint num3 = num;
				uint num4 = num2;
				BannerCode bannerCode2 = bannerCode;
				Team team3 = this.Peer.Team;
				this.CompassElement = new MPTeammateCompassTargetVM(targetIconType2, num3, num4, bannerCode2, team3 != null && team3.IsPlayerAlly);
				this.HasSetCompassElement = true;
				this.Name = this.Peer.DisplayedName;
				this.RefreshActivePerks();
				this.CultureID = this._cachedCulture.StringId;
			}
			this.CompassElement.RefreshTargetIconType(targetIconType);
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0001700C File Offset: 0x0001520C
		public void RefreshGold()
		{
			if (this.Peer != null && this._gameMode.IsGameModeUsingGold)
			{
				FlagDominationMissionRepresentative flagDominationMissionRepresentative;
				if ((flagDominationMissionRepresentative = this._missionRepresentative as FlagDominationMissionRepresentative) != null)
				{
					this.Gold = flagDominationMissionRepresentative.Gold;
					this.IsSpawnActive = this.Gold >= 100;
					return;
				}
			}
			else
			{
				this.IsSpawnActive = false;
			}
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x00017064 File Offset: 0x00015264
		public void RefreshTeam()
		{
			if (this.Peer == null)
			{
				return;
			}
			string bannerCode = this.Peer.Peer.BannerCode;
			Team team = this.Peer.Team;
			uint num = ((team != null) ? team.Color : 0U);
			Team team2 = this.Peer.Team;
			BannerCode bannerCode2 = BannerCode.CreateFrom(new Banner(bannerCode, num, (team2 != null) ? team2.Color2 : 0U));
			MPTeammateCompassTargetVM compassElement = this.CompassElement;
			BannerCode bannerCode3 = bannerCode2;
			Team team3 = this.Peer.Team;
			compassElement.RefreshTeam(bannerCode3, team3 != null && team3.IsPlayerAlly);
			CompassTargetVM compassElement2 = this.CompassElement;
			Team team4 = this.Peer.Team;
			uint num2 = ((team4 != null) ? team4.Color : 0U);
			Team team5 = this.Peer.Team;
			compassElement2.RefreshColor(num2, (team5 != null) ? team5.Color2 : 0U);
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x00017120 File Offset: 0x00015320
		public void RefreshProperties()
		{
			bool flag = MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0;
			MissionPeer peer = this.Peer;
			this.IsValueEnabled = (((peer != null) ? peer.Team : null) != null && this.Peer.Team == this._playerTeam) || flag;
			if (this.IsValueEnabled)
			{
				if (flag)
				{
					this.ValuePercent = ((this.Peer.BotsUnderControlTotal != 0) ? ((int)((float)this.Peer.BotsUnderControlAlive / (float)this.Peer.BotsUnderControlTotal * 100f)) : 0);
					return;
				}
				this.ValuePercent = ((this.Peer.ControlledAgent != null) ? MathF.Ceiling(this.Peer.ControlledAgent.Health / this.Peer.ControlledAgent.HealthLimit * 100f) : 0);
			}
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x000171EF File Offset: 0x000153EF
		public void RefreshPreview(BasicCharacterObject character, DynamicBodyProperties dynamicBodyProperties, bool isFemale)
		{
			this.Preview = new MPArmoryHeroPreviewVM();
			this.Preview.SetCharacter(character, dynamicBodyProperties, character.Race, isFemale);
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x00017210 File Offset: 0x00015410
		public void RefreshActivePerks()
		{
			this.ActivePerks.Clear();
			MultiplayerClassDivisions.MPHeroClass mpheroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(this.Peer, false);
			if (this.Peer != null && this.Peer.Culture != null && mpheroClassForPeer != null)
			{
				foreach (MPPerkObject mpperkObject in this.Peer.SelectedPerks)
				{
					this.ActivePerks.Add(new MPPerkVM(null, mpperkObject, false, 0));
				}
			}
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x000172A8 File Offset: 0x000154A8
		public void RefreshAvatar()
		{
			int num;
			if (!NetworkMain.GameClient.HasUserGeneratedContentPrivilege)
			{
				num = AvatarServices.GetForcedAvatarIndexOfPlayer(this.Peer.Peer.Id);
			}
			else
			{
				num = ((!BannerlordConfig.EnableGenericAvatars || this._isKnownPlayer) ? (-1) : AvatarServices.GetForcedAvatarIndexOfPlayer(this.Peer.Peer.Id));
			}
			this.Avatar = new ImageIdentifierVM(this.Peer.Peer.Id, num);
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x0001731F File Offset: 0x0001551F
		public void ExecuteFocusBegin()
		{
			this.SetFocusState(true);
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x00017328 File Offset: 0x00015528
		public void ExecuteFocusEnd()
		{
			this.SetFocusState(false);
		}

		// Token: 0x06000555 RID: 1365 RVA: 0x00017334 File Offset: 0x00015534
		private void SetFocusState(bool isFocused)
		{
			uint num = (isFocused ? 4278255612U : 0U);
			if (this.Peer != null)
			{
				IAgentVisual agentVisualForPeer = this.Peer.GetAgentVisualForPeer(0);
				if (agentVisualForPeer != null)
				{
					agentVisualForPeer.GetCopyAgentVisualsData().AgentVisuals.SetContourColor(new uint?(num), true);
				}
			}
			this.IsFocused = isFocused;
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x00017383 File Offset: 0x00015583
		// (set) Token: 0x06000557 RID: 1367 RVA: 0x0001738B File Offset: 0x0001558B
		[DataSourceProperty]
		public int Gold
		{
			get
			{
				return this._gold;
			}
			set
			{
				if (value != this._gold)
				{
					this._gold = value;
					base.OnPropertyChangedWithValue(value, "Gold");
				}
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000558 RID: 1368 RVA: 0x000173A9 File Offset: 0x000155A9
		// (set) Token: 0x06000559 RID: 1369 RVA: 0x000173B1 File Offset: 0x000155B1
		[DataSourceProperty]
		public int ValuePercent
		{
			get
			{
				return this._valuePercent;
			}
			set
			{
				if (value != this._valuePercent)
				{
					this._valuePercent = value;
					base.OnPropertyChangedWithValue(value, "ValuePercent");
				}
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x0600055A RID: 1370 RVA: 0x000173CF File Offset: 0x000155CF
		// (set) Token: 0x0600055B RID: 1371 RVA: 0x000173D7 File Offset: 0x000155D7
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x0600055C RID: 1372 RVA: 0x000173FA File Offset: 0x000155FA
		// (set) Token: 0x0600055D RID: 1373 RVA: 0x00017402 File Offset: 0x00015602
		[DataSourceProperty]
		public string CultureID
		{
			get
			{
				return this._cultureID;
			}
			set
			{
				if (value != this._cultureID)
				{
					this._cultureID = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureID");
				}
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x0600055E RID: 1374 RVA: 0x00017425 File Offset: 0x00015625
		// (set) Token: 0x0600055F RID: 1375 RVA: 0x0001742D File Offset: 0x0001562D
		[DataSourceProperty]
		public bool IsDead
		{
			get
			{
				return this._isDead;
			}
			set
			{
				if (value != this._isDead)
				{
					this._isDead = value;
					base.OnPropertyChangedWithValue(value, "IsDead");
				}
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000560 RID: 1376 RVA: 0x0001744B File Offset: 0x0001564B
		// (set) Token: 0x06000561 RID: 1377 RVA: 0x00017453 File Offset: 0x00015653
		[DataSourceProperty]
		public bool IsValueEnabled
		{
			get
			{
				return this._isValueEnabled;
			}
			set
			{
				if (value != this._isValueEnabled)
				{
					this._isValueEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsValueEnabled");
				}
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000562 RID: 1378 RVA: 0x00017471 File Offset: 0x00015671
		// (set) Token: 0x06000563 RID: 1379 RVA: 0x00017479 File Offset: 0x00015679
		[DataSourceProperty]
		public bool HasSetCompassElement
		{
			get
			{
				return this._hasSetCompassElement;
			}
			set
			{
				if (value != this._hasSetCompassElement)
				{
					this._hasSetCompassElement = value;
					base.OnPropertyChangedWithValue(value, "HasSetCompassElement");
				}
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000564 RID: 1380 RVA: 0x00017497 File Offset: 0x00015697
		// (set) Token: 0x06000565 RID: 1381 RVA: 0x0001749F File Offset: 0x0001569F
		[DataSourceProperty]
		public bool IsSpawnActive
		{
			get
			{
				return this._isSpawnActive;
			}
			set
			{
				if (value != this._isSpawnActive)
				{
					this._isSpawnActive = value;
					base.OnPropertyChangedWithValue(value, "IsSpawnActive");
				}
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000566 RID: 1382 RVA: 0x000174BD File Offset: 0x000156BD
		// (set) Token: 0x06000567 RID: 1383 RVA: 0x000174C5 File Offset: 0x000156C5
		[DataSourceProperty]
		public bool IsFocused
		{
			get
			{
				return this._isFocused;
			}
			set
			{
				if (value != this._isFocused)
				{
					this._isFocused = value;
					base.OnPropertyChangedWithValue(value, "IsFocused");
				}
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000568 RID: 1384 RVA: 0x000174E3 File Offset: 0x000156E3
		// (set) Token: 0x06000569 RID: 1385 RVA: 0x000174EB File Offset: 0x000156EB
		[DataSourceProperty]
		public MPTeammateCompassTargetVM CompassElement
		{
			get
			{
				return this._compassElement;
			}
			set
			{
				if (value != this._compassElement)
				{
					this._compassElement = value;
					base.OnPropertyChangedWithValue<MPTeammateCompassTargetVM>(value, "CompassElement");
				}
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x0600056A RID: 1386 RVA: 0x00017509 File Offset: 0x00015709
		// (set) Token: 0x0600056B RID: 1387 RVA: 0x00017511 File Offset: 0x00015711
		[DataSourceProperty]
		public ImageIdentifierVM Avatar
		{
			get
			{
				return this._avatar;
			}
			set
			{
				if (value != this._avatar)
				{
					this._avatar = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Avatar");
				}
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x0600056C RID: 1388 RVA: 0x0001752F File Offset: 0x0001572F
		// (set) Token: 0x0600056D RID: 1389 RVA: 0x00017537 File Offset: 0x00015737
		[DataSourceProperty]
		public MPArmoryHeroPreviewVM Preview
		{
			get
			{
				return this._preview;
			}
			set
			{
				if (value != this._preview)
				{
					this._preview = value;
					base.OnPropertyChangedWithValue<MPArmoryHeroPreviewVM>(value, "Preview");
				}
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x0600056E RID: 1390 RVA: 0x00017555 File Offset: 0x00015755
		// (set) Token: 0x0600056F RID: 1391 RVA: 0x0001755D File Offset: 0x0001575D
		[DataSourceProperty]
		public MBBindingList<MPPerkVM> ActivePerks
		{
			get
			{
				return this._activePerks;
			}
			set
			{
				if (value != this._activePerks)
				{
					this._activePerks = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPPerkVM>>(value, "ActivePerks");
				}
			}
		}

		// Token: 0x040002AE RID: 686
		private MultiplayerClassDivisions.MPHeroClass _cachedClass;

		// Token: 0x040002AF RID: 687
		private BasicCultureObject _cachedCulture;

		// Token: 0x040002B0 RID: 688
		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		// Token: 0x040002B1 RID: 689
		private readonly MissionRepresentativeBase _missionRepresentative;

		// Token: 0x040002B2 RID: 690
		private readonly bool _isInParty;

		// Token: 0x040002B3 RID: 691
		private readonly bool _isKnownPlayer;

		// Token: 0x040002B4 RID: 692
		private TextObject _genericPlayerName = new TextObject("{=RN6zHak0}Player", null);

		// Token: 0x040002B5 RID: 693
		private const uint _focusedContourColor = 4278255612U;

		// Token: 0x040002B6 RID: 694
		private const uint _defaultContourColor = 0U;

		// Token: 0x040002B7 RID: 695
		private const uint _invalidColor = 0U;

		// Token: 0x040002B8 RID: 696
		private int _gold;

		// Token: 0x040002B9 RID: 697
		private int _valuePercent;

		// Token: 0x040002BA RID: 698
		private string _name;

		// Token: 0x040002BB RID: 699
		private string _cultureID;

		// Token: 0x040002BC RID: 700
		private bool _isDead;

		// Token: 0x040002BD RID: 701
		private bool _isValueEnabled;

		// Token: 0x040002BE RID: 702
		private bool _hasSetCompassElement;

		// Token: 0x040002BF RID: 703
		private bool _isSpawnActive;

		// Token: 0x040002C0 RID: 704
		private bool _isFocused;

		// Token: 0x040002C1 RID: 705
		private MPTeammateCompassTargetVM _compassElement;

		// Token: 0x040002C2 RID: 706
		private ImageIdentifierVM _avatar = new ImageIdentifierVM(ImageIdentifierType.Null);

		// Token: 0x040002C3 RID: 707
		private MPArmoryHeroPreviewVM _preview;

		// Token: 0x040002C4 RID: 708
		private MBBindingList<MPPerkVM> _activePerks;
	}
}
