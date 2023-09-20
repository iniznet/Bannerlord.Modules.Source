using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	[OverrideView(typeof(MissionSpectatorControlView))]
	public class MissionGauntletSpectatorControl : MissionView
	{
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

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.MissionScreen.OnSpectateAgentFocusIn -= this._dataSource.OnSpectatedAgentFocusIn;
			base.MissionScreen.OnSpectateAgentFocusOut -= this._dataSource.OnSpectatedAgentFocusOut;
			this._dataSource.OnFinalize();
		}

		private GauntletLayer _gauntletLayer;

		private MissionSpectatorControlVM _dataSource;
	}
}
