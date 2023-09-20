using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class SpawnedItemEntity : UsableMissionObject
	{
		public MissionWeapon WeaponCopy
		{
			get
			{
				return this._weapon;
			}
		}

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

		public bool IsRemoved
		{
			get
			{
				return this._ownerGameEntity == null;
			}
		}

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

		public TextObject GetDescriptionMessage(bool fillUp)
		{
			if (!fillUp)
			{
				return this._weapon.GetModifiedItemName();
			}
			return GameTexts.FindText("str_inventory_weapon", ((int)this._weapon.CurrentUsageItem.WeaponClass).ToString());
		}

		protected internal override bool LockUserFrames
		{
			get
			{
				return false;
			}
		}

		public Mission.WeaponSpawnFlags SpawnFlags { get; private set; }

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

		protected internal override void OnTickOccasionally(float currentFrameDeltaTime)
		{
			this.OnTickParallel2(currentFrameDeltaTime);
		}

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

		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			if (base.CreatedAtRuntime)
			{
				Mission.Current.AddSpawnedItemEntityCreatedAtRuntime(this);
			}
		}

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

		public void AttachWeaponToWeapon(MissionWeapon attachedWeapon, ref MatrixFrame attachLocalFrame)
		{
			this._weapon.AttachWeapon(attachedWeapon, ref attachLocalFrame);
		}

		public bool IsReadyToBeDeleted()
		{
			return (!base.HasUser && this._readyToBeDeleted) || (this._groundEntityWhenDisabled != null && !this._groundEntityWhenDisabled.HasScene()) || (this._groundEntityWhenDisabled != null && !this._groundEntityWhenDisabled.IsVisibleIncludeParents() && (!this._groundEntityWhenDisabled.HasBody() || this._groundEntityWhenDisabled.BodyFlag.HasAnyFlag(BodyFlags.Disabled)));
		}

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

		public override bool IsDisabledForAgent(Agent agent)
		{
			return (this._weapon.IsAnyConsumable() && this._weapon.Amount == 0) || (this._weapon.IsBanner() && !MissionGameModels.Current.BattleBannerBearersModel.IsInteractableFormationBanner(this, agent));
		}

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

		public bool IsStuckMissile()
		{
			return this.SpawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.AsMissile);
		}

		public bool IsQuiverAndNotEmpty()
		{
			return this._weapon.Item.PrimaryWeapon.IsConsumable && this._weapon.Amount > 0;
		}

		public bool IsBanner()
		{
			return this._weapon.IsBanner();
		}

		public override TextObject GetInfoTextForBeingNotInteractable(Agent userAgent)
		{
			if (!base.IsDeactivated && this._weapon.IsAnyConsumable() && this._weapon.Amount == 0)
			{
				return GameTexts.FindText("str_ui_empty_quiver", null);
			}
			return base.GetInfoTextForBeingNotInteractable(userAgent);
		}

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

		public void ConsumeWeaponAmount(short consumedAmount)
		{
			this._weapon.Consume(consumedAmount);
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return string.Empty;
		}

		public void RequestDeletionOnNextTick()
		{
			this._deletionTimer = new Timer(Mission.Current.CurrentTime, -1f, true);
		}

		public SpawnedItemEntity()
			: base(false)
		{
		}

		private static readonly ActionIndexCache act_pickup_down_begin = ActionIndexCache.Create("act_pickup_down_begin");

		private static readonly ActionIndexCache act_pickup_down_end = ActionIndexCache.Create("act_pickup_down_end");

		private static readonly ActionIndexCache act_pickup_down_begin_left_stance = ActionIndexCache.Create("act_pickup_down_begin_left_stance");

		private static readonly ActionIndexCache act_pickup_down_end_left_stance = ActionIndexCache.Create("act_pickup_down_end_left_stance");

		private static readonly ActionIndexCache act_pickup_down_left_begin = ActionIndexCache.Create("act_pickup_down_left_begin");

		private static readonly ActionIndexCache act_pickup_down_left_end = ActionIndexCache.Create("act_pickup_down_left_end");

		private static readonly ActionIndexCache act_pickup_down_left_begin_left_stance = ActionIndexCache.Create("act_pickup_down_left_begin_left_stance");

		private static readonly ActionIndexCache act_pickup_down_left_end_left_stance = ActionIndexCache.Create("act_pickup_down_left_end_left_stance");

		private static readonly ActionIndexCache act_pickup_middle_begin = ActionIndexCache.Create("act_pickup_middle_begin");

		private static readonly ActionIndexCache act_pickup_middle_end = ActionIndexCache.Create("act_pickup_middle_end");

		private static readonly ActionIndexCache act_pickup_middle_begin_left_stance = ActionIndexCache.Create("act_pickup_middle_begin_left_stance");

		private static readonly ActionIndexCache act_pickup_middle_end_left_stance = ActionIndexCache.Create("act_pickup_middle_end_left_stance");

		private static readonly ActionIndexCache act_pickup_middle_left_begin = ActionIndexCache.Create("act_pickup_middle_left_begin");

		private static readonly ActionIndexCache act_pickup_middle_left_end = ActionIndexCache.Create("act_pickup_middle_left_end");

		private static readonly ActionIndexCache act_pickup_middle_left_begin_left_stance = ActionIndexCache.Create("act_pickup_middle_left_begin_left_stance");

		private static readonly ActionIndexCache act_pickup_middle_left_end_left_stance = ActionIndexCache.Create("act_pickup_middle_left_end_left_stance");

		private static readonly ActionIndexCache act_pickup_up_begin = ActionIndexCache.Create("act_pickup_up_begin");

		private static readonly ActionIndexCache act_pickup_up_end = ActionIndexCache.Create("act_pickup_up_end");

		private static readonly ActionIndexCache act_pickup_up_begin_left_stance = ActionIndexCache.Create("act_pickup_up_begin_left_stance");

		private static readonly ActionIndexCache act_pickup_up_end_left_stance = ActionIndexCache.Create("act_pickup_up_end_left_stance");

		private static readonly ActionIndexCache act_pickup_up_left_begin = ActionIndexCache.Create("act_pickup_up_left_begin");

		private static readonly ActionIndexCache act_pickup_up_left_end = ActionIndexCache.Create("act_pickup_up_left_end");

		private static readonly ActionIndexCache act_pickup_up_left_begin_left_stance = ActionIndexCache.Create("act_pickup_up_left_begin_left_stance");

		private static readonly ActionIndexCache act_pickup_up_left_end_left_stance = ActionIndexCache.Create("act_pickup_up_left_end_left_stance");

		private static readonly ActionIndexCache act_pickup_from_right_down_horseback_begin = ActionIndexCache.Create("act_pickup_from_right_down_horseback_begin");

		private static readonly ActionIndexCache act_pickup_from_right_down_horseback_end = ActionIndexCache.Create("act_pickup_from_right_down_horseback_end");

		private static readonly ActionIndexCache act_pickup_from_right_down_horseback_left_begin = ActionIndexCache.Create("act_pickup_from_right_down_horseback_left_begin");

		private static readonly ActionIndexCache act_pickup_from_right_down_horseback_left_end = ActionIndexCache.Create("act_pickup_from_right_down_horseback_left_end");

		private static readonly ActionIndexCache act_pickup_from_right_middle_horseback_begin = ActionIndexCache.Create("act_pickup_from_right_middle_horseback_begin");

		private static readonly ActionIndexCache act_pickup_from_right_middle_horseback_end = ActionIndexCache.Create("act_pickup_from_right_middle_horseback_end");

		private static readonly ActionIndexCache act_pickup_from_right_middle_horseback_left_begin = ActionIndexCache.Create("act_pickup_from_right_middle_horseback_left_begin");

		private static readonly ActionIndexCache act_pickup_from_right_middle_horseback_left_end = ActionIndexCache.Create("act_pickup_from_right_middle_horseback_left_end");

		private static readonly ActionIndexCache act_pickup_from_right_up_horseback_begin = ActionIndexCache.Create("act_pickup_from_right_up_horseback_begin");

		private static readonly ActionIndexCache act_pickup_from_right_up_horseback_end = ActionIndexCache.Create("act_pickup_from_right_up_horseback_end");

		private static readonly ActionIndexCache act_pickup_from_right_up_horseback_left_begin = ActionIndexCache.Create("act_pickup_from_right_up_horseback_left_begin");

		private static readonly ActionIndexCache act_pickup_from_right_up_horseback_left_end = ActionIndexCache.Create("act_pickup_from_right_up_horseback_left_end");

		private static readonly ActionIndexCache act_pickup_from_left_down_horseback_begin = ActionIndexCache.Create("act_pickup_from_left_down_horseback_begin");

		private static readonly ActionIndexCache act_pickup_from_left_down_horseback_end = ActionIndexCache.Create("act_pickup_from_left_down_horseback_end");

		private static readonly ActionIndexCache act_pickup_from_left_down_horseback_left_begin = ActionIndexCache.Create("act_pickup_from_left_down_horseback_left_begin");

		private static readonly ActionIndexCache act_pickup_from_left_down_horseback_left_end = ActionIndexCache.Create("act_pickup_from_left_down_horseback_left_end");

		private static readonly ActionIndexCache act_pickup_from_left_middle_horseback_begin = ActionIndexCache.Create("act_pickup_from_left_middle_horseback_begin");

		private static readonly ActionIndexCache act_pickup_from_left_middle_horseback_end = ActionIndexCache.Create("act_pickup_from_left_middle_horseback_end");

		private static readonly ActionIndexCache act_pickup_from_left_middle_horseback_left_begin = ActionIndexCache.Create("act_pickup_from_left_middle_horseback_left_begin");

		private static readonly ActionIndexCache act_pickup_from_left_middle_horseback_left_end = ActionIndexCache.Create("act_pickup_from_left_middle_horseback_left_end");

		private static readonly ActionIndexCache act_pickup_from_left_up_horseback_begin = ActionIndexCache.Create("act_pickup_from_left_up_horseback_begin");

		private static readonly ActionIndexCache act_pickup_from_left_up_horseback_end = ActionIndexCache.Create("act_pickup_from_left_up_horseback_end");

		private static readonly ActionIndexCache act_pickup_from_left_up_horseback_left_begin = ActionIndexCache.Create("act_pickup_from_left_up_horseback_left_begin");

		private static readonly ActionIndexCache act_pickup_from_left_up_horseback_left_end = ActionIndexCache.Create("act_pickup_from_left_up_horseback_left_end");

		private static readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create("act_pickup_boulder_begin");

		private static readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create("act_pickup_boulder_end");

		private MissionWeapon _weapon;

		private bool _hasLifeTime;

		public string WeaponName = "";

		private const float LongLifeTime = 180f;

		private const float DisablePhysicsTime = 10f;

		private const float QuickFadeoutLifeTime = 5f;

		private const float TotalFadeOutInDuration = 0.5f;

		private const float PreventStationaryCheckTime = 1f;

		private Timer _disablePhysicsTimer;

		private bool _physicsStopped;

		private bool _readyToBeDeleted;

		private Timer _deletionTimer;

		private int _usedChannelIndex;

		private ActionIndexCache _progressActionIndex;

		private ActionIndexCache _successActionIndex;

		private float _lastSoundPlayTime;

		private const float MinSoundDelay = 0.333f;

		private SoundEvent _rollingSoundEvent;

		private SpawnedItemEntity.ClientSyncData _clientSyncData;

		private GameEntity _ownerGameEntity;

		private Vec3 _fakeSimulationVelocity;

		private GameEntity _groundEntityWhenDisabled;

		private class ClientSyncData
		{
			public MatrixFrame Frame;

			public GameEntity Parent;

			public Timer Timer;
		}
	}
}
