using System;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	// Token: 0x02000011 RID: 17
	internal class SandBoxEditorMissionTester : IEditorMissionTester
	{
		// Token: 0x060000C3 RID: 195 RVA: 0x0000646A File Offset: 0x0000466A
		void IEditorMissionTester.StartMissionForEditor(string missionName, string sceneName, string levels)
		{
			MBGameManager.StartNewGame(new EditorSceneMissionManager(missionName, sceneName, levels, false, "", false, 0f, 0f));
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000648A File Offset: 0x0000468A
		void IEditorMissionTester.StartMissionForReplayEditor(string missionName, string sceneName, string levels, string fileName, bool record, float startTime, float endTime)
		{
			MBGameManager.StartNewGame(new EditorSceneMissionManager(missionName, sceneName, levels, true, fileName, record, startTime, endTime));
		}
	}
}
