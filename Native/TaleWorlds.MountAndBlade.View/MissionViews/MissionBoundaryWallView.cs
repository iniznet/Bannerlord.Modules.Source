using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	// Token: 0x02000046 RID: 70
	public class MissionBoundaryWallView : MissionView
	{
		// Token: 0x0600031F RID: 799 RVA: 0x0001B4C0 File Offset: 0x000196C0
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			foreach (ICollection<Vec2> collection in base.Mission.Boundaries.Values)
			{
				this.CreateBoundaryEntity(collection);
			}
		}

		// Token: 0x06000320 RID: 800 RVA: 0x0001B520 File Offset: 0x00019720
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
