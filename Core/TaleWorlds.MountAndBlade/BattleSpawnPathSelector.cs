using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001F6 RID: 502
	public class BattleSpawnPathSelector
	{
		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x06001BE9 RID: 7145 RVA: 0x000630F4 File Offset: 0x000612F4
		// (set) Token: 0x06001BEA RID: 7146 RVA: 0x000630FC File Offset: 0x000612FC
		public bool IsInitialized { get; private set; }

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x06001BEB RID: 7147 RVA: 0x00063105 File Offset: 0x00061305
		public Path InitialPath
		{
			get
			{
				return this._initialPath;
			}
		}

		// Token: 0x06001BEC RID: 7148 RVA: 0x0006310D File Offset: 0x0006130D
		public BattleSpawnPathSelector(Mission mission)
		{
			this.IsInitialized = false;
			this._initialPath = null;
			this._mission = mission;
		}

		// Token: 0x06001BED RID: 7149 RVA: 0x0006312C File Offset: 0x0006132C
		public void Initialize()
		{
			float num;
			bool flag;
			Path path = BattleSpawnPathSelector.FindBestInitialPath(this._mission, out num, out flag);
			if (path != null)
			{
				this._initialPath = path;
				this._battleSideSelectors = new BattleSideSpawnPathSelector[2];
				this._battleSideSelectors[0] = new BattleSideSpawnPathSelector(this._mission, path, num, flag);
				this._battleSideSelectors[1] = new BattleSideSpawnPathSelector(this._mission, path, MathF.Max(1f - num, 0f), !flag);
				this.IsInitialized = true;
				return;
			}
			this._initialPath = null;
			this.IsInitialized = false;
		}

		// Token: 0x06001BEE RID: 7150 RVA: 0x000631BC File Offset: 0x000613BC
		public bool HasPath(Path path)
		{
			if (!this.IsInitialized)
			{
				Debug.FailedAssert("BattleSpawnPathSelector must be initialized.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Deployment\\BattleSpawnPathSelector.cs", "HasPath", 63);
				return false;
			}
			BattleSideSpawnPathSelector battleSideSpawnPathSelector = this._battleSideSelectors[1];
			BattleSideSpawnPathSelector battleSideSpawnPathSelector2 = this._battleSideSelectors[0];
			return path != null && (this._initialPath.Pointer == path.Pointer || battleSideSpawnPathSelector.HasReinforcementPath(path) || battleSideSpawnPathSelector2.HasReinforcementPath(path));
		}

		// Token: 0x06001BEF RID: 7151 RVA: 0x00063230 File Offset: 0x00061430
		public bool GetInitialPathDataOfSide(BattleSideEnum side, out SpawnPathData pathPathData)
		{
			if (!this.IsInitialized)
			{
				Debug.FailedAssert("BattleSpawnPathSelector must be initialized.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Deployment\\BattleSpawnPathSelector.cs", "GetInitialPathDataOfSide", 77);
				pathPathData = SpawnPathData.Invalid;
				return false;
			}
			pathPathData = this._battleSideSelectors[(int)side].InitialSpawnPath;
			return pathPathData.IsValid;
		}

		// Token: 0x06001BF0 RID: 7152 RVA: 0x00063281 File Offset: 0x00061481
		public MBReadOnlyList<SpawnPathData> GetReinforcementPathsDataOfSide(BattleSideEnum side)
		{
			if (!this.IsInitialized)
			{
				Debug.FailedAssert("BattleSpawnPathSelector must be initialized.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Deployment\\BattleSpawnPathSelector.cs", "GetReinforcementPathsDataOfSide", 91);
				return null;
			}
			return this._battleSideSelectors[(int)side].ReinforcementPaths;
		}

		// Token: 0x06001BF1 RID: 7153 RVA: 0x000632B0 File Offset: 0x000614B0
		public static Path FindBestInitialPath(Mission mission, out float centerRatio, out bool isInverted)
		{
			centerRatio = 0f;
			isInverted = false;
			MBList<Path> allSpawnPaths = MBSceneUtilities.GetAllSpawnPaths(mission.Scene);
			if (allSpawnPaths.IsEmpty<Path>())
			{
				return null;
			}
			int num = 2;
			foreach (Path path in allSpawnPaths)
			{
				num = MathF.Max(path.NumberOfPoints, num);
			}
			Path path2 = null;
			if (mission.HasSceneMapPatch())
			{
				Path path3 = null;
				bool flag = false;
				float num2 = float.MinValue;
				MatrixFrame[] array = new MatrixFrame[num];
				Vec3 vec;
				mission.GetPatchSceneEncounterPosition(out vec);
				Vec2 asVec = vec.AsVec2;
				Vec2 vec2;
				mission.GetPatchSceneEncounterDirection(out vec2);
				foreach (Path path4 in allSpawnPaths)
				{
					if (path4.NumberOfPoints > 1)
					{
						path4.GetPoints(array);
						float num3 = 0f;
						for (int i = 1; i < path4.NumberOfPoints; i++)
						{
							Vec2 asVec2 = array[i - 1].origin.AsVec2;
							Vec2 vec3 = (array[i].origin.AsVec2 - asVec2).Normalized();
							float num4 = vec2.DotProduct(vec3);
							float num5 = 1000f / (1f + asVec2.Distance(asVec));
							num3 += num5 * num4;
						}
						num3 /= (float)(path4.NumberOfPoints - 1);
						bool flag2 = false;
						if (num3 < 0f)
						{
							num3 = -num3;
							flag2 = true;
						}
						if (num3 >= num2)
						{
							path3 = path4;
							num2 = num3;
							flag = flag2;
						}
					}
				}
				if (path3 != null)
				{
					path3.GetPoints(array);
					float num6 = array[0].origin.AsVec2.DistanceSquared(asVec);
					float num7 = 0f;
					float num8 = 0f;
					for (int j = 1; j < path3.NumberOfPoints; j++)
					{
						float num9 = array[j].origin.AsVec2.DistanceSquared(asVec);
						num8 += path3.GetArcLength(j - 1);
						if (num9 < num6)
						{
							num6 = num9;
							num7 = num8;
						}
					}
					path2 = path3;
					centerRatio = num7 / path2.GetTotalLength();
					isInverted = flag;
				}
			}
			else
			{
				Path randomElement = allSpawnPaths.GetRandomElement<Path>();
				if (randomElement.NumberOfPoints > 1)
				{
					path2 = randomElement;
					centerRatio = 0.37f + MBRandom.RandomFloat * 0.26f;
					isInverted = false;
				}
			}
			return path2;
		}

		// Token: 0x0400090A RID: 2314
		private readonly Mission _mission;

		// Token: 0x0400090B RID: 2315
		private Path _initialPath;

		// Token: 0x0400090C RID: 2316
		private BattleSideSpawnPathSelector[] _battleSideSelectors;
	}
}
