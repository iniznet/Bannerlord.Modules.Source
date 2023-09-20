using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000271 RID: 625
	public class MissionBoundaryPlacer : MissionLogic
	{
		// Token: 0x06002176 RID: 8566 RVA: 0x00079F88 File Offset: 0x00078188
		public override void EarlyStart()
		{
			base.EarlyStart();
			this.AddBoundaries();
		}

		// Token: 0x06002177 RID: 8567 RVA: 0x00079F98 File Offset: 0x00078198
		public void AddBoundaries()
		{
			string text;
			List<Vec2> sceneBoundaryPoints = MBSceneUtilities.GetSceneBoundaryPoints(base.Mission.Scene, out text);
			base.Mission.Boundaries.Add(text, sceneBoundaryPoints);
		}

		// Token: 0x04000C62 RID: 3170
		public const string DefaultWalkAreaBoundaryName = "walk_area";
	}
}
