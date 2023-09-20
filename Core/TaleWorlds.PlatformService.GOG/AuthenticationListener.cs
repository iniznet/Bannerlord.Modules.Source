using System;
using Galaxy.Api;
using TaleWorlds.Library;

namespace TaleWorlds.PlatformService.GOG
{
	// Token: 0x0200000A RID: 10
	public class AuthenticationListener : GlobalAuthListener
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600006D RID: 109 RVA: 0x0000305F File Offset: 0x0000125F
		// (set) Token: 0x0600006E RID: 110 RVA: 0x00003067 File Offset: 0x00001267
		public bool GotResult { get; private set; }

		// Token: 0x0600006F RID: 111 RVA: 0x00003070 File Offset: 0x00001270
		public AuthenticationListener(GOGPlatformServices gogPlatformServices)
		{
			this._gogPlatformServices = gogPlatformServices;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000307F File Offset: 0x0000127F
		public override void OnAuthSuccess()
		{
			Debug.Print("Successfully signed in", 0, Debug.DebugColor.White, 17592186044416UL);
			GalaxyInstance.User().GetGalaxyID();
			this.GotResult = true;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000030A9 File Offset: 0x000012A9
		public override void OnAuthFailure(IAuthListener.FailureReason failureReason)
		{
			Debug.Print("Failed to sign in for reason " + failureReason, 0, Debug.DebugColor.White, 17592186044416UL);
			this.GotResult = true;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000030D3 File Offset: 0x000012D3
		public override void OnAuthLost()
		{
			Debug.Print("Authorization lost", 0, Debug.DebugColor.White, 17592186044416UL);
			this.GotResult = true;
		}

		// Token: 0x0400001F RID: 31
		private GOGPlatformServices _gogPlatformServices;
	}
}
