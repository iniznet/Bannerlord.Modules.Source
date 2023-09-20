using System;
using System.ComponentModel;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.DamageFeed;

namespace TaleWorlds.MountAndBlade.ViewModelCollection
{
	public class MissionAgentStatusVM : ViewModel
	{
		public bool IsInDeployement { get; set; }

		private MissionPeer _myMissionPeer
		{
			get
			{
				if (this._missionPeer != null)
				{
					return this._missionPeer;
				}
				if (GameNetwork.MyPeer != null)
				{
					this._missionPeer = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				}
				return this._missionPeer;
			}
		}

		public MissionAgentStatusVM(Mission mission, Camera missionCamera, Func<float> getCameraToggleProgress)
		{
			this.InteractionInterface = new AgentInteractionInterfaceVM(mission);
			this._mission = mission;
			this._missionCamera = missionCamera;
			this._getCameraToggleProgress = getCameraToggleProgress;
			this.PrimaryWeapon = new ImageIdentifierVM(ImageIdentifierType.Item);
			this.OffhandWeapon = new ImageIdentifierVM(ImageIdentifierType.Item);
			this.TakenDamageFeed = new MissionAgentDamageFeedVM();
			this.TakenDamageController = new MissionAgentTakenDamageVM(this._missionCamera);
			this.IsInteractionAvailable = true;
			this.RefreshValues();
		}

		public void InitializeMainAgentPropterties()
		{
			Mission.Current.OnMainAgentChanged += this.OnMainAgentChanged;
			this.OnMainAgentChanged(this._mission, null);
			this.OnMainAgentWeaponChange();
			this._mpGameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CameraToggleText = GameTexts.FindText("str_toggle_camera", null).ToString();
		}

		private void OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (Agent.Main != null)
			{
				Agent main = Agent.Main;
				main.OnMainAgentWieldedItemChange = (Agent.OnMainAgentWieldedItemChangeDelegate)Delegate.Combine(main.OnMainAgentWieldedItemChange, new Agent.OnMainAgentWieldedItemChangeDelegate(this.OnMainAgentWeaponChange));
				this.OnMainAgentWeaponChange();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			Mission.Current.OnMainAgentChanged -= this.OnMainAgentChanged;
			this.TakenDamageFeed.OnFinalize();
		}

