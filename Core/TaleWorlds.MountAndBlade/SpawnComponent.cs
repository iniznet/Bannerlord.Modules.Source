using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class SpawnComponent : MissionLogic
	{
		public SpawnFrameBehaviorBase SpawnFrameBehavior { get; private set; }

		public SpawningBehaviorBase SpawningBehavior { get; private set; }

		public SpawnComponent(SpawnFrameBehaviorBase spawnFrameBehavior, SpawningBehaviorBase spawningBehavior)
		{
			this.SpawnFrameBehavior = spawnFrameBehavior;
			this.SpawningBehavior = spawningBehavior;
		}

		public bool AreAgentsSpawning()
		{
			return this.SpawningBehavior.AreAgentsSpawning();
		}

		public void SetNewSpawnFrameBehavior(SpawnFrameBehaviorBase spawnFrameBehavior)
		{
			this.SpawnFrameBehavior = spawnFrameBehavior;
			if (this.SpawnFrameBehavior != null)
			{
				this.SpawnFrameBehavior.Initialize();
			}
		}

		public void SetNewSpawningBehavior(SpawningBehaviorBase spawningBehavior)
		{
			this.SpawningBehavior = spawningBehavior;
			if (this.SpawningBehavior != null)
			{
				this.SpawningBehavior.Initialize(this);
			}
		}

		protected override void OnEndMission()
		{
			base.OnEndMission();
			this.SpawningBehavior.Clear();
		}

		public static void SetSiegeSpawningBehavior()
		{
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetNewSpawnFrameBehavior(new SiegeSpawnFrameBehavior());
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetNewSpawningBehavior(new SiegeSpawningBehavior());
		}

		public static void SetFlagDominationSpawningBehavior()
		{
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetNewSpawnFrameBehavior(new FlagDominationSpawnFrameBehavior());
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetNewSpawningBehavior(new FlagDominationSpawningBehavior());
		}

		public static void SetWarmupSpawningBehavior()
		{
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetNewSpawnFrameBehavior(new FFASpawnFrameBehavior());
			Mission.Current.GetMissionBehavior<SpawnComponent>().SetNewSpawningBehavior(new WarmupSpawningBehavior());
		}

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

		public override void AfterStart()
		{
			base.AfterStart();
			this.SetNewSpawnFrameBehavior(this.SpawnFrameBehavior);
			this.SetNewSpawningBehavior(this.SpawningBehavior);
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			this.SpawningBehavior.OnTick(dt);
		}

		protected void StartSpawnSession()
		{
			this.SpawningBehavior.RequestStartSpawnSession();
		}

		public MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn = false)
		{
			SpawnFrameBehaviorBase spawnFrameBehavior = this.SpawnFrameBehavior;
			if (spawnFrameBehavior == null)
			{
				return MatrixFrame.Identity;
			}
			return spawnFrameBehavior.GetSpawnFrame(team, hasMount, isInitialSpawn);
		}

		protected void SpawnEquipmentUpdated(MissionPeer lobbyPeer, Equipment equipment)
		{
			if (GameNetwork.IsServer && lobbyPeer != null && this.SpawningBehavior.CanUpdateSpawnEquipment(lobbyPeer) && lobbyPeer.HasSpawnedAgentVisuals)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new EquipEquipmentToPeer(lobbyPeer.GetNetworkPeer(), equipment));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
		}

		public void SetEarlyAgentVisualsDespawning(MissionPeer missionPeer, bool canDespawnEarly = true)
		{
			if (missionPeer != null && this.AllowEarlyAgentVisualsDespawning(missionPeer))
			{
				missionPeer.EquipmentUpdatingExpired = canDespawnEarly;
			}
		}

		public void ToggleUpdatingSpawnEquipment(bool canUpdate)
		{
			this.SpawningBehavior.ToggleUpdatingSpawnEquipment(canUpdate);
		}

		public bool AllowEarlyAgentVisualsDespawning(MissionPeer lobbyPeer)
		{
			return this.SpawningBehavior.AllowEarlyAgentVisualsDespawning(lobbyPeer);
		}

		public int GetMaximumReSpawnPeriodForPeer(MissionPeer lobbyPeer)
		{
			return this.SpawningBehavior.GetMaximumReSpawnPeriodForPeer(lobbyPeer);
		}

		public override void OnClearScene()
		{
			base.OnClearScene();
			this.SpawningBehavior.OnClearScene();
		}
	}
}
