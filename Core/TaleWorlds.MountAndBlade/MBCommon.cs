using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001AC RID: 428
	public class MBCommon
	{
		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x060018F2 RID: 6386 RVA: 0x0005AA09 File Offset: 0x00058C09
		// (set) Token: 0x060018F3 RID: 6387 RVA: 0x0005AA10 File Offset: 0x00058C10
		public static MBCommon.GameType CurrentGameType
		{
			get
			{
				return MBCommon._currentGameType;
			}
			set
			{
				MBCommon._currentGameType = value;
				MBAPI.IMBWorld.SetGameType((int)value);
			}
		}

		// Token: 0x060018F4 RID: 6388 RVA: 0x0005AA23 File Offset: 0x00058C23
		public static void PauseGameEngine()
		{
			MBCommon.IsPaused = true;
			MBAPI.IMBWorld.PauseGame();
		}

		// Token: 0x060018F5 RID: 6389 RVA: 0x0005AA35 File Offset: 0x00058C35
		public static void UnPauseGameEngine()
		{
			MBCommon.IsPaused = false;
			MBAPI.IMBWorld.UnpauseGame();
		}

		// Token: 0x060018F6 RID: 6390 RVA: 0x0005AA47 File Offset: 0x00058C47
		public static float GetApplicationTime()
		{
			return MBAPI.IMBWorld.GetGlobalTime(MBCommon.TimeType.Application);
		}

		// Token: 0x060018F7 RID: 6391 RVA: 0x0005AA54 File Offset: 0x00058C54
		public static float GetTotalMissionTime()
		{
			return MBAPI.IMBWorld.GetGlobalTime(MBCommon.TimeType.Mission);
		}

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x060018F8 RID: 6392 RVA: 0x0005AA61 File Offset: 0x00058C61
		public static bool IsDebugMode
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060018F9 RID: 6393 RVA: 0x0005AA64 File Offset: 0x00058C64
		public static void FixSkeletons()
		{
			MBAPI.IMBWorld.FixSkeletons();
		}

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x060018FA RID: 6394 RVA: 0x0005AA70 File Offset: 0x00058C70
		// (set) Token: 0x060018FB RID: 6395 RVA: 0x0005AA77 File Offset: 0x00058C77
		public static bool IsPaused { get; private set; }

		// Token: 0x060018FC RID: 6396 RVA: 0x0005AA7F File Offset: 0x00058C7F
		public static void CheckResourceModifications()
		{
			MBAPI.IMBWorld.CheckResourceModifications();
		}

		// Token: 0x060018FD RID: 6397 RVA: 0x0005AA8C File Offset: 0x00058C8C
		public static int Hash(int i, object o)
		{
			return ((i * 397) ^ o.GetHashCode()).ToString().GetHashCode();
		}

		// Token: 0x040007B5 RID: 1973
		private static MBCommon.GameType _currentGameType;

		// Token: 0x02000514 RID: 1300
		public enum GameType
		{
			// Token: 0x04001BB7 RID: 7095
			Single,
			// Token: 0x04001BB8 RID: 7096
			MultiClient,
			// Token: 0x04001BB9 RID: 7097
			MultiServer,
			// Token: 0x04001BBA RID: 7098
			MultiClientServer,
			// Token: 0x04001BBB RID: 7099
			SingleReplay,
			// Token: 0x04001BBC RID: 7100
			SingleRecord
		}

		// Token: 0x02000515 RID: 1301
		public enum TimeType
		{
			// Token: 0x04001BBE RID: 7102
			Application,
			// Token: 0x04001BBF RID: 7103
			Mission
		}
	}
}
