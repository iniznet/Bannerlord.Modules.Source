﻿using System;

namespace TaleWorlds.MountAndBlade.View
{
	public class AgentVisualsCreator : IAgentVisualCreator
	{
		public IAgentVisual Create(AgentVisualsData data, string name, bool needBatchedVersionForWeaponMeshes, bool forceUseFaceCache)
		{
			return AgentVisuals.Create(data, name, false, needBatchedVersionForWeaponMeshes, forceUseFaceCache);
		}
	}
}
