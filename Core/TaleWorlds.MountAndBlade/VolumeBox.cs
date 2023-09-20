using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class VolumeBox : MissionObject
	{
		protected internal override void OnInit()
		{
		}

		public void AddToCheckList(Agent agent)
		{
		}

		public void RemoveFromCheckList(Agent agent)
		{
		}

		public void SetIsOccupiedDelegate(VolumeBox.VolumeBoxDelegate volumeBoxDelegate)
		{
			this._volumeBoxIsOccupiedDelegate = volumeBoxDelegate;
		}

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

		public bool IsPointIn(Vec3 point)
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			Vec3 scaleVector = globalFrame.rotation.GetScaleVector();
			globalFrame.rotation.ApplyScaleLocal(new Vec3(1f / scaleVector.x, 1f / scaleVector.y, 1f / scaleVector.z, -1f));
			point = globalFrame.TransformToLocal(point);
			return MathF.Abs(point.x) <= scaleVector.x / 2f && MathF.Abs(point.y) <= scaleVector.y / 2f && MathF.Abs(point.z) <= scaleVector.z / 2f;
		}

		private VolumeBox.VolumeBoxDelegate _volumeBoxIsOccupiedDelegate;

		public delegate void VolumeBoxDelegate(VolumeBox volumeBox, List<Agent> agentsInVolume);
	}
}
