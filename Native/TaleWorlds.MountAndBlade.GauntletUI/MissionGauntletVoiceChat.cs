using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	[OverrideView(typeof(MissionMultiplayerVoiceChatUIHandler))]
	public class MissionGauntletVoiceChat : MissionView
	{
		public MissionGauntletVoiceChat()
		{
			this.ViewOrderPriority = 60;
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new MultiplayerVoiceChatVM(base.Mission);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerVoiceChat", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		public override void OnMissionScreenFinalize()
		{
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer = null;
			base.OnMissionScreenFinalize();
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.OnTick(dt);
		}

		private MultiplayerVoiceChatVM _dataSource;

		private GauntletLayer _gauntletLayer;
	}
}
