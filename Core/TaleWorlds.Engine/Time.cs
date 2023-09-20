using System;

namespace TaleWorlds.Engine
{
	public static class Time
	{
		public static float ApplicationTime
		{
			get
			{
				return EngineApplicationInterface.ITime.GetApplicationTime();
			}
		}
	}
}
