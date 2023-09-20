using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class FlagDominationSpawnFrameBehavior : SpawnFrameBehaviorBase
	{
		public override void Initialize()
		{
			base.Initialize();
			this._spawnPointsByTeam = new List<GameEntity>[2];
			this._spawnZonesByTeam = new List<GameEntity>[2];
			this._spawnPointsByTeam[1] = this.SpawnPoints.Where((GameEntity x) => x.HasTag("attacker")).ToList<GameEntity>();
			this._spawnPointsByTeam[0] = this.SpawnPoints.Where((GameEntity x) => x.HasTag("defender")).ToList<GameEntity>();
			this._spawnZonesByTeam[1] = (from sz in this._spawnPointsByTeam[1].Select((GameEntity sp) => sp.Parent).Distinct<GameEntity>()
				where sz != null
				select sz).ToList<GameEntity>();
			this._spawnZonesByTeam[0] = (from sz in this._spawnPointsByTeam[0].Select((GameEntity sp) => sp.Parent).Distinct<GameEntity>()
				where sz != null
				select sz).ToList<GameEntity>();
		}

		public override MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn)
		{
			GameEntity bestZone = this.GetBestZone(team, isInitialSpawn);
			List<GameEntity> list;
			if (bestZone != null)
			{
				list = this._spawnPointsByTeam[(int)team.Side].Where((GameEntity sp) => sp.Parent == bestZone).ToList<GameEntity>();
			}
			else
			{
				list = this._spawnPointsByTeam[(int)team.Side].ToList<GameEntity>();
			}
			return this.GetBestSpawnPoint(list, hasMount);
		}

		private GameEntity GetBestZone(Team team, bool isInitialSpawn)
		{
			if (this._spawnZonesByTeam[(int)team.Side].Count == 0)
			{
				return null;
			}
			if (isInitialSpawn)
			{
				return this._spawnZonesByTeam[(int)team.Side].Single((GameEntity sz) => sz.HasTag("starting"));
			}
			List<GameEntity> list = this._spawnZonesByTeam[(int)team.Side].Where((GameEntity sz) => !sz.HasTag("starting")).ToList<GameEntity>();
			if (list.Count == 0)
			{
				return null;
			}
			float[] array = new float[list.Count];
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (((component != null) ? component.Team : null) != null && component.Team.Side != BattleSideEnum.None && component.ControlledAgent != null && component.ControlledAgent.IsActive())
				{
					for (int i = 0; i < list.Count; i++)
					{
						Vec3 globalPosition = list[i].GlobalPosition;
						if (component.Team != team)
						{
							array[i] -= 1f / (0.0001f + component.ControlledAgent.Position.Distance(globalPosition)) * 1f;
						}
						else
						{
							array[i] += 1f / (0.0001f + component.ControlledAgent.Position.Distance(globalPosition)) * 1.5f;
						}
					}
				}
			}
			int num = -1;
			for (int j = 0; j < array.Length; j++)
			{
				if (num < 0 || array[j] > array[num])
				{
					num = j;
				}
			}
			return list[num];
		}

		private MatrixFrame GetBestSpawnPoint(List<GameEntity> spawnPointList, bool hasMount)
		{
			float num = float.MinValue;
			int num2 = -1;
			for (int i = 0; i < spawnPointList.Count; i++)
			{
				float num3 = MBRandom.RandomFloat * 0.2f;
				AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(Mission.Current, spawnPointList[i].GlobalPosition.AsVec2, 2f, false);
				while (proximityMapSearchStruct.LastFoundAgent != null)
				{
					float num4 = proximityMapSearchStruct.LastFoundAgent.Position.DistanceSquared(spawnPointList[i].GlobalPosition);
					if (num4 < 4f)
					{
						float num5 = MathF.Sqrt(num4);
						num3 -= (2f - num5) * 5f;
					}
					AgentProximityMap.FindNext(Mission.Current, ref proximityMapSearchStruct);
				}
				if (hasMount && spawnPointList[i].HasTag("exclude_mounted"))
				{
					num3 -= 100f;
				}
				if (num3 > num)
				{
					num = num3;
					num2 = i;
				}
			}
			MatrixFrame globalFrame = spawnPointList[num2].GetGlobalFrame();
			globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			return globalFrame;
		}

		private List<GameEntity>[] _spawnPointsByTeam;

		private List<GameEntity>[] _spawnZonesByTeam;
	}
}
