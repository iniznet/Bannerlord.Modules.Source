using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x02000014 RID: 20
	[OverrideView(typeof(MissionMultiplayerVoiceChatUIHandler))]
	public class MissionGauntletVoiceChat : MissionView
	{
		// Token: 0x0600009C RID: 156 RVA: 0x00005417 File Offset: 0x00003617
		public MissionGauntletVoiceChat()
		{
			this.ViewOrderPriority = 60;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005428 File Offset: 0x00003628
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new MultiplayerVoiceChatVM(base.Mission);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerVoiceChat", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x0000548B File Offset: 0x0000368B
		public override void OnMissionScreenFinalize()
		{
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer = null;
			base.OnMissionScreenFinalize();
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000054BD File Offset: 0x000036BD
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.OnTick(dt);
		}

		// Token: 0x04000065 RID: 101
		private MultiplayerVoiceChatVM _dataSource;

		// Token: 0x04000066 RID: 102
		private GauntletLayer _gauntletLayer;
	}
}
