using System;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	// Token: 0x02000063 RID: 99
	public class DeploymentView : MissionView
	{
		// Token: 0x06000439 RID: 1081 RVA: 0x00021986 File Offset: 0x0001FB86
		public override void AfterStart()
		{
			base.AfterStart();
			this._deploymentHandler = base.Mission.GetMissionBehavior<DeploymentHandler>();
			this.CreateWidgets();
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x000219A5 File Offset: 0x0001FBA5
		public override void OnRemoveBehavior()
		{
			this.RemoveWidgets();
			base.OnRemoveBehavior();
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x000219B3 File Offset: 0x0001FBB3
		protected virtual void CreateWidgets()
		{
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x000219B5 File Offset: 0x0001FBB5
		protected virtual void RemoveWidgets()
		{
		}

		// Token: 0x040002AC RID: 684
		private DeploymentHandler _deploymentHandler;
	}
}
