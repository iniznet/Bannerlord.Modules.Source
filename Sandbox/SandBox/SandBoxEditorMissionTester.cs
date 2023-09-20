using System;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	internal class SandBoxEditorMissionTester : IEditorMissionTester
	{
		void IEditorMissionTester.StartMissionForEditor(string missionName, string sceneName, string levels)
		{
			MBGameManager.StartNewGame(new EditorSceneMissionManager(missionName, sceneName, levels, false, "", false, 0f, 0f));
		}

		void IEditorMissionTester.StartMissionForReplayEditor(string missionName, string sceneName, string levels, string fileName, bool record, float startTime, float endTime)
		{
			MBGameManager.StartNewGame(new EditorSceneMissionManager(missionName, sceneName, levels, true, fileName, record, startTime, endTime));
		}
	}
}
