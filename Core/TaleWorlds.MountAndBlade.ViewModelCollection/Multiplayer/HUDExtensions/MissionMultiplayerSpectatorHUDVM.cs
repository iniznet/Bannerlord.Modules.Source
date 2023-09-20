using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.HUDExtensions
{
	// Token: 0x020000BB RID: 187
	public class MissionMultiplayerSpectatorHUDVM : ViewModel
	{
		// Token: 0x060011EA RID: 4586 RVA: 0x0003AEE0 File Offset: 0x000390E0
		public MissionMultiplayerSpectatorHUDVM(Mission mission)
		{
			this._mission = mission;
			MissionLobbyComponent missionBehavior = mission.GetMissionBehavior<MissionLobbyComponent>();
			this._isTeamsEnabled = missionBehavior.MissionType != MissionLobbyComponent.MultiplayerGameType.FreeForAll && missionBehavior.MissionType != MissionLobbyComponent.MultiplayerGameType.Duel;
			this._isFlagDominationMode = Mission.Current.HasMissionBehavior<MissionMultiplayerGameModeFlagDominationClient>();
			this.RefreshValues();
		}

		// Token: 0x060011EB RID: 4587 RVA: 0x0003AF3C File Offset: 0x0003913C
		public override void RefreshValues()
		{
			base.RefreshValues();
			string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
			GameTexts.SetVariable("USE_KEY", keyHyperlinkText);
			this.TakeControlText = GameTexts.FindText("str_sergeant_battle_press_action_to_control_bot_2", null).ToString();
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x0003AF82 File Offset: 0x00039182
		public void Tick(float dt)
		{
			if (this._mission.MainAgent != null)
			{
				this.SpectatedPlayerNeutrality = -1;
			}
			this.UpdateDynamicProperties();
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x0003AFA0 File Offset: 0x000391A0
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

		// Token: 0x060011EE RID: 4590 RVA: 0x0003B260 File Offset: 0x00039460
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

		// Token: 0x060011EF RID: 4591 RVA: 0x0003B3BE File Offset: 0x000395BE
		internal void OnSpectatedAgentFocusOut(Agent followedPeer)
		{
			this._spectatedAgent = null;
			this.SpectatedPlayerNeutrality = -1;
		}

		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x060011F0 RID: 4592 RVA: 0x0003B3CE File Offset: 0x000395CE
		// (set) Token: 0x060011F1 RID: 4593 RVA: 0x0003B3D6 File Offset: 0x000395D6
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

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x060011F2 RID: 4594 RVA: 0x0003B401 File Offset: 0x00039601
		// (set) Token: 0x060011F3 RID: 4595 RVA: 0x0003B409 File Offset: 0x00039609
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

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x060011F4 RID: 4596 RVA: 0x0003B427 File Offset: 0x00039627
		// (set) Token: 0x060011F5 RID: 4597 RVA: 0x0003B42F File Offset: 0x0003962F
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

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x060011F6 RID: 4598 RVA: 0x0003B44D File Offset: 0x0003964D
		// (set) Token: 0x060011F7 RID: 4599 RVA: 0x0003B455 File Offset: 0x00039655
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

		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x060011F8 RID: 4600 RVA: 0x0003B473 File Offset: 0x00039673
		// (set) Token: 0x060011F9 RID: 4601 RVA: 0x0003B47B File Offset: 0x0003967B
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

		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x060011FA RID: 4602 RVA: 0x0003B499 File Offset: 0x00039699
		// (set) Token: 0x060011FB RID: 4603 RVA: 0x0003B4A1 File Offset: 0x000396A1
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

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x060011FC RID: 4604 RVA: 0x0003B4BF File Offset: 0x000396BF
		// (set) Token: 0x060011FD RID: 4605 RVA: 0x0003B4C7 File Offset: 0x000396C7
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

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x060011FE RID: 4606 RVA: 0x0003B4E5 File Offset: 0x000396E5
		// (set) Token: 0x060011FF RID: 4607 RVA: 0x0003B4ED File Offset: 0x000396ED
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

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x06001200 RID: 4608 RVA: 0x0003B50B File Offset: 0x0003970B
		// (set) Token: 0x06001201 RID: 4609 RVA: 0x0003B513 File Offset: 0x00039713
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

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06001202 RID: 4610 RVA: 0x0003B531 File Offset: 0x00039731
		// (set) Token: 0x06001203 RID: 4611 RVA: 0x0003B539 File Offset: 0x00039739
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

		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x06001204 RID: 4612 RVA: 0x0003B55C File Offset: 0x0003975C
		// (set) Token: 0x06001205 RID: 4613 RVA: 0x0003B564 File Offset: 0x00039764
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

		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x06001206 RID: 4614 RVA: 0x0003B587 File Offset: 0x00039787
		// (set) Token: 0x06001207 RID: 4615 RVA: 0x0003B58F File Offset: 0x0003978F
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

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x06001208 RID: 4616 RVA: 0x0003B5AD File Offset: 0x000397AD
		// (set) Token: 0x06001209 RID: 4617 RVA: 0x0003B5B5 File Offset: 0x000397B5
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

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x0600120A RID: 4618 RVA: 0x0003B5D3 File Offset: 0x000397D3
		// (set) Token: 0x0600120B RID: 4619 RVA: 0x0003B5DB File Offset: 0x000397DB
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

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x0600120C RID: 4620 RVA: 0x0003B5F9 File Offset: 0x000397F9
		// (set) Token: 0x0600120D RID: 4621 RVA: 0x0003B601 File Offset: 0x00039801
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

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x0600120E RID: 4622 RVA: 0x0003B61F File Offset: 0x0003981F
		// (set) Token: 0x0600120F RID: 4623 RVA: 0x0003B627 File Offset: 0x00039827
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

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x06001210 RID: 4624 RVA: 0x0003B645 File Offset: 0x00039845
		// (set) Token: 0x06001211 RID: 4625 RVA: 0x0003B64D File Offset: 0x0003984D
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

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x06001212 RID: 4626 RVA: 0x0003B66B File Offset: 0x0003986B
		// (set) Token: 0x06001213 RID: 4627 RVA: 0x0003B673 File Offset: 0x00039873
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

		// Token: 0x0400088B RID: 2187
		private readonly Mission _mission;

		// Token: 0x0400088C RID: 2188
		private readonly bool _isTeamsEnabled;

		// Token: 0x0400088D RID: 2189
		private readonly bool _isFlagDominationMode;

		// Token: 0x0400088E RID: 2190
		private Agent _spectatedAgent;

		// Token: 0x0400088F RID: 2191
		private string _spectatedPlayerName;

		// Token: 0x04000890 RID: 2192
		private string _takeControlText;

		// Token: 0x04000891 RID: 2193
		private int _spectatedPlayerNeutrality = -1;

		// Token: 0x04000892 RID: 2194
		private bool _isSpectatingPlayer;

		// Token: 0x04000893 RID: 2195
		private bool _canTakeControlOfSpectatedAgent;

		// Token: 0x04000894 RID: 2196
		private bool _agentHasMount;

		// Token: 0x04000895 RID: 2197
		private bool _agentHasShield;

		// Token: 0x04000896 RID: 2198
		private bool _showAgentHealth;

		// Token: 0x04000897 RID: 2199
		private bool _agentHasRangedWeapon;

		// Token: 0x04000898 RID: 2200
		private bool _agentHasCompassElement;

		// Token: 0x04000899 RID: 2201
		private float _spectatedPlayerHealthLimit;

		// Token: 0x0400089A RID: 2202
		private float _spectatedPlayerCurrentHealth;

		// Token: 0x0400089B RID: 2203
		private float _spectatedPlayerMountCurrentHealth;

		// Token: 0x0400089C RID: 2204
		private float _spectatedPlayerMountHealthLimit;

		// Token: 0x0400089D RID: 2205
		private float _spectatedPlayerShieldCurrentHealth;

		// Token: 0x0400089E RID: 2206
		private float _spectatedPlayerShieldHealthLimit;

		// Token: 0x0400089F RID: 2207
		private int _spectatedPlayerAmmoAmount;

		// Token: 0x040008A0 RID: 2208
		private MPTeammateCompassTargetVM _compassElement;
	}
}
