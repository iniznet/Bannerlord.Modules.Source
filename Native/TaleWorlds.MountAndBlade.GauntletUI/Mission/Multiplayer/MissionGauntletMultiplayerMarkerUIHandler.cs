using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FlagMarker;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x02000043 RID: 67
	[OverrideView(typeof(MissionMultiplayerMarkerUIHandler))]
	public class MissionGauntletMultiplayerMarkerUIHandler : MissionView
	{
		// Token: 0x06000312 RID: 786 RVA: 0x000111D8 File Offset: 0x0000F3D8
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new MultiplayerMissionMarkerVM(base.MissionScreen.CombatCamera);
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MPMissionMarkers", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0001123B File Offset: 0x0000F43B
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0001126D File Offset: 0x0000F46D
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.Input.IsGameKeyDown(5))
			{
				this._dataSource.IsEnabled = true;
			}
			else
			{
				this._dataSource.IsEnabled = false;
			}
			this._dataSource.Tick(dt);
		}

		// Token: 0x040001A1 RID: 417
		private GauntletLayer _gauntletLayer;

		// Token: 0x040001A2 RID: 418
		private MultiplayerMissionMarkerVM _dataSource;
	}
}
