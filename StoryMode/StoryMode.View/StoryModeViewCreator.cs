using System;
using StoryMode.View.Missions;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace StoryMode.View
{
	// Token: 0x02000002 RID: 2
	public static class StoryModeViewCreator
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public static MissionView CreateTrainingFieldObjectiveView(Mission mission)
		{
			return ViewCreatorManager.CreateMissionView<MissionTrainingFieldObjectiveView>(true, mission, Array.Empty<object>());
		}
	}
}