		public void Tick(float dt)
		{
			if (this._mission == null)
			{
				return;
			}
			this.CouchLanceState = this.GetCouchLanceState();
			this.SpearBraceState = this.GetSpearBraceState();
			Func<float> getCameraToggleProgress = this._getCameraToggleProgress;
			this.CameraToggleProgress = ((getCameraToggleProgress != null) ? getCameraToggleProgress() : 0f);
			if (this._mission.MainAgent != null && !this.IsInDeployement)
			{
				this.ShowAgentHealthBar = true;
				this.InteractionInterface.Tick();
				if (this._mission.Mode == MissionMode.Battle && !this._mission.IsFriendlyMission && this._myMissionPeer != null)
				{
					MissionPeer myMissionPeer = this._myMissionPeer;
					this.IsTroopsActive = ((myMissionPeer != null) ? myMissionPeer.ControlledFormation : null) != null;
					if (this.IsTroopsActive)
					{
						this.TroopCount = this._myMissionPeer.ControlledFormation.CountOfUnits;
						FormationClass defaultFormationGroup = (FormationClass)MultiplayerClassDivisions.GetMPHeroClassForPeer(this._myMissionPeer, false).TroopCharacter.DefaultFormationGroup;
						this.TroopsAmmoAvailable = defaultFormationGroup == FormationClass.Ranged || defaultFormationGroup == FormationClass.HorseArcher;
						if (this.TroopsAmmoAvailable)
						{
							int totalCurrentAmmo = 0;
							int totalMaxAmmo = 0;
							this._myMissionPeer.ControlledFormation.ApplyActionOnEachUnit(delegate(Agent agent)
							{
								if (!agent.IsMainAgent)
								{
									int num;
									int num2;
									this.GetMaxAndCurrentAmmoOfAgent(agent, out num, out num2);
									totalCurrentAmmo += num;
									totalMaxAmmo += num2;
								}
							}, null);
							this.TroopsAmmoPercentage = (float)totalCurrentAmmo / (float)totalMaxAmmo;
						}
					}
				}
				this.UpdateWeaponStatuses();
				this.UpdateAgentAndMountStatuses();
				this.IsPlayerActive = true;
				this.IsCombatUIActive = true;
			}
			else
			{
				this.AgentHealth = 0;
				this.ShowMountHealthBar = false;
				this.ShowShieldHealthBar = false;
				if (this.IsCombatUIActive)
				{
					this._combatUIRemainTimer += dt;
					if (this._combatUIRemainTimer >= 3f)
					{
						this.IsCombatUIActive = false;
					}
				}
			}
			MissionMultiplayerGameModeBaseClient mpGameMode = this._mpGameMode;
			this.IsGoldActive = mpGameMode != null && mpGameMode.IsGameModeUsingGold;
			if (this.IsGoldActive && this._myMissionPeer != null && this._myMissionPeer.GetNetworkPeer().IsSynchronized)
			{
				MissionMultiplayerGameModeBaseClient mpGameMode2 = this._mpGameMode;
				this.GoldAmount = ((mpGameMode2 != null) ? mpGameMode2.GetGoldAmount() : 0);
			}
			MissionAgentTakenDamageVM takenDamageController = this.TakenDamageController;
			if (takenDamageController == null)
			{
				return;
			}
			takenDamageController.Tick(dt);
		}

		private void UpdateWeaponStatuses()
		{
			bool flag = false;
			if (this._mission.MainAgent != null)
			{
				int num = -1;
				EquipmentIndex wieldedItemIndex = this._mission.MainAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
				EquipmentIndex wieldedItemIndex2 = this._mission.MainAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
				if (wieldedItemIndex != EquipmentIndex.None && this._mission.MainAgent.Equipment[wieldedItemIndex].CurrentUsageItem != null)
				{
					if (this._mission.MainAgent.Equipment[wieldedItemIndex].CurrentUsageItem.IsRangedWeapon && this._mission.MainAgent.Equipment[wieldedItemIndex].CurrentUsageItem.IsConsumable)
					{
						int num2;
						if (!this._mission.MainAgent.Equipment[wieldedItemIndex].Item.PrimaryWeapon.IsConsumable && this._mission.MainAgent.Equipment[wieldedItemIndex].CurrentUsageItem.IsConsumable)
						{
							num2 = 1;
						}
						else
						{
							num2 = this._mission.MainAgent.Equipment.GetAmmoAmount(wieldedItemIndex);
						}
						if (this._mission.MainAgent.Equipment[wieldedItemIndex].ModifiedMaxAmount == 1 || num2 > 0)
						{
							num = num2;
						}
					}
					else if (this._mission.MainAgent.Equipment[wieldedItemIndex].CurrentUsageItem.IsRangedWeapon)
					{
						bool flag2 = this._mission.MainAgent.Equipment[wieldedItemIndex].CurrentUsageItem.WeaponClass == WeaponClass.Crossbow;
						num = this._mission.MainAgent.Equipment.GetAmmoAmount(wieldedItemIndex) + (int)(flag2 ? this._mission.MainAgent.Equipment[wieldedItemIndex].Ammo : 0);
					}
					if (!this._mission.MainAgent.Equipment[wieldedItemIndex].IsEmpty)
					{
						int num3;
						if (!this._mission.MainAgent.Equipment[wieldedItemIndex].Item.PrimaryWeapon.IsConsumable && this._mission.MainAgent.Equipment[wieldedItemIndex].CurrentUsageItem.IsConsumable)
						{
							num3 = 1;
						}
						else
						{
							num3 = this._mission.MainAgent.Equipment.GetMaxAmmo(wieldedItemIndex);
						}
						float num4 = (float)num3 * 0.2f;
						flag = num3 != this.AmmoCount && this.AmmoCount <= MathF.Ceiling(num4);
					}
				}
				if (wieldedItemIndex2 != EquipmentIndex.None && this._mission.MainAgent.Equipment[wieldedItemIndex2].CurrentUsageItem != null)
				{
					MissionWeapon missionWeapon = this._mission.MainAgent.Equipment[wieldedItemIndex2];
					this.ShowShieldHealthBar = missionWeapon.CurrentUsageItem.IsShield;
					if (this.ShowShieldHealthBar)
					{
						this.ShieldHealthMax = (int)missionWeapon.ModifiedMaxHitPoints;
						this.ShieldHealth = (int)missionWeapon.HitPoints;
					}
				}
				this.AmmoCount = num;
			}
			else
			{
				this.ShieldHealth = 0;
				this.AmmoCount = 0;
				this.ShowShieldHealthBar = false;
			}
			this.IsAmmoCountAlertEnabled = flag;
		}

