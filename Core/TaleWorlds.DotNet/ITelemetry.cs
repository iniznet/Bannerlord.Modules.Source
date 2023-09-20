using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000016 RID: 22
	[LibraryInterfaceBase]
	internal interface ITelemetry
	{
		// Token: 0x0600004F RID: 79
		[EngineMethod("get_telemetry_level_mask", false)]
		uint GetTelemetryLevelMask();

		// Token: 0x06000050 RID: 80
		[EngineMethod("start_telemetry_connection", false)]
		void StartTelemetryConnection(bool showErrors);

		// Token: 0x06000051 RID: 81
		[EngineMethod("stop_telemetry_connection", false)]
		void StopTelemetryConnection();

		// Token: 0x06000052 RID: 82
		[EngineMethod("begin_telemetry_scope", false)]
		void BeginTelemetryScope(TelemetryLevelMask levelMask, string scopeName);

		// Token: 0x06000053 RID: 83
		[EngineMethod("end_telemetry_scope", false)]
		void EndTelemetryScope();

		// Token: 0x06000054 RID: 84
		[EngineMethod("has_telemetry_connection", false)]
		bool HasTelemetryConnection();
	}
}
