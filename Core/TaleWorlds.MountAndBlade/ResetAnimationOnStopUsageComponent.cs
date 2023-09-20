using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200037E RID: 894
	public class ResetAnimationOnStopUsageComponent : UsableMissionObjectComponent
	{
		// Token: 0x06003068 RID: 12392 RVA: 0x000C6C73 File Offset: 0x000C4E73
		public ResetAnimationOnStopUsageComponent(ActionIndexCache successfulResetActionCode)
		{
			this._successfulResetAction = successfulResetActionCode;
		}

		// Token: 0x06003069 RID: 12393 RVA: 0x000C6C90 File Offset: 0x000C4E90
		protected internal override void OnUseStopped(Agent userAgent, bool isSuccessful = true)
		{
			ActionIndexCache actionIndexCache = (isSuccessful ? this._successfulResetAction : ActionIndexCache.act_none);
			if (actionIndexCache == ActionIndexCache.act_none)
			{
				userAgent.SetActionChannel(1, actionIndexCache, false, 72UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
			userAgent.SetActionChannel(0, actionIndexCache, false, 72UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
		}

		// Token: 0x04001429 RID: 5161
		private readonly ActionIndexCache _successfulResetAction = ActionIndexCache.act_none;
	}
}