		public void OnEquipmentInteractionViewToggled(bool isActive)
		{
			this.IsInteractionAvailable = !isActive;
		}

		private void UpdateAgentAndMountStatuses()
		{
			if (this._mission.MainAgent == null)
			{
				this.AgentHealthMax = 1;
				this.AgentHealth = (int)this._mission.MainAgent.Health;
				this.HorseHealthMax = 1;
				this.HorseHealth = 0;
				this.ShowMountHealthBar = false;
				return;
			}
			this.AgentHealthMax = (int)this._mission.MainAgent.HealthLimit;
			this.AgentHealth = (int)this._mission.MainAgent.Health;
			if (this._mission.MainAgent.MountAgent != null)
			{
				this.HorseHealthMax = (int)this._mission.MainAgent.MountAgent.HealthLimit;
				this.HorseHealth = (int)this._mission.MainAgent.MountAgent.Health;
				this.ShowMountHealthBar = true;
				return;
			}
			this.ShowMountHealthBar = false;
		}

		public void OnMainAgentWeaponChange()
		{
			if (this._mission.MainAgent == null)
			{
				return;
			}
			MissionWeapon missionWeapon = MissionWeapon.Invalid;
			MissionWeapon missionWeapon2 = MissionWeapon.Invalid;
			EquipmentIndex equipmentIndex = this._mission.MainAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			if (equipmentIndex > EquipmentIndex.None && equipmentIndex < EquipmentIndex.NumAllWeaponSlots)
			{
				missionWeapon = this._mission.MainAgent.Equipment[equipmentIndex];
			}
			equipmentIndex = this._mission.MainAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			if (equipmentIndex > EquipmentIndex.None && equipmentIndex < EquipmentIndex.NumAllWeaponSlots)
			{
				missionWeapon2 = this._mission.MainAgent.Equipment[equipmentIndex];
			}
			WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
			this.ShowShieldHealthBar = currentUsageItem != null && currentUsageItem.IsShield;
			this.PrimaryWeapon = (missionWeapon2.IsEmpty ? new ImageIdentifierVM(ImageIdentifierType.Null) : new ImageIdentifierVM(missionWeapon2.Item, ""));
			this.OffhandWeapon = (missionWeapon.IsEmpty ? new ImageIdentifierVM(ImageIdentifierType.Null) : new ImageIdentifierVM(missionWeapon.Item, ""));
		}

		public void OnAgentRemoved(Agent agent)
		{
			this.InteractionInterface.CheckAndClearFocusedAgent(agent);
		}

		public void OnAgentDeleted(Agent agent)
		{
			this.InteractionInterface.CheckAndClearFocusedAgent(agent);
		}

