using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002B1 RID: 689
	public class SpawnComponent : MissionLogic
	{
		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x0600261F RID: 9759 RVA: 0x00090966 File Offset: 0x0008EB66
		// (set) Token: 0x06002620 RID: 9760 RVA: 0x0009096E File Offset: 0x0008EB6E
		public SpawnFrameBehaviorBase SpawnFrameBehavior { get; private set; }

		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x06002621 RID: 9761 RVA: 0x00090977 File Offset: 0x0008EB77
		// (set) Token: 0x06002622 RID: 9762 RVA: 0x0009097F File Offset: 0x0008EB7F
		public SpawningBehaviorBase SpawningBehavior { get; private set; }

		// Token: 0x06002623 RID: 9763 RVA: 0x00090988 File Offset: 0x0008EB88
		public SpawnComponent(SpawnFrameBehaviorBase spawnFrameBehavior, SpawningBehaviorBase spawningBehavior)
		{
			this.SpawnFrameBehavior = spawnFrameBehavior;
			this.SpawningBehavior = spawningBehavior;
		}

		// Token: 0x06002624 RID: 9764 RVA: 0x0009099E File Offset: 0x0008EB9E
		public bool AreAgentsSpawning()
		{
			return this.SpawningBehavior.AreAgentsSpawning();
		}

		// Token: 0x06002625 RID: 9765 RVA: 0x000909AB File Offset: 0x0008EBAB
		public void SetNewSpawnFrameBehavior(SpawnFrameBehaviorBase spawnFrameBehavior)
		{
			this.SpawnFrameBehavior = spawnFrameBehavior;
			if (this.SpawnFrameBehavior != null)
			{
				this.SpawnFrameBehavior.Initialize();
			}
		}

		// Token: 0x06002626 RID: 9766 RVA: 0x000909C7 File Offset: 0x0008EBC7
		public void SetNewSpawningBehavior(SpawningBehaviorBase spawningBehavior)
		{
			this.SpawningBehavior = spawningBehavior;
			if (this.SpawningBehavior != null)
			{
				this.SpawningBehavior.Initialize(this);
			}
		}

		// Token: 0x06002627 RID: 9767 RVA: 0x000909E4 File Offset: 0x0008EBE4
		protected override void OnEndMission()
		{
			base.OnEndMission();
			this.SpawningBehavior.Clear();
		}

		// Token: 0x06002628 RID: 9768 RVA: 0x000909F7 File Offset: 0x0008EBF7
		public static void SetSiegeSpawningBehavior()
		{
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetNewSpawnFrameBehavior(new SiegeSpawnFrameBehavior());
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetNewSpawningBehavior(new SiegeSpawningBehavior());
		}

		// Token: 0x06002629 RID: 9769 RVA: 0x00090A21 File Offset: 0x0008EC21
		public static void SetFlagDominationSpawningBehavior()
		{
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetNewSpawnFrameBehavior(new FlagDominationSpawnFrameBehavior());
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetNewSpawningBehavior(new FlagDominationSpawningBehavior());
		}

		// Token: 0x0600262A RID: 9770 RVA: 0x00090A4B File Offset: 0x0008EC4B
		public static void SetWarmupSpawningBehavior()
		{
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetNewSpawnFrameBehavior(new FFASpawnFrameBehavior());
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetNewSpawningBehavior(new WarmupSpawningBehavior());
		}

		// Token: 0x0600262B RID: 9771 RVA: 0x00090A75 File Offset: 0x0008EC75
		public static void SetSpawningBehaviorForCurrentGameType(MissionLobbyComponent.MultiplayerGameType currentGameType)
		{
			if (currentGameType == MissionLobbyComponent.MultiplayerGameType.Siege)
			{
				SpawnComponent.SetSiegeSpawningBehavior();
				return;
			}
			if (currentGameType - MissionLobbyComponent.MultiplayerGameType.Battle > 2)
			{
				return;
			}
			SpawnComponent.SetFlagDominationSpawningBehavior();
		}

		// Token: 0x0600262C RID: 9772 RVA: 0x00090A8D File Offset: 0x0008EC8D
		public override void AfterStart()
		{
			base.AfterStart();
			this.SetNewSpawnFrameBehavior(this.SpawnFrameBehavior);
			this.SetNewSpawningBehavior(this.SpawningBehavior);
		}

		// Token: 0x0600262D RID: 9773 RVA: 0x00090AAD File Offset: 0x0008ECAD
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			this.SpawningBehavior.OnTick(dt);
		}

		// Token: 0x0600262E RID: 9774 RVA: 0x00090AC2 File Offset: 0x0008ECC2
		protected void StartSpawnSession()
		{
			this.SpawningBehavior.RequestStartSpawnSession();
		}

		// Token: 0x0600262F RID: 9775 RVA: 0x00090ACF File Offset: 0x0008ECCF
		public MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn = false)
		{
			SpawnFrameBehaviorBase spawnFrameBehavior = this.SpawnFrameBehavior;
			if (spawnFrameBehavior == null)
			{
				return MatrixFrame.Identity;
			}
			return spawnFrameBehavior.GetSpawnFrame(team, hasMount, isInitialSpawn);
		}

		// Token: 0x06002630 RID: 9776 RVA: 0x00090AE9 File Offset: 0x0008ECE9
		protected void SpawnEquipmentUpdated(MissionPeer lobbyPeer, Equipment equipment)
		{
			if (GameNetwork.IsServer && lobbyPeer != null && this.SpawningBehavior.CanUpdateSpawnEquipment(lobbyPeer) && lobbyPeer.HasSpawnedAgentVisuals)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new EquipEquipmentToPeer(lobbyPeer.GetNetworkPeer(), equipment));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
		}

		// Token: 0x06002631 RID: 9777 RVA: 0x00090B28 File Offset: 0x0008ED28
		public void SetEarlyAgentVisualsDespawning(MissionPeer missionPeer, bool canDespawnEarly = true)
		{
			if (missionPeer != null && this.AllowEarlyAgentVisualsDespawning(missionPeer))
			{
				missionPeer.EquipmentUpdatingExpired = canDespawnEarly;
			}
		}

		// Token: 0x06002632 RID: 9778 RVA: 0x00090B3D File Offset: 0x0008ED3D
		public void ToggleUpdatingSpawnEquipment(bool canUpdate)
		{
			this.SpawningBehavior.ToggleUpdatingSpawnEquipment(canUpdate);
		}

		// Token: 0x06002633 RID: 9779 RVA: 0x00090B4B File Offset: 0x0008ED4B
		public bool AllowEarlyAgentVisualsDespawning(MissionPeer lobbyPeer)
		{
			return this.SpawningBehavior.AllowEarlyAgentVisualsDespawning(lobbyPeer);
		}

		// Token: 0x06002634 RID: 9780 RVA: 0x00090B59 File Offset: 0x0008ED59
		public int GetMaximumReSpawnPeriodForPeer(MissionPeer lobbyPeer)
		{
			return this.SpawningBehavior.GetMaximumReSpawnPeriodForPeer(lobbyPeer);
		}

		// Token: 0x06002635 RID: 9781 RVA: 0x00090B67 File Offset: 0x0008ED67
		public override void OnClearScene()
		{
			base.OnClearScene();
			this.SpawningBehavior.OnClearScene();
		}
	}
}
