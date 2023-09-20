using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	public class NativeTelemetryManager : ITelemetryManager
	{
		public static uint TelemetryLevelMask { get; private set; }

		public uint GetTelemetryLevelMask()
		{
			return NativeTelemetryManager.TelemetryLevelMask;
		}

		public NativeTelemetryManager()
		{
			NativeTelemetryManager.TelemetryLevelMask = 4096U;
		}

		internal void Update()
		{
			NativeTelemetryManager.TelemetryLevelMask = LibraryApplicationInterface.ITelemetry.GetTelemetryLevelMask();
		}

		public void StartTelemetryConnection(bool showErrors)
		{
			LibraryApplicationInterface.ITelemetry.StartTelemetryConnection(showErrors);
		}

		public void StopTelemetryConnection()
		{
			LibraryApplicationInterface.ITelemetry.StopTelemetryConnection();
		}

		public void BeginTelemetryScopeInternal(TelemetryLevelMask levelMask, string scopeName)
		{
			if ((NativeTelemetryManager.TelemetryLevelMask & (uint)levelMask) != 0U)
			{
				LibraryApplicationInterface.ITelemetry.BeginTelemetryScope(levelMask, scopeName);
			}
		}

		public void EndTelemetryScopeInternal()
		{
			LibraryApplicationInterface.ITelemetry.EndTelemetryScope();
		}

		public void BeginTelemetryScopeBaseLevelInternal(TelemetryLevelMask levelMask, string scopeName)
		{
			if ((NativeTelemetryManager.TelemetryLevelMask & (uint)levelMask) != 0U)
			{
				LibraryApplicationInterface.ITelemetry.BeginTelemetryScope(levelMask, scopeName);
			}
		}

		public void EndTelemetryScopeBaseLevelInternal()
		{
			LibraryApplicationInterface.ITelemetry.EndTelemetryScope();
		}
	}
}
