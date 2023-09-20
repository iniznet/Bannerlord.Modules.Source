using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x02000047 RID: 71
	[OverrideView(typeof(MissionMultiplayerServerStatusUIHandler))]
	public class MissionGauntletServerStatus : MissionView
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000349 RID: 841 RVA: 0x00012AAB File Offset: 0x00010CAB
		private bool IsOptionEnabled
		{
			get
			{
				return BannerlordConfig.EnableNetworkAlertIcons;
			}
		}

		// Token: 0x0600034A RID: 842 RVA: 0x00012AB4 File Offset: 0x00010CB4
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new MultiplayerMissionServerStatusVM();
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerServerStatus", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			NetworkCommunicator.OnPeerAveragePingUpdated += this.OnPeerPingUpdated;
		}

		// Token: 0x0600034B RID: 843 RVA: 0x00012B22 File Offset: 0x00010D22
		private void OnPeerPingUpdated(NetworkCommunicator obj)
		{
			if (this.IsOptionEnabled && obj.IsMine)
			{
				this._dataSource.UpdatePeerPing(obj.AveragePingInMilliseconds);
			}
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00012B48 File Offset: 0x00010D48
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this.IsOptionEnabled && GameNetwork.IsClient && GameNetwork.IsMyPeerReady)
			{
				this._dataSource.UpdatePacketLossRatio((GameNetwork.MyPeer != null) ? ((float)GameNetwork.MyPeer.AverageLossPercent) : 0f);
				MultiplayerMissionServerStatusVM dataSource = this._dataSource;
				NetworkCommunicator myPeer = GameNetwork.MyPeer;
				dataSource.UpdateServerPerformanceState((myPeer != null) ? myPeer.ServerPerformanceProblemState : 0);
				return;
			}
			this._dataSource.ResetStates();
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00012BBE File Offset: 0x00010DBE
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			NetworkCommunicator.OnPeerAveragePingUpdated -= this.OnPeerPingUpdated;
		}

		// Token: 0x040001C6 RID: 454
		private MultiplayerMissionServerStatusVM _dataSource;

		// Token: 0x040001C7 RID: 455
		private GauntletLayer _gauntletLayer;
	}
}
