using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class MissionBoundaryPlacer : MissionLogic
	{
		public override void EarlyStart()
		{
			base.EarlyStart();
			this.AddBoundaries();
		}

		public void AddBoundaries()
		{
			string text;
			List<Vec2> sceneBoundaryPoints = MBSceneUtilities.GetSceneBoundaryPoints(base.Mission.Scene, out text);
			base.Mission.Boundaries.Add(text, sceneBoundaryPoints);
		}

		public const string DefaultWalkAreaBoundaryName = "walk_area";
	}
}
