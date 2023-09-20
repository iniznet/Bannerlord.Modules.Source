using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class BattleSideSpawnPathSelector
	{
		public SpawnPathData InitialSpawnPath
		{
			get
			{
				return this._initialSpawnPath;
			}
		}

		public MBReadOnlyList<SpawnPathData> ReinforcementPaths
		{
			get
			{
				return this._reinforcementSpawnPaths;
			}
		}

		public BattleSideSpawnPathSelector(Mission mission, Path initialPath, float initialPathCenterRatio, bool initialPathIsInverted)
		{
			this._mission = mission;
			this._initialSpawnPath = new SpawnPathData(initialPath, SpawnPathOrientation.PathCenter, initialPathCenterRatio, initialPathIsInverted);
			this._reinforcementSpawnPaths = new MBList<SpawnPathData>();
			this.FindReinforcementPaths();
		}

		public bool HasReinforcementPath(Path path)
		{
			return path != null && this._reinforcementSpawnPaths.Exists((SpawnPathData pathData) => pathData.Path.Pointer == path.Pointer);
		}

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

		public const float MaxNeighborCount = 2f;

		private readonly Mission _mission;

		private readonly SpawnPathData _initialSpawnPath;

		private readonly MBList<SpawnPathData> _reinforcementSpawnPaths;
	}
}
