using System;
using Galaxy.Api;
using TaleWorlds.Library;

namespace TaleWorlds.PlatformService.GOG
{
	public class GogServicesConnectionStateListener : GlobalGogServicesConnectionStateListener
	{
		public override void OnConnectionStateChange(GogServicesConnectionState connected)
		{
			Debug.Print("Connection state to GOG services changed to " + connected, 0, Debug.DebugColor.White, 17592186044416UL);
		}
	}
}
