using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	public class MBCommon
	{
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

		public static void PauseGameEngine()
		{
			MBCommon.IsPaused = true;
			MBAPI.IMBWorld.PauseGame();
		}

		public static void UnPauseGameEngine()
		{
			MBCommon.IsPaused = false;
			MBAPI.IMBWorld.UnpauseGame();
		}

		public static float GetApplicationTime()
		{
			return MBAPI.IMBWorld.GetGlobalTime(MBCommon.TimeType.Application);
		}

		public static float GetTotalMissionTime()
		{
			return MBAPI.IMBWorld.GetGlobalTime(MBCommon.TimeType.Mission);
		}

		public static bool IsDebugMode
		{
			get
			{
				return false;
			}
		}

		public static void FixSkeletons()
		{
			MBAPI.IMBWorld.FixSkeletons();
		}

		public static bool IsPaused { get; private set; }

		public static void CheckResourceModifications()
		{
			MBAPI.IMBWorld.CheckResourceModifications();
		}

		public static int Hash(int i, object o)
		{
			return ((i * 397) ^ o.GetHashCode()).ToString().GetHashCode();
		}

		private static MBCommon.GameType _currentGameType;

		public enum GameType
		{
			Single,
			MultiClient,
			MultiServer,
			MultiClientServer,
			SingleReplay,
			SingleRecord
		}

		[EngineStruct("rglTimer_type", false)]
		public enum TimeType
		{
			Application,
			Mission
		}
	}
}
