using System;
using Galaxy.Api;

namespace TaleWorlds.PlatformService.GOG
{
	// Token: 0x0200000B RID: 11
	public class UserInformationRetrieveListener : IUserInformationRetrieveListener
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000073 RID: 115 RVA: 0x000030F2 File Offset: 0x000012F2
		// (set) Token: 0x06000074 RID: 116 RVA: 0x000030FA File Offset: 0x000012FA
		public bool GotResult { get; private set; }

		// Token: 0x06000075 RID: 117 RVA: 0x00003103 File Offset: 0x00001303
		public override void OnUserInformationRetrieveFailure(GalaxyID userID, IUserInformationRetrieveListener.FailureReason failureReason)
		{
			this.GotResult = true;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000310C File Offset: 0x0000130C
		public override void OnUserInformationRetrieveSuccess(GalaxyID userID)
		{
			this.GotResult = true;
		}
	}
}
