using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200023E RID: 574
	internal class OnSessionInvitationAcceptedJob : Job
	{
		// Token: 0x06001F5C RID: 8028 RVA: 0x0006F208 File Offset: 0x0006D408
		public OnSessionInvitationAcceptedJob(SessionInvitationType sessionInvitationType)
		{
			this._sessionInvitationType = sessionInvitationType;
		}

		// Token: 0x06001F5D RID: 8029 RVA: 0x0006F218 File Offset: 0x0006D418
		public override void DoJob(float dt)
		{
			base.DoJob(dt);
			if (MBGameManager.Current != null)
			{
				MBGameManager.Current.OnSessionInvitationAccepted(this._sessionInvitationType);
			}
			else if (GameStateManager.Current != null && GameStateManager.Current.ActiveState != null)
			{
				GameStateManager.Current.CleanStates(0);
			}
			base.Finished = true;
		}

		// Token: 0x04000B75 RID: 2933
		private readonly SessionInvitationType _sessionInvitationType;
	}
}
