using System;
using Galaxy.Api;
using TaleWorlds.Library;

namespace TaleWorlds.PlatformService.GOG
{
	public class AuthenticationListener : GlobalAuthListener
	{
		public bool GotResult { get; private set; }

		public AuthenticationListener(GOGPlatformServices gogPlatformServices)
		{
			this._gogPlatformServices = gogPlatformServices;
		}

		public override void OnAuthSuccess()
		{
			Debug.Print("Successfully signed in", 0, Debug.DebugColor.White, 17592186044416UL);
			GalaxyInstance.User().GetGalaxyID();
			this.GotResult = true;
		}

		public override void OnAuthFailure(IAuthListener.FailureReason failureReason)
		{
			Debug.Print("Failed to sign in for reason " + failureReason, 0, Debug.DebugColor.White, 17592186044416UL);
			this.GotResult = true;
		}

		public override void OnAuthLost()
		{
			Debug.Print("Authorization lost", 0, Debug.DebugColor.White, 17592186044416UL);
			this.GotResult = true;
		}

		private GOGPlatformServices _gogPlatformServices;
	}
}
