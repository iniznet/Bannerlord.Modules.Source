using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	internal class OnSessionInvitationAcceptedJob : Job
	{
		public OnSessionInvitationAcceptedJob(SessionInvitationType sessionInvitationType)
		{
			this._sessionInvitationType = sessionInvitationType;
		}

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

		private readonly SessionInvitationType _sessionInvitationType;
	}
}
