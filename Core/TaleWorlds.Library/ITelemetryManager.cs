using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000043 RID: 67
	public interface ITelemetryManager
	{
		// Token: 0x06000216 RID: 534
		uint GetTelemetryLevelMask();

		// Token: 0x06000217 RID: 535
		void StartTelemetryConnection(bool showErrors);

		// Token: 0x06000218 RID: 536
		void StopTelemetryConnection();

		// Token: 0x06000219 RID: 537
		void BeginTelemetryScopeInternal(TelemetryLevelMask levelMask, string scopeName);

		// Token: 0x0600021A RID: 538
		void BeginTelemetryScopeBaseLevelInternal(TelemetryLevelMask levelMask, string scopeName);

		// Token: 0x0600021B RID: 539
		void EndTelemetryScopeInternal();

		// Token: 0x0600021C RID: 540
		void EndTelemetryScopeBaseLevelInternal();
	}
}
