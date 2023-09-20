using System;
using Galaxy.Api;
using TaleWorlds.Library;

namespace TaleWorlds.PlatformService.GOG
{
	// Token: 0x0200000C RID: 12
	public class GogServicesConnectionStateListener : GlobalGogServicesConnectionStateListener
	{
		// Token: 0x06000078 RID: 120 RVA: 0x0000311D File Offset: 0x0000131D
		public override void OnConnectionStateChange(GogServicesConnectionState connected)
		{
			Debug.Print("Connection state to GOG services changed to " + connected, 0, Debug.DebugColor.White, 17592186044416UL);
		}
	}
}