		public void OnMainAgentHit(int damage, float distance)
		{
			this.TakenDamageController.OnMainAgentHit(damage, distance);
		}

		public void OnFocusGained(Agent mainAgent, IFocusable focusableObject, bool isInteractable)
		{
			this.InteractionInterface.OnFocusGained(mainAgent, focusableObject, isInteractable);
		}

		public void OnFocusLost(Agent agent, IFocusable focusableObject)
		{
			this.InteractionInterface.OnFocusLost(agent, focusableObject);
		}

		public void OnSecondaryFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
		{
		}

		public void OnSecondaryFocusLost(Agent agent, IFocusable focusableObject)
		{
		}

		public void OnAgentInteraction(Agent userAgent, Agent agent)
		{
			this.InteractionInterface.OnAgentInteraction(userAgent, agent);
		}

		private void GetMaxAndCurrentAmmoOfAgent(Agent agent, out int currentAmmo, out int maxAmmo)
		{
			currentAmmo = 0;
			maxAmmo = 0;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
			{
				if (!agent.Equipment[equipmentIndex].IsEmpty && agent.Equipment[equipmentIndex].CurrentUsageItem.IsRangedWeapon)
				{
					currentAmmo = agent.Equipment.GetAmmoAmount(equipmentIndex);
					maxAmmo = agent.Equipment.GetMaxAmmo(equipmentIndex);
					return;
				}
			}
		}

		private int GetCouchLanceState()
		{
			int num = 0;
			if (Agent.Main != null)
			{
				MissionWeapon wieldedWeapon = Agent.Main.WieldedWeapon;
				if (Agent.Main.HasMount && this.IsWeaponCouchable(wieldedWeapon))
				{
					if (this.IsPassiveUsageActiveWithCurrentWeapon(wieldedWeapon))
					{
						num = 3;
					}
					else if (this.IsConditionsMetForCouching())
					{
						num = 2;
					}
				}
			}
			return num;
		}

