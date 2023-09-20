using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000253 RID: 595
	public abstract class MissionBehavior : IMissionBehavior
	{
		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x06002004 RID: 8196 RVA: 0x0007168A File Offset: 0x0006F88A
		// (set) Token: 0x06002005 RID: 8197 RVA: 0x00071692 File Offset: 0x0006F892
		public Mission Mission { get; internal set; }

		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x06002006 RID: 8198 RVA: 0x0007169B File Offset: 0x0006F89B
		public IInputContext DebugInput
		{
			get
			{
				return Input.DebugInput;
			}
		}

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x06002007 RID: 8199
		public abstract MissionBehaviorType BehaviorType { get; }

		// Token: 0x06002008 RID: 8200 RVA: 0x000716A2 File Offset: 0x0006F8A2
		public virtual void OnAfterMissionCreated()
		{
		}

		// Token: 0x06002009 RID: 8201 RVA: 0x000716A4 File Offset: 0x0006F8A4
		public virtual void OnBehaviorInitialize()
		{
		}

		// Token: 0x0600200A RID: 8202 RVA: 0x000716A6 File Offset: 0x0006F8A6
		public virtual void OnCreated()
		{
		}

		// Token: 0x0600200B RID: 8203 RVA: 0x000716A8 File Offset: 0x0006F8A8
		public virtual void EarlyStart()
		{
		}

		// Token: 0x0600200C RID: 8204 RVA: 0x000716AA File Offset: 0x0006F8AA
		public virtual void AfterStart()
		{
		}

		// Token: 0x0600200D RID: 8205 RVA: 0x000716AC File Offset: 0x0006F8AC
		public virtual void OnMissileHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
		{
		}

		// Token: 0x0600200E RID: 8206 RVA: 0x000716AE File Offset: 0x0006F8AE
		public virtual void OnMeleeHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
		{
		}

		// Token: 0x0600200F RID: 8207 RVA: 0x000716B0 File Offset: 0x0006F8B0
		public virtual void OnMissileCollisionReaction(Mission.MissileCollisionReaction collisionReaction, Agent attackerAgent, Agent attachedAgent, sbyte attachedBoneIndex)
		{
		}

		// Token: 0x06002010 RID: 8208 RVA: 0x000716B2 File Offset: 0x0006F8B2
		public virtual void OnMissionScreenPreLoad()
		{
		}

		// Token: 0x06002011 RID: 8209 RVA: 0x000716B4 File Offset: 0x0006F8B4
		public virtual void OnAgentCreated(Agent agent)
		{
		}

		// Token: 0x06002012 RID: 8210 RVA: 0x000716B6 File Offset: 0x0006F8B6
		public virtual void OnAgentBuild(Agent agent, Banner banner)
		{
		}

		// Token: 0x06002013 RID: 8211 RVA: 0x000716B8 File Offset: 0x0006F8B8
		public virtual void OnAgentTeamChanged(Team prevTeam, Team newTeam, Agent agent)
		{
		}

		// Token: 0x06002014 RID: 8212 RVA: 0x000716BA File Offset: 0x0006F8BA
		public virtual void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
		}

		// Token: 0x06002015 RID: 8213 RVA: 0x000716BC File Offset: 0x0006F8BC
		public virtual void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
		}

		// Token: 0x06002016 RID: 8214 RVA: 0x000716BE File Offset: 0x0006F8BE
		public virtual void OnEarlyAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
		}

		// Token: 0x06002017 RID: 8215 RVA: 0x000716C0 File Offset: 0x0006F8C0
		public virtual void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
		}

		// Token: 0x06002018 RID: 8216 RVA: 0x000716C2 File Offset: 0x0006F8C2
		public virtual void OnAgentDeleted(Agent affectedAgent)
		{
		}

		// Token: 0x06002019 RID: 8217 RVA: 0x000716C4 File Offset: 0x0006F8C4
		public virtual void OnAgentFleeing(Agent affectedAgent)
		{
		}

		// Token: 0x0600201A RID: 8218 RVA: 0x000716C6 File Offset: 0x0006F8C6
		public virtual void OnAgentPanicked(Agent affectedAgent)
		{
		}

		// Token: 0x0600201B RID: 8219 RVA: 0x000716C8 File Offset: 0x0006F8C8
		public virtual void OnFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
		{
		}

		// Token: 0x0600201C RID: 8220 RVA: 0x000716CA File Offset: 0x0006F8CA
		public virtual void OnFocusLost(Agent agent, IFocusable focusableObject)
		{
		}

		// Token: 0x0600201D RID: 8221 RVA: 0x000716CC File Offset: 0x0006F8CC
		public virtual void OnAddTeam(Team team)
		{
		}

		// Token: 0x0600201E RID: 8222 RVA: 0x000716CE File Offset: 0x0006F8CE
		public virtual void AfterAddTeam(Team team)
		{
		}

		// Token: 0x0600201F RID: 8223 RVA: 0x000716D0 File Offset: 0x0006F8D0
		public virtual void OnAgentInteraction(Agent userAgent, Agent agent)
		{
		}

		// Token: 0x06002020 RID: 8224 RVA: 0x000716D2 File Offset: 0x0006F8D2
		public virtual void OnClearScene()
		{
		}

		// Token: 0x06002021 RID: 8225 RVA: 0x000716D4 File Offset: 0x0006F8D4
		public virtual void OnEndMissionInternal()
		{
			this.OnEndMission();
		}

		// Token: 0x06002022 RID: 8226 RVA: 0x000716DC File Offset: 0x0006F8DC
		protected virtual void OnEndMission()
		{
		}

		// Token: 0x06002023 RID: 8227 RVA: 0x000716DE File Offset: 0x0006F8DE
		public virtual void OnRemoveBehavior()
		{
		}

		// Token: 0x06002024 RID: 8228 RVA: 0x000716E0 File Offset: 0x0006F8E0
		public virtual void OnPreMissionTick(float dt)
		{
		}

		// Token: 0x06002025 RID: 8229 RVA: 0x000716E2 File Offset: 0x0006F8E2
		public virtual void OnPreDisplayMissionTick(float dt)
		{
		}

		// Token: 0x06002026 RID: 8230 RVA: 0x000716E4 File Offset: 0x0006F8E4
		public virtual void OnMissionTick(float dt)
		{
		}

		// Token: 0x06002027 RID: 8231 RVA: 0x000716E6 File Offset: 0x0006F8E6
		public virtual void OnAgentMount(Agent agent)
		{
		}

		// Token: 0x06002028 RID: 8232 RVA: 0x000716E8 File Offset: 0x0006F8E8
		public virtual void OnAgentDismount(Agent agent)
		{
		}

		// Token: 0x06002029 RID: 8233 RVA: 0x000716EA File Offset: 0x0006F8EA
		public virtual bool IsThereAgentAction(Agent userAgent, Agent otherAgent)
		{
			return false;
		}

		// Token: 0x0600202A RID: 8234 RVA: 0x000716ED File Offset: 0x0006F8ED
		public virtual void OnEntityRemoved(GameEntity entity)
		{
		}

		// Token: 0x0600202B RID: 8235 RVA: 0x000716EF File Offset: 0x0006F8EF
		public virtual void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
		{
		}

		// Token: 0x0600202C RID: 8236 RVA: 0x000716F1 File Offset: 0x0006F8F1
		public virtual void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usedObject)
		{
		}

		// Token: 0x0600202D RID: 8237 RVA: 0x000716F3 File Offset: 0x0006F8F3
		public virtual void OnRenderingStarted()
		{
		}

		// Token: 0x0600202E RID: 8238 RVA: 0x000716F5 File Offset: 0x0006F8F5
		public virtual void OnMissionStateActivated()
		{
		}

		// Token: 0x0600202F RID: 8239 RVA: 0x000716F7 File Offset: 0x0006F8F7
		public virtual void OnMissionStateFinalized()
		{
		}

		// Token: 0x06002030 RID: 8240 RVA: 0x000716F9 File Offset: 0x0006F8F9
		public virtual void OnMissionStateDeactivated()
		{
		}

		// Token: 0x06002031 RID: 8241 RVA: 0x000716FB File Offset: 0x0006F8FB
		public virtual void OnMissionRestart()
		{
		}

		// Token: 0x06002032 RID: 8242 RVA: 0x000716FD File Offset: 0x0006F8FD
		public virtual List<CompassItemUpdateParams> GetCompassTargets()
		{
			return null;
		}

		// Token: 0x06002033 RID: 8243 RVA: 0x00071700 File Offset: 0x0006F900
		public virtual void OnAssignPlayerAsSergeantOfFormation(Agent agent)
		{
		}

		// Token: 0x06002034 RID: 8244 RVA: 0x00071702 File Offset: 0x0006F902
		public virtual void OnDeploymentFinished()
		{
		}

		// Token: 0x06002035 RID: 8245 RVA: 0x00071704 File Offset: 0x0006F904
		public virtual void OnTeamDeployed(Team team)
		{
		}

		// Token: 0x06002036 RID: 8246 RVA: 0x00071706 File Offset: 0x0006F906
		protected internal virtual void OnGetAgentState(Agent agent, bool usedSurgery)
		{
		}

		// Token: 0x06002037 RID: 8247 RVA: 0x00071708 File Offset: 0x0006F908
		public virtual void OnAgentAlarmedStateChanged(Agent agent, Agent.AIStateFlag flag)
		{
		}

		// Token: 0x06002038 RID: 8248 RVA: 0x0007170A File Offset: 0x0006F90A
		protected internal virtual void OnObjectDisabled(DestructableComponent destructionComponent)
		{
		}

		// Token: 0x06002039 RID: 8249 RVA: 0x0007170C File Offset: 0x0006F90C
		public virtual void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
		{
		}

		// Token: 0x0600203A RID: 8250 RVA: 0x0007170E File Offset: 0x0006F90E
		protected internal virtual void OnAgentControllerChanged(Agent agent, Agent.ControllerType oldController)
		{
		}

		// Token: 0x0600203B RID: 8251 RVA: 0x00071710 File Offset: 0x0006F910
		public virtual void OnItemPickup(Agent agent, SpawnedItemEntity item)
		{
		}

		// Token: 0x0600203C RID: 8252 RVA: 0x00071712 File Offset: 0x0006F912
		public virtual void OnItemDrop(Agent agent, SpawnedItemEntity item)
		{
		}

		// Token: 0x0600203D RID: 8253 RVA: 0x00071714 File Offset: 0x0006F914
		public virtual void OnRegisterBlow(Agent attacker, Agent victim, GameEntity realHitEntity, Blow b, ref AttackCollisionData collisionData, in MissionWeapon attackerWeapon)
		{
		}

		// Token: 0x0600203E RID: 8254 RVA: 0x00071716 File Offset: 0x0006F916
		public virtual void OnAgentShootMissile(Agent shooterAgent, EquipmentIndex weaponIndex, Vec3 position, Vec3 velocity, Mat3 orientation, bool hasRigidBody, int forcedMissileIndex)
		{
		}
	}
}
