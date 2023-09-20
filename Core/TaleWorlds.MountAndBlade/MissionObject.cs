using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000248 RID: 584
	public abstract class MissionObject : ScriptComponentBehavior
	{
		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x06001FC7 RID: 8135 RVA: 0x00070A0A File Offset: 0x0006EC0A
		private Mission Mission
		{
			get
			{
				return Mission.Current;
			}
		}

		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x06001FC8 RID: 8136 RVA: 0x00070A11 File Offset: 0x0006EC11
		// (set) Token: 0x06001FC9 RID: 8137 RVA: 0x00070A19 File Offset: 0x0006EC19
		public MissionObjectId Id { get; set; }

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x06001FCA RID: 8138 RVA: 0x00070A22 File Offset: 0x0006EC22
		// (set) Token: 0x06001FCB RID: 8139 RVA: 0x00070A2A File Offset: 0x0006EC2A
		public bool IsDisabled { get; private set; }

		// Token: 0x06001FCC RID: 8140 RVA: 0x00070A34 File Offset: 0x0006EC34
		public MissionObject()
		{
			MissionObjectId missionObjectId = new MissionObjectId(-1, false);
			this.Id = missionObjectId;
		}

		// Token: 0x06001FCD RID: 8141 RVA: 0x00070A64 File Offset: 0x0006EC64
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

		// Token: 0x06001FCE RID: 8142 RVA: 0x00070B2C File Offset: 0x0006ED2C
		protected internal override void OnInit()
		{
			base.OnInit();
			if (!GameNetwork.IsClientOrReplay)
			{
				this.AttachDynamicNavmeshToEntity();
				this.SetAbilityOfFaces(base.GameEntity != null && base.GameEntity.IsVisibleIncludeParents());
			}
		}

		// Token: 0x06001FCF RID: 8143 RVA: 0x00070B64 File Offset: 0x0006ED64
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

		// Token: 0x06001FD0 RID: 8144 RVA: 0x00070C28 File Offset: 0x0006EE28
		protected virtual GameEntity GetEntityToAttachNavMeshFaces()
		{
			return base.GameEntity;
		}

		// Token: 0x06001FD1 RID: 8145 RVA: 0x00070C30 File Offset: 0x0006EE30
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

		// Token: 0x06001FD2 RID: 8146 RVA: 0x00070D38 File Offset: 0x0006EF38
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

		// Token: 0x06001FD3 RID: 8147 RVA: 0x00070DAC File Offset: 0x0006EFAC
		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		// Token: 0x06001FD4 RID: 8148 RVA: 0x00070DCD File Offset: 0x0006EFCD
		protected internal virtual void OnMissionReset()
		{
		}

		// Token: 0x06001FD5 RID: 8149 RVA: 0x00070DCF File Offset: 0x0006EFCF
		public virtual void AfterMissionStart()
		{
		}

		// Token: 0x06001FD6 RID: 8150 RVA: 0x00070DD1 File Offset: 0x0006EFD1
		protected internal virtual bool OnHit(Agent attackerAgent, int damage, Vec3 impactPosition, Vec3 impactDirection, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, out bool reportDamage)
		{
			reportDamage = false;
			return false;
		}

		// Token: 0x06001FD7 RID: 8151 RVA: 0x00070DD8 File Offset: 0x0006EFD8
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

		// Token: 0x06001FD8 RID: 8152 RVA: 0x00070ED4 File Offset: 0x0006F0D4
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

		// Token: 0x06001FD9 RID: 8153 RVA: 0x00070FE8 File Offset: 0x0006F1E8
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

		// Token: 0x06001FDA RID: 8154 RVA: 0x00071015 File Offset: 0x0006F215
		public virtual void OnEndMission()
		{
		}

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x06001FDB RID: 8155 RVA: 0x00071017 File Offset: 0x0006F217
		public bool CreatedAtRuntime
		{
			get
			{
				return this.Id.CreatedAtRuntime;
			}
		}

		// Token: 0x06001FDC RID: 8156 RVA: 0x00071024 File Offset: 0x0006F224
		protected internal override bool MovesEntity()
		{
			return true;
		}

		// Token: 0x06001FDD RID: 8157 RVA: 0x00071027 File Offset: 0x0006F227
		public virtual void AddStuckMissile(GameEntity missileEntity)
		{
			base.GameEntity.AddChild(missileEntity, false);
		}

		// Token: 0x04000BC6 RID: 3014
		protected const int InsideNavMeshIdLocal = 1;

		// Token: 0x04000BC7 RID: 3015
		protected const int EnterNavMeshIdLocal = 2;

		// Token: 0x04000BC8 RID: 3016
		protected const int ExitNavMeshIdLocal = 3;

		// Token: 0x04000BC9 RID: 3017
		protected const int BlockerNavMeshIdLocal = 4;

		// Token: 0x04000BCA RID: 3018
		protected const int ExtraNavMesh1IdLocal = 5;

		// Token: 0x04000BCB RID: 3019
		protected const int ExtraNavMesh2IdLocal = 6;

		// Token: 0x04000BCC RID: 3020
		protected const int ExtraNavMesh3IdLocal = 7;

		// Token: 0x04000BCF RID: 3023
		[EditableScriptComponentVariable(true)]
		protected string NavMeshPrefabName = "";

		// Token: 0x04000BD0 RID: 3024
		protected int DynamicNavmeshIdStart;
	}
}
