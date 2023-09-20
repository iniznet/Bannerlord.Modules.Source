using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	[OverrideView(typeof(MissionMultiplayerServerStatusUIHandler))]
	public class MissionGauntletServerStatus : MissionView
	{
		private bool IsOptionEnabled
		{
			get
			{
				return BannerlordConfig.EnableNetworkAlertIcons;
			}
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new MultiplayerMissionServerStatusVM();
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerServerStatus", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			NetworkCommunicator.OnPeerAveragePingUpdated += this.OnPeerPingUpdated;
		}

		private void OnPeerPingUpdated(NetworkCommunicator obj)
		{
			if (this.IsOptionEnabled && obj.IsMine)
			{
				this._dataSource.UpdatePeerPing(obj.AveragePingInMilliseconds);
			}
		}

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

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			NetworkCommunicator.OnPeerAveragePingUpdated -= this.OnPeerPingUpdated;
		}

		private MultiplayerMissionServerStatusVM _dataSource;

		private GauntletLayer _gauntletLayer;
	}
}
