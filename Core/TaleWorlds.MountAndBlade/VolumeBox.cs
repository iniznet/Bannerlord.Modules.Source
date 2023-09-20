using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000385 RID: 901
	public class VolumeBox : MissionObject
	{
		// Token: 0x0600318E RID: 12686 RVA: 0x000CD3B9 File Offset: 0x000CB5B9
		protected internal override void OnInit()
		{
		}

		// Token: 0x0600318F RID: 12687 RVA: 0x000CD3BB File Offset: 0x000CB5BB
		public void AddToCheckList(Agent agent)
		{
		}

		// Token: 0x06003190 RID: 12688 RVA: 0x000CD3BD File Offset: 0x000CB5BD
		public void RemoveFromCheckList(Agent agent)
		{
		}

		// Token: 0x06003191 RID: 12689 RVA: 0x000CD3BF File Offset: 0x000CB5BF
		public void SetIsOccupiedDelegate(VolumeBox.VolumeBoxDelegate volumeBoxDelegate)
		{
			this._volumeBoxIsOccupiedDelegate = volumeBoxDelegate;
		}

		// Token: 0x06003192 RID: 12690 RVA: 0x000CD3C8 File Offset: 0x000CB5C8
		public bool HasAgentsInAttackerSide()
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(Mission.Current, globalFrame.origin.AsVec2, globalFrame.rotation.GetScaleVector().AsVec2.Length, false);
			while (proximityMapSearchStruct.LastFoundAgent != null)
			{
				Agent lastFoundAgent = proximityMapSearchStruct.LastFoundAgent;
				if (lastFoundAgent.Team != null && lastFoundAgent.Team.Side == BattleSideEnum.Attacker && this.IsPointIn(lastFoundAgent.Position))
				{
					return true;
				}
				AgentProximityMap.FindNext(Mission.Current, ref proximityMapSearchStruct);
			}
			return false;
		}

		// Token: 0x06003193 RID: 12691 RVA: 0x000CD460 File Offset: 0x000CB660
		public bool IsPointIn(Vec3 point)
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			Vec3 scaleVector = globalFrame.rotation.GetScaleVector();
			globalFrame.rotation.ApplyScaleLocal(new Vec3(1f / scaleVector.x, 1f / scaleVector.y, 1f / scaleVector.z, -1f));
			point = globalFrame.TransformToLocal(point);
			return MathF.Abs(point.x) <= scaleVector.x / 2f && MathF.Abs(point.y) <= scaleVector.y / 2f && MathF.Abs(point.z) <= scaleVector.z / 2f;
		}

		// Token: 0x040014B2 RID: 5298
		private VolumeBox.VolumeBoxDelegate _volumeBoxIsOccupiedDelegate;

		// Token: 0x02000693 RID: 1683
		// (Invoke) Token: 0x06003ED7 RID: 16087
		public delegate void VolumeBoxDelegate(VolumeBox volumeBox, List<Agent> agentsInVolume);
	}
}
