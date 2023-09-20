using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200034C RID: 844
	public class DestructableComponent : SynchedMissionObject, IFocusable
	{
		// Token: 0x14000090 RID: 144
		// (add) Token: 0x06002D5C RID: 11612 RVA: 0x000B1988 File Offset: 0x000AFB88
		// (remove) Token: 0x06002D5D RID: 11613 RVA: 0x000B19C0 File Offset: 0x000AFBC0
		public event Action OnNextDestructionState;

		// Token: 0x14000091 RID: 145
		// (add) Token: 0x06002D5E RID: 11614 RVA: 0x000B19F8 File Offset: 0x000AFBF8
		// (remove) Token: 0x06002D5F RID: 11615 RVA: 0x000B1A30 File Offset: 0x000AFC30
		public event DestructableComponent.OnHitTakenAndDestroyedDelegate OnDestroyed;

		// Token: 0x14000092 RID: 146
		// (add) Token: 0x06002D60 RID: 11616 RVA: 0x000B1A68 File Offset: 0x000AFC68
		// (remove) Token: 0x06002D61 RID: 11617 RVA: 0x000B1AA0 File Offset: 0x000AFCA0
		public event DestructableComponent.OnHitTakenAndDestroyedDelegate OnHitTaken;

		// Token: 0x17000823 RID: 2083
		// (get) Token: 0x06002D62 RID: 11618 RVA: 0x000B1AD5 File Offset: 0x000AFCD5
		// (set) Token: 0x06002D63 RID: 11619 RVA: 0x000B1AE0 File Offset: 0x000AFCE0
		public float HitPoint
		{
			get
			{
				return this._hitPoint;
			}
			set
			{
				if (!this._hitPoint.Equals(value))
				{
					this._hitPoint = MathF.Max(value, 0f);
					if (GameNetwork.IsServerOrRecorder)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SyncObjectHitpoints(this, value));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
					}
				}
			}
		}

		// Token: 0x17000824 RID: 2084
		// (get) Token: 0x06002D64 RID: 11620 RVA: 0x000B1B2C File Offset: 0x000AFD2C
		public FocusableObjectType FocusableObjectType
		{
			get
			{
				return FocusableObjectType.None;
			}
		}

		// Token: 0x17000825 RID: 2085
		// (get) Token: 0x06002D65 RID: 11621 RVA: 0x000B1B2F File Offset: 0x000AFD2F
		public bool IsDestroyed
		{
			get
			{
				return this.HitPoint <= 0f;
			}
		}

		// Token: 0x17000826 RID: 2086
		// (get) Token: 0x06002D66 RID: 11622 RVA: 0x000B1B41 File Offset: 0x000AFD41
		// (set) Token: 0x06002D67 RID: 11623 RVA: 0x000B1B49 File Offset: 0x000AFD49
		public GameEntity CurrentState { get; private set; }

		// Token: 0x06002D68 RID: 11624 RVA: 0x000B1B54 File Offset: 0x000AFD54
		private DestructableComponent()
		{
		}

		// Token: 0x06002D69 RID: 11625 RVA: 0x000B1BAC File Offset: 0x000AFDAC
		protected internal override void OnInit()
		{
			base.OnInit();
			this._hitPoint = this.MaxHitPoint;
			this._referenceEntity = (string.IsNullOrEmpty(this.ReferenceEntityTag) ? base.GameEntity : base.GameEntity.GetChildren().FirstOrDefault((GameEntity x) => x.HasTag(this.ReferenceEntityTag)));
			if (!string.IsNullOrEmpty(this.DestructionStates))
			{
				this._destructionStates = this.DestructionStates.Replace(" ", string.Empty).Split(new char[] { ',' });
				bool flag = false;
				string[] destructionStates = this._destructionStates;
				for (int i = 0; i < destructionStates.Length; i++)
				{
					string item = destructionStates[i];
					if (!string.IsNullOrEmpty(item))
					{
						GameEntity gameEntity = base.GameEntity.GetChildren().FirstOrDefault((GameEntity x) => x.Name == item);
						if (gameEntity != null)
						{
							gameEntity.AddBodyFlags(BodyFlags.Moveable, true);
							PhysicsShape bodyShape = gameEntity.GetBodyShape();
							if (bodyShape != null)
							{
								PhysicsShape.AddPreloadQueueWithName(bodyShape.GetName(), gameEntity.GetGlobalScale());
								flag = true;
							}
						}
						else
						{
							GameEntity gameEntity2 = GameEntity.Instantiate(null, item, false);
							List<GameEntity> list = new List<GameEntity>();
							gameEntity2.GetChildrenRecursive(ref list);
							list.Add(gameEntity2);
							foreach (GameEntity gameEntity3 in list)
							{
								PhysicsShape bodyShape2 = gameEntity3.GetBodyShape();
								if (bodyShape2 != null)
								{
									Vec3 globalScale = gameEntity3.GetGlobalScale();
									Vec3 globalScale2 = this._referenceEntity.GetGlobalScale();
									Vec3 vec = new Vec3(globalScale.x * globalScale2.x, globalScale.y * globalScale2.y, globalScale.z * globalScale2.z, -1f);
									PhysicsShape.AddPreloadQueueWithName(bodyShape2.GetName(), vec);
									flag = true;
								}
							}
						}
					}
				}
				if (flag)
				{
					PhysicsShape.ProcessPreloadQueue();
				}
			}
			this._originalState = this.GetOriginalState(base.GameEntity);
			if (this._originalState == null)
			{
				this._originalState = base.GameEntity;
			}
			this.CurrentState = this._originalState;
			this._originalState.AddBodyFlags(BodyFlags.Moveable, true);
			List<GameEntity> list2 = new List<GameEntity>();
			base.GameEntity.GetChildrenRecursive(ref list2);
			foreach (GameEntity gameEntity4 in list2.Where((GameEntity child) => child.BodyFlag.HasAnyFlag(BodyFlags.Dynamic)))
			{
				gameEntity4.SetPhysicsState(false, true);
				gameEntity4.SetFrameChanged();
			}
			this._heavyHitParticles = base.GameEntity.CollectChildrenEntitiesWithTag(this.HeavyHitParticlesTag);
			base.GameEntity.SetAnimationSoundActivation(true);
		}

		// Token: 0x06002D6A RID: 11626 RVA: 0x000B1EAC File Offset: 0x000B00AC
		public GameEntity GetOriginalState(GameEntity parent)
		{
			int childCount = parent.ChildCount;
			for (int i = 0; i < childCount; i++)
			{
				GameEntity child = parent.GetChild(i);
				if (!child.HasScriptOfType<DestructableComponent>())
				{
					if (child.HasTag(this.OriginalStateTag))
					{
						return child;
					}
					GameEntity originalState = this.GetOriginalState(child);
					if (originalState != null)
					{
						return originalState;
					}
				}
			}
			return null;
		}

		// Token: 0x06002D6B RID: 11627 RVA: 0x000B1F00 File Offset: 0x000B0100
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._referenceEntity = (string.IsNullOrEmpty(this.ReferenceEntityTag) ? base.GameEntity : base.GameEntity.GetChildren().FirstOrDefault((GameEntity x) => x.HasTag(this.ReferenceEntityTag)));
		}

		// Token: 0x06002D6C RID: 11628 RVA: 0x000B1F40 File Offset: 0x000B0140
		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName.Equals(this.ReferenceEntityTag))
			{
				this._referenceEntity = (string.IsNullOrEmpty(this.ReferenceEntityTag) ? base.GameEntity : base.GameEntity.GetChildren().SingleOrDefault((GameEntity x) => x.HasTag(this.ReferenceEntityTag)));
			}
		}

		// Token: 0x06002D6D RID: 11629 RVA: 0x000B1F99 File Offset: 0x000B0199
		protected internal override void OnMissionReset()
		{
			base.OnMissionReset();
			this.Reset();
		}

		// Token: 0x06002D6E RID: 11630 RVA: 0x000B1FA7 File Offset: 0x000B01A7
		public void Reset()
		{
			this.RestoreEntity();
			this._hitPoint = this.MaxHitPoint;
			this._currentStateIndex = 0;
		}

		// Token: 0x06002D6F RID: 11631 RVA: 0x000B1FC4 File Offset: 0x000B01C4
		private void RestoreEntity()
		{
			if (this._destructionStates != null)
			{
				int j;
				int i;
				for (i = 0; i < this._destructionStates.Length; i = j + 1)
				{
					GameEntity gameEntity = base.GameEntity.GetChildren().FirstOrDefault((GameEntity x) => x.Name == this._destructionStates[i].ToString());
					if (gameEntity != null)
					{
						Skeleton skeleton = gameEntity.Skeleton;
						if (skeleton != null)
						{
							skeleton.SetAnimationAtChannel(-1, 0, 1f, -1f, 0f);
						}
					}
					j = i;
				}
			}
			if (this.CurrentState != this._originalState)
			{
				this.CurrentState.SetVisibilityExcludeParents(false);
				this.CurrentState = this._originalState;
			}
			this.CurrentState.SetVisibilityExcludeParents(true);
			this.CurrentState.SetPhysicsState(true, true);
			this.CurrentState.SetFrameChanged();
		}

		// Token: 0x06002D70 RID: 11632 RVA: 0x000B20A4 File Offset: 0x000B02A4
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			if (this._referenceEntity != null && this._referenceEntity != base.GameEntity && MBEditor.IsEntitySelected(this._referenceEntity))
			{
				new Vec3(-2f, -0.5f, -1f, -1f);
				new Vec3(2f, 0.5f, 1f, -1f);
				MatrixFrame identity = MatrixFrame.Identity;
				GameEntity gameEntity = this._referenceEntity;
				while (gameEntity.Parent != null)
				{
					gameEntity = gameEntity.Parent;
				}
				gameEntity.GetMeshBendedFrame(this._referenceEntity.GetGlobalFrame(), ref identity);
			}
		}

		// Token: 0x06002D71 RID: 11633 RVA: 0x000B2154 File Offset: 0x000B0354
		public void TriggerOnHit(Agent attackerAgent, int inflictedDamage, Vec3 impactPosition, Vec3 impactDirection, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior)
		{
			bool flag;
			this.OnHit(attackerAgent, inflictedDamage, impactPosition, impactDirection, weapon, attackerScriptComponentBehavior, out flag);
		}

		// Token: 0x06002D72 RID: 11634 RVA: 0x000B2174 File Offset: 0x000B0374
		protected internal override bool OnHit(Agent attackerAgent, int inflictedDamage, Vec3 impactPosition, Vec3 impactDirection, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, out bool reportDamage)
		{
			reportDamage = false;
			if (base.IsDisabled)
			{
				return true;
			}
			MissionWeapon missionWeapon = weapon;
			if (missionWeapon.IsEmpty && !(attackerScriptComponentBehavior is BatteringRam))
			{
				inflictedDamage = 0;
			}
			else if (this.DestroyedByStoneOnly)
			{
				missionWeapon = weapon;
				WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
				if ((currentUsageItem.WeaponClass != WeaponClass.Stone && currentUsageItem.WeaponClass != WeaponClass.Boulder) || !currentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.NotUsableWithOneHand))
				{
					inflictedDamage = 0;
				}
			}
			bool isDestroyed = this.IsDestroyed;
			if (this.DestroyOnAnyHit)
			{
				inflictedDamage = (int)(this.MaxHitPoint + 1f);
			}
			if (inflictedDamage > 0 && !isDestroyed)
			{
				this.HitPoint -= (float)inflictedDamage;
				if ((float)inflictedDamage > this.HeavyHitParticlesThreshold)
				{
					this.BurstHeavyHitParticles();
				}
				int num = this.CalculateNextDestructionLevel(inflictedDamage);
				if (!this.IsDestroyed)
				{
					DestructableComponent.OnHitTakenAndDestroyedDelegate onHitTaken = this.OnHitTaken;
					if (onHitTaken != null)
					{
						onHitTaken(this, attackerAgent, weapon, attackerScriptComponentBehavior, inflictedDamage);
					}
				}
				else if (this.IsDestroyed && !isDestroyed)
				{
					Mission.Current.OnObjectDisabled(this);
					DestructableComponent.OnHitTakenAndDestroyedDelegate onHitTaken2 = this.OnHitTaken;
					if (onHitTaken2 != null)
					{
						onHitTaken2(this, attackerAgent, weapon, attackerScriptComponentBehavior, inflictedDamage);
					}
					DestructableComponent.OnHitTakenAndDestroyedDelegate onDestroyed = this.OnDestroyed;
					if (onDestroyed != null)
					{
						onDestroyed(this, attackerAgent, weapon, attackerScriptComponentBehavior, inflictedDamage);
					}
					MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
					globalFrame.origin += globalFrame.rotation.u * this.SoundAndParticleEffectHeightOffset + globalFrame.rotation.f * this.SoundAndParticleEffectForwardOffset;
					globalFrame.rotation.Orthonormalize();
					if (this.ParticleEffectOnDestroy != "")
					{
						Mission.Current.Scene.CreateBurstParticle(ParticleSystemManager.GetRuntimeIdByName(this.ParticleEffectOnDestroy), globalFrame);
					}
					if (this.SoundEffectOnDestroy != "")
					{
						Mission.Current.MakeSound(SoundEvent.GetEventIdFromString(this.SoundEffectOnDestroy), globalFrame.origin, false, true, (attackerAgent != null) ? attackerAgent.Index : (-1), -1);
					}
				}
				this.SetDestructionLevel(num, -1, (float)inflictedDamage, impactPosition, impactDirection, false);
				reportDamage = true;
			}
			return !this.PassHitOnToParent;
		}

		// Token: 0x06002D73 RID: 11635 RVA: 0x000B239C File Offset: 0x000B059C
		public void BurstHeavyHitParticles()
		{
			foreach (GameEntity gameEntity in this._heavyHitParticles)
			{
				gameEntity.BurstEntityParticle(false);
			}
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new BurstAllHeavyHitParticles(this));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		// Token: 0x06002D74 RID: 11636 RVA: 0x000B2408 File Offset: 0x000B0608
		private int CalculateNextDestructionLevel(int inflictedDamage)
		{
			if (this.HasDestructionState)
			{
				int num = this._destructionStates.Length;
				float num2 = this.MaxHitPoint / (float)num;
				float num3 = this.MaxHitPoint;
				int num4 = 0;
				while (num3 - num2 >= this.HitPoint)
				{
					num3 -= num2;
					num4++;
				}
				Func<int, int, int, int> onCalculateDestructionStateIndex = this.OnCalculateDestructionStateIndex;
				return (onCalculateDestructionStateIndex != null) ? onCalculateDestructionStateIndex(num4, inflictedDamage, this.DestructionStates.Length) : num4;
			}
			if (this.IsDestroyed)
			{
				return this._currentStateIndex + 1;
			}
			return this._currentStateIndex;
		}

		// Token: 0x06002D75 RID: 11637 RVA: 0x000B2488 File Offset: 0x000B0688
		public void SetDestructionLevel(int state, int forcedId, float blowMagnitude, Vec3 blowPosition, Vec3 blowDirection, bool noEffects = false)
		{
			if (this._currentStateIndex != state)
			{
				float num = MBMath.ClampFloat(blowMagnitude, 1f, DestructableComponent.MaxBlowMagnitude);
				this._currentStateIndex = state;
				this.ReplaceEntityWithBrokenEntity(forcedId);
				if (this.CurrentState != null)
				{
					foreach (GameEntity gameEntity in from child in this.CurrentState.GetChildren()
						where child.BodyFlag.HasAnyFlag(BodyFlags.Dynamic)
						select child)
					{
						gameEntity.SetPhysicsState(true, true);
						gameEntity.SetFrameChanged();
					}
					if (!GameNetwork.IsDedicatedServer && !noEffects)
					{
						this.CurrentState.BurstEntityParticle(true);
						this.ApplyPhysics(num, blowPosition, blowDirection);
					}
					Action onNextDestructionState = this.OnNextDestructionState;
					if (onNextDestructionState != null)
					{
						onNextDestructionState();
					}
				}
				if (GameNetwork.IsServerOrRecorder)
				{
					if (this.CurrentState != null)
					{
						MissionObject firstScriptOfType = this.CurrentState.GetFirstScriptOfType<MissionObject>();
						if (firstScriptOfType != null)
						{
							forcedId = firstScriptOfType.Id.Id;
						}
					}
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SyncObjectDestructionLevel(this, state, forcedId, num, blowPosition, blowDirection));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
		}

		// Token: 0x06002D76 RID: 11638 RVA: 0x000B25C4 File Offset: 0x000B07C4
		private void ApplyPhysics(float blowMagnitude, Vec3 blowPosition, Vec3 blowDirection)
		{
			if (this.CurrentState != null)
			{
				IEnumerable<GameEntity> enumerable = from child in this.CurrentState.GetChildren()
					where child.HasBody() && child.BodyFlag.HasAnyFlag(BodyFlags.Dynamic) && !child.HasScriptOfType<SpawnedItemEntity>()
					select child;
				int num = enumerable.Count<GameEntity>();
				float num2 = ((num > 1) ? (blowMagnitude / (float)num) : blowMagnitude);
				foreach (GameEntity gameEntity in enumerable)
				{
					gameEntity.ApplyLocalImpulseToDynamicBody(Vec3.Zero, blowDirection * num2);
					Mission.Current.AddTimerToDynamicEntity(gameEntity, 10f + MBRandom.RandomFloat * 2f);
				}
			}
		}

		// Token: 0x06002D77 RID: 11639 RVA: 0x000B2688 File Offset: 0x000B0888
		private void ReplaceEntityWithBrokenEntity(int forcedId)
		{
			this._previousState = this.CurrentState;
			this._previousState.SetVisibilityExcludeParents(false);
			if (this.HasDestructionState)
			{
				bool flag;
				this.CurrentState = this.AddBrokenEntity(this._destructionStates[this._currentStateIndex - 1], out flag);
				if (flag)
				{
					if (this._originalState != base.GameEntity)
					{
						base.GameEntity.AddChild(this.CurrentState, true);
					}
					if (forcedId != -1)
					{
						MissionObject firstScriptOfType = this.CurrentState.GetFirstScriptOfType<MissionObject>();
						if (firstScriptOfType != null)
						{
							firstScriptOfType.Id = new MissionObjectId(forcedId, true);
							using (IEnumerator<GameEntity> enumerator = this.CurrentState.GetChildren().GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									GameEntity gameEntity = enumerator.Current;
									MissionObject firstScriptOfType2 = gameEntity.GetFirstScriptOfType<MissionObject>();
									if (firstScriptOfType2 != null && firstScriptOfType2.Id.CreatedAtRuntime)
									{
										firstScriptOfType2.Id = new MissionObjectId(++forcedId, true);
									}
								}
								return;
							}
						}
						MBDebug.ShowWarning("Current destruction state doesn't have mission object script component.");
						return;
					}
				}
				else
				{
					MatrixFrame frame = this._previousState.GetFrame();
					this.CurrentState.SetFrame(ref frame);
				}
			}
		}

		// Token: 0x06002D78 RID: 11640 RVA: 0x000B27B0 File Offset: 0x000B09B0
		protected internal override bool MovesEntity()
		{
			return true;
		}

		// Token: 0x06002D79 RID: 11641 RVA: 0x000B27B3 File Offset: 0x000B09B3
		public void PreDestroy()
		{
			DestructableComponent.OnHitTakenAndDestroyedDelegate onDestroyed = this.OnDestroyed;
			if (onDestroyed != null)
			{
				onDestroyed(this, null, MissionWeapon.Invalid, null, 0);
			}
			this.SetVisibleSynched(false, true);
		}

		// Token: 0x06002D7A RID: 11642 RVA: 0x000B27D8 File Offset: 0x000B09D8
		private GameEntity AddBrokenEntity(string prefab, out bool newCreated)
		{
			if (!string.IsNullOrEmpty(prefab))
			{
				GameEntity gameEntity = base.GameEntity.GetChildren().FirstOrDefault((GameEntity x) => x.Name == prefab);
				if (gameEntity != null)
				{
					gameEntity.SetVisibilityExcludeParents(true);
					if (!GameNetwork.IsClientOrReplay)
					{
						MissionObject missionObject = gameEntity.GetScriptComponents<MissionObject>().FirstOrDefault<MissionObject>();
						if (missionObject != null)
						{
							missionObject.SetAbilityOfFaces(true);
						}
					}
					newCreated = false;
				}
				else
				{
					gameEntity = GameEntity.Instantiate(Mission.Current.Scene, prefab, this._referenceEntity.GetGlobalFrame());
					if (gameEntity != null)
					{
						gameEntity.SetMobility(GameEntity.Mobility.stationary);
					}
					if (base.GameEntity.Parent != null)
					{
						base.GameEntity.Parent.AddChild(gameEntity, true);
					}
					newCreated = true;
				}
				if (this._referenceEntity.Skeleton != null && gameEntity.Skeleton != null)
				{
					Skeleton skeleton = ((this.CurrentState != this._originalState) ? this.CurrentState : this._referenceEntity).Skeleton;
					int animationIndexAtChannel = skeleton.GetAnimationIndexAtChannel(0);
					float animationParameterAtChannel = skeleton.GetAnimationParameterAtChannel(0);
					if (animationIndexAtChannel != -1)
					{
						gameEntity.Skeleton.SetAnimationAtChannel(animationIndexAtChannel, 0, 1f, -1f, animationParameterAtChannel);
						gameEntity.ResumeSkeletonAnimation();
					}
				}
				return gameEntity;
			}
			newCreated = false;
			return null;
		}

		// Token: 0x06002D7B RID: 11643 RVA: 0x000B292C File Offset: 0x000B0B2C
		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			GameNetworkMessage.WriteFloatToPacket(MathF.Max(this.HitPoint, 0f), CompressionMission.UsableGameObjectHealthCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this._currentStateIndex, CompressionMission.UsableGameObjectDestructionStateCompressionInfo);
			if (this._currentStateIndex != 0)
			{
				MissionObject firstScriptOfType = this.CurrentState.GetFirstScriptOfType<MissionObject>();
				GameNetworkMessage.WriteBoolToPacket(firstScriptOfType != null);
				if (firstScriptOfType != null)
				{
					GameNetworkMessage.WriteMissionObjectIdToPacket(firstScriptOfType.Id);
				}
			}
		}

		// Token: 0x06002D7C RID: 11644 RVA: 0x000B2994 File Offset: 0x000B0B94
		public override bool ReadFromNetwork()
		{
			bool flag = true;
			flag = flag && base.ReadFromNetwork();
			float num = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.UsableGameObjectHealthCompressionInfo, ref flag);
			int num2 = GameNetworkMessage.ReadIntFromPacket(CompressionMission.UsableGameObjectDestructionStateCompressionInfo, ref flag);
			if (flag)
			{
				int num3 = -1;
				if (num2 != 0 && GameNetworkMessage.ReadBoolFromPacket(ref flag))
				{
					num3 = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag).Id;
				}
				if (flag)
				{
					this.HitPoint = num;
					if (num2 != 0)
					{
						if (this.IsDestroyed)
						{
							DestructableComponent.OnHitTakenAndDestroyedDelegate onDestroyed = this.OnDestroyed;
							if (onDestroyed != null)
							{
								onDestroyed(this, null, MissionWeapon.Invalid, null, 0);
							}
						}
						this.SetDestructionLevel(num2, num3, 0f, Vec3.Zero, Vec3.Zero, true);
					}
				}
			}
			return flag;
		}

		// Token: 0x17000827 RID: 2087
		// (get) Token: 0x06002D7D RID: 11645 RVA: 0x000B2A30 File Offset: 0x000B0C30
		private bool HasDestructionState
		{
			get
			{
				return this._destructionStates != null && !this._destructionStates.IsEmpty<string>();
			}
		}

		// Token: 0x06002D7E RID: 11646 RVA: 0x000B2A4A File Offset: 0x000B0C4A
		public override void AddStuckMissile(GameEntity missileEntity)
		{
			if (this.CurrentState != null)
			{
				this.CurrentState.AddChild(missileEntity, false);
				return;
			}
			base.GameEntity.AddChild(missileEntity, false);
		}

		// Token: 0x06002D7F RID: 11647 RVA: 0x000B2A78 File Offset: 0x000B0C78
		protected internal override bool OnCheckForProblems()
		{
			bool flag = base.OnCheckForProblems();
			if ((string.IsNullOrEmpty(this.ReferenceEntityTag) ? base.GameEntity : base.GameEntity.GetChildren().FirstOrDefault((GameEntity x) => x.HasTag(this.ReferenceEntityTag))) == null)
			{
				MBEditor.AddEntityWarning(base.GameEntity, "Reference entity must be assigned. Root entity is " + base.GameEntity.Root.Name + ", child is " + base.GameEntity.Name);
				flag = true;
			}
			string[] array = this.DestructionStates.Replace(" ", string.Empty).Split(new char[] { ',' });
			for (int i = 0; i < array.Length; i++)
			{
				string destructionState = array[i];
				if (!string.IsNullOrEmpty(destructionState) && !(base.GameEntity.GetChildren().FirstOrDefault((GameEntity x) => x.Name == destructionState) != null) && GameEntity.Instantiate(null, destructionState, false) == null)
				{
					MBEditor.AddEntityWarning(base.GameEntity, "Destruction state '" + destructionState + "' is not valid.");
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06002D80 RID: 11648 RVA: 0x000B2BAB File Offset: 0x000B0DAB
		public void OnFocusGain(Agent userAgent)
		{
		}

		// Token: 0x06002D81 RID: 11649 RVA: 0x000B2BAD File Offset: 0x000B0DAD
		public void OnFocusLose(Agent userAgent)
		{
		}

		// Token: 0x06002D82 RID: 11650 RVA: 0x000B2BAF File Offset: 0x000B0DAF
		public TextObject GetInfoTextForBeingNotInteractable(Agent userAgent)
		{
			return new TextObject("", null);
		}

		// Token: 0x06002D83 RID: 11651 RVA: 0x000B2BBC File Offset: 0x000B0DBC
		public string GetDescriptionText(GameEntity gameEntity = null)
		{
			int num;
			if (int.TryParse(gameEntity.Name.Split(new char[] { '_' }).Last<string>(), out num))
			{
				string text = gameEntity.Name;
				text = text.Remove(text.Length - num.ToString().Length);
				text += "x";
				TextObject textObject;
				if (GameTexts.TryGetText("str_destructible_component", out textObject, text))
				{
					return textObject.ToString();
				}
				return "";
			}
			else
			{
				TextObject textObject2;
				if (GameTexts.TryGetText("str_destructible_component", out textObject2, gameEntity.Name))
				{
					return textObject2.ToString();
				}
				return "";
			}
		}

		// Token: 0x040011BB RID: 4539
		public const string CleanStateTag = "operational";

		// Token: 0x040011BC RID: 4540
		public static float MaxBlowMagnitude = 20f;

		// Token: 0x040011BD RID: 4541
		public string DestructionStates;

		// Token: 0x040011BE RID: 4542
		public bool DestroyedByStoneOnly;

		// Token: 0x040011BF RID: 4543
		public bool CanBeDestroyedInitially = true;

		// Token: 0x040011C0 RID: 4544
		public float MaxHitPoint = 100f;

		// Token: 0x040011C1 RID: 4545
		public bool DestroyOnAnyHit;

		// Token: 0x040011C2 RID: 4546
		public bool PassHitOnToParent;

		// Token: 0x040011C3 RID: 4547
		public string ReferenceEntityTag;

		// Token: 0x040011C4 RID: 4548
		public string HeavyHitParticlesTag;

		// Token: 0x040011C5 RID: 4549
		public float HeavyHitParticlesThreshold = 5f;

		// Token: 0x040011C6 RID: 4550
		public string ParticleEffectOnDestroy = "";

		// Token: 0x040011C7 RID: 4551
		public string SoundEffectOnDestroy = "";

		// Token: 0x040011C8 RID: 4552
		public float SoundAndParticleEffectHeightOffset;

		// Token: 0x040011C9 RID: 4553
		public float SoundAndParticleEffectForwardOffset;

		// Token: 0x040011CD RID: 4557
		public BattleSideEnum BattleSide = BattleSideEnum.None;

		// Token: 0x040011CE RID: 4558
		[EditableScriptComponentVariable(false)]
		public Func<int, int, int, int> OnCalculateDestructionStateIndex;

		// Token: 0x040011CF RID: 4559
		private float _hitPoint;

		// Token: 0x040011D0 RID: 4560
		private string OriginalStateTag = "operational";

		// Token: 0x040011D1 RID: 4561
		private GameEntity _referenceEntity;

		// Token: 0x040011D2 RID: 4562
		private GameEntity _previousState;

		// Token: 0x040011D3 RID: 4563
		private GameEntity _originalState;

		// Token: 0x040011D5 RID: 4565
		private string[] _destructionStates;

		// Token: 0x040011D6 RID: 4566
		private int _currentStateIndex;

		// Token: 0x040011D7 RID: 4567
		private IEnumerable<GameEntity> _heavyHitParticles;

		// Token: 0x0200065B RID: 1627
		// (Invoke) Token: 0x06003E6A RID: 15978
		public delegate void OnHitTakenAndDestroyedDelegate(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage);
	}
}
