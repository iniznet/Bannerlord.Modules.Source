﻿using System;
using System.ComponentModel;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	[DefaultView]
	public class MissionMainAgentController : MissionView
	{
		public event MissionMainAgentController.OnLockedAgentChangedDelegate OnLockedAgentChanged;

		public event MissionMainAgentController.OnPotentialLockedAgentChangedDelegate OnPotentialLockedAgentChanged;

		public bool IsDisabled { get; set; }

		public Vec3 CustomLookDir { get; set; }

		public bool IsPlayerAiming
		{
			get
			{
				if (this._isPlayerAiming)
				{
					return true;
				}
				if (base.Mission.MainAgent == null)
				{
					return false;
				}
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				if (base.Input != null)
				{
					flag2 = base.Input.IsGameKeyDown(9);
				}
				if (base.Mission.MainAgent != null)
				{
					if (base.Mission.MainAgent.WieldedWeapon.CurrentUsageItem != null)
					{
						flag = base.Mission.MainAgent.WieldedWeapon.CurrentUsageItem.IsRangedWeapon || base.Mission.MainAgent.WieldedWeapon.CurrentUsageItem.IsAmmo;
					}
					flag3 = Extensions.HasAnyFlag<Agent.MovementControlFlag>(base.Mission.MainAgent.MovementFlags, 960);
				}
				return flag && flag2 && flag3;
			}
		}

		public Agent LockedAgent
		{
			get
			{
				return this._lockedAgent;
			}
			private set
			{
				if (this._lockedAgent != value)
				{
					this._lockedAgent = value;
					MissionMainAgentController.OnLockedAgentChangedDelegate onLockedAgentChanged = this.OnLockedAgentChanged;
					if (onLockedAgentChanged == null)
					{
						return;
					}
					onLockedAgentChanged(value);
				}
			}
		}

		public Agent PotentialLockTargetAgent
		{
			get
			{
				return this._potentialLockTargetAgent;
			}
			private set
			{
				if (this._potentialLockTargetAgent != value)
				{
					this._potentialLockTargetAgent = value;
					MissionMainAgentController.OnPotentialLockedAgentChangedDelegate onPotentialLockedAgentChanged = this.OnPotentialLockedAgentChanged;
					if (onPotentialLockedAgentChanged == null)
					{
						return;
					}
					onPotentialLockedAgentChanged(value);
				}
			}
		}

		public MissionMainAgentController()
		{
			this.InteractionComponent = new MissionMainAgentInteractionComponent(this);
			this.CustomLookDir = Vec3.Zero;
			this.IsChatOpen = false;
		}

		public override void EarlyStart()
		{
			base.EarlyStart();
			Game.Current.EventManager.RegisterEvent<MissionPlayerToggledOrderViewEvent>(new Action<MissionPlayerToggledOrderViewEvent>(this.OnPlayerToggleOrder));
			base.Mission.OnMainAgentChanged += this.Mission_OnMainAgentChanged;
			MissionMultiplayerGameModeBaseClient missionBehavior = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			if (((missionBehavior != null) ? missionBehavior.RoundComponent : null) != null)
			{
				missionBehavior.RoundComponent.OnRoundStarted += this.Disable;
				missionBehavior.RoundComponent.OnPreparationEnded += this.Enable;
			}
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			this.UpdateLockTargetOption();
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.Mission.OnMainAgentChanged -= this.Mission_OnMainAgentChanged;
			Game.Current.EventManager.UnregisterEvent<MissionPlayerToggledOrderViewEvent>(new Action<MissionPlayerToggledOrderViewEvent>(this.OnPlayerToggleOrder));
			MissionMultiplayerGameModeBaseClient missionBehavior = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			if (((missionBehavior != null) ? missionBehavior.RoundComponent : null) != null)
			{
				missionBehavior.RoundComponent.OnRoundStarted -= this.Disable;
				missionBehavior.RoundComponent.OnPreparationEnded -= this.Enable;
			}
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
		}

		public override bool IsReady()
		{
			bool flag = true;
			if (base.Mission.MainAgent != null)
			{
				flag = base.Mission.MainAgent.AgentVisuals.CheckResources(true);
			}
			return flag;
		}

		private void Mission_OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (base.Mission.MainAgent != null)
			{
				this._isPlayerAgentAdded = true;
				this._strafeModeActive = false;
				this._autoDismountModeActive = false;
			}
		}

		public override void OnPreMissionTick(float dt)
		{
			base.OnPreMissionTick(dt);
			if (base.MissionScreen == null)
			{
				return;
			}
			if (base.Mission.MainAgent == null && GameNetwork.MyPeer != null)
			{
				MissionPeer component = PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer);
				if (component != null)
				{
					if (component.HasSpawnedAgentVisuals)
					{
						this.AgentVisualsMovementCheck();
					}
					else if (component.FollowedAgent != null)
					{
						this.RequestToSpawnAsBotCheck();
					}
				}
			}
			Agent mainAgent = base.Mission.MainAgent;
			if (mainAgent != null && mainAgent.State == 1 && !base.MissionScreen.IsCheatGhostMode && !base.Mission.MainAgent.IsAIControlled && !this.IsDisabled && this._activated)
			{
				this.InteractionComponent.FocusTick();
				this.InteractionComponent.FocusedItemHealthTick();
				this.ControlTick();
				this.InteractionComponent.FocusStateCheckTick();
				this.LookTick(dt);
				return;
			}
			this.LockedAgent = null;
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (this.InteractionComponent.CurrentFocusedObject == affectedAgent || affectedAgent == base.Mission.MainAgent)
			{
				this.InteractionComponent.ClearFocus();
			}
		}

		public override void OnAgentDeleted(Agent affectedAgent)
		{
			if (this.InteractionComponent.CurrentFocusedObject == affectedAgent)
			{
				this.InteractionComponent.ClearFocus();
			}
		}

		public override void OnClearScene()
		{
			this.InteractionComponent.OnClearScene();
		}

		private void LookTick(float dt)
		{
			if (!this.IsDisabled)
			{
				Agent mainAgent = base.Mission.MainAgent;
				if (mainAgent != null)
				{
					if (this._isPlayerAgentAdded)
					{
						this._isPlayerAgentAdded = false;
						mainAgent.LookDirectionAsAngle = mainAgent.MovementDirectionAsAngle;
					}
					if (base.Mission.ClearSceneTimerElapsedTime >= 0f)
					{
						Vec3 vec;
						if (this.LockedAgent != null)
						{
							float num = 0f;
							float agentScale = this.LockedAgent.AgentScale;
							float agentScale2 = mainAgent.AgentScale;
							if (!Extensions.HasAnyFlag<AgentFlag>(this.LockedAgent.GetAgentFlags(), 2048))
							{
								num += this.LockedAgent.Monster.BodyCapsulePoint1.z * agentScale;
							}
							else if (this.LockedAgent.HasMount)
							{
								num += (this.LockedAgent.MountAgent.Monster.RiderCameraHeightAdder + this.LockedAgent.MountAgent.Monster.BodyCapsulePoint1.z + this.LockedAgent.MountAgent.Monster.BodyCapsuleRadius) * this.LockedAgent.MountAgent.AgentScale + this.LockedAgent.Monster.CrouchEyeHeight * agentScale;
							}
							else if (this.LockedAgent.CrouchMode || this.LockedAgent.IsSitting())
							{
								num += (this.LockedAgent.Monster.CrouchEyeHeight + 0.2f) * agentScale;
							}
							else
							{
								num += (this.LockedAgent.Monster.StandingEyeHeight + 0.2f) * agentScale;
							}
							if (!Extensions.HasAnyFlag<AgentFlag>(mainAgent.GetAgentFlags(), 2048))
							{
								num -= this.LockedAgent.Monster.BodyCapsulePoint1.z * agentScale2;
							}
							else if (mainAgent.HasMount)
							{
								num -= (mainAgent.MountAgent.Monster.RiderCameraHeightAdder + mainAgent.MountAgent.Monster.BodyCapsulePoint1.z + mainAgent.MountAgent.Monster.BodyCapsuleRadius) * mainAgent.MountAgent.AgentScale + mainAgent.Monster.CrouchEyeHeight * agentScale2;
							}
							else if (mainAgent.CrouchMode || mainAgent.IsSitting())
							{
								num -= (mainAgent.Monster.CrouchEyeHeight + 0.2f) * agentScale2;
							}
							else
							{
								num -= (mainAgent.Monster.StandingEyeHeight + 0.2f) * agentScale2;
							}
							if (Extensions.HasAnyFlag<AgentFlag>(this.LockedAgent.GetAgentFlags(), 2048))
							{
								num -= 0.3f * agentScale;
							}
							num = MBMath.Lerp(this._lastLockedAgentHeightDifference, num, MathF.Min(8f * dt, 1f), 1E-05f);
							this._lastLockedAgentHeightDifference = num;
							vec = (this.LockedAgent.VisualPosition + ((this.LockedAgent.MountAgent != null) ? (this.LockedAgent.MountAgent.GetMovementDirection().ToVec3(0f) * this.LockedAgent.MountAgent.Monster.RiderBodyCapsuleForwardAdder) : Vec3.Zero) + new Vec3(0f, 0f, num, -1f) - (mainAgent.VisualPosition + ((mainAgent.MountAgent != null) ? (mainAgent.MountAgent.GetMovementDirection().ToVec3(0f) * mainAgent.MountAgent.Monster.RiderBodyCapsuleForwardAdder) : Vec3.Zero))).NormalizedCopy();
						}
						else if (this.CustomLookDir.IsNonZero)
						{
							vec = this.CustomLookDir;
						}
						else
						{
							Mat3 identity = Mat3.Identity;
							identity.RotateAboutUp(base.MissionScreen.CameraBearing);
							identity.RotateAboutSide(base.MissionScreen.CameraElevation);
							vec = identity.f;
						}
						if (!base.MissionScreen.IsViewingCharacter() && !mainAgent.IsLookDirectionLocked && mainAgent.MovementLockedState != 2)
						{
							mainAgent.LookDirection = vec;
						}
						mainAgent.HeadCameraMode = base.Mission.CameraIsFirstPerson;
					}
				}
			}
		}

		private void AgentVisualsMovementCheck()
		{
			if (base.Input.IsGameKeyReleased(13))
			{
				this.BreakAgentVisualsInvulnerability();
			}
		}

		public void BreakAgentVisualsInvulnerability()
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new AgentVisualsBreakInvulnerability());
				GameNetwork.EndModuleEventAsClient();
				return;
			}
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetEarlyAgentVisualsDespawning(PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer), true);
		}

		private void RequestToSpawnAsBotCheck()
		{
			if (base.Input.IsGameKeyPressed(13))
			{
				if (GameNetwork.IsClient)
				{
					GameNetwork.BeginModuleEventAsClient();
					GameNetwork.WriteMessage(new RequestToSpawnAsBot());
					GameNetwork.EndModuleEventAsClient();
					return;
				}
				if (PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer).HasSpawnTimerExpired)
				{
					PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer).WantsToSpawnAsBot = true;
				}
			}
		}

		private Agent FindTargetedLockableAgent(Agent player)
		{
			Vec3 direction = base.MissionScreen.CombatCamera.Direction;
			Vec3 vec = direction;
			Vec3 position = base.MissionScreen.CombatCamera.Position;
			Vec3 visualPosition = player.VisualPosition;
			float num = new Vec3(position.x, position.y, 0f, -1f).Distance(new Vec3(visualPosition.x, visualPosition.y, 0f, -1f));
			Vec3 vec2 = position * (1f - num) + (position + direction) * num;
			float num2 = 0f;
			Agent agent = null;
			foreach (Agent agent2 in base.Mission.Agents)
			{
				if ((agent2.IsMount && agent2.RiderAgent != null && agent2.RiderAgent.IsEnemyOf(player)) || (!agent2.IsMount && agent2.IsEnemyOf(player)))
				{
					Vec3 vec3 = agent2.GetChestGlobalPosition() - vec2;
					float num3 = vec3.Normalize();
					if (num3 < 20f)
					{
						float num4 = Vec2.DotProduct(vec.AsVec2.Normalized(), vec3.AsVec2.Normalized());
						float num5 = Vec2.DotProduct(new Vec2(vec.AsVec2.Length, vec.z), new Vec2(vec3.AsVec2.Length, vec3.z));
						if (num4 > 0.95f && num5 > 0.95f)
						{
							float num6 = num4 * num4 * num4 / MathF.Pow(num3, 0.15f);
							if (num6 > num2)
							{
								num2 = num6;
								agent = agent2;
							}
						}
					}
				}
			}
			if (agent != null && agent.IsMount && agent.RiderAgent != null)
			{
				return agent.RiderAgent;
			}
			return agent;
		}

		private void ControlTick()
		{
			if (base.MissionScreen != null && base.MissionScreen.IsPhotoModeEnabled)
			{
				return;
			}
			if (this.IsChatOpen)
			{
				return;
			}
			Agent mainAgent = base.Mission.MainAgent;
			bool flag = false;
			if (this.LockedAgent != null && (!LinQuick.ContainsQ<Agent>(base.Mission.Agents, this.LockedAgent) || !this.LockedAgent.IsActive() || this.LockedAgent.Position.DistanceSquared(mainAgent.Position) > 625f || base.Input.IsGameKeyReleased(26) || base.Input.IsGameKeyDown(25) || (base.Mission.Mode != 2 && base.Mission.Mode != 4) || (!mainAgent.WieldedWeapon.IsEmpty && mainAgent.WieldedWeapon.CurrentUsageItem.IsRangedWeapon) || base.MissionScreen == null || base.MissionScreen.GetSpectatingData(base.MissionScreen.CombatCamera.Frame.origin).CameraType != 1))
			{
				this.LockedAgent = null;
				flag = true;
			}
			if (base.Mission.Mode == 1)
			{
				mainAgent.MovementFlags = 0;
				mainAgent.MovementInputVector = Vec2.Zero;
				return;
			}
			if (base.Mission.ClearSceneTimerElapsedTime >= 0f && mainAgent.State == 1)
			{
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				Vec2 vec;
				vec..ctor(base.Input.GetGameKeyAxis("MovementAxisX"), base.Input.GetGameKeyAxis("MovementAxisY"));
				if (this._autoDismountModeActive)
				{
					if (!base.Input.IsGameKeyDown(0) && mainAgent.MountAgent != null)
					{
						if (mainAgent.GetCurrentVelocity().y > 0f)
						{
							vec.y = -1f;
						}
					}
					else
					{
						this._autoDismountModeActive = false;
					}
				}
				if (MathF.Abs(vec.x) < 0.2f)
				{
					vec.x = 0f;
				}
				if (MathF.Abs(vec.y) < 0.2f)
				{
					vec.y = 0f;
				}
				if (vec.IsNonZero())
				{
					float rotationInRadians = vec.RotationInRadians;
					if (rotationInRadians > -0.7853982f && rotationInRadians < 0.7853982f)
					{
						flag3 = true;
					}
					else if (rotationInRadians < -2.3561945f || rotationInRadians > 2.3561945f)
					{
						flag5 = true;
					}
					else if (rotationInRadians < 0f)
					{
						flag2 = true;
					}
					else
					{
						flag4 = true;
					}
				}
				mainAgent.EventControlFlags = 0;
				mainAgent.MovementFlags = 0;
				mainAgent.MovementInputVector = Vec2.Zero;
				if (!base.MissionScreen.IsRadialMenuActive && !base.Mission.IsOrderMenuOpen)
				{
					if (base.Input.IsGameKeyPressed(14))
					{
						if (mainAgent.MountAgent == null || mainAgent.MovementVelocity.LengthSquared > 0.09f)
						{
							mainAgent.EventControlFlags |= 8;
						}
						else
						{
							mainAgent.EventControlFlags |= 4;
						}
					}
					if (base.Input.IsGameKeyPressed(13))
					{
						mainAgent.MovementFlags |= 65536;
					}
				}
				if (mainAgent.MountAgent != null && mainAgent.GetCurrentVelocity().y < 0.5f && (base.Input.IsGameKeyDown(3) || base.Input.IsGameKeyDown(2)))
				{
					if (base.Input.IsGameKeyPressed(16))
					{
						this._strafeModeActive = true;
					}
				}
				else
				{
					this._strafeModeActive = false;
				}
				Agent.MovementControlFlag movementControlFlag = this._lastMovementKeyPressed;
				if (base.Input.IsGameKeyPressed(0))
				{
					movementControlFlag = 1;
				}
				else if (base.Input.IsGameKeyPressed(1))
				{
					movementControlFlag = 2;
				}
				else if (base.Input.IsGameKeyPressed(2))
				{
					movementControlFlag = 8;
				}
				else if (base.Input.IsGameKeyPressed(3))
				{
					movementControlFlag = 4;
				}
				if (movementControlFlag != this._lastMovementKeyPressed)
				{
					this._lastMovementKeyPressed = movementControlFlag;
					Game game = Game.Current;
					if (game != null)
					{
						game.EventManager.TriggerEvent<MissionPlayerMovementFlagsChangeEvent>(new MissionPlayerMovementFlagsChangeEvent(this._lastMovementKeyPressed));
					}
				}
				if (!base.Input.GetIsMouseActive())
				{
					bool flag6 = true;
					if (flag3)
					{
						movementControlFlag = 1;
					}
					else if (flag5)
					{
						movementControlFlag = 2;
					}
					else if (flag4)
					{
						movementControlFlag = 8;
					}
					else if (flag2)
					{
						movementControlFlag = 4;
					}
					else
					{
						flag6 = false;
					}
					if (flag6)
					{
						base.Mission.SetLastMovementKeyPressed(movementControlFlag);
					}
				}
				else
				{
					base.Mission.SetLastMovementKeyPressed(this._lastMovementKeyPressed);
				}
				if (base.Input.IsGameKeyPressed(0))
				{
					if (this._lastForwardKeyPressTime + 0.3f > Time.ApplicationTime)
					{
						mainAgent.EventControlFlags &= -458753;
						mainAgent.EventControlFlags |= 65536;
					}
					this._lastForwardKeyPressTime = Time.ApplicationTime;
				}
				if (base.Input.IsGameKeyPressed(1))
				{
					if (this._lastBackwardKeyPressTime + 0.3f > Time.ApplicationTime)
					{
						mainAgent.EventControlFlags &= -458753;
						mainAgent.EventControlFlags |= 131072;
					}
					this._lastBackwardKeyPressTime = Time.ApplicationTime;
				}
				if (base.Input.IsGameKeyPressed(2))
				{
					if (this._lastLeftKeyPressTime + 0.3f > Time.ApplicationTime)
					{
						mainAgent.EventControlFlags &= -458753;
						mainAgent.EventControlFlags |= 196608;
					}
					this._lastLeftKeyPressTime = Time.ApplicationTime;
				}
				if (base.Input.IsGameKeyPressed(3))
				{
					if (this._lastRightKeyPressTime + 0.3f > Time.ApplicationTime)
					{
						mainAgent.EventControlFlags &= -458753;
						mainAgent.EventControlFlags |= 262144;
					}
					this._lastRightKeyPressTime = Time.ApplicationTime;
				}
				if (this._isTargetLockEnabled)
				{
					if (base.Input.IsGameKeyDown(26) && this.LockedAgent == null && !base.Input.IsGameKeyDown(25) && (base.Mission.Mode == 2 || base.Mission.Mode == 4) && (mainAgent.WieldedWeapon.IsEmpty || !mainAgent.WieldedWeapon.CurrentUsageItem.IsRangedWeapon) && !GameNetwork.IsMultiplayer)
					{
						float applicationTime = Time.ApplicationTime;
						if (this._lastLockKeyPressTime <= 0f)
						{
							this._lastLockKeyPressTime = applicationTime;
						}
						if (applicationTime > this._lastLockKeyPressTime + 0.3f)
						{
							this.PotentialLockTargetAgent = this.FindTargetedLockableAgent(mainAgent);
						}
					}
					else
					{
						this.PotentialLockTargetAgent = null;
					}
					if (this.LockedAgent == null && !flag && base.Input.IsGameKeyReleased(26) && !GameNetwork.IsMultiplayer)
					{
						this._lastLockKeyPressTime = 0f;
						if (!base.Input.IsGameKeyDown(25) && (base.Mission.Mode == 2 || base.Mission.Mode == 4) && (mainAgent.WieldedWeapon.IsEmpty || !mainAgent.WieldedWeapon.CurrentUsageItem.IsRangedWeapon) && base.MissionScreen != null && base.MissionScreen.GetSpectatingData(base.MissionScreen.CombatCamera.Frame.origin).CameraType == 1)
						{
							this.LockedAgent = this.FindTargetedLockableAgent(mainAgent);
						}
					}
				}
				if (mainAgent.MountAgent != null && !this._strafeModeActive)
				{
					if (flag2 || vec.x > 0f)
					{
						mainAgent.MovementFlags |= 16;
					}
					else if (flag4 || vec.x < 0f)
					{
						mainAgent.MovementFlags |= 32;
					}
				}
				mainAgent.MovementInputVector = vec;
				if (!base.MissionScreen.MouseVisible && !base.MissionScreen.IsRadialMenuActive && !this._isPlayerOrderOpen && mainAgent.CombatActionsEnabled)
				{
					WeaponComponentData currentUsageItem = mainAgent.WieldedWeapon.CurrentUsageItem;
					bool flag7 = currentUsageItem != null && Extensions.HasAllFlags<WeaponFlags>(currentUsageItem.WeaponFlags, 3072L);
					WeaponComponentData currentUsageItem2 = mainAgent.WieldedWeapon.CurrentUsageItem;
					if (currentUsageItem2 != null && currentUsageItem2.IsRangedWeapon)
					{
						bool isConsumable = mainAgent.WieldedWeapon.CurrentUsageItem.IsConsumable;
					}
					WeaponComponentData currentUsageItem3 = mainAgent.WieldedWeapon.CurrentUsageItem;
					bool flag8 = currentUsageItem3 != null && currentUsageItem3.IsRangedWeapon && !mainAgent.WieldedWeapon.CurrentUsageItem.IsConsumable && !Extensions.HasAllFlags<WeaponFlags>(mainAgent.WieldedWeapon.CurrentUsageItem.WeaponFlags, 3072L);
					bool flag9 = NativeOptions.GetConfig(19) != 0f && (flag7 || flag8);
					if (flag9)
					{
						this.HandleRangedWeaponAttackAlternativeAiming(mainAgent);
					}
					else if (base.Input.IsGameKeyDown(9))
					{
						mainAgent.MovementFlags |= mainAgent.AttackDirectionToMovementFlag(mainAgent.GetAttackDirection());
					}
					if (!flag9 && base.Input.IsGameKeyDown(10))
					{
						if (ManagedOptions.GetConfig(2) == 2f && MissionGameModels.Current.AutoBlockModel != null)
						{
							Agent.UsageDirection blockDirection = MissionGameModels.Current.AutoBlockModel.GetBlockDirection(base.Mission);
							if (blockDirection == 2)
							{
								mainAgent.MovementFlags |= 2048;
							}
							else if (blockDirection == 3)
							{
								mainAgent.MovementFlags |= 1024;
							}
							else if (blockDirection == null)
							{
								mainAgent.MovementFlags |= 4096;
							}
							else if (blockDirection == 1)
							{
								mainAgent.MovementFlags |= 8192;
							}
						}
						else
						{
							mainAgent.MovementFlags |= mainAgent.GetDefendMovementFlag();
						}
					}
				}
				if (!base.MissionScreen.IsRadialMenuActive && !base.Mission.IsOrderMenuOpen)
				{
					if (base.Input.IsGameKeyPressed(16) && (mainAgent.KickClear() || mainAgent.MountAgent != null))
					{
						mainAgent.EventControlFlags |= 32768;
					}
					if (base.Input.IsGameKeyPressed(18))
					{
						mainAgent.TryToWieldWeaponInSlot(0, 0, false);
					}
					else if (base.Input.IsGameKeyPressed(19))
					{
						mainAgent.TryToWieldWeaponInSlot(1, 0, false);
					}
					else if (base.Input.IsGameKeyPressed(20))
					{
						mainAgent.TryToWieldWeaponInSlot(2, 0, false);
					}
					else if (base.Input.IsGameKeyPressed(21))
					{
						mainAgent.TryToWieldWeaponInSlot(3, 0, false);
					}
					else if (base.Input.IsGameKeyPressed(11) && this._lastWieldNextPrimaryWeaponTriggerTime + 0.2f < Time.ApplicationTime)
					{
						this._lastWieldNextPrimaryWeaponTriggerTime = Time.ApplicationTime;
						mainAgent.WieldNextWeapon(0, 0);
					}
					else if (base.Input.IsGameKeyPressed(12) && this._lastWieldNextOffhandWeaponTriggerTime + 0.2f < Time.ApplicationTime)
					{
						this._lastWieldNextOffhandWeaponTriggerTime = Time.ApplicationTime;
						mainAgent.WieldNextWeapon(1, 0);
					}
					else if (base.Input.IsGameKeyPressed(23))
					{
						mainAgent.TryToSheathWeaponInHand(0, 0);
					}
					if (base.Input.IsGameKeyPressed(17) || this._weaponUsageToggleRequested)
					{
						mainAgent.EventControlFlags |= 1024;
						this._weaponUsageToggleRequested = false;
					}
					if (base.Input.IsGameKeyPressed(30))
					{
						mainAgent.EventControlFlags |= (mainAgent.WalkMode ? 4096 : 2048);
					}
					if (mainAgent.MountAgent != null)
					{
						if (base.Input.IsGameKeyPressed(15) || this._autoDismountModeActive)
						{
							if (mainAgent.GetCurrentVelocity().y < 0.5f && mainAgent.MountAgent.GetCurrentActionType(0) != 46)
							{
								mainAgent.EventControlFlags |= 1;
								return;
							}
							if (base.Input.IsGameKeyPressed(15))
							{
								this._autoDismountModeActive = true;
								mainAgent.EventControlFlags &= -458753;
								mainAgent.EventControlFlags |= 131072;
								return;
							}
						}
					}
					else if (base.Input.IsGameKeyPressed(15))
					{
						mainAgent.EventControlFlags |= (mainAgent.CrouchMode ? 16384 : 8192);
					}
				}
			}
		}

		private void HandleRangedWeaponAttackAlternativeAiming(Agent player)
		{
			if (base.Input.GetKeyState(254).x > 0.2f)
			{
				if (base.Input.GetKeyState(255).x < 0.6f)
				{
					player.MovementFlags |= player.AttackDirectionToMovementFlag(player.GetAttackDirection());
				}
				this._isPlayerAiming = true;
				return;
			}
			if (this._isPlayerAiming)
			{
				player.MovementFlags |= 4096;
				this._isPlayerAiming = false;
			}
		}

		private void HandleTriggeredWeaponAttack(Agent player)
		{
			if (base.Input.GetKeyState(255).x <= 0.2f)
			{
				if (this._isPlayerAiming)
				{
					this._playerShotMissile = false;
					this._isPlayerAiming = false;
					player.MovementFlags |= 4096;
				}
				return;
			}
			if (!this._isPlayerAiming && player.WieldedWeapon.MaxAmmo > 0 && player.WieldedWeapon.Ammo == 0)
			{
				player.MovementFlags |= player.AttackDirectionToMovementFlag(player.GetAttackDirection());
				return;
			}
			if (!this._playerShotMissile && base.Input.GetKeyState(255).x < 0.99f)
			{
				player.MovementFlags |= player.AttackDirectionToMovementFlag(player.GetAttackDirection());
				this._isPlayerAiming = true;
				return;
			}
			this._isPlayerAiming = true;
			this._playerShotMissile = true;
		}

		public override bool IsThereAgentAction(Agent userAgent, Agent otherAgent)
		{
			return otherAgent.IsMount && otherAgent.IsActive();
		}

		public void Disable()
		{
			this._activated = false;
		}

		public void Enable()
		{
			this._activated = true;
		}

		private void OnPlayerToggleOrder(MissionPlayerToggledOrderViewEvent obj)
		{
			this._isPlayerOrderOpen = obj.IsOrderEnabled;
		}

		public void OnWeaponUsageToggleRequested()
		{
			this._weaponUsageToggleRequested = true;
		}

		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType optionType)
		{
			if (optionType == 15)
			{
				this.UpdateLockTargetOption();
			}
		}

		private void UpdateLockTargetOption()
		{
			this._isTargetLockEnabled = ManagedOptions.GetConfig(15) == 1f;
			this.LockedAgent = null;
			this.PotentialLockTargetAgent = null;
			this._lastLockKeyPressTime = 0f;
			this._lastLockedAgentHeightDifference = 0f;
		}

		private const float _minValueForAimStart = 0.2f;

		private const float _maxValueForAttackEnd = 0.6f;

		private float _lastForwardKeyPressTime;

		private float _lastBackwardKeyPressTime;

		private float _lastLeftKeyPressTime;

		private float _lastRightKeyPressTime;

		private float _lastWieldNextPrimaryWeaponTriggerTime;

		private float _lastWieldNextOffhandWeaponTriggerTime;

		private bool _activated = true;

		private bool _strafeModeActive;

		private bool _autoDismountModeActive;

		private bool _isPlayerAgentAdded;

		private bool _isPlayerAiming;

		private bool _playerShotMissile;

		private bool _isPlayerOrderOpen;

		private bool _isTargetLockEnabled;

		private Agent.MovementControlFlag _lastMovementKeyPressed = 1;

		private Agent _lockedAgent;

		private Agent _potentialLockTargetAgent;

		private float _lastLockKeyPressTime;

		private float _lastLockedAgentHeightDifference;

		public readonly MissionMainAgentInteractionComponent InteractionComponent;

		public bool IsChatOpen;

		private bool _weaponUsageToggleRequested;

		public delegate void OnLockedAgentChangedDelegate(Agent newAgent);

		public delegate void OnPotentialLockedAgentChangedDelegate(Agent newPotentialAgent);
	}
}
