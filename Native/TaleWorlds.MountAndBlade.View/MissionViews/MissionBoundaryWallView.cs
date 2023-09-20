using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	public class MissionBoundaryWallView : MissionView
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			foreach (ICollection<Vec2> collection in base.Mission.Boundaries.Values)
			{
				this.CreateBoundaryEntity(collection);
			}
		}

		private void CreateBoundaryEntity(ICollection<Vec2> boundaryPoints)
		{
			Mesh mesh = BoundaryWallView.CreateBoundaryMesh(base.Mission.Scene, boundaryPoints, 536918784U);
			if (mesh != null)
			{
				GameEntity gameEntity = GameEntity.CreateEmpty(base.Mission.Scene, true);
				gameEntity.AddMesh(mesh, true);
				MatrixFrame identity = MatrixFrame.Identity;
				gameEntity.SetGlobalFrame(ref identity);
				gameEntity.Name = "boundary_wall";
				gameEntity.SetMobility(0);
				gameEntity.EntityFlags |= 1073741824;
			}
		}
	}
}
