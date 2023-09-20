using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	// Token: 0x0200002B RID: 43
	public class NativeTelemetryManager : ITelemetryManager
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600010A RID: 266 RVA: 0x000051A2 File Offset: 0x000033A2
		// (set) Token: 0x0600010B RID: 267 RVA: 0x000051A9 File Offset: 0x000033A9
		public static uint TelemetryLevelMask { get; private set; }

		// Token: 0x0600010C RID: 268 RVA: 0x000051B1 File Offset: 0x000033B1
		public uint GetTelemetryLevelMask()
		{
			return NativeTelemetryManager.TelemetryLevelMask;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x000051B8 File Offset: 0x000033B8
		public NativeTelemetryManager()
		{
			NativeTelemetryManager.TelemetryLevelMask = 4096U;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000051CA File Offset: 0x000033CA
		internal void Update()
		{
			NativeTelemetryManager.TelemetryLevelMask = LibraryApplicationInterface.ITelemetry.GetTelemetryLevelMask();
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000051DB File Offset: 0x000033DB
		public void StartTelemetryConnection(bool showErrors)
		{
			LibraryApplicationInterface.ITelemetry.StartTelemetryConnection(showErrors);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000051E8 File Offset: 0x000033E8
		public void StopTelemetryConnection()
		{
			LibraryApplicationInterface.ITelemetry.StopTelemetryConnection();
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000051F4 File Offset: 0x000033F4
		public void BeginTelemetryScopeInternal(TelemetryLevelMask levelMask, string scopeName)
		{
			if ((NativeTelemetryManager.TelemetryLevelMask & (uint)levelMask) != 0U)
			{
				LibraryApplicationInterface.ITelemetry.BeginTelemetryScope(levelMask, scopeName);
			}
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000520B File Offset: 0x0000340B
		public void EndTelemetryScopeInternal()
		{
			LibraryApplicationInterface.ITelemetry.EndTelemetryScope();
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00005217 File Offset: 0x00003417
		public void BeginTelemetryScopeBaseLevelInternal(TelemetryLevelMask levelMask, string scopeName)
		{
			if ((NativeTelemetryManager.TelemetryLevelMask & (uint)levelMask) != 0U)
			{
				LibraryApplicationInterface.ITelemetry.BeginTelemetryScope(levelMask, scopeName);
			}
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000522E File Offset: 0x0000342E
		public void EndTelemetryScopeBaseLevelInternal()
		{
			LibraryApplicationInterface.ITelemetry.EndTelemetryScope();
		}
	}
}
