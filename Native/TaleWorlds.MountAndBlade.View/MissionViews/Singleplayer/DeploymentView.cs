using System;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	public class DeploymentView : MissionView
	{
		public override void AfterStart()
		{
			base.AfterStart();
			this._deploymentHandler = base.Mission.GetMissionBehavior<DeploymentHandler>();
			this.CreateWidgets();
		}

		public override void OnRemoveBehavior()
		{
			this.RemoveWidgets();
			base.OnRemoveBehavior();
		}

		protected virtual void CreateWidgets()
		{
		}

		protected virtual void RemoveWidgets()
		{
		}

		private DeploymentHandler _deploymentHandler;
	}
}
