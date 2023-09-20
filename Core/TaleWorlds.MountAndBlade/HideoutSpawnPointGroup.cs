using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class HideoutSpawnPointGroup : SynchedMissionObject
	{
		protected internal override void OnInit()
		{
			base.OnInit();
			this._spawnPoints = new GameEntity[4];
			string spawnPointTagAffix = this.Side.ToString().ToLower() + "_";
			string[] array = new string[4];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = spawnPointTagAffix + ((FormationClass)i).GetName().ToLower();
			}
			IEnumerable<GameEntity> children = base.GameEntity.GetChildren();
			Func<GameEntity, bool> <>9__0;
			Func<GameEntity, bool> func;
			if ((func = <>9__0) == null)
			{
				Func<string, bool> <>9__1;
				func = (<>9__0 = delegate(GameEntity ce)
				{
					IEnumerable<string> tags = ce.Tags;
					Func<string, bool> func2;
					if ((func2 = <>9__1) == null)
					{
						func2 = (<>9__1 = (string t) => t.StartsWith(spawnPointTagAffix));
					}
					return tags.Any(func2);
				});
			}
			foreach (GameEntity gameEntity in children.Where(func))
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (gameEntity.HasTag(array[j]))
					{
						this._spawnPoints[j] = gameEntity;
						break;
					}
				}
			}
		}

		public MatrixFrame[] GetSpawnPointFrames()
		{
			MatrixFrame[] array = new MatrixFrame[this._spawnPoints.Length];
			for (int i = 0; i < this._spawnPoints.Length; i++)
			{
				array[i] = ((this._spawnPoints[i] != null) ? this._spawnPoints[i].GetGlobalFrame() : MatrixFrame.Identity);
			}
			return array;
		}

		public void RemoveWithAllChildren()
		{
			base.GameEntity.RemoveAllChildren();
			base.GameEntity.Remove(83);
		}

		private const int NumberOfDefaultFormations = 4;

		public BattleSideEnum Side;

		public int PhaseNumber;

		private GameEntity[] _spawnPoints;
	}
}
