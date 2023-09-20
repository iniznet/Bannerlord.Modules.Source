using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200037F RID: 895
	public class SpawnedItemEntity : UsableMissionObject
	{
		// Token: 0x170008AC RID: 2220
		// (get) Token: 0x0600306A RID: 12394 RVA: 0x000C6D17 File Offset: 0x000C4F17
		public MissionWeapon WeaponCopy
		{
			get
			{
				return this._weapon;
			}
		}

		// Token: 0x170008AD RID: 2221
		// (get) Token: 0x0600306B RID: 12395 RVA: 0x000C6D1F File Offset: 0x000C4F1F
		// (set) Token: 0x0600306C RID: 12396 RVA: 0x000C6D27 File Offset: 0x000C4F27
		public bool HasLifeTime
		{
			get
			{
				return this._hasLifeTime;
			}
			set
			{
				if (this._hasLifeTime != value)
				{
					this._hasLifeTime = value;
					base.SetScriptComponentToTickMT(this.GetTickRequirement());
				}
			}
		}

		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x0600306D RID: 12397 RVA: 0x000C6D45 File Offset: 0x000C4F45
		// (set) Token: 0x0600306E RID: 12398 RVA: 0x000C6D4D File Offset: 0x000C4F4D
		private bool PhysicsStopped
		{
			get
			{
				return this._physicsStopped;
			}
			set
			{
				if (this._physicsStopped != value)
				{
					this._physicsStopped = value;
					base.SetScriptComponentToTickMT(this.GetTickRequirement());
				}
			}
		}

		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x0600306F RID: 12399 RVA: 0x000C6D6B File Offset: 0x000C4F6B
		public bool IsRemoved
		{
			get
			{
				return this._ownerGameEntity == null;
			}
		}

		// Token: 0x06003070 RID: 12400 RVA: 0x000C6D7C File Offset: 0x000C4F7C
		public TextObject GetActionMessage(ItemObject weaponToReplaceWith, bool fillUp)
		{
			GameTexts.SetVariable("USE_KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			MBTextManager.SetTextVariable("KEY", GameTexts.FindText("str_ui_agent_interaction_use", null), false);
			if (weaponToReplaceWith == null)
			{
				MBTextManager.SetTextVariable("ACTION", fillUp ? GameTexts.FindText("str_ui_fill", null) : GameTexts.FindText("str_ui_equip", null), false);
			}
			else
			{
				MBTextManager.SetTextVariable("ACTION", GameTexts.FindText("str_ui_swap", null), false);
				MBTextManager.SetTextVariable("ITEM_NAME", weaponToReplaceWith.Name, false);
			}
			return GameTexts.FindText("str_key_action", null);
		}

		// Token: 0x06003071 RID: 12401 RVA: 0x000C6E18 File Offset: 0x000C5018
		public TextObject GetDescriptionMessage(bool fillUp)
		{
			if (!fillUp)
			{
				return this._weapon.GetModifiedItemName();
			}
			return GameTexts.FindText("str_inventory_weapon", ((int)this._weapon.CurrentUsageItem.WeaponClass).ToString());
		}

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x06003072 RID: 12402 RVA: 0x000C6E56 File Offset: 0x000C5056
		protected internal override bool LockUserFrames
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x06003073 RID: 12403 RVA: 0x000C6E59 File Offset: 0x000C5059
		// (set) Token: 0x06003074 RID: 12404 RVA: 0x000C6E61 File Offset: 0x000C5061
		public Mission.WeaponSpawnFlags SpawnFlags { get; private set; }

		// Token: 0x06003075 RID: 12405 RVA: 0x000C6E6C File Offset: 0x000C506C
		public void Initialize(MissionWeapon weapon, bool hasLifeTime, Mission.WeaponSpawnFlags spawnFlags, in Vec3 fakeSimulationVelocity)
		{
			this._weapon = weapon;
			this.HasLifeTime = hasLifeTime;
			this.SpawnFlags = spawnFlags;
			this._fakeSimulationVelocity = fakeSimulationVelocity;
			if (this.HasLifeTime)
			{
				float num = 0f;
				if (!this._weapon.IsEmpty)
				{
					num = (this._weapon.Item.ItemFlags.HasAnyFlag(ItemFlags.QuickFadeOut) ? 5f : 180f);
					base.IsDeactivated = this._weapon.Item.ItemFlags.HasAnyFlag(ItemFlags.CannotBePickedUp);
					if (this._weapon.CurrentUsageItem.WeaponFlags.HasAllFlags(WeaponFlags.RangedWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.Consumable))
					{
						this._lastSoundPlayTime = 0.333f;
					}
					else
					{
						this._lastSoundPlayTime = -0.333f;
					}
				}
				else
				{
					base.IsDeactivated = true;
				}
				this._deletionTimer = new Timer(Mission.Current.CurrentTime, num, true);
			}
			else
			{
				this._deletionTimer = new Timer(Mission.Current.CurrentTime, float.MaxValue, true);
			}
			if (spawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.WithPhysics))
			{
				this._disablePhysicsTimer = new Timer(Mission.Current.CurrentTime, 10f, true);
			}
		}

		// Token: 0x06003076 RID: 12406 RVA: 0x000C6F9C File Offset: 0x000C519C
		protected internal override void OnInit()
		{
			base.OnInit();
			this._ownerGameEntity = base.GameEntity;
			if (!string.IsNullOrEmpty(this.WeaponName))
			{
				ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(this.WeaponName);
				this._weapon = new MissionWeapon(@object, null, null);
			}
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06003077 RID: 12407 RVA: 0x000C6FF8 File Offset: 0x000C51F8
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (GameNetwork.IsClientOrReplay || base.HasUser || !this.PhysicsStopped)
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel2;
			}
			if (!this.HasLifeTime)
			{
				return base.GetTickRequirement();
			}
			if (!base.GetTickRequirement().HasAnyFlag(ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel2))
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.TickOccasionally;
			}
			return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel2;
		}

		// Token: 0x06003078 RID: 12408 RVA: 0x000C7058 File Offset: 0x000C5258
		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (GameNetwork.IsClientOrReplay && this._clientSyncData != null)
			{
				if (this._clientSyncData.Timer.Check(Mission.Current.CurrentTime))
				{
					this._ownerGameEntity.SetAlpha(1f);
					this._clientSyncData = null;
					return;
				}
				float duration = this._clientSyncData.Timer.Duration;
				float num = MBMath.ClampFloat(this._clientSyncData.Timer.ElapsedTime() / duration, 0f, 1f);
				if (num < (1f - 0.1f / duration) * 0.5f)
				{
					this._ownerGameEntity.SetAlpha(1f - num * 2f);
					return;
				}
				if (num < (1f + 0.1f / duration) * 0.5f)
				{
					this._ownerGameEntity.SetAlpha(0f);
					this._ownerGameEntity.SetGlobalFrame(this._clientSyncData.Frame);
					GameEntity parent = this._clientSyncData.Parent;
					if (parent != null)
					{
						parent.AddChild(this._ownerGameEntity, true);
					}
					this._clientSyncData.Timer.Reset(Mission.Current.CurrentTime - duration * (1f + 0.1f / duration) * 0.5f);
					return;
				}
				this._ownerGameEntity.SetAlpha(num * 2f - 1f);
			}
		}

		// Token: 0x06003079 RID: 12409 RVA: 0x000C71BC File Offset: 0x000C53BC
		protected internal override void OnTickParallel2(float dt)
		{
			base.OnTickParallel2(dt);
			if (!GameNetwork.IsClientOrReplay)
			{
				if (base.HasUser)
				{
					ActionIndexValueCache currentActionValue = base.UserAgent.GetCurrentActionValue(this._usedChannelIndex);
					if (currentActionValue == this._successActionIndex)
					{
						base.UserAgent.StopUsingGameObjectMT(base.UserAgent.CanUseObject(this), Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
					else if (currentActionValue != this._progressActionIndex)
					{
						base.UserAgent.StopUsingGameObjectMT(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
				}
				else if (this.HasLifeTime && this._deletionTimer.Check(Mission.Current.CurrentTime))
				{
					this._readyToBeDeleted = true;
				}
				if (!this.PhysicsStopped)
				{
					if (this._ownerGameEntity != null)
					{
						if (this._weapon.IsBanner())
						{
							MatrixFrame globalFrame = this._ownerGameEntity.GetGlobalFrame();
							this._fakeSimulationVelocity.z = this._fakeSimulationVelocity.z - dt * 9.8f;
							globalFrame.origin += this._fakeSimulationVelocity * dt;
							this._ownerGameEntity.SetGlobalFrameMT(globalFrame);
							using (new TWSharedMutexReadLock(Scene.PhysicsAndRayCastLock))
							{
								if (this._ownerGameEntity.Scene.GetGroundHeightAtPositionMT(globalFrame.origin, BodyFlags.CommonCollisionExcludeFlags) > globalFrame.origin.z + 0.3f)
								{
									this.PhysicsStopped = true;
								}
								return;
							}
						}
						Vec3 globalPosition = this._ownerGameEntity.GlobalPosition;
						if (globalPosition.z <= CompressionBasic.PositionCompressionInfo.GetMinimumValue() + 5f)
						{
							this._readyToBeDeleted = true;
						}
						if (!this._ownerGameEntity.BodyFlag.HasAnyFlag(BodyFlags.Dynamic))
						{
							this.PhysicsStopped = true;
							return;
						}
						MatrixFrame globalFrame2 = this._ownerGameEntity.GetGlobalFrame();
						if (!globalFrame2.rotation.IsUnit())
						{
							globalFrame2.rotation.Orthonormalize();
							this._ownerGameEntity.SetGlobalFrame(globalFrame2);
						}
						bool flag = this._disablePhysicsTimer.Check(Mission.Current.CurrentTime);
						if (flag || this._disablePhysicsTimer.ElapsedTime() > 1f)
						{
							bool flag2;
							using (new TWSharedMutexUpgradeableReadLock(Scene.PhysicsAndRayCastLock))
							{
								flag2 = flag || this._ownerGameEntity.IsDynamicBodyStationaryMT();
								if (flag2)
								{
									this._groundEntityWhenDisabled = this.TryFindProperGroundEntityForSpawnedEntity();
									if (this._groundEntityWhenDisabled != null)
									{
										this._groundEntityWhenDisabled.AddChild(base.GameEntity, true);
									}
									using (new TWSharedMutexWriteLock(Scene.PhysicsAndRayCastLock))
									{
										if (!this._weapon.IsEmpty && !this._ownerGameEntity.BodyFlag.HasAnyFlag(BodyFlags.Disabled))
										{
											this._ownerGameEntity.DisableDynamicBodySimulationMT();
										}
										else
										{
											this._ownerGameEntity.RemovePhysicsMT(false);
										}
									}
								}
							}
							if (flag2)
							{
								this.ClampEntityPositionForStoppingIfNeeded();
								this.PhysicsStopped = true;
								if ((!base.IsDeactivated || this._groundEntityWhenDisabled != null) && !this._weapon.IsEmpty && GameNetwork.IsServerOrRecorder)
								{
									GameNetwork.BeginBroadcastModuleEvent();
									MissionObjectId id = base.Id;
									GameEntity groundEntityWhenDisabled = this._groundEntityWhenDisabled;
									GameNetwork.WriteMessage(new StopPhysicsAndSetFrameOfMissionObject(id, (groundEntityWhenDisabled != null) ? groundEntityWhenDisabled.GetFirstScriptOfType<MissionObject>() : null, this._ownerGameEntity.GetFrame()));
									GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
								}
							}
						}
						if (!this.PhysicsStopped)
						{
							Vec3 vec;
							Vec3 vec2;
							this._ownerGameEntity.GetPhysicsMinMax(true, out vec, out vec2, true);
							MatrixFrame globalFrame3 = this._ownerGameEntity.GetGlobalFrame();
							MatrixFrame previousGlobalFrame = this._ownerGameEntity.GetPreviousGlobalFrame();
							Vec3 vec3 = globalFrame3.TransformToParent(vec);
							Vec3 vec4 = previousGlobalFrame.TransformToParent(vec);
							Vec3 vec5 = globalFrame3.TransformToParent(vec2);
							Vec3 vec6 = previousGlobalFrame.TransformToParent(vec2);
							Vec3 vec7 = Vec3.Vec3Min(vec3, vec5);
							Vec3 vec8 = Vec3.Vec3Min(vec4, vec6);
							float waterLevelAtPositionMT;
							using (new TWSharedMutexReadLock(Scene.PhysicsAndRayCastLock))
							{
								waterLevelAtPositionMT = Mission.Current.GetWaterLevelAtPositionMT(vec7.AsVec2);
							}
							bool flag3 = vec7.z < waterLevelAtPositionMT;
							if (vec8.z >= waterLevelAtPositionMT && flag3)
							{
								Vec3 linearVelocityMT;
								using (new TWSharedMutexReadLock(Scene.PhysicsAndRayCastLock))
								{
									linearVelocityMT = this._ownerGameEntity.GetLinearVelocityMT();
								}
								float num = this._ownerGameEntity.Mass * linearVelocityMT.Length;
								if (num > 0f)
								{
									num *= 0.0625f;
									num = MathF.Min(num, 1f);
									Vec3 vec9 = globalPosition;
									vec9.z = waterLevelAtPositionMT;
									SoundEventParameter soundEventParameter = new SoundEventParameter("Size", num);
									Mission.Current.MakeSound(ItemPhysicsSoundContainer.SoundCodePhysicsWater, vec9, true, false, -1, -1, ref soundEventParameter);
									return;
								}
							}
						}
					}
					else
					{
						this.PhysicsStopped = true;
					}
				}
			}
		}

		// Token: 0x0600307A RID: 12410 RVA: 0x000C76AC File Offset: 0x000C58AC
		private GameEntity TryFindProperGroundEntityForSpawnedEntity()
		{
			Vec3 vec;
			Vec3 vec2;
			this._ownerGameEntity.GetPhysicsMinMax(true, out vec, out vec2, false);
			float num = vec2.z - vec.z;
			vec.z = vec2.z - 0.001f;
			Vec3 vec3 = (vec2 + vec) * 0.5f;
			float num2;
			Vec3 vec4;
			this._ownerGameEntity.Scene.RayCastForClosestEntityOrTerrainMT(vec3, vec3 - new Vec3(0f, 0f, num + 0.05f, -1f), out num2, out vec4, out this._groundEntityWhenDisabled, 0.01f, BodyFlags.CommonCollisionExcludeFlags);
			GameEntity groundEntityWhenDisabled = this._groundEntityWhenDisabled;
			GameEntity gameEntity;
			if (groundEntityWhenDisabled == null)
			{
				gameEntity = null;
			}
			else
			{
				MissionObject firstScriptOfTypeInFamily = groundEntityWhenDisabled.GetFirstScriptOfTypeInFamily<MissionObject>();
				gameEntity = ((firstScriptOfTypeInFamily != null) ? firstScriptOfTypeInFamily.GameEntity : null);
			}
			this._groundEntityWhenDisabled = gameEntity;
			if (MathF.Abs(vec4.z - vec3.z) <= num + 0.05f)
			{
				if (this._groundEntityWhenDisabled != null)
				{
					return this._groundEntityWhenDisabled;
				}
				return null;
			}
			else
			{
				this._ownerGameEntity.Scene.BoxCast(vec, vec2, false, Vec3.Zero, -Vec3.Up, num + 0.05f, out num2, out vec4, out this._groundEntityWhenDisabled, BodyFlags.CommonCollisionExcludeFlags);
				GameEntity groundEntityWhenDisabled2 = this._groundEntityWhenDisabled;
				GameEntity gameEntity2;
				if (groundEntityWhenDisabled2 == null)
				{
					gameEntity2 = null;
				}
				else
				{
					MissionObject firstScriptOfTypeInFamily2 = groundEntityWhenDisabled2.GetFirstScriptOfTypeInFamily<MissionObject>();
					gameEntity2 = ((firstScriptOfTypeInFamily2 != null) ? firstScriptOfTypeInFamily2.GameEntity : null);
				}
				this._groundEntityWhenDisabled = gameEntity2;
				if (this._groundEntityWhenDisabled != null && MathF.Abs(vec4.z - vec3.z) <= num + 0.05f)
				{
					return this._groundEntityWhenDisabled;
				}
				return null;
			}
		}

		// Token: 0x0600307B RID: 12411 RVA: 0x000C782A File Offset: 0x000C5A2A
		protected internal override void OnTickOccasionally(float currentFrameDeltaTime)
		{
			this.OnTickParallel2(currentFrameDeltaTime);
		}

		// Token: 0x0600307C RID: 12412 RVA: 0x000C7834 File Offset: 0x000C5A34
		private void ClampEntityPositionForStoppingIfNeeded()
		{
			GameEntity gameEntity = base.GameEntity;
			float minimumValue = CompressionBasic.PositionCompressionInfo.GetMinimumValue();
			float maximumValue = CompressionBasic.PositionCompressionInfo.GetMaximumValue();
			Vec3 vec = gameEntity.GetFrame().origin;
			bool flag;
			vec = vec.ClampedCopy(minimumValue, maximumValue, out flag);
			if (flag)
			{
				gameEntity.SetLocalPosition(vec);
			}
		}

		// Token: 0x0600307D RID: 12413 RVA: 0x000C7881 File Offset: 0x000C5A81
		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			if (base.CreatedAtRuntime)
			{
				Mission.Current.AddSpawnedItemEntityCreatedAtRuntime(this);
			}
		}

		// Token: 0x0600307E RID: 12414 RVA: 0x000C789C File Offset: 0x000C5A9C
		protected override void OnRemoved(int removeReason)
		{
			if (base.HasUser && !GameNetwork.IsClientOrReplay)
			{
				base.UserAgent.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
			}
			base.OnRemoved(removeReason);
			base.InvalidateWeakPointersIfValid();
			this._ownerGameEntity = null;
			Agent userAgent = base.UserAgent;
			if (userAgent != null)
			{
				userAgent.OnItemRemovedFromScene();
			}
			Agent movingAgent = this.MovingAgent;
			if (movingAgent == null)
			{
				return;
			}
			movingAgent.OnItemRemovedFromScene();
		}

		// Token: 0x0600307F RID: 12415 RVA: 0x000C78FA File Offset: 0x000C5AFA
		public void AttachWeaponToWeapon(MissionWeapon attachedWeapon, ref MatrixFrame attachLocalFrame)
		{
			this._weapon.AttachWeapon(attachedWeapon, ref attachLocalFrame);
		}

		// Token: 0x06003080 RID: 12416 RVA: 0x000C790C File Offset: 0x000C5B0C
		public bool IsReadyToBeDeleted()
		{
			return (!base.HasUser && this._readyToBeDeleted) || (this._groundEntityWhenDisabled != null && !this._groundEntityWhenDisabled.HasScene()) || (this._groundEntityWhenDisabled != null && !this._groundEntityWhenDisabled.IsVisibleIncludeParents() && (!this._groundEntityWhenDisabled.HasBody() || this._groundEntityWhenDisabled.BodyFlag.HasAnyFlag(BodyFlags.Disabled)));
		}

		// Token: 0x06003081 RID: 12417 RVA: 0x000C7984 File Offset: 0x000C5B84
		public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
			base.OnUseStopped(userAgent, isSuccessful, preferenceIndex);
			if (isSuccessful)
			{
				if (this._clientSyncData != null)
				{
					this._clientSyncData = null;
					base.GameEntity.SetAlpha(1f);
				}
				bool flag;
				userAgent.OnItemPickup(this, (EquipmentIndex)preferenceIndex, out flag);
				if (flag)
				{
					this._readyToBeDeleted = true;
					this.PhysicsStopped = true;
					base.IsDeactivated = true;
				}
			}
		}

		// Token: 0x06003082 RID: 12418 RVA: 0x000C79E0 File Offset: 0x000C5BE0
		public override void OnUse(Agent userAgent)
		{
			base.OnUse(userAgent);
			if (!GameNetwork.IsClientOrReplay)
			{
				MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
				float num = globalFrame.origin.z;
				num = Math.Max(num, globalFrame.origin.z + globalFrame.rotation.u.z * (float)this._weapon.CurrentUsageItem.WeaponLength * 0.0075f);
				float eyeGlobalHeight = userAgent.GetEyeGlobalHeight();
				bool isLeftStance = userAgent.GetIsLeftStance();
				ItemObject.ItemTypeEnum itemType = this._weapon.Item.ItemType;
				if (userAgent.HasMount)
				{
					this._usedChannelIndex = 1;
					MatrixFrame frame = userAgent.Frame;
					bool flag = Vec2.DotProduct(frame.rotation.f.AsVec2.LeftVec(), (base.GameEntity.GetGlobalFrame().origin - frame.origin).AsVec2) > 0f;
					if (num < eyeGlobalHeight * 0.7f + userAgent.Position.z)
					{
						if (this._weapon.Item.ItemFlags.HasAnyFlag(ItemFlags.AttachmentMask) || itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Shield)
						{
							this._progressActionIndex = (flag ? SpawnedItemEntity.act_pickup_from_left_down_horseback_left_begin : SpawnedItemEntity.act_pickup_from_right_down_horseback_left_begin);
							this._successActionIndex = (flag ? SpawnedItemEntity.act_pickup_from_left_down_horseback_left_end : SpawnedItemEntity.act_pickup_from_right_down_horseback_left_end);
						}
						else
						{
							this._progressActionIndex = (flag ? SpawnedItemEntity.act_pickup_from_left_down_horseback_begin : SpawnedItemEntity.act_pickup_from_right_down_horseback_begin);
							this._successActionIndex = (flag ? SpawnedItemEntity.act_pickup_from_left_down_horseback_end : SpawnedItemEntity.act_pickup_from_right_down_horseback_end);
						}
					}
					else if (num < eyeGlobalHeight * 1.1f + userAgent.Position.z)
					{
						if (this._weapon.Item.ItemFlags.HasAnyFlag(ItemFlags.AttachmentMask) || itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Shield)
						{
							this._progressActionIndex = (flag ? SpawnedItemEntity.act_pickup_from_left_middle_horseback_left_begin : SpawnedItemEntity.act_pickup_from_right_middle_horseback_left_begin);
							this._successActionIndex = (flag ? SpawnedItemEntity.act_pickup_from_left_middle_horseback_left_end : SpawnedItemEntity.act_pickup_from_right_middle_horseback_left_end);
						}
						else
						{
							this._progressActionIndex = (flag ? SpawnedItemEntity.act_pickup_from_left_middle_horseback_begin : SpawnedItemEntity.act_pickup_from_right_middle_horseback_begin);
							this._successActionIndex = (flag ? SpawnedItemEntity.act_pickup_from_left_middle_horseback_end : SpawnedItemEntity.act_pickup_from_right_middle_horseback_end);
						}
					}
					else if (this._weapon.Item.ItemFlags.HasAnyFlag(ItemFlags.AttachmentMask) || itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Shield)
					{
						this._progressActionIndex = (flag ? SpawnedItemEntity.act_pickup_from_left_up_horseback_left_begin : SpawnedItemEntity.act_pickup_from_right_up_horseback_left_begin);
						this._successActionIndex = (flag ? SpawnedItemEntity.act_pickup_from_left_up_horseback_left_end : SpawnedItemEntity.act_pickup_from_right_up_horseback_left_end);
					}
					else
					{
						this._progressActionIndex = (flag ? SpawnedItemEntity.act_pickup_from_left_up_horseback_begin : SpawnedItemEntity.act_pickup_from_right_up_horseback_begin);
						this._successActionIndex = (flag ? SpawnedItemEntity.act_pickup_from_left_up_horseback_end : SpawnedItemEntity.act_pickup_from_right_up_horseback_end);
					}
				}
				else if (this._weapon.CurrentUsageItem.WeaponFlags.HasAllFlags(WeaponFlags.RangedWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.Consumable))
				{
					this._usedChannelIndex = 0;
					this._progressActionIndex = SpawnedItemEntity.act_pickup_boulder_begin;
					this._successActionIndex = SpawnedItemEntity.act_pickup_boulder_end;
				}
				else if (num < eyeGlobalHeight * 0.4f + userAgent.Position.z)
				{
					this._usedChannelIndex = 0;
					if (this._weapon.Item.ItemFlags.HasAnyFlag(ItemFlags.AttachmentMask) || itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Shield)
					{
						this._progressActionIndex = (isLeftStance ? SpawnedItemEntity.act_pickup_down_left_begin_left_stance : SpawnedItemEntity.act_pickup_down_left_begin);
						this._successActionIndex = (isLeftStance ? SpawnedItemEntity.act_pickup_down_left_end_left_stance : SpawnedItemEntity.act_pickup_down_left_end);
					}
					else
					{
						this._progressActionIndex = (isLeftStance ? SpawnedItemEntity.act_pickup_down_begin_left_stance : SpawnedItemEntity.act_pickup_down_begin);
						this._successActionIndex = (isLeftStance ? SpawnedItemEntity.act_pickup_down_end_left_stance : SpawnedItemEntity.act_pickup_down_end);
					}
				}
				else if (num < eyeGlobalHeight * 1.1f + userAgent.Position.z)
				{
					this._usedChannelIndex = 1;
					if (this._weapon.Item.ItemFlags.HasAnyFlag(ItemFlags.AttachmentMask) || itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Shield)
					{
						this._progressActionIndex = (isLeftStance ? SpawnedItemEntity.act_pickup_middle_left_begin_left_stance : SpawnedItemEntity.act_pickup_middle_left_begin);
						this._successActionIndex = (isLeftStance ? SpawnedItemEntity.act_pickup_middle_left_end_left_stance : SpawnedItemEntity.act_pickup_middle_left_end);
					}
					else
					{
						this._progressActionIndex = (isLeftStance ? SpawnedItemEntity.act_pickup_middle_begin_left_stance : SpawnedItemEntity.act_pickup_middle_begin);
						this._successActionIndex = (isLeftStance ? SpawnedItemEntity.act_pickup_middle_end_left_stance : SpawnedItemEntity.act_pickup_middle_end);
					}
				}
				else
				{
					this._usedChannelIndex = 1;
					if (this._weapon.Item.ItemFlags.HasAnyFlag(ItemFlags.AttachmentMask) || itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Shield)
					{
						this._progressActionIndex = (isLeftStance ? SpawnedItemEntity.act_pickup_up_left_begin_left_stance : SpawnedItemEntity.act_pickup_up_left_begin);
						this._successActionIndex = (isLeftStance ? SpawnedItemEntity.act_pickup_up_left_end_left_stance : SpawnedItemEntity.act_pickup_up_left_end);
					}
					else
					{
						this._progressActionIndex = (isLeftStance ? SpawnedItemEntity.act_pickup_up_begin_left_stance : SpawnedItemEntity.act_pickup_up_begin);
						this._successActionIndex = (isLeftStance ? SpawnedItemEntity.act_pickup_up_end_left_stance : SpawnedItemEntity.act_pickup_up_end);
					}
				}
				userAgent.SetActionChannel(this._usedChannelIndex, this._progressActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
		}

		// Token: 0x06003083 RID: 12419 RVA: 0x000C7EE4 File Offset: 0x000C60E4
		public override bool IsDisabledForAgent(Agent agent)
		{
			return (this._weapon.IsAnyConsumable() && this._weapon.Amount == 0) || (this._weapon.IsBanner() && !MissionGameModels.Current.BattleBannerBearersModel.IsInteractableFormationBanner(this, agent));
		}

		// Token: 0x06003084 RID: 12420 RVA: 0x000C7F30 File Offset: 0x000C6130
		protected internal override void OnPhysicsCollision(ref PhysicsContact contact)
		{
			if (!GameNetwork.IsDedicatedServer && contact.NumberOfContactPairs > 0)
			{
				PhysicsContactInfo physicsContactInfo = default(PhysicsContactInfo);
				bool flag = false;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < contact.NumberOfContactPairs; i++)
				{
					for (int j = 0; j < contact[i].NumberOfContacts; j++)
					{
						if (!flag || contact[i][j].Impulse.LengthSquared > physicsContactInfo.Impulse.LengthSquared)
						{
							physicsContactInfo = contact[i][j];
							flag = true;
						}
					}
					switch (contact[i].ContactEventType)
					{
					case PhysicsEventType.CollisionStart:
						num++;
						break;
					case PhysicsEventType.CollisionStay:
						num2++;
						break;
					case PhysicsEventType.CollisionEnd:
						num3++;
						break;
					default:
						Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Usables\\SpawnedItemEntity.cs", "OnPhysicsCollision", 778);
						break;
					}
				}
				if (num2 > 0)
				{
					this.PlayPhysicsRollSound(physicsContactInfo.Impulse, physicsContactInfo.Position, physicsContactInfo.PhysicsMaterial1);
					return;
				}
				if (num > 0)
				{
					this.PlayPhysicsCollisionSound(physicsContactInfo.Impulse, physicsContactInfo.PhysicsMaterial1, physicsContactInfo.Position);
				}
			}
		}

		// Token: 0x06003085 RID: 12421 RVA: 0x000C8070 File Offset: 0x000C6270
		private void PlayPhysicsCollisionSound(Vec3 impulse, PhysicsMaterial collidedMat, Vec3 collisionPoint)
		{
			float num = this._deletionTimer.ElapsedTime();
			if (impulse.LengthSquared > 0.0025000002f && this._lastSoundPlayTime + 0.333f < num)
			{
				this._lastSoundPlayTime = num;
				WeaponClass weaponClass = this._weapon.CurrentUsageItem.WeaponClass;
				float num2 = impulse.Length;
				bool flag = false;
				int num3;
				int num4;
				int num5;
				switch (weaponClass)
				{
				case WeaponClass.Dagger:
				case WeaponClass.ThrowingAxe:
				case WeaponClass.ThrowingKnife:
					num3 = ItemPhysicsSoundContainer.SoundCodePhysicsDaggerlikeDefault;
					num4 = ItemPhysicsSoundContainer.SoundCodePhysicsDaggerlikeWood;
					num5 = ItemPhysicsSoundContainer.SoundCodePhysicsDaggerlikeStone;
					goto IL_1A0;
				case WeaponClass.OneHandedSword:
				case WeaponClass.OneHandedAxe:
				case WeaponClass.Mace:
					num3 = ItemPhysicsSoundContainer.SoundCodePhysicsSwordlikeDefault;
					num4 = ItemPhysicsSoundContainer.SoundCodePhysicsSwordlikeWood;
					num5 = ItemPhysicsSoundContainer.SoundCodePhysicsSwordlikeStone;
					goto IL_1A0;
				case WeaponClass.TwoHandedSword:
				case WeaponClass.TwoHandedAxe:
				case WeaponClass.TwoHandedMace:
					num3 = ItemPhysicsSoundContainer.SoundCodePhysicsGreatswordlikeDefault;
					num4 = ItemPhysicsSoundContainer.SoundCodePhysicsGreatswordlikeWood;
					num5 = ItemPhysicsSoundContainer.SoundCodePhysicsGreatswordlikeStone;
					goto IL_1A0;
				case WeaponClass.OneHandedPolearm:
				case WeaponClass.TwoHandedPolearm:
				case WeaponClass.LowGripPolearm:
					num3 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeDefault;
					num4 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeWood;
					num5 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeStone;
					goto IL_1A0;
				case WeaponClass.Arrow:
				case WeaponClass.Bolt:
					num3 = ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeDefault;
					num4 = ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeWood;
					num5 = ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeStone;
					goto IL_1A0;
				case WeaponClass.Bow:
				case WeaponClass.Crossbow:
				case WeaponClass.Javelin:
					num3 = ItemPhysicsSoundContainer.SoundCodePhysicsBowlikeDefault;
					num4 = ItemPhysicsSoundContainer.SoundCodePhysicsBowlikeWood;
					num5 = ItemPhysicsSoundContainer.SoundCodePhysicsBowlikeStone;
					goto IL_1A0;
				case WeaponClass.Stone:
					num3 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeDefault;
					num4 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeWood;
					num5 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeStone;
					goto IL_1A0;
				case WeaponClass.Boulder:
					num3 = ItemPhysicsSoundContainer.SoundCodePhysicsBoulderDefault;
					num4 = ItemPhysicsSoundContainer.SoundCodePhysicsBoulderWood;
					num5 = ItemPhysicsSoundContainer.SoundCodePhysicsBoulderStone;
					flag = true;
					goto IL_1A0;
				case WeaponClass.SmallShield:
				case WeaponClass.LargeShield:
					num3 = ItemPhysicsSoundContainer.SoundCodePhysicsShieldlikeDefault;
					num4 = ItemPhysicsSoundContainer.SoundCodePhysicsShieldlikeWood;
					num5 = ItemPhysicsSoundContainer.SoundCodePhysicsShieldlikeStone;
					goto IL_1A0;
				}
				num3 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeDefault;
				num4 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeWood;
				num5 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeStone;
				IL_1A0:
				if (!flag)
				{
					num2 *= 0.16666667f;
					num2 = MBMath.ClampFloat(num2, 0f, 1f);
				}
				else
				{
					num2 = (num2 - 7f) * 0.030303031f * 0.1f + 0.9f;
					num2 = MBMath.ClampFloat(num2, 0.9f, 1f);
				}
				string name = collidedMat.Name;
				int num6 = num3;
				if (name.Contains("wood"))
				{
					num6 = num4;
				}
				else if (name.Contains("stone"))
				{
					num6 = num5;
				}
				SoundEventParameter soundEventParameter = new SoundEventParameter("Force", num2);
				Mission.Current.MakeSound(num6, collisionPoint, true, false, -1, -1, ref soundEventParameter);
			}
		}

		// Token: 0x06003086 RID: 12422 RVA: 0x000C82C0 File Offset: 0x000C64C0
		private void PlayPhysicsRollSound(Vec3 impulse, Vec3 collisionPoint, PhysicsMaterial collidedMat)
		{
			WeaponComponentData currentUsageItem = this._weapon.CurrentUsageItem;
			if (currentUsageItem.WeaponClass == WeaponClass.Boulder && currentUsageItem.WeaponFlags.HasAllFlags(WeaponFlags.RangedWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.Consumable))
			{
				float num = this._deletionTimer.ElapsedTime();
				if (impulse.LengthSquared > 0.0001f && this._lastSoundPlayTime + 0.333f < num)
				{
					if (this._rollingSoundEvent == null || !this._rollingSoundEvent.IsValid)
					{
						this._lastSoundPlayTime = num;
						int num2 = ItemPhysicsSoundContainer.SoundCodePhysicsBoulderDefault;
						string name = collidedMat.Name;
						if (name.Contains("stone"))
						{
							num2 = ItemPhysicsSoundContainer.SoundCodePhysicsBoulderStone;
						}
						else if (name.Contains("wood"))
						{
							num2 = ItemPhysicsSoundContainer.SoundCodePhysicsBoulderWood;
						}
						this._rollingSoundEvent = SoundEvent.CreateEvent(num2, Mission.Current.Scene);
						this._rollingSoundEvent.PlayInPosition(collisionPoint);
					}
					float num3 = impulse.Length * 0.033333335f;
					num3 = MBMath.ClampFloat(num3, 0f, 1f);
					this._rollingSoundEvent.SetParameter("Force", num3);
					this._rollingSoundEvent.SetPosition(collisionPoint);
				}
			}
		}

		// Token: 0x06003087 RID: 12423 RVA: 0x000C83E1 File Offset: 0x000C65E1
		public bool IsStuckMissile()
		{
			return this.SpawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.AsMissile);
		}

		// Token: 0x06003088 RID: 12424 RVA: 0x000C83EF File Offset: 0x000C65EF
		public bool IsQuiverAndNotEmpty()
		{
			return this._weapon.Item.PrimaryWeapon.IsConsumable && this._weapon.Amount > 0;
		}

		// Token: 0x06003089 RID: 12425 RVA: 0x000C8418 File Offset: 0x000C6618
		public bool IsBanner()
		{
			return this._weapon.IsBanner();
		}

		// Token: 0x0600308A RID: 12426 RVA: 0x000C8425 File Offset: 0x000C6625
		public override TextObject GetInfoTextForBeingNotInteractable(Agent userAgent)
		{
			if (!base.IsDeactivated && this._weapon.IsAnyConsumable() && this._weapon.Amount == 0)
			{
				return GameTexts.FindText("str_ui_empty_quiver", null);
			}
			return base.GetInfoTextForBeingNotInteractable(userAgent);
		}

		// Token: 0x0600308B RID: 12427 RVA: 0x000C845C File Offset: 0x000C665C
		public void StopPhysicsAndSetFrameForClient(MatrixFrame frame, GameEntity parent)
		{
			if (parent != null)
			{
				frame = parent.GetGlobalFrame().TransformToParent(frame);
			}
			frame.rotation.Orthonormalize();
			this._clientSyncData = new SpawnedItemEntity.ClientSyncData();
			this._clientSyncData.Frame = frame;
			this._clientSyncData.Timer = new Timer(Mission.Current.CurrentTime, 0.5f, false);
			this._clientSyncData.Parent = parent;
			if (!this.PhysicsStopped)
			{
				this.PhysicsStopped = true;
				GameEntity gameEntity = base.GameEntity;
				if (!this._weapon.IsEmpty && !gameEntity.BodyFlag.HasAnyFlag(BodyFlags.Disabled))
				{
					gameEntity.DisableDynamicBodySimulation();
					return;
				}
				gameEntity.RemovePhysics(false);
			}
		}

		// Token: 0x0600308C RID: 12428 RVA: 0x000C8511 File Offset: 0x000C6711
		public void ConsumeWeaponAmount(short consumedAmount)
		{
			this._weapon.Consume(consumedAmount);
		}

		// Token: 0x0600308D RID: 12429 RVA: 0x000C8520 File Offset: 0x000C6720
		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return string.Empty;
		}

		// Token: 0x0600308E RID: 12430 RVA: 0x000C8527 File Offset: 0x000C6727
		public void RequestDeletionOnNextTick()
		{
			this._deletionTimer = new Timer(Mission.Current.CurrentTime, -1f, true);
		}

		// Token: 0x0600308F RID: 12431 RVA: 0x000C8544 File Offset: 0x000C6744
		public SpawnedItemEntity()
			: base(false)
		{
		}

		// Token: 0x0400142A RID: 5162
		private static readonly ActionIndexCache act_pickup_down_begin = ActionIndexCache.Create("act_pickup_down_begin");

		// Token: 0x0400142B RID: 5163
		private static readonly ActionIndexCache act_pickup_down_end = ActionIndexCache.Create("act_pickup_down_end");

		// Token: 0x0400142C RID: 5164
		private static readonly ActionIndexCache act_pickup_down_begin_left_stance = ActionIndexCache.Create("act_pickup_down_begin_left_stance");

		// Token: 0x0400142D RID: 5165
		private static readonly ActionIndexCache act_pickup_down_end_left_stance = ActionIndexCache.Create("act_pickup_down_end_left_stance");

		// Token: 0x0400142E RID: 5166
		private static readonly ActionIndexCache act_pickup_down_left_begin = ActionIndexCache.Create("act_pickup_down_left_begin");

		// Token: 0x0400142F RID: 5167
		private static readonly ActionIndexCache act_pickup_down_left_end = ActionIndexCache.Create("act_pickup_down_left_end");

		// Token: 0x04001430 RID: 5168
		private static readonly ActionIndexCache act_pickup_down_left_begin_left_stance = ActionIndexCache.Create("act_pickup_down_left_begin_left_stance");

		// Token: 0x04001431 RID: 5169
		private static readonly ActionIndexCache act_pickup_down_left_end_left_stance = ActionIndexCache.Create("act_pickup_down_left_end_left_stance");

		// Token: 0x04001432 RID: 5170
		private static readonly ActionIndexCache act_pickup_middle_begin = ActionIndexCache.Create("act_pickup_middle_begin");

		// Token: 0x04001433 RID: 5171
		private static readonly ActionIndexCache act_pickup_middle_end = ActionIndexCache.Create("act_pickup_middle_end");

		// Token: 0x04001434 RID: 5172
		private static readonly ActionIndexCache act_pickup_middle_begin_left_stance = ActionIndexCache.Create("act_pickup_middle_begin_left_stance");

		// Token: 0x04001435 RID: 5173
		private static readonly ActionIndexCache act_pickup_middle_end_left_stance = ActionIndexCache.Create("act_pickup_middle_end_left_stance");

		// Token: 0x04001436 RID: 5174
		private static readonly ActionIndexCache act_pickup_middle_left_begin = ActionIndexCache.Create("act_pickup_middle_left_begin");

		// Token: 0x04001437 RID: 5175
		private static readonly ActionIndexCache act_pickup_middle_left_end = ActionIndexCache.Create("act_pickup_middle_left_end");

		// Token: 0x04001438 RID: 5176
		private static readonly ActionIndexCache act_pickup_middle_left_begin_left_stance = ActionIndexCache.Create("act_pickup_middle_left_begin_left_stance");

		// Token: 0x04001439 RID: 5177
		private static readonly ActionIndexCache act_pickup_middle_left_end_left_stance = ActionIndexCache.Create("act_pickup_middle_left_end_left_stance");

		// Token: 0x0400143A RID: 5178
		private static readonly ActionIndexCache act_pickup_up_begin = ActionIndexCache.Create("act_pickup_up_begin");

		// Token: 0x0400143B RID: 5179
		private static readonly ActionIndexCache act_pickup_up_end = ActionIndexCache.Create("act_pickup_up_end");

		// Token: 0x0400143C RID: 5180
		private static readonly ActionIndexCache act_pickup_up_begin_left_stance = ActionIndexCache.Create("act_pickup_up_begin_left_stance");

		// Token: 0x0400143D RID: 5181
		private static readonly ActionIndexCache act_pickup_up_end_left_stance = ActionIndexCache.Create("act_pickup_up_end_left_stance");

		// Token: 0x0400143E RID: 5182
		private static readonly ActionIndexCache act_pickup_up_left_begin = ActionIndexCache.Create("act_pickup_up_left_begin");

		// Token: 0x0400143F RID: 5183
		private static readonly ActionIndexCache act_pickup_up_left_end = ActionIndexCache.Create("act_pickup_up_left_end");

		// Token: 0x04001440 RID: 5184
		private static readonly ActionIndexCache act_pickup_up_left_begin_left_stance = ActionIndexCache.Create("act_pickup_up_left_begin_left_stance");

		// Token: 0x04001441 RID: 5185
		private static readonly ActionIndexCache act_pickup_up_left_end_left_stance = ActionIndexCache.Create("act_pickup_up_left_end_left_stance");

		// Token: 0x04001442 RID: 5186
		private static readonly ActionIndexCache act_pickup_from_right_down_horseback_begin = ActionIndexCache.Create("act_pickup_from_right_down_horseback_begin");

		// Token: 0x04001443 RID: 5187
		private static readonly ActionIndexCache act_pickup_from_right_down_horseback_end = ActionIndexCache.Create("act_pickup_from_right_down_horseback_end");

		// Token: 0x04001444 RID: 5188
		private static readonly ActionIndexCache act_pickup_from_right_down_horseback_left_begin = ActionIndexCache.Create("act_pickup_from_right_down_horseback_left_begin");

		// Token: 0x04001445 RID: 5189
		private static readonly ActionIndexCache act_pickup_from_right_down_horseback_left_end = ActionIndexCache.Create("act_pickup_from_right_down_horseback_left_end");

		// Token: 0x04001446 RID: 5190
		private static readonly ActionIndexCache act_pickup_from_right_middle_horseback_begin = ActionIndexCache.Create("act_pickup_from_right_middle_horseback_begin");

		// Token: 0x04001447 RID: 5191
		private static readonly ActionIndexCache act_pickup_from_right_middle_horseback_end = ActionIndexCache.Create("act_pickup_from_right_middle_horseback_end");

		// Token: 0x04001448 RID: 5192
		private static readonly ActionIndexCache act_pickup_from_right_middle_horseback_left_begin = ActionIndexCache.Create("act_pickup_from_right_middle_horseback_left_begin");

		// Token: 0x04001449 RID: 5193
		private static readonly ActionIndexCache act_pickup_from_right_middle_horseback_left_end = ActionIndexCache.Create("act_pickup_from_right_middle_horseback_left_end");

		// Token: 0x0400144A RID: 5194
		private static readonly ActionIndexCache act_pickup_from_right_up_horseback_begin = ActionIndexCache.Create("act_pickup_from_right_up_horseback_begin");

		// Token: 0x0400144B RID: 5195
		private static readonly ActionIndexCache act_pickup_from_right_up_horseback_end = ActionIndexCache.Create("act_pickup_from_right_up_horseback_end");

		// Token: 0x0400144C RID: 5196
		private static readonly ActionIndexCache act_pickup_from_right_up_horseback_left_begin = ActionIndexCache.Create("act_pickup_from_right_up_horseback_left_begin");

		// Token: 0x0400144D RID: 5197
		private static readonly ActionIndexCache act_pickup_from_right_up_horseback_left_end = ActionIndexCache.Create("act_pickup_from_right_up_horseback_left_end");

		// Token: 0x0400144E RID: 5198
		private static readonly ActionIndexCache act_pickup_from_left_down_horseback_begin = ActionIndexCache.Create("act_pickup_from_left_down_horseback_begin");

		// Token: 0x0400144F RID: 5199
		private static readonly ActionIndexCache act_pickup_from_left_down_horseback_end = ActionIndexCache.Create("act_pickup_from_left_down_horseback_end");

		// Token: 0x04001450 RID: 5200
		private static readonly ActionIndexCache act_pickup_from_left_down_horseback_left_begin = ActionIndexCache.Create("act_pickup_from_left_down_horseback_left_begin");

		// Token: 0x04001451 RID: 5201
		private static readonly ActionIndexCache act_pickup_from_left_down_horseback_left_end = ActionIndexCache.Create("act_pickup_from_left_down_horseback_left_end");

		// Token: 0x04001452 RID: 5202
		private static readonly ActionIndexCache act_pickup_from_left_middle_horseback_begin = ActionIndexCache.Create("act_pickup_from_left_middle_horseback_begin");

		// Token: 0x04001453 RID: 5203
		private static readonly ActionIndexCache act_pickup_from_left_middle_horseback_end = ActionIndexCache.Create("act_pickup_from_left_middle_horseback_end");

		// Token: 0x04001454 RID: 5204
		private static readonly ActionIndexCache act_pickup_from_left_middle_horseback_left_begin = ActionIndexCache.Create("act_pickup_from_left_middle_horseback_left_begin");

		// Token: 0x04001455 RID: 5205
		private static readonly ActionIndexCache act_pickup_from_left_middle_horseback_left_end = ActionIndexCache.Create("act_pickup_from_left_middle_horseback_left_end");

		// Token: 0x04001456 RID: 5206
		private static readonly ActionIndexCache act_pickup_from_left_up_horseback_begin = ActionIndexCache.Create("act_pickup_from_left_up_horseback_begin");

		// Token: 0x04001457 RID: 5207
		private static readonly ActionIndexCache act_pickup_from_left_up_horseback_end = ActionIndexCache.Create("act_pickup_from_left_up_horseback_end");

		// Token: 0x04001458 RID: 5208
		private static readonly ActionIndexCache act_pickup_from_left_up_horseback_left_begin = ActionIndexCache.Create("act_pickup_from_left_up_horseback_left_begin");

		// Token: 0x04001459 RID: 5209
		private static readonly ActionIndexCache act_pickup_from_left_up_horseback_left_end = ActionIndexCache.Create("act_pickup_from_left_up_horseback_left_end");

		// Token: 0x0400145A RID: 5210
		private static readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create("act_pickup_boulder_begin");

		// Token: 0x0400145B RID: 5211
		private static readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create("act_pickup_boulder_end");

		// Token: 0x0400145C RID: 5212
		private MissionWeapon _weapon;

		// Token: 0x0400145D RID: 5213
		private bool _hasLifeTime;

		// Token: 0x0400145E RID: 5214
		public string WeaponName = "";

		// Token: 0x0400145F RID: 5215
		private const float LongLifeTime = 180f;

		// Token: 0x04001460 RID: 5216
		private const float DisablePhysicsTime = 10f;

		// Token: 0x04001461 RID: 5217
		private const float QuickFadeoutLifeTime = 5f;

		// Token: 0x04001462 RID: 5218
		private const float TotalFadeOutInDuration = 0.5f;

		// Token: 0x04001463 RID: 5219
		private const float PreventStationaryCheckTime = 1f;

		// Token: 0x04001464 RID: 5220
		private Timer _disablePhysicsTimer;

		// Token: 0x04001465 RID: 5221
		private bool _physicsStopped;

		// Token: 0x04001466 RID: 5222
		private bool _readyToBeDeleted;

		// Token: 0x04001467 RID: 5223
		private Timer _deletionTimer;

		// Token: 0x04001468 RID: 5224
		private int _usedChannelIndex;

		// Token: 0x04001469 RID: 5225
		private ActionIndexCache _progressActionIndex;

		// Token: 0x0400146A RID: 5226
		private ActionIndexCache _successActionIndex;

		// Token: 0x0400146B RID: 5227
		private float _lastSoundPlayTime;

		// Token: 0x0400146C RID: 5228
		private const float MinSoundDelay = 0.333f;

		// Token: 0x0400146D RID: 5229
		private SoundEvent _rollingSoundEvent;

		// Token: 0x0400146E RID: 5230
		private SpawnedItemEntity.ClientSyncData _clientSyncData;

		// Token: 0x0400146F RID: 5231
		private GameEntity _ownerGameEntity;

		// Token: 0x04001470 RID: 5232
		private Vec3 _fakeSimulationVelocity;

		// Token: 0x04001471 RID: 5233
		private GameEntity _groundEntityWhenDisabled;

		// Token: 0x02000687 RID: 1671
		private class ClientSyncData
		{
			// Token: 0x04002147 RID: 8519
			public MatrixFrame Frame;

			// Token: 0x04002148 RID: 8520
			public GameEntity Parent;

			// Token: 0x04002149 RID: 8521
			public Timer Timer;
		}
	}
}
