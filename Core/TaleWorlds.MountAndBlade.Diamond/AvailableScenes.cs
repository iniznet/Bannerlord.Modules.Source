using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class AvailableScenes
	{
		public static AvailableScenes Empty { get; private set; } = new AvailableScenes(new Dictionary<string, string[]>());

		public Dictionary<string, string[]> ScenesByGameTypes { get; set; }

		public AvailableScenes()
		{
		}

		public AvailableScenes(Dictionary<string, string[]> scenesByGameTypes)
		{
			this.ScenesByGameTypes = scenesByGameTypes;
		}
	}
}