		private bool IsWeaponCouchable(MissionWeapon weapon)
		{
			if (weapon.IsEmpty)
			{
				return false;
			}
			foreach (WeaponComponentData weaponComponentData in weapon.Item.Weapons)
			{
				string weaponDescriptionId = weaponComponentData.WeaponDescriptionId;
				if (weaponDescriptionId != null && weaponDescriptionId.IndexOf("couch", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsConditionsMetForCouching()
		{
			return Agent.Main.HasMount && Agent.Main.IsPassiveUsageConditionsAreMet;
		}

		private int GetSpearBraceState()
		{
			int num = 0;
			if (Agent.Main != null)
			{
				MissionWeapon wieldedWeapon = Agent.Main.WieldedWeapon;
				if (!Agent.Main.HasMount && Agent.Main.GetWieldedItemIndex(Agent.HandIndex.OffHand) == EquipmentIndex.None && this.IsWeaponBracable(wieldedWeapon))
				{
					if (this.IsPassiveUsageActiveWithCurrentWeapon(wieldedWeapon))
					{
						num = 3;
					}
					else if (this.IsConditionsMetForBracing())
					{
						num = 2;
					}
				}
			}
			return num;
		}

		private bool IsWeaponBracable(MissionWeapon weapon)
		{
			if (weapon.IsEmpty)
			{
				return false;
			}
			foreach (WeaponComponentData weaponComponentData in weapon.Item.Weapons)
			{
				string weaponDescriptionId = weaponComponentData.WeaponDescriptionId;
				if (weaponDescriptionId != null && weaponDescriptionId.IndexOf("bracing", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsConditionsMetForBracing()
		{
			return !Agent.Main.HasMount && !Agent.Main.WalkMode && Agent.Main.IsPassiveUsageConditionsAreMet;
		}

		private bool IsPassiveUsageActiveWithCurrentWeapon(MissionWeapon weapon)
		{
			return !weapon.IsEmpty && MBItem.GetItemIsPassiveUsage(weapon.CurrentUsageItem.ItemUsage);
		}

		[DataSourceProperty]
		public MissionAgentTakenDamageVM TakenDamageController
		{
			get
			{
				return this._takenDamageController;
			}
			set
			{
				if (value != this._takenDamageController)
				{
					this._takenDamageController = value;
					base.OnPropertyChangedWithValue<MissionAgentTakenDamageVM>(value, "TakenDamageController");
				}
			}
		}

		[DataSourceProperty]
		public AgentInteractionInterfaceVM InteractionInterface
		{
			get
			{
				return this._interactionInterface;
			}
			set
			{
				if (value != this._interactionInterface)
				{
					this._interactionInterface = value;
					base.OnPropertyChangedWithValue<AgentInteractionInterfaceVM>(value, "InteractionInterface");
				}
			}
		}

		[DataSourceProperty]
		public int AgentHealth
		{
			get
			{
				return this._agentHealth;
			}
			set
			{
				if (value != this._agentHealth)
				{
					if (value <= 0)
					{
						this._agentHealth = 0;
						this.OffhandWeapon = new ImageIdentifierVM(ImageIdentifierType.Null);
						this.PrimaryWeapon = new ImageIdentifierVM(ImageIdentifierType.Null);
						this.AmmoCount = -1;
						this.ShieldHealth = 100;
						this.IsPlayerActive = false;
					}
					else
					{
						this._agentHealth = value;
					}
					base.OnPropertyChangedWithValue(value, "AgentHealth");
				}
			}
		}

		[DataSourceProperty]
		public int AgentHealthMax
		{
			get
			{
				return this._agentHealthMax;
			}
			set
			{
				if (value != this._agentHealthMax)
				{
					this._agentHealthMax = value;
					base.OnPropertyChangedWithValue(value, "AgentHealthMax");
				}
			}
		}

		[DataSourceProperty]
		public int HorseHealth
		{
			get
			{
				return this._horseHealth;
			}
			set
			{
				if (value != this._horseHealth)
				{
					this._horseHealth = value;
					base.OnPropertyChangedWithValue(value, "HorseHealth");
				}
			}
		}

		[DataSourceProperty]
		public int HorseHealthMax
		{
			get
			{
				return this._horseHealthMax;
			}
			set
			{
				if (value != this._horseHealthMax)
				{
					this._horseHealthMax = value;
					base.OnPropertyChangedWithValue(value, "HorseHealthMax");
				}
			}
		}

		[DataSourceProperty]
		public int ShieldHealth
		{
			get
			{
				return this._shieldHealth;
			}
			set
			{
				if (value != this._shieldHealth)
				{
					this._shieldHealth = value;
					base.OnPropertyChangedWithValue(value, "ShieldHealth");
				}
			}
		}

		[DataSourceProperty]
		public int ShieldHealthMax
		{
			get
			{
				return this._shieldHealthMax;
			}
			set
			{
				if (value != this._shieldHealthMax)
				{
					this._shieldHealthMax = value;
					base.OnPropertyChangedWithValue(value, "ShieldHealthMax");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPlayerActive
		{
			get
			{
				return this._isPlayerActive;
			}
			set
			{
				if (value != this._isPlayerActive)
				{
					this._isPlayerActive = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerActive");
				}
			}
		}

		public bool IsCombatUIActive
		{
			get
			{
				return this._isCombatUIActive;
			}
			set
			{
				if (value != this._isCombatUIActive)
				{
					this._isCombatUIActive = value;
					base.OnPropertyChangedWithValue(value, "IsCombatUIActive");
					this._combatUIRemainTimer = 0f;
				}
			}
		}

		[DataSourceProperty]
		public bool ShowAgentHealthBar
		{
			get
			{
				return this._showAgentHealthBar;
			}
			set
			{
				if (value != this._showAgentHealthBar)
				{
					this._showAgentHealthBar = value;
					base.OnPropertyChangedWithValue(value, "ShowAgentHealthBar");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowMountHealthBar
		{
			get
			{
				return this._showMountHealthBar;
			}
			set
			{
				if (value != this._showMountHealthBar)
				{
					this._showMountHealthBar = value;
					base.OnPropertyChangedWithValue(value, "ShowMountHealthBar");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowShieldHealthBar
		{
			get
			{
				return this._showShieldHealthBar;
			}
			set
			{
				if (value != this._showShieldHealthBar)
				{
					this._showShieldHealthBar = value;
					base.OnPropertyChangedWithValue(value, "ShowShieldHealthBar");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInteractionAvailable
		{
			get
			{
				return this._isInteractionAvailable;
			}
			set
			{
				if (value != this._isInteractionAvailable)
				{
					this._isInteractionAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsInteractionAvailable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAgentStatusAvailable
		{
			get
			{
				return this._isAgentStatusAvailable;
			}
			set
			{
				if (value != this._isAgentStatusAvailable)
				{
					this._isAgentStatusAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsAgentStatusAvailable");
				}
			}
		}

		[DataSourceProperty]
		public int CouchLanceState
		{
			get
			{
				return this._couchLanceState;
			}
			set
			{
				if (value != this._couchLanceState)
				{
					this._couchLanceState = value;
					base.OnPropertyChangedWithValue(value, "CouchLanceState");
				}
			}
		}

		[DataSourceProperty]
		public int SpearBraceState
		{
			get
			{
				return this._spearBraceState;
			}
			set
			{
				if (value != this._spearBraceState)
				{
					this._spearBraceState = value;
					base.OnPropertyChangedWithValue(value, "SpearBraceState");
				}
			}
		}

		[DataSourceProperty]
		public int TroopCount
		{
			get
			{
				return this._troopCount;
			}
			set
			{
				if (value != this._troopCount)
				{
					this._troopCount = value;
					base.OnPropertyChangedWithValue(value, "TroopCount");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTroopsActive
		{
			get
			{
				return this._isTroopsActive;
			}
			set
			{
				if (value != this._isTroopsActive)
				{
					this._isTroopsActive = value;
					base.OnPropertyChangedWithValue(value, "IsTroopsActive");
				}
			}
		}

		[DataSourceProperty]
		public bool IsGoldActive
		{
			get
			{
				return this._isGoldActive;
			}
			set
			{
				if (value != this._isGoldActive)
				{
					this._isGoldActive = value;
					base.OnPropertyChangedWithValue(value, "IsGoldActive");
				}
			}
		}

		[DataSourceProperty]
		public int GoldAmount
		{
			get
			{
				return this._goldAmount;
			}
			set
			{
				if (value != this._goldAmount)
				{
					this._goldAmount = value;
					base.OnPropertyChangedWithValue(value, "GoldAmount");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowAmmoCount
		{
			get
			{
				return this._showAmmoCount;
			}
			set
			{
				if (value != this._showAmmoCount)
				{
					this._showAmmoCount = value;
					base.OnPropertyChangedWithValue(value, "ShowAmmoCount");
				}
			}
		}

		[DataSourceProperty]
		public int AmmoCount
		{
			get
			{
				return this._ammoCount;
			}
			set
			{
				if (value != this._ammoCount)
				{
					this._ammoCount = value;
					base.OnPropertyChangedWithValue(value, "AmmoCount");
					this.ShowAmmoCount = value >= 0;
				}
			}
		}

		[DataSourceProperty]
		public float TroopsAmmoPercentage
		{
			get
			{
				return this._troopsAmmoPercentage;
			}
			set
			{
				if (value != this._troopsAmmoPercentage)
				{
					this._troopsAmmoPercentage = value;
					base.OnPropertyChangedWithValue(value, "TroopsAmmoPercentage");
				}
			}
		}

		[DataSourceProperty]
		public bool TroopsAmmoAvailable
		{
			get
			{
				return this._troopsAmmoAvailable;
			}
			set
			{
				if (value != this._troopsAmmoAvailable)
				{
					this._troopsAmmoAvailable = value;
					base.OnPropertyChangedWithValue(value, "TroopsAmmoAvailable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAmmoCountAlertEnabled
		{
			get
			{
				return this._isAmmoCountAlertEnabled;
			}
			set
			{
				if (value != this._isAmmoCountAlertEnabled)
				{
					this._isAmmoCountAlertEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsAmmoCountAlertEnabled");
				}
			}
		}

		[DataSourceProperty]
		public float CameraToggleProgress
		{
			get
			{
				return this._cameraToggleProgress;
			}
			set
			{
				if (value != this._cameraToggleProgress)
				{
					this._cameraToggleProgress = value;
					base.OnPropertyChangedWithValue(value, "CameraToggleProgress");
				}
			}
		}

		[DataSourceProperty]
		public string CameraToggleText
		{
			get
			{
				return this._cameraToggleText;
			}
			set
			{
				if (value != this._cameraToggleText)
				{
					this._cameraToggleText = value;
					base.OnPropertyChangedWithValue<string>(value, "CameraToggleText");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM OffhandWeapon
		{
			get
			{
				return this._offhandWeapon;
			}
			set
			{
				if (value != this._offhandWeapon)
				{
					this._offhandWeapon = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "OffhandWeapon");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM PrimaryWeapon
		{
			get
			{
				return this._primaryWeapon;
			}
			set
			{
				if (value != this._primaryWeapon)
				{
					this._primaryWeapon = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "PrimaryWeapon");
				}
			}
		}

		[DataSourceProperty]
		public MissionAgentDamageFeedVM TakenDamageFeed
		{
			get
			{
				return this._takenDamageFeed;
			}
			set
			{
				if (value != this._takenDamageFeed)
				{
					this._takenDamageFeed = value;
					base.OnPropertyChangedWithValue<MissionAgentDamageFeedVM>(value, "TakenDamageFeed");
				}
			}
		}

		private const string _couchLanceUsageString = "couch";

		private const string _spearBraceUsageString = "spear";

		private readonly Mission _mission;

		private readonly Camera _missionCamera;

		private float _combatUIRemainTimer;

		private MissionPeer _missionPeer;

		private MissionMultiplayerGameModeBaseClient _mpGameMode;

		private readonly Func<float> _getCameraToggleProgress;

		private int _agentHealth;

		private int _agentHealthMax;

		private int _horseHealth;

		private int _horseHealthMax;

		private int _shieldHealth;

		private int _shieldHealthMax;

		private bool _isPlayerActive = true;

		private bool _isCombatUIActive;

		private bool _showAgentHealthBar;

		private bool _showMountHealthBar;

		private bool _showShieldHealthBar;

		private bool _troopsAmmoAvailable;

		private bool _isAgentStatusAvailable;

		private bool _isInteractionAvailable;

		private float _troopsAmmoPercentage;

		private int _troopCount;

		private int _goldAmount;

		private bool _isTroopsActive;

		private bool _isGoldActive;

		private AgentInteractionInterfaceVM _interactionInterface;

		private ImageIdentifierVM _offhandWeapon;

		private ImageIdentifierVM _primaryWeapon;

		private MissionAgentTakenDamageVM _takenDamageController;

		private MissionAgentDamageFeedVM _takenDamageFeed;

		private int _ammoCount;

		private int _couchLanceState = -1;

		private int _spearBraceState = -1;

		private bool _showAmmoCount;

		private bool _isAmmoCountAlertEnabled;

		private float _cameraToggleProgress;

		private string _cameraToggleText;

		private enum PassiveUsageStates
		{
			NotPossible,
			ConditionsNotMet,
			Possible,
			Active
		}
	}
}
