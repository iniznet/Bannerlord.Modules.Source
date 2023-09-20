using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002C7 RID: 711
	public abstract class SpawnFrameBehaviorBase
	{
		// Token: 0x06002705 RID: 9989 RVA: 0x00093B9D File Offset: 0x00091D9D
		public virtual void Initialize()
		{
			this.SpawnPoints = Mission.Current.Scene.FindEntitiesWithTag("spawnpoint");
		}

		// Token: 0x06002706 RID: 9990
		public abstract MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn);

		// Token: 0x06002707 RID: 9991 RVA: 0x00093BBC File Offset: 0x00091DBC
		protected MatrixFrame GetSpawnFrameFromSpawnPoints(IList<GameEntity> spawnPointsList, Team team, bool hasMount)
		{
			float num = float.MinValue;
			int num2 = -1;
			for (int i = 0; i < spawnPointsList.Count; i++)
			{
				float num3 = MBRandom.RandomFloat * 0.2f;
				float num4 = 0f;
				if (hasMount && spawnPointsList[i].HasTag("exclude_mounted"))
				{
					num3 -= 1000f;
				}
				if (!hasMount && spawnPointsList[i].HasTag("exclude_footmen"))
				{
					num3 -= 1000f;
				}
				SpawnFrameBehaviorBase.WeightCache weightCache = SpawnFrameBehaviorBase.WeightCache.CreateDecreasingCache();
				SpawnFrameBehaviorBase.WeightCache weightCache2 = SpawnFrameBehaviorBase.WeightCache.CreateDecreasingCache();
				foreach (Agent agent in Mission.Current.Agents)
				{
					if (!agent.IsMount)
					{
						float length = (agent.Position - spawnPointsList[i].GlobalPosition).Length;
						float num6;
						if (team == null || agent.Team.IsEnemyOf(team))
						{
							float num5 = 3.75f - length * 0.125f;
							num6 = MathF.Tanh(num5 * num5) * -2f + 3.1f - length * 0.0125f - 1f / ((length + 0.0001f) * 0.05f);
						}
						else
						{
							float num7 = 1.8f - length * 0.1f;
							num6 = -MathF.Tanh(num7 * num7) + 1.7f - length * 0.01f - 1f / ((length + 0.0001f) * 0.1f);
						}
						float num9;
						if (num6 >= 0f)
						{
							float num8;
							if (weightCache.CheckAndInsertNewValueIfLower(num6, out num8))
							{
								num4 -= num8;
							}
						}
						else if (weightCache2.CheckAndInsertNewValueIfLower(num6, out num9))
						{
							num4 -= num9;
						}
					}
				}
				if (num4 > 0f)
				{
					num4 /= (float)Mission.Current.Agents.Count;
				}
				num3 += num4;
				if (num3 > num)
				{
					num = num3;
					num2 = i;
				}
			}
			MatrixFrame globalFrame = spawnPointsList[num2].GetGlobalFrame();
			globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			return globalFrame;
		}

		// Token: 0x04000E7B RID: 3707
		private const string ExcludeMountedTag = "exclude_mounted";

		// Token: 0x04000E7C RID: 3708
		private const string ExcludeFootmenTag = "exclude_footmen";

		// Token: 0x04000E7D RID: 3709
		protected const string SpawnPointTag = "spawnpoint";

		// Token: 0x04000E7E RID: 3710
		public IEnumerable<GameEntity> SpawnPoints;

		// Token: 0x020005E4 RID: 1508
		private struct WeightCache
		{
			// Token: 0x170009B6 RID: 2486
			private float this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this._value1;
					case 1:
						return this._value2;
					case 2:
						return this._value3;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
				set
				{
					switch (index)
					{
					case 0:
						this._value1 = value;
						return;
					case 1:
						this._value2 = value;
						return;
					case 2:
						this._value3 = value;
						return;
					default:
						return;
					}
				}
			}

			// Token: 0x06003CBB RID: 15547 RVA: 0x000F17FF File Offset: 0x000EF9FF
			private WeightCache(float value1, float value2, float value3)
			{
				this._value1 = value1;
				this._value2 = value2;
				this._value3 = value3;
			}

			// Token: 0x06003CBC RID: 15548 RVA: 0x000F1816 File Offset: 0x000EFA16
			public static SpawnFrameBehaviorBase.WeightCache CreateDecreasingCache()
			{
				return new SpawnFrameBehaviorBase.WeightCache(float.NaN, float.NaN, float.NaN);
			}

			// Token: 0x06003CBD RID: 15549 RVA: 0x000F182C File Offset: 0x000EFA2C
			public bool CheckAndInsertNewValueIfLower(float value, out float valueDifference)
			{
				int num = 0;
				for (int i = 1; i < 3; i++)
				{
					if (this[i] > this[num])
					{
						num = i;
					}
				}
				if (float.IsNaN(this[num]) || value < this[num])
				{
					valueDifference = (float.IsNaN(this[num]) ? MathF.Abs(value) : (this[num] - value));
					this[num] = value;
					return true;
				}
				valueDifference = float.NaN;
				return false;
			}

			// Token: 0x04001EED RID: 7917
			private const int Length = 3;

			// Token: 0x04001EEE RID: 7918
			private float _value1;

			// Token: 0x04001EEF RID: 7919
			private float _value2;

			// Token: 0x04001EF0 RID: 7920
			private float _value3;
		}
	}
}
