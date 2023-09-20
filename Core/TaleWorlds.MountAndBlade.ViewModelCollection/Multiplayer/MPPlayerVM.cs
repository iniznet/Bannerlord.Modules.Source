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
	public class MPPlayerVM : ViewModel
	{
		public MissionPeer Peer { get; private set; }

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

		public void UpdateDisabled()
		{
			this.IsDead = !this.Peer.IsControlledAgentActive;
		}

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

		public void RefreshPreview(BasicCharacterObject character, DynamicBodyProperties dynamicBodyProperties, bool isFemale)
		{
			this.Preview = new MPArmoryHeroPreviewVM();
			this.Preview.SetCharacter(character, dynamicBodyProperties, character.Race, isFemale);
		}

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

		public void ExecuteFocusBegin()
		{
			this.SetFocusState(true);
		}

		public void ExecuteFocusEnd()
		{
			this.SetFocusState(false);
		}

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

		private MultiplayerClassDivisions.MPHeroClass _cachedClass;

		private BasicCultureObject _cachedCulture;

		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		private readonly MissionRepresentativeBase _missionRepresentative;

		private readonly bool _isInParty;

		private readonly bool _isKnownPlayer;

		private TextObject _genericPlayerName = new TextObject("{=RN6zHak0}Player", null);

		private const uint _focusedContourColor = 4278255612U;

		private const uint _defaultContourColor = 0U;

		private const uint _invalidColor = 0U;

		private int _gold;

		private int _valuePercent;

		private string _name;

		private string _cultureID;

		private bool _isDead;

		private bool _isValueEnabled;

		private bool _hasSetCompassElement;

		private bool _isSpawnActive;

		private bool _isFocused;

		private MPTeammateCompassTargetVM _compassElement;

		private ImageIdentifierVM _avatar = new ImageIdentifierVM(ImageIdentifierType.Null);

		private MPArmoryHeroPreviewVM _preview;

		private MBBindingList<MPPerkVM> _activePerks;
	}
}
