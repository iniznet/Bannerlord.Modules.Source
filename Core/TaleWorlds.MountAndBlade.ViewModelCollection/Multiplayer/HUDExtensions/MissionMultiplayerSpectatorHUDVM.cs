using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.HUDExtensions
{
	public class MissionMultiplayerSpectatorHUDVM : ViewModel
	{
		public MissionMultiplayerSpectatorHUDVM(Mission mission)
		{
			this._mission = mission;
			MissionLobbyComponent missionBehavior = mission.GetMissionBehavior<MissionLobbyComponent>();
			this._isTeamsEnabled = missionBehavior.MissionType != MissionLobbyComponent.MultiplayerGameType.FreeForAll && missionBehavior.MissionType != MissionLobbyComponent.MultiplayerGameType.Duel;
			this._isFlagDominationMode = Mission.Current.HasMissionBehavior<MissionMultiplayerGameModeFlagDominationClient>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
			GameTexts.SetVariable("USE_KEY", keyHyperlinkText);
			this.TakeControlText = GameTexts.FindText("str_sergeant_battle_press_action_to_control_bot_2", null).ToString();
		}

		public void Tick(float dt)
		{
			if (this._mission.MainAgent != null)
			{
				this.SpectatedPlayerNeutrality = -1;
			}
			this.UpdateDynamicProperties();
		}

		private void UpdateDynamicProperties()
		{
			this.AgentHasShield = false;
			this.AgentHasMount = false;
			this.ShowAgentHealth = false;
			this.AgentHasRangedWeapon = false;
			if (this.SpectatedPlayerNeutrality > 0 && this._spectatedAgent != null)
			{
				this.ShowAgentHealth = true;
				this.SpectatedPlayerHealthLimit = this._spectatedAgent.HealthLimit;
				this.SpectatedPlayerCurrentHealth = this._spectatedAgent.Health;
				this.AgentHasMount = this._spectatedAgent.MountAgent != null;
				if (this.AgentHasMount)
				{
					this.SpectatedPlayerMountCurrentHealth = this._spectatedAgent.MountAgent.Health;
					this.SpectatedPlayerMountHealthLimit = this._spectatedAgent.MountAgent.HealthLimit;
				}
				EquipmentIndex wieldedItemIndex = this._spectatedAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
				EquipmentIndex wieldedItemIndex2 = this._spectatedAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
				int num = -1;
				if (wieldedItemIndex != EquipmentIndex.None && this._spectatedAgent.Equipment[wieldedItemIndex].CurrentUsageItem != null)
				{
					if (this._spectatedAgent.Equipment[wieldedItemIndex].CurrentUsageItem.IsRangedWeapon && this._spectatedAgent.Equipment[wieldedItemIndex].CurrentUsageItem.IsConsumable)
					{
						int ammoAmount = this._spectatedAgent.Equipment.GetAmmoAmount(this._spectatedAgent.Equipment[wieldedItemIndex].CurrentUsageItem.AmmoClass);
						if (this._spectatedAgent.Equipment[wieldedItemIndex].ModifiedMaxAmount == 1 || ammoAmount > 0)
						{
							num = ((this._spectatedAgent.Equipment[wieldedItemIndex].ModifiedMaxAmount == 1) ? (-1) : ammoAmount);
						}
					}
					else if (this._spectatedAgent.Equipment[wieldedItemIndex].CurrentUsageItem.IsRangedWeapon)
					{
						bool flag = this._spectatedAgent.Equipment[wieldedItemIndex].CurrentUsageItem.WeaponClass == WeaponClass.Crossbow;
						num = this._spectatedAgent.Equipment.GetAmmoAmount(this._spectatedAgent.Equipment[wieldedItemIndex].CurrentUsageItem.AmmoClass) + (int)(flag ? this._spectatedAgent.Equipment[wieldedItemIndex].Ammo : 0);
					}
				}
				if (wieldedItemIndex2 != EquipmentIndex.None && this._spectatedAgent.Equipment[wieldedItemIndex2].CurrentUsageItem != null)
				{
					MissionWeapon missionWeapon = this._spectatedAgent.Equipment[wieldedItemIndex2];
					this.AgentHasShield = missionWeapon.CurrentUsageItem.IsShield;
					if (this.AgentHasShield)
					{
						this.SpectatedPlayerShieldHealthLimit = (float)missionWeapon.ModifiedMaxHitPoints;
						this.SpectatedPlayerShieldCurrentHealth = (float)missionWeapon.HitPoints;
					}
				}
				this.AgentHasRangedWeapon = num >= 0;
				this.SpectatedPlayerAmmoAmount = num;
			}
		}

		internal void OnSpectatedAgentFocusIn(Agent followedAgent)
		{
			this._spectatedAgent = followedAgent;
			int num = 0;
			MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
			if (component != null && component.Team != this._mission.SpectatorTeam && component.Team == followedAgent.Team && this._isTeamsEnabled)
			{
				num = 1;
			}
			this.SpectatedPlayerNeutrality = num;
			MissionPeer missionPeer = followedAgent.MissionPeer;
			this.SpectatedPlayerName = ((missionPeer != null) ? missionPeer.DisplayedName : null) ?? followedAgent.Name.ToString();
			this.CanTakeControlOfSpectatedAgent = this._isFlagDominationMode && ((component != null) ? component.ControlledFormation : null) != null && component.ControlledFormation == followedAgent.Formation;
			this.CompassElement = null;
			this.AgentHasCompassElement = false;
			MissionPeer missionPeer2;
			if ((missionPeer2 = followedAgent.MissionPeer) == null)
			{
				Formation formation = followedAgent.Formation;
				if (formation == null)
				{
					missionPeer2 = null;
				}
				else
				{
					Agent playerOwner = formation.PlayerOwner;
					missionPeer2 = ((playerOwner != null) ? playerOwner.MissionPeer : null);
				}
			}
			MissionPeer missionPeer3 = missionPeer2;
			if (missionPeer3 != null)
			{
				MultiplayerClassDivisions.MPHeroClass mpheroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(missionPeer3, false);
				TargetIconType targetIconType = ((mpheroClassForPeer != null) ? mpheroClassForPeer.IconType : TargetIconType.None);
				BannerCode bannerCode = BannerCode.CreateFrom(new Banner(missionPeer3.Peer.BannerCode, missionPeer3.Team.Color, missionPeer3.Team.Color2));
				this.CompassElement = new MPTeammateCompassTargetVM(targetIconType, missionPeer3.Team.Color, missionPeer3.Team.Color2, bannerCode, missionPeer3.Team.IsPlayerAlly);
				this.AgentHasCompassElement = true;
			}
		}

		internal void OnSpectatedAgentFocusOut(Agent followedPeer)
		{
			this._spectatedAgent = null;
			this.SpectatedPlayerNeutrality = -1;
		}

		[DataSourceProperty]
		public int SpectatedPlayerNeutrality
		{
			get
			{
				return this._spectatedPlayerNeutrality;
			}
			set
			{
				if (value != this._spectatedPlayerNeutrality)
				{
					this._spectatedPlayerNeutrality = value;
					base.OnPropertyChangedWithValue(value, "SpectatedPlayerNeutrality");
					this.IsSpectatingAgent = value >= 0;
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
		public bool IsSpectatingAgent
		{
			get
			{
				return this._isSpectatingPlayer;
			}
			set
			{
				if (value != this._isSpectatingPlayer)
				{
					this._isSpectatingPlayer = value;
					base.OnPropertyChangedWithValue(value, "IsSpectatingAgent");
				}
			}
		}

		[DataSourceProperty]
		public bool AgentHasCompassElement
		{
			get
			{
				return this._agentHasCompassElement;
			}
			set
			{
				if (value != this._agentHasCompassElement)
				{
					this._agentHasCompassElement = value;
					base.OnPropertyChangedWithValue(value, "AgentHasCompassElement");
				}
			}
		}

		[DataSourceProperty]
		public bool AgentHasMount
		{
			get
			{
				return this._agentHasMount;
			}
			set
			{
				if (value != this._agentHasMount)
				{
					this._agentHasMount = value;
					base.OnPropertyChangedWithValue(value, "AgentHasMount");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowAgentHealth
		{
			get
			{
				return this._showAgentHealth;
			}
			set
			{
				if (value != this._showAgentHealth)
				{
					this._showAgentHealth = value;
					base.OnPropertyChangedWithValue(value, "ShowAgentHealth");
				}
			}
		}

		[DataSourceProperty]
		public bool AgentHasRangedWeapon
		{
			get
			{
				return this._agentHasRangedWeapon;
			}
			set
			{
				if (value != this._agentHasRangedWeapon)
				{
					this._agentHasRangedWeapon = value;
					base.OnPropertyChangedWithValue(value, "AgentHasRangedWeapon");
				}
			}
		}

		[DataSourceProperty]
		public bool AgentHasShield
		{
			get
			{
				return this._agentHasShield;
			}
			set
			{
				if (value != this._agentHasShield)
				{
					this._agentHasShield = value;
					base.OnPropertyChangedWithValue(value, "AgentHasShield");
				}
			}
		}

		[DataSourceProperty]
		public bool CanTakeControlOfSpectatedAgent
		{
			get
			{
				return this._canTakeControlOfSpectatedAgent;
			}
			set
			{
				if (value != this._canTakeControlOfSpectatedAgent)
				{
					this._canTakeControlOfSpectatedAgent = value;
					base.OnPropertyChangedWithValue(value, "CanTakeControlOfSpectatedAgent");
				}
			}
		}

		[DataSourceProperty]
		public string SpectatedPlayerName
		{
			get
			{
				return this._spectatedPlayerName;
			}
			set
			{
				if (value != this._spectatedPlayerName)
				{
					this._spectatedPlayerName = value;
					base.OnPropertyChangedWithValue<string>(value, "SpectatedPlayerName");
				}
			}
		}

		[DataSourceProperty]
		public string TakeControlText
		{
			get
			{
				return this._takeControlText;
			}
			set
			{
				if (value != this._takeControlText)
				{
					this._takeControlText = value;
					base.OnPropertyChangedWithValue<string>(value, "TakeControlText");
				}
			}
		}

		[DataSourceProperty]
		public float SpectatedPlayerHealthLimit
		{
			get
			{
				return this._spectatedPlayerHealthLimit;
			}
			set
			{
				if (value != this._spectatedPlayerHealthLimit)
				{
					this._spectatedPlayerHealthLimit = value;
					base.OnPropertyChangedWithValue(value, "SpectatedPlayerHealthLimit");
				}
			}
		}

		[DataSourceProperty]
		public float SpectatedPlayerCurrentHealth
		{
			get
			{
				return this._spectatedPlayerCurrentHealth;
			}
			set
			{
				if (value != this._spectatedPlayerCurrentHealth)
				{
					this._spectatedPlayerCurrentHealth = value;
					base.OnPropertyChangedWithValue(value, "SpectatedPlayerCurrentHealth");
				}
			}
		}

		[DataSourceProperty]
		public float SpectatedPlayerMountCurrentHealth
		{
			get
			{
				return this._spectatedPlayerMountCurrentHealth;
			}
			set
			{
				if (value != this._spectatedPlayerMountCurrentHealth)
				{
					this._spectatedPlayerMountCurrentHealth = value;
					base.OnPropertyChangedWithValue(value, "SpectatedPlayerMountCurrentHealth");
				}
			}
		}

		[DataSourceProperty]
		public float SpectatedPlayerMountHealthLimit
		{
			get
			{
				return this._spectatedPlayerMountHealthLimit;
			}
			set
			{
				if (value != this._spectatedPlayerMountHealthLimit)
				{
					this._spectatedPlayerMountHealthLimit = value;
					base.OnPropertyChangedWithValue(value, "SpectatedPlayerMountHealthLimit");
				}
			}
		}

		[DataSourceProperty]
		public float SpectatedPlayerShieldCurrentHealth
		{
			get
			{
				return this._spectatedPlayerShieldCurrentHealth;
			}
			set
			{
				if (value != this._spectatedPlayerShieldCurrentHealth)
				{
					this._spectatedPlayerShieldCurrentHealth = value;
					base.OnPropertyChangedWithValue(value, "SpectatedPlayerShieldCurrentHealth");
				}
			}
		}

		[DataSourceProperty]
		public float SpectatedPlayerShieldHealthLimit
		{
			get
			{
				return this._spectatedPlayerShieldHealthLimit;
			}
			set
			{
				if (value != this._spectatedPlayerShieldHealthLimit)
				{
					this._spectatedPlayerShieldHealthLimit = value;
					base.OnPropertyChangedWithValue(value, "SpectatedPlayerShieldHealthLimit");
				}
			}
		}

		[DataSourceProperty]
		public int SpectatedPlayerAmmoAmount
		{
			get
			{
				return this._spectatedPlayerAmmoAmount;
			}
			set
			{
				if (value != this._spectatedPlayerAmmoAmount)
				{
					this._spectatedPlayerAmmoAmount = value;
					base.OnPropertyChangedWithValue(value, "SpectatedPlayerAmmoAmount");
				}
			}
		}

		private readonly Mission _mission;

		private readonly bool _isTeamsEnabled;

		private readonly bool _isFlagDominationMode;

		private Agent _spectatedAgent;

		private string _spectatedPlayerName;

		private string _takeControlText;

		private int _spectatedPlayerNeutrality = -1;

		private bool _isSpectatingPlayer;

		private bool _canTakeControlOfSpectatedAgent;

		private bool _agentHasMount;

		private bool _agentHasShield;

		private bool _showAgentHealth;

		private bool _agentHasRangedWeapon;

		private bool _agentHasCompassElement;

		private float _spectatedPlayerHealthLimit;

		private float _spectatedPlayerCurrentHealth;

		private float _spectatedPlayerMountCurrentHealth;

		private float _spectatedPlayerMountHealthLimit;

		private float _spectatedPlayerShieldCurrentHealth;

		private float _spectatedPlayerShieldHealthLimit;

		private int _spectatedPlayerAmmoAmount;

		private MPTeammateCompassTargetVM _compassElement;
	}
}
