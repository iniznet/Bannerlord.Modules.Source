using System;
using System.ComponentModel;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	// Token: 0x0200004C RID: 76
	[DefaultView]
	public class MissionMainAgentController : MissionView
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000343 RID: 835 RVA: 0x0001CA34 File Offset: 0x0001AC34
		// (remove) Token: 0x06000344 RID: 836 RVA: 0x0001CA6C File Offset: 0x0001AC6C
		public event MissionMainAgentController.OnLockedAgentChangedDelegate OnLockedAgentChanged;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000345 RID: 837 RVA: 0x0001CAA4 File Offset: 0x0001ACA4
		// (remove) Token: 0x06000346 RID: 838 RVA: 0x0001CADC File Offset: 0x0001ACDC
		public event MissionMainAgentController.OnPotentialLockedAgentChangedDelegate OnPotentialLockedAgentChanged;

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000347 RID: 839 RVA: 0x0001CB11 File Offset: 0x0001AD11
		// (set) Token: 0x06000348 RID: 840 RVA: 0x0001CB19 File Offset: 0x0001AD19
		public bool IsDisabled { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000349 RID: 841 RVA: 0x0001CB22 File Offset: 0x0001AD22
		// (set) Token: 0x0600034A RID: 842 RVA: 0x0001CB2A File Offset: 0x0001AD2A
		public Vec3 CustomLookDir { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600034B RID: 843 RVA: 0x0001CB33 File Offset: 0x0001AD33
		// (set) Token: 0x0600034C RID: 844 RVA: 0x0001CB3B File Offset: 0x0001AD3B
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

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600034D RID: 845 RVA: 0x0001CB5E File Offset: 0x0001AD5E
		// (set) Token: 0x0600034E RID: 846 RVA: 0x0001CB66 File Offset: 0x0001AD66
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

		// Token: 0x0600034F RID: 847 RVA: 0x0001CB89 File Offset: 0x0001AD89
		public MissionMainAgentController()
		{
			this.InteractionComponent = new MissionMainAgentInteractionComponent(this);
			this.CustomLookDir = Vec3.Zero;
			this.IsChatOpen = false;
		}

		// Token: 0x06000350 RID: 848 RVA: 0x0001CBC0 File Offset: 0x0001ADC0
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

		// Token: 0x06000351 RID: 849 RVA: 0x0001CC74 File Offset: 0x0001AE74
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

		// Token: 0x06000352 RID: 850 RVA: 0x0001CD24 File Offset: 0x0001AF24
		public override bool IsReady()
		{
			bool flag = true;
			if (base.Mission.MainAgent != null)
			{
				flag = base.Mission.MainAgent.AgentVisuals.CheckResources(true);
			}
			return flag;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0001CD58 File Offset: 0x0001AF58
		private void Mission_OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (base.Mission.MainAgent != null)
			{
				this._isPlayerAgentAdded = true;
				this._strafeModeActive = false;
				this._autoDismountModeActive = false;
			}
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0001CD7C File Offset: 0x0001AF7C
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

		// Token: 0x06000355 RID: 853 RVA: 0x0001CE5A File Offset: 0x0001B05A
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (this.InteractionComponent.CurrentFocusedObject == affectedAgent || affectedAgent == base.Mission.MainAgent)
			{
				this.InteractionComponent.ClearFocus();
			}
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0001CE83 File Offset: 0x0001B083
		public override void OnAgentDeleted(Agent affectedAgent)
		{
			if (this.InteractionComponent.CurrentFocusedObject == affectedAgent)
			{
				this.InteractionComponent.ClearFocus();
			}
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0001CE9E File Offset: 0x0001B09E
		public override void OnClearScene()
		{
			this.InteractionComponent.OnClearScene();
		}

		// Token: 0x06000358 RID: 856 RVA: 0x0001CEAC File Offset: 0x0001B0AC
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

		// Token: 0x06000359 RID: 857 RVA: 0x0001D29D File Offset: 0x0001B49D
		private void AgentVisualsMovementCheck()
		{
			if (base.Input.IsGameKeyReleased(13))
			{
				this.BreakAgentVisualsInvulnerability();
			}
		}

		// Token: 0x0600035A RID: 858 RVA: 0x0001D2B4 File Offset: 0x0001B4B4
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

		// Token: 0x0600035B RID: 859 RVA: 0x0001D2EC File Offset: 0x0001B4EC
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

		// Token: 0x0600035C RID: 860 RVA: 0x0001D348 File Offset: 0x0001B548
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

		// Token: 0x0600035D RID: 861 RVA: 0x0001D564 File Offset: 0x0001B764
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
			if (mainAgent.State == 1 && mainAgent.IsCheering)
			{
				if (base.Input.IsGameKeyReleased(0) || base.Input.IsGameKeyReleased(1) || base.Input.IsGameKeyReleased(2) || base.Input.IsGameKeyReleased(3) || base.Input.IsGameKeyReleased(14) || base.Input.IsGameKeyReleased(9) || base.Input.IsGameKeyReleased(10))
				{
					if (GameNetwork.IsClient)
					{
						GameNetwork.BeginModuleEventAsClient();
						GameNetwork.WriteMessage(new CancelCheering());
						GameNetwork.EndModuleEventAsClient();
						return;
					}
					mainAgent.CancelCheering();
					return;
				}
			}
			else if (base.Mission.ClearSceneTimerElapsedTime >= 0f && mainAgent.State == 1)
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
					if (base.Input.IsGameKeyDown(9))
					{
						mainAgent.MovementFlags |= mainAgent.AttackDirectionToMovementFlag(mainAgent.GetAttackDirection(false));
					}
					if (base.Input.IsGameKeyDown(10))
					{
						if (ManagedOptions.GetConfig(1) == 2f && MissionGameModels.Current.AutoBlockModel != null)
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

		// Token: 0x0600035E RID: 862 RVA: 0x0001E108 File Offset: 0x0001C308
		public override bool IsThereAgentAction(Agent userAgent, Agent otherAgent)
		{
			return otherAgent.IsMount && otherAgent.IsActive();
		}

		// Token: 0x0600035F RID: 863 RVA: 0x0001E11A File Offset: 0x0001C31A
		public void Disable()
		{
			this._activated = false;
		}

		// Token: 0x06000360 RID: 864 RVA: 0x0001E123 File Offset: 0x0001C323
		public void Enable()
		{
			this._activated = true;
		}

		// Token: 0x06000361 RID: 865 RVA: 0x0001E12C File Offset: 0x0001C32C
		private void OnPlayerToggleOrder(MissionPlayerToggledOrderViewEvent obj)
		{
			this._isPlayerOrderOpen = obj.IsOrderEnabled;
		}

		// Token: 0x06000362 RID: 866 RVA: 0x0001E13A File Offset: 0x0001C33A
		public void OnWeaponUsageToggleRequested()
		{
			this._weaponUsageToggleRequested = true;
		}

		// Token: 0x06000363 RID: 867 RVA: 0x0001E143 File Offset: 0x0001C343
		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType optionType)
		{
			if (optionType == 14)
			{
				this.UpdateLockTargetOption();
			}
		}

		// Token: 0x06000364 RID: 868 RVA: 0x0001E150 File Offset: 0x0001C350
		private void UpdateLockTargetOption()
		{
			this._isTargetLockEnabled = ManagedOptions.GetConfig(14) == 1f;
			this.LockedAgent = null;
			this.PotentialLockTargetAgent = null;
			this._lastLockKeyPressTime = 0f;
			this._lastLockedAgentHeightDifference = 0f;
		}

		// Token: 0x0400023C RID: 572
		private float _lastForwardKeyPressTime;

		// Token: 0x0400023D RID: 573
		private float _lastBackwardKeyPressTime;

		// Token: 0x0400023E RID: 574
		private float _lastLeftKeyPressTime;

		// Token: 0x0400023F RID: 575
		private float _lastRightKeyPressTime;

		// Token: 0x04000240 RID: 576
		private float _lastWieldNextPrimaryWeaponTriggerTime;

		// Token: 0x04000241 RID: 577
		private float _lastWieldNextOffhandWeaponTriggerTime;

		// Token: 0x04000242 RID: 578
		private bool _activated = true;

		// Token: 0x04000243 RID: 579
		private bool _strafeModeActive;

		// Token: 0x04000244 RID: 580
		private bool _autoDismountModeActive;

		// Token: 0x04000245 RID: 581
		private bool _isPlayerAgentAdded;

		// Token: 0x04000246 RID: 582
		private bool _isPlayerOrderOpen;

		// Token: 0x04000247 RID: 583
		private bool _isTargetLockEnabled;

		// Token: 0x04000248 RID: 584
		private Agent.MovementControlFlag _lastMovementKeyPressed = 1;

		// Token: 0x04000249 RID: 585
		private Agent _lockedAgent;

		// Token: 0x0400024A RID: 586
		private Agent _potentialLockTargetAgent;

		// Token: 0x0400024B RID: 587
		private float _lastLockKeyPressTime;

		// Token: 0x0400024C RID: 588
		private float _lastLockedAgentHeightDifference;

		// Token: 0x0400024D RID: 589
		public readonly MissionMainAgentInteractionComponent InteractionComponent;

		// Token: 0x0400024E RID: 590
		public bool IsChatOpen;

		// Token: 0x0400024F RID: 591
		private bool _weaponUsageToggleRequested;

		// Token: 0x020000BA RID: 186
		// (Invoke) Token: 0x06000556 RID: 1366
		public delegate void OnLockedAgentChangedDelegate(Agent newAgent);

		// Token: 0x020000BB RID: 187
		// (Invoke) Token: 0x0600055A RID: 1370
		public delegate void OnPotentialLockedAgentChangedDelegate(Agent newPotentialAgent);
	}
}
