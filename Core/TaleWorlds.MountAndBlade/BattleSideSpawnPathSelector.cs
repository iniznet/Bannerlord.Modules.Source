using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001F5 RID: 501
	public class BattleSideSpawnPathSelector
	{
		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x06001BE4 RID: 7140 RVA: 0x00062E3E File Offset: 0x0006103E
		public SpawnPathData InitialSpawnPath
		{
			get
			{
				return this._initialSpawnPath;
			}
		}

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x06001BE5 RID: 7141 RVA: 0x00062E46 File Offset: 0x00061046
		public MBReadOnlyList<SpawnPathData> ReinforcementPaths
		{
			get
			{
				return this._reinforcementSpawnPaths;
			}
		}

		// Token: 0x06001BE6 RID: 7142 RVA: 0x00062E4E File Offset: 0x0006104E
		public BattleSideSpawnPathSelector(Mission mission, Path initialPath, float initialPathCenterRatio, bool initialPathIsInverted)
		{
			this._mission = mission;
			this._initialSpawnPath = new SpawnPathData(initialPath, SpawnPathOrientation.PathCenter, initialPathCenterRatio, initialPathIsInverted);
			this._reinforcementSpawnPaths = new MBList<SpawnPathData>();
			this.FindReinforcementPaths();
		}

		// Token: 0x06001BE7 RID: 7143 RVA: 0x00062E80 File Offset: 0x00061080
		public bool HasReinforcementPath(Path path)
		{
			return path != null && this._reinforcementSpawnPaths.Exists((SpawnPathData pathData) => pathData.Path.Pointer == path.Pointer);
		}

		// Token: 0x06001BE8 RID: 7144 RVA: 0x00062EC4 File Offset: 0x000610C4
		private void FindReinforcementPaths()
		{
			this._reinforcementSpawnPaths.Clear();
			SpawnPathData spawnPathData = new SpawnPathData(this._initialSpawnPath.Path, SpawnPathOrientation.Local, 0f, this._initialSpawnPath.IsInverted);
			this._reinforcementSpawnPaths.Add(spawnPathData);
			MBList<Path> allSpawnPaths = MBSceneUtilities.GetAllSpawnPaths(this._mission.Scene);
			if (allSpawnPaths.Count == 0)
			{
				return;
			}
			bool flag = false;
			if (allSpawnPaths.Count > 1)
			{
				MatrixFrame[] array = new MatrixFrame[100];
				spawnPathData.Path.GetPoints(array);
				MatrixFrame matrixFrame = (spawnPathData.IsInverted ? array[spawnPathData.Path.NumberOfPoints - 1] : array[0]);
				SortedList<float, SpawnPathData> sortedList = new SortedList<float, SpawnPathData>();
				foreach (Path path in allSpawnPaths)
				{
					if (path.NumberOfPoints > 1 && path.Pointer != spawnPathData.Path.Pointer)
					{
						path.GetPoints(array);
						MatrixFrame matrixFrame2 = array[0];
						MatrixFrame matrixFrame3 = array[path.NumberOfPoints - 1];
						float num = matrixFrame2.origin.DistanceSquared(matrixFrame.origin);
						float num2 = matrixFrame3.origin.DistanceSquared(matrixFrame.origin);
						sortedList.Add(num, new SpawnPathData(path, SpawnPathOrientation.Local, 0f, false));
						sortedList.Add(num2, new SpawnPathData(path, SpawnPathOrientation.Local, 0f, true));
					}
					else
					{
						flag = flag || spawnPathData.Path.Pointer == path.Pointer;
					}
				}
				int num3 = 0;
				using (IEnumerator<KeyValuePair<float, SpawnPathData>> enumerator2 = sortedList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<float, SpawnPathData> keyValuePair = enumerator2.Current;
						this._reinforcementSpawnPaths.Add(keyValuePair.Value);
						num3++;
						if ((float)num3 >= 2f)
						{
							break;
						}
					}
					return;
				}
			}
			flag = spawnPathData.Path.Pointer == allSpawnPaths[0].Pointer;
		}

		// Token: 0x04000905 RID: 2309
		public const float MaxNeighborCount = 2f;

		// Token: 0x04000906 RID: 2310
		private readonly Mission _mission;

		// Token: 0x04000907 RID: 2311
		private readonly SpawnPathData _initialSpawnPath;

		// Token: 0x04000908 RID: 2312
		private readonly MBList<SpawnPathData> _reinforcementSpawnPaths;
	}
}
