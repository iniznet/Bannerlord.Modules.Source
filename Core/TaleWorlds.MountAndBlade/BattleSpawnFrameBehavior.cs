using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002C2 RID: 706
	public class BattleSpawnFrameBehavior : SpawnFrameBehaviorBase
	{
		// Token: 0x060026F2 RID: 9970 RVA: 0x000931D4 File Offset: 0x000913D4
		public override void Initialize()
		{
			base.Initialize();
			this._spawnPointsOfAttackers = this.SpawnPoints.Where((GameEntity x) => x.HasTag("attacker")).ToList<GameEntity>();
			this._spawnPointsOfDefenders = this.SpawnPoints.Where((GameEntity x) => x.HasTag("defender")).ToList<GameEntity>();
		}

		// Token: 0x060026F3 RID: 9971 RVA: 0x00093254 File Offset: 0x00091454
		public override MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn)
		{
			List<GameEntity> list = ((team == Mission.Current.AttackerTeam) ? this._spawnPointsOfAttackers : this._spawnPointsOfDefenders).ToList<GameEntity>();
			float num = float.MinValue;
			int num2 = -1;
			for (int i = 0; i < list.Count; i++)
			{
				float num3 = MBRandom.RandomFloat * 0.2f;
				AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(Mission.Current, list[i].GlobalPosition.AsVec2, 2f, false);
				while (proximityMapSearchStruct.LastFoundAgent != null)
				{
					float num4 = proximityMapSearchStruct.LastFoundAgent.Position.DistanceSquared(list[i].GlobalPosition);
					if (num4 < 4f)
					{
						float num5 = MathF.Sqrt(num4);
						num3 -= (2f - num5) * 5f;
					}
					AgentProximityMap.FindNext(Mission.Current, ref proximityMapSearchStruct);
				}
				if (hasMount && list[i].HasTag("exclude_mounted"))
				{
					num3 -= 100f;
				}
				if (num3 > num)
				{
					num = num3;
					num2 = i;
				}
			}
			return list[num2].GetGlobalFrame();
		}

		// Token: 0x04000E6D RID: 3693
		private List<GameEntity> _spawnPointsOfAttackers;

		// Token: 0x04000E6E RID: 3694
		private List<GameEntity> _spawnPointsOfDefenders;
	}
}
