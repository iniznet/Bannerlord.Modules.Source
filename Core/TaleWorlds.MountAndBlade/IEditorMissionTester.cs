using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002D2 RID: 722
	public interface IEditorMissionTester
	{
		// Token: 0x06002804 RID: 10244
		void StartMissionForEditor(string missionName, string sceneName, string levels);

		// Token: 0x06002805 RID: 10245
		void StartMissionForReplayEditor(string missionName, string sceneName, string levels, string fileName, bool record, float startTime, float endTime);
	}
}
