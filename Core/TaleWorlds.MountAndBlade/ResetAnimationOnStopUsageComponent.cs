using System;

namespace TaleWorlds.MountAndBlade
{
	public class ResetAnimationOnStopUsageComponent : UsableMissionObjectComponent
	{
		public ResetAnimationOnStopUsageComponent(ActionIndexCache successfulResetActionCode)
		{
			this._successfulResetAction = successfulResetActionCode;
		}

		protected internal override void OnUseStopped(Agent userAgent, bool isSuccessful = true)
		{
			ActionIndexCache actionIndexCache = (isSuccessful ? this._successfulResetAction : ActionIndexCache.act_none);
			if (actionIndexCache == ActionIndexCache.act_none)
			{
				userAgent.SetActionChannel(1, actionIndexCache, false, 72UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
			userAgent.SetActionChannel(0, actionIndexCache, false, 72UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
		}

		private readonly ActionIndexCache _successfulResetAction = ActionIndexCache.act_none;
	}
}
