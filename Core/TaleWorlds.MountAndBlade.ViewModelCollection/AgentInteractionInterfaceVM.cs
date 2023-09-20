using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace TaleWorlds.MountAndBlade.ViewModelCollection
{
	public class AgentInteractionInterfaceVM : ViewModel
	{
		private bool IsPlayerActive
		{
			get
			{
				Agent main = Agent.Main;
				return main != null && main.Health > 0f;
			}
		}

		public AgentInteractionInterfaceVM(Mission mission)
		{
			this._mission = mission;
			this.IsActive = false;
		}

		internal void Tick()
		{
			if (this.IsActive && this._mission.Mode == MissionMode.StartUp && this._currentFocusedObject is Agent && ((Agent)this._currentFocusedObject).IsEnemyOf(this._mission.MainAgent))
			{
				this.IsActive = false;
			}
		}

		internal void CheckAndClearFocusedAgent(Agent agent)
		{
			if (this._currentFocusedObject != null && this._currentFocusedObject as Agent == agent)
			{
				this.IsActive = false;
				this.ResetFocus();
				this.SecondaryInteractionMessage = "";
			}
		}

		public void OnFocusedHealthChanged(IFocusable focusable, float healthPercentage, bool hideHealthbarWhenFull)
		{
			this.SetHealth(healthPercentage, hideHealthbarWhenFull);
		}

		internal void OnFocusGained(Agent mainAgent, IFocusable focusableObject, bool isInteractable)
		{
			if (this.IsPlayerActive && (this._currentFocusedObject != focusableObject || this._currentObjectInteractable != isInteractable))
			{
				this.ResetFocus();
				this._currentFocusedObject = focusableObject;
				this._currentObjectInteractable = isInteractable;
				Agent agent;
				UsableMissionObject usableMissionObject;
				if ((agent = focusableObject as Agent) != null)
				{
					if (agent.IsHuman)
					{
						this.SetAgent(mainAgent, agent, isInteractable);
						return;
					}
					this.SetMount(mainAgent, agent, isInteractable);
					return;
				}
				else if ((usableMissionObject = focusableObject as UsableMissionObject) != null)
				{
					SpawnedItemEntity spawnedItemEntity;
					if ((spawnedItemEntity = usableMissionObject as SpawnedItemEntity) != null)
					{
						bool flag = Agent.Main.CanQuickPickUp(spawnedItemEntity);
						this.SetItem(spawnedItemEntity, flag, isInteractable);
						return;
					}
					this.SetUsableMissionObject(usableMissionObject, isInteractable);
					return;
				}
				else
				{
					UsableMachine usableMachine;
					if ((usableMachine = focusableObject as UsableMachine) != null)
					{
						this.SetUsableMachine(usableMachine, isInteractable);
						return;
					}
					DestructableComponent destructableComponent;
					if ((destructableComponent = focusableObject as DestructableComponent) != null)
					{
						this.SetDestructibleComponent(destructableComponent, false);
					}
				}
			}
		}

		internal void OnFocusLost(Agent agent, IFocusable focusableObject)
		{
			this.ResetFocus();
			this.IsActive = false;
		}

		internal void OnAgentInteraction(Agent userAgent, Agent agent)
		{
			if (this._mission.Mode == MissionMode.Stealth && agent.IsHuman && agent.IsActive() && !agent.IsEnemyOf(userAgent))
			{
				this.SetAgent(userAgent, agent, true);
			}
		}

		private void SetItem(SpawnedItemEntity item, bool canQuickPickup, bool isInteractable)
		{
			this.IsFocusedOnExit = false;
			EquipmentIndex equipmentIndex;
			ItemObject weaponToReplaceOnQuickAction = Agent.Main.GetWeaponToReplaceOnQuickAction(item, out equipmentIndex);
			bool flag = equipmentIndex != EquipmentIndex.None && !Agent.Main.Equipment[equipmentIndex].IsEmpty && Agent.Main.Equipment[equipmentIndex].IsAnyConsumable() && Agent.Main.Equipment[equipmentIndex].Amount < Agent.Main.Equipment[equipmentIndex].ModifiedMaxAmount;
			TextObject actionMessage = item.GetActionMessage(weaponToReplaceOnQuickAction, flag);
			TextObject descriptionMessage = item.GetDescriptionMessage(flag);
			if (!TextObject.IsNullOrEmpty(actionMessage) && !TextObject.IsNullOrEmpty(descriptionMessage))
			{
				this.FocusType = 0;
				if (isInteractable)
				{
					MBTextManager.SetTextVariable("USE_KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)), false);
					if (canQuickPickup)
					{
						Agent main = Agent.Main;
						if (main != null && main.CanInteractableWeaponBePickedUp(item))
						{
							this.PrimaryInteractionMessage = actionMessage.ToString();
							MBTextManager.SetTextVariable("KEY", GameTexts.FindText("str_ui_agent_interaction_use", null), false);
							MBTextManager.SetTextVariable("ACTION", GameTexts.FindText("str_select_item_to_replace", null), false);
							this.SecondaryInteractionMessage = GameTexts.FindText("str_hold_key_action", null).ToString() + this.GetWeaponSpecificText(item);
						}
						else
						{
							this.PrimaryInteractionMessage = actionMessage.ToString();
							MBTextManager.SetTextVariable("STR1", descriptionMessage, false);
							MBTextManager.SetTextVariable("STR2", this.GetWeaponSpecificText(item), false);
							this.SecondaryInteractionMessage = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
						}
					}
					else
					{
						MBTextManager.SetTextVariable("KEY", GameTexts.FindText("str_ui_agent_interaction_use", null), false);
						MBTextManager.SetTextVariable("ACTION", GameTexts.FindText("str_select_item_to_replace", null), false);
						this.PrimaryInteractionMessage = GameTexts.FindText("str_hold_key_action", null).ToString();
						MBTextManager.SetTextVariable("STR1", descriptionMessage, false);
						MBTextManager.SetTextVariable("STR2", this.GetWeaponSpecificText(item), false);
						this.SecondaryInteractionMessage = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
					}
				}
				else
				{
					this.PrimaryInteractionMessage = item.GetInfoTextForBeingNotInteractable(Agent.Main).ToString();
					MBTextManager.SetTextVariable("STR1", descriptionMessage, false);
					MBTextManager.SetTextVariable("STR2", this.GetWeaponSpecificText(item), false);
					this.SecondaryInteractionMessage = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
				}
				this.IsActive = true;
			}
		}

		private void SetUsableMissionObject(UsableMissionObject usableObject, bool isInteractable)
		{
			this.FocusType = (int)usableObject.FocusableObjectType;
			this.IsFocusedOnExit = false;
			if (!string.IsNullOrEmpty(usableObject.ActionMessage.ToString()) && !string.IsNullOrEmpty(usableObject.DescriptionMessage.ToString()))
			{
				this.PrimaryInteractionMessage = (isInteractable ? usableObject.ActionMessage.ToString() : " ");
				this.SecondaryInteractionMessage = usableObject.DescriptionMessage.ToString();
				this.IsFocusedOnExit = usableObject.FocusableObjectType == FocusableObjectType.Door || usableObject.FocusableObjectType == FocusableObjectType.Gate;
			}
			else
			{
				UsableMachine usableMachineFromPoint = this.GetUsableMachineFromPoint(usableObject);
				if (usableMachineFromPoint != null)
				{
					this.PrimaryInteractionMessage = usableMachineFromPoint.GetDescriptionText(usableObject.GameEntity) ?? "";
					string text;
					if (!isInteractable)
					{
						text = "";
					}
					else
					{
						TextObject actionTextForStandingPoint = usableMachineFromPoint.GetActionTextForStandingPoint(usableObject);
						text = ((actionTextForStandingPoint != null) ? actionTextForStandingPoint.ToString() : null) ?? "";
					}
					this.SecondaryInteractionMessage = text;
				}
			}
			this.IsActive = true;
		}

		private void SetUsableMachine(UsableMachine machine, bool isInteractable)
		{
			this.PrimaryInteractionMessage = machine.GetDescriptionText(machine.GameEntity) ?? "";
			this.SecondaryInteractionMessage = " ";
			if (machine is CastleGate)
			{
				this.FocusType = 1;
			}
			if (machine.DestructionComponent != null)
			{
				this.TargetHealth = (int)(100f * machine.DestructionComponent.HitPoint / machine.DestructionComponent.MaxHitPoint);
				this.ShowHealthBar = true;
			}
			this.IsActive = true;
		}

		private void SetDestructibleComponent(DestructableComponent machine, bool isInteractable)
		{
			string descriptionText = machine.GetDescriptionText(machine.GameEntity);
			bool flag = descriptionText != "" && descriptionText != null;
			this.PrimaryInteractionMessage = (flag ? descriptionText : "null");
			this.SecondaryInteractionMessage = " ";
			this.TargetHealth = (int)(100f * machine.HitPoint / machine.MaxHitPoint);
			this.ShowHealthBar = machine.HitPoint < machine.MaxHitPoint;
			this.IsActive = flag;
		}

		private void SetAgent(Agent mainAgent, Agent focusedAgent, bool isInteractable)
		{
			this.IsFocusedOnExit = false;
			bool flag = true;
			this.FocusType = 3;
			if (focusedAgent.MissionPeer != null)
			{
				this.PrimaryInteractionMessage = focusedAgent.MissionPeer.DisplayedName;
			}
			else
			{
				this.PrimaryInteractionMessage = focusedAgent.Name.ToString();
			}
			if (isInteractable && (this._mission.Mode == MissionMode.StartUp || this._mission.Mode == MissionMode.Duel || this._mission.Mode == MissionMode.Battle || this._mission.Mode == MissionMode.Stealth) && focusedAgent.IsHuman)
			{
				MBTextManager.SetTextVariable("USE_KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)), false);
				if (focusedAgent.IsActive())
				{
					if (this._mission.Mode == MissionMode.Duel)
					{
						DuelMissionRepresentative duelMissionRepresentative = Agent.Main.MissionRepresentative as DuelMissionRepresentative;
						TextObject textObject = ((duelMissionRepresentative != null && duelMissionRepresentative.CheckHasRequestFromAndRemoveRequestIfNeeded(focusedAgent.MissionPeer)) ? GameTexts.FindText("str_ui_respond", null) : GameTexts.FindText("str_ui_duel", null));
						MBTextManager.SetTextVariable("KEY", GameTexts.FindText("str_ui_agent_interaction_use", null), false);
						MBTextManager.SetTextVariable("ACTION", textObject, false);
						this.SecondaryInteractionMessage = GameTexts.FindText("str_key_action", null).ToString();
					}
					else if (this._mission.Mode == MissionMode.Stealth && !focusedAgent.IsEnemyOf(mainAgent))
					{
						MBTextManager.SetTextVariable("KEY", GameTexts.FindText("str_ui_agent_interaction_use", null), false);
						MBTextManager.SetTextVariable("ACTION", GameTexts.FindText("str_ui_prison_break", null), false);
						this.SecondaryInteractionMessage = GameTexts.FindText("str_key_action", null).ToString();
					}
					else if (focusedAgent.IsEnemyOf(mainAgent))
					{
						flag = false;
					}
					else if (!Mission.Current.IsAgentInteractionAllowed())
					{
						flag = false;
					}
					else if (this._mission.Mode != MissionMode.Battle)
					{
						MBTextManager.SetTextVariable("KEY", GameTexts.FindText("str_ui_agent_interaction_use", null), false);
						MBTextManager.SetTextVariable("ACTION", GameTexts.FindText("str_ui_talk", null), false);
						this.SecondaryInteractionMessage = GameTexts.FindText("str_key_action", null).ToString();
					}
					else
					{
						this.FocusType = -1;
					}
				}
				else if (this._mission.Mode != MissionMode.Battle)
				{
					MBTextManager.SetTextVariable("KEY", GameTexts.FindText("str_ui_agent_interaction_use", null), false);
					MBTextManager.SetTextVariable("ACTION", GameTexts.FindText("str_ui_search", null), false);
					this.SecondaryInteractionMessage = GameTexts.FindText("str_key_action", null).ToString();
				}
			}
			this.IsActive = flag;
		}

		private void SetMount(Agent agent, Agent focusedAgent, bool isInteractable)
		{
			this.IsFocusedOnExit = false;
			if (focusedAgent.IsActive() && focusedAgent.IsMount)
			{
				string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
				this.SecondaryInteractionMessage = focusedAgent.Name.ToString();
				this.FocusType = 2;
				if (focusedAgent.RiderAgent == null)
				{
					if ((float)MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(agent, DefaultSkills.Riding) < focusedAgent.GetAgentDrivenPropertyValue(DrivenProperty.MountDifficulty))
					{
						this.PrimaryInteractionMessage = GameTexts.FindText("str_ui_riding_skill_not_adequate_to_mount", null).ToString();
					}
					else if ((agent.GetAgentFlags() & AgentFlag.CanRide) > AgentFlag.None)
					{
						MBTextManager.SetTextVariable("KEY", keyHyperlinkText, false);
						MBTextManager.SetTextVariable("ACTION", GameTexts.FindText("str_ui_mount", null), false);
						this.PrimaryInteractionMessage = GameTexts.FindText("str_key_action", null).ToString();
					}
					this.ShowHealthBar = false;
				}
				else if (focusedAgent.RiderAgent == agent)
				{
					MBTextManager.SetTextVariable("KEY", keyHyperlinkText, false);
					MBTextManager.SetTextVariable("ACTION", GameTexts.FindText("str_ui_dismount", null), false);
					this.PrimaryInteractionMessage = GameTexts.FindText("str_key_action", null).ToString();
				}
				this.IsActive = true;
			}
		}

		private void SetHealth(float healthPercentage, bool hideHealthBarWhenFull)
		{
			this.TargetHealth = (int)(100f * healthPercentage);
			if (hideHealthBarWhenFull)
			{
				this.ShowHealthBar = this.TargetHealth < 100;
				return;
			}
			this.ShowHealthBar = true;
		}

		public void ResetFocus()
		{
			this._currentFocusedObject = null;
			this.PrimaryInteractionMessage = "";
			this.FocusType = -1;
		}

		private UsableMachine GetUsableMachineFromPoint(UsableMissionObject standingPoint)
		{
			GameEntity gameEntity = standingPoint.GameEntity;
			while (gameEntity != null && !gameEntity.HasScriptOfType<UsableMachine>())
			{
				gameEntity = gameEntity.Parent;
			}
			UsableMachine usableMachine = null;
			if (gameEntity != null)
			{
				usableMachine = gameEntity.GetFirstScriptOfType<UsableMachine>();
			}
			return usableMachine;
		}

		[DataSourceProperty]
		public bool IsFocusedOnExit
		{
			get
			{
				return this._isFocusedOnExit;
			}
			set
			{
				if (value != this._isFocusedOnExit)
				{
					this._isFocusedOnExit = value;
					base.OnPropertyChangedWithValue(value, "IsFocusedOnExit");
				}
			}
		}

		[DataSourceProperty]
		public int TargetHealth
		{
			get
			{
				return this._targetHealth;
			}
			set
			{
				if (value != this._targetHealth)
				{
					this._targetHealth = value;
					base.OnPropertyChangedWithValue(value, "TargetHealth");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowHealthBar
		{
			get
			{
				return this._showHealthBar;
			}
			set
			{
				if (value != this._showHealthBar)
				{
					this._showHealthBar = value;
					base.OnPropertyChangedWithValue(value, "ShowHealthBar");
				}
			}
		}

		[DataSourceProperty]
		public int FocusType
		{
			get
			{
				return this._focusType;
			}
			set
			{
				if (this._focusType != value)
				{
					this._focusType = value;
					base.OnPropertyChangedWithValue(value, "FocusType");
				}
			}
		}

		[DataSourceProperty]
		public string PrimaryInteractionMessage
		{
			get
			{
				return this._primaryInteractionMessage;
			}
			set
			{
				if (this._primaryInteractionMessage != value)
				{
					this._primaryInteractionMessage = value;
					base.OnPropertyChangedWithValue<string>(value, "PrimaryInteractionMessage");
					if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
					{
						this.IsFocusedOnExit = false;
					}
				}
			}
		}

		[DataSourceProperty]
		public string SecondaryInteractionMessage
		{
			get
			{
				return this._secondaryInteractionMessage;
			}
			set
			{
				if (this._secondaryInteractionMessage != value)
				{
					this._secondaryInteractionMessage = value;
					base.OnPropertyChangedWithValue<string>(value, "SecondaryInteractionMessage");
				}
			}
		}

		[DataSourceProperty]
		public string BackgroundColor
		{
			get
			{
				return this._backgroundColor;
			}
			set
			{
				if (this._backgroundColor != value)
				{
					this._backgroundColor = value;
					base.OnPropertyChangedWithValue<string>(value, "BackgroundColor");
				}
			}
		}

		[DataSourceProperty]
		public string TextColor
		{
			get
			{
				return this._textColor;
			}
			set
			{
				if (this._textColor != value)
				{
					this._textColor = value;
					base.OnPropertyChangedWithValue<string>(value, "TextColor");
				}
			}
		}

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
					if (!value)
					{
						this.ShowHealthBar = false;
						this.PrimaryInteractionMessage = "";
						this.SecondaryInteractionMessage = "";
					}
				}
			}
		}

		private string GetWeaponSpecificText(SpawnedItemEntity spawnedItem)
		{
			MissionWeapon weaponCopy = spawnedItem.WeaponCopy;
			WeaponComponentData currentUsageItem = weaponCopy.CurrentUsageItem;
			if (currentUsageItem != null && currentUsageItem.IsShield)
			{
				MBTextManager.SetTextVariable("LEFT", (int)weaponCopy.HitPoints);
				MBTextManager.SetTextVariable("RIGHT", (int)weaponCopy.ModifiedMaxHitPoints);
				return GameTexts.FindText("str_LEFT_over_RIGHT_in_paranthesis", null).ToString();
			}
			WeaponComponentData currentUsageItem2 = weaponCopy.CurrentUsageItem;
			if (currentUsageItem2 != null && currentUsageItem2.IsAmmo && weaponCopy.ModifiedMaxAmount > 1 && !spawnedItem.IsStuckMissile())
			{
				MBTextManager.SetTextVariable("LEFT", (int)weaponCopy.Amount);
				MBTextManager.SetTextVariable("RIGHT", (int)weaponCopy.ModifiedMaxAmount);
				return GameTexts.FindText("str_LEFT_over_RIGHT_in_paranthesis", null).ToString();
			}
			return "";
		}

		private readonly Mission _mission;

		private bool _currentObjectInteractable;

		private IFocusable _currentFocusedObject;

		private bool _isActive;

		private string _secondaryInteractionMessage;

		private string _primaryInteractionMessage;

		private int _focusType;

		private int _targetHealth;

		private bool _showHealthBar;

		private bool _isFocusedOnExit;

		private string _backgroundColor;

		private string _textColor;
	}
}
