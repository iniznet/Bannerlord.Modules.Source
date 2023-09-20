using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	// Token: 0x02000037 RID: 55
	[OverrideView(typeof(MissionSpectatorControlView))]
	public class MissionGauntletSpectatorControl : MissionView
	{
		// Token: 0x060002B3 RID: 691 RVA: 0x0000F55C File Offset: 0x0000D75C
		public override void EarlyStart()
		{
			base.EarlyStart();
			this.ViewOrderPriority = 14;
			this._dataSource = new MissionSpectatorControlVM(base.Mission);
			this._dataSource.SetPrevCharacterInputKey(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(10));
			this._dataSource.SetNextCharacterInputKey(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(9));
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("SpectatorControl", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			base.MissionScreen.OnSpectateAgentFocusIn += this._dataSource.OnSpectatedAgentFocusIn;
			base.MissionScreen.OnSpectateAgentFocusOut += this._dataSource.OnSpectatedAgentFocusOut;
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x0000F638 File Offset: 0x0000D838
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._dataSource != null)
			{
				Mission.SpectatorData spectatingData = base.MissionScreen.GetSpectatingData(base.MissionScreen.CombatCamera.Frame.origin);
				bool flag = spectatingData.CameraType == 1 || spectatingData.CameraType == 7;
				MissionSpectatorControlVM dataSource = this._dataSource;
				bool flag2;
				if ((!flag && base.Mission.Mode != 6) || (base.MissionScreen.IsCheatGhostMode && !base.Mission.IsOrderMenuOpen))
				{
					MissionMultiplayerGameModeBaseClient missionBehavior = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
					if ((missionBehavior == null || missionBehavior.IsRoundInProgress) && !base.MissionScreen.LockCameraMovement)
					{
						flag2 = base.MissionScreen.CustomCamera == null;
						goto IL_B6;
					}
				}
				flag2 = false;
				IL_B6:
				dataSource.IsEnabled = flag2;
				bool flag3 = base.Mission.PlayerTeam != null && base.Mission.MainAgent == null;
				this._dataSource.SetMainAgentStatus(flag3);
			}
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0000F72C File Offset: 0x0000D92C
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.MissionScreen.OnSpectateAgentFocusIn -= this._dataSource.OnSpectatedAgentFocusIn;
			base.MissionScreen.OnSpectateAgentFocusOut -= this._dataSource.OnSpectatedAgentFocusOut;
			this._dataSource.OnFinalize();
		}

		// Token: 0x04000162 RID: 354
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000163 RID: 355
		private MissionSpectatorControlVM _dataSource;
	}
}
