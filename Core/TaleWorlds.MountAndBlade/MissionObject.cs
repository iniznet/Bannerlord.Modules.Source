using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MissionObject : ScriptComponentBehavior
	{
		private Mission Mission
		{
			get
			{
				return Mission.Current;
			}
		}

		public MissionObjectId Id { get; set; }

		public bool IsDisabled { get; private set; }

		public MissionObject()
		{
			MissionObjectId missionObjectId = new MissionObjectId(-1, false);
			this.Id = missionObjectId;
		}

		public virtual void SetAbilityOfFaces(bool enabled)
		{
			if (this.DynamicNavmeshIdStart > 0)
			{
				base.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 1, enabled);
				base.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 2, enabled);
				base.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 3, enabled);
				base.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 4, enabled);
				base.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 5, enabled);
				base.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 6, enabled);
				base.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 7, enabled);
			}
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			if (!GameNetwork.IsClientOrReplay)
			{
				this.AttachDynamicNavmeshToEntity();
				this.SetAbilityOfFaces(base.GameEntity != null && base.GameEntity.IsVisibleIncludeParents());
			}
		}

		protected virtual void AttachDynamicNavmeshToEntity()
		{
			if (this.NavMeshPrefabName.Length > 0)
			{
				this.DynamicNavmeshIdStart = Mission.Current.GetNextDynamicNavMeshIdStart();
				base.GameEntity.Scene.ImportNavigationMeshPrefab(this.NavMeshPrefabName, this.DynamicNavmeshIdStart);
				this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 1, false, false, false);
				this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 2, true, false, false);
				this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 3, true, false, false);
				this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 4, false, true, false);
				this.SetAbilityOfFaces(base.GameEntity != null && base.GameEntity.GetPhysicsState());
			}
		}

		protected virtual GameEntity GetEntityToAttachNavMeshFaces()
		{
			return base.GameEntity;
		}

		protected internal override bool OnCheckForProblems()
		{
			base.OnCheckForProblems();
			bool flag = false;
			List<GameEntity> list = new List<GameEntity>();
			list.Add(base.GameEntity);
			base.GameEntity.GetChildrenRecursive(ref list);
			bool flag2 = false;
			foreach (GameEntity gameEntity in list)
			{
				flag2 = flag2 || (gameEntity.HasPhysicsDefinitionWithoutFlags(1) && !gameEntity.PhysicsDescBodyFlag.HasAnyFlag(BodyFlags.CommonCollisionExcludeFlagsForMissile));
			}
			Vec3 scaleVector = base.GameEntity.GetGlobalFrame().rotation.GetScaleVector();
			bool flag3 = MathF.Abs(scaleVector.x - scaleVector.y) >= 0.01f || MathF.Abs(scaleVector.x - scaleVector.z) >= 0.01f;
			if (flag2 && flag3)
			{
				MBEditor.AddEntityWarning(base.GameEntity, "Mission object has non-uniform scale and physics object. This is not supported because any attached focusable item to this mesh will not work within this configuration.");
				flag = true;
			}
			return flag;
		}

		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			if (this.Mission != null)
			{
				int num = -1;
				bool flag;
				if (this.Mission.IsLoadingFinished)
				{
					flag = true;
					if (!GameNetwork.IsClientOrReplay)
					{
						num = this.Mission.GetFreeRuntimeMissionObjectId();
					}
				}
				else
				{
					flag = false;
					num = this.Mission.GetFreeSceneMissionObjectId();
				}
				this.Id = new MissionObjectId(num, flag);
				this.Mission.AddActiveMissionObject(this);
			}
			base.GameEntity.SetAsReplayEntity();
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		protected internal virtual void OnMissionReset()
		{
		}

		public virtual void AfterMissionStart()
		{
		}

		protected internal virtual bool OnHit(Agent attackerAgent, int damage, Vec3 impactPosition, Vec3 impactDirection, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, out bool reportDamage)
		{
			reportDamage = false;
			return false;
		}

		public void SetDisabled(bool isParentObject = false)
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				this.SetAbilityOfFaces(false);
			}
			if (isParentObject && base.GameEntity != null)
			{
				List<GameEntity> list = new List<GameEntity>();
				base.GameEntity.GetChildrenRecursive(ref list);
				foreach (MissionObject missionObject in from sc in list.SelectMany((GameEntity ac) => ac.GetScriptComponents())
					where sc is MissionObject
					select sc as MissionObject)
				{
					missionObject.SetDisabled(false);
				}
			}
			Mission.Current.DeactivateMissionObject(this);
			this.IsDisabled = true;
		}

		public void SetDisabledAndMakeInvisible(bool isParentObject = false)
		{
			if (isParentObject && base.GameEntity != null)
			{
				List<GameEntity> list = new List<GameEntity>();
				base.GameEntity.GetChildrenRecursive(ref list);
				foreach (MissionObject missionObject in from sc in list.SelectMany((GameEntity ac) => ac.GetScriptComponents())
					where sc is MissionObject
					select sc as MissionObject)
				{
					missionObject.SetDisabledAndMakeInvisible(false);
				}
			}
			Mission.Current.DeactivateMissionObject(this);
			this.IsDisabled = true;
			if (base.GameEntity != null)
			{
				base.GameEntity.SetVisibilityExcludeParents(false);
				base.SetScriptComponentToTick(this.GetTickRequirement());
			}
		}

		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			if (!GameNetwork.IsClientOrReplay)
			{
				this.SetAbilityOfFaces(false);
			}
			if (this.Mission != null)
			{
				this.Mission.OnMissionObjectRemoved(this, removeReason);
			}
		}

		public virtual void OnEndMission()
		{
		}

		public bool CreatedAtRuntime
		{
			get
			{
				return this.Id.CreatedAtRuntime;
			}
		}

		protected internal override bool MovesEntity()
		{
			return true;
		}

		public virtual void AddStuckMissile(GameEntity missileEntity)
		{
			base.GameEntity.AddChild(missileEntity, false);
		}

		protected const int InsideNavMeshIdLocal = 1;

		protected const int EnterNavMeshIdLocal = 2;

		protected const int ExitNavMeshIdLocal = 3;

		protected const int BlockerNavMeshIdLocal = 4;

		protected const int ExtraNavMesh1IdLocal = 5;

		protected const int ExtraNavMesh2IdLocal = 6;

		protected const int ExtraNavMesh3IdLocal = 7;

		[EditableScriptComponentVariable(true)]
		protected string NavMeshPrefabName = "";

		protected int DynamicNavmeshIdStart;
	}
}
