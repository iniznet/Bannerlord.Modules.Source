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
	public class DestructableComponent : SynchedMissionObject, IFocusable
	{
		public event Action OnNextDestructionState;

		public event DestructableComponent.OnHitTakenAndDestroyedDelegate OnDestroyed;

		public event DestructableComponent.OnHitTakenAndDestroyedDelegate OnHitTaken;

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

		public FocusableObjectType FocusableObjectType
		{
			get
			{
				return FocusableObjectType.None;
			}
		}

		public bool IsDestroyed
		{
			get
			{
				return this.HitPoint <= 0f;
			}
		}

		public GameEntity CurrentState { get; private set; }

		private DestructableComponent()
		{
		}

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

		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._referenceEntity = (string.IsNullOrEmpty(this.ReferenceEntityTag) ? base.GameEntity : base.GameEntity.GetChildren().FirstOrDefault((GameEntity x) => x.HasTag(this.ReferenceEntityTag)));
		}

		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName.Equals(this.ReferenceEntityTag))
			{
				this._referenceEntity = (string.IsNullOrEmpty(this.ReferenceEntityTag) ? base.GameEntity : base.GameEntity.GetChildren().SingleOrDefault((GameEntity x) => x.HasTag(this.ReferenceEntityTag)));
			}
		}

		protected internal override void OnMissionReset()
		{
			base.OnMissionReset();
			this.Reset();
		}

		public void Reset()
		{
			this.RestoreEntity();
			this._hitPoint = this.MaxHitPoint;
			this._currentStateIndex = 0;
		}

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

		public void TriggerOnHit(Agent attackerAgent, int inflictedDamage, Vec3 impactPosition, Vec3 impactDirection, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior)
		{
			bool flag;
			this.OnHit(attackerAgent, inflictedDamage, impactPosition, impactDirection, weapon, attackerScriptComponentBehavior, out flag);
		}

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

		protected internal override bool MovesEntity()
		{
			return true;
		}

		public void PreDestroy()
		{
			DestructableComponent.OnHitTakenAndDestroyedDelegate onDestroyed = this.OnDestroyed;
			if (onDestroyed != null)
			{
				onDestroyed(this, null, MissionWeapon.Invalid, null, 0);
			}
			this.SetVisibleSynched(false, true);
		}

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

		private bool HasDestructionState
		{
			get
			{
				return this._destructionStates != null && !this._destructionStates.IsEmpty<string>();
			}
		}

		public override void AddStuckMissile(GameEntity missileEntity)
		{
			if (this.CurrentState != null)
			{
				this.CurrentState.AddChild(missileEntity, false);
				return;
			}
			base.GameEntity.AddChild(missileEntity, false);
		}

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

		public void OnFocusGain(Agent userAgent)
		{
		}

		public void OnFocusLose(Agent userAgent)
		{
		}

		public TextObject GetInfoTextForBeingNotInteractable(Agent userAgent)
		{
			return new TextObject("", null);
		}

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

		public const string CleanStateTag = "operational";

		public static float MaxBlowMagnitude = 20f;

		public string DestructionStates;

		public bool DestroyedByStoneOnly;

		public bool CanBeDestroyedInitially = true;

		public float MaxHitPoint = 100f;

		public bool DestroyOnAnyHit;

		public bool PassHitOnToParent;

		public string ReferenceEntityTag;

		public string HeavyHitParticlesTag;

		public float HeavyHitParticlesThreshold = 5f;

		public string ParticleEffectOnDestroy = "";

		public string SoundEffectOnDestroy = "";

		public float SoundAndParticleEffectHeightOffset;

		public float SoundAndParticleEffectForwardOffset;

		public BattleSideEnum BattleSide = BattleSideEnum.None;

		[EditableScriptComponentVariable(false)]
		public Func<int, int, int, int> OnCalculateDestructionStateIndex;

		private float _hitPoint;

		private string OriginalStateTag = "operational";

		private GameEntity _referenceEntity;

		private GameEntity _previousState;

		private GameEntity _originalState;

		private string[] _destructionStates;

		private int _currentStateIndex;

		private IEnumerable<GameEntity> _heavyHitParticles;

		public delegate void OnHitTakenAndDestroyedDelegate(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage);
	}
}
