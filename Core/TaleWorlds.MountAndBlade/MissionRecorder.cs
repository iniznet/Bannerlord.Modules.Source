using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000284 RID: 644
	public class MissionRecorder
	{
		// Token: 0x06002230 RID: 8752 RVA: 0x0007CFC0 File Offset: 0x0007B1C0
		public MissionRecorder(Mission mission)
		{
			this._mission = mission;
		}

		// Token: 0x06002231 RID: 8753 RVA: 0x0007CFCF File Offset: 0x0007B1CF
		public void RestartRecord()
		{
			MBAPI.IMBMission.RestartRecord(this._mission.Pointer);
		}

		// Token: 0x06002232 RID: 8754 RVA: 0x0007CFE6 File Offset: 0x0007B1E6
		public void ProcessRecordUntilTime(float time)
		{
			MBAPI.IMBMission.ProcessRecordUntilTime(this._mission.Pointer, time);
		}

		// Token: 0x06002233 RID: 8755 RVA: 0x0007CFFE File Offset: 0x0007B1FE
		public bool IsEndOfRecord()
		{
			return MBAPI.IMBMission.EndOfRecord(this._mission.Pointer);
		}

		// Token: 0x06002234 RID: 8756 RVA: 0x0007D015 File Offset: 0x0007B215
		public void StartRecording()
		{
			MBAPI.IMBMission.StartRecording();
		}

		// Token: 0x06002235 RID: 8757 RVA: 0x0007D021 File Offset: 0x0007B221
		public void RecordCurrentState()
		{
			MBAPI.IMBMission.RecordCurrentState(this._mission.Pointer);
		}

		// Token: 0x06002236 RID: 8758 RVA: 0x0007D038 File Offset: 0x0007B238
		public void BackupRecordToFile(string fileName, string gameType, string sceneLevels)
		{
			MBAPI.IMBMission.BackupRecordToFile(this._mission.Pointer, fileName, gameType, sceneLevels);
		}

		// Token: 0x06002237 RID: 8759 RVA: 0x0007D052 File Offset: 0x0007B252
		public void RestoreRecordFromFile(string fileName)
		{
			MBAPI.IMBMission.RestoreRecordFromFile(this._mission.Pointer, fileName);
		}

		// Token: 0x06002238 RID: 8760 RVA: 0x0007D06A File Offset: 0x0007B26A
		public void ClearRecordBuffers()
		{
			MBAPI.IMBMission.ClearRecordBuffers(this._mission.Pointer);
		}

		// Token: 0x06002239 RID: 8761 RVA: 0x0007D081 File Offset: 0x0007B281
		public static string GetSceneNameForReplay(PlatformFilePath fileName)
		{
			return MBAPI.IMBMission.GetSceneNameForReplay(fileName);
		}

		// Token: 0x0600223A RID: 8762 RVA: 0x0007D08E File Offset: 0x0007B28E
		public static string GetGameTypeForReplay(PlatformFilePath fileName)
		{
			return MBAPI.IMBMission.GetGameTypeForReplay(fileName);
		}

		// Token: 0x0600223B RID: 8763 RVA: 0x0007D09B File Offset: 0x0007B29B
		public static string GetSceneLevelsForReplay(PlatformFilePath fileName)
		{
			return MBAPI.IMBMission.GetSceneLevelsForReplay(fileName);
		}

		// Token: 0x0600223C RID: 8764 RVA: 0x0007D0A8 File Offset: 0x0007B2A8
		public static string GetAtmosphereNameForReplay(PlatformFilePath fileName)
		{
			return MBAPI.IMBMission.GetAtmosphereNameForReplay(fileName);
		}

		// Token: 0x0600223D RID: 8765 RVA: 0x0007D0B5 File Offset: 0x0007B2B5
		public static int GetAtmosphereSeasonForReplay(PlatformFilePath fileName)
		{
			return MBAPI.IMBMission.GetAtmosphereSeasonForReplay(fileName);
		}

		// Token: 0x04000CCF RID: 3279
		private readonly Mission _mission;
	}
}
